using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Xna.Framework;


using System.Threading;

//using System.Windows.Forms;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        kinectApp app;
        int valueChanged = 0;
        private featureExtractor fExtract;
      
        private bool learnOn, recordOn, detectOn, settingsOn;

        public MainWindow()
        {
            
            InitializeComponent();
           // populateListbox();

            globalVars.resultChart = lineChart;
            globalVars.lseries = arousalpoints;
            int gpt;
            try { gpt = (int)Convert.ToInt32(graphPt.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); gpt = 30;
            }

            globalVars.chart = new resultViz(lineChart, gpt, 2);
            globalVars.chart2 = new resultViz(featureChart, gpt, 5);

            slider1.Value = (double)-3;
            slider1.Maximum = Camera.ElevationMaximum;
            slider1.Minimum = Camera.ElevationMinimum;
        }
        private void populateListbox(){
            ObservableCollection <string> list1 = new ObservableCollection<string>();
            list1.Add("BipolarSigmoid");
            list1.Add("BinarySigmoid");
            comboBox1.ItemsSource = list1;
            comboBox1.SelectedItem = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            learnOn = false;
            detectOn = true;
            recordOn = false;
            settingsOn = false;
            //label1.Content = "ahhaaaha";
            //labelFrame.Content = "frame";
            globalVars.screenH = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            globalVars.screenW = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth;

            
            globalVars.a1 = labelFrame;
            //globalVars.a2 = wDistLabel;
            globalVars.error = AnnError;
            globalVars.annIter = iterCount;
            globalVars.saveANNbutn = saveANNbutn;
            //globalVars.jerkLabel = jerkLabel;
            globalVars.AnnOutput = outputLabel;
            globalVars.typeOfLearning = 1;//= Convert.ToInt32(typeofann.Text.ToString());
            try { globalVars.outputCount = Convert.ToInt32(outputCount.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.outputCount = 1;
            }

            try { globalVars.hiddenCount = Convert.ToInt32(hiddenCount.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.hiddenCount = 4;
            }


            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;

            double a,b,c,d,ef;

            try { a = Math.Abs(Convert.ToDouble(lThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); a = 0.005;
            }

            try { b = Math.Abs(Convert.ToDouble(uThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); b=1;
            }

            if (a >= b) 
            {
                System.Windows.MessageBox.Show("Lower Threshold can not be greater or eqal to Upper Threshold. Reverting back to default Values of 0.005 and 1", "you moron.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                lThres.Text = "0.005";
                uThres.Text = "1";
                a = 0.005;
                b = 1;
            }

            try { c = Math.Abs(Convert.ToDouble(surgeThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); c=10;
            }

            try { d = Math.Abs(Convert.ToDouble(ufDelay.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); d = 0.2;
            }

            try { ef = Math.Abs(Convert.ToDouble(gfDelay.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); ef = 1;
            }

            if (d >= ef)
            {
                System.Windows.MessageBox.Show("UpdateFeature delay can not be greater or eqal to EmotionRecognition delay. Reverting back to default Values of 0.2 and 1", "you moron.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                lThres.Text = "0.2";
                uThres.Text = "1";
                d = 0.2;
                ef = 1;
            }

            //initializig feature extractor and passing the update graph.


            fExtract = new featureExtractor(d, ef, a, b, c, globalVars.chart2);
            globalVars.fExtract = this.fExtract;

            //Thread kinAppThrd = new Thread(() => { app = new kinectApp(canvas1, canvas2, label1, image1, fExtract); });
            //kinAppThrd.Start();

            app = new kinectApp(canvas1, canvas2, label1, image1, fExtract);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //fExtract.thrd.Join();
            globalVars.chartRighthand = false;
            app.stopApp();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            valueChanged = 1;
        }

        private void slider1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (globalVars.kinectOn == true && valueChanged == 1 && (int)slider1.Value >= Camera.ElevationMinimum && (int)slider1.Value <= Camera.ElevationMaximum)
                app.moveCamera((int)slider1.Value);
            valueChanged = 0;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            //record feature
            if (globalVars.kinectOn == true)
            {
                globalVars.logFeatures = true;
                recordFeature.IsEnabled = false;
                stopRecord.IsEnabled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Kinect is not running. Please Plug in the Kinect and try again when the green light is on.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //stop recording
            globalVars.logFeatures = false;
            /*"this is not needed now"*/
            //globalVars.gFeature.saveFeatures();
            
            fExtract.saveFeatures();

            recordFeature.IsEnabled = true;
            stopRecord.IsEnabled = false;
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
        //    if (checkBox1.IsChecked == true)
        //        globalVars.logSkele = true;
        //    else
        //        globalVars.logSkele = false;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
        //    if (checkBox1.IsChecked == true)
        //        globalVars.logSkele = true;
        //    else
        //        globalVars.logSkele = false;
            
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ///Start Emotion Detection
            if (globalVars.detectorOn == false)
            {
                if (globalVars.kinectOn == false)
                    System.Windows.MessageBox.Show("Kinect is not running. Please Plug in the Kinect and try again when the green light is on.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    try
                    {


                        //elow, this was for just a test purpose
                        //recognizer R = new recognizer(loadANN.Text.ToString());

                        globalVars.detector = new newDynamicDetection(loadANN.Text.ToString());
                        try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
                        catch
                        {
                            System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval = 10;
                        }


                        globalVars.detectorOn = true;
                        startDetect.Content = "Stop Emotion Detection";

                        ufDelay.IsEnabled = false;
                        gfDelay.IsEnabled = false;

                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("osme problem with detector moduel.", "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                ufDelay.IsEnabled = true;
                gfDelay.IsEnabled = true;

                globalVars.detector.stopDetection();
                globalVars.detectorOn = false;
                startDetect.Content = "Start Emotion Detection";
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //String[] str =textBox1.Text.ToString().Split(':');
            ///Start Learning
            //System.Windows.MessageBox.Show(textBox1.Text.ToString(), "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            try
            {
                //System.Windows.MessageBox.Show("number " + " " + textBox1.Text.ToString(), "meh.", MessageBoxButton.OK, MessageBoxImage.Error);

                try { globalVars.outputCount = Convert.ToInt32(outputCount.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.outputCount = 1;
                }

                try { globalVars.hiddenCount = Convert.ToInt32(hiddenCount.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.hiddenCount = 4;
                }



                globalVars.typeOfLearning = Convert.ToInt32(((ComboBoxItem)comboBox1.SelectedItem).Tag.ToString());
                Learner L = new Learner(textBox1.Text.ToString());
                
            }
            catch {
                System.Windows.MessageBox.Show("Enter a filename", "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //nothing
        }

        private void browsebtn_Click(object sender, RoutedEventArgs e)
        {
            
            System.Windows.Forms.OpenFileDialog browse = new System.Windows.Forms.OpenFileDialog();
            
            if (browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox1.Text = browse.FileName;
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog browse1 = new System.Windows.Forms.OpenFileDialog();

            if (browse1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                loadANN.Text = browse1.FileName;
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            //Save ANN Button
            Learner.saveANNtoFile();

        }

        private void ArousalChkbx_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ArousalChkbx.IsChecked == true )
                {
                    arousalLev.IsEnabled = true;
                }
                else if (ArousalChkbx.IsChecked == false)
                {
                    arousalLev.IsEnabled = false;
                }
            }
            catch { }

        }

        private void valenceChkbx_Checked(object sender, RoutedEventArgs e)
        {
                valenceLev.IsEnabled = true;
        }

        private void ArousalChkbx_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ArousalChkbx.IsChecked == true)
                {
                    arousalLev.IsEnabled = true;
                }
                else if (ArousalChkbx.IsChecked == false)
                {
                    arousalLev.IsEnabled = false;
                }
            }
            catch { }
        }

        private void valenceChkbx_Unchecked(object sender, RoutedEventArgs e)
        {
               
                valenceLev.IsEnabled = false;

        }

        private void updateIntervalText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (globalVars.detector != null && updateIntervalText.Text.ToString() != null)
            {
                try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval=10;
                }
            }
        }

        private void reducedFeature_Checked(object sender, RoutedEventArgs e)
        {
            globalVars.reducedRecord = true;
        }

        private void reducedFeature_Unchecked(object sender, RoutedEventArgs e)
        {
            globalVars.reducedRecord = false;
        }

        private void button1_Click_2(object sender, RoutedEventArgs e)
        {
            globalVars.detector = new newDynamicDetection(loadANN.Text.ToString());

            try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval=10;
            }
            globalVars.detectorOn = true;
            globalVars.detector.test();
        }

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            if (globalVars.detector != null)
                globalVars.detector.SlidingWindowOn = true;


        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            if (globalVars.detector != null)
                globalVars.detector.SlidingWindowOn = false;

        }

        private void DetectMenu_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void DetectMenu_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void DetectMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            detectOn = !detectOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.DeepSkyBlue;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Visible;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void RecordMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            recordOn = !recordOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.DeepSkyBlue;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;    
            recordCanvas.Visibility = System.Windows.Visibility.Visible;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void LearnMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            learnOn = !learnOn;
            LearnMenu.Foreground = Brushes.DeepSkyBlue;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Visible;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void advancedMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            settingsOn = !settingsOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.DeepSkyBlue;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Visible;
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            //app.stopApp();

        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //comboBox1.Tag.ToString();
        }

        private void rHandChartingOn_Checked(object sender, RoutedEventArgs e)
        {
            globalVars.chartRighthand = true;
        }

        private void rHandChartingOn_Unchecked(object sender, RoutedEventArgs e)
        {
            globalVars.chartRighthand = false;
        }

    }




    /*Achtung: This is the main kinect app which extracts the kinect output*/




    public class kinectApp //: MainWindow
    {
        fileWriter file;
        startFeatures features;
        

        private Runtime kinect;
        private Canvas canvas, topCanvas;
        Label spinePos;
        featureExtractor fExtractor;
        
        public kinectApp(Canvas c, Canvas top, Label l, Image i, featureExtractor featExtractor)
        {
            //update the feature
 
            this.fExtractor = featExtractor;            
            this.features  = new startFeatures();
            
            globalVars.gFeature = features;
            //globalVars.mFeature = 


            this.file = new fileWriter(null);

            
            kinect = Runtime.Kinects[0];

            if (kinect == null)
            {
                globalVars.kinectOn = false;
                string messageBoxText = "Please Switch on the Kinect and connect the USB to the computer";
                string caption = "Kinect Not Connected";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

            kinect.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);
            if (kinect != null)
            {
                //l.Content = "Kinect on";
                globalVars.kinectOn = true;
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri("/Haixiu;component/images/green.png", UriKind.Relative);
                bi3.EndInit();
                i.Source = bi3;

                var parameters = new TransformSmoothParameters();
                kinect.SkeletonEngine.TransformSmooth = true;

                parameters.Smoothing = 0.5f;
                parameters.Correction = 0.5f;
                parameters.Prediction = 0.1f;
                parameters.JitterRadius = 0.1f;
                parameters.MaxDeviationRadius = 0.5f;

                // Enable Smoothing
                kinect.SkeletonEngine.SmoothParameters = parameters;
                //sketopframeready
                kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
                kinect.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(kinect_VideoFrameReady);

                this.canvas = c;
                this.topCanvas = top;
                this.spinePos = l;
            }

 

        }

        void kinect_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            foreach (SkeletonData skeleData in e.SkeletonFrame.Skeletons)
            {
                if (SkeletonTrackingState.Tracked == skeleData.TrackingState)
                {
                    this.canvas.Children.Clear();
                    foreach (JointID i in globalVars.jid)
                    {
                        if (skeleData.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            drawJoint(skeleData, i);
                           // topView(skeleData, JointID.ShoulderLeft, JointID.ShoulderRight);

                        }
                    }
                    if (globalVars.chartRighthand == true)
                    {
                        this.fExtractor.getFeatures(skeleData, spinePos);
                        globalVars.a1.Content = this.fExtractor.frames;
                    }

                    //feature logging
                    if (globalVars.logFeatures == true)
                    {
                        // replaced with neww featuee extractor
                        //features.addFeatures(skeleData);
                        this.fExtractor.getFeatures(skeleData, spinePos);
                        globalVars.a1.Content = this.fExtractor.frames;

                    }
                    //feature detection

                    if (globalVars.detectorOn == true)
                        globalVars.detector.pollFeatures(skeleData);
                }
                
            }
            //throw new NotImplementedException();
        }
        private void drawJoint(SkeletonData sd, JointID i)
        {
            Joint j;
            dot a;
            j = sd.Joints[i];
            j = j.ScaleTo(310, 244, 1.6f,1.6f);
            Point p = new Point(j.Position.X, j.Position.Y);
            Point off = new Point(0, 5);

            if (i == JointID.Head)
            {
                a = new dot(7,7,2,Brushes.Blue, off);
            }

            else
            {
                a = new dot(7, 7, 2, Brushes.Red, off);
            }
            a.moveDot(canvas, p);
        }
        private void topView(SkeletonData sd, params JointID[] ids)
        {

            Joint j;
            PointCollection points = new PointCollection(ids.Length);
            topCanvas.Children.Clear();
            foreach (JointID i in ids)
            {
                j= sd.Joints[i];
                j = j.ScaleTo(340, 200, .7f, .7f);
                Point p = new Point(j.Position.X, j.Position.Z.scale(1,200));
                points.Add(p);
            }
            Polyline polyline = new Polyline();
            polyline.Points = points;
            polyline.Stroke = Brushes.Red;
            polyline.StrokeThickness = 5;
            Canvas.SetTop(polyline, sd.Joints[JointID.Spine].Position.Z.scale(1,35));
            topCanvas.Children.Add(polyline);

            //spinePos.Content = "righthandPos: X: " + sd.Joints[JointID.HandRight].Position.X + ", Y: " + sd.Joints[JointID.HandRight].Position.Y +", Z: " + sd.Joints[JointID.HandRight].Position.Z;
            
            if (globalVars.logSkele == true)
            {
                file.writeLog(sd);
            }


        }

        public void stopApp()
        {
            globalVars.needToStopLearning = true;
            this.kinect.Uninitialize();
            if(globalVars.logSkele == true)
                this.file.closeFile();
            if (globalVars.ANNthread != null)
                globalVars.ANNthread.Join();
            globalVars.ANNthread = null;
        }

        public void moveCamera(int a)
        {
            this.kinect.NuiCamera.ElevationAngle = a;
        }
        
    }



    public static class depthConverter
    {

        public static float scale(this float a, int b, int off)
            /*scales the depth co ord*/
        {
            return (a-b) * off;
        }
    }


    public class dot
    {
        Ellipse a;
        Point offset;
        public dot() 
        {
            this.a = new Ellipse { Height = 15, Width = 15, StrokeThickness = 4, Stroke = Brushes.Red };
            this.offset = new Point(0,0);
        }
        public dot(int h, int w, int thick, Brush b, Point offset)
        {
            this.a = new Ellipse { Height = h, Width = w, StrokeThickness = thick, Stroke = b };
            this.offset = offset;
        }

        public void moveDot(Canvas c, Point p)
        {
            Canvas.SetLeft(this.a, p.X + offset.X);
            Canvas.SetTop(this.a, p.Y + offset.Y);
            c.Children.Add(this.a);
            
        }
    }

    public class bone
    {
        Line l;
        public bone()
        {
            this.l = new Line();

        }

    }

}
