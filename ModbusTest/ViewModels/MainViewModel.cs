using ModbusTest.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Windows;
using ModbusTest.Base;
using System.Windows.Forms;
using WindowsAPICodePack.Dialogs;

namespace ModbusTest.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        
        private SerialPort serialPort=new SerialPort();
        
        private bool _isStart;
        public bool IsStart
        {
            get
            {
                return _isStart;
            }
            set
            {
                _isStart = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("IsStart"));
            }
        }
        
        private bool _isLog;

        public bool IsLog
        {
            get
            {
                return _isLog;
            }
            set
            {
                _isLog = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("_isLog"));
            }
        }
        
        //虚化
        private int _blurRadius=0;
        public int BlurRadius
        {
            get
            {
                return _blurRadius;
            }
            set
            {
                _blurRadius = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("BlurRadius"));
            }
        }
        
        //时间显示
        private DateTime _currentDate;

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("CurrentDate"));
            }
        }
        
        //
        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("Message"));
            }
        }
        
        private string _msgColor="#90EE90";

        public string MsgColor
        {
            get
            {
                return _msgColor;
            }
            set
            {
                _msgColor = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("MsgColor"));
            }
        }
        
        //参数配置区
        public string PortName { get; set; }
        public List<String> PortNameList { get; set; }

        public int BaudRate { get; set; } = 9600;
        public List<String> BaudRateList { get; set; }=new List<String>
        {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
        };
        
       
        
        private string _logpath;

        public string LogPath
        {
            get
            {
                return _logpath;
            }
            set
            {
                _logpath = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("LogPath"));
            }
        }
      
        
        //按钮配置
        
        public Command ConfigCommand { get; set; }
        public Command ConfigCloseCommand { get; set; }
        public Command LogPathSelectCommand{ get; set; }
        
        public Command OpenLogPathCommand { get; set; }
        public List<ItemModel> ItemList { get; set; }
        
        private List<Task>tasks = new List<Task>();
        
        private CancellationTokenSource cts = new CancellationTokenSource();

        public MainViewModel()
        {
            
            //初始化配置数据的初始化
            try
            {
                string json = File.ReadAllText("config.json");
                Config config=System.Text.Json.JsonSerializer.Deserialize<Config>(json);
                this.PortName = config.PortName;
                this.BaudRate = config.BaudRate;
                this.LogPath = config.LogPath;

            }
            catch (Exception e)
            {
                return;
            }
            
            ItemList = new List<ItemModel>();
            ItemList.Add(new ItemModel(){SlaveId=1});
            ItemList.Add(new ItemModel(){SlaveId=2});
            ItemList.Add(new ItemModel(){SlaveId=3});
            ItemList.Add(new ItemModel(){SlaveId=4});

            var task = Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    CurrentDate = DateTime.Now;
                    await Task.Delay(new TimeSpan(0, 1, 0));
                }
            },cts.Token);
            tasks.Add(task);
            
            PortNameList=SerialPort.GetPortNames().ToList();
            
            

            ConfigCommand = new Command(Config);
            ConfigCloseCommand = new Command(ConfigClose);
            LogPathSelectCommand = new Command(LogPathSelect);
            OpenLogPathCommand = new Command(OpenLogFolder);
            
            this.StartMonitor();
        }

        private void Config()
        {
            IsStart =false;
            BlurRadius = 5;
            
        }
        private void ConfigClose()
        {
            BlurRadius = 0;
            
            //数据保存配置

            try
            {
                Config config =new Config();
                config.PortName = this.PortName;
                
                config.BaudRate = this.BaudRate;
                
                config.LogPath = this.LogPath;
                
                var json = System.Text.Json.JsonSerializer.Serialize(config);
                File.WriteAllText("config.json", json);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void LogPathSelect()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.IsFolderPicker = true;

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.LogPath = openFileDialog.FileName;
            }
        }

        private void OpenLogFolder()
        {
            if (Directory.Exists(this.LogPath))
            {
                Process.Start("explorer.exe", "/select," + this.LogPath);
            }
        }

        private void StartMonitor()
        {
            var task = Task.Factory.StartNew(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(500);
                    if (!IsStart) continue;

                    bool state = serialPort.IsOpen;
                    if (!state)
                    {
                        state = OpenSerial();
                    }

                    if (state)
                    {
                        try
                        {
                            foreach (var item in this.ItemList)
                            {
                                await this.Read(item);
                            
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                       
                        
                    }
                   
                }

            },cts.Token);
            tasks.Add(task);
        }

        private bool OpenSerial()
        {
            try
            {
                serialPort.PortName = this.PortName;
                serialPort.BaudRate = this.BaudRate;
                serialPort.DataBits = this.BaudRate;
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None; 
                            
                serialPort.Open();
                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task Read(ItemModel model)
        {
            try
            {
                byte[] bytes = new byte[8];
                bytes[0] = (byte)model.SlaveId;
                bytes[1] = 0x03;
                bytes[2] = 0x00;
                bytes[3] = 0x00;
                bytes[4] = 0x00;
                bytes[5] = 0x02;
                CRC16(bytes);
                this.serialPort.Write(bytes, 0, bytes.Length);
                
                await Task.Delay(100);
                byte[] respBytes = new byte[serialPort.BytesToRead];
                
                this.serialPort.Read(respBytes, 0, respBytes.Length);
                
                byte[] checkArray = new byte[respBytes.Length];
                
                Array.Copy(respBytes, checkArray, respBytes.Length);
                
                this.CRC16(checkArray);
                if (!Enumerable.SequenceEqual(respBytes, checkArray))
                    throw new Exception("接收报文校验错误");
                if ((checkArray[1] & 0x80) == 0x80)
                    throw new Exception("异常码-" + checkArray[2]);
                var temp = BitConverter.ToInt16(new byte[] { checkArray[4], checkArray[3] }, 0);
                var humi = BitConverter.ToInt16(new byte[] { checkArray[6], checkArray[5] }, 0);
                
                model.Temperature = temp*0.1;
                model.Humidity = humi*0.1;



            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void CRC16(byte[] value)
        {
            if (value == null || !value.Any())
                throw new ArgumentException("");

            ushort crc = 0xFFFF;
            for (int i = 0; i < value.Length-2; i++)
            {
                crc = (ushort)(crc ^ value[i]);
                for (int j = 0; j < 8; j++)
                {
                    crc=(crc&i)!=0?(ushort)((crc>>1) ^ 0xA001):(ushort)(crc>>1);
                    
                }
                
            }
            byte hi =(byte)((crc & 0xFF00)>>8); //高位置
            byte lo =(byte)(crc & 0xFF) ;//低位置
            
            value[value.Length - 2] = lo;
            value[value.Length - 1] = hi;
            


        }
    }
}

