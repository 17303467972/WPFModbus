using ModbusTest.Models;
using System.Collections.Generic;
using System.ComponentModel;

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
        
        public List<ItemModel> ItemList { get; set; }

        public MainViewModel()
        {
            ItemList = new List<ItemModel>();
            ItemList.Add(new ItemModel(){SlaveId=1});
            ItemList.Add(new ItemModel(){SlaveId=2});
            ItemList.Add(new ItemModel(){SlaveId=3});
            ItemList.Add(new ItemModel(){SlaveId=4});
        }
    }
}

