
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
//using System.Windows.Controls.DataVisualization.Charting;

using System.Threading;


namespace WpfApplication1{
    

    public struct _qbit
    {
        public float x, y, z;
    }
    public struct _quanta
    {
        public _qbit minx, maxx;
        public _qbit miny, maxy;
        public _qbit minz, maxz;
    };
    public struct _feature
    {
        public _quanta head, rShoulder, lShoulder;
        public _quanta spine;
        public _quanta lElbow, lWrist, lHand, rElbow, rWrist, rHand;
        public _quanta lKnee, rKnee, lAnkle, rAnkle;
        
        public double speedMps, lHandSpeedMps, lElbowSpeedMps, rHandSpeedMps, rElbowSpeedMps;
        public double lKneeSpeedMps, rKneeSpeedMps, rAnkleSpeedMps, lAnkleSpeedMps;

        public double[] realAvgAccel;
        public double[] peakAccel, avgAccel, peakDec, jerkIndex, roundedness;
        // indexes are [0,1,2,3] =lHand,lWrist,rHand,rWrist
        public double[] peakAccelLeg, avgAccelLeg, peakDecLeg, roundednessLeg;
        //refer to initFeatures

        //dummyFeatures
        public String acc, dis, sp;


        public _feature(double neg)
        {
            speedMps=0; lHandSpeedMps=0; lElbowSpeedMps=0; rHandSpeedMps=0; rElbowSpeedMps=0;
            lKneeSpeedMps=0; rKneeSpeedMps=0; rAnkleSpeedMps=0; lAnkleSpeedMps=0;
            acc = null; dis = null; sp = null;

            this.realAvgAccel = new double[4] { 0, 0, 0, 0 };
            
            this.jerkIndex   = new double[4] {0,0,0,0};
            this.roundedness   = new double[4] {0,0,0,0};
            this.avgAccel   = new double[4] {0,0,0,0};
            this.peakDec   = new double[4] {0,0,0,0};
            this.peakAccel   = new double[4] {0,0,0,0};

            this.roundednessLeg   = new double[4] {0,0,0,0};
            this.avgAccelLeg   = new double[4] {0,0,0,0};
            this.peakDecLeg   = new double[4] {0,0,0,0};
            this.peakAccelLeg   = new double[4] {0,0,0,0};
            
/*          this.peakAccel[0] = this.peakDec[0] = 0;
            this.peakAccel[1] = this.peakDec[1] = 0;
            this.peakAccel[2] = this.peakDec[2] = 0;
            this.peakAccel[3] = this.peakDec[3] = 0;
*/
   
            this.head.minx.x = 10;
            this.head.minx.y = 10;
            this.head.minx.z = 10;
            this.head.miny.x = 10;
            this.head.miny.y = 10;
            this.head.miny.z = 10;
            this.head.minz.x = 10;
            this.head.minz.y = 10;
            this.head.minz.z = 10;

            this.head.maxx.x = -10;
            this.head.maxx.y = -10;
            this.head.maxx.z = -10;
            this.head.maxy.x = -10;
            this.head.maxy.y = -10;
            this.head.maxy.z = -10;
            this.head.maxz.x = -10;
            this.head.maxz.y = -10;
            this.head.maxz.z = -10;


            this.lShoulder.minx.x = 10;
            this.lShoulder.minx.y = 10;
            this.lShoulder.minx.z = 10;
            this.lShoulder.miny.x = 10;
            this.lShoulder.miny.y = 10;
            this.lShoulder.miny.z = 10;
            this.lShoulder.minz.x = 10;
            this.lShoulder.minz.y = 10;
            this.lShoulder.minz.z = 10;

            this.lShoulder.maxx.x = -10;
            this.lShoulder.maxx.y = -10;
            this.lShoulder.maxx.z = -10;
            this.lShoulder.maxy.x = -10;
            this.lShoulder.maxy.y = -10;
            this.lShoulder.maxy.z = -10;
            this.lShoulder.maxz.x = -10;
            this.lShoulder.maxz.y = -10;
            this.lShoulder.maxz.z = -10;

            
            this.rShoulder.maxx.x = -10;
            this.rShoulder.maxx.y = -10;
            this.rShoulder.maxx.z = -10;
            this.rShoulder.maxy.x = -10;
            this.rShoulder.maxy.y = -10;
            this.rShoulder.maxy.z = -10;
            this.rShoulder.maxz.x = -10;
            this.rShoulder.maxz.y = -10;
            this.rShoulder.maxz.z = -10;

            this.rShoulder.minx.x = 10;
            this.rShoulder.minx.y = 10;
            this.rShoulder.minx.z = 10;
            this.rShoulder.miny.x = 10;
            this.rShoulder.miny.y = 10;
            this.rShoulder.miny.z = 10;
            this.rShoulder.minz.x = 10;
            this.rShoulder.minz.y = 10;
            this.rShoulder.minz.z = 10;


            this.spine.maxx.x = -10;
            this.spine.maxx.y = -10;
            this.spine.maxx.z = -10;
            this.spine.maxy.x = -10;
            this.spine.maxy.y = -10;
            this.spine.maxy.z = -10;
            this.spine.maxz.x = -10;
            this.spine.maxz.y = -10;
            this.spine.maxz.z = -10;

            this.spine.minx.x = 10;
            this.spine.minx.y = 10;
            this.spine.minx.z = 10;
            this.spine.miny.x = 10;
            this.spine.miny.y = 10;
            this.spine.miny.z = 10;
            this.spine.minz.x = 10;
            this.spine.minz.y = 10;
            this.spine.minz.z = 10;

            
            //Lwrist
            this.lWrist.minx.x = 10;
            this.lWrist.minx.y = 10;
            this.lWrist.minx.z = 10;
            this.lWrist.miny.x = 10;
            this.lWrist.miny.y = 10;
            this.lWrist.miny.z = 10;
            this.lWrist.minz.x = 10;
            this.lWrist.minz.y = 10;
            this.lWrist.minz.z = 10;

            this.lWrist.maxx.x = -10;
            this.lWrist.maxx.y = -10;
            this.lWrist.maxx.z = -10;
            this.lWrist.maxy.x = -10;
            this.lWrist.maxy.y = -10;
            this.lWrist.maxy.z = -10;
            this.lWrist.maxz.x = -10;
            this.lWrist.maxz.y = -10;
            this.lWrist.maxz.z = -10;

            //Rwrist
            this.rWrist.minx.x = 10;
            this.rWrist.minx.y = 10;
            this.rWrist.minx.z = 10;
            this.rWrist.miny.x = 10;
            this.rWrist.miny.y = 10;
            this.rWrist.miny.z = 10;
            this.rWrist.minz.x = 10;
            this.rWrist.minz.y = 10;
            this.rWrist.minz.z = 10;

            this.rWrist.maxx.x = -10;
            this.rWrist.maxx.y = -10;
            this.rWrist.maxx.z = -10;
            this.rWrist.maxy.x = -10;
            this.rWrist.maxy.y = -10;
            this.rWrist.maxy.z = -10;
            this.rWrist.maxz.x = -10;
            this.rWrist.maxz.y = -10;
            this.rWrist.maxz.z = -10;

            //Lwrist
            this.lHand.minx.x = 10;
            this.lHand.minx.y = 10;
            this.lHand.minx.z = 10;
            this.lHand.miny.x = 10;
            this.lHand.miny.y = 10;
            this.lHand.miny.z = 10;
            this.lHand.minz.x = 10;
            this.lHand.minz.y = 10;
            this.lHand.minz.z = 10;

            this.lHand.maxx.x = -10;
            this.lHand.maxx.y = -10;
            this.lHand.maxx.z = -10;
            this.lHand.maxy.x = -10;
            this.lHand.maxy.y = -10;
            this.lHand.maxy.z = -10;
            this.lHand.maxz.x = -10;
            this.lHand.maxz.y = -10;
            this.lHand.maxz.z = -10;

            //Rwrist
            this.rHand.minx.x = 10;
            this.rHand.minx.y = 10;
            this.rHand.minx.z = 10;
            this.rHand.miny.x = 10;
            this.rHand.miny.y = 10;
            this.rHand.miny.z = 10;
            this.rHand.minz.x = 10;
            this.rHand.minz.y = 10;
            this.rHand.minz.z = 10;

            this.rHand.maxx.x = -10;
            this.rHand.maxx.y = -10;
            this.rHand.maxx.z = -10;
            this.rHand.maxy.x = -10;
            this.rHand.maxy.y = -10;
            this.rHand.maxy.z = -10;
            this.rHand.maxz.x = -10;
            this.rHand.maxz.y = -10;
            this.rHand.maxz.z = -10;


            //Lwrist
            this.lElbow.minx.x = 10;
            this.lElbow.minx.y = 10;
            this.lElbow.minx.z = 10;
            this.lElbow.miny.x = 10;
            this.lElbow.miny.y = 10;
            this.lElbow.miny.z = 10;
            this.lElbow.minz.x = 10;
            this.lElbow.minz.y = 10;
            this.lElbow.minz.z = 10;

            this.lElbow.maxx.x = -10;
            this.lElbow.maxx.y = -10;
            this.lElbow.maxx.z = -10;
            this.lElbow.maxy.x = -10;
            this.lElbow.maxy.y = -10;
            this.lElbow.maxy.z = -10;
            this.lElbow.maxz.x = -10;
            this.lElbow.maxz.y = -10;
            this.lElbow.maxz.z = -10;

            //Rwrist
            this.rElbow.minx.x = 10;
            this.rElbow.minx.y = 10;
            this.rElbow.minx.z = 10;
            this.rElbow.miny.x = 10;
            this.rElbow.miny.y = 10;
            this.rElbow.miny.z = 10;
            this.rElbow.minz.x = 10;
            this.rElbow.minz.y = 10;
            this.rElbow.minz.z = 10;

            this.rElbow.maxx.x = -10;
            this.rElbow.maxx.y = -10;
            this.rElbow.maxx.z = -10;
            this.rElbow.maxy.x = -10;
            this.rElbow.maxy.y = -10;
            this.rElbow.maxy.z = -10;
            this.rElbow.maxz.x = -10;
            this.rElbow.maxz.y = -10;
            this.rElbow.maxz.z = -10;

            //Lwrist
            this.lAnkle.minx.x = 10;
            this.lAnkle.minx.y = 10;
            this.lAnkle.minx.z = 10;
            this.lAnkle.miny.x = 10;
            this.lAnkle.miny.y = 10;
            this.lAnkle.miny.z = 10;
            this.lAnkle.minz.x = 10;
            this.lAnkle.minz.y = 10;
            this.lAnkle.minz.z = 10;

            this.lAnkle.maxx.x = -10;
            this.lAnkle.maxx.y = -10;
            this.lAnkle.maxx.z = -10;
            this.lAnkle.maxy.x = -10;
            this.lAnkle.maxy.y = -10;
            this.lAnkle.maxy.z = -10;
            this.lAnkle.maxz.x = -10;
            this.lAnkle.maxz.y = -10;
            this.lAnkle.maxz.z = -10;

            //Rwrist
            this.rAnkle.minx.x = 10;
            this.rAnkle.minx.y = 10;
            this.rAnkle.minx.z = 10;
            this.rAnkle.miny.x = 10;
            this.rAnkle.miny.y = 10;
            this.rAnkle.miny.z = 10;
            this.rAnkle.minz.x = 10;
            this.rAnkle.minz.y = 10;
            this.rAnkle.minz.z = 10;

            this.rAnkle.maxx.x = -10;
            this.rAnkle.maxx.y = -10;
            this.rAnkle.maxx.z = -10;
            this.rAnkle.maxy.x = -10;
            this.rAnkle.maxy.y = -10;
            this.rAnkle.maxy.z = -10;
            this.rAnkle.maxz.x = -10;
            this.rAnkle.maxz.y = -10;
            this.rAnkle.maxz.z = -10;


            //Lwrist
            this.lKnee.minx.x = 10;
            this.lKnee.minx.y = 10;
            this.lKnee.minx.z = 10;
            this.lKnee.miny.x = 10;
            this.lKnee.miny.y = 10;
            this.lKnee.miny.z = 10;
            this.lKnee.minz.x = 10;
            this.lKnee.minz.y = 10;
            this.lKnee.minz.z = 10;

            this.lKnee.maxx.x = -10;
            this.lKnee.maxx.y = -10;
            this.lKnee.maxx.z = -10;
            this.lKnee.maxy.x = -10;
            this.lKnee.maxy.y = -10;
            this.lKnee.maxy.z = -10;
            this.lKnee.maxz.x = -10;
            this.lKnee.maxz.y = -10;
            this.lKnee.maxz.z = -10;

            //Rwrist
            this.rKnee.minx.x = 10;
            this.rKnee.minx.y = 10;
            this.rKnee.minx.z = 10;
            this.rKnee.miny.x = 10;
            this.rKnee.miny.y = 10;
            this.rKnee.miny.z = 10;
            this.rKnee.minz.x = 10;
            this.rKnee.minz.y = 10;
            this.rKnee.minz.z = 10;

            this.rKnee.maxx.x = -10;
            this.rKnee.maxx.y = -10;
            this.rKnee.maxx.z = -10;
            this.rKnee.maxy.x = -10;
            this.rKnee.maxy.y = -10;
            this.rKnee.maxy.z = -10;
            this.rKnee.maxz.x = -10;
            this.rKnee.maxz.y = -10;
            this.rKnee.maxz.z = -10;


            /*          this.head.meanx.x = -10;
                        this.head.meanx.y = -10;
                        this.head.meanx.z = -10;
                        this.head.wmean.x = -10;
                        this.head.wmean.y = -10;
                        this.head.wmean.z = -10;
             */
        }
    
    };

    public static class initFeatures { 
    
        public static void initToZero(this _feature f)
        {

            for (int iii = 0; iii < 4; iii++)
            {
                f.jerkIndex[iii] = 0;
                f.roundedness[iii] = 0;
                f.avgAccel[iii] = 0;
                f.peakDec[iii] = 0;
                f.peakAccel[iii] = 0;

                f.roundednessLeg[iii] = 0;
                f.avgAccelLeg[iii] = 0;
                f.peakDecLeg[iii] = 0;
                f.peakAccelLeg[iii] = 0;
            }
            f.head.minx.x = 10;
            f.head.minx.y = 10;
            f.head.minx.z = 10;
            f.head.miny.x = 10;
            f.head.miny.y = 10;
            f.head.miny.z = 10;
            f.head.minz.x = 10;
            f.head.minz.y = 10;
            f.head.minz.z = 10;

            f.head.maxx.x = -10;
            f.head.maxx.y = -10;
            f.head.maxx.z = -10;
            f.head.maxy.x = -10;
            f.head.maxy.y = -10;
            f.head.maxy.z = -10;
            f.head.maxz.x = -10;
            f.head.maxz.y = -10;
            f.head.maxz.z = -10;


            f.lShoulder.minx.x = 10;
            f.lShoulder.minx.y = 10;
            f.lShoulder.minx.z = 10;
            f.lShoulder.miny.x = 10;
            f.lShoulder.miny.y = 10;
            f.lShoulder.miny.z = 10;
            f.lShoulder.minz.x = 10;
            f.lShoulder.minz.y = 10;
            f.lShoulder.minz.z = 10;

            f.lShoulder.maxx.x = -10;
            f.lShoulder.maxx.y = -10;
            f.lShoulder.maxx.z = -10;
            f.lShoulder.maxy.x = -10;
            f.lShoulder.maxy.y = -10;
            f.lShoulder.maxy.z = -10;
            f.lShoulder.maxz.x = -10;
            f.lShoulder.maxz.y = -10;
            f.lShoulder.maxz.z = -10;


            f.rShoulder.maxx.x = -10;
            f.rShoulder.maxx.y = -10;
            f.rShoulder.maxx.z = -10;
            f.rShoulder.maxy.x = -10;
            f.rShoulder.maxy.y = -10;
            f.rShoulder.maxy.z = -10;
            f.rShoulder.maxz.x = -10;
            f.rShoulder.maxz.y = -10;
            f.rShoulder.maxz.z = -10;

            f.rShoulder.minx.x = 10;
            f.rShoulder.minx.y = 10;
            f.rShoulder.minx.z = 10;
            f.rShoulder.miny.x = 10;
            f.rShoulder.miny.y = 10;
            f.rShoulder.miny.z = 10;
            f.rShoulder.minz.x = 10;
            f.rShoulder.minz.y = 10;
            f.rShoulder.minz.z = 10;


            f.spine.maxx.x = -10;
            f.spine.maxx.y = -10;
            f.spine.maxx.z = -10;
            f.spine.maxy.x = -10;
            f.spine.maxy.y = -10;
            f.spine.maxy.z = -10;
            f.spine.maxz.x = -10;
            f.spine.maxz.y = -10;
            f.spine.maxz.z = -10;

            f.spine.minx.x = 10;
            f.spine.minx.y = 10;
            f.spine.minx.z = 10;
            f.spine.miny.x = 10;
            f.spine.miny.y = 10;
            f.spine.miny.z = 10;
            f.spine.minz.x = 10;
            f.spine.minz.y = 10;
            f.spine.minz.z = 10;


            //Lwrist
            f.lWrist.minx.x = 10;
            f.lWrist.minx.y = 10;
            f.lWrist.minx.z = 10;
            f.lWrist.miny.x = 10;
            f.lWrist.miny.y = 10;
            f.lWrist.miny.z = 10;
            f.lWrist.minz.x = 10;
            f.lWrist.minz.y = 10;
            f.lWrist.minz.z = 10;

            f.lWrist.maxx.x = -10;
            f.lWrist.maxx.y = -10;
            f.lWrist.maxx.z = -10;
            f.lWrist.maxy.x = -10;
            f.lWrist.maxy.y = -10;
            f.lWrist.maxy.z = -10;
            f.lWrist.maxz.x = -10;
            f.lWrist.maxz.y = -10;
            f.lWrist.maxz.z = -10;

            //Rwrist
            f.rWrist.minx.x = 10;
            f.rWrist.minx.y = 10;
            f.rWrist.minx.z = 10;
            f.rWrist.miny.x = 10;
            f.rWrist.miny.y = 10;
            f.rWrist.miny.z = 10;
            f.rWrist.minz.x = 10;
            f.rWrist.minz.y = 10;
            f.rWrist.minz.z = 10;

            f.rWrist.maxx.x = -10;
            f.rWrist.maxx.y = -10;
            f.rWrist.maxx.z = -10;
            f.rWrist.maxy.x = -10;
            f.rWrist.maxy.y = -10;
            f.rWrist.maxy.z = -10;
            f.rWrist.maxz.x = -10;
            f.rWrist.maxz.y = -10;
            f.rWrist.maxz.z = -10;

            //Lwrist
            f.lHand.minx.x = 10;
            f.lHand.minx.y = 10;
            f.lHand.minx.z = 10;
            f.lHand.miny.x = 10;
            f.lHand.miny.y = 10;
            f.lHand.miny.z = 10;
            f.lHand.minz.x = 10;
            f.lHand.minz.y = 10;
            f.lHand.minz.z = 10;

            f.lHand.maxx.x = -10;
            f.lHand.maxx.y = -10;
            f.lHand.maxx.z = -10;
            f.lHand.maxy.x = -10;
            f.lHand.maxy.y = -10;
            f.lHand.maxy.z = -10;
            f.lHand.maxz.x = -10;
            f.lHand.maxz.y = -10;
            f.lHand.maxz.z = -10;

            //Rwrist
            f.rHand.minx.x = 10;
            f.rHand.minx.y = 10;
            f.rHand.minx.z = 10;
            f.rHand.miny.x = 10;
            f.rHand.miny.y = 10;
            f.rHand.miny.z = 10;
            f.rHand.minz.x = 10;
            f.rHand.minz.y = 10;
            f.rHand.minz.z = 10;

            f.rHand.maxx.x = -10;
            f.rHand.maxx.y = -10;
            f.rHand.maxx.z = -10;
            f.rHand.maxy.x = -10;
            f.rHand.maxy.y = -10;
            f.rHand.maxy.z = -10;
            f.rHand.maxz.x = -10;
            f.rHand.maxz.y = -10;
            f.rHand.maxz.z = -10;


            //Lwrist
            f.lElbow.minx.x = 10;
            f.lElbow.minx.y = 10;
            f.lElbow.minx.z = 10;
            f.lElbow.miny.x = 10;
            f.lElbow.miny.y = 10;
            f.lElbow.miny.z = 10;
            f.lElbow.minz.x = 10;
            f.lElbow.minz.y = 10;
            f.lElbow.minz.z = 10;

            f.lElbow.maxx.x = -10;
            f.lElbow.maxx.y = -10;
            f.lElbow.maxx.z = -10;
            f.lElbow.maxy.x = -10;
            f.lElbow.maxy.y = -10;
            f.lElbow.maxy.z = -10;
            f.lElbow.maxz.x = -10;
            f.lElbow.maxz.y = -10;
            f.lElbow.maxz.z = -10;

            //Rwrist
            f.rElbow.minx.x = 10;
            f.rElbow.minx.y = 10;
            f.rElbow.minx.z = 10;
            f.rElbow.miny.x = 10;
            f.rElbow.miny.y = 10;
            f.rElbow.miny.z = 10;
            f.rElbow.minz.x = 10;
            f.rElbow.minz.y = 10;
            f.rElbow.minz.z = 10;

            f.rElbow.maxx.x = -10;
            f.rElbow.maxx.y = -10;
            f.rElbow.maxx.z = -10;
            f.rElbow.maxy.x = -10;
            f.rElbow.maxy.y = -10;
            f.rElbow.maxy.z = -10;
            f.rElbow.maxz.x = -10;
            f.rElbow.maxz.y = -10;
            f.rElbow.maxz.z = -10;

            //Lwrist
            f.lAnkle.minx.x = 10;
            f.lAnkle.minx.y = 10;
            f.lAnkle.minx.z = 10;
            f.lAnkle.miny.x = 10;
            f.lAnkle.miny.y = 10;
            f.lAnkle.miny.z = 10;
            f.lAnkle.minz.x = 10;
            f.lAnkle.minz.y = 10;
            f.lAnkle.minz.z = 10;

            f.lAnkle.maxx.x = -10;
            f.lAnkle.maxx.y = -10;
            f.lAnkle.maxx.z = -10;
            f.lAnkle.maxy.x = -10;
            f.lAnkle.maxy.y = -10;
            f.lAnkle.maxy.z = -10;
            f.lAnkle.maxz.x = -10;
            f.lAnkle.maxz.y = -10;
            f.lAnkle.maxz.z = -10;

            //Rwrist
            f.rAnkle.minx.x = 10;
            f.rAnkle.minx.y = 10;
            f.rAnkle.minx.z = 10;
            f.rAnkle.miny.x = 10;
            f.rAnkle.miny.y = 10;
            f.rAnkle.miny.z = 10;
            f.rAnkle.minz.x = 10;
            f.rAnkle.minz.y = 10;
            f.rAnkle.minz.z = 10;

            f.rAnkle.maxx.x = -10;
            f.rAnkle.maxx.y = -10;
            f.rAnkle.maxx.z = -10;
            f.rAnkle.maxy.x = -10;
            f.rAnkle.maxy.y = -10;
            f.rAnkle.maxy.z = -10;
            f.rAnkle.maxz.x = -10;
            f.rAnkle.maxz.y = -10;
            f.rAnkle.maxz.z = -10;


            //Lwrist
            f.lKnee.minx.x = 10;
            f.lKnee.minx.y = 10;
            f.lKnee.minx.z = 10;
            f.lKnee.miny.x = 10;
            f.lKnee.miny.y = 10;
            f.lKnee.miny.z = 10;
            f.lKnee.minz.x = 10;
            f.lKnee.minz.y = 10;
            f.lKnee.minz.z = 10;

            f.lKnee.maxx.x = -10;
            f.lKnee.maxx.y = -10;
            f.lKnee.maxx.z = -10;
            f.lKnee.maxy.x = -10;
            f.lKnee.maxy.y = -10;
            f.lKnee.maxy.z = -10;
            f.lKnee.maxz.x = -10;
            f.lKnee.maxz.y = -10;
            f.lKnee.maxz.z = -10;

            //Rwrist
            f.rKnee.minx.x = 10;
            f.rKnee.minx.y = 10;
            f.rKnee.minx.z = 10;
            f.rKnee.miny.x = 10;
            f.rKnee.miny.y = 10;
            f.rKnee.miny.z = 10;
            f.rKnee.minz.x = 10;
            f.rKnee.minz.y = 10;
            f.rKnee.minz.z = 10;

            f.rKnee.maxx.x = -10;
            f.rKnee.maxx.y = -10;
            f.rKnee.maxx.z = -10;
            f.rKnee.maxy.x = -10;
            f.rKnee.maxy.y = -10;
            f.rKnee.maxy.z = -10;
            f.rKnee.maxz.x = -10;
            f.rKnee.maxz.y = -10;
            f.rKnee.maxz.z = -10;


            /*          f.head.meanx.x = -10;
                        f.head.meanx.y = -10;
                        f.head.meanx.z = -10;
                        f.head.wmean.x = -10;
                        f.head.wmean.y = -10;
                        f.head.wmean.z = -10;
             */
        }
}

    public class startFeatures //: globalVars
    {
        protected int frame;

        protected _feature feature;
        protected double[] featureSet;
        
        protected SkeletonData sdata;
        protected double[] wDist, wDistLeg;
        protected double spineDist;
        protected _qbit[] wprev, wprevLeg;
        protected _qbit spinePrev;
        protected double[] prevAccel, prevSpeed, prevAccelLeg, prevSpeedLeg, totJI;

        protected float baseX, baseY, baseZ;
        protected float spineX, spineY, spineZ;

        public startFeatures(){

            this.wDist = new double[4];
            this.wDistLeg = new double[4];
            this.prevAccel = new double[4] ;
            this.prevAccelLeg = new double[4];

            this.prevSpeed = new double[4] ;
            this.prevSpeedLeg = new double[4] ;
            this.totJI = new double[4];

            this.wprevLeg = new _qbit[4];

            this.wprev = new _qbit[4];


            refreshVars();

            //passing a double value to the constructor is very important otherwise the default structconstructor is called and thigs don't get initialised. problem: objectreference error in feature arrays.
            feature = new _feature(0.0);
            //initFeatures is for the actual features while the prev inits are local vars.
            //initFeatures();

        }

        protected void refreshVars() {

            this.frame = 0;
            this.spineDist = 0;
            this.spineX=0;
            this.spineY = 0;
            this.spineZ=0;

            for (int i = 0; i < 4; i++)
            {
                this.wDist[i] = 0;
                this.wDistLeg[i] = 0;
                this.prevAccel[i] = 0;
                this.prevAccelLeg[i] = 0;

                this.prevSpeed[i] = 0;
                this.prevSpeedLeg[i] = 0;
                this.totJI[i] = 0;
            }

            this.wprev[0].x = -100;
            this.wprev[0].y = -100;
            this.wprev[0].z = -100;
            this.wprev[1].x = -100;
            this.wprev[1].y = -100;
            this.wprev[1].z = -100;
            this.wprev[2].x = -100;
            this.wprev[2].y = -100;
            this.wprev[2].z = -100;
            this.wprev[3].x = -100;
            this.wprev[3].y = -100;
            this.wprev[3].z = -100;


            this.wprevLeg[0].x = -100;
            this.wprevLeg[0].y = -100;
            this.wprevLeg[0].z = -100;
            this.wprevLeg[1].x = -100;
            this.wprevLeg[1].y = -100;
            this.wprevLeg[1].z = -100;
            this.wprevLeg[2].x = -100;
            this.wprevLeg[2].y = -100;
            this.wprevLeg[2].z = -100;
            this.wprevLeg[3].x = -100;
            this.wprevLeg[3].y = -100;
            this.wprevLeg[3].z = -100;

            this.spinePrev.x = -100;
            this.spinePrev.y = -100;
            this.spinePrev.z = -100;

            this.baseX = 0;
            this.baseY = 0;
            this.baseZ = 0;
         
        }

        
        private void initFeatures(){

            this.feature.jerkIndex = new double[4];
            this.feature.roundedness = new double[4];
            this.feature.avgAccel = new double[4];
            this.feature.peakDec = new double[4];
            this.feature.peakAccel = new double[4];

            this.feature.roundednessLeg = new double[4];
            this.feature.avgAccelLeg = new double[4];
            this.feature.peakDecLeg = new double[4];
            this.feature.peakAccelLeg = new double[4];

            this.feature.peakAccel[0] = this.feature.peakDec[0] = 0;

            this.feature.head.minx.x = 10; 
            this.feature.head.minx.y = 10; 
            this.feature.head.minx.z = 10;
            this.feature.head.miny.x = 10;
            this.feature.head.miny.y = 10;
            this.feature.head.miny.z = 10;
            this.feature.head.minz.x = 10;
            this.feature.head.minz.y = 10;
            this.feature.head.minz.z = 10;
            
            this.feature.head.maxx.x = -10;
            this.feature.head.maxx.y = -10;
            this.feature.head.maxx.z = -10;
            this.feature.head.maxy.x = -10;
            this.feature.head.maxy.y = -10;
            this.feature.head.maxy.z = -10;
            this.feature.head.maxz.x = -10;
            this.feature.head.maxz.y = -10;
            this.feature.head.maxz.z = -10;


            this.feature.lShoulder.minx.x = 10;
            this.feature.lShoulder.minx.y = 10;
            this.feature.lShoulder.minx.z = 10;
            this.feature.lShoulder.miny.x = 10;
            this.feature.lShoulder.miny.y = 10;
            this.feature.lShoulder.miny.z = 10;
            this.feature.lShoulder.minz.x = 10;
            this.feature.lShoulder.minz.y = 10;
            this.feature.lShoulder.minz.z = 10;

            this.feature.rShoulder.maxx.x = -10;
            this.feature.rShoulder.maxx.y = -10;
            this.feature.rShoulder.maxx.z = -10;
            this.feature.rShoulder.maxy.x = -10;
            this.feature.rShoulder.maxy.y = -10;
            this.feature.rShoulder.maxy.z = -10;
            this.feature.rShoulder.maxz.x = -10;
            this.feature.rShoulder.maxz.y = -10;
            this.feature.rShoulder.maxz.z = -10;

            this.feature.spine.maxz.x = -10;
            this.feature.spine.maxz.y = -10;
            this.feature.spine.maxz.z = -10;
            
            this.feature.spine.minz.x = 10;
            this.feature.spine.minz.y = 10;
            this.feature.spine.minz.z = 10;

            //Lwrist
            this.feature.lWrist.minx.x = 10;
            this.feature.lWrist.minx.y = 10;
            this.feature.lWrist.minx.z = 10;
            this.feature.lWrist.miny.x = 10;
            this.feature.lWrist.miny.y = 10;
            this.feature.lWrist.miny.z = 10;
            this.feature.lWrist.minz.x = 10;
            this.feature.lWrist.minz.y = 10;
            this.feature.lWrist.minz.z = 10;

            this.feature.lWrist.maxx.x = -10;
            this.feature.lWrist.maxx.y = -10;
            this.feature.lWrist.maxx.z = -10;
            this.feature.lWrist.maxy.x = -10;
            this.feature.lWrist.maxy.y = -10;
            this.feature.lWrist.maxy.z = -10;
            this.feature.lWrist.maxz.x = -10;
            this.feature.lWrist.maxz.y = -10;
            this.feature.lWrist.maxz.z = -10;

            //Rwrist
            this.feature.rWrist.minx.x = 10;
            this.feature.rWrist.minx.y = 10;
            this.feature.rWrist.minx.z = 10;
            this.feature.rWrist.miny.x = 10;
            this.feature.rWrist.miny.y = 10;
            this.feature.rWrist.miny.z = 10;
            this.feature.rWrist.minz.x = 10;
            this.feature.rWrist.minz.y = 10;
            this.feature.rWrist.minz.z = 10;

            this.feature.rWrist.maxx.x = -10;
            this.feature.rWrist.maxx.y = -10;
            this.feature.rWrist.maxx.z = -10;
            this.feature.rWrist.maxy.x = -10;
            this.feature.rWrist.maxy.y = -10;
            this.feature.rWrist.maxy.z = -10;
            this.feature.rWrist.maxz.x = -10;
            this.feature.rWrist.maxz.y = -10;
            this.feature.rWrist.maxz.z = -10;

            //Lwrist
            this.feature.lHand.minx.x = 10;
            this.feature.lHand.minx.y = 10;
            this.feature.lHand.minx.z = 10;
            this.feature.lHand.miny.x = 10;
            this.feature.lHand.miny.y = 10;
            this.feature.lHand.miny.z = 10;
            this.feature.lHand.minz.x = 10;
            this.feature.lHand.minz.y = 10;
            this.feature.lHand.minz.z = 10;

            this.feature.lHand.maxx.x = -10;
            this.feature.lHand.maxx.y = -10;
            this.feature.lHand.maxx.z = -10;
            this.feature.lHand.maxy.x = -10;
            this.feature.lHand.maxy.y = -10;
            this.feature.lHand.maxy.z = -10;
            this.feature.lHand.maxz.x = -10;
            this.feature.lHand.maxz.y = -10;
            this.feature.lHand.maxz.z = -10;

            //Rwrist
            this.feature.rHand.minx.x = 10;
            this.feature.rHand.minx.y = 10;
            this.feature.rHand.minx.z = 10;
            this.feature.rHand.miny.x = 10;
            this.feature.rHand.miny.y = 10;
            this.feature.rHand.miny.z = 10;
            this.feature.rHand.minz.x = 10;
            this.feature.rHand.minz.y = 10;
            this.feature.rHand.minz.z = 10;

            this.feature.rHand.maxx.x = -10;
            this.feature.rHand.maxx.y = -10;
            this.feature.rHand.maxx.z = -10;
            this.feature.rHand.maxy.x = -10;
            this.feature.rHand.maxy.y = -10;
            this.feature.rHand.maxy.z = -10;
            this.feature.rHand.maxz.x = -10;
            this.feature.rHand.maxz.y = -10;
            this.feature.rHand.maxz.z = -10;


            //Lwrist
            this.feature.lElbow.minx.x = 10;
            this.feature.lElbow.minx.y = 10;
            this.feature.lElbow.minx.z = 10;
            this.feature.lElbow.miny.x = 10;
            this.feature.lElbow.miny.y = 10;
            this.feature.lElbow.miny.z = 10;
            this.feature.lElbow.minz.x = 10;
            this.feature.lElbow.minz.y = 10;
            this.feature.lElbow.minz.z = 10;

            this.feature.lElbow.maxx.x = -10;
            this.feature.lElbow.maxx.y = -10;
            this.feature.lElbow.maxx.z = -10;
            this.feature.lElbow.maxy.x = -10;
            this.feature.lElbow.maxy.y = -10;
            this.feature.lElbow.maxy.z = -10;
            this.feature.lElbow.maxz.x = -10;
            this.feature.lElbow.maxz.y = -10;
            this.feature.lElbow.maxz.z = -10;

            //Rwrist
            this.feature.rElbow.minx.x = 10;
            this.feature.rElbow.minx.y = 10;
            this.feature.rElbow.minx.z = 10;
            this.feature.rElbow.miny.x = 10;
            this.feature.rElbow.miny.y = 10;
            this.feature.rElbow.miny.z = 10;
            this.feature.rElbow.minz.x = 10;
            this.feature.rElbow.minz.y = 10;
            this.feature.rElbow.minz.z = 10;

            this.feature.rElbow.maxx.x = -10;
            this.feature.rElbow.maxx.y = -10;
            this.feature.rElbow.maxx.z = -10;
            this.feature.rElbow.maxy.x = -10;
            this.feature.rElbow.maxy.y = -10;
            this.feature.rElbow.maxy.z = -10;
            this.feature.rElbow.maxz.x = -10;
            this.feature.rElbow.maxz.y = -10;
            this.feature.rElbow.maxz.z = -10;

            //Lwrist
            this.feature.lAnkle.minx.x = 10;
            this.feature.lAnkle.minx.y = 10;
            this.feature.lAnkle.minx.z = 10;
            this.feature.lAnkle.miny.x = 10;
            this.feature.lAnkle.miny.y = 10;
            this.feature.lAnkle.miny.z = 10;
            this.feature.lAnkle.minz.x = 10;
            this.feature.lAnkle.minz.y = 10;
            this.feature.lAnkle.minz.z = 10;

            this.feature.lAnkle.maxx.x = -10;
            this.feature.lAnkle.maxx.y = -10;
            this.feature.lAnkle.maxx.z = -10;
            this.feature.lAnkle.maxy.x = -10;
            this.feature.lAnkle.maxy.y = -10;
            this.feature.lAnkle.maxy.z = -10;
            this.feature.lAnkle.maxz.x = -10;
            this.feature.lAnkle.maxz.y = -10;
            this.feature.lAnkle.maxz.z = -10;

            //Rwrist
            this.feature.rAnkle.minx.x = 10;
            this.feature.rAnkle.minx.y = 10;
            this.feature.rAnkle.minx.z = 10;
            this.feature.rAnkle.miny.x = 10;
            this.feature.rAnkle.miny.y = 10;
            this.feature.rAnkle.miny.z = 10;
            this.feature.rAnkle.minz.x = 10;
            this.feature.rAnkle.minz.y = 10;
            this.feature.rAnkle.minz.z = 10;

            this.feature.rAnkle.maxx.x = -10;
            this.feature.rAnkle.maxx.y = -10;
            this.feature.rAnkle.maxx.z = -10;
            this.feature.rAnkle.maxy.x = -10;
            this.feature.rAnkle.maxy.y = -10;
            this.feature.rAnkle.maxy.z = -10;
            this.feature.rAnkle.maxz.x = -10;
            this.feature.rAnkle.maxz.y = -10;
            this.feature.rAnkle.maxz.z = -10;


            //Lwrist
            this.feature.lKnee.minx.x = 10;
            this.feature.lKnee.minx.y = 10;
            this.feature.lKnee.minx.z = 10;
            this.feature.lKnee.miny.x = 10;
            this.feature.lKnee.miny.y = 10;
            this.feature.lKnee.miny.z = 10;
            this.feature.lKnee.minz.x = 10;
            this.feature.lKnee.minz.y = 10;
            this.feature.lKnee.minz.z = 10;

            this.feature.lKnee.maxx.x = -10;
            this.feature.lKnee.maxx.y = -10;
            this.feature.lKnee.maxx.z = -10;
            this.feature.lKnee.maxy.x = -10;
            this.feature.lKnee.maxy.y = -10;
            this.feature.lKnee.maxy.z = -10;
            this.feature.lKnee.maxz.x = -10;
            this.feature.lKnee.maxz.y = -10;
            this.feature.lKnee.maxz.z = -10;

            //Rwrist
            this.feature.rKnee.minx.x = 10;
            this.feature.rKnee.minx.y = 10;
            this.feature.rKnee.minx.z = 10;
            this.feature.rKnee.miny.x = 10;
            this.feature.rKnee.miny.y = 10;
            this.feature.rKnee.miny.z = 10;
            this.feature.rKnee.minz.x = 10;
            this.feature.rKnee.minz.y = 10;
            this.feature.rKnee.minz.z = 10;

            this.feature.rKnee.maxx.x = -10;
            this.feature.rKnee.maxx.y = -10;
            this.feature.rKnee.maxx.z = -10;
            this.feature.rKnee.maxy.x = -10;
            this.feature.rKnee.maxy.y = -10;
            this.feature.rKnee.maxy.z = -10;
            this.feature.rKnee.maxz.x = -10;
            this.feature.rKnee.maxz.y = -10;
            this.feature.rKnee.maxz.z = -10;


/*          this.feature.head.meanx.x = -10;
            this.feature.head.meanx.y = -10;
            this.feature.head.meanx.z = -10;
            this.feature.head.wmean.x = -10;
            this.feature.head.wmean.y = -10;
            this.feature.head.wmean.z = -10;
 */
        }


        public void addFeatures(SkeletonData s)
        {   
            //creating base to substract from raw sensor posiiton to normalize values
            this.sdata = s;

            if (baseZ == 0)
            {   
                this.baseX = this.spineX = s.Joints[JointID.Spine].Position.X;
                this.baseY = this.spineY = s.Joints[JointID.Spine].Position.Y;
                this.baseZ = this.spineZ = s.Joints[JointID.Spine].Position.Z;
            }

                //here the positions are being normalised wrt spine position
            else{            
                this.baseX = sdata.Joints[JointID.Head].Position.X + spineX;
                this.baseY = sdata.Joints[JointID.Head].Position.Y + spineY;
                this.baseZ = sdata.Joints[JointID.Head].Position.Y + spineZ;
            }
            //foreach (JointID i in jid){
            //        double a = Math.Round(this.sdata.Joints[i].Position.X, 3);
            //        SkeletonPoint pos = new SkeletonPoint()
            //        {
            //            X = Math.Round(this.sdata.Joints[i].Position.X, 3);
            //            Y = Math.Round(this.sdata.Joints[i].Position.Y, 3);
            //            Z = Math.Round(this.sdata.Joints[i].Position.Z, 3);
            //        };
                
            //}


            stableSkele();
            danglingSkele();
            //underBeltSkele();

            ++frame;
            globalVars.a1.Content = frame;

        }
      
        public void saveFeatures(){
            speed();
            dangSpeed();
            dangQuality();
            try
            {
                writeFeatures();
                refreshVars();
                feature.initToZero();
            }
            catch 
            {
                System.Windows.MessageBox.Show("cant record :( sorry dude.","error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        
        private void writeFeatures(){
                fileWriter f = new fileWriter(null, "feature.csv");
                f.writeFeatures(feature);
                f.closeFile();
                if (globalVars.reducedRecord == true)
                {
                    makeFeatureSet();
                    fileWriter f1 = new fileWriter(null, "Shortfeature.csv");
                    f1.WritefeatureSet(this.featureSet);
                    f1.closeFile();
                }
            
        }
        
        protected void danglingSkele() {
            dangPos();
            pollDangDistance();
        }
        
        protected void stableSkele(){
            updateSpine();
            headAndShoulders();
        }
        
        protected void underBeltSkele() {
            legPos();
            legSpeed();
            //latter 
        }
        
        private void legPos() {

            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
            if (temp > feature.lKnee.maxx.x)
            {
                feature.lKnee.maxx.x = temp;
                feature.lKnee.maxx.y = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
                feature.lKnee.maxx.z = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
            if (temp > feature.lKnee.maxy.y)
            {
                feature.lKnee.maxy.x = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
                feature.lKnee.maxy.y = temp;
                feature.lKnee.maxy.z = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            if (temp > feature.lKnee.maxz.z)
            {
                feature.lKnee.maxz.x = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
                feature.lKnee.maxz.y = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
                feature.lKnee.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
            if (temp < feature.lKnee.minx.x)
            {
                feature.lKnee.minx.x = temp;
                feature.lKnee.minx.y = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
                feature.lKnee.minx.z = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
            if (temp < feature.lKnee.miny.y)
            {
                feature.lKnee.miny.x = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
                feature.lKnee.miny.y = temp;
                feature.lKnee.miny.z = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeLeft].Position.Z - this.baseZ;
            if (temp < feature.lKnee.minz.z)
            {
                feature.lKnee.minz.x = sdata.Joints[JointID.KneeLeft].Position.X - this.baseX;
                feature.lKnee.minz.y = sdata.Joints[JointID.KneeLeft].Position.Y - this.baseY;
                feature.lKnee.minz.z = temp;
            }


        //Ankle
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
            if (temp > feature.lAnkle.maxx.x)
            {
                feature.lAnkle.maxx.x = temp;
                feature.lAnkle.maxx.y = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
                feature.lAnkle.maxx.z = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
            if (temp > feature.lAnkle.maxy.y)
            {
                feature.lAnkle.maxy.x = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
                feature.lAnkle.maxy.y = temp;
                feature.lAnkle.maxy.z = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            if (temp > feature.lAnkle.maxz.z)
            {
                feature.lAnkle.maxz.x = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
                feature.lAnkle.maxz.y = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
                feature.lAnkle.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
            if (temp < feature.lAnkle.minx.x)
            {
                feature.lAnkle.minx.x = temp;
                feature.lAnkle.minx.y = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
                feature.lAnkle.minx.z = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
            if (temp < feature.lAnkle.miny.y)
            {
                feature.lAnkle.miny.x = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
                feature.lAnkle.miny.y = temp;
                feature.lAnkle.miny.z = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleLeft].Position.Z - this.baseZ;
            if (temp < feature.lAnkle.minz.z)
            {
                feature.lAnkle.minz.x = sdata.Joints[JointID.AnkleLeft].Position.X - this.baseX;
                feature.lAnkle.minz.y = sdata.Joints[JointID.AnkleLeft].Position.Y - this.baseY;
                feature.lAnkle.minz.z = temp;
            }


            //rightKnee
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
            if (temp > feature.rKnee.maxx.x)
            {
                feature.rKnee.maxx.x = temp;
                feature.rKnee.maxx.y = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
                feature.rKnee.maxx.z = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
            if (temp > feature.rKnee.maxy.y)
            {
                feature.rKnee.maxy.x = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
                feature.rKnee.maxy.y = temp;
                feature.rKnee.maxy.z = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            if (temp > feature.rKnee.maxz.z)
            {
                feature.rKnee.maxz.x = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
                feature.rKnee.maxz.y = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
                feature.rKnee.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
            if (temp < feature.rKnee.minx.x)
            {
                feature.rKnee.minx.x = temp;
                feature.rKnee.minx.y = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
                feature.rKnee.minx.z = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
            if (temp < feature.rKnee.miny.y)
            {
                feature.rKnee.miny.x = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
                feature.rKnee.miny.y = temp;
                feature.rKnee.miny.z = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.KneeRight].Position.Z - this.baseZ;
            if (temp < feature.rKnee.minz.z)
            {
                feature.rKnee.minz.x = sdata.Joints[JointID.KneeRight].Position.X - this.baseX;
                feature.rKnee.minz.y = sdata.Joints[JointID.KneeRight].Position.Y - this.baseY;
                feature.rKnee.minz.z = temp;
            }


            //right Ankle
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
            if (temp > feature.rAnkle.maxx.x)
            {
                feature.rAnkle.maxx.x = temp;
                feature.rAnkle.maxx.y = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
                feature.rAnkle.maxx.z = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
            if (temp > feature.rAnkle.maxy.y)
            {
                feature.rAnkle.maxy.x = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
                feature.rAnkle.maxy.y = temp;
                feature.rAnkle.maxy.z = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            if (temp > feature.rAnkle.maxz.z)
            {
                feature.rAnkle.maxz.x = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
                feature.rAnkle.maxz.y = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
                feature.rAnkle.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
            if (temp < feature.rAnkle.minx.x)
            {
                feature.rAnkle.minx.x = temp;
                feature.rAnkle.minx.y = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
                feature.rAnkle.minx.z = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
            if (temp < feature.rAnkle.miny.y)
            {
                feature.rAnkle.miny.x = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
                feature.rAnkle.miny.y = temp;
                feature.rAnkle.miny.z = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            }
            temp = sdata.Joints[JointID.AnkleRight].Position.Z - this.baseZ;
            if (temp < feature.rAnkle.minz.z)
            {
                feature.rAnkle.minz.x = sdata.Joints[JointID.AnkleRight].Position.X - this.baseX;
                feature.rAnkle.minz.y = sdata.Joints[JointID.AnkleRight].Position.Y - this.baseY;
                feature.rAnkle.minz.z = temp;
            }

        
        }

        private void legSpeed() { 
           
           // double f0=0;
            if (this.wprevLeg[0].x == -100)
            {
                wDistLeg[0] = 0;
                this.wprevLeg[0].x = this.sdata.Joints[JointID.AnkleLeft].Position.X - 0 * 1;
                this.wprevLeg[0].y = this.sdata.Joints[JointID.AnkleLeft].Position.Y - 0 * 2;
                this.wprevLeg[0].z = this.sdata.Joints[JointID.AnkleLeft].Position.Z - 0 * 3;
                wDistLeg[1] = 0;
                this.wprevLeg[1].x = this.sdata.Joints[JointID.AnkleRight].Position.X - 0 * 1;
                this.wprevLeg[1].y = this.sdata.Joints[JointID.AnkleRight].Position.Y - 0 * 2;
                this.wprevLeg[1].z = this.sdata.Joints[JointID.AnkleRight].Position.Z - 0 * 3;
                wDistLeg[2] = 0;
                this.wprevLeg[2].x = this.sdata.Joints[JointID.KneeLeft].Position.X - 0 * 1;
                this.wprevLeg[2].y = this.sdata.Joints[JointID.KneeLeft].Position.Y - 0 * 2;
                this.wprevLeg[2].z = this.sdata.Joints[JointID.KneeLeft].Position.Z - 0 * 3;
                wDistLeg[3] = 0;
                this.wprevLeg[3].x = this.sdata.Joints[JointID.KneeRight].Position.X - 0 * 1;
                this.wprevLeg[3].y = this.sdata.Joints[JointID.KneeRight].Position.Y - 0 * 2;
                this.wprevLeg[3].z = this.sdata.Joints[JointID.KneeRight].Position.Z - 0 * 3;
            }
            else if (this.frame % 6 == 0 && this.wprevLeg[0].x != -100)
            {

                double[] S = new double[4];
                if ((this.wprevLeg[0].x - this.sdata.Joints[JointID.AnkleLeft].Position.X - 0 * 2) >= 0.005 || (this.wprevLeg[0].y - this.sdata.Joints[JointID.AnkleLeft].Position.Y - 0 * 2) >= 0.005 || (this.wprevLeg[0].z - this.sdata.Joints[JointID.AnkleLeft].Position.Z - 0 * 2) >= 0.005)
                {
                    S[0] = Math.Sqrt(Math.Pow((this.wprevLeg[0].x - this.sdata.Joints[JointID.AnkleLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprevLeg[0].y - this.sdata.Joints[JointID.AnkleLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprevLeg[0].z - this.sdata.Joints[JointID.AnkleLeft].Position.Z - 0 * 3), 2));
                    this.wprevLeg[0].x = this.sdata.Joints[JointID.AnkleLeft].Position.X - 0 * 1;
                    this.wprevLeg[0].y = this.sdata.Joints[JointID.AnkleLeft].Position.Y - 0 * 2;
                    this.wprevLeg[0].z = this.sdata.Joints[JointID.AnkleLeft].Position.Z - 0 * 3;
                }

                if ((this.wprevLeg[1].x - this.sdata.Joints[JointID.AnkleRight].Position.X - 0 * 2) >= 0.005 || (this.wprevLeg[1].y - this.sdata.Joints[JointID.AnkleRight].Position.Y - 0 * 2) >= 0.005 || (this.wprevLeg[1].z - this.sdata.Joints[JointID.AnkleRight].Position.Z - 0 * 2) >= 0.005)
                {

                    S[1] = Math.Sqrt(Math.Pow((this.wprevLeg[1].x - this.sdata.Joints[JointID.AnkleRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprevLeg[1].y - this.sdata.Joints[JointID.AnkleRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprevLeg[1].z - this.sdata.Joints[JointID.AnkleRight].Position.Z - 0 * 3), 2));

                    this.wprevLeg[1].x = this.sdata.Joints[JointID.AnkleRight].Position.X - 0 * 1;
                    this.wprevLeg[1].y = this.sdata.Joints[JointID.AnkleRight].Position.Y - 0 * 2;
                    this.wprevLeg[1].z = this.sdata.Joints[JointID.AnkleRight].Position.Z - 0 * 3;
                }


                if ((this.wprevLeg[2].x - this.sdata.Joints[JointID.KneeLeft].Position.X - 0 * 2) >= 0.005 || (this.wprevLeg[2].y - this.sdata.Joints[JointID.KneeLeft].Position.Y - 0 * 2) >= 0.005 || (this.wprevLeg[2].z - this.sdata.Joints[JointID.KneeLeft].Position.Z - 0 * 2) >= 0.005)
                {

                    S[2] = Math.Sqrt(Math.Pow((this.wprevLeg[2].x - this.sdata.Joints[JointID.KneeLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprevLeg[2].y - this.sdata.Joints[JointID.KneeLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprevLeg[2].z - this.sdata.Joints[JointID.KneeLeft].Position.Z - 0 * 3), 2));
                    
                    this.wprevLeg[2].x = this.sdata.Joints[JointID.KneeLeft].Position.X - 0 * 1;
                    this.wprevLeg[2].y = this.sdata.Joints[JointID.KneeLeft].Position.Y - 0 * 2;
                    this.wprevLeg[2].z = this.sdata.Joints[JointID.KneeLeft].Position.Z - 0 * 3;
                }

                if ((this.wprevLeg[3].x - this.sdata.Joints[JointID.KneeRight].Position.X - 0 * 2) >= 0.005 || (this.wprevLeg[3].y - this.sdata.Joints[JointID.KneeRight].Position.Y - 0 * 2) >= 0.005 || (this.wprevLeg[3].z - this.sdata.Joints[JointID.KneeRight].Position.Z - 0 * 2) >= 0.005)
                {

                    S[3] = Math.Sqrt(Math.Pow((this.wprevLeg[3].x - this.sdata.Joints[JointID.KneeRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprevLeg[3].y - this.sdata.Joints[JointID.KneeRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprevLeg[3].z - this.sdata.Joints[JointID.KneeRight].Position.Z - 0 * 3), 2));
                    
                    this.wprevLeg[3].x = this.sdata.Joints[JointID.KneeRight].Position.X - 0 * 1;
                    this.wprevLeg[3].y = this.sdata.Joints[JointID.KneeRight].Position.Y - 0 * 2;
                    this.wprevLeg[3].z = this.sdata.Joints[JointID.KneeRight].Position.Z - 0 * 3;
                }


                wDistLeg[0] += S[0];
                wDistLeg[1] += S[1];
                wDistLeg[2] += S[2];
                wDistLeg[3] += S[3];

                //this.feature.dis += S + ",";

                // a2.Content = wDistLeg[0];//updating label

                if (this.prevSpeedLeg[0] == 0 && this.frame == 6)
                {
                    this.prevSpeedLeg[0] = S[0] / 0.2;
                    this.prevSpeedLeg[1] = S[1] / 0.2;
                    this.prevSpeedLeg[2] = S[2] / 0.2;
                    this.prevSpeedLeg[3] = S[3] / 0.2;
                    this.prevAccelLeg[0] = 0;
                    this.prevAccelLeg[1] = 0;
                    this.prevAccelLeg[2] = 0;
                    this.prevAccelLeg[3] = 0;
                    //this.feature.sp += this.prevSpeedLeg + ",";
                    //this.feature.acc += this.prevAccelLeg + ",";
                }
                else
                {

                    //acceleration and jerk index calculation
                    //     f0 = this.prevAccelLeg[0];
                    this.prevAccelLeg[0] = (2 * S[0] - 2 * this.prevSpeedLeg[0] * 0.2) / 0.04; //s=ut+.5ft^2
                    //     jIndex[0] = (this.prevAccelLeg[0] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    //     totJI[0] += Math.Abs(jIndex[0]);

                    if (this.feature.peakAccelLeg[0] < this.prevAccelLeg[0])
                    {
                        this.feature.peakAccelLeg[0] = this.prevAccelLeg[0];
                    }
                    else if (this.feature.peakDecLeg[0] > this.prevAccelLeg[0])
                    {
                        this.feature.peakDecLeg[0] = this.prevAccelLeg[0];
                    }

                    //       f0 = this.prevAccelLeg[1];
                    this.prevAccelLeg[1] = (2 * S[1] - 2 * this.prevSpeedLeg[1] * 0.2) / 0.04; //s=ut+.5ft^2
                    //       jIndex[1] = (this.prevAccelLeg[1] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    //       totJI[1] += Math.Abs(jIndex[1]);

                    if (this.feature.peakAccelLeg[1] < this.prevAccelLeg[1])
                    {
                        this.feature.peakAccelLeg[1] = this.prevAccelLeg[1];
                    }
                    else if (this.feature.peakDecLeg[1] > this.prevAccelLeg[1])
                    {
                        this.feature.peakDecLeg[1] = this.prevAccelLeg[1];
                    }


                    //       f0 = this.prevAccelLeg[2];
                    this.prevAccelLeg[2] = (2 * S[2] - 2 * this.prevSpeedLeg[2] * 0.2) / 0.04; //s=ut+.5ft^2
                    //       jIndex[2] = (this.prevAccelLeg[2] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    //       totJI[2] += Math.Abs(jIndex[2]);

                    if (this.feature.peakAccelLeg[2] < this.prevAccelLeg[2])
                    {
                        this.feature.peakAccelLeg[2] = this.prevAccelLeg[2];
                    }
                    else if (this.feature.peakDecLeg[2] > this.prevAccelLeg[2])
                    {
                        this.feature.peakDecLeg[2] = this.prevAccelLeg[2];
                    }


                    //       f0 = this.prevAccelLeg[3];
                    this.prevAccelLeg[3] = (2 * S[3] - 2 * this.prevSpeedLeg[3] * 0.2) / 0.04; //s=ut+.5ft^2
                    //       jIndex[3] = (this.prevAccelLeg[3] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    //       totJI[3] += Math.Abs(jIndex[3]);

                    if (this.feature.peakAccelLeg[3] < this.prevAccelLeg[3])
                    {
                        this.feature.peakAccelLeg[3] = this.prevAccelLeg[3];
                    }
                    else if (this.feature.peakDecLeg[3] > this.prevAccelLeg[3])
                    {
                        this.feature.peakDecLeg[3] = this.prevAccelLeg[3];
                    }

                    //this.totAccel += this.prevAccelLeg; //now calculating the whole distance*2 / t^2
                    //this.feature.acc += this.prevAccelLeg + ",";
                    //this.prevSpeedLeg = this.prevSpeedLeg + this.prevAccelLeg * 0.2; //v=u+ft
                    //this.feature.sp += this.prevSpeedLeg + ",";
                    this.prevSpeedLeg[0] = S[0] / 0.2; // the v=u+ft was toohot to handle for negative speed values.
                    this.prevSpeedLeg[1] = S[1] / 0.2;
                    this.prevSpeedLeg[2] = S[2] / 0.2;
                    this.prevSpeedLeg[3] = S[3] / 0.2;

                }




            }//else if
        }


        protected void dangPos() {
            //rElbow
            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
            if (temp > feature.rElbow.maxx.x)
            {
                feature.rElbow.maxx.x = temp;
                feature.rElbow.maxx.y = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
                feature.rElbow.maxx.z = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
            if (temp > feature.rElbow.maxy.y)
            {
                feature.rElbow.maxy.x = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
                feature.rElbow.maxy.y = temp;
                feature.rElbow.maxy.z = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            if (temp > feature.rElbow.maxz.z)
            {
                feature.rElbow.maxz.x = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
                feature.rElbow.maxz.y = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
                feature.rElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
            if (temp < feature.rElbow.minx.x)
            {
                feature.rElbow.minx.x = temp;
                feature.rElbow.minx.y = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
                feature.rElbow.minx.z = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
            if (temp < feature.rElbow.miny.y)
            {
                feature.rElbow.miny.x = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
                feature.rElbow.miny.y = temp;
                feature.rElbow.miny.z = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z  - this.baseZ;
            if (temp < feature.rElbow.minz.z)
            {
                feature.rElbow.minz.x = sdata.Joints[JointID.ElbowRight].Position.X  - this.baseX;
                feature.rElbow.minz.y = sdata.Joints[JointID.ElbowRight].Position.Y  - this.baseY;
                feature.rElbow.minz.z = temp;
            }



            //lElbow
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
            if (temp > feature.lElbow.maxx.x)
            {
                feature.lElbow.maxx.x = temp;
                feature.lElbow.maxx.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
                feature.lElbow.maxx.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
            if (temp > feature.lElbow.maxy.y)
            {
                feature.lElbow.maxy.x = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
                feature.lElbow.maxy.y = temp;
                feature.lElbow.maxy.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            if (temp > feature.lElbow.maxz.z)
            {
                feature.lElbow.maxz.x = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
                feature.lElbow.maxz.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
                feature.lElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
            if (temp < feature.lElbow.minx.x)
            {
                feature.lElbow.minx.x = temp;
                feature.lElbow.minx.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
                feature.lElbow.minx.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
            if (temp < feature.lElbow.miny.y)
            {
                feature.lElbow.miny.x = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
                feature.lElbow.miny.y = temp;
                feature.lElbow.miny.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z  - this.baseZ;
            if (temp < feature.lElbow.minz.z)
            {
                feature.lElbow.minz.x = sdata.Joints[JointID.ElbowLeft].Position.X  - this.baseX;
                feature.lElbow.minz.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - this.baseY;
                feature.lElbow.minz.z = temp;
            }


            //rHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
            if (temp > feature.rHand.maxx.x)
            {
                feature.rHand.maxx.x = temp;
                feature.rHand.maxx.y = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
                feature.rHand.maxx.z = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
            if (temp > feature.rHand.maxy.y)
            {
                feature.rHand.maxy.x = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
                feature.rHand.maxy.y = temp;
                feature.rHand.maxy.z = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            if (temp > feature.rHand.maxz.z)
            {
                feature.rHand.maxz.x = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
                feature.rHand.maxz.y = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
                feature.rHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
            if (temp < feature.rHand.minx.x)
            {
                feature.rHand.minx.x = temp;
                feature.rHand.minx.y = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
                feature.rHand.minx.z = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
            if (temp < feature.rHand.miny.y)
            {
                feature.rHand.miny.x = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
                feature.rHand.miny.y = temp;
                feature.rHand.miny.z = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z  - this.baseZ;
            if (temp < feature.rHand.minz.z)
            {
                feature.rHand.minz.x = sdata.Joints[JointID.HandRight].Position.X  - this.baseX;
                feature.rHand.minz.y = sdata.Joints[JointID.HandRight].Position.Y  - this.baseY;
                feature.rHand.minz.z = temp;
            }



            //lHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
            if (temp > feature.lHand.maxx.x)
            {
                feature.lHand.maxx.x = temp;
                feature.lHand.maxx.y = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
                feature.lHand.maxx.z = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
            if (temp > feature.lHand.maxy.y)
            {
                feature.lHand.maxy.x = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
                feature.lHand.maxy.y = temp;
                feature.lHand.maxy.z = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            if (temp > feature.lHand.maxz.z)
            {
                feature.lHand.maxz.x = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
                feature.lHand.maxz.y = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
                feature.lHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
            if (temp < feature.lHand.minx.x)
            {
                feature.lHand.minx.x = temp;
                feature.lHand.minx.y = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
                feature.lHand.minx.z = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
            if (temp < feature.lHand.miny.y)
            {
                feature.lHand.miny.x = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
                feature.lHand.miny.y = temp;
                feature.lHand.miny.z = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z  - this.baseZ;
            if (temp < feature.lHand.minz.z)
            {
                feature.lHand.minz.x = sdata.Joints[JointID.HandLeft].Position.X  - this.baseX;
                feature.lHand.minz.y = sdata.Joints[JointID.HandLeft].Position.Y  - this.baseY;
                feature.lHand.minz.z = temp;
            }


            //rWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
            if (temp > feature.rWrist.maxx.x)
            {
                feature.rWrist.maxx.x = temp;
                feature.rWrist.maxx.y = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
                feature.rWrist.maxx.z = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
            if (temp > feature.rWrist.maxy.y)
            {
                feature.rWrist.maxy.x = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
                feature.rWrist.maxy.y = temp;
                feature.rWrist.maxy.z = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            if (temp > feature.rWrist.maxz.z)
            {
                feature.rWrist.maxz.x = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
                feature.rWrist.maxz.y = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
                feature.rWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
            if (temp < feature.rWrist.minx.x)
            {
                feature.rWrist.minx.x = temp;
                feature.rWrist.minx.y = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
                feature.rWrist.minx.z = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
            if (temp < feature.rWrist.miny.y)
            {
                feature.rWrist.miny.x = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
                feature.rWrist.miny.y = temp;
                feature.rWrist.miny.z = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z  - this.baseZ;
            if (temp < feature.rWrist.minz.z)
            {
                feature.rWrist.minz.x = sdata.Joints[JointID.WristRight].Position.X  - this.baseX;
                feature.rWrist.minz.y = sdata.Joints[JointID.WristRight].Position.Y  - this.baseY;
                feature.rWrist.minz.z = temp;
            }



            //lWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
            if (temp > feature.lWrist.maxx.x)
            {
                feature.lWrist.maxx.x = temp;
                feature.lWrist.maxx.y = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
                feature.lWrist.maxx.z = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
            if (temp > feature.lWrist.maxy.y)
            {
                feature.lWrist.maxy.x = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
                feature.lWrist.maxy.y = temp;
                feature.lWrist.maxy.z = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            if (temp > feature.lWrist.maxz.z)
            {
                feature.lWrist.maxz.x = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
                feature.lWrist.maxz.y = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
                feature.lWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
            if (temp < feature.lWrist.minx.x)
            {
                feature.lWrist.minx.x = temp;
                feature.lWrist.minx.y = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
                feature.lWrist.minx.z = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
            if (temp < feature.lWrist.miny.y)
            {
                feature.lWrist.miny.x = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
                feature.lWrist.miny.y = temp;
                feature.lWrist.miny.z = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z  - this.baseZ;
            if (temp < feature.lWrist.minz.z)
            {
                feature.lWrist.minz.x = sdata.Joints[JointID.WristLeft].Position.X  - this.baseX;
                feature.lWrist.minz.y = sdata.Joints[JointID.WristLeft].Position.Y  - this.baseY;
                feature.lWrist.minz.z = temp;
            }

        }

        protected double[][] pollDangDistance() {

            double[] jIndex = new double[4] { 0,0,0,0 };
            double[] S = new double[4] { 0, 0, 0, 0 };

            double[][] returnVal = new double[2][];
            returnVal[0] = new double[4] { 0, 0, 0, 0 };
            returnVal[1] = new double[4] { 0, 0, 0, 0 };

            double f0=0;
            if (this.wprev[0].x == -100)
            {
               // System.Windows.MessageBox.Show("ping.", "featureset error",  MessageBoxButton.OK, MessageBoxImage.Error); 
                wDist[0] = 0;
                this.wprev[0].x = this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 1;
                this.wprev[0].y = this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2;
                this.wprev[0].z = this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 3;
                wDist[1] = 0;
                this.wprev[1].x = this.sdata.Joints[JointID.WristRight].Position.X - 0 * 1;
                this.wprev[1].y = this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2;
                this.wprev[1].z = this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 3;
                wDist[2] = 0;
                this.wprev[2].x = this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 1;
                this.wprev[2].y = this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2;
                this.wprev[2].z = this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 3;
                wDist[3] = 0;
                this.wprev[3].x = this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 1;
                this.wprev[3].y = this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2;
                this.wprev[3].z = this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 3;
            }
            else if (this.frame % 6 == 0 && this.wprev[0].x != -100)
            {


                if ((this.wprev[0].x - this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 2) >= 0.005 || (this.wprev[0].y - this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2) >= 0.005 || (this.wprev[0].z - this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 2) >= 0.005)
                {
                    S[0] = Math.Sqrt(Math.Pow((this.wprev[0].x - this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[0].y - this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[0].z - this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 3), 2));

                    this.wprev[0].x = this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 1;
                    this.wprev[0].y = this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2;
                    this.wprev[0].z = this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 3;
                }

                if ((this.wprev[1].x - this.sdata.Joints[JointID.WristRight].Position.X - 0 * 2) >= 0.005 || (this.wprev[1].y - this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2) >= 0.005 || (this.wprev[1].z - this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 2) >= 0.005)
                {

                    S[1] = Math.Sqrt(Math.Pow((this.wprev[1].x - this.sdata.Joints[JointID.WristRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[1].y - this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[1].z - this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 3), 2));
                 
                    this.wprev[1].x = this.sdata.Joints[JointID.WristRight].Position.X - 0 * 1;
                    this.wprev[1].y = this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2;
                    this.wprev[1].z = this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 3;
                }


                if ((this.wprev[2].x - this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 2) >= 0.005 || (this.wprev[2].y - this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2) >= 0.005 || (this.wprev[2].z - this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 2) >= 0.005)
                {

                    S[2] = Math.Sqrt(Math.Pow((this.wprev[2].x - this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[2].y - this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[2].z - this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 3), 2));
                    
                    this.wprev[2].x = this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 1;
                    this.wprev[2].y = this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2;
                    this.wprev[2].z = this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 3;
                }

                if ((this.wprev[3].x - this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 2) >= 0.005 || (this.wprev[3].y - this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2) >= 0.005 || (this.wprev[3].z - this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 2) >= 0.005)
                {

                    S[3] = Math.Sqrt(Math.Pow((this.wprev[3].x - this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[3].y - this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[3].z - this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 3), 2));

                    this.wprev[3].x = this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 1;
                    this.wprev[3].y = this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2;
                    this.wprev[3].z = this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 3;
                }
                
                wDist[0] += S[0];
                wDist[1] += S[1];
                wDist[2] += S[2];
                wDist[3] += S[3];

                //this.feature.dis += S + ",";
                
                //globalVars.a2.Content = wDist[0] + ","+wDist[1] + ","+wDist[2]+ ","+wDist[3];//updating label

                if (this.prevSpeed[0] == 0 && this.frame == 6)
                {
                    this.prevSpeed[0] = S[0] / 0.2;
                    this.prevSpeed[1] = S[1] / 0.2;
                    this.prevSpeed[2] = S[2] / 0.2;
                    this.prevSpeed[3] = S[3] / 0.2;
                    this.prevAccel[0] = 0;
                    this.prevAccel[1] = 0;
                    this.prevAccel[2] = 0;
                    this.prevAccel[3] = 0;
                    //this.feature.sp += this.prevSpeed + ",";
                    //this.feature.acc += this.prevAccel + ",";
                }
                else
                {
                    
                    //acceleration and jerk index calculation
                    f0 = this.prevAccel[0];
                    this.prevAccel[0] = (2 * S[0] - 2 * this.prevSpeed[0] * 0.2) / 0.04; //s=ut+.5ft^2
                    jIndex[0] = (this.prevAccel[0] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    totJI[0] += Math.Abs(jIndex[0]);
                   // System.Windows.MessageBox.Show(", " + this.prevAccel[0],                "test", MessageBoxButton.OK,                MessageBoxImage.Information);
                   // globalVars.jerkLabel.Content = this.prevAccel[0] + ", " + f0 + ", " + totJI[0];
                    
              //      if (feature.peakAccel[0] < this.prevAccel[0])
                    {
                        feature.peakAccel[0] = this.prevAccel[0];
                    }
                  
                    //else 
                    if (this.feature.peakDec[0] > this.prevAccel[0])
                    {
                        this.feature.peakDec[0] = this.prevAccel[0];
                    }

                    f0 = this.prevAccel[1];
                    this.prevAccel[1] = (2 * S[1] - 2 * this.prevSpeed[1] * 0.2) / 0.04; //s=ut+.5ft^2
                    jIndex[1] = (this.prevAccel[1] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    totJI[1] += Math.Abs(jIndex[1]);

                    if (this.feature.peakAccel[1] < this.prevAccel[1])
                    {
                        this.feature.peakAccel[1] = this.prevAccel[1];
                    }
                    else if (this.feature.peakDec[1] > this.prevAccel[1])
                    {
                        this.feature.peakDec[1] = this.prevAccel[1];
                    }


                    f0 = this.prevAccel[2];
                    this.prevAccel[2] = (2 * S[2] - 2 * this.prevSpeed[2] * 0.2) / 0.04; //s=ut+.5ft^2
                    jIndex[2] = (this.prevAccel[2] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    totJI[2] += Math.Abs(jIndex[2]);

                    if (this.feature.peakAccel[2] < this.prevAccel[2])
                    {
                        this.feature.peakAccel[2] = this.prevAccel[2];
                    }
                    else if (this.feature.peakDec[2] > this.prevAccel[2])
                    {
                        this.feature.peakDec[2] = this.prevAccel[2];
                    }


                    f0 = this.prevAccel[3];
                    this.prevAccel[3] = (2 * S[3] - 2 * this.prevSpeed[3] * 0.2) / 0.04; //s=ut+.5ft^2
                    jIndex[3] = (this.prevAccel[3] - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    totJI[3] += Math.Abs(jIndex[3]);

                    if (this.feature.peakAccel[3] < this.prevAccel[3])
                    {
                        this.feature.peakAccel[3] = this.prevAccel[3];
                    }
                    else if (this.feature.peakDec[3] > this.prevAccel[3])
                    {
                        this.feature.peakDec[3] = this.prevAccel[3];
                    }

                    //this.totAccel += this.prevAccel; //now calculating the whole distance*2 / t^2
                    //this.feature.acc += this.prevAccel + ",";
                    //this.prevSpeed = this.prevSpeed + this.prevAccel * 0.2; //v=u+ft
                    //this.feature.sp += this.prevSpeed + ",";
                    this.prevSpeed[0] = S[0] / 0.2; // the v=u+ft was toohot to handle for negative speed values.
                    this.prevSpeed[1] = S[1] / 0.2; 
                    this.prevSpeed[2] = S[2] / 0.2; 
                    this.prevSpeed[3] = S[3] / 0.2; 

                }

                returnVal[0][0] = S[0];
                returnVal[0][1] = S[1];
                returnVal[0][2] = S[2];
                returnVal[0][3] = S[3];

                returnVal[1][0] = jIndex[0];
                returnVal[1][1] = jIndex[1];
                returnVal[1][2] = jIndex[2];
                returnVal[1][3] = jIndex[3];
           

            }

            return returnVal;
        
        }
        
        protected void dangSpeed(){

            // this is 30/frame because total distance / total time in second
            feature.lHandSpeedMps = this.wDist[0] / (this.frame/30);
            feature.rHandSpeedMps = this.wDist[1] / (this.frame / 30);
            feature.lElbowSpeedMps = this.wDist[2] / (this.frame / 30);
            feature.rElbowSpeedMps = this.wDist[3] / (this.frame / 30); 

            feature.avgAccel[0] = this.wDist[0] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[1] = this.wDist[1] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[2] = this.wDist[2] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[3] = this.wDist[3] / Math.Pow(this.frame / 30, 2);
        }

        protected void dangQuality() {
            roundedness();
            jerkIndex();

        }
        
        protected void speed(){
            
         //   double distance = Math.Sqrt(Math.Pow((feature.spine.maxz.x - feature.spine.minz.x), 2) + Math.Pow((feature.spine.maxz.y - feature.spine.minz.y), 2) + Math.Pow((feature.spine.maxz.z - feature.spine.minz.z), 2));
            feature.speedMps = (spineDist * 6) / this.frame;
        }
        
        protected double updateSpine (){
            float temp = sdata.Joints[JointID.Spine].Position.Z  - 0*3;
            double S = 0;
                
            if (temp > feature.spine.maxz.z)
            {
                feature.spine.maxz.x = sdata.Joints[JointID.Spine].Position.X  - 0*1;
                feature.spine.maxz.y = sdata.Joints[JointID.Spine].Position.Y  - 0*2;
                feature.spine.maxz.z = temp;
            }
            temp = sdata.Joints[JointID.Spine].Position.Z  - 0*3;
            if (temp < feature.spine.minz.z)
            {
                feature.spine.minz.x = sdata.Joints[JointID.Spine].Position.X  - 0*1;
                feature.spine.minz.y = sdata.Joints[JointID.Spine].Position.Y  - 0*2;
                feature.spine.minz.z = temp;
            }
            
            
            if (this.spinePrev.x == -100)
            {
                spineDist = 0;
                this.spinePrev.x = this.sdata.Joints[JointID.Spine].Position.X  - 0*1;
                this.spinePrev.y = this.sdata.Joints[JointID.Spine].Position.Y  - 0*2;
                this.spinePrev.z = this.sdata.Joints[JointID.Spine].Position.Z  - 0*3;
                
            }
            else if (this.frame % 5 == 0 && this.spinePrev.x != -100)
            {
                S = 0;
                if ((this.spinePrev.x - this.sdata.Joints[JointID.Spine].Position.X - 0 * 2) >= 0.005 || (this.spinePrev.y - this.sdata.Joints[JointID.Spine].Position.Y - 0 * 2) >= 0.005 || (this.spinePrev.z - this.sdata.Joints[JointID.Spine].Position.Z - 0 * 2) >= 0.005)
                {

                    S = Math.Sqrt(Math.Pow((this.spinePrev.x - this.sdata.Joints[JointID.Spine].Position.X - 0 * 1), 2) + Math.Pow((this.spinePrev.y - this.sdata.Joints[JointID.Spine].Position.Y - 0 * 2), 2) + Math.Pow((this.spinePrev.z - this.sdata.Joints[JointID.Spine].Position.Z - 0 * 3), 2));
                    this.spinePrev.x = this.sdata.Joints[JointID.Spine].Position.X;
                    this.spinePrev.y = this.sdata.Joints[JointID.Spine].Position.Y;
                    this.spinePrev.z = this.sdata.Joints[JointID.Spine].Position.Z;
                }
                spineDist += S;
            }
            return S;
        }

        protected void headAndShoulders(){
            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.Head].Position.X  - this.baseX;
            if (temp > feature.head.maxx.x)
            {
                feature.head.maxx.x = temp;
                feature.head.maxx.y = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
                feature.head.maxx.z = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
            if (temp > feature.head.maxy.y)
            {
                feature.head.maxy.x = sdata.Joints[JointID.Head].Position.X  - this.baseX;
                feature.head.maxy.y = temp; 
                feature.head.maxy.z = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            } 
            temp = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            if (temp > feature.head.maxz.z)
            {
                feature.head.maxz.x = sdata.Joints[JointID.Head].Position.X  - this.baseX;
                feature.head.maxz.y = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
                feature.head.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.Head].Position.X  - this.baseX;
            if (temp < feature.head.minx.x)
            {
                feature.head.minx.x = temp;
                feature.head.minx.y = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
                feature.head.minx.z = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
            if (temp < feature.head.miny.y)
            {
                feature.head.miny.x = sdata.Joints[JointID.Head].Position.X  - this.baseX;
                feature.head.miny.y = temp;
                feature.head.miny.z = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.Head].Position.Z  - this.baseZ;
            if (temp < feature.head.minz.z)
            {
                feature.head.minz.x = sdata.Joints[JointID.Head].Position.X  - this.baseX;
                feature.head.minz.y = sdata.Joints[JointID.Head].Position.Y  - this.baseY;
                feature.head.minz.z = temp;
            }

            //lShoulder
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
            if (temp > feature.lShoulder.maxx.x)
            {
                feature.lShoulder.maxx.x = temp;
                feature.lShoulder.maxx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
                feature.lShoulder.maxx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
            if (temp > feature.lShoulder.maxy.y)
            {
                feature.lShoulder.maxy.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
                feature.lShoulder.maxy.y = temp;
                feature.lShoulder.maxy.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            if (temp > feature.lShoulder.maxz.z)
            {
                feature.lShoulder.maxz.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
                feature.lShoulder.maxz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
                feature.lShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
            if (temp < feature.lShoulder.minx.x)
            {
                feature.lShoulder.minx.x = temp;
                feature.lShoulder.minx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
                feature.lShoulder.minx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
            if (temp < feature.lShoulder.miny.y)
            {
                feature.lShoulder.miny.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
                feature.lShoulder.miny.y = temp;
                feature.lShoulder.miny.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z  - this.baseZ;
            if (temp < feature.lShoulder.minz.z)
            {
                feature.lShoulder.minz.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - this.baseX;
                feature.lShoulder.minz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - this.baseY;
                feature.lShoulder.minz.z = temp;
            }


            //rShoulder
            temp = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
            if (temp > feature.rShoulder.maxx.x)
            {
                feature.rShoulder.maxx.x = temp;
                feature.rShoulder.maxx.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
                feature.rShoulder.maxx.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
            if (temp > feature.rShoulder.maxy.y)
            {
                feature.rShoulder.maxy.x = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
                feature.rShoulder.maxy.y = temp;
                feature.rShoulder.maxy.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            if (temp > feature.rShoulder.maxz.z)
            {
                feature.rShoulder.maxz.x = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
                feature.rShoulder.maxz.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
                feature.rShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
            if (temp < feature.rShoulder.minx.x)
            {
                feature.rShoulder.minx.x = temp;
                feature.rShoulder.minx.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
                feature.rShoulder.minx.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
            if (temp < feature.rShoulder.miny.y)
            {
                feature.rShoulder.miny.x = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
                feature.rShoulder.miny.y = temp;
                feature.rShoulder.miny.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z  - this.baseZ;
            if (temp < feature.rShoulder.minz.z)
            {
                feature.rShoulder.minz.x = sdata.Joints[JointID.ShoulderRight].Position.X  - this.baseX;
                feature.rShoulder.minz.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - this.baseY;
                feature.rShoulder.minz.z = temp;
            }

        
        }

        private void roundedness(){
            //later
        }

        private void jerkIndex(){
            this.feature.jerkIndex[0] = ( this.totJI[0] * 6) / (this.frame * this.feature.lHandSpeedMps);
            this.feature.jerkIndex[1] = ( this.totJI[1] * 6) / (this.frame * this.feature.rHandSpeedMps);
            this.feature.jerkIndex[2] = ( this.totJI[2] * 6) / (this.frame * this.feature.lElbowSpeedMps);
            this.feature.jerkIndex[3] = ( this.totJI[3] * 6) / (this.frame * this.feature.rElbowSpeedMps);
        }
        //
        private void makeFeatureSet()
        {
            this.featureSet = new double[7];

            this.featureSet[0] = this.feature.speedMps;
            this.featureSet[1] = this.feature.lHandSpeedMps;
            this.featureSet[2] = this.feature.rHandSpeedMps;
            this.featureSet[3] = this.feature.avgAccel[0];
            //            this.featureSet[4] = this.feature.jerkIndex[0] / 120;
            this.featureSet[4] = this.feature.avgAccel[1];
            //            this.featureSet[9] = this.feature.jerkIndex[1] / 120;
            this.featureSet[5] = this.feature.avgAccel[2];
//            this.featureSet[14] = jerkIndex[2] / 120;
            this.featureSet[6] = this.feature.avgAccel[3];
            //            this.featureSet[19] = this.feature.f.jerkIndex[3] / 120;


        }
    }


}