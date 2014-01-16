﻿using System;
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
//using Microsoft.Research.Research.Kinect.Nui;
using System.Windows.Controls.DataVisualization.Charting;

using System.Threading;

namespace WpfApplication1
{
    public class globalVars
    {

        public static Button circumplexMaximizeButton;
        public static Window mainwin;

        public static JointID[] jid = new JointID[20]{JointID.AnkleLeft, JointID.AnkleRight, JointID.ElbowLeft, JointID.ElbowRight, JointID.FootLeft, JointID.FootRight, JointID.HandLeft, JointID.HandRight, 
                                JointID.Head, JointID.HipCenter, JointID.HipLeft, JointID.HipRight, JointID.KneeLeft, JointID.KneeRight, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ShoulderRight,
                                JointID.Spine, JointID.WristLeft, JointID.WristRight};
        
        public static bool logFeatures, chartRighthand;
        public static startFeatures gFeature;
        //public static movementFeature mFeature;
        public static bool kinectOn=false, logSkele, detectorOn, reducedRecord=true;

        public static Chart resultChart;
        public static LineSeries aseries, vseries;
        public static resultViz chart, chart2;

        public static featureExtractor fExtract;


        public static Canvas Circumplex, CircumplexBig;
        public static bool isCircmplexBigOpen = false;
        public static code.circumplexWindow bigCircumpexWindow;


        public static Label a1/*, a2*/, error, annIter, /*jerkLabel,*/ ValenceOutput, ArousalOutput;
        public static Button saveANNbutn;

        public static Thread ANNthread;
        public static bool needToStopLearning;
        public static int typeOfLearning, outputCount, hiddenCount;

        public static newDynamicDetection detector;
        public static dynamicDetection detector1;

        /// <summary>
        /// //special purpose
        /// </summary>
        public static double idk;

        public static AVQueue avq; 

        public static double screenH, screenW;
    }


    public class AVQueue
    {
        /// <summary>Used as a lock target to ensure thread safety.</summary>
        //private readonly Locker _Locker = new Locker();

        private readonly System.Collections.Generic.Queue<string> _Queue = new System.Collections.Generic.Queue<string>();

        /// <summary></summary>
        public void Enqueue(string item)
        {
            lock (this)
            {
                _Queue.Enqueue(item);
            }
        }
        public string Dequeue()
        {
            string data;

            lock (this)
            {
                if (_Queue.Count > 0)
                {
                    data = _Queue.Dequeue();
                    return data;
                }
                else
                    return null;
            }
        }
    }
}