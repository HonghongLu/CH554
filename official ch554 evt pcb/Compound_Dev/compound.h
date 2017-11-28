/********************************** (C) COPYRIGHT *******************************
* File Name          :Compound_Dev.C											*	
* Author             : WCH                                                      *
* Version            : V1.2                                                     *
* Date               : 2017/02/24                                               *
* Description        : A demo for USB compound device created by CH554, support *
					   keyboard , and HID-compliant device.                     *
********************************************************************************/


#ifndef	__COMPOUND_H__
#define __COMPOUND_H__

extern UINT16I 	TouchKeyButton;	

extern	void 	USBDeviceInit();
extern	void 	HIDValueHandle();
	
#endif
/**************************** END *************************************/
