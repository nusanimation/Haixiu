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
//using Microsoft.Research.Research.Kinect.Nui;
using System.Windows.Controls.DataVisualization.Charting;

using System.Threading;

namespace WpfApplication1
{
    public class globalVars
    {
        public static JointID[] jid = new JointID[20]{JointID.AnkleLeft, JointID.AnkleRight, JointID.ElbowLeft, JointID.ElbowRight, JointID.FootLeft, JointID.FootRight, JointID.HandLeft, JointID.HandRight, 
                                JointID.Head, JointID.HipCenter, JointID.HipLeft, JointID.HipRight, JointID.KneeLeft, JointID.KneeRight, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ShoulderRight,
                                JointID.Spine, JointID.WristLeft, JointID.WristRight};
        public static bool logFeatures;
        public static startFeatures gFeature;
        public static bool kinectOn, logSkele, detectorOn, reducedRecord=true;

        public static Chart resultChart;
        public static LineSeries lseries;
        public static resultViz chart;


        public static Label a1/*, a2*/, error, annIter, /*jerkLabel,*/ AnnOutput;
        public static Button saveANNbutn;

        public static Thread ANNthread;
        public static bool needToStopLearning;
        public static int typeOfLearning;

        public static dynamicDetection detector;
    }
}