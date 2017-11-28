/********************************** (C) COPYRIGHT *******************************
* File Name          : USBCOM.C
* Author             : WCH
* Version            : V1.0
* Date               : 2015/05/20
* Description        : CH559模拟串口
*******************************************************************************/
#include <CH559.H>
#include <string.h>
#include "DEBUG.C"


#define THIS_ENDP0_SIZE         DEFAULT_ENDP0_SIZE

UINT8X	Ep0Buffer[THIS_ENDP0_SIZE] _at_ 0x0000;                                //端点0 OUT&IN缓冲区，必须是偶地址
UINT8X	Ep2Buffer[2*MAX_PACKET_SIZE] _at_ 0x0008;                              //端点2 IN&OUT缓冲区,必须是偶地址
UINT8X  Ep1Buffer[MAX_PACKET_SIZE] _at_ 0x00a0;

UINT8	  SetReqtp,SetupReq,SetupLen,UsbConfig,Flag;
PUINT8  pDescr;	                                                               
UINT8   num = 0;
UINT8   LEN = 0;
USB_SETUP_REQ	           SetupReqBuf;                                          //暂存Setup包
#define UsbSetupBuf     ((PUSB_SETUP_REQ)Ep0Buffer)

 
UINT8C DevDesc[18]={0x12,0x01,0x10,0x01,0xff,0x00,0x02,0x08,                   //设备描述符
                    0x86,0x1a,0x23,0x55,0x04,0x03,0x00,0x00,
                    0x00,0x01};

UINT8C CfgDesc[39]={0x09,0x02,0x27,0x00,0x01,0x01,0x00,0x80,0xf0,              //配置描述符，接口描述符,端点描述符
	                  0x09,0x04,0x00,0x00,0x03,0xff,0x01,0x02,0x00,           
                    0x07,0x05,0x82,0x02,0x20,0x00,0x00,                        //批量上传端点
		                0x07,0x05,0x02,0x02,0x20,0x00,0x00,                        //批量下传端点      
			              0x07,0x05,0x81,0x03,0x08,0x00,0x01};                       //中断上传端点

UINT8C DataBuf[26]={0x30,0x00,0xc3,0x00,0xff,0xec,0x9f,0xec,0xff,0xec,0xdf,0xec,
                    0xdf,0xec,0xdf,0xec,0x9f,0xec,0x9f,0xec,0x9f,0xec,0x9f,0xec,
                    0xff,0xec};
UINT8 RecBuf[64];
/*******************************************************************************
* Function Name  : USBDeviceCfg()
* Description    : USB设备模式配置
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USBDeviceCfg()
{
    USB_CTRL = 0x00;                                                           //清空USB控制寄存器
    USB_CTRL &= ~bUC_HOST_MODE;                                                //该位为选择设备模式
    USB_CTRL |=  bUC_DEV_PU_EN | bUC_INT_BUSY | bUC_DMA_EN;					           //USB设备和内部上拉使能,在中断期间中断标志未清除前自动返回NAK
    USB_DEV_AD = 0x00;                                                         //设备地址初始化

    UDEV_CTRL &= ~bUD_RECV_DIS;                                                //使能接收器
//  USB_CTRL |= bUC_LOW_SPEED;    
//  UDEV_CTRL |= bUD_LOW_SPEED;                                                //选择低速1.5M模式

    USB_CTRL &= ~bUC_LOW_SPEED;
    UDEV_CTRL &= ~bUD_LOW_SPEED;                                               //选择全速12M模式，默认方式

    UDEV_CTRL |= bUD_DP_PD_DIS | bUD_DM_PD_DIS;                                //禁止DM、DP下拉电阻
    UDEV_CTRL |= bUD_PORT_EN;                                                  //使能物理端口
}


/*******************************************************************************
* Function Name  : USBDeviceIntCfg()
* Description    : USB设备模式中断初始化
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USBDeviceIntCfg()
{
    USB_INT_EN |= bUIE_SUSPEND;                                                //使能设备挂起中断
    USB_INT_EN |= bUIE_TRANSFER;                                               //使能USB传输完成中断
    USB_INT_EN |= bUIE_BUS_RST;                                                //使能设备模式USB总线复位中断
    USB_INT_FG |= 0x1F;                                                        //清中断标志
    IE_USB = 1;                                                                //使能USB中断
    EA = 1; 																                                   //允许单片机中断
}


/*******************************************************************************
* Function Name  : USBDeviceEndPointCfg()
* Description    : USB设备模式端点配置
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void USBDeviceEndPointCfg()
{
    UEP2_DMA = Ep2Buffer;                                                      //端点2数据传输地址																			                                         
    UEP2_3_MOD |= bUEP2_TX_EN;                                                 //端点2发送使能
    UEP2_3_MOD |= bUEP2_RX_EN;                                                 //端点2接收使能
    UEP2_3_MOD &= ~bUEP2_BUF_MOD;                                              //端点2单64字节发送缓冲区、单64字节接收缓冲区，共128字节
    UEP2_CTRL = bUEP_AUTO_TOG | UEP_T_RES_NAK | UEP_R_RES_ACK;								 //端点2自动翻转同步标志位，IN事务返回NAK，OUT返回ACK
	
	
	  UEP1_DMA = Ep1Buffer;                                                      //端点1数据传输地址																			                                         
    UEP4_1_MOD |= bUEP1_TX_EN;                                                 //端点1发送使能
//  UEP4_1_MOD |= bUEP1_RX_EN;                                                 //端点1接收使能
    UEP4_1_MOD &= ~bUEP1_BUF_MOD;                                              //端点1单64字节发送缓冲区
    UEP1_CTRL = bUEP_AUTO_TOG | UEP_T_RES_NAK | UEP_R_RES_ACK;								 //端点1自动翻转同步标志位，IN事务返回NAK，OUT返回ACK
	
	
    UEP0_DMA = Ep0Buffer;                                                      //端点0数据传输地址
    UEP4_1_MOD &= ~(bUEP4_RX_EN | bUEP4_TX_EN);								                 //端点0单64字节收发缓冲区
    UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;                                 //OUT事务返回ACK，IN事务返回NAK
}


/*******************************************************************************
* Function Name  : SendData( PUINT8 SendBuf )
* Description    : 发送数据给主机串口
* Input          : PUINT8 SendBuf
* Output         : None
* Return         : None
*******************************************************************************/
void SendData( PUINT8 SendBuf )
{
	 if(Flag==1)                             
	 {
     while(LEN > 32){		 
     memcpy(&Ep2Buffer[MAX_PACKET_SIZE],SendBuf,32);
	   UEP2_T_LEN = 32;
	   UEP2_CTRL &= ~(bUEP_T_RES1 | bUEP_T_RES0);	
     while(( UEP2_CTRL & MASK_UEP_T_RES ) == UEP_T_RES_ACK);                  //
     LEN -= 32;
     }
     memcpy(&Ep2Buffer[MAX_PACKET_SIZE],SendBuf,LEN);
	   UEP2_T_LEN = LEN;
	   UEP2_CTRL &= ~(bUEP_T_RES1 | bUEP_T_RES0);		 		
     Flag = 0;		 
   }
}


/*******************************************************************************
* Function Name  : RecieveData()
* Description    : USB设备模式端点配置
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void RecieveData()
{
	  memcpy(RecBuf,Ep2Buffer,USB_RX_LEN); 
	  UEP2_CTRL = UEP2_CTRL & ~ MASK_UEP_R_RES | UEP_R_RES_NAK;                    //默认应答ACK
	  Flag = 1;
}


/*******************************************************************************
* Function Name  : DeviceInterrupt()
* Description    : CH559USB中断处理函数
*******************************************************************************/
void	DeviceInterrupt( void ) interrupt INT_NO_USB using 1                       //USB中断服务程序,使用寄存器组1
{   
	UINT8 len; 
	if(UIF_TRANSFER)                                                               //USB传输完成标志
  {
    switch (USB_INT_ST & (MASK_UIS_TOKEN | MASK_UIS_ENDP))
    {
			 case UIS_TOKEN_OUT | 2:                                                   //endpoint 2# 中断下传					 
						LEN = USB_RX_LEN; 
			      RecieveData();
            SendData(RecBuf);			 
						break;
		   case UIS_TOKEN_IN | 2:                                                    //endpoint 2# 中断上传
            UEP2_T_LEN = 0;	                                                     //预使用发送长度一定要清空						 
	          UEP2_CTRL = UEP2_CTRL & ~ MASK_UEP_R_RES | UEP_R_RES_ACK;                    //默认应答ACK					 
    			  UEP2_CTRL = UEP2_CTRL & ~ MASK_UEP_T_RES | UEP_T_RES_NAK;            //默认应答NAK
						break;
    	 case UIS_TOKEN_SETUP | 0:                                                  //SETUP事务
            len = USB_RX_LEN;
            if(len == (sizeof(USB_SETUP_REQ)))
            {   
							 SetReqtp = UsbSetupBuf->bRequestType;
               SetupLen = UsbSetupBuf->wLengthL;
               len = 0;                                                           //默认为成功并且上传0长度,标准请求                                                              
               SetupReq = UsbSetupBuf->bRequest;
               if(SetReqtp == 0xc0)
						   {
								  Ep0Buffer[0] = DataBuf[num];
								  Ep0Buffer[1] = DataBuf[num+1];
								  len = 2;
								  if(num<24)
								  {	
								    num += 2;
									}
									else
									{
										num = 24;
									}
						   }
					     else if(SetReqtp == 0x40)
						   {
							    len = 9;                                                        //保证状态阶段，这里只要比8大，且不等于0xff即可
						   }
						   else
						   { 
							    switch(SetupReq)                                                //请求码
							    {
								     case USB_GET_DESCRIPTOR:
											    switch(UsbSetupBuf->wValueH)
											    {
													   case 1:	                                            //设备描述符
																 pDescr = DevDesc;                                //把设备描述符送到要发送的缓冲区
																 len = sizeof(DevDesc);								       
													   break;	 
													   case 2:									                            //配置描述符
																 pDescr = CfgDesc;                                //把配置描述符送到要发送的缓冲区
																 len = sizeof(CfgDesc);
													   break;	
													   default:
																 len = 0xff;                                      //不支持的命令或者出错
													   break;
											     }
									         if ( SetupLen > len ) SetupLen = len;                  //限制总长度
									         len = SetupLen >= 8 ? 8 : SetupLen;                    //本次传输长度
									         memcpy(Ep0Buffer,pDescr,len);                          //加载上传数据
									         SetupLen -= len;
									         pDescr += len;
										       break;						 
							        case USB_SET_ADDRESS:
										       SetupLen = UsbSetupBuf->wValueL;                       //暂存USB设备地址
										       break;
							        case USB_GET_CONFIGURATION:
									         Ep0Buffer[0] = UsbConfig;
									         if ( SetupLen >= 1 ) len = 1;
									         break;
							        case USB_SET_CONFIGURATION:
									         UsbConfig = UsbSetupBuf->wValueL;
									         break;
							        default:
										       len = 0xff;                                            //操作失败
										       break;    
							       }
					        }
				      }
					    else
					    {
							    len = 0xff;                                                     //包长度错误
					    }

						  if(len == 0xff)
						  {
								  SetupReq = 0xFF;
								  UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_STALL | UEP_T_RES_STALL;//STALL				     
						  }
						  else if(len <= 8)                                                         //上传数据或者状态阶段返回0长度包
						  {
								  UEP0_T_LEN = len;
								  UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_ACK | UEP_T_RES_ACK;  //默认数据包是DATA1，返回应答ACK
						  }
						  else
						  {
								  UEP0_T_LEN = 0;                                                       //虽然尚未到状态阶段，但是提前预置上传0长度数据包以防主机提前进入状态阶段
								  UEP0_CTRL = bUEP_R_TOG | bUEP_T_TOG | UEP_R_RES_ACK | UEP_T_RES_ACK;  //默认数据包是DATA1,返回应答ACK				     
						  }
					    break;
				 case UIS_TOKEN_IN | 0:                                                         //endpoint0 IN
						  switch(SetupReq)
						  {
							   case USB_GET_DESCRIPTOR:
								      len = SetupLen >= 8 ? 8 : SetupLen;                               //本次传输长度
											memcpy( Ep0Buffer, pDescr, len );                                 //加载上传数据
											SetupLen -= len;
											pDescr += len;
											UEP0_T_LEN = len;
											UEP0_CTRL ^= bUEP_T_TOG;                                          //同步标志位翻转
								      break;
							   case USB_SET_ADDRESS:
											USB_DEV_AD = USB_DEV_AD & bUDA_GP_BIT | SetupLen;
											UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;
								      break;
							   default:
								      UEP0_T_LEN = 0;                                                    //状态阶段完成中断或者是强制上传0长度数据包结束控制传输
								      UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_NAK;
								      break;
						  }
						  break;
				 case UIS_TOKEN_OUT | 0:                                                 // endpoint0 OUT
							len = USB_RX_LEN;
							UEP0_T_LEN = 0;                                                    //虽然尚未到状态阶段，但是提前预置上传0长度数据包以防主机提前进入状态阶段
							UEP0_CTRL = UEP_R_RES_ACK | UEP_T_RES_ACK;                         //默认数据包是DATA0,返回应答ACK									
						  break;
					default:
						  break;
				}
				UIF_TRANSFER = 0;                                                        //写0清空中断  
    }
    if(UIF_BUS_RST)                                                              //设备模式USB总线复位中断
    {
			USB_DEV_AD = 0x00;
			UIF_SUSPEND = 0;
			UIF_TRANSFER = 0;
			UIF_BUS_RST = 0;                                                           //清中断标志
    }
	  if (UIF_SUSPEND) 
		{                                                                            //USB总线挂起/唤醒完成
			UIF_SUSPEND = 0;
			if ( USB_MIS_ST & bUMS_SUSPEND ) 
			{                                                                          //挂起
				while ( XBUS_AUX & bUART0_TX );                                          //等待发送完成
				SAFE_MOD = 0x55;
				SAFE_MOD = 0xAA;
				WAKE_CTRL = bWAK_BY_USB | bWAK_RXD0_LO;                                  //USB或者RXD0有信号时可被唤醒
				PCON |= PD;                                                              //睡眠
				SAFE_MOD = 0x55;
				SAFE_MOD = 0xAA;
				WAKE_CTRL = 0x00;
			}
    } 
	  else 
	  {                                                                             //意外的中断,不可能发生的情况
		  USB_INT_FG = 0x00;                                                          //清中断标志
	  }      
}


void main()
{
	  mDelaymS(30);                                                                 //上电延时
//  CfgFsys( );                                                                   //CH559时钟选择配置    
    mInitSTDIO( );                                                                //串口0,可以用于调试
	  USBDeviceCfg();                                                               //设备模式配置
    USBDeviceEndPointCfg();														                            //端点配置
    USBDeviceIntCfg();															                              //中断初始化
	  UEP0_T_LEN = 0;
    UEP1_T_LEN = 0;	                                                              //预使用发送长度一定要清空	
    UEP2_T_LEN = 0;	                                                      
    while(1)
    {   
//         SendData(RecBuf);
// 		    mDelaymS( 500 );                                                         //模拟单片机做其它事				
    }
}

