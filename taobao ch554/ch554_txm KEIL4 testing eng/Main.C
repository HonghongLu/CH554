
/********************************** (C) COPYRIGHT *******************************
* File Name          : Main.C
* Author             : WCH
* Version            : V1.0
* Date               : 2017/01/20
* Description        : CH554 触摸按键中断和查询方式进行采集并报告当前采样通道按键状态，包含初始化和按键采样等演示函数
*******************************************************************************/
#include "CH554.H"
#include "Debug.H"
#include "TouchKey.H"
#include <stdio.h>

#pragma  NOAREGS

#define	LED_PIN 5
sbit	LED=P1^5;
main( )
{

    CfgFsys( );                                                                //CH554时钟选择配置
    mDelaymS(5);
    P1_DIR_PU &= 0x0C;
    P1_MOD_OC = P1_MOD_OC & ~(1<<LED_PIN);
    P1_DIR_PU = P1_DIR_PU |	(1<<LED_PIN);
  //  TouchKeyQueryCyl2ms();                                                     //TouchKey查询周期2ms
  //  GetTouckKeyFreeBuf();                                                      //获取采样基准值

    UART1Setup();
     EA = 1;

    while(1)
    {
        mDelaymS(200);
        LED=!LED;
        CH554UART1SendByte('R');


       
    }

}