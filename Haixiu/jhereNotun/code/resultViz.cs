using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WpfApplication1
{
    public class resultViz
    {
        int length = 30, iter=0;
        ObservableCollection<KeyValuePair<string, double>> Power;

        public resultViz() {

            Power = new ObservableCollection<KeyValuePair<string, double>>();
            Power.Add(new KeyValuePair<string, double>("0", 0.0));

            showColumnChart();
            //DispatcherTimer timer = new DispatcherTimer();

            //timer.Interval = new TimeSpan(0, 0, 5);  // per 5 seconds, you could change it
            //timer.Tick += new EventHandler(timer_Tick);
            //timer.IsEnabled = true;
        }

        //int i = 5;
        //Random random = new Random(DateTime.Now.Millisecond);
        //void timer_Tick(object sender, EventArgs e)
        //{
        //    Power.Add(new KeyValuePair<int, double>(i, random.NextDouble()));
        //    i += 5;
        //}

        private void showColumnChart()
        {
            

                  //  LineSeries series = new LineSeries();
                    Style dataPointStyle = GetNewDataPointStyle();
                  //  series.DataPointStyle = dataPointStyle;
            
            globalVars.lseries.DataPointStyle = dataPointStyle;
            globalVars.resultChart.DataContext = Power;
            //        series.DataContext = Power;

                    //globalVars.resultChart.Series.Add(series);
                
    
        }

        /// <summary>
        /// Gets the new data point style.
        /// </summary>
        /// <returns></returns>
        
        private static Style GetNewDataPointStyle()
        {
            LinearGradientBrush myLinearGradientBrush =
    new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0.5, 1);
            myLinearGradientBrush.EndPoint = new Point(0.5, 0);
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Blue, 0.0));
            myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Red, 1.0));  


            Color background = Color.FromRgb((byte)255,
                                         (byte)0,
                                         (byte)0);

            Style style = new Style(typeof(DataPoint));

            Setter st1 = new Setter(DataPoint.BackgroundProperty,
                                        myLinearGradientBrush);
            Setter st2 = new Setter(DataPoint.BorderBrushProperty,
                                        new SolidColorBrush(Colors.White));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));

            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);
            style.Setters.Add(st3);
            style.Setters.Add(st4);

            return style;

        }


        public void update(double data)
        {
            iter++;
            if (this.Power.Count >= this.length)
            {
                this.Power.RemoveAt(0);
            }
            if (data>=-1 && data <= 100)
                this.Power.Add(new KeyValuePair<string, double>(DateTime.Now.Hour+ ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, (data)));
            else
                this.Power.Add(new KeyValuePair<string, double>(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, -1));
            
        }

    }
}
