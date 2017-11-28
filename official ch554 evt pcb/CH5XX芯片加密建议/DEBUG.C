/********************************** (C) COPYRIGHT *******************************
* File Name          : DEBUG.C
* Author             : WCH
* Version            : V1.3
* Date               : 2016/06/24
* Description        : CH559 DEBUG Interface
                     (1)、串口0输出打印信息，波特率可变;              				   
*******************************************************************************/

#include <stdio.h>
#include "CH559.H"

#define	 FREQ_SYS	12000000	                                                   //系统主频12MHz
#ifndef  BUAD
#define  BUAD    57600
#endif

/*******************************************************************************
* Function Name  : CfgFsys( )
* Description    : CH559时钟选择和配置函数,默认使用内部晶振12MHz，如果定义了FREQ_SYS可以
                   根据PLL_CFG和CLOCK_CFG配置得到，公式如下：
                   Fsys = (Fosc * ( PLL_CFG & MASK_PLL_MULT ))/(CLOCK_CFG & MASK_SYS_CK_DIV);
                   具体时钟需要自己配置
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/ 
void	CfgFsys( )  
{
    SAFE_MOD = 0x55;                                                           //开启安全模式
    SAFE_MOD = 0xAA;                                                 
//  CLOCK_CFG |= bOSC_EN_XT;                                                   //使能外部晶振                                         
//  CLOCK_CFG &= ~bOSC_EN_INT;                                              
// 	CLOCK_CFG &= ~MASK_SYS_CK_DIV;
//  CLOCK_CFG |= 6;                                                            //配置系统时钟48MHz
//  CLOCK_CFG |= 8;                                                            //配置系统时钟36MHz
//  CLOCK_CFG |= 10;                                                           //配置系统时钟28.8MHz
//  CLOCK_CFG |= 12;                                                           //配置系统时钟24MHz
//  CLOCK_CFG |= 16;                                                           //配置系统时钟18MHz  	
    SAFE_MOD = 0xFF;                                                           //关闭安全模式  
//  如果修改主频，要同时修改FREQ_SYS，否则会造成延时函数不准
}

/*******************************************************************************
* Function Name  : mDelayus(UNIT16 n)
* Description    : us延时函数
* Input          : UNIT16 n
* Output         : None
* Return         : None
*******************************************************************************/ 
void	mDelayuS( UINT16 n )  // 以uS为单位延时
{
	while ( n ) {  // total = 12~13 Fsys cycles, 1uS @Fsys=12MHz
		++ SAFE_MOD;  // 2 Fsys cycles, for higher Fsys, add operation here
#ifdef	FREQ_SYS
#if		FREQ_SYS >= 14000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 16000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 18000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 20000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 22000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 24000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 26000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 28000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 30000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 32000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 34000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 36000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 38000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 40000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 42000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 44000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 46000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 48000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 50000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 52000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 54000000
		++ SAFE_MOD;
#endif
#if		FREQ_SYS >= 56000000
		++ SAFE_MOD;
#endif
#endif
		-- n;
	}
}

/*******************************************************************************
* Function Name  : mDelayms(UNIT16 n)
* Description    : ms延时函数
* Input          : UNIT16 n
* Output         : None
* Return         : None
*******************************************************************************/
void	mDelaymS( UINT16 n )                                                  // 以mS为单位延时
{
	while ( n ) 
	{
		mDelayuS( 1000 );
		-- n;
	}
}                                         

/*******************************************************************************
* Function Name  : CH559UART0Alter()
* Description    : CH559串口0引脚映射,串口映射到P0.2和P0.3
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void CH559UART0Alter()
{
    PORT_CFG |= bP0_OC;
    P0_DIR |= bTXD_;
    P0_PU |= bTXD_ | bRXD_;
    PIN_FUNC |= bUART0_PIN_X;                                                  //串口映射到P0.2和P0.3
}

/*******************************************************************************
* Function Name  : mInitSTDIO()
* Description    : CH559串口0初始化,默认使用T1作UART0的波特率发生器,也可以使用T2
                   作为波特率发生器
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/
void	mInitSTDIO( )
{
    UINT32 x;
    UINT8 x2; 

    SM0 = 0;
    SM1 = 1;
    SM2 = 0;                                                                   //串口0使用模式1
                                                                               //使用Timer1作为波特率发生器
    RCLK = 0;                                                                  //UART0接收时钟
    TCLK = 0;                                                                  //UART0发送时钟
    PCON |= SMOD;
    x = 10 * FREQ_SYS / BUAD / 16;                                             //如果更改主频，注意x的值不要溢出                            
    x2 = x % 10;
    x /= 10;
    if ( x2 >= 5 ) x ++;                                                       //四舍五入

    TMOD = TMOD & ~ bT1_GATE & ~ bT1_CT & ~ MASK_T1_MOD | bT1_M1;              //0X20，Timer1作为8位自动重载定时器
    T2MOD = T2MOD | bTMR_CLK | bT1_CLK;                                        //Timer1时钟选择
    TH1 = 0-x;                                                                 //12MHz晶振,buad/12为实际需设置波特率
    TR1 = 1;                                                                   //启动定时器1
    TI = 1;
    REN = 1;                                                                   //串口0接收使能
}

/*******************************************************************************
* Function Name  : CH559UART0RcvByte()
* Description    : CH559UART0接收一个字节
* Input          : None
* Output         : None
* Return         : SBUF
*******************************************************************************/
UINT8  CH559UART0RcvByte( )
{
    while(RI == 0);                                                            //查询接收，中断方式可不用
    RI = 0;
    return SBUF;
}

/*******************************************************************************
* Function Name  : CH559UART0SendByte(UINT8 SendDat)
* Description    : CH559UART0发送一个字节
* Input          : UINT8 SendDat；要发送的数据
* Output         : None
* Return         : None
*******************************************************************************/
void CH559UART0SendByte(UINT8 SendDat)
{
	SBUF = SendDat;                                                              //查询发送，中断方式可不用下面2条语句,但发送前需TI=0
	while(TI ==0);
	TI = 1;
}