
/********************************** (C) COPYRIGHT *******************************
* File Name          : USBHOST.C
* Author             : WCH
* Version            : V1.0
* Date               : 2017/01/20
* Description        : CH554 USB 主机接口函数
*******************************************************************************/

#include "..\..\Public\CH554.H"                                                 
#include "..\..\Public\Debug.H"
#include "stdio.h"
#include "USBHOST.H"

// #include "CH554UFI.H"                                                        //如果使用USBH_HUB_KM.C 此行屏蔽掉             
/*******************************************************************************
* Function Name  : DisableRootHubPort( )
* Description    : 关闭HUB端口
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void    DisableRootHubPort( )          
{
#ifdef	FOR_ROOT_UDISK_ONLY
	  CH554DiskStatus = DISK_DISCONNECT;
#endif
#ifndef	DISK_BASE_BUF_LEN
	  ThisUsbDev.DeviceStatus = ROOT_DEV_DISCONNECT;
    ThisUsbDev.DeviceAddress = 0x00;
#endif
}
/*******************************************************************************
* Function Name  : AnalyzeRootHub(void)
* Description    : 分析ROOT-HUB状态,处理ROOT-HUB端口的设备插拔事件
                   如果设备拔出,函数中调用DisableRootHubPort()函数,将端口关闭,插入事件,置相应端口的状态位
* Input          : None
* Output         : None
* Return         : 返回ERR_SUCCESS为没有情况,返回ERR_USB_CONNECT为检测到新连接,返回ERR_USB_DISCON为检测到断开
*******************************************************************************/
UINT8   AnalyzeRootHub( void )
{ 
	UINT8	s;
	s = ERR_SUCCESS;
	if ( USB_MIS_ST & bUMS_DEV_ATTACH ) {                                        // 设备存在
#ifdef DISK_BASE_BUF_LEN
		if ( CH554DiskStatus == DISK_DISCONNECT
#else
		if ( ThisUsbDev.DeviceStatus == ROOT_DEV_DISCONNECT                        // 检测到有设备插入
#endif
			|| ( UHOST_CTRL & bUH_PORT_EN ) == 0x00 ) {                              // 检测到有设备插入,但尚未允许,说明是刚插入
			DisableRootHubPort( );                                                   // 关闭端口
#ifdef DISK_BASE_BUF_LEN
			CH554DiskStatus = DISK_CONNECT;
#else
//		ThisUsbDev.DeviceSpeed = USB_HUB_ST & bUHS_DM_LEVEL ? 0 : 1;
			ThisUsbDev.DeviceStatus = ROOT_DEV_CONNECTED;                            //置连接标志
#endif
#if DE_PRINTF
			printf( "USB dev in\n" );
#endif
			s = ERR_USB_CONNECT;
		}
	}
#ifdef DISK_BASE_BUF_LEN
	else if ( CH554DiskStatus >= DISK_CONNECT ) {
#else
	else if ( ThisUsbDev.DeviceStatus >= ROOT_DEV_CONNECTED ) {                  //检测到设备拔出
#endif
		DisableRootHubPort( );                                                     // 关闭端口
#if DE_PRINTF		
		printf( "USB dev out\n" );
#endif
		if ( s == ERR_SUCCESS ) s = ERR_USB_DISCON;
	}
//	UIF_DETECT = 0;                                                            // 清中断标志
	return( s );
}
/*******************************************************************************
* Function Name  : SetHostUsbAddr
* Description    : 设置USB主机当前操作的USB设备地址
* Input          : UINT8 addr
* Output         : None
* Return         : None
*******************************************************************************/
void    SetHostUsbAddr( UINT8 addr )
{
    USB_DEV_AD = USB_DEV_AD & bUDA_GP_BIT | addr & 0x7F;
}

#ifndef	FOR_ROOT_UDISK_ONLY
/*******************************************************************************
* Function Name  : SetUsbSpeed
* Description    : 设置当前USB速度
* Input          : UINT8 FullSpeed
* Output         : None
* Return         : None
*******************************************************************************/
void    SetUsbSpeed( UINT8 FullSpeed )  
{
    if ( FullSpeed )                                                           // 全速
    {
        USB_CTRL &= ~ bUC_LOW_SPEED;                                           // 全速
        UH_SETUP &= ~ bUH_PRE_PID_EN;                                          // 禁止PRE PID
    }
    else
    {
        USB_CTRL |= bUC_LOW_SPEED;                                             // 低速
    }
}
#endif

/*******************************************************************************
* Function Name  : ResetRootHubPort( )
* Description    : 检测到设备后,复位总线,为枚举设备准备,设置为默认为全速
* Input          : None   
* Output         : None
* Return         : None
*******************************************************************************/
void    ResetRootHubPort( )
{
    UsbDevEndp0Size = DEFAULT_ENDP0_SIZE;                                      //USB设备的端点0的最大包尺寸
    SetHostUsbAddr( 0x00 );
    SetUsbSpeed( 1 );                                                          // 默认为全速
	  UHOST_CTRL = UHOST_CTRL & ~ bUH_LOW_SPEED | bUH_BUS_RESET;                 // 默认为全速,开始复位
    mDelaymS( 15 );                                                            // 复位时间10mS到20mS
    UHOST_CTRL = UHOST_CTRL & ~ bUH_BUS_RESET;                                 // 结束复位
    mDelayuS( 250 );
    UIF_DETECT = 0;                                                            // 清中断标志
}
/*******************************************************************************
* Function Name  : EnableRootHubPort( )
* Description    : 使能ROOT-HUB端口,相应的bUH_PORT_EN置1开启端口,设备断开可能导致返回失败
* Input          : None
* Output         : None
* Return         : 返回ERR_SUCCESS为检测到新连接,返回ERR_USB_DISCON为无连接
*******************************************************************************/
UINT8   EnableRootHubPort( )
{
#ifdef DISK_BASE_BUF_LEN
	if ( CH554DiskStatus < DISK_CONNECT ) CH554DiskStatus = DISK_CONNECT;
#else
	if ( ThisUsbDev.DeviceStatus < ROOT_DEV_CONNECTED ) ThisUsbDev.DeviceStatus = ROOT_DEV_CONNECTED;
#endif
	if ( USB_MIS_ST & bUMS_DEV_ATTACH ) {                                        // 有设备
#ifndef DISK_BASE_BUF_LEN
		if ( ( UHOST_CTRL & bUH_PORT_EN ) == 0x00 ) {                              // 尚未使能
			ThisUsbDev.DeviceSpeed = USB_MIS_ST & bUMS_DM_LEVEL ? 0 : 1;
			if ( ThisUsbDev.DeviceSpeed == 0 ) UHOST_CTRL |= bUH_LOW_SPEED;          // 低速
		}
#endif
		UHOST_CTRL |= bUH_PORT_EN;                                                 //使能HUB端口
		return( ERR_SUCCESS );
	}
	return( ERR_USB_DISCON );
}
#ifndef DISK_BASE_BUF_LEN
/*******************************************************************************
* Function Name  : SelectHubPort( UINT8 HubPortIndex )
* Description    : 选定需要操作的HUB口
* Input          : UINT8 HubPortIndex 选择操作指定的ROOT-HUB端口的外部HUB的指定端口
* Output         : None
* Return         : None
*******************************************************************************/
void    SelectHubPort( UINT8 HubPortIndex )  
{
    if ( HubPortIndex )                                                        // 选择操作指定的ROOT-HUB端口的外部HUB的指定端口
    {
        SetHostUsbAddr( DevOnHubPort[HubPortIndex-1].DeviceAddress );          // 设置USB主机当前操作的USB设备地址
        if ( DevOnHubPort[HubPortIndex-1].DeviceSpeed == 0 )                   // 通过外部HUB与低速USB设备通讯需要前置ID
        {
            UH_SETUP |= bUH_PRE_PID_EN;                                        // 启用PRE PID
        }
        SetUsbSpeed( DevOnHubPort[HubPortIndex-1].DeviceSpeed );               // 设置当前USB速度
    }
    else                                                                       
    {
        SetHostUsbAddr( ThisUsbDev.DeviceAddress );                            // 设置USB主机当前操作的USB设备地址
        SetUsbSpeed( ThisUsbDev.DeviceSpeed );                                 // 设置USB设备的速度
    }
}
#endif
/*******************************************************************************
* Function Name  : WaitUSB_Interrupt
* Description    : 等待USB中断
* Input          : None
* Output         : None
* Return         : 返回ERR_SUCCESS 数据接收或者发送成功
                   ERR_USB_UNKNOWN 数据接收或者发送失败
*******************************************************************************/
UINT8   WaitUSB_Interrupt( void )
{
    UINT16  i;
    for ( i = WAIT_USB_TOUT_200US; i != 0 && UIF_TRANSFER == 0; i -- ){;}
    return( UIF_TRANSFER ? ERR_SUCCESS : ERR_USB_UNKNOWN );
}
/*******************************************************************************
* Function Name  : USBHostTransact
* Description    : CH554传输事务,输入目的端点地址/PID令牌,同步标志,以20uS为单位的NAK重试总时间(0则不重试,0xFFFF无限重试),返回0成功,超时/出错重试
                   本子程序着重于易理解,而在实际应用中,为了提供运行速度,应该对本子程序代码进行优化
* Input          : UINT8 endp_pid 令牌和地址  endp_pid: 高4位是token_pid令牌, 低4位是端点地址
                   UINT8 tog      同步标志
                   UINT16 timeout 超时时间
* Output         : None
* Return         : ERR_USB_UNKNOWN 超时，可能硬件异常
                   ERR_USB_DISCON  设备断开
                   ERR_USB_CONNECT 设备连接
                   ERR_SUCCESS     传输完成
*******************************************************************************/
UINT8   USBHostTransact( UINT8 endp_pid, UINT8 tog, UINT16 timeout )
{
//	UINT8	TransRetry;
#define	TransRetry	UEP0_T_LEN	                                               // 节约内存
	UINT8	s, r;
	UINT16	i;
	UH_RX_CTRL = UH_TX_CTRL = tog;
	TransRetry = 0;
	
	do {
		UH_EP_PID = endp_pid;                                                      // 指定令牌PID和目的端点号
		UIF_TRANSFER = 0;                                                          // 允许传输
//  s = WaitUSB_Interrupt( );
		for ( i = WAIT_USB_TOUT_200US; i != 0 && UIF_TRANSFER == 0; i -- );
		UH_EP_PID = 0x00;                                                          // 停止USB传输
//	if ( s != ERR_SUCCESS ) return( s );  // 中断超时,可能是硬件异常
		if ( UIF_TRANSFER == 0 ) return( ERR_USB_UNKNOWN );
		if ( UIF_DETECT ) {                                                        // USB设备插拔事件
//			mDelayuS( 200 );                                                       // 等待传输完成
			UIF_DETECT = 0;                                                          // 清中断标志
			s = AnalyzeRootHub( );                                                   // 分析ROOT-HUB状态
			if ( s == ERR_USB_CONNECT ) FoundNewDev = 1;
#ifdef DISK_BASE_BUF_LEN
			if ( CH554DiskStatus == DISK_DISCONNECT ) return( ERR_USB_DISCON );      // USB设备断开事件
			if ( CH554DiskStatus == DISK_CONNECT ) return( ERR_USB_CONNECT );        // USB设备连接事件
#else
			if ( ThisUsbDev.DeviceStatus == ROOT_DEV_DISCONNECT ) return( ERR_USB_DISCON );// USB设备断开事件
			if ( ThisUsbDev.DeviceStatus == ROOT_DEV_CONNECTED ) return( ERR_USB_CONNECT );// USB设备连接事件
#endif
			mDelayuS( 200 );  // 等待传输完成
		}
		if ( UIF_TRANSFER ) {  // 传输完成
			if ( U_TOG_OK ) return( ERR_SUCCESS );
			r = USB_INT_ST & MASK_UIS_H_RES;  // USB设备应答状态
			if ( r == USB_PID_STALL ) return( r | ERR_USB_TRANSFER );
			if ( r == USB_PID_NAK ) {
				if ( timeout == 0 ) return( r | ERR_USB_TRANSFER );
				if ( timeout < 0xFFFF ) timeout --;
				-- TransRetry;
			}
			else switch ( endp_pid >> 4 ) {
				case USB_PID_SETUP:
				case USB_PID_OUT:
//					if ( U_TOG_OK ) return( ERR_SUCCESS );
//					if ( r == USB_PID_ACK ) return( ERR_SUCCESS );
//					if ( r == USB_PID_STALL || r == USB_PID_NAK ) return( r | ERR_USB_TRANSFER );
					if ( r ) return( r | ERR_USB_TRANSFER );  // 不是超时/出错,意外应答
					break;  // 超时重试
				case USB_PID_IN:
//					if ( U_TOG_OK ) return( ERR_SUCCESS );
//					if ( tog ? r == USB_PID_DATA1 : r == USB_PID_DATA0 ) return( ERR_SUCCESS );
//					if ( r == USB_PID_STALL || r == USB_PID_NAK ) return( r | ERR_USB_TRANSFER );
					if ( r == USB_PID_DATA0 && r == USB_PID_DATA1 ) {  // 不同步则需丢弃后重试
					}  // 不同步重试
					else if ( r ) return( r | ERR_USB_TRANSFER );  // 不是超时/出错,意外应答
					break;  // 超时重试
				default:
					return( ERR_USB_UNKNOWN );  // 不可能的情况
					break;
			}
		}
		else {  // 其它中断,不应该发生的情况
			USB_INT_FG = 0xFF;  /* 清中断标志 */
		}
		mDelayuS( 15 );
	} while ( ++ TransRetry < 3 );
	return( ERR_USB_TRANSFER );  // 应答超时
}
/*******************************************************************************
* Function Name  : HostCtrlTransfer
* Description    : 执行控制传输,8字节请求码在pSetupReq中,DataBuf为可选的收发缓冲区
* Input          : PUINT8X DataBuf 如果需要接收和发送数据,那么DataBuf需指向有效缓冲区用于存放后续数据
                   PUINT8 RetLen  实际成功收发的总长度保存在RetLen指向的字节变量中
* Output         : None
* Return         : ERR_USB_BUF_OVER IN状态阶段出错
                   ERR_SUCCESS     数据交换成功
                   其他错误状态
*******************************************************************************/
UINT8   HostCtrlTransfer( PUINT8X DataBuf, PUINT8 RetLen )  
{
    UINT16  RemLen  = 0;
    UINT8   s, RxLen, RxCnt, TxCnt;
    PUINT8  xdata   pBuf;
    PUINT8  xdata   pLen;
    pBuf = DataBuf;
    pLen = RetLen;
    mDelayuS( 200 );
    if ( pLen )
    {
        *pLen = 0;                                                              // 实际成功收发的总长度
    }
    UH_TX_LEN = sizeof( USB_SETUP_REQ );
    s = USBHostTransact( USB_PID_SETUP << 4 | 0x00, 0x00, 200000/20 );          // SETUP阶段,200mS超时
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    UH_RX_CTRL = UH_TX_CTRL = bUH_R_TOG | bUH_R_AUTO_TOG | bUH_T_TOG | bUH_T_AUTO_TOG;// 默认DATA1
    UH_TX_LEN = 0x01;                                                           // 默认无数据故状态阶段为IN
    RemLen = (pSetupReq -> wLengthH << 8)|( pSetupReq -> wLengthL);
    if ( RemLen && pBuf )                                                       // 需要收发数据
    {
        if ( pSetupReq -> bRequestType & USB_REQ_TYP_IN )                       // 收
        {
            while ( RemLen )
            {
                mDelayuS( 200 );
                s = USBHostTransact( USB_PID_IN << 4 | 0x00, UH_RX_CTRL, 200000/20 );// IN数据
                if ( s != ERR_SUCCESS )
                {
                    return( s );
                }
                RxLen = USB_RX_LEN < RemLen ? USB_RX_LEN : RemLen;
                RemLen -= RxLen;
                if ( pLen )
                {
                    *pLen += RxLen;                                              // 实际成功收发的总长度
                }
//              memcpy( pBuf, RxBuffer, RxLen );
//              pBuf += RxLen;
                for ( RxCnt = 0; RxCnt != RxLen; RxCnt ++ )
                {
                    *pBuf = RxBuffer[ RxCnt ];
                    pBuf ++;
                }
                if ( USB_RX_LEN == 0 || ( USB_RX_LEN & ( UsbDevEndp0Size - 1 ) ) )
                {
                    break;                                                       // 短包
                }
            }
            UH_TX_LEN = 0x00;                                                    // 状态阶段为OUT
        }
        else                                                                     // 发
        {
            while ( RemLen )
            {
                mDelayuS( 200 );
                UH_TX_LEN = RemLen >= UsbDevEndp0Size ? UsbDevEndp0Size : RemLen;
//              memcpy( TxBuffer, pBuf, UH_TX_LEN );
//              pBuf += UH_TX_LEN;
#ifndef DISK_BASE_BUF_LEN
                if(pBuf[1] == 0x09)                                              //HID类命令处理
                {
                    Set_Port = Set_Port^1;
                    *pBuf = Set_Port;
#if DE_PRINTF									
                    printf("SET_PORT  %02X  %02X ",(UINT16)(*pBuf),(UINT16)(Set_Port));
#endif									
                }
#endif
                for ( TxCnt = 0; TxCnt != UH_TX_LEN; TxCnt ++ )
                {
                    TxBuffer[ TxCnt ] = *pBuf;
                    pBuf ++;
                }
                s = USBHostTransact( USB_PID_OUT << 4 | 0x00, UH_TX_CTRL, 200000/20 );// OUT数据
                if ( s != ERR_SUCCESS )
                {
                    return( s );
                }
                RemLen -= UH_TX_LEN;
                if ( pLen )
                {
                    *pLen += UH_TX_LEN;                                           // 实际成功收发的总长度
                }
            }
//          UH_TX_LEN = 0x01;                                                     // 状态阶段为IN
        }
    }
    mDelayuS( 200 );
    s = USBHostTransact( ( UH_TX_LEN ? USB_PID_IN << 4 | 0x00: USB_PID_OUT << 4 | 0x00 ), bUH_R_TOG | bUH_T_TOG, 200000/20 );  // STATUS阶段
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( UH_TX_LEN == 0 )
    {
        return( ERR_SUCCESS );                                                    // 状态OUT
    }
    if ( USB_RX_LEN == 0 )
    {
        return( ERR_SUCCESS );                                                    // 状态IN,检查IN状态返回数据长度
    }
    return( ERR_USB_BUF_OVER );                                                   // IN状态阶段错误
}
/*******************************************************************************
* Function Name  : CopySetupReqPkg
* Description    : 复制控制传输的请求包
* Input          : PUINT8C pReqPkt 控制请求包地址
* Output         : None
* Return         : None
*******************************************************************************/
void    CopySetupReqPkg( PUINT8C pReqPkt )                                        // 复制控制传输的请求包
{
    UINT8   i;
    for ( i = 0; i != sizeof( USB_SETUP_REQ ); i ++ )
    {
        ((PUINT8X)pSetupReq)[ i ] = *pReqPkt;
        pReqPkt ++;
    }
}
/*******************************************************************************
* Function Name  : CtrlGetDeviceDescr
* Description    : 获取设备描述符,返回在TxBuffer中
* Input          : None
* Output         : None
* Return         : ERR_USB_BUF_OVER 描述符长度错误
                   ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlGetDeviceDescr( void )  
{
    UINT8   s;
    UINT8   len;
    UsbDevEndp0Size = DEFAULT_ENDP0_SIZE;
    CopySetupReqPkg( SetupGetDevDescr );
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                      // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    UsbDevEndp0Size = ( (PXUSB_DEV_DESCR)TxBuffer ) -> bMaxPacketSize0;          // 端点0最大包长度,这是简化处理,正常应该先获取前8字节后立即更新UsbDevEndp0Size再继续
    if ( len < ( (PUSB_SETUP_REQ)SetupGetDevDescr ) -> wLengthL )
    {
        return( ERR_USB_BUF_OVER );                                              // 描述符长度错误
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : CtrlGetConfigDescr
* Description    : 获取配置描述符,返回在TxBuffer中
* Input          : None
* Output         : None
* Return         : ERR_USB_BUF_OVER 描述符长度错误
                   ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlGetConfigDescr( void )
{
    UINT8   s;
    UINT8  len;
    CopySetupReqPkg( SetupGetCfgDescr );
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                      // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < ( (PUSB_SETUP_REQ)SetupGetCfgDescr ) -> wLengthL )
    {
        return( ERR_USB_BUF_OVER );                                              // 返回长度错误
    }
    len = ( (PXUSB_CFG_DESCR)TxBuffer ) -> wTotalLengthL;
    CopySetupReqPkg( SetupGetCfgDescr );
    pSetupReq -> wLengthL = len;                                                 // 完整配置描述符的总长度
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                      // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < ( (PUSB_SETUP_REQ)SetupGetCfgDescr ) -> wLengthL || len < ( (PXUSB_CFG_DESCR)TxBuffer ) -> wTotalLengthL )
    {
        return( ERR_USB_BUF_OVER );                                              // 描述符长度错误
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : CtrlSetUsbAddress
* Description    : 设置USB设备地址
* Input          : UINT8 addr 设备地址
* Output         : None
* Return         : ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlSetUsbAddress( UINT8 addr ) 
{
    UINT8   s;
    CopySetupReqPkg( SetupSetUsbAddr );
    pSetupReq -> wValueL = addr;                                                // USB设备地址
    s = HostCtrlTransfer( NULL, (PUINT8)NULL );                                         // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    SetHostUsbAddr( addr );                                                     // 设置USB主机当前操作的USB设备地址
    mDelaymS( 10 );                                                             // 等待USB设备完成操作
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : CtrlSetUsbConfig
* Description    : 设置USB设备配置
* Input          : UINT8 cfg       配置值
* Output         : None
* Return         : ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlSetUsbConfig( UINT8 cfg )                   
{
    CopySetupReqPkg( SetupSetUsbConfig );
    pSetupReq -> wValueL = cfg;                                                // USB设备配置
    return( HostCtrlTransfer( NULL, (PUINT8)NULL ) );                                  // 执行控制传输
}
/*******************************************************************************
* Function Name  : CtrlClearEndpStall
* Description    : 清除端点STALL
* Input          : UINT8 endp       端点地址
* Output         : None
* Return         : ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlClearEndpStall( UINT8 endp )  
{
    CopySetupReqPkg( SetupClrEndpStall );                                      // 清除端点的错误
    pSetupReq -> wIndexL = endp;                                               // 端点地址
    return( HostCtrlTransfer( NULL, (PUINT8)NULL ) );                                  // 执行控制传输
}

#ifndef DISK_BASE_BUF_LEN
/*******************************************************************************
* Function Name  : CtrlSetUsbIntercace
* Description    : 设置USB设备接口
* Input          : UINT8 cfg       配置值
* Output         : None
* Return         : ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlSetUsbIntercace( UINT8 cfg )                   
{
    CopySetupReqPkg( SetupSetUsbInterface );
    pSetupReq -> wValueL = cfg;                                                // USB设备配置
    return( HostCtrlTransfer( NULL, (PUINT8)NULL ) );                                  // 执行控制传输
}

/*******************************************************************************
* Function Name  : CtrlGetHIDDeviceReport
* Description    : 获取HID设备报表描述符,返回在TxBuffer中
* Input          : None
* Output         : None
* Return         : ERR_SUCCESS 成功
                   其他        错误
*******************************************************************************/
UINT8   CtrlGetHIDDeviceReport( void )  
{
    UINT8   s;
    UINT8   len;
    UINT8 tmp[]= {0x21,0x0a,0x00,0x00,0x00,0x00,0x00,0x00};
    for ( s = 0; s != sizeof( tmp ); s ++ )
    {
        ((PUINT8X)pSetupReq)[ s ] = tmp[s];
    }
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                     // 执行控制传输
//    if ( s != ERR_SUCCESS )
//    {
//        return( s );
//    }
    CopySetupReqPkg( SetupGetHIDDevReport );
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                    // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < ( (PUSB_SETUP_REQ)SetupGetDevDescr ) -> wLengthL )
    {
        return( ERR_USB_BUF_OVER );                                            // 描述符长度错误
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : CtrlGetHubDescr
* Description    : 获取HUB描述符,返回在TxBuffer中
* Input          : None
* Output         : None
* Return         : ERR_SUCCESS 成功
                   ERR_USB_BUF_OVER 长度错误
*******************************************************************************/
UINT8   CtrlGetHubDescr( void )  
{
    UINT8   s;
    UINT8D  len;
    CopySetupReqPkg( SetupGetHubDescr );
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                                    // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < ( (PUSB_SETUP_REQ)SetupGetHubDescr ) -> wLengthL )
    {
        return( ERR_USB_BUF_OVER );                                            // 描述符长度错误
    }
//  if ( len < 4 ) return( ERR_USB_BUF_OVER );                                 // 描述符长度错误
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : HubGetPortStatus
* Description    : 查询HUB端口状态,返回在TxBuffer中
* Input          : UINT8 HubPortIndex 
* Output         : None
* Return         : ERR_SUCCESS 成功
                   ERR_USB_BUF_OVER 长度错误
*******************************************************************************/
UINT8   HubGetPortStatus( UINT8 HubPortIndex )   
{
    UINT8   s;
    UINT8  len;
    pSetupReq -> bRequestType = HUB_GET_PORT_STATUS;
    pSetupReq -> bRequest = HUB_GET_STATUS;
    pSetupReq -> wValueL = 0x00;
    pSetupReq -> wValueH = 0x00;
    pSetupReq -> wIndexL = HubPortIndex;
    pSetupReq -> wIndexH = 0x00;
    pSetupReq -> wLengthL = 0x04;
    pSetupReq -> wLengthH = 0x00;
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                           // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < 4 )
    {
        return( ERR_USB_BUF_OVER );                                            // 描述符长度错误
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : HubSetPortFeature
* Description    : 设置HUB端口特性
* Input          : UINT8 HubPortIndex    //HUB端口
                   UINT8 FeatureSelt     //HUB端口特性
* Output         : None
* Return         : ERR_SUCCESS 成功
                   其他        错误
*******************************************************************************/
UINT8   HubSetPortFeature( UINT8 HubPortIndex, UINT8 FeatureSelt ) 
{
    pSetupReq -> bRequestType = HUB_SET_PORT_FEATURE;
    pSetupReq -> bRequest = HUB_SET_FEATURE;
    pSetupReq -> wValueL = FeatureSelt;
    pSetupReq -> wValueH = 0x00;
    pSetupReq -> wIndexL = HubPortIndex;
    pSetupReq -> wIndexH = 0x00;
    pSetupReq -> wLengthL = 0x00;
    pSetupReq -> wLengthH = 0x00;
    return( HostCtrlTransfer( NULL, NULL ) );                                 // 执行控制传输
}
/*******************************************************************************
* Function Name  : HubClearPortFeature
* Description    : 清除HUB端口特性
* Input          : UINT8 HubPortIndex                                         //HUB端口
                   UINT8 FeatureSelt                                          //HUB端口特性
* Output         : None
* Return         : ERR_SUCCESS 成功
                   其他        错误
*******************************************************************************/
UINT8   HubClearPortFeature( UINT8 HubPortIndex, UINT8 FeatureSelt ) 
{
    pSetupReq -> bRequestType = HUB_CLEAR_PORT_FEATURE;
    pSetupReq -> bRequest = HUB_CLEAR_FEATURE;
    pSetupReq -> wValueL = FeatureSelt;
    pSetupReq -> wValueH = 0x00;
    pSetupReq -> wIndexL = HubPortIndex;
    pSetupReq -> wIndexH = 0x00;
    pSetupReq -> wLengthL = 0x00;
    pSetupReq -> wLengthH = 0x00;
    return( HostCtrlTransfer( NULL, NULL ) );                                // 执行控制传输
}

/*******************************************************************************
* Function Name  : CtrlGetXPrinterReport1
* Description    : 打印机类命令
* Input          : None
* Output         : None
* Return         : ERR_USB_BUF_OVER 描述符长度错误
                   ERR_SUCCESS      成功
                   其他
*******************************************************************************/
UINT8   CtrlGetXPrinterReport1( void )  
{
    UINT8   s;
    UINT16   len;
    CopySetupReqPkg( XPrinterReport );
    s = HostCtrlTransfer( TxBuffer, (PUINT8)&len );                         // 执行控制传输
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    if ( len < ( (XPrinterReport[7]<<8)|(XPrinterReport[6]) ))
    {
        return( ERR_USB_BUF_OVER );                                         // 描述符长度错误
    }
    return( ERR_SUCCESS );
}

/*******************************************************************************
* Function Name  : AnalyzeHidIntEndp
* Description    : 从描述符中分析出HID中断端点的地址
* Input          : PUINT8X buf       待分析数据缓冲区地址
* Output         : None
* Return         : 中断端点地址
*******************************************************************************/
UINT8   AnalyzeHidIntEndp( PUINT8X buf ) 
{
    UINT8   i, s, l;
    s = 0;
    for ( i = 0; i < ( (PXUSB_CFG_DESCR)buf ) -> wTotalLengthL; i += l )       // 搜索中断端点描述符,跳过配置描述符和接口描述符
    {
        if ( ( (PXUSB_ENDP_DESCR)(buf+i) ) -> bDescriptorType == USB_DESCR_TYP_ENDP  // 是端点描述符
                && ( ( (PXUSB_ENDP_DESCR)(buf+i) ) -> bmAttributes & USB_ENDP_TYPE_MASK ) == USB_ENDP_TYPE_INTER// 是中断端点
                && ( ( (PXUSB_ENDP_DESCR)(buf+i) ) -> bEndpointAddress & USB_ENDP_DIR_MASK ) )// 是IN端点
        {
            s = ( (PXUSB_ENDP_DESCR)(buf+i) ) -> bEndpointAddress & USB_ENDP_ADDR_MASK;// 中断端点的地址
            break;                                                             // 可以根据需要保存wMaxPacketSize和bInterval
        }
        l = ( (PXUSB_ENDP_DESCR)(buf+i) ) -> bLength;                          // 当前描述符长度,跳过
        if ( l > 16 )
        {
            break;
        }
    }
    return( s );
}
/*******************************************************************************
* Function Name  : InitRootDevice
* Description    : 初始化指定ROOT-HUB端口的USB设备
* Input          : UINT8 RootHubIndex 指定端口，内置HUB端口号0/1
* Output         : None
* Return         :
*******************************************************************************/
UINT8   InitRootDevice( UINT8 RootHubIndex ) 
{
    UINT8   i, s, cfg, dv_cls, if_cls;
#if DE_PRINTF	
    printf( "Reset root hub %1d# port\n", (UINT16)RootHubIndex );
#endif
    ResetRootHubPort( );                                                    // 检测到设备后,复位相应端口的USB总线
//     mDelaymS( 10 );
    for ( i = 0, s = 0; i < 100; i ++ )                                     // 等待USB设备复位后重新连接,100mS超时
    {
        mDelaymS( 1 );
        if ( EnableRootHubPort( ) == ERR_SUCCESS )                          // 使能ROOT-HUB端口
        {
            i = 0;
            s ++;                                                           // 计时等待USB设备连接后稳定
            if ( s > 20 )
            {
                break;                                                      // 已经稳定连接15mS
            }
        }
    }	
    if ( i )                                                                 // 复位后设备没有连接
    {
        DisableRootHubPort( );
#if DE_PRINTF			
        printf( "Disable root hub %1d# port because of disconnect\n", (UINT16)RootHubIndex );
#endif	
        return( ERR_USB_DISCON );
    }
    SelectHubPort( 0 );
#if DE_PRINTF		
    printf( "GetDevDescr: " );
#endif
    s = CtrlGetDeviceDescr( );                                               // 获取设备描述符
    if ( s == ERR_SUCCESS )
    {
#if DE_PRINTF	
        for ( i = 0; i < ( (PUSB_SETUP_REQ)SetupGetDevDescr ) -> wLengthL; i ++ )
        {				
            printf( "x%02X ", (UINT16)( TxBuffer[i] ) );				
        }
        printf( "\n" );                                                       // 显示出描述符
#endif				
        dv_cls = ( (PXUSB_DEV_DESCR)TxBuffer ) -> bDeviceClass;               // 设备类代码
        s = CtrlSetUsbAddress( RootHubIndex + ( (PUSB_SETUP_REQ)SetupSetUsbAddr ) -> wValueL );// 设置USB设备地址,加上RootHubIndex可以保证2个HUB端口分配不同的地址
        if ( s == ERR_SUCCESS )
        {
            ThisUsbDev.DeviceAddress = RootHubIndex + ( (PUSB_SETUP_REQ)SetupSetUsbAddr ) -> wValueL;  // 保存USB地址
#if DE_PRINTF						
            printf( "GetCfgDescr: " );
#endif					
            s = CtrlGetConfigDescr( );                                        // 获取配置描述符
            if ( s == ERR_SUCCESS )
            {
                cfg = ( (PXUSB_CFG_DESCR)TxBuffer ) -> bConfigurationValue;
#if DE_PRINTF							
                for ( i = 0; i < ( (PXUSB_CFG_DESCR)TxBuffer ) -> wTotalLengthL; i ++ )
                {
                    printf( "x%02X ", (UINT16)( TxBuffer[i] ) );
                }
                printf("\n");
#endif								
                                                                              //分析配置描述符,获取端点数据/各端点地址/各端点大小等,更新变量endp_addr和endp_size等
                if_cls = ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceClass;  // 接口类代码
                if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_STORAGE )      // 是USB存储类设备,基本上确认是U盘
                {
                    s = CtrlSetUsbConfig( cfg );                              // 设置USB设备配置
                    if ( s == ERR_SUCCESS )
                    {
                        ThisUsbDev.DeviceStatus = ROOT_DEV_SUCCESS;
                        ThisUsbDev.DeviceType = USB_DEV_CLASS_STORAGE;
#if DE_PRINTF												
                        printf( "USB-Disk Ready\n" );
#endif											
                        SetUsbSpeed( 1 );                                     // 默认为全速
                        return( ERR_SUCCESS );
                    }
                }
                else if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_PRINTER && ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceSubClass == 0x01 )// 是打印机类设备
                {
#if DE_PRINTF										
                    printf( "USB-Print OK\n" );
#endif									
                    if((TxBuffer[19] == 5)&&(TxBuffer[20]&&0x80)){
                       ThisUsbDev.GpVar = TxBuffer[20];                     //IN 端点											
                    }
                    else if((TxBuffer[19] == 5)&&((TxBuffer[20]&&0x80) == 0)){
                       ThisUsbDev.GpVar1 = TxBuffer[20];                    //OUT 端点												
                    }		
                    if((TxBuffer[26] == 5)&&(TxBuffer[20]&&0x80)){
                       ThisUsbDev.GpVar = TxBuffer[27];                     //IN 端点											
                    }
                    else if((TxBuffer[26] == 5)&&((TxBuffer[20]&&0x80) == 0)){
                       ThisUsbDev.GpVar1 = TxBuffer[27];                    //OUT 端点												
                    }										
//                  ThisUsbDev.GpVar = ( (PUSB_CFG_DESCR_LONG)TxBuffer ) -> endp_descr[0].bEndpointAddress;// 保存批量传输端点
                    s = CtrlSetUsbConfig( cfg );                            // 设置USB设备配置
                    if ( s == ERR_SUCCESS )
                    {									
                        s = CtrlSetUsbIntercace(cfg);
//                         if(s == ERR_SUCCESS){
                                                                           //需保存端点信息以便主程序进行USB传输
                         s = CtrlGetXPrinterReport1( );                    //打印机类命令
                         if(s == ERR_SUCCESS){													 
                           ThisUsbDev.DeviceStatus = ROOT_DEV_SUCCESS;
                           ThisUsbDev.DeviceType = USB_DEV_CLASS_PRINTER;
#if DE_PRINTF														 
                           printf( "USB-Print Ready\n" );
#endif													 
                           SetUsbSpeed( 1 );                               // 默认为全速
                           return( ERR_SUCCESS );
												 }													 
//                         }
                    }
                }
                else if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_HID && ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceSubClass <= 0x01 )// 是HID类设备,键盘/鼠标等
                { 
                    s = AnalyzeHidIntEndp( TxBuffer );                    // 从描述符中分析出HID中断端点的地址
                    ThisUsbDev.GpVar = s & USB_ENDP_ADDR_MASK ;           // 保存中断端点的地址,位7用于同步标志位,清0
                    if_cls = ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceProtocol;
                    s = CtrlSetUsbConfig( cfg );                          // 设置USB设备配置
                    if ( s == ERR_SUCCESS )
                    {
#if DE_PRINTF												
                        printf( "GetHIDReport: " );
#endif											
                        s = CtrlGetHIDDeviceReport();                    //获取报表描述符
                        if(s == ERR_SUCCESS)
                        {
#if DE_PRINTF														
                            for ( i = 0; i < 64; i++ )
                            {
                                printf( "x%02X ", (UINT16)( TxBuffer[i] ) );
                            }
                            printf("\n");
#endif														
                        }
                        //Set_Idle( );
                                                                         //需保存端点信息以便主程序进行USB传输
                        ThisUsbDev.DeviceStatus = ROOT_DEV_SUCCESS;
                        if ( if_cls == 1 )
                        {
                            ThisUsbDev.DeviceType = DEV_TYPE_KEYBOARD;
                                                                         //进一步初始化,例如设备键盘指示灯LED等
#if DE_PRINTF														
                            printf( "USB-Keyboard Ready\n" );
#endif													
                            SetUsbSpeed( 1 );                            // 默认为全速
                            return( ERR_SUCCESS );
                        }
                        else if ( if_cls == 2 )
                        {
                            ThisUsbDev.DeviceType = DEV_TYPE_MOUSE;
                                                                         //为了以后查询鼠标状态,应该分析描述符,取得中断端口的地址,长度等信息
#if DE_PRINTF													
                            printf( "USB-Mouse Ready\n" );
#endif													
                            SetUsbSpeed( 1 );                            // 默认为全速
                            return( ERR_SUCCESS );
                        }
                        s = ERR_USB_UNSUPPORT;
                    }
                }
                else if ( dv_cls == USB_DEV_CLASS_HUB )                   // 是HUB类设备,集线器等
                {
                    s = AnalyzeHidIntEndp( TxBuffer );                    // 从描述符中分析出HID中断端点的地址
                    ThisUsbDev.GpVar1 = s & USB_ENDP_ADDR_MASK ;          // 保存中断端点的地址,位7用于同步标志位,清0
#if DE_PRINTF										
                    printf( "GetHubDescr: ");
#endif									
                    s = CtrlGetHubDescr( );
                    if ( s == ERR_SUCCESS )
                    {
#if DE_PRINTF												
                        for( i = 0; i < TxBuffer[0]; i++ )
                        {
                            printf( "x%02X ",(UINT16)(TxBuffer[i]) );
                        }                       
												printf("\n");
#endif												
                        ThisUsbDev.GpVar = ( (PXUSB_HUB_DESCR)TxBuffer ) -> bNbrPorts;// 保存HUB的端口数量
                        if ( ThisUsbDev.GpVar > HUB_MAX_PORTS )
                        {
                            ThisUsbDev.GpVar = HUB_MAX_PORTS;// 因为定义结构DevOnHubPort时人为假定每个HUB不超过HUB_MAX_PORTS个端口
                        }
                        //if ( ( (PXUSB_HUB_DESCR)TxBuffer ) -> wHubCharacteristics[0] & 0x04 ) printf("带有集线器的复合设备\n");
                        //else printf("单一的集线器产品\n");
                        s = CtrlSetUsbConfig( cfg );                     // 设置USB设备配置
                        if ( s == ERR_SUCCESS )
                        {
                            ThisUsbDev.DeviceStatus = ROOT_DEV_SUCCESS;
                            ThisUsbDev.DeviceType = USB_DEV_CLASS_HUB;
                            //需保存端点信息以便主程序进行USB传输,本来中断端点可用于HUB事件通知,但本程序使用查询状态控制传输代替
                            //给HUB各端口上电,查询各端口状态,初始化有设备连接的HUB端口,初始化设备
                            for ( i = 1; i <= ThisUsbDev.GpVar; i ++ )// 给HUB各端口都上电
                            {
                                DevOnHubPort[i-1].DeviceStatus = ROOT_DEV_DISCONNECT;  // 清外部HUB端口上设备的状态
                                s = HubSetPortFeature( i, HUB_PORT_POWER );
                                if ( s != ERR_SUCCESS )
                                {
#if DE_PRINTF																		
                                    printf( "Ext-HUB Port_%1d# power on error\n",(UINT16)i );// 端口上电失败
#endif																	
                                }
                            }
                            for ( i = 1; i <= ThisUsbDev.GpVar; i ++ )         // 清除HUB各端口连接状态
                            {
                                s = HubClearPortFeature( i, HUB_C_PORT_CONNECTION );
                                if ( s != ERR_SUCCESS )
                                {
#if DE_PRINTF																		
                                    printf( "Ext-HUB Port_%1d#  clear connection error\n",(UINT16)i );// 端口连接状态清除失败
#endif																	
                                }
                            }  
														for ( i = 1; i <= ThisUsbDev.GpVar; i ++ )            // 查询HUB各端口连接状态
														{	
															s = HubGetPortStatus( i );                           // 获取端口状态
															if ( s != ERR_SUCCESS ) 
#if DE_PRINTF																	
																printf( "Ext-HUB Port_%1d#	clear connection error\n",(UINT16)i );	// 端口连接状态清除失败
#endif															
															if ( ( TxBuffer[0] != 0 ) && ( DevOnHubPort[i-1].DeviceStatus != ROOT_DEV_SUCCESS) ){ //HUB下游端口已有设备连接
															s = HubSetPortFeature( i, HUB_PORT_RESET );  // 对有设备连接的端口复位
															if ( s != ERR_SUCCESS ) 
															return s;  // 可能是该HUB断开了
															do // 查询复位端口,直到复位完成,把完成后的状态显示出来
															{
																mDelaymS( 1 );
																s = HubGetPortStatus( i );
																if ( s != ERR_SUCCESS ) return s;  // 可能是该HUB断开了
															} while ( TxBuffer[0] & (1<<(HUB_PORT_RESET&0x07)) );  // 端口正在复位则等待							
														 }
														}														
                            SetUsbSpeed( 1 );                                        // 默认为全速
                            return( ERR_SUCCESS );
                        }
                    }
                }
                else                                                                 // 可以进一步分析
                {
                    s = CtrlSetUsbConfig( cfg );                                     // 设置USB设备配置
                    if ( s == ERR_SUCCESS ) 
                    {
                        //需保存端点信息以便主程序进行USB传输
                        ThisUsbDev.DeviceStatus = ROOT_DEV_SUCCESS;
                        ThisUsbDev.DeviceStatus = dv_cls ? dv_cls : if_cls;
                        SetUsbSpeed( 1 );                                            // 默认为全速
                        return( ERR_SUCCESS );                                       // 未知设备初始化成功
                    }
                }
            }
        }
    }
#if DE_PRINTF			
    printf( "InitRootDev Err = %02X\n", (UINT16)s );
#endif		
    ThisUsbDev.DeviceStatus = ROOT_DEV_FAILED;
    SetUsbSpeed( 1 );                                                                 // 默认为全速
    return( s );
}
/*******************************************************************************
* Function Name  : EnumAllRootDevice
* Description    : 枚举所有ROOT-HUB端口的USB设备
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
UINT8   EnumAllRootDevice( void )   
{
    UINT8I   s, RootHubIndex;
#if DE_PRINTF	
    printf( "EnumAllRootDev\n" );
#endif
    for ( RootHubIndex = 0; RootHubIndex < 2; RootHubIndex ++ )
    {
#if DE_PRINTF			
        printf( "RootHubIndex %02x\n",(UINT16)RootHubIndex );
#endif		
        if ( ThisUsbDev.DeviceStatus == ROOT_DEV_CONNECTED )                        // 刚插入设备尚未初始化
        {
            s = InitRootDevice( RootHubIndex );                                      // 初始化/枚举指定HUB端口的USB设备
            if ( s != ERR_SUCCESS )
            {
                return( s );
            }
        }
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : InitDevOnHub
* Description    : 初始化枚举外部HUB后的二级USB设备
* Input          : UINT8 HubPortIndex  指定外部HUB
* Output         : None
* Return         : ERR_SUCCESS 成功
                   ERR_USB_UNKNOWN 未知设备                 
*******************************************************************************/
UINT8   InitDevOnHub( UINT8 HubPortIndex ) 
{
    UINT8   i, s, cfg, dv_cls, if_cls;
#if DE_PRINTF		
    printf( "Init dev @ExtHub-port_%1d ", (UINT16)HubPortIndex );
#endif
    if ( HubPortIndex == 0 )
    {
        return( ERR_USB_UNKNOWN );
    }
    SelectHubPort( HubPortIndex );                                      // 选择操作指定的ROOT-HUB端口的外部HUB的指定端口,选择速度
#if DE_PRINTF		
    printf( "GetDevDescr: " );
#endif
    s = CtrlGetDeviceDescr( );                                          // 获取设备描述符
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    dv_cls = ( (PXUSB_DEV_DESCR)TxBuffer ) -> bDeviceClass;             // 设备类代码
    cfg = ( 1<<4 ) + HubPortIndex;                                      // 计算出一个USB地址,避免地址重叠
    s = CtrlSetUsbAddress( cfg );                                       // 设置USB设备地址
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    DevOnHubPort[HubPortIndex-1].DeviceAddress = cfg;                   // 保存分配的USB地址
#if DE_PRINTF			
    printf( "GetCfgDescr: " );
#endif
    s = CtrlGetConfigDescr( );                                          // 获取配置描述符
    if ( s != ERR_SUCCESS )
    {
        return( s );
    }
    cfg = ( (PXUSB_CFG_DESCR)TxBuffer ) -> bConfigurationValue;
#if DE_PRINTF			
    for ( i = 0; i < ( (PXUSB_CFG_DESCR)TxBuffer ) -> wTotalLengthL; i ++ )
    {
        printf( "x%02X ", (UINT16)( TxBuffer[i] ) );
    }
    printf("\n");
#endif		
    /* 分析配置描述符,获取端点数据/各端点地址/各端点大小等,更新变量endp_addr和endp_size等 */
    if_cls = ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceClass; // 接口类代码
    if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_STORAGE )                  // 是USB存储类设备,基本上确认是U盘
    {
        s = CtrlSetUsbConfig( cfg );                                          // 设置USB设备配置
        if ( s == ERR_SUCCESS )
        {
            DevOnHubPort[HubPortIndex-1].DeviceStatus = ROOT_DEV_SUCCESS;
            DevOnHubPort[HubPortIndex-1].DeviceType = USB_DEV_CLASS_STORAGE;
#if DE_PRINTF						
            printf( "USB-Disk Ready\n" );
#endif					
            SetUsbSpeed( 1 );                                                 // 默认为全速
            return( ERR_SUCCESS );
        }
    }
    else if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_HID && ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceSubClass <= 0x01 )    // 是HID类设备,键盘/鼠标等
    {
        s = AnalyzeHidIntEndp( TxBuffer );                                     // 从描述符中分析出HID中断端点的地址
        DevOnHubPort[HubPortIndex-1].GpVar = s;                                // 保存中断端点的地址,位7用于同步标志位,清0
        if_cls = ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceProtocol;
        s = CtrlSetUsbConfig( cfg );                                           // 设置USB设备配置
        if ( s == ERR_SUCCESS )
        {
            //需保存端点信息以便主程序进行USB传输
            DevOnHubPort[HubPortIndex-1].DeviceStatus = ROOT_DEV_SUCCESS;
            if ( if_cls == 1 )
            {
                DevOnHubPort[HubPortIndex-1].DeviceType = DEV_TYPE_KEYBOARD;
                //进一步初始化,例如设备键盘指示灯LED等
#if DE_PRINTF								
                printf( "USB-Keyboard Ready\n" );
#endif						
                SetUsbSpeed( 1 );                                               // 默认为全速
                return( ERR_SUCCESS );
            }
            else if ( if_cls == 2 )
            {
                DevOnHubPort[HubPortIndex-1].DeviceType = DEV_TYPE_MOUSE;
#if DE_PRINTF	                                                                  //为了以后查询鼠标状态,应该分析描述符,取得中断端口的地址,长度等信息
                printf( "USB-Mouse Ready\n" );
#endif							
                SetUsbSpeed( 1 );                                               // 默认为全速
                return( ERR_SUCCESS );
            }
            s = ERR_USB_UNSUPPORT;
        }
    }
    else if ( dv_cls == USB_DEV_CLASS_HUB )                                     // 是HUB类设备,集线器等
    {
        DevOnHubPort[HubPortIndex-1].DeviceType = USB_DEV_CLASS_HUB;
#if DE_PRINTF				
        printf( "This program don't support Level 2 HUB\n");                    // 需要支持多级HUB级联请参考本程序进行扩展
#endif		
        s = HubClearPortFeature( i, HUB_PORT_ENABLE );                          // 禁止HUB端口
        if ( s != ERR_SUCCESS )
        {
            return( s );
        }
        s = ERR_USB_UNSUPPORT;
    }
    else                                                                         // 可以进一步分析
    {
        s = CtrlSetUsbConfig( cfg );                                             // 设置USB设备配置
        if ( s == ERR_SUCCESS )
        {
            //需保存端点信息以便主程序进行USB传输
            DevOnHubPort[HubPortIndex-1].DeviceStatus = ROOT_DEV_SUCCESS;
            DevOnHubPort[HubPortIndex-1].DeviceType = dv_cls ? dv_cls : if_cls;
            SetUsbSpeed( 1 );                                                    // 默认为全速
            return( ERR_SUCCESS );                                               //未知设备初始化成功
        }
    }
#if DE_PRINTF			
    printf( "InitDevOnHub Err = %02X\n", (UINT16)s );
#endif		
    DevOnHubPort[HubPortIndex-1].DeviceStatus = ROOT_DEV_FAILED;
    SetUsbSpeed( 1 );                                                            // 默认为全速
    return( s );
}
/*******************************************************************************
* Function Name  : EnumHubPort
* Description    : 枚举指定ROOT-HUB端口上的外部HUB集线器的各个端口,检查各端口有无连接或移除事件并初始化二级USB设备
* Input          : UINT8 RootHubIndex ROOT_HUB0和ROOT_HUB1
* Output         : None
* Return         : ERR_SUCCESS 成功
                   其他        失败
*******************************************************************************/
UINT8   EnumHubPort( ) 
{
    UINT8   i, s;
#if DE_PRINTF		
    printf( "EnumHubPort\n" );
#endif	
    for ( i = 1; i <= ThisUsbDev.GpVar; i ++ )                                       // 查询集线器的端口是否有变化
    {
        SelectHubPort( 0 );                                                          // 选择操作指定的ROOT-HUB端口,设置当前USB速度以及被操作设备的USB地址
        s = HubGetPortStatus( i );                                                   // 获取端口状态
        if ( s != ERR_SUCCESS )
        {
            return( s );                                                              // 可能是该HUB断开了
        }
        if ( (( TxBuffer[0]&(1<<(HUB_PORT_CONNECTION&0x07)) ) && ( TxBuffer[2]&(1<<(HUB_C_PORT_CONNECTION&0x07)) ))||(TxBuffer[2] == 0x10) ) 
        {// 发现有设备连接
            DevOnHubPort[i-1].DeviceStatus = ROOT_DEV_CONNECTED;                     // 有设备连接
            DevOnHubPort[i-1].DeviceAddress = 0x00;
            s = HubGetPortStatus( i );                                               // 获取端口状态
            if ( s != ERR_SUCCESS )
            {
                return( s );                                                         // 可能是该HUB断开了
            }
            DevOnHubPort[i-1].DeviceSpeed = TxBuffer[1] & (1<<(HUB_PORT_LOW_SPEED&0x07)) ? 0 : 1;// 低速还是全速
            if ( DevOnHubPort[i-1].DeviceSpeed )
            {
#if DE_PRINTF								
                printf( "Found full speed device on port %1d\n", (UINT16)i );
#endif							
            }
            else
            {
#if DE_PRINTF								
                printf( "Found low speed device on port %1d\n", (UINT16)i );
#endif							
            }
            mDelaymS( 200 );                                                          // 等待设备上电稳定
            s = HubSetPortFeature( i, HUB_PORT_RESET );                               // 对有设备连接的端口复位
            if ( s != ERR_SUCCESS )
            {
                return( s );                                                          // 可能是该HUB断开了
            }
#if DE_PRINTF							
            printf( "Reset port and then wait in\n" );
#endif						
            do                                                                        // 查询复位端口,直到复位完成,把完成后的状态显示出来
            {
                mDelaymS( 1 );
                s = HubGetPortStatus( i );
                if ( s != ERR_SUCCESS )
                {
                    return( s );                                                      // 可能是该HUB断开了
                }
            }
            while ( TxBuffer[0] & (1<<(HUB_PORT_RESET&0x07)) );                       // 端口正在复位则等待
            mDelaymS( 100 );
            s = HubClearPortFeature( i, HUB_C_PORT_RESET );                           // 清除复位完成标志
//             s = HubSetPortFeature( i, HUB_PORT_ENABLE );                              // 启用HUB端口
            s = HubClearPortFeature( i, HUB_C_PORT_CONNECTION );                      // 清除连接或移除变化标志
            if ( s != ERR_SUCCESS )
            {
                return( s );
            }
            s = HubGetPortStatus( i );                                                // 再读取状态,复查设备是否还在
            if ( s != ERR_SUCCESS )
            {
                return( s );
            }
            if ( ( TxBuffer[0]&(1<<(HUB_PORT_CONNECTION&0x07)) ) == 0 )
            {
                DevOnHubPort[i-1].DeviceStatus = ROOT_DEV_DISCONNECT;                 // 设备不在了
            }
            s = InitDevOnHub( i );                                                    // 初始化二级USB设备
            if ( s != ERR_SUCCESS )
            {
                return( s );
            }
            SetUsbSpeed( 1 );                                                         // 默认为全速
        }
				else if (TxBuffer[2]&(1<<(HUB_C_PORT_ENABLE&0x07))  )                         // 设备连接出错
				{
						HubClearPortFeature( i, HUB_C_PORT_ENABLE );                              // 清除连接错误标志
#if DE_PRINTF						
					  printf( "Device on port error\n" );		
#endif					
				  	s = HubSetPortFeature( i, HUB_PORT_RESET );                               // 对有设备连接的端口复位
					  if ( s != ERR_SUCCESS ) 
						return( s );                                                              // 可能是该HUB断开了
					  do                                                                        // 查询复位端口,直到复位完成,把完成后的状态显示出来
					  {
						  mDelaymS( 1 );
						  s = HubGetPortStatus( i );
						  if ( s != ERR_SUCCESS ) return( s );                                    // 可能是该HUB断开了
					  } while ( TxBuffer[0] & (1<<(HUB_PORT_RESET&0x07)) );                     // 端口正在复位则等待
				}
        else if ( ( TxBuffer[0]&(1<<(HUB_PORT_CONNECTION&0x07)) ) == 0 )              // 设备已经断开
        {
            if ( DevOnHubPort[i-1].DeviceStatus >= ROOT_DEV_CONNECTED )
            {
#if DE_PRINTF								
                printf( "Device on port %1d removed\n", (UINT16)i );
#endif							
            }
            DevOnHubPort[i-1].DeviceStatus = ROOT_DEV_DISCONNECT;                     // 有设备连接
            if ( TxBuffer[2]&(1<<(HUB_C_PORT_CONNECTION&0x07)) )
            {
                HubClearPortFeature( i, HUB_C_PORT_CONNECTION );                      // 清除移除变化标志
            }
        }
    }
    return( ERR_SUCCESS );                                                            // 返回操作成功
}
/*******************************************************************************
* Function Name  : EnumAllHubPort
* Description    : 枚举所有ROOT-HUB端口下外部HUB后的二级USB设备
* Input          : None
* Output         : None
* Return         : ERR_SUCCESS 成功
                   其他        失败
*******************************************************************************/
UINT8   EnumAllHubPort( void ) 
{
    UINT8   s, RootHubIndex;
#if DE_PRINTF		
    printf( "EnumAllHubPort\n" );
#endif	
    for ( RootHubIndex = 0; RootHubIndex != 2; RootHubIndex ++ )
    {
        if ( ThisUsbDev.DeviceStatus >= ROOT_DEV_SUCCESS && ThisUsbDev.DeviceType == USB_DEV_CLASS_HUB )// HUB枚举成功
        { 
            SelectHubPort( 0 );                                                        // 选择操作指定的ROOT-HUB端口,设置当前USB速度以及被操作设备的USB地址
            //做点什么?  给HUB各端口上电,查询各端口状态,初始化有设备连接的HUB端口,初始化设备
//             for ( i = 1; i <= ThisUsbDev.GpVar; i ++ ){                             // 初始化HUB各端口
//               s = HubSetPortFeature( i, HUB_PORT_POWER );                           // 给HUB各端口上电
//               if ( s != ERR_SUCCESS )
//               {
//                 return( s );                                                        // 可能是该HUB断开了
//               }             							
//             }				
            s = EnumHubPort( );                                                        // 枚举指定ROOT-HUB端口上的外部HUB集线器的各个端口,检查各端口有无连接或移除事件
            if ( s != ERR_SUCCESS )                                                    // 可能是HUB断开了
            {
#if DE_PRINTF								
                printf( "EnumAllHubPort err = %02X\n", (UINT16)s );
#endif							
            }
            SetUsbSpeed( 1 );                                                          // 默认为全速
        }
    }
    return( ERR_SUCCESS );
}
/*******************************************************************************
* Function Name  : SearchTypeDevice
* Description    : 在ROOT-HUB以及外部HUB各端口上搜索指定类型的设备所在的端口号,输出端口号为0xFFFF则未搜索到
* Input          : UINT8 type 搜索的设备类型
* Output         : None
* Return         : 输出高8位为ROOT-HUB端口号,低8位为外部HUB的端口号,低8位为0则设备直接在ROOT-HUB端口上
                   当然也可以根据USB的厂商VID产品PID进行搜索(事先要记录各设备的VID和PID),以及指定搜索序号
*******************************************************************************/
UINT16  SearchTypeDevice( UINT8 type )   
{

    UINT8   RootHubIndex, HubPortIndex;
    for ( RootHubIndex = 0; RootHubIndex != 2; RootHubIndex ++ )                      // 现时搜索可以避免设备中途拔出而某些信息未及时更新的问题
    {
        if ( ThisUsbDev.DeviceType == type && ThisUsbDev.DeviceStatus >= ROOT_DEV_SUCCESS )
        {
            return( (UINT16)RootHubIndex << 8 );                                      // 类型匹配且枚举成功,在ROOT-HUB端口上
        }
        if ( ThisUsbDev.DeviceType == USB_DEV_CLASS_HUB && ThisUsbDev.DeviceStatus >= ROOT_DEV_SUCCESS )// 外部集线器HUB且枚举成功
        {
            for ( HubPortIndex = 1; HubPortIndex <= ThisUsbDev.GpVar; HubPortIndex ++ )// 搜索外部HUB的各个端口
            {
                if ( DevOnHubPort[HubPortIndex-1].DeviceType == type && DevOnHubPort[HubPortIndex-1].DeviceStatus >= ROOT_DEV_SUCCESS )
                {
                    return( ( (UINT16)RootHubIndex << 8 ) | HubPortIndex );           // 类型匹配且枚举成功
                }
            }
        }
    }
    return( 0xFFFF );
}
/*******************************************************************************
* Function Name  : SETorOFFNumLock
* Description    : NumLock的点灯判断
* Input          : PUINT8 buf 点灯键值
* Output         : None
* Return         : None
*******************************************************************************/
UINT8 SETorOFFNumLock(PUINT8 buf)
{
    UINT8 tmp[]= {0x21,0x09,0x00,0x02,0x00,0x00,0x01,0x00};
    UINT8 len,s;
    if((buf[2]==0x53)&(buf[0]|buf[1]|buf[3]|buf[4]|buf[5]|buf[6]|buf[7]==0))
    {
        for ( s = 0; s != sizeof( tmp ); s ++ )
        {
            ((PUINT8X)pSetupReq)[ s ] = tmp[s];
        }
        s = HostCtrlTransfer( TxBuffer, &len );                                     // 执行控制传输
        if ( s != ERR_SUCCESS )
        {
            return( s );
        }
    }
    return( ERR_SUCCESS );
}
#endif

#ifdef DISK_BASE_BUF_LEN
UINT8	InitRootDevice( void )                                                       // 初始化USB设备
{
	UINT8	i, s, cfg, dv_cls, if_cls;
#if DE_PRINTF	
	printf( "Reset host port\n" );
#endif
	ResetRootHubPort( );                                                            // 检测到设备后,复位相应端口的USB总线
	for ( i = 0, s = 0; i < 100; i ++ ) {                                           // 等待USB设备复位后重新连接,100mS超时
		mDelaymS( 1 );
		if ( EnableRootHubPort( ) == ERR_SUCCESS ) {                                  // 使能端口
			i = 0;
			s ++;                                                                       // 计时等待USB设备连接后稳定
			if ( s > 100 ) break;                                                       // 已经稳定连接100mS
		}
	}
	if ( i ) {                                                                      // 复位后设备没有连接
		DisableRootHubPort( );
#if DE_PRINTF			
		printf( "Disable host port because of disconnect\n" );
#endif		
		return( ERR_USB_DISCON );
	}
	SetUsbSpeed( 1 );                                                              // 设置当前USB速度
	s = CtrlGetDeviceDescr( );                                                     // 获取设备描述符
	if ( s == ERR_SUCCESS ) {
#if DE_PRINTF			
	  printf( "GetDevDescr: " );		
		for ( i = 0; i < ( (PUSB_SETUP_REQ)SetupGetDevDescr ) -> wLengthL; i ++ ) printf( "x%02X ", (UINT16)( TxBuffer[i] ) );
		printf( "\n" );                                                             // 显示出描述符
#endif		
		dv_cls = ( (PXUSB_DEV_DESCR)TxBuffer ) -> bDeviceClass;                     // 设备类代码
		s = CtrlSetUsbAddress( ( (PUSB_SETUP_REQ)SetupSetUsbAddr ) -> wValueL );    // 设置USB设备地址
		if ( s == ERR_SUCCESS ) {

			s = CtrlGetConfigDescr( );                                                // 获取配置描述符
			if ( s == ERR_SUCCESS ) {
				cfg = ( (PXUSB_CFG_DESCR)TxBuffer ) -> bConfigurationValue;
#if DE_PRINTF					
			  printf( "GetCfgDescr: " );				
				for ( i = 0; i < ( (PXUSB_CFG_DESCR)TxBuffer ) -> wTotalLengthL; i ++ ) printf( "x%02X ", (UINT16)( TxBuffer[i] ) );
				printf("\n");
#endif				
/* 分析配置描述符,获取端点数据/各端点地址/各端点大小等,更新变量endp_addr和endp_size等 */
				if_cls = ( (PXUSB_CFG_DESCR_LONG)TxBuffer ) -> itf_descr.bInterfaceClass; // 接口类代码
				if ( dv_cls == 0x00 && if_cls == USB_DEV_CLASS_STORAGE ) {                // 是USB存储类设备,基本上确认是U盘
					CH554DiskStatus = DISK_USB_ADDR;
					return( ERR_SUCCESS );
				}
				else {
					return( ERR_USB_UNSUPPORT );
				}
			}
		}
	}
#if DE_PRINTF		
	printf( "InitRootDev Err = %02X\n", (UINT16)s );
#endif
	CH554DiskStatus = DISK_CONNECT;
	SetUsbSpeed( 1 );                                                              // 默认为全速
	return( s );
}
#endif
/*******************************************************************************
* Function Name  : InitUSB_Host
* Description    : 初始化USB主机
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void    InitUSB_Host( void )
{
    UINT8   i;
    IE_USB = 0;
//  LED_CFG = 1;
//  LED_RUN = 0;
    USB_CTRL = bUC_HOST_MODE;                                                    // 先设定模式
    USB_DEV_AD = 0x00;
    UH_EP_MOD = bUH_EP_TX_EN | bUH_EP_RX_EN ;
    UH_RX_DMA = RxBuffer;
    UH_TX_DMA = TxBuffer;
    UH_RX_CTRL = 0x00;
    UH_TX_CTRL = 0x00;
    USB_CTRL = bUC_HOST_MODE | bUC_INT_BUSY | bUC_DMA_EN;                        // 启动USB主机及DMA,在中断标志未清除前自动暂停
//  UHUB0_CTRL = 0x00;
//  UHUB1_CTRL = 0x00;
    UH_SETUP = bUH_SOF_EN;
    USB_INT_FG = 0xFF;                                                           // 清中断标志
    for ( i = 0; i != 2; i ++ )
    {
        DisableRootHubPort( );                                                   // 清空
    }
    USB_INT_EN = bUIE_TRANSFER | bUIE_DETECT;
//  IE_USB = 1;                                                                  // 查询方式
}
