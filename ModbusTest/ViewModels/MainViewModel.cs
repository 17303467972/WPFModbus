using ModbusTest.Models;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        public List<ItemModel> ItemList { get; set; }

        public MainViewModel()
        {
            ItemList = new List<ItemModel>();
            ItemList.Add(new ItemModel(){SlaveId=1});
            ItemList.Add(new ItemModel(){SlaveId=2});
            ItemList.Add(new ItemModel(){SlaveId=3});
            ItemList.Add(new ItemModel(){SlaveId=4});

            Task.Run(async () =>
            {
                while (true)
                {
                    CurrentDate = DateTime.Now;
                    await Task.Delay(new TimeSpan(0, 1, 0));
                }
            });
            
            PortNameList=SerialPort.GetPortNames().ToList();

            ConfigCommand = new Command(Config);
            ConfigCloseCommand = new Command(ConfigClose);
            LogPathSelectCommand = new Command(LogPathSelect);
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
        
    }
}

