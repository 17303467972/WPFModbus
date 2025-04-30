using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ModbusTest.Components
{
    public partial class Meter : UserControl
    {
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
               SetValue(ValueProperty, value);
            }
            
        }
        public static readonly DependencyProperty ValueProperty=
            DependencyProperty.Register("Value",typeof(double),typeof(Meter),
                new PropertyMetadata(0.0,new PropertyChangedCallback(OnValueChanged)));
        
        //监听圆弧上的数值变化

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Meter).Refreshvlaue();
        }
        public Meter()
        {
            InitializeComponent();
            this.SizeChanged += Meter_SizeChanged;
            
        }

        private void Meter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshScale();
        }

        double radius = 0.0;
        private double step = 0.0;

        private void RefreshScale()
        {
            this.root.Width=Math.Min(this.RenderSize.Width, this.RenderSize.Height);
            this.root.Height = this.root.Width;
            radius = this.root.Height / 2;
            if (double.IsNaN(radius) || radius<=0)return;
            
            scale_canvas.Children.Clear();
            
            double minValue = -40,maxValue=80;
            int interval = 10;
            step=270.0/(maxValue-minValue+24);
            for (int i = 0; i <= maxValue-minValue+24; i++)
            {
                if (i % 12 != 0) continue;
                
                double angle =step*i-45 ;
                Line lineScale = new Line();
                lineScale.Stroke=Brushes.White;
                lineScale.StrokeThickness = 1;
                lineScale.Opacity = 0.3;
                
                lineScale.X1=radius-(radius-15)*Math.Cos(angle*Math.PI/180);
                lineScale.Y1=radius-(radius-15)*Math.Sin(angle*Math.PI/180);
                lineScale.X2=radius-(radius-12)*Math.Cos(angle*Math.PI/180);
                lineScale.Y2=radius-(radius-12)*Math.Sin(angle*Math.PI/180);
                
                this.scale_canvas.Children.Add(lineScale);
                
                TextBlock textScale = new TextBlock();
                textScale.Text=(minValue+interval*i/12).ToString();
                textScale.Width = 34;
                textScale.FontSize = 9;
                textScale.Foreground=Brushes.White;
                textScale.Opacity = 0.3;
                textScale.TextAlignment = TextAlignment.Center; 
                
                Canvas.SetLeft(textScale, radius-(radius-2)*Math.Cos(angle*Math.PI/180)-19);
                Canvas.SetTop(textScale, radius-(radius-2)*Math.Sin(angle*Math.PI/180)-5);
                this.scale_canvas.Children.Add(textScale);
            }
            
            
            //M0 100 A100 100 0 1 1 100 200
            string path_data_str = $"M{radius * 0.3} {radius}" +
                                   $"A{radius * 0.7} {radius * 0.7} 0 1 1 {radius} {radius * 1.7}";

            this.path.Data = Geometry.Parse(path_data_str);
            this.Refreshvlaue();
            
        }

        private void Refreshvlaue()
        {
            double newAngle = (this.Value - -40) * 270 / 120;
           
            
            double x = radius-(radius-21)*Math.Cos(newAngle*Math.PI/180)-2;
            double y = radius-(radius-21)*Math.Sin(newAngle*Math.PI/180)-2;
            
            int flag = newAngle<=180?0:1;
            string path_data_str = $"M{radius * 0.3} {radius}" +
                                   $"A{radius * 0.7} {radius * 0.7} 0 {flag} 1 {x} {y}";

            this.path_value.Data = Geometry.Parse(path_data_str);
            
        }
    }
}
