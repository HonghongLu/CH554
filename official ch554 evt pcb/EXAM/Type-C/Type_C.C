
/********************************** (C) COPYRIGHT *******************************
* File Name          : Type_C.C
* Author             : WCH
* Version            : V1.1
* Date               : 2017/07/05
* Description        : CH554 Type-C使用
                       主模式 设备正反插检测、充电电流通知从设备
                       从模式 检测主机供电能力	 
*******************************************************************************/

#include "..\Public\CH554.H"                                                         
#include "..\Public\Debug.H"
#include "Type_C.H"
#include "stdio.h"

#pragma  NOAREGS
/********************************************************************************
DFP (Downstream Facing Port) Host端
UFP (Upstream Facing Port)   Dev端

在DFP的CC pin会有上拉电阻Rp,在UFP会有下拉电阻Rd
在DFP和UFP未连接前,DFP的VBUS是没有输出的

CC PIN是用来侦测正反插，从DPF来看，当CC1接到下拉就是正插;如果CC2接到下拉就是反插；
	 侦测完正反插之后，就会输出相应的USB信号
	 
DPF在不同的模式下，定义了在CC PIN要供多大的电流或是要用多大的Rp值
********************************************************************************/
#ifdef TYPE_C_DFP

/*******************************************************************************
* Function Name  : TypeC_DFP_Init(UINT8 Power)
* Description    : Type-C UPF检测初始化
* Input          : UINT8 Power
                   0 禁止UCC1&2上拉
                   1 默认电流
                   2 1.5A
                   3 3.0A									 
* Output         : None
* Return         : NONE
*******************************************************************************/
void TypeC_DFP_Init( UINT8 Power )
{
   P1_MOD_OC &= ~(bUCC2|bUCC1);                                                   
   P1_DIR_PU &= ~(bUCC2|bUCC1);                                                   //UCC1 UCC2 设置浮空输入
   if(Power == 0){
     DFP_DisableRpUCC1();                                                         //UCC1禁止
     DFP_DisableRpUCC2();	                                                        //UCC2禁止	
   }
	 else if(Power == 1){
     DFP_DefaultPowerUCC1();                                                      //输出能力默认
     DFP_DefaultPowerUCC2();	                                                    	 
   }
   else if(Power == 2){
     DFP_1_5APowerUCC1();                                                         //输出能力1.5A
     DFP_1_5APowerUCC2();		 
   }
   else if(Power == 3){
     DFP_3_0APowerUCC1();                                                         //输出能力3.0A
     DFP_3_0APowerUCC2();		 
   }
   ADC_CFG = ADC_CFG & ~bADC_CLK | bADC_EN;											                  //ADC时钟配置,0(96clk) 1(384clk),ADC模块开启	
	 P1_DIR_PU &= ~(bAIN0 | bAIN1);																									//配置UCC1和UCC2作为ADC引脚
   mDelayuS(2);                                                                   //等待上拉完全关闭和ADC电源稳定	

}

/*******************************************************************************
* Function Name  : TypeC_DFP_Channle(void)
* Description    : Type-C DPF检测UFP正插，反插以及未插入和已插入
* Input          : NONE
* Output         : None
* Return         : 0   未连接
                   1   正向连接
                   2   反向连接
                   3   连接，无法判定正反
*******************************************************************************/
UINT8 TypeC_DFP_Insert( void )
{
		UINT8 UCC1_Value,UCC2_Value;

    ADC_CHAN1 =0;ADC_CHAN0=1;                                                     //UCC1通道的ADC
		mDelayuS(1);                                                                  //通道切换稍作延时	
		ADC_START = 1;                                                                //开始采样
		while(ADC_START);                                                             //ADC_START采样完成
		UCC1_Value = ADC_DATA;
//printf("UCC1_Value=%02x\n",(UINT16)UCC1_Value);  

    ADC_CHAN1 =1;ADC_CHAN0=0;                                                     //UCC2通道的ADC
		mDelayuS(1);                                                                  //通道切换稍作延时	
		ADC_START = 1;                                                                //开始采样
		while(ADC_START);                                                             //ADC_START采样完成
		UCC2_Value = ADC_DATA;
//printf("UCC2_Value=%02x\n",(UINT16)UCC2_Value);                                                                                
		if( UCC1_Value<=UCC_Connect_Vlaue && UCC2_Value<=UCC_Connect_Vlaue )          //双向连接
    {	 
			return UCC_CONNECT;
		}
		else if( UCC1_Value<=UCC_Connect_Vlaue && UCC2_Value>=UCC_Connect_Vlaue )     //正向连接
    { 
			return UCC1_CONNECT;
		}
		else if( UCC1_Value>=UCC_Connect_Vlaue && UCC2_Value<=UCC_Connect_Vlaue )     //反向连接
    {	 
			return UCC2_CONNECT;
		}
		else if( UCC1_Value>=UCC_Connect_Vlaue && UCC2_Value>=UCC_Connect_Vlaue )     //未连接
    {  
			return UCC_DISCONNECT;
		}
	  return UCC_DISCONNECT;
}
#endif

/********************************************************************************
UPF,需要检测CC管脚的电压值来获取DFP的电流输出能力
--------  CC电压Min   CC电压Max
默认电流   0.25V         0.61V
1.5A       0.70V         1.16V
3.0A       1.31V         2.04V
********************************************************************************/
#ifdef TYPE_C_UFP
/*******************************************************************************
* Function Name  : TypeC_UPF_PDInit()
* Description    : Type-C UPF初始化
* Input          : None
* Output         : None
* Return         : None							 
*******************************************************************************/
void TypeC_UPF_PDInit( void )
{
   P1_MOD_OC &= ~(bUCC2|bUCC1);                                                   
   P1_DIR_PU &= ~(bUCC2|bUCC1);                                                   //UCC1 UCC2 设置浮空输入
	 UPF_DisableRd(1);                                                              //开启UCC下拉电阻
   ADC_CFG = ADC_CFG & ~bADC_CLK | bADC_EN;											                  //ADC时钟配置,0(96clk) 1(384clk),ADC模块开启	
	 P1_DIR_PU &= ~(bAIN0 | bAIN1);																									//配置UCC1和UCC2作为ADC引脚
   mDelayuS(2);                                                                   //等待上拉完全关闭和ADC电源稳定	
}

/*******************************************************************************
* Function Name  : TypeC_UPF_PDCheck()
* Description    : Type-C UPF检测DPF供电能力
* Input          : None
* Output         : None
* Return         : UINT8 RE  
                   0  defaultPower
                   1  1.5A
                   2  3.0A	
                   0xff 悬空									 
*******************************************************************************/
UINT8 TypeC_UPF_PDCheck()
{
		UINT8 ADC_VALUE;
	
		ADC_CHAN1 =0;ADC_CHAN0=1;
		mDelayuS(2);                                                                  //通道切换稍作延时	
	  ADC_START = 1;                                                                //开始采样
    while(ADC_START);                                                             //ADC_START变为0时，表示采样完成
		ADC_VALUE = ADC_DATA;
	  if((ADC_VALUE >= Power3_0AMin)&&(ADC_VALUE <= Power3_0AMax))
    {
      return UPF_PD_3A;                                                           //3.0A供电能力			
    }
    else if((ADC_VALUE >= Power1_5AMin)&&(ADC_VALUE <= Power1_5AMax))
    {
      return UPF_PD_1A5;                                                          //1.5A供电能力			
    }		
    else if((ADC_VALUE >= DufaultPowerMin)&&(ADC_VALUE <= DufaultPowerMax))
    {
      return UPF_PD_Normal;                                                       //默认供电能力			
    }		
		
    ADC_CHAN1 =1;ADC_CHAN0=0;
    mDelayuS(2);                                                                  //通道切换稍作延时	
    ADC_START = 1;                                                                //开始采样，采样完成进入中断	
    while(ADC_START);                                                             //ADC_START变为0时，表示采样完成
		ADC_VALUE = ADC_DATA;
		
    if((ADC_VALUE >= Power3_0AMin)&&(ADC_VALUE <= Power3_0AMax))
    {
      return UPF_PD_3A;                                                           //3.0A供电能力			
    }
    else if((ADC_VALUE >= Power1_5AMin)&&(ADC_VALUE <= Power1_5AMax))
    {
      return UPF_PD_1A5;                                                          //1.5A供电能力			
    }		
    else if((ADC_VALUE >= DufaultPowerMin)&&(ADC_VALUE <= DufaultPowerMax))
    {
      return UPF_PD_Normal;                                                       //默认供电能力			
    }	
    return UPD_PD_DISCONNECT;		
}
#endif
