
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
using System.IO;

using System.Threading;

//using System.Windows.Forms;

namespace WpfApplication1
{
    public enum userContext
    {
        Standing,
        Sitting,
        Lying,
        Confused,
        kinectoff

    };
    public class kinectApp //: MainWindow
    {
        fileWriter file;
        startFeatures features;
        

        private Runtime kinect;
        private Canvas canvas, topCanvas;
        Label spinePos;
        featureExtractor fExtractor;

        //wpf controls to change
        public Label poseContextLabel1, poseContextLabel2, kinectstateLabel;
        public Image poseContextImage, kinectBulb;
        public Button refreshKinect;

        //to let know mainwindow class when to init
        public bool isKinectRunning;

        private int contextCounter;
        private userContext poseOfUser;

        public kinectApp(Canvas c, Canvas top, Label l,  featureExtractor featExtractor)
        {
            //update the feature

            globalVars.kinectOn = false;
            contextCounter = 0;

            this.kinect = null;
            this.fExtractor = featExtractor;            
            this.features  = new startFeatures();
            
            globalVars.gFeature = features;
            //globalVars.mFeature = 


            this.file = new fileWriter(null);
            this.canvas = c;
            this.topCanvas = top;
            this.spinePos = l;


            //Runtime.Kinects.
            if (Runtime.Kinects.Count == 0)
            {
                isKinectRunning = false;
            }

            else if (Runtime.Kinects.Count > 0 )
            {
                isKinectRunning = true;
            }

 

        }

        public bool initKinect()
        {
            kinect = Runtime.Kinects[0];
            if (kinect == null)
                return false;
            refreshKinect.Content = "Please wait a little...";
            refreshKinect.IsEnabled = false;

            try
            {

                kinect.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);

                //l.Content = "Kinect on";

                updateKinectStatusOn();

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

                return true; 
                
            }
            catch
            {
                System.Windows.MessageBox.Show("Probably there is no power in Kinect. Kindly swtich on the power suppy and connect kinect's USB connector to your computer.", "Kinect problem", MessageBoxButton.OK, MessageBoxImage.Error);
                updateKinectStatusOff();
                return false;
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

                    //check context every 30 frames = 1 sec
                    contextCounter++;
                    if (contextCounter % 30 == 0)
                    {
                        poseOfUser = getSittingOrStanding(skeleData);
                        contextCounter = 0;
                        updateUserContextVisualization(poseOfUser);
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
                    {
                        ////// strictly experimental
                        //globalVars.detector1.detect(skeleData);

                        globalVars.detector.pollFeatures(skeleData, poseOfUser);
                    }
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
        private void updateKinectStatusOn()
        {
            kinectstateLabel.Content = "On";
            kinectstateLabel.Foreground = Brushes.LimeGreen;
            refreshKinect.IsEnabled = false;
            refreshKinect.Content = "All system's go! Start detection";
           
            updateUserContextVisualization(userContext.Confused);

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Haixiu;component/images/green.png", UriKind.Relative);
            bi3.EndInit();
            kinectBulb.Source = bi3;


        }
        private void updateKinectStatusOff()
        {
            kinectstateLabel.Content = "Off";
            kinectstateLabel.Foreground = Brushes.Red;
            refreshKinect.IsEnabled = true;
            refreshKinect.Content = "Connect, Power up and press here";

            updateUserContextVisualization(userContext.kinectoff);

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("/Haixiu;component/images/red.png", UriKind.Relative);
            bi3.EndInit();
            kinectBulb.Source = bi3;

        }

        private void updateUserContextVisualization(userContext pose)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();

            if (pose == userContext.Standing)
            {
                bi3.UriSource = new Uri("/Haixiu;component/images/standingXKCD.jpg", UriKind.Relative); 
                poseContextLabel1.Content = "I see you're";
                poseContextLabel2.Content = "Standing";
 
            }
            else if (pose == userContext.Sitting)
            {
                bi3.UriSource = new Uri("/Haixiu;component/images/sittingXKCD.png", UriKind.Relative);
                poseContextLabel1.Content = "I see you're";
                poseContextLabel2.Content = "Sitting";
 
            }
            else if (pose == userContext.kinectoff)
            {
                bi3.UriSource = new Uri("/Haixiu;component/images/zoozoo.jpg", UriKind.Relative);
                poseContextLabel1.Content = "Can't see you,";
                poseContextLabel2.Content = "   kinect is off.";
            }

            else if (pose == userContext.Confused)
            {
                bi3.UriSource = new Uri("/Haixiu;component/images/moveToKinect.jpg", UriKind.Relative);
                poseContextLabel1.Content = "Please face to";
                poseContextLabel2.Content = "    kinect and move";
            }
            else if (pose == userContext.Lying)
            {
                bi3.UriSource = new Uri("/Haixiu;component/images/zoozoo.jpg", UriKind.Relative);
                poseContextLabel1.Content = "I see you're";
                poseContextLabel2.Content = "Sleeping! Whoa";
            }
            bi3.EndInit();
            poseContextImage.Source = bi3;
        }

        /// <summary>
        /// gets the position of the tracked body whether it is stnding, sitting or sleeping.
        /// returns 1 while standing, returns 2 while sitting and returens 3 while sleeping. returns 0 otherwise.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// 
        public userContext getSittingOrStanding(SkeletonData s)
        {

                double a, b;
                a = Math.Abs(s.Joints[JointID.ShoulderCenter].Position.Y - s.Joints[JointID.KneeLeft].Position.Y);
                b = Math.Abs(s.Joints[JointID.ShoulderCenter].Position.Y - s.Joints[JointID.KneeRight].Position.Y);
                //this.spinePos.Content = "left: "+a+" right: "+b+ "Kinect is okay";

                if (a < 0.2 || b < 0.2)
                    return userContext.Lying;
                else if (a < 0.75 || b < 0.75)
                    return userContext.Sitting;
                else if (a > 0.75 || b > 0.75)
                    return userContext.Standing;
                else
                    return userContext.Confused;
            
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
        public Ellipse ellipse;
        Point offset;
        public dot(int type = 1, int name = 0) 
        {
            if (type == 1)
            {
                this.ellipse = new Ellipse { Height = 15, Width = 15, StrokeThickness = 4, Stroke = Brushes.Red };
                this.offset = new Point(0, 0);
            }
            else if (type == 2)
            {
                this.ellipse = new Ellipse { Height = 6, Width = 6, StrokeThickness = 3, Stroke = Brushes.Blue};
                this.offset = new Point(0, 0);
            }
            if (name != 0)
                ellipse.Name = "dot" + name;
    
        }
        public dot(int h, int w, int thick, Brush b, Point offset)
        {
            this.ellipse = new Ellipse { Height = h, Width = w, StrokeThickness = thick, Stroke = b };
            this.offset = offset;
        }

        public void moveDot(Canvas c, Point p)
        {
            Canvas.SetLeft(this.ellipse, p.X + offset.X);
            Canvas.SetTop(this.ellipse, p.Y + offset.Y);
            c.Children.Add(this.ellipse);

        }
        public void refreshDot(Canvas c, Point p)
        {
            Canvas.SetLeft(this.ellipse, p.X + offset.X);
            Canvas.SetTop(this.ellipse, p.Y + offset.Y);
            //c.Children.Add(this.ellipse);

        }
    }

    public class bone
    {
        Line line;
        Point offset;
        public bone(int x1, int y1, int x2, int y2)
        {
            this.line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
        }
        public void moveBone(Canvas c, Point p)
        {
            Canvas.SetLeft(this.line, p.X + offset.X);
            Canvas.SetTop(this.line, p.Y + offset.Y);
            c.Children.Add(this.line);

        }
        public void drawBone(Canvas c)
        {

            c.Children.Add(this.line);
        }
    }

}
