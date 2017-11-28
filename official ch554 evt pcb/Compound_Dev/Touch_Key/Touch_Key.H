
#define INTERRUPT_TouchKey   0   //开启TouchKey中断方式

/*******************************************************************************
* Function Name  : TouchKeyQueryCylSet(UINT8 cyl)
* Description    : 触摸按键查询周期设置
* Input          : UINT8 cyl 时钟设置                  								
                   1   2ms
                   0   1ms									 
* Output         : None 
* Return         : None
*******************************************************************************/
void TouchKeyQueryCylSet1Or2ms(UINT8 cyl);

/*******************************************************************************
* Function Name  : TouchKeyChanelSelect(UINT8 ch)
* Description    : 触摸按键通道选择
* Input          : UINT8 ch 采用通道
* Output         : None
* Return         : 成功 SUCCESS
                   失败 FAIL
*******************************************************************************/
UINT8 TouchKeyChanelSelect(UINT8 ch);

