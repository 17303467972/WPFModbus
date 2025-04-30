using System.ComponentModel;

namespace ModbusTest.Models
{
    public class ItemModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public int SlaveId{ get; set; }
        
        private double _temperature;

        public double Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("Temperature"));
            }
        } 
        
        private double _humidity;

        public double Humidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                _humidity = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("Humidity"));
            }
        } 

    }
}

