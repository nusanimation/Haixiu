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
        private ObservableCollection<KeyValuePair<string, double>>[] Power;
        private ObservableCollection<KeyValuePair<string, double>> Power1;
        public ObservableCollection<ObservableCollection<KeyValuePair<string, double>>> dataSourceList;
        Chart chart;
        public resultViz(Chart chart, int GraphLength, int numOfLines = 1) {


            if (numOfLines > 2)
            {
                Power = new ObservableCollection<KeyValuePair<string, double>>[numOfLines];

                this.dataSourceList = new ObservableCollection<ObservableCollection<KeyValuePair<string, double>>>();
                for (int ii = 0; ii < numOfLines; ii++)
                {
                    Power[ii] = new ObservableCollection<KeyValuePair<string, double>>();
                    Power[ii].Add(new KeyValuePair<string, double>("0", 0.0));
                    this.dataSourceList.Add(Power[ii]);
                }

                //            this.dataSourceList.Add(Power[0]);
                this.chart = chart;
                if (GraphLength > 0 && GraphLength < 1000)
                    this.length = (int)Math.Abs(GraphLength);
       
                this.chart.DataContext = dataSourceList;
         
            }
            else if (numOfLines == 2)
            {
                {
                    Power = new ObservableCollection<KeyValuePair<string, double>>[numOfLines];

                    this.dataSourceList = new ObservableCollection<ObservableCollection<KeyValuePair<string, double>>>();
                    for (int ii = 0; ii < numOfLines; ii++)
                    {
                        Power[ii] = new ObservableCollection<KeyValuePair<string, double>>();
                        Power[ii].Add(new KeyValuePair<string, double>("0", 0.0));
                        this.dataSourceList.Add(Power[ii]);
                    }

                    //            this.dataSourceList.Add(Power[0]);
                    this.chart = chart;
                    if (GraphLength > 0 && GraphLength < 1000)
                        this.length = (int)Math.Abs(GraphLength);

                    this.chart.DataContext = dataSourceList;

                    //globalVars.resultChart.Series.Add(series);
                    Style dataPointStyle = GetNewDataPointStyle(1);
                    //series.DataPointStyle = dataPointStyle;
                    globalVars.aseries.DataPointStyle = dataPointStyle;

                    //globalVars.resultChart.Series.Add(series);
                    Style dataPointStyle1 = GetNewDataPointStyle(2);
                    //series.DataPointStyle = dataPointStyle;
                    globalVars.vseries.DataPointStyle = dataPointStyle1;
                }
            }
            else if (numOfLines == 1)
            {
                //LineSeries series = new LineSeries();
                Power1 = new ObservableCollection<KeyValuePair<string, double>>();
                Power1 = new ObservableCollection<KeyValuePair<string, double>>();
                Power1.Add(new KeyValuePair<string, double>("0", 0.0));


                globalVars.aseries.DataContext = Power1;
                //globalVars.resultChart.Series.Add(series);
                Style dataPointStyle = GetNewDataPointStyle(1);
                //series.DataPointStyle = dataPointStyle;
                globalVars.aseries.DataPointStyle = dataPointStyle;
            }
            else
                System.Windows.MessageBox.Show("somethings wrong creating the chart.", "error", MessageBoxButton.OK, MessageBoxImage.Error);

        }


        private void showColumnChart()
        {
            
           
        }

        /// <summary>
        /// Gets the new data point style.
        /// </summary>
        /// <returns></returns>
        
        private static Style GetNewDataPointStyle(int op)
        {
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0.5, 1);
            myLinearGradientBrush.EndPoint = new Point(0.5, 0);
            if (op == 1)
            {
                myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Cyan, 0.0));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.DarkBlue, 1.0));
            }
            if (op == 2)
            {
                myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Pink, 0.0));
                myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.DarkRed, 1.0));
            }

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
            if (this.Power1.Count >= this.length)
            {
                this.Power1.RemoveAt(0);
            }
            if (data>=-1 && data <= 100)
                this.Power1.Add(new KeyValuePair<string, double>(DateTime.Now.Hour+ ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, (data)));
            else
                this.Power1.Add(new KeyValuePair<string, double>(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, -1));
            
        }

        public void update(double[] data)
        {
            iter++;
            int i;
            for (i = 0; i < data.Length; i++)
            {

                if (this.Power[i].Count >= this.length)
                {
                    this.Power[i].RemoveAt(0);
                }
                if (data[i] >= -1 && data[i] <= 100)
                    this.Power[i].Add(new KeyValuePair<string, double>(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, (data[i])));
                else
                    this.Power[i].Add(new KeyValuePair<string, double>(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, -1));
            }

        }

    }
}
