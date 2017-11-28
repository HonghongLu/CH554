
#define ADC_INTERRUPT   1 

/*******************************************************************************
* Function Name  : ADCClkSet(UINT8 div)
* Description    : ADC����ʱ������,ģ�鿪�����жϿ���
* Input          : UINT8 div ʱ������ 
                   0 ��  384��Fosc                   								
                   1 ��  96��Fosc									 
* Output         : None
* Return         : None
*******************************************************************************/
void ADCInit(UINT8 div);

/*******************************************************************************
* Function Name  : ADC_ChannelSelect(UINT8 ch)
* Description    : ADC����ͨ������
* Input          : UINT8 ch ����ͨ��0-3
* Output         : None
* Return         : �ɹ� SUCCESS
                   ʧ�� FAIL ͨ�����ó�����Χ
*******************************************************************************/
UINT8 ADC_ChannelSelect(UINT8 ch);

/*******************************************************************************
* Function Name  : VoltageCMPModeInit()
* Description    : ��ѹ�Ƚ���ģʽ��ʼ��
* Input          : UINT8 fo ����˿� 0\1\2\3
                   UINT8 re ����˿� 1\3
* Output         : None
* Return         : �ɹ� SUCCESS
                   ʧ�� FAIL
*******************************************************************************/
UINT8 VoltageCMPModeInit(UINT8 fo,UINT8 re);
