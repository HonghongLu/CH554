/********************************** (C) COPYRIGHT *******************************
* File Name          : udisk.c
* Author             : WCH
* Version            : V1.0
* Date               : 2017/03/08
* Description        : CH554模拟U盘
*******************************************************************************/
#include "CH554.H"
#include "DEBUG.H"
#include <stdio.h>
#include <string.h>
#pragma  NOAREGS

UINT8X  Ep0Buffer[DEFAULT_ENDP0_SIZE] _at_ 0x0000; 
UINT8X  Ep1Buffer[2*MAX_PACKET_SIZE] _at_ DEFAULT_ENDP0_SIZE;

UINT8   UsbConfig = 0; 
#define UsbSetupBuf     ((PUSB_SETUP_REQ)Ep0Buffer)

/*设备描述符*/
UINT8C  MyDevDescr[] = { 0x12, 0x01, 0x10, 0x01,
                         0x00, 0x00, 0x00, DEFAULT_ENDP0_SIZE,
                         0x44, 0x33, 0x33, 0x35,                              // 厂商ID和产品ID
                         0x00, 0x01, 0x01, 0x02,
                         0x00, 0x01
                       };
/*配置描述符*/
UINT8C  MyCfgDescr[] = { 0x09, 0x02, 0x20, 0x00, 0x01, 0x01, 0x00, 0xa0, 0x32,
                         0x09, 0x04, 0x00, 0x00, 0x02, 0x08, 0x06, 0x50, 0x00,                     
                         0x07, 0x05, 0x01, 0x02, 0x40, 0x00, 0x00,
                         0x07, 0x05, 0x81, 0x02, 0x40, 0x00, 0x00
                       };
/*语言描述符*/
UINT8C  MyLangDescr[] = { 0x04, 0x03, 0x09, 0x04 };
/*厂家信息*/
UINT8C  MyManuInfo[] = { 0x0E, 0x03, 'w', 0, 'c', 0, 'h', 0, '.', 0, 'c', 0, 'n', 0 };
/*产品信息*/
UINT8C  MyProdInfo[] = { 0x0C, 0x03, 'C', 0, 'H', 0, '5', 0, '5', 0, '4', 0 };

UINT8C  MAX_LUN[] = {0};

//INQUIRY inform
UINT8C	DBINQUITY[]={
						0x00,             //Peripheral Device Type
						0x80, 			//
						0x02 ,			//ISO/ECMA
						0x02 ,			//
						0x1f ,			//Additional Length

						00 ,			//Reserved
						00 ,			//Reserved
						00 ,				//Reserved


					    'w' ,			//Vendor Information
						'c' ,			//
						'h' ,			//
						'.' ,			//
						'c' ,			//
						'n' ,			//
						' ' ,			//
						' ' ,			//


        			    0xc7,			//Product Identification
						0xdf, 			//
						0xba,			//
						0xe3,			//
						0xb5,			//
						0xe7,			//
						0xd7,			//
						0xd3,			//
						0x55,			//
						0xc5,			//
						0xcc,			//
						0xb7,			//
						0xbd,			//
						0xb0,			//
						0xb8,			//
						0x00,          //

               			'1' ,			//Product Revision Level
						'.' ,			//
						'1' ,			//
						'0'  			//
						  };
UINT8C DBFORMATCAP[]={0x00,0x00,0x00,0x08,0x00,0xec,0xdf,0xff,0x03,0x00,0x02,0x00}; //可格式化容量
UINT8C DBCAPACITY[]={0x00,0xec,0xdf,0xff,0x00,0x00,0x02,0x00};
UINT8C modesense3F[]={0x0b, 0x00, 0x80, 0x08, 0x00,0x00,0x00,0x00,0x00, 0x00, 0x02, 0x00 };   //写保护(0x80换成0x00可以去除写保护)
UINT8C DBR[512]={
	             0xeb,0xfe,0x90,0x4d,0x53,0x44,0x4f,0x53, 0x35,0x2e,0x30,0x00,0x02,0x08,0x20,0x00,
                 0x02,0x00,0x00,0x00,0x00,0xf0,0x00,0x00, 0x3f,0x00,0xff,0x00,0x00,0x00,0x00,0x00,
				 0x00,0xe0,0xec,0x00,0x39,0x3b,0x00,0x00, 0x00,0x00,0x00,0x00,0x02,0x00,0x00,0x00,
				 0x01,0x00,0x06,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x80,0x00,0x29,0x00,0x00,0x8a,0x49,0x4e, 0x4f,0x20,0x4e,0x41,0x4d,0x45,0x20,0x20,
				 0x20,0x20,0x46,0x41,0x54,0x33,0x32,0x20, 0x20,0x20,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x55,0xaa,
};
UINT8C FAT[512]={
	             0xF0,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0x0F,0x00,0x00,0x00,0x00,
                 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
                 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
				 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,	
};

//UFI通讯
#define FORMAT_UNIT 	0x04
#define INQUIRY 		0x12
#define FORMATCAP 		0x23						  
#define MODE_SELECT 	0x15
#define MODE_SENSE5 	0x5A
#define MODE_SENSE 		0x1A
#define PER_RES_IN 		0x5E
#define PER_RES_OUT 	0x5F
#define PRE_OR_MED 		0x1E
#define READ 			0x28
#define READ_CAPACITY 	0x25
#define RELEASE 		0x17
#define REQUEST_SENSE 	0x03
#define RESERVE 		0x16
#define STA_STO_UNIT 	0x1B
#define SYN_CACHE 		0x35
#define TEST_UNIT 		0x00
#define VERIFY 			0x2F
#define WRITE 			0x2A
#define WRITE_BUFFER 	0x3B

typedef union _CBWCB{
	unsigned char buf1[16];
}CBWCB;
typedef  union _MASS_PARA {
	unsigned char buf[64];
	struct  _SENSE{
		unsigned char ErrorCode;
		unsigned char Reserved1;
		unsigned char SenseKey;
		unsigned char Information[4];
		unsigned char AddSenseLength;
		unsigned char Reserved2[4];
		unsigned char AddSenseCode;
		unsigned char AddSenseCodeQua;
		unsigned char Reserved3[4];
	}Sense;
	struct  _CBW{
	unsigned char dCBWsig[4];
	unsigned char dCBWTag[4];
	unsigned long dCBWDatL;
	unsigned char bmCBWFlags;
	unsigned char bCBWLUN;
	unsigned char bCBWCBLength;
	CBWCB        cbwcb;
	}cbw;
		struct _CSW{
		unsigned char buf2[13];
	}csw;
}MASS_PARA;

union {
unsigned long mDataLength;							//数据长度
unsigned char mdataLen[4];							//
} UFI_Length;
unsigned char mdCBWTag[4];						    //dCBWTag
MASS_PARA  MassPara;
bit CH375BULKUP=0;									//数据上传
bit CH375BULKDOWN = 0;						       //数据下传						  
bit CH375CSW=0;										//CSW上传标志
unsigned char	BcswStatus;							//CSW状态
unsigned char mSenseKey;
unsigned char mASC;

unsigned char *pBuf;

unsigned long SecNum;                              //当前操作的扇区号
		
/*******************************************************************************
* Function Name  : InitUSB_Device()
* Description    : USB设备模式配置
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void InitUSB_Device( void )                                                      // 初始化USB设备
{
    USB_CTRL = 0x00;                                                           //清空USB控制寄存器
    USB_CTRL &= ~bUC_HOST_MODE;                                                //该位为选择设备模式
    USB_CTRL |=  bUC_DEV_PU_EN | bUC_INT_BUSY | bUC_DMA_EN;                    //USB设备和内部上拉使能,在中断期间中断标志未清除前自动返回NAK
    USB_DEV_AD = 0x00;                                                         //设备地址初始化
//     USB_CTRL |= bUC_LOW_SPEED;
//     UDEV_CTRL |= bUD_LOW_SPEED;                                                //选择低速1.5M模式
    USB_CTRL &= ~bUC_LOW_SPEED;
    UDEV_CTRL &= ~bUD_LOW_SPEED;                                             //选择全速12M模式，默认方式
	  UDEV_CTRL = bUD_PD_DIS;  // 禁止DP/DM下拉电阻
    UDEV_CTRL |= bUD_PORT_EN;                                                  //使能物理端口

	UEP1_DMA = Ep1Buffer;                                                      //端点1 数据传输地址
    UEP4_1_MOD = 0xC0;                                                         //端点1上下传缓冲区；端点0单64字节收发缓冲区
    UEP1_CTRL = bUEP_AUTO_TOG | UEP_T_RES_NAK | UEP_R_RES_ACK;                 //端点1自动翻转同步标志位，IN事务返回NAK，OUT返回ACK
	UEP0_DMA = Ep0Buffer;                                                      //端点0数据传输地址
    UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;                                 //手动翻转，OUT事务返回ACK，IN事务返回NAK
	UEP0_T_LEN = 0;
    UEP1_T_LEN = 0;                                                            //预使用发送长度一定要清空
	
    USB_INT_EN |= bUIE_SUSPEND;                                               //使能设备挂起中断
    USB_INT_EN |= bUIE_TRANSFER;                                              //使能USB传输完成中断
    USB_INT_EN |= bUIE_BUS_RST;                                               //使能设备模式USB总线复位中断
    USB_INT_FG |= 0x1F;                                                       //清中断标志
    IE_USB = 1;                                                               //使能USB中断
    EA = 1;                                                                   //允许单片机中断	
}
/*******************************************************************************
* Function Name  : UFI_Hunding
* Description    : 命令的分类与识别 UFI  CMD
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void UFI_Hunding(void ){		
		switch(MassPara.cbw.cbwcb.buf1[0]){
			case INQUIRY:                
				pBuf = DBINQUITY;					                                      //查询U盘信息
				if(UFI_Length.mDataLength>sizeof(DBINQUITY)) UFI_Length.mDataLength=sizeof(DBINQUITY);
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;
			break;
			case FORMATCAP:                                                              //可格式化容量（模拟8G盘）
				pBuf = DBFORMATCAP;
				if(UFI_Length.mDataLength>sizeof(DBFORMATCAP)) UFI_Length.mDataLength=sizeof(DBFORMATCAP);
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;			
			break;
//			case WRITE:	
//               UFI_write();
//			break;
			case PRE_OR_MED:
			case TEST_UNIT:
				CH375BULKDOWN=0;
				CH375BULKUP=0;
				BcswStatus=0;			
				mSenseKey=0;
				mASC=0;
			break;
			case READ:
				UFI_Length.mDataLength=(((UINT32)MassPara.cbw.cbwcb.buf1[7]<<8) | (UINT32)MassPara.cbw.cbwcb.buf1[8])*512;  //发送长度
				SecNum = ((UINT32)MassPara.cbw.cbwcb.buf1[2]<<24) | ((UINT32)MassPara.cbw.cbwcb.buf1[3]<<16) | ((UINT32)MassPara.cbw.cbwcb.buf1[4]<<8) | (UINT32)MassPara.cbw.cbwcb.buf1[5];//起始扇区号
				if(SecNum==0 || SecNum==6)
				{
					pBuf = DBR;
				}
				else if(SecNum==0x20 || SecNum==0x3b59)
				{
					pBuf = FAT;
				}
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;	
				
			break;
			case REQUEST_SENSE:          
				MassPara.Sense.ErrorCode=0x70;
				MassPara.Sense.Reserved1=0;
				MassPara.Sense.SenseKey=mSenseKey;
				MassPara.Sense.Information[0]=0;
				MassPara.Sense.Information[1]=0;
				MassPara.Sense.Information[2]=0;
				MassPara.Sense.Information[3]=0;
				MassPara.Sense.AddSenseLength=0x0a;
				MassPara.Sense.Reserved2[0]=0;
				MassPara.Sense.Reserved2[1]=0;
				MassPara.Sense.Reserved2[2]=0;
				MassPara.Sense.Reserved2[3]=0;
				MassPara.Sense.AddSenseCode=mASC;
				MassPara.Sense.AddSenseCodeQua=0;
				MassPara.Sense.Reserved3[0]=0;
				MassPara.Sense.Reserved3[1]=0;
				MassPara.Sense.Reserved3[2]=0;
				MassPara.Sense.Reserved3[3]=0;
				pBuf=MassPara.buf;
				if ( UFI_Length.mDataLength > 18 ) UFI_Length.mDataLength = 18;
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;				
			break;
			case READ_CAPACITY:
				if ( UFI_Length.mDataLength > sizeof(DBCAPACITY) ) UFI_Length.mDataLength = sizeof(DBCAPACITY);
				pBuf=(unsigned char*)DBCAPACITY;	
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;  								
			break;
			case MODE_SENSE:
				if ( UFI_Length.mDataLength > sizeof(modesense3F) ) UFI_Length.mDataLength = sizeof(modesense3F);
				pBuf=(unsigned char*)modesense3F;	
				BcswStatus=0;
				mSenseKey=0;
				mASC=0;				
			break;
			default:			
				mSenseKey=5;
				mASC=0x24;
				BcswStatus=1;
				if(CH375BULKUP)
				{
					UEP1_CTRL = UEP1_CTRL | UEP_T_RES_STALL;
				}
				else
				{
					UEP1_CTRL = UEP1_CTRL | UEP_R_RES_STALL;
				}
				break;
			}
}
/*******************************************************************************
* Function Name  : mCH375UpCsw
* Description    : 批量协议状态上传
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void mCH375UpCsw()
{
	unsigned char i;													//如果数据为0
	pBuf=&MassPara.buf[0];
	CH375CSW=0;																	//上传CSW
	CH375BULKUP=0;														//取消数据上传
	MassPara.buf[0]=0x55;												//dCSWSignature
	MassPara.buf[1]=0x53;
	MassPara.buf[2]=0x42;
	MassPara.buf[3]=0x53;
	MassPara.buf[4]=mdCBWTag[0];
	MassPara.buf[5]=mdCBWTag[1];
	MassPara.buf[6]=mdCBWTag[2];
	MassPara.buf[7]=mdCBWTag[3];
	MassPara.buf[8]=UFI_Length.mdataLen[3];
	MassPara.buf[9]=UFI_Length.mdataLen[2];
	MassPara.buf[10]=UFI_Length.mdataLen[1];
	MassPara.buf[11]=UFI_Length.mdataLen[0];
	MassPara.buf[12]=BcswStatus;
	for(i = 0;i<13;i++)
	{
		Ep1Buffer[MAX_PACKET_SIZE+i] = *pBuf;
		pBuf++;
	}
	UEP1_T_LEN = 13;
	UEP1_CTRL = UEP1_CTRL & ~ MASK_UEP_T_RES | UEP_T_RES_ACK;          // 允许上传
}

/*******************************************************************************
* Function Name  : mCH375BulkOnly
* Description    : 批量协议处理
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void mCH375BulkOnly(){
	if(MassPara.buf[0]==0x55){
		if(MassPara.buf[1]==0x53){
		   if(MassPara.buf[2]==0x42){
				if(MassPara.buf[3]==0x43){
					UFI_Length.mdataLen[3] = *(unsigned char *)(&MassPara.cbw.dCBWDatL);             /* 将PC机的低字节在前的16位字数据转换为C51的高字节在前的数据 */
					UFI_Length.mdataLen[2] = *( (unsigned char *)(&MassPara.cbw.dCBWDatL) + 1 );
					UFI_Length.mdataLen[1] = *( (unsigned char *)(&MassPara.cbw.dCBWDatL) + 2 );
					UFI_Length.mdataLen[0] = *( (unsigned char *)(&MassPara.cbw.dCBWDatL) + 3 );
					mdCBWTag[0]=MassPara.buf[4];
					mdCBWTag[1]=MassPara.buf[5];
					mdCBWTag[2]=MassPara.buf[6];
					mdCBWTag[3]=MassPara.buf[7];										     //取出数据长度
					if(UFI_Length.mDataLength){
							CH375BULKDOWN=(MassPara.cbw.bmCBWFlags&0X80)?0:1;	             //判断是上传还是下传数据
							CH375BULKUP=(MassPara.cbw.bmCBWFlags&0X80)?1:0;
						}
						CH375CSW=1;
						UFI_Hunding();                                                       //调用UFI协议处理
			   }
				else
				UEP1_CTRL = UEP1_CTRL | UEP_T_RES_STALL ;
		  }
		   else
			UEP1_CTRL = UEP1_CTRL | UEP_T_RES_STALL ;
		 }
		else
		UEP1_CTRL = UEP1_CTRL | UEP_T_RES_STALL ;
	 }
	else
	UEP1_CTRL = UEP1_CTRL | UEP_T_RES_STALL ;
}
/*******************************************************************************
* Function Name  : CH375bulkUpData
* Description    : 批量协议上传
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CH375bulkUpData(){											           //调用端点1上传数据
		unsigned char len,i;
		if(UFI_Length.mDataLength>MAX_PACKET_SIZE){
			len=MAX_PACKET_SIZE;
			UFI_Length.mDataLength-=MAX_PACKET_SIZE;
		}
		else {
			len= (unsigned char) UFI_Length.mDataLength;
			UFI_Length.mDataLength=0;
			CH375BULKUP=0;
		}		
		{
			for(i = 0;i<len;i++)
			{
				Ep1Buffer[MAX_PACKET_SIZE+i] = *pBuf;
				pBuf++;
			}
		}
		UEP1_T_LEN = len;
		UEP1_CTRL = UEP1_CTRL & ~ MASK_UEP_T_RES | UEP_T_RES_ACK;        // 允许上传
}
/*******************************************************************************
* Function Name  : USB_DeviceInterrupt
* Description    : USB模拟设置中断处理函数
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USB_DeviceInterrupt( void ) interrupt INT_NO_USB  using 1             /* USB中断服务程序,使用寄存器组1 */
{
    UINT8   len,length;
    static  UINT8   SetupReqCode;
	static  UINT16 SetupLen;
    static  PUINT8  pDescr;
	
    if ( UIF_TRANSFER )                                                        // USB传输完成
    {
		switch ( USB_INT_ST & ( MASK_UIS_TOKEN | MASK_UIS_ENDP ) )         // 分析操作令牌和端点号
		{
		case UIS_TOKEN_IN | 1:                                             // endpoint 1# 中断端点上传
			if(CH375BULKUP) CH375bulkUpData();								//调用数据上传
			else if(CH375CSW) {
				CH375CSW = 0;
				mCH375UpCsw();									//上传CSW
			}
			else
			UEP1_CTRL = UEP1_CTRL & ~ MASK_UEP_T_RES | UEP_T_RES_NAK;      // 暂停上传
			break;
		case UIS_TOKEN_OUT | 1:
			if ( U_TOG_OK )                                                // 不同步的数据包将丢弃
			{
				if(CH375BULKDOWN) 
				{							
					//暂时不可写
//					mCH375BulkDownData();				                   //如果上传数据阶段则调用数据上传
				}
				else{	 										           //不是数据下传则判断是否
						length = USB_RX_LEN;
						if(!length)break;								   //数据包长度为零则跳出																		
						for(len=0;len!=length;len++)						
							MassPara.buf[len]=Ep1Buffer[len];	           //将数据读入到缓冲区
						mCH375BulkOnly();
						if(!CH375BULKDOWN){
							if(CH375BULKUP) CH375bulkUpData();			    //调用批量数据上传
							else mCH375UpCsw();                             //test
						}
				}	
			}				
		
			break;
		case UIS_TOKEN_SETUP | 0:                                          // endpoint 0# SETUP
			len = USB_RX_LEN;
			if ( len == sizeof( USB_SETUP_REQ ) )                          // SETUP包长度
			{
				SetupLen = ((UINT16)UsbSetupBuf->wLengthH<<8) | (UsbSetupBuf->wLengthL);
				len = 0;                                                   // 默认为成功并且上传0长度
				SetupReqCode = UsbSetupBuf->bRequest;
				if ( ( UsbSetupBuf->bRequestType & USB_REQ_TYP_MASK ) != USB_REQ_TYP_STANDARD )/* 类请求 */
				{                                                                  
					if(SetupReqCode == 0xFE)                               //GET MAX LUN
					{
						pDescr = (PUINT8)( &MAX_LUN[0] ); 							
						len = 1;
						if ( SetupLen > len )
						{
							SetupLen = len;                                 // 限制总长度
						}
						len = SetupLen >= DEFAULT_ENDP0_SIZE ? DEFAULT_ENDP0_SIZE : SetupLen;  // 本次传输长度
						memcpy( Ep0Buffer, pDescr, len );                   /* 加载上传数据 */
						SetupLen -= len;
						pDescr += len;						
					}
					else
						len = 0xFF;					
				}
				else                                                       // 标准请求
				{					
					switch( SetupReqCode )                                 // 请求码
					{
					case USB_GET_DESCRIPTOR:
						switch( UsbSetupBuf->wValueH )
						{
						case 1:                                            // 设备描述符
							pDescr = (PUINT8)( &MyDevDescr[0] );
							len = sizeof( MyDevDescr );
							break;
						case 2:                                            // 配置描述符
							pDescr = (PUINT8)( &MyCfgDescr[0] );
							len = sizeof( MyCfgDescr );
							break;
						case 3:                                            // 字符串描述符
							switch( UsbSetupBuf->wValueL )
							{
							case 1:
								pDescr = (PUINT8)( &MyManuInfo[0] );       
								len = sizeof( MyManuInfo );
								break;
							case 2:
								pDescr = (PUINT8)( &MyProdInfo[0] );        
								len = sizeof( MyProdInfo );
								break;
							case 0:
								pDescr = (PUINT8)( &MyLangDescr[0] );
								len = sizeof( MyLangDescr );
								break;
							default:
								len = 0xFF;                                 // 不支持的字符串描述符
								break;
							}
							break;
						default:
							len = 0xFF;                                     // 不支持的描述符类型
							break;
						}
						if ( SetupLen > len )
						{
							SetupLen = len;                                 // 限制总长度
						}
						len = SetupLen >= DEFAULT_ENDP0_SIZE ? DEFAULT_ENDP0_SIZE : SetupLen;  // 本次传输长度
						memcpy( Ep0Buffer, pDescr, len );                   /* 加载上传数据 */
						SetupLen -= len;
						pDescr += len;
						break;
					case USB_SET_ADDRESS:
						SetupLen = UsbSetupBuf->wValueL;                    // 暂存USB设备地址
						break;
					case USB_GET_CONFIGURATION:
						Ep0Buffer[0] = UsbConfig;
						if ( SetupLen >= 1 )
						{
							len = 1;
						}
						break;
					case USB_SET_CONFIGURATION:
						UsbConfig = UsbSetupBuf->wValueL;
					#if DE_PRINTF
						printf("Config\n");
					#endif												
						break;
					case USB_CLEAR_FEATURE:
                        if( ( UsbSetupBuf->bRequestType & USB_REQ_RECIP_MASK ) == USB_REQ_RECIP_DEVICE )                  /* 清除设备 */
                        {
                            if( ( ( ( UINT16 )UsbSetupBuf->wValueH << 8 ) | UsbSetupBuf->wValueL ) == 0x01 )
                            {
                                if( MyCfgDescr[ 7 ] & 0x20 )
                                {
                                    /* 唤醒 */
                                }
                                else
                                {
                                    len = 0xFF;                                        /* 操作失败 */
                                }
                            }
                            else
                            {
                                len = 0xFF;                                            /* 操作失败 */
                            }
                        }						
						else if ( ( UsbSetupBuf->bRequestType & USB_REQ_RECIP_MASK ) == USB_REQ_RECIP_ENDP )// 端点
						{
							switch( UsbSetupBuf->wIndexL )
							{
							case 0x82:
								UEP2_CTRL = UEP2_CTRL & ~ ( bUEP_T_TOG | MASK_UEP_T_RES ) | UEP_T_RES_NAK;
								break;
							case 0x02:
								UEP2_CTRL = UEP2_CTRL & ~ ( bUEP_R_TOG | MASK_UEP_R_RES ) | UEP_R_RES_ACK;
								break;
							case 0x81:
								UEP1_CTRL = UEP1_CTRL & ~ ( bUEP_T_TOG | MASK_UEP_T_RES ) | UEP_T_RES_NAK;
								if(CH375CSW) 
								{
									CH375CSW=0;
									mCH375UpCsw();
								}
								break;
							case 0x01:
								UEP1_CTRL = UEP1_CTRL & ~ ( bUEP_R_TOG | MASK_UEP_R_RES ) | UEP_R_RES_ACK;						
								if(CH375CSW) 
								{
									CH375CSW = 0;
									mCH375UpCsw();
								}
								break;
							default:
								len = 0xFF;                                     // 不支持的端点
								break;
							}
						}
						else
						{
							len = 0xFF;                                         // 不是端点不支持
						}
						break;
                    case USB_SET_FEATURE:                                          /* Set Feature */
                        if( ( UsbSetupBuf->bRequestType & 0x1F ) == USB_REQ_RECIP_DEVICE )                  /* 设置设备 */
                        {
                            if( ( ( ( UINT16 )UsbSetupBuf->wValueH << 8 ) | UsbSetupBuf->wValueL ) == 0x01 )
                            {
                                if( MyCfgDescr[ 7 ] & 0x20 )
                                {
                                    /* 休眠 */
						#if DE_PRINTF
									printf( "suspend\n" );                                                             //睡眠状态
						#endif
									while ( XBUS_AUX & bUART0_TX )
									{
										;    //等待发送完成
									}
									SAFE_MOD = 0x55;
									SAFE_MOD = 0xAA;
									WAKE_CTRL = bWAK_BY_USB | bWAK_RXD0_LO;                      //USB或者RXD0有信号时可被唤醒
									PCON |= PD;                                                                 //睡眠
									SAFE_MOD = 0x55;
									SAFE_MOD = 0xAA;
									WAKE_CTRL = 0x00;
                                }
                                else
                                {
                                    len = 0xFF;                                        /* 操作失败 */
                                }
                            }
                            else
                            {
                                len = 0xFF;                                            /* 操作失败 */
                            }
                        }
                        else if( ( UsbSetupBuf->bRequestType & 0x1F ) == USB_REQ_RECIP_ENDP )             /* 设置端点 */
                        {
                            if( ( ( ( UINT16 )UsbSetupBuf->wValueH << 8 ) | UsbSetupBuf->wValueL ) == 0x00 )
                            {
                                switch( ( ( UINT16 )UsbSetupBuf->wIndexH << 8 ) | UsbSetupBuf->wIndexL )
                                {								
                                case 0x82:
                                    UEP2_CTRL = UEP2_CTRL & (~bUEP_T_TOG) | UEP_T_RES_STALL;/* 设置端点2 IN STALL */
                                    break;
                                case 0x02:
                                    UEP2_CTRL = UEP2_CTRL & (~bUEP_R_TOG) | UEP_R_RES_STALL;/* 设置端点2 OUT Stall */
                                    break;
                                case 0x81:
                                    UEP1_CTRL = UEP1_CTRL & (~bUEP_T_TOG) | UEP_T_RES_STALL;/* 设置端点1 IN STALL */
                                    break;
								case 0x01:
									UEP1_CTRL = UEP1_CTRL & (~bUEP_R_TOG) | UEP_R_RES_STALL;/* 设置端点1 OUT Stall */
                                default:
                                    len = 0xFF;                                    /* 操作失败 */
                                    break;
                                }
                            }
                            else
                            {
                                len = 0xFF;                                      /* 操作失败 */
                            }
                        }
                        else
                        {
                            len = 0xFF;                                          /* 操作失败 */
                        }
                        break;											
					case USB_GET_INTERFACE:
						Ep0Buffer[0] = 0x00;
						if ( SetupLen >= 1 )
						{
							len = 1;
						}
						break;
					case USB_GET_STATUS:
						if( ( UsbSetupBuf->bRequestType & 0x1F ) == USB_REQ_RECIP_DEVICE ) 
						{
							Ep0Buffer[0] = 0x02;
							Ep0Buffer[1] = 0x00;							
						}
						else
						{
							Ep0Buffer[0] = 0x00;
							Ep0Buffer[1] = 0x00;							
						}
						if ( SetupLen >= 2 )
						{
							len = 2;
						}
						else
						{
							len = SetupLen;
						}
						break;
					default:
						len = 0xFF;                                             // 操作失败
						#if DE_PRINTF
							printf("ErrEp0ReqCode=%02X\n",(UINT16)SetupReqCode);                                                             //睡眠状态
						#endif
						
						break;
					}
				}
			}
			else
			{
				len = 0xFF;                                                    			
				#if DE_PRINTF
					printf("ErrEp0ReqSize\n");                               // SETUP包长度错误                                                 
				#endif
			}
			if ( len == 0xFF )                                                 // 操作失败
			{
				SetupReqCode = 0xFF;
				UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_STALL | UEP_T_RES_STALL;// STALL
			}
			else if ( len <= DEFAULT_ENDP0_SIZE )                                 // 上传数据或者状态阶段返回0长度包
			{
				UEP0_T_LEN = len;
				UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_ACK | UEP_T_RES_ACK;// 默认数据包是DATA1
			}
			else                                                               // 下传数据或其它
			{
				UEP0_T_LEN = 0;                                                // 虽然尚未到状态阶段，但是提前预置上传0长度数据包以防主机提前进入状态阶段
				UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_ACK | UEP_T_RES_ACK;// 默认数据包是DATA1
			}
			break;
		case UIS_TOKEN_IN | 0:                                                 // endpoint 0# IN
			switch( SetupReqCode )
			{
			case USB_GET_DESCRIPTOR:
				len = SetupLen >= DEFAULT_ENDP0_SIZE ? DEFAULT_ENDP0_SIZE : SetupLen; // 本次传输长度
				memcpy( Ep0Buffer, pDescr, len );                               /* 加载上传数据 */
				SetupLen -= len;
				pDescr += len;
				UEP0_T_LEN = len;
				UEP0_CTRL ^= bUEP_T_TOG;                                        // 翻转
				break;
			case USB_SET_ADDRESS:
				USB_DEV_AD = USB_DEV_AD & bUDA_GP_BIT | SetupLen;
				UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;
				break;
			default:
				UEP0_T_LEN = 0;                                                 // 状态阶段完成中断或者是强制上传0长度数据包结束控制传输
				UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;
				break;
			}
			break;
		case UIS_TOKEN_OUT | 0:                                                 // endpoint 0# OUT
			UEP0_T_LEN = 0;
			UEP0_CTRL ^= bUEP_R_TOG;
//			UEP0_CTRL |= UEP_R_RES_ACK | UEP_T_RES_NAK;                      // 准备下一控制传输
			break;
		default:
			#if DE_PRINTF
				printf("ErrEndp INT\n");
			#endif			
			break;
		}
        UIF_TRANSFER = 0;                                                           // 清中断标志
    }
    else if ( UIF_BUS_RST )                                                         // USB总线复位
    {
#if DE_PRINTF
            printf( "reset\n" );                                                             //睡眠状态
#endif	
        UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;
        UEP1_CTRL = bUEP_AUTO_TOG | UEP_R_RES_ACK | UEP_T_RES_NAK;
        USB_DEV_AD = 0x00;
        UIF_SUSPEND = 0;
        UIF_TRANSFER = 0;
        UIF_BUS_RST = 0;                                                            // 清中断标志
    }
    else if ( UIF_SUSPEND )                                                         // USB总线挂起/唤醒完成
    {
        UIF_SUSPEND = 0;
        if ( USB_MIS_ST & bUMS_SUSPEND )                                            // 挂起
        {
#if DE_PRINTF
             printf( "Suspend\n" );                                                  // 睡眠状态
#endif                                                   
            while ( XBUS_AUX & bUART0_TX );                                         // 等待发送完成
            SAFE_MOD = 0x55;
            SAFE_MOD = 0xAA;
            WAKE_CTRL = bWAK_BY_USB | bWAK_RXD0_LO;                                 // USB或者RXD0有信号时可被唤醒
            PCON |= PD;                                                             // 睡眠
            SAFE_MOD = 0x55;
            SAFE_MOD = 0xAA;
            WAKE_CTRL = 0x00;
        }
        else                                                                        
        {
#if DE_PRINTF
            printf( "Awake\n" );                                                   // 唤醒
#endif    
        }
    }
    else 
    {                                                                               // 意外的中断,不可能发生的情况
#if DE_PRINTF
        printf("Unknown USB_INT_FG:0X%02x\n",(UINT16)USB_INT_FG);                                                
#endif   
        
        USB_INT_FG = 0xFF;                                                          // 清中断标志
    }
	
}

void main()
{
    CfgFsys( );                                                           //CH559时钟选择配置
    mDelaymS(5);                                                          //修改主频等待内部晶振稳定,必加	
    mInitSTDIO( );                                                        //串口0,可以用于调试	
#if DE_PRINTF
    printf("start ...\n");
#endif
	InitUSB_Device();                                                     //设备模式初始化	
	while(1)
	{
		
	}








}