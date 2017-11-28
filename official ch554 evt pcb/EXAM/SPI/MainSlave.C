
/********************************** (C) COPYRIGHT *******************************
* File Name          : MainSlave.C
* Author             : WCH
* Version            : V1.1 
* Date               : 2017/07/05
* Description        : CH554 SPI设备例子演示，连接SPI主机进行数据收发，从机获取主机的数据取反
                       然后发送给主机
*******************************************************************************/
#include "..\Public\CH554.H"                                                   
#include "..\Public\Debug.H"
#include "SPI.H"
#include "stdio.h"
#include "string.h"

/*硬件接口定义*/
/******************************************************************************
使用CH554 硬件SPI接口 
         CH554               
         P1.4        =       SCS
         P1.5        =       MOSI
         P1.6        =       MISO
         P1.7        =       SCK
*******************************************************************************/


void main()
{
    UINT8 ret,i=0;
    CfgFsys( ); 
    mDelaymS(5);                                                               //修改系统主频，建议稍加延时等待主频稳定    
    
    mInitSTDIO( );                                                             //串口0初始化
    printf("start ...\n");  
    SPI0_S_PRE = 0x66;	
	
    SPISlvModeSet( );                                                          //SPI从机模式设置
    printf("PRE  SPI0_STAT %02x，SPI0_CTRL %02x\n  ",(UINT16)SPI0_STAT,(UINT16)SPI0_CTRL);

    while(1)
    {   
        ret = CH554SPISlvRead();	                                             //主机保持CS=0		                         	
        CH554SPISlvWrite(ret^0xFF);                                            //SPI等待主机把数据取走,SPI 主机每次读之前先将CS=0，读完后CS=1     
			  printf("Write %02x \n",(UINT16)(ret^0xff));								
    }
}