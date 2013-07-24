using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    public partial class MainWindow : Window
    {
        kinectApp app;
        int valueChanged = 0;

        public MainWindow()
        {
            InitializeComponent();
            slider1.Value = (double)-3;
            slider1.Maximum = Camera.ElevationMaximum;
            slider1.Minimum = Camera.ElevationMinimum;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //label1.Content = "ahhaaaha";
            //labelFrame.Content = "frame";
            globalVars.a1 = labelFrame;
            globalVars.a2 = wDistLabel;
            globalVars.error = AnnError;
            globalVars.annIter = iterCount;
            globalVars.saveANNbutn = saveANNbutn;


            app = new kinectApp(canvas1, canvas2, label1, image1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
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
                System.Windows.MessageBox.Show("output:",
                  "Output for test .1,.5", MessageBoxButton.OK,
                  MessageBoxImage.Information);
                globalVars.logFeatures = true;
                recordFeature.IsEnabled = false;
                stopRecord.IsEnabled = true;
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //stop recording
            globalVars.logFeatures = false;
            globalVars.gFeature.saveFeatures();
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

            try
            {

                recognizer R = new recognizer(loadANN.Text.ToString());

            }
            catch
            {
                System.Windows.MessageBox.Show("Enter a filename", "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //String[] str =textBox1.Text.ToString().Split(':');
            ///Start Learning
            //System.Windows.MessageBox.Show(textBox1.Text.ToString(), "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            try
            {
                
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




    }


    public class kinectApp 
    {
        fileWriter file;
        startFeatures features;
        

        private Runtime kinect;
        private Canvas canvas, topCanvas;
        Label spinePos;

        
        public kinectApp(Canvas c, Canvas top, Label l, Image i)
        {
            //update the feature
            
            this.features  = new startFeatures();
            globalVars.gFeature = features;

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

            kinect.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
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


                this.canvas = c;
                this.topCanvas = top;
                this.spinePos = l;
            }

 

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
                            topView(skeleData, JointID.ShoulderLeft, JointID.ShoulderRight);

                        }
                    }
                    //feature logging
                    if (globalVars.logFeatures == true)
                        features.addFeatures(skeleData);

                }
                
            }
            //throw new NotImplementedException();
        }
        private void drawJoint(SkeletonData sd, JointID i)
        {
            Joint j;
            dot a;
            j = sd.Joints[i];
            j = j.ScaleTo(800, 680, 1.6f,1.6f);
            Point p = new Point(j.Position.X, j.Position.Y);
            if (i == JointID.Head)
            {
                Point off = new Point(0,5);
                a = new dot(15,15,4,Brushes.Blue, off);
            }

            else
            {
                a = new dot();
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

            spinePos.Content = "righthandPosition (M): \nX: " + sd.Joints[JointID.HandRight].Position.X + "\nY: " + sd.Joints[JointID.HandRight].Position.Y +
                "\nZ: " + sd.Joints[JointID.HandRight].Position.Z;
            
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
