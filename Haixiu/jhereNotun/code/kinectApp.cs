
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

            globalVars.kinectOn = false;

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
                System.Windows.MessageBox.Show("Kinect is not running. Please power on the Kinect and connect the USB connector to your computer.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error);

            }

            else if (Runtime.Kinects.Count > 0 )
            {
                globalVars.kinectOn = initKinect(i);

            }

 

        }

        public bool initKinect( Image i)
        {
            kinect = Runtime.Kinects[0];
            if (kinect == null)
                return false;

            try
            {
                kinect.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor);

                //l.Content = "Kinect on";
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

                return true; 

            }
            catch
            {
                System.Windows.MessageBox.Show("Probably there is no power in Kinect. Kindly swtich on the power suppy and connect kinect's USB connector to your computer.", "Kinect problem", MessageBoxButton.OK, MessageBoxImage.Error);
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

                        globalVars.detector.pollFeatures(skeleData);
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
        public dot(int type = 1) 
        {
            if (type == 1)
            {
                this.a = new Ellipse { Height = 15, Width = 15, StrokeThickness = 4, Stroke = Brushes.Red };
                this.offset = new Point(0, 0);
            }
            else if (type == 2)
            {
                this.a = new Ellipse { Height = 6, Width = 6, StrokeThickness = 3, Stroke = Brushes.Blue};
                this.offset = new Point(0, 0);
            }

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
        public void refreshDot(Canvas c, Point p)
        {
            Canvas.SetLeft(this.a, p.X + offset.X);
            Canvas.SetTop(this.a, p.Y + offset.Y);
            //c.Children.Add(this.a);

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
