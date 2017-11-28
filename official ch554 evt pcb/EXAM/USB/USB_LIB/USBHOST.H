

// 各子程序返回状态码
#define ERR_SUCCESS         0x00    // 操作成功
#define ERR_USB_CONNECT     0x15    /* 检测到USB设备连接事件,已经连接 */
#define ERR_USB_DISCON      0x16    /* 检测到USB设备断开事件,已经断开 */
#define ERR_USB_BUF_OVER    0x17    /* USB传输的数据有误或者数据太多缓冲区溢出 */
#define ERR_USB_DISK_ERR    0x1F    /* USB存储器操作失败,在初始化时可能是USB存储器不支持,在读写操作中可能是磁盘损坏或者已经断开 */
#define ERR_USB_TRANSFER    0x20    /* NAK/STALL等更多错误码在0x20~0x2F */
#define ERR_USB_UNSUPPORT   0xFB    /*不支持的USB设备*/
#define ERR_USB_UNKNOWN     0xFE    /*设备操作出错*/ 

/*USB设备相关信息表,CH554最多支持1个设备*/
#define ROOT_DEV_DISCONNECT  0
#define ROOT_DEV_CONNECTED   1
#define ROOT_DEV_FAILED      2
#define ROOT_DEV_SUCCESS     3
#define DEV_TYPE_KEYBOARD   ( USB_DEV_CLASS_HID | 0x20 )
#define DEV_TYPE_MOUSE      ( USB_DEV_CLASS_HID | 0x30 )

/*
约定: USB设备地址分配规则(参考USB_DEVICE_ADDR)
地址值  设备位置
0x02    内置Root-HUB下的USB设备或外部HUB
0x1x    内置Root-HUB下的外部HUB的端口x下的USB设备,x为1~n
*/
#define HUB_MAX_PORTS       4
#define WAIT_USB_TOUT_200US     200   // 等待USB中断超时时间200uS@Fsys=12MHz

extern UINT8C  SetupGetDevDescr[];    //*获取设备描述符*/
extern UINT8C  SetupGetCfgDescr[];    //*获取配置描述符*/
extern UINT8C  SetupSetUsbAddr[];     //*设置USB地址*/
extern UINT8C  SetupSetUsbConfig[];   //*设置USB配置*/
extern UINT8C  SetupSetUsbInterface[];//*设置USB接口配置*/
extern UINT8C  SetupClrEndpStall[];   //*清除端点STALL*/
#ifndef DISK_BASE_BUF_LEN
extern UINT8C  SetupGetHubDescr[];    //*获取HUB描述符*/
extern UINT8C  SetupGetHIDDevReport[];//*获取HID设备报表描述符*/
extern UINT8C  XPrinterReport[];      //*打印机类命令*/
#endif
extern UINT8X  UsbDevEndp0Size;       //* USB设备的端点0的最大包尺寸 */


#ifndef DISK_BASE_BUF_LEN
typedef struct
{
    UINT8   DeviceStatus;              // 设备状态,0-无设备,1-有设备但尚未初始化,2-有设备但初始化枚举失败,3-有设备且初始化枚举成功
    UINT8   DeviceAddress;             // 设备被分配的USB地址
    UINT8   DeviceSpeed;               // 0为低速,非0为全速
    UINT8   DeviceType;                // 设备类型
//  union {
//      struct MOUSE {
//      UINT8   MouseInterruptEndp;    // 鼠标中断端点号
//      UINT8   MouseIntEndpTog;       // 鼠标中断端点的同步标志
//      UINT8   MouseIntEndpSize;      // 鼠标中断端点的长度
//      }
//      struct PRINT {
//      }
//  }
//.....    struct  _Endp_Attr   Endp_Attr[4];   //端点的属性,最多支持4个端点
    UINT8   GpVar;                    // 通用变量
    UINT8   GpVar1;                   // 通用变量1
} _RootHubDev;

typedef struct 
{
    UINT8   DeviceStatus;             // 设备状态,0-无设备,1-有设备但尚未初始化,2-有设备但初始化枚举失败,3-有设备且初始化枚举成功
    UINT8   DeviceAddress;            // 设备被分配的USB地址
    UINT8   DeviceSpeed;              // 0为低速,非0为全速
    UINT8   DeviceType;               // 设备类型
//.....    struct  _Endp_Attr   Endp_Attr[4];   //端点的属性,最多支持4个端点
    UINT8   GpVar;                    // 通用变量
} _DevOnHubPort;                      // 假定:不超过1个外部HUB,每个外部HUB不超过HUB_MAX_PORTS个端口(多了不管)

extern _RootHubDev xdata ThisUsbDev;
extern _DevOnHubPort xdata DevOnHubPort[HUB_MAX_PORTS];// 假定:不超过1个外部HUB,每个外部HUB不超过HUB_MAX_PORTS个端口(多了不管)
extern UINT8 Set_Port;
#endif
extern UINT8X  RxBuffer[ MAX_PACKET_SIZE ];  // IN, must even address
extern UINT8X  TxBuffer[ MAX_PACKET_SIZE ];  // OUT, must even address
extern bit     FoundNewDev;

#define pSetupReq   ((PXUSB_SETUP_REQ)TxBuffer)


void    DisableRootHubPort( );                        // 关闭ROOT-HUB端口,实际上硬件已经自动关闭,此处只是清除一些结构状态
UINT8   AnalyzeRootHub( void );                       // 分析ROOT-HUB状态,处理ROOT-HUB端口的设备插拔事件
// 返回ERR_SUCCESS为没有情况,返回ERR_USB_CONNECT为检测到新连接,返回ERR_USB_DISCON为检测到断开
void    SetHostUsbAddr( UINT8 addr );                 // 设置USB主机当前操作的USB设备地址
void    SetUsbSpeed( UINT8 FullSpeed );               // 设置当前USB速度
void    ResetRootHubPort( );                          // 检测到设备后,复位相应端口的总线,为枚举设备准备,设置为默认为全速
UINT8   EnableRootHubPort( );                         // 使能ROOT-HUB端口,相应的bUH_PORT_EN置1开启端口,设备断开可能导致返回失败
void    SelectHubPort( UINT8 HubPortIndex );// HubPortIndex=0选择操作指定的ROOT-HUB端口,否则选择操作指定的ROOT-HUB端口的外部HUB的指定端口
UINT8   WaitUSB_Interrupt( void );                    // 等待USB中断
// CH554传输事务,输入目的端点地址/PID令牌,同步标志,以20uS为单位的NAK重试总时间(0则不重试,0xFFFF无限重试),返回0成功,超时/出错重试
UINT8   USBHostTransact( UINT8 endp_pid, UINT8 tog, UINT16 timeout );  // endp_pid: 高4位是token_pid令牌, 低4位是端点地址
UINT8   HostCtrlTransfer( PUINT8X DataBuf, PUINT8 RetLen );  // 执行控制传输,8字节请求码在pSetupReq中,DataBuf为可选的收发缓冲区
// 如果需要接收和发送数据,那么DataBuf需指向有效缓冲区用于存放后续数据,实际成功收发的总长度返回保存在ReqLen指向的字节变量中
void    CopySetupReqPkg( PUINT8C pReqPkt );            // 复制控制传输的请求包
UINT8   CtrlGetDeviceDescr( void );                    // 获取设备描述符,返回在TxBuffer中
UINT8   CtrlGetConfigDescr( void );                    // 获取配置描述符,返回在TxBuffer中
UINT8   CtrlSetUsbAddress( UINT8 addr );               // 设置USB设备地址
UINT8   CtrlSetUsbConfig( UINT8 cfg );                 // 设置USB设备配置
UINT8   CtrlSetUsbIntercace( UINT8 cfg );              // 设置USB设备接口 
#ifndef DISK_BASE_BUF_LEN
UINT8   CtrlClearEndpStall( UINT8 endp );              // 清除端点STALL
UINT8   CtrlGetHubDescr( void );                       // 获取HUB描述符,返回在TxBuffer中
UINT8   HubGetPortStatus( UINT8 HubPortIndex );        // 查询HUB端口状态,返回在TxBuffer中
UINT8   HubSetPortFeature( UINT8 HubPortIndex, UINT8 FeatureSelt );  // 设置HUB端口特性
UINT8   HubClearPortFeature( UINT8 HubPortIndex, UINT8 FeatureSelt );  // 清除HUB端口特性
UINT8   AnalyzeHidIntEndp( PUINT8X buf );              // 从描述符中分析出HID中断端点的地址
UINT8   EnumAllRootDevice( void );                     // 枚举所有ROOT-HUB端口的USB设备
UINT8   InitDevOnHub(UINT8 HubPortIndex );             // 初始化枚举外部HUB后的二级USB设备
UINT8   EnumHubPort( );                                // 枚举指定ROOT-HUB端口上的外部HUB集线器的各个端口,检查各端口有无连接或移除事件并初始化二级USB设备
UINT8   EnumAllHubPort( void );                        // 枚举所有ROOT-HUB端口下外部HUB后的二级USB设备
UINT16  SearchTypeDevice( UINT8 type );                // 在ROOT-HUB以及外部HUB各端口上搜索指定类型的设备所在的端口号,输出端口号为0xFFFF则未搜索到
// 输出高8位为ROOT-HUB端口号,低8位为外部HUB的端口号,低8位为0则设备直接在ROOT-HUB端口上
UINT8 SETorOFFNumLock(PUINT8 buf);
#endif
UINT8   InitRootDevice( );                             // 初始化指定ROOT-HUB端口的USB设备
void    InitUSB_Host( void );                          // 初始化USB主机