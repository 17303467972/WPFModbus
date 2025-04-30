using System.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;

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
        
        public ChartValues<LvcModel> TemperatureValues { get; set; }=new ChartValues<LvcModel>();
        
        public ChartValues<LvcModel> HumidityValues { get; set; }=new ChartValues<LvcModel>();

        public ItemModel()
        {
            CartesianMapper<LvcModel> mapper =Mappers.Xy<LvcModel>()
                .X(models=>models.DateTime.Ticks)
                .Y(models=>models.Value);
            Charting.For<LvcModel>(mapper);

            XFormatter = new Func<double, string>(x => new DateTime((long)x).ToString("mm:ss"));
            
            UpdateRange();
        }
        
        public Func<double,string> XFormatter { get; set; }

        private double _axisMin;

        public double AxisMin
        {
            get
            {
                return _axisMin;
                
            }
            set
            {
                _axisMin = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("AxisMin"));
            }
        }
        
        private double _axisMax;

        public double AxisMax
        {
            get
            {
                return _axisMax;
                
            }
            set
            {
                _axisMax = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs("AxisMax"));
            }
        }

        private void UpdateRange()
        {
            AxisMax = DateTime.Now.Ticks;
            AxisMin = DateTime.Now.Ticks-TimeSpan.FromSeconds(9).Ticks;
        }
        public double AxisUnit{get;set;}=TimeSpan.TicksPerSecond;
        public double AxisStep{get;set;}=TimeSpan.FromSeconds(1).Ticks;


    }
}

