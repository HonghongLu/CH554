
/********************************** (C) COPYRIGHT *******************************
* File Name          :EXAM11.C
* Author             : WCH
* Version            : V1.0
* Date               : 2017/01/20
* Description        : CH554 C语言的U盘目录文件枚举程序
 支持: FAT12/FAT16/FAT32  
*******************************************************************************/

#include "..\..\..\Public\CH554.H"                                                        
#include <stdio.h>
#include <string.h>

//#define DISK_BASE_BUF_LEN		512	/* 默认的磁盘数据缓冲区大小为512字节(可以选择为2048甚至4096以支持某些大扇区的U盘),为0则禁止在本文件中定义缓冲区并由应用程序在pDISK_BASE_BUF中指定 */
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

void main( ) 
{
		UINT8	s,i;
		UINT8	 *pCodeStr;
    UINT16 j;
    CfgFsys();
    mDelaymS(5);                                                              //修改主频稍加延时等待主频稳定
	  mInitSTDIO( );                                                            //初始化串口0为了让计算机通过串口监控演示过程 */
		InitUSB_Host( );
		CH554LibInit( );                                                          //初始化CH554程序库以支持U盘文件
		FoundNewDev = 0;
		printf( "Wait Device In\n" );
		while ( 1 ) 
		{
				s = ERR_SUCCESS;
				if ( UIF_DETECT )                                                     // 如果有USB主机检测中断则处理
			  {  
				  UIF_DETECT = 0;                                                     // 清中断标志
					s = AnalyzeRootHub( );                                              // 分析ROOT-HUB状态
					if ( s == ERR_USB_CONNECT ) 
					FoundNewDev = 1;
		    }
				if ( FoundNewDev || s == ERR_USB_CONNECT ) 
				{                                                                     // 有新的USB设备插入
					FoundNewDev = 0;
					mDelaymS( 200 );                                                    // 由于USB设备刚插入尚未稳定,故等待USB设备数百毫秒,消除插拔抖动
					s = InitRootDevice( );                                              // 初始化USB设备
					if ( s == ERR_SUCCESS ) 
					{
						 printf( "Start UDISK_demo @CH554UFI library\n" );
						// U盘操作流程：USB总线复位、U盘连接、获取设备描述符和设置USB地址、可选的获取配置描述符，之后到达此处，由CH554子程序库继续完成后续工作
						 CH554DiskStatus = DISK_USB_ADDR;
						 for ( i = 0; i != 10; i ++ ) 
						 {
							 printf( "Wait DiskReady\n" );
							 s = CH554DiskReady( );
							 if ( s == ERR_SUCCESS ) break;
							 mDelaymS( 50 );
						 }
						 if ( CH554DiskStatus >= DISK_MOUNTED )                           //U盘准备好
						 {  
/* 读文件 */
							 printf( "Open\n" );
		           strcpy( mCmdParam.Open.mPathName, "/C51/CH554HFT.C" );         //设置要操作的文件名和路径
		           s = CH554FileOpen( );                                          //打开文件
							 if ( s == ERR_MISS_DIR ) {printf("不存在该文件夹则列出根目录所有文件\n");pCodeStr = "/*"; }
							 else pCodeStr = "/C51/CH554*";                                 //CH554HFT.C文件不存在则列出\C51子目录下的以CH554开头的文件
							 printf( "List file %s\n", pCodeStr );                           
							 for ( j = 0; j < 10000; j ++ )                                 //限定10000个文件,实际上没有限制 
							 {  
									 strcpy( mCmdParam.Open.mPathName, pCodeStr );              //搜索文件名,*为通配符,适用于所有文件或者子目录
									 i = strlen( mCmdParam.Open.mPathName );
									 mCmdParam.Open.mPathName[ i ] = 0xFF;                      //根据字符串长度将结束符替换为搜索的序号,从0到254,如果是0xFF即255则说明搜索序号在CH554vFileSize变量中
									 CH554vFileSize = j;                                        //指定搜索/枚举的序号
									 i = CH554FileOpen( );                                      //打开文件,如果文件名中含有通配符*,则为搜索文件而不打开 
									 /* CH554FileEnum 与 CH554FileOpen 的唯一区别是当后者返回ERR_FOUND_NAME时那么对应于前者返回ERR_SUCCESS */
									 if ( i == ERR_MISS_FILE ) break;                           //再也搜索不到匹配的文件,已经没有匹配的文件名 
									 if ( i == ERR_FOUND_NAME )                                 //搜索到与通配符相匹配的文件名,文件名及其完整路径在命令缓冲区中 
									 {  
										 printf( "  match file %04d#: %s\n", (unsigned int)j, mCmdParam.Open.mPathName );  /* 显示序号和搜索到的匹配文件名或者子目录名 */
										 continue;                                                /* 继续搜索下一个匹配的文件名,下次搜索时序号会加1 */
									 }
									 else                                                       //出错
									 {  
										 mStopIfError( i );
										 break;
									 }	 
		            }
			        i = CH554FileClose( );                                          //关闭文件
		          printf( "U盘演示完成\n" );
		       }
		       else 
		       {
		          printf( "U盘没有准备好 ERR =%02X\n", (UINT16)s );
		       }
		     }
		     else
		     {
		        printf("初始化U盘失败，请拔下U盘重试\n");
		     }
       }
		   mDelaymS( 100 );  // 模拟单片机做其它事
		   SetUsbSpeed( 1 );  // 默认为全速
    }
}


