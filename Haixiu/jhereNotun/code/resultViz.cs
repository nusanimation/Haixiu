﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Collections.ObjectModel;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Documents;



namespace WpfApplication1
{
    public class resultViz
    {
        circumplexViz cViz, cVizBig;

        int length = 30, iter=0;
        private ObservableCollection<KeyValuePair<string, double>>[] Power;
        private ObservableCollection<KeyValuePair<string, double>> Power1;
        public ObservableCollection<ObservableCollection<KeyValuePair<string, double>>> dataSourceList;
        Chart chart;
        public resultViz(Chart chart, int GraphLength, int numOfLines = 1) {

            cViz = new circumplexViz(globalVars.Circumplex);
            cVizBig = new circumplexViz();

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
            cViz.draw(data[0], data[1], 3.3, 2.25);
            if (globalVars.isCircmplexBigOpen == true )
            {
                cVizBig.canvasToDraw = globalVars.CircumplexBig;
                cVizBig.draw(data[0], data[1], 7, 7);

            }
            else if (globalVars.isCircmplexBigOpen == false)
            {
                cVizBig.canvasToDraw = null;
            }
            

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

    public class circumplexViz
    {
        bone xAxis, yAxis;
        dot d;
        int counter;
        int threshold;
        Point prev;
        bool reddot = true;

        dotQ dotqueue = new dotQ();
        bool firstTimeSmall = true; bool firstTimeBig = true;

        public Canvas canvasToDraw;
        private Canvas previousCanvas;

        public circumplexViz(Canvas canvasToDraw = null,int threshold = 3)
        {
            this.canvasToDraw = canvasToDraw;
            Point p1 = new Point(0,200);
            Point p2 = new Point(0,0);
            counter = 1;
            this.threshold = threshold;
            d = new dot(1, counter);
            if (this.canvasToDraw != null)
            {
                xAxis = new bone(13, 0, 13, 244);
                yAxis = new bone(3, 234, 330, 234);
                //a = new dot(7, 7, 2, Brushes.Red, off);
                xAxis.drawBone(this.canvasToDraw);
                yAxis.drawBone(this.canvasToDraw);
            }            // xAxis.moveBone(this.canvasToDraw, p2);

            
            
        }

        public void draw(double Arousal, double Valence, double offsetA, double offsetV) 
        {
            if (this.previousCanvas != null && this.previousCanvas != this.canvasToDraw)
            {
                //Basically refreshing everything. :/
                try { previousCanvas.Children.Remove(d.ellipse); }
                catch { System.Windows.MessageBox.Show("kaput.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error); }
                reddot = true;
                dotqueue.numberOfDots = 0;
            }
            ++counter;
            //this counter is for name of the dot

            //dot a;
            Point p = new Point(Math.Round(Valence * offsetV), Math.Round((100 - Arousal) * offsetV));
            Point off = new Point(0, 5);
            if (reddot == true)
            {
                d.moveDot(this.canvasToDraw, p);
                animate(d.ellipse);
                reddot = false;
            }
            else
            {
                //d.refreshDot(this.canvasToDraw, p);
                smoothmove(d.ellipse, prev, p);
                dot pdot = new dot(2);//, counter);
                pdot.moveDot(this.canvasToDraw, prev);
                //animate(pdot.ellipse);
                dotqueue.Enqueue(pdot);

            }

            if (dotqueue.numberOfDots >= this.threshold)
            {
                fadeOut(dotqueue.Dequeue().ellipse);
            }


            prev = p;
            previousCanvas = this.canvasToDraw;

        }


        /**** Animation part ****/


        public void animate(Ellipse elem)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));


            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;


            
        Storyboard myStoryboard;
        myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, elem.Name);
            Storyboard.SetTarget(myDoubleAnimation, elem);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Ellipse.OpacityProperty));
            myStoryboard.Begin();
        }

        public void fadeOut(Ellipse elem)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));


            //myDoubleAnimation.AutoReverse = true;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;


            
        Storyboard myStoryboard;
        myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, elem.Name);
            Storyboard.SetTarget(myDoubleAnimation, elem);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Ellipse.OpacityProperty));
            myStoryboard.Begin();
        }

        public void smoothmove (Ellipse elem, Point prev, Point newpoint)
        {
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = prev.Y;
            myDoubleAnimation.To = newpoint.Y;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            //myDoubleAnimation.AutoReverse = true;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard myStoryboard;
            myStoryboard = new Storyboard();
            myStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, elem.Name);
            Storyboard.SetTarget(myDoubleAnimation, elem);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.TopProperty));
            myStoryboard.Begin();


            DoubleAnimation myDoubleAnimation1 = new DoubleAnimation();
            myDoubleAnimation1.From = prev.X;
            myDoubleAnimation1.To = newpoint.X;
            myDoubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(1));
            //myDoubleAnimation.AutoReverse = true;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard myStoryboard1;
            myStoryboard1 = new Storyboard();
            myStoryboard1.Children.Add(myDoubleAnimation1);
            //Storyboard.SetTargetName(myDoubleAnimation, elem.Name);
            Storyboard.SetTarget(myDoubleAnimation1, elem);
            Storyboard.SetTargetProperty(myDoubleAnimation1, new PropertyPath(Canvas.LeftProperty));
            myStoryboard1.Begin();

        }
    }


    public class dotQ
    {
        /// <summary>Used as a lock target to ensure thread safety.</summary>
        //private readonly Locker _Locker = new Locker();
        public int numberOfDots = 0;

        private readonly System.Collections.Generic.Queue<dot> _Queue = new System.Collections.Generic.Queue<dot>();

        /// <summary></summary>
        public void Enqueue(dot dot)
        {
            lock (this)
            {
                _Queue.Enqueue(dot);
                //numberOfDots++;
                numberOfDots = _Queue.Count;
            }
        }
        public dot Dequeue()
        {
            dot data;

            lock (this)
            {
                if (_Queue.Count > 0)
                {
                    data = _Queue.Dequeue();
                    return data;
                    //numberOfDots--;
                    numberOfDots = _Queue.Count;

                }
                else
                    return null;
            }
        }
    }

}
