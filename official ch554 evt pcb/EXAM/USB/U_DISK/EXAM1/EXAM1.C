
/********************************** (C) COPYRIGHT *******************************
* File Name          :EXAM1.C
* Author             : WCH
* Version            : V1.0
* Date               : 2017/01/20
* Description        : 
 CH554 C语言的U盘文件字节读写示例程序，文件指针偏移，修改文件属性，删除文件等操作
 支持: FAT12/FAT16/FAT32  
*******************************************************************************/

#include "..\..\..\Public\CH554.H"                                                      
#include <stdio.h>
#include <string.h>

#define DISK_BASE_BUF_LEN		512	/* 默认的磁盘数据缓冲区大小为512字节(可以选择为2048甚至4096以支持某些大扇区的U盘),为0则禁止在本文件中定义缓冲区并由应用程序在pDISK_BASE_BUF中指定 */
#define FOR_ROOT_UDISK_ONLY		 1// 只用于DP/DM端口的U盘文件操作(使用子程序库CH554UFI/X),不支持HUB下U盘操作
//还需要添加LIB库文件

//#define NO_DEFAULT_ACCESS_SECTOR	1		/* 禁止默认的磁盘扇区读写子程序,下面用自行编写的程序代替它 */
//#define NO_DEFAULT_DISK_CONNECT		1		/* 禁止默认的检查磁盘连接子程序,下面用自行编写的程序代替它 */
//#define NO_DEFAULT_FILE_ENUMER		1		/* 禁止默认的文件名枚举回调程序,下面用自行编写的程序代替它 */

#include "..\..\USB_LIB\CH554UFI.H"
#include "..\..\USB_LIB\CH554UFI.C"
#include "..\..\..\Public\DEBUG.H"
#include "..\..\USB_LIB\USBHOST.H"

// 各子程序返回状态码
#define	ERR_SUCCESS			  0x00	// 操作成功
#define	ERR_USB_CONNECT		0x15	/* 检测到USB设备连接事件,已经连接 */
#define	ERR_USB_DISCON		0x16	/* 检测到USB设备断开事件,已经断开 */
#define	ERR_USB_BUF_OVER	0x17	/* USB传输的数据有误或者数据太多缓冲区溢出 */
#define	ERR_USB_DISK_ERR	0x1F	/* USB存储器操作失败,在初始化时可能是USB存储器不支持,在读写操作中可能是磁盘损坏或者已经断开 */
#define	ERR_USB_TRANSFER	0x20	/* NAK/STALL等更多错误码在0x20~0x2F */
#define	ERR_USB_UNSUPPORT	0xFB
#define	ERR_USB_UNKNOWN		0xFE

#define	WAIT_USB_TOUT_200US		200  // 等待USB中断超时时间200uS@Fsys=12MHz
#define	SetUsbSpeed( x )

// 获取设备描述符
UINT8C	SetupGetDevDescr[] = { USB_REQ_TYP_IN, USB_GET_DESCRIPTOR, 0x00, USB_DESCR_TYP_DEVICE, 0x00, 0x00, sizeof( USB_DEV_DESCR ), 0x00 };
// 获取配置描述符
UINT8C	SetupGetCfgDescr[] = { USB_REQ_TYP_IN, USB_GET_DESCRIPTOR, 0x00, USB_DESCR_TYP_CONFIG, 0x00, 0x00, 0x04, 0x00 };
// 设置USB地址
UINT8C	SetupSetUsbAddr[] = { USB_REQ_TYP_OUT, USB_SET_ADDRESS, USB_DEVICE_ADDR, 0x00, 0x00, 0x00, 0x00, 0x00 };
// 设置USB配置
UINT8C	SetupSetUsbConfig[] = { USB_REQ_TYP_OUT, USB_SET_CONFIGURATION, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
// 清除端点STALL
UINT8C	SetupClrEndpStall[] = { USB_REQ_TYP_OUT | USB_REQ_RECIP_ENDP, USB_CLEAR_FEATURE, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

UINT8X	UsbDevEndp0Size;	/* USB设备的端点0的最大包尺寸 */

//USB设备相关信息表,CH554最多支持2个设备
#define	ROOT_DEV_DISCONNECT		0
#define	ROOT_DEV_CONNECTED		1
#define	ROOT_DEV_FAILED			2
#define	ROOT_DEV_SUCCESS		3

UINT8X	RxBuffer[ MAX_PACKET_SIZE ] _at_ 0x0000 ;  // IN, must even address
UINT8X	TxBuffer[ MAX_PACKET_SIZE ] _at_ 0x0040 ;  // OUT, must even address
#define	pSetupReq	((PXUSB_SETUP_REQ)TxBuffer)
bit		FoundNewDev;

#pragma NOAREGS


/* 检查操作状态,如果错误则显示错误代码并停机 */
void	mStopIfError( UINT8 iError )
{
	if ( iError == ERR_SUCCESS ) return;  /* 操作成功 */
	printf( "Error: %02X\n", (UINT16)iError );  /* 显示错误 */
/* 遇到错误后,应该分析错误码以及CH554DiskStatus状态,例如调用CH554DiskReady查询当前U盘是否连接,如果U盘已断开那么就重新等待U盘插上再操作,
   建议出错后的处理步骤:
   1、调用一次CH554DiskReady,成功则继续操作,例如Open,Read/Write等
   2、如果CH554DiskReady不成功,那么强行将从头开始操作(等待U盘连接，CH554DiskReady等) */
	while ( 1 ) {
//		LED_TMP=0;  /* LED闪烁 */
//		mDelaymS( 100 );
//		LED_TMP=1;
//		mDelaymS( 100 );
	}
}

void main( ) {
	UINT8	s, c,i;
	UINT16	TotalCount;
	UINT8  buf[100];                                                          //长度可以根据应用自己指定
	
  CfgFsys();
  mDelaymS(5);                                                              //修改主频，稍加延时等待主频稳定
	mInitSTDIO( );                                                            //初始化串口0为了让计算机通过串口监控演示过程 
	InitUSB_Host( );
	CH554LibInit( );                                                          //初始化CH554程序库以支持U盘文件
	FoundNewDev = 0;
	while ( 1 ) {
		   s = ERR_SUCCESS;
		   if ( UIF_DETECT ) {                                                  // 如果有USB主机检测中断则处理
			      UIF_DETECT = 0;                                                 // 清连接中断标志
		        s = AnalyzeRootHub( );                                          // 分析ROOT-HUB状态
		        if ( s == ERR_USB_CONNECT ) FoundNewDev = 1;
		    }
		    if ( FoundNewDev || s == ERR_USB_CONNECT ) {                        // 有新的USB设备插入
			       FoundNewDev = 0;
		         mDelaymS( 200 );                                               // 由于USB设备刚插入尚未稳定,故等待USB设备数百毫秒,消除插拔抖动
			       s = InitRootDevice( );                                         // 初始化USB设备
			       if ( s == ERR_SUCCESS ){
			             // U盘操作流程：USB总线复位、U盘连接、获取设备描述符和设置USB地址、可选的获取配置描述符，之后到达此处，由CH554子程序库继续完成后续工作
				         CH554DiskStatus = DISK_USB_ADDR;
				         for ( i = 0; i != 10; i ++ ) {
					           printf( "Wait DiskReady\n" );
					           s = CH554DiskReady( );                                 //等待U盘准备好
					           if ( s == ERR_SUCCESS ) break;
					           mDelaymS( 50 );
				         }
				         if ( CH554DiskStatus >= DISK_MOUNTED ) {  
/* 读文件 */
                       strcpy( mCmdParam.Open.mPathName, "/C51/CH554HFT.C" ); //设置将要操作的文件路径和文件名/C51/CH554HFT.C
                       s = CH554FileOpen( );                                //打开文件
                       if ( s == ERR_MISS_DIR || s == ERR_MISS_FILE ) {     //没有找到文件
                         printf( "没有找到文件\n" );
				               }
				               else 
                       {                                                      //找到文件或者出错
					                TotalCount = 100;                                   //设置准备读取总长度100字节
					                printf( "从文件中读出的前%d个字符是:\n",TotalCount );
					                while ( TotalCount ) {                                 //如果文件比较大,一次读不完,可以再调用CH554ByteRead继续读取,文件指针自动向后移动
						                 if ( TotalCount > (MAX_PATH_LEN-1) ) c = MAX_PATH_LEN-1;/* 剩余数据较多,限制单次读写的长度不能超过 sizeof( mCmdParam.Other.mBuffer ) */
						                 else c = TotalCount;                                 /* 最后剩余的字节数 */
						                 mCmdParam.ByteRead.mByteCount = c;                   /* 请求读出几十字节数据 */
                             mCmdParam.ByteRead.mByteBuffer= &buf[0];
						                 s = CH554ByteRead( );                                /* 以字节为单位读取数据块,单次读写的长度不能超过MAX_BYTE_IO,第二次调用时接着刚才的向后读 */
						                 TotalCount -= mCmdParam.ByteRead.mByteCount;         /* 计数,减去当前实际已经读出的字符数 */
						                 for ( i=0; i!=mCmdParam.ByteRead.mByteCount; i++ ) printf( "%C", mCmdParam.ByteRead.mByteBuffer[i] );  /* 显示读出的字符 */
						                 if ( mCmdParam.ByteRead.mByteCount < c ) {           /* 实际读出的字符数少于要求读出的字符数,说明已经到文件的结尾 */
							                  printf( "\n" );
							                  printf( "文件已经结束\n" );
							                  break;
						                 }
					                }
			                    printf( "Close\n" );
		                      i = CH554FileClose( );                                 /* 关闭文件 */
					                mStopIfError( i );
				                }
//移动文件指针								
/*  如果希望从指定位置开始读写,可以移动文件指针
		mCmdParam.ByteLocate.mByteOffset = 608;  跳过文件的前608个字节开始读写
		CH554ByteLocate( );
		mCmdParam.ByteRead.mByteCount = 5;  读取5个字节
    mCmdParam.ByteRead.mByteBuffer= &buf[0];
		CH554ByteRead( );   直接读取文件的第608个字节到612个字节数据,前608个字节被跳过

	  如果希望将新数据添加到原文件的尾部,可以移动文件指针
		CH554FileOpen( );
		mCmdParam.ByteLocate.mByteOffset = 0xffffffff;  移到文件的尾部
		CH554ByteLocate( );
		mCmdParam.ByteWrite.mByteCount = 13;  写入13个字节的数据
		CH554ByteWrite( );   在原文件的后面添加数据,新加的13个字节接着原文件的尾部放置
		mCmdParam.ByteWrite.mByteCount = 2;  写入2个字节的数据
		CH554ByteWrite( );   继续在原文件的后面添加数据
		mCmdParam.ByteWrite.mByteCount = 0;  写入0个字节的数据,实际上该操作用于通知程序库更新文件长度
		CH554ByteWrite( );   写入0字节的数据,用于自动更新文件的长度,所以文件长度增加15,如果不这样做,那么执行CH554FileClose时也会自动更新文件长度
*/

//创建文件演示								
		          printf( "Create\n" );
		          strcpy( mCmdParam.Create.mPathName, "/NEWFILE.TXT" );          /* 新文件名,在根目录下,中文文件名 */
		          s = CH554FileCreate( );                                        /* 新建文件并打开,如果文件已经存在则先删除后再新建 */
		          mStopIfError( s );
					    printf( "ByteWrite\n" );
						//实际应该判断写数据长度和定义缓冲区长度是否相符，如果大于缓冲区长度则需要多次写入
					    i = sprintf( buf,"Note: \xd\xa这个程序是以字节为单位进行U盘文件读写,559简单演示功能。\xd\xa");  /*演示 */
						  for(c=0;c<10;c++)
						  {
					      mCmdParam.ByteWrite.mByteCount = i;                          /* 指定本次写入的字节数 */
				   	    mCmdParam.ByteWrite.mByteBuffer = buf;                       /* 指向缓冲区 */
					      s = CH554ByteWrite( );                                       /* 以字节为单位向文件写入数据 */
						    mStopIfError( s );
						    printf("成功写入 %02X次\n",(UINT16)c);
						}
						
//演示修改文件属性						
/*	printf( "Modify\n" );
		mCmdParam.Modify.mFileAttr = 0xff;   输入参数: 新的文件属性,为0FFH则不修改
		mCmdParam.Modify.mFileTime = 0xffff;   输入参数: 新的文件时间,为0FFFFH则不修改,使用新建文件产生的默认时间
		mCmdParam.Modify.mFileDate = MAKE_FILE_DATE( 2015, 5, 18 );  输入参数: 新的文件日期: 2015.05.18
		mCmdParam.Modify.mFileSize = 0xffffffff;   输入参数: 新的文件长度,以字节为单位写文件应该由程序库关闭文件时自动更新长度,所以此处不修改
		i = CH554FileModify( );   修改当前文件的信息,修改日期
		mStopIfError( i );
*/
		         printf( "Close\n" );
		         mCmdParam.Close.mUpdateLen = 1;                                  /* 自动计算文件长度,以字节为单位写文件,建议让程序库关闭文件以便自动更新文件长度 */
		         i = CH554FileClose( );
		         mStopIfError( i );

/* 删除某文件 */
/*	printf( "Erase\n" );
		strcpy( mCmdParam.Create.mPathName, "/OLD" );  将被删除的文件名,在根目录下
		i = CH554FileErase( );  删除文件并关闭
		if ( i != ERR_SUCCESS ) printf( "Error: %02X\n", (UINT16)i );  显示错误
*/
             }
					}
		  }
		  mDelaymS( 100 );  // 模拟单片机做其它事
	    SetUsbSpeed( 1 );  // 默认为全速
	  }
}

