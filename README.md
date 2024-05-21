# AutomaticController
用于自动化开发的简易框架包含以下功能:
1. 托管的modbus-rtu功能，实现IModbus_RTU_Unit对象创建后的自动绑定，后续可能添加更多通讯协议
2. 一些关联PLC变量的控件，UBitButton、ULabel、UTextBox等，在后端创建变量后可以直接与前端控件绑定，示例如下：
    ```C#
    public class PLC
    {
        public static Modbus_RTU PLC { get; set; } = new Modbus_RTU(1);//创建一个MODBUL-rtu通讯,设置站号为1
        public static Modbus_RTU_Num D0 { get; set; } = new Modbus_RTU_Num(PLC, Modbus_EasyAddress.D(0));
        public void Main()
        {
            //启动MOODBUS连接
            PLC1.PLC.Start(new System.IO.Ports.SerialPort()
            {
                PortName = "COM1",
                BaudRate = 9600,
                Parity = System.IO.Ports.Parity.None,
                DataBits = 8,
                StopBits = System.IO.Ports.StopBits.One,
                WriteTimeout = 1000,
                ReadTimeout = 1000,
            });
        }
    }
    ```
    ```xaml
    <!--创建一个绑定PLC的变量-->
    <ui:ULabel DataContext="{Binding Source={x:Static ds:PLC1.D0}}" FontSize="20" BorderBrush="#FF8C8C8C" BorderThickness="1,1,1,1" Width="100"/>
    ```
3. 使用框架时可以引用项目，也可以直接在Windows文件夹中，创建简易的窗体，并在App.xaml.cs=>Application_Startup中设置启动窗口。
    Windows文件夹中包含了一些项目示例，主要目的是为了更方便的copy