
南京沁恒电子有限公司 2017.01
最新资料请到我们的网站上查询
http://wch.cn
http://winchiphead.com


文件目录
  |
  |-- README.TXT：目录文件说明
  |-- BRIEF
  |      |--CH554DS0.PDF：CH554简介
  |-- DS
  |      |--CH554DS1.PDF：CH554应用手册
  |-- EVT
  |      |-- EXAM: C例程
  |      |      |--CH554.uvproj：CH554 keil4工程文件
  |      |      |--Public
  |      |      |      |--CH554.H：CH554 C语言头文件，SFR定义
  |      |      |      |--CH554.INC：CH554汇编头文件，SFR定义
  |      |      |      |--Debug.C：UART0&1初始化、常用延时函数、UART0&1数据收发子函数,看门狗初始化
  |      |      |      |--Debug.H：UART0&1初始化、常用延时函数、UART0&1数据收发子函数、看门狗初始化声明
  |      |      |--ADC：
  |      |      |      |-- Main.C:ADC使用示例
  |      |      |      |-- ADC.C&H:ADC初始化，采样时钟设定，通道变化和电压比较功能函数定义
  |      |      |      |-- ADC.hex
  |      |      |--DataFlash
  |      |      |      |-- Main.C:DataFlash使用示例
  |      |      |      |-- DataFlash.C&H:DataFlash读写函数定义
  |      |      |      |-- DataFlash.hex
  |      |      |--GPIO
  |      |      |      |-- Main.C:GPIO使用示例
  |      |      |      |-- GPIO.C&H:GPIO设置函数定义
  |      |      |      |-- GPIO.hex
  |      |      |--IAP
  |      |      |      |--CH554IAPDemo.C IAP跳转参考文件
  |      |      |      |--CH554IAPDemo.hex
  |      |      |--PWM
  |      |      |      |-- Main.C:PWM使用示例
  |      |      |      |-- PWM.C&H:PWM初始化、设置函数定义
  |      |      |      |-- PWM.hex
  |      |      |--SPI
  |      |      |      |-- MainMaster.C:SPI主模式使用示例
  |      |      |      |-- MainSLV.C:SPI从模式使用示例
  |      |      |      |-- SPI.C&H:SPI主从设置和收发函数定义
  |      |      |      |-- MainMaster.hex
  |      |      |      |-- MainSLV.hex
  |      |      |--Timer
  |      |      |      |-- Main.C:Timer0&1&2定时，计数设置和捕捉功能使用示例
  |      |      |      |-- Timer.C&H:Timer0&1&2定时，计数设置和捕捉函数定义
  |      |      |      |-- Timer.hex  
  |      |      |--Type-C
  |      |      |      |-- Main.C:Type-C 主从模式使用示例
  |      |      |      |-- Type_C.C&H:Type-C主从模式配置函数定义
  |      |      |      |-- Type-C_DFP.hex 主模式 
  |      |      |      |-- Type-C_UFP.hex 从模式   
  |      |      |--TouchKey
  |      |      |      |-- Main.C:CH554 触摸按键中断和查询方式进行采集并报告当前采样通道按键状态，包含初始化和按键采样等演示函数  
  |      |      |      |-- TouchKey.C&H:TouchKey开启，通道切换和中断采集函数
  |      |      |      |-- TouchKey.hex
  |      |      |--USB
  |      |      |      |-- Device
  |      |      |      |      |-- CompositeKM .C:模拟键鼠例程，支持部分类命令
  |      |      |      |      |-- CompositeKM.hex 
  |      |      |      |      |-- VendorDefinedDev.C:模拟厂商自定义设备，需要安装CH372驱动，可以通过372test.exe调试(批量数据收发)
  |      |      |      |      |-- VendorDefinedDev.hex
  |      |      |      |      |-- CompatibilityHID.C:模拟HID兼容设备，支持中断上下传
  |      |      |      |      |-- CompatibilityHID.hex  
  |      |      |      |-- Host
  |      |      |      |      |-- USBHostHUB_KM.C: USB主机应用例子,初始化和枚举USB端口连接的设备,支持一级外部HUB,可以操作USB键鼠和HUB，打印机,包含HID类命令处理
  |      |      |      |      |-- USBHostHUB_KM.hex: 
  |      |      |      |-- U_DISK
  |      |      |      |      |      |-- EXAM1.C:C示例源程序,以字节为单位读写文件，包括文件创建、删除、修改文件属性,修改文件名
  |      |      |      |      |      |-- EXAM1.hex
  |      |      |      |      |      |-- EXAM11.C:C示例源程序,枚举根目录或者指定目下的文件
  |      |      |      |      |      |-- EXAM11.hex:
  |      |      |      |-- USB_LIB
  |      |      |      |      |-- CH554UFI.C:CH554 U盘读写文件系统库常用接口函数
  |      |      |      |      |-- CH554UFR.H:CH554 U盘只读文件系统库变量和函数声明头文件
  |      |      |      |      |-- CH554UFR.LIB:CH554 U盘只读文件系统库文件(编译器内存模式设置成:small/large)  
  |      |      |      |      |-- CH554UFI.LIB:CH554 U盘文件系统库文件(编译器内存模式设置成:small)
  |      |      |      |      |-- CH554UFX.LIB:U盘文件系统库文件(编译器内存模式设置成:large)
  |      |      |      |      |-- USBHOST.C&H:CH554 USB主机控制传输函数定义
  |      |      |      |      |-- CH554UF_README.TXT:CH554 U盘文件系统变更文档
  |      |-- PCB: 评估板资料
  |      |      |-- CH554MINIEVT.DDB: CH554 L和E封装PCB、原理图
  |      |-- PUB: 评估板说明
  |      |      |-- CH554EVT.PDF: 评估板和EVT资料包说明
  |      |      |-- CH554SCH.PDF: 评估板原理图
  |      |      |-- CH55X汇编指令.PDF: 汇编指令说明
