/********************************** (C) COPYRIGHT *******************************
* File Name          : Encryption.C
* Author             : WCH
* Version            : V1.3
* Date               : 2016/06/24
* Description        : 使用数值替代或插值来灵活地实现单片机HEX加密
                       例如，使用Encoded_0()和Encoded_1()分别替代0和1，其他同理可以用类似方法加密密钥
*******************************************************************************/

#include "./DEBUG.C"
#include "./DEBUG.H"
#define  PI  3.141592657

#pragma  NOAREGS

UINT8  ID;
UINT8  IDX;

/*******************************************************************************
* Function Name  : EraseBlock(UNIT16 Addr)
* Description    : Dataflash块擦除函数
* Input          : UINT16 Addr
* Output         : None
* Return         : 状态status
*******************************************************************************/ 
UINT8	EraseBlock( UINT16 Addr )
{
	ROM_ADDR = Addr;
	if ( ROM_STATUS & bROM_ADDR_OK ) {  // 操作地址有效
		ROM_CTRL = ROM_CMD_ERASE;
		return( ( ROM_STATUS ^ bROM_ADDR_OK ) & 0x7F );  // 返回状态,0x00=success, 0x01=time out(bROM_CMD_TOUT), 0x02=unknown command(bROM_CMD_ERR)
	}
	else return( 0x40 );
}

/*******************************************************************************
* Function Name  : ProgWord( UINT16 Addr, UINT16 Data )
* Description    : Dataflash写入函数
* Input          : UNIT16 Addr,UINT16 Data
* Output         : None
* Return         : 状态status
*******************************************************************************/ 
UINT8	ProgWord( UINT16 Addr, UINT16 Data )
{
	ROM_ADDR = Addr;
	ROM_DATA = Data;
	if ( ROM_STATUS & bROM_ADDR_OK ) {  // 操作地址有效
		ROM_CTRL = ROM_CMD_PROG;
		return( ( ROM_STATUS ^ bROM_ADDR_OK ) & 0x7F );  // 返回状态,0x00=success, 0x01=time out(bROM_CMD_TOUT), 0x02=unknown command(bROM_CMD_ERR)
	}
	else return( 0x40 );
}

/*******************************************************************************
* Function Name  : EncodedID_AndWR_ToDataflash()
* Description    : ID转化功能函数，使用不可逆运算（这里可以选择别的更复杂的算法）操作ID，
                   获得一个整数IDX，存入Dataflash
* Input          : None
* Output         : None
* Return         : None
*******************************************************************************/ 
UINT8 EncodedID_AndWR_ToDataflash( )
{
	double i;
	UINT8 status;
	i=(double)ID/PI;
	IDX=(UINT8)i;
	
	SAFE_MOD = 0x55;
	SAFE_MOD = 0xAA;/*进入安全模式*/
	GLOBAL_CFG |= bDATA_WE;/*Dataflash写使能*/
	status=EraseBlock(0xF000);/*擦除1K的Dataflash*/
	SAFE_MOD = 0x55;
	SAFE_MOD = 0xAA;
  GLOBAL_CFG &= ~ bDATA_WE;/*Dataflash写使能关闭*/
	SAFE_MOD = 0xFF;/*退出安全模式*/
	
	SAFE_MOD = 0x55;
	SAFE_MOD = 0xAA;/*进入安全模式*/
	GLOBAL_CFG |= bDATA_WE;/*Dataflash写使能*/
	status=ProgWord( 0xF000,(UINT16)IDX);/*将解码基数写入Dataflash*/
	SAFE_MOD = 0x55;
	SAFE_MOD = 0xAA;
	GLOBAL_CFG &= ~ bDATA_WE;/*Dataflash写使能关闭*/
	SAFE_MOD = 0xFF;/*退出安全模式*/
	
	return IDX;
}

/*******************************************************************************
* Function Name  : GetIDXFromDataflash()
* Description    : IDX获取函数
* Input          : None
* Output         : None
* Return         : IDX
*******************************************************************************/ 
UINT8 GetIDXFromDataflash()
{
	return (UINT16)*((PUINT8C)(0xF000));
}

/*******************************************************************************
* Function Name  : Encoded_0()
* Description    : 数值0替代函数
* Input          : None
* Output         : None
* Return         : 0
*******************************************************************************/ 
UINT8 Encoded_0()
{
	return (GetIDXFromDataflash()-EncodedID_AndWR_ToDataflash());
}

/*******************************************************************************
* Function Name  : Encoded_1()
* Description    : 数值1替代函数
* Input          : None
* Output         : None
* Return         : 1
*******************************************************************************/ 
UINT8 Encoded_1()
{
	return (GetIDXFromDataflash()-EncodedID_AndWR_ToDataflash()+1);
}

void main()
{
	UINT8 i;
	mDelaymS(50);
	mInitSTDIO( );
	printf("start...\n");                                                
  ID=CHIP_ID;/*获取芯片ID*/ 

/*调试用*/ 	
// 	i=Encoded_0();
//  printf("0的替代值  %02X\n",(UINT16)i);  
// 	i=Encoded_1();
// 	printf("1的替代值  %02X\n",(UINT16)i);
	
	//功能部分，使用数值替代的for循环
	for(i= Encoded_0();i<10*Encoded_1();i=i+Encoded_1())
	{
		printf("%02X\n",(UINT16)i);
	}
	
	while(1);
}