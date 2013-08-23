using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace WpfApplication1
{
    public class resultViz
    {
        int length = 30, iter=0;
        ObservableCollection<KeyValuePair<string, double>> Power;

        public resultViz() {

            Power = new ObservableCollection<KeyValuePair<string, double>>();
            Power.Add(new KeyValuePair<string, double>("0", 0.0));
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
            globalVars.resultChart.DataContext = Power;
        }

        public void update(double data)
        {
            iter++;
            if (this.Power.Count >= this.length)
            {
                this.Power.RemoveAt(0);
            }
            if (data>=-1 && data <= 1)
                this.Power.Add(new KeyValuePair<string, double>(DateTime.Now.Hour+ ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, (data+1)*50));
            else
                this.Power.Add(new KeyValuePair<string, double>(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second, -1));
            
        }

    }
}
