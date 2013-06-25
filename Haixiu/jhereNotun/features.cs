
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



namespace WpfApplication1{
    public class globalVars
    {
        public static JointID[] jid = new JointID[20]{JointID.AnkleLeft, JointID.AnkleRight, JointID.ElbowLeft, JointID.ElbowRight, JointID.FootLeft, JointID.FootRight, JointID.HandLeft, JointID.HandRight, 
                                JointID.Head, JointID.HipCenter, JointID.HipLeft, JointID.HipRight, JointID.KneeLeft, JointID.KneeRight, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ShoulderRight,
                                JointID.Spine, JointID.WristLeft, JointID.WristRight};
        public static bool logFeatures;
        public static startFeatures gFeature;
        public static bool kinectOn, logSkele;
        public static Label a1,a2;
    }

    public struct _qbit
    {
        public float x, y, z;
    }
    public struct _quanta
    {
        public _qbit minx, maxx, wmeanx, meanx;
        public _qbit miny, maxy, wmeany, meany;
        public _qbit minz, maxz, wmeanz, meanz;
    };
    public struct _feature
    {
        public _quanta head, rShoulder, lShoulder;
        public _quanta spine;
        public _quanta lElbow, lWrist, lHand, rElbow, rWrist, rHand;
        public double speedMps, lHandSpeedMps, lElbowSpeedMps, rHandSpeedMps, rElbowSpeedMps;
        public double[] peakAccel, avgAccel, peakDec, jerkIndex, roundedness;// indexes are [0,1,2,3] =lHand,lWrist,rHand,rWrist
        //refer to initFeatures

        //dummyFeatures
        public String acc, dis, sp;

    };



    public class startFeatures : globalVars
    {
        protected int frame;

        private _feature feature;
        private SkeletonData sdata;
        private double[] wDist;
        private double spineDist;
        private _qbit[] wprev;
        private _qbit spinePrev;
        private double[] prevAccel, prevSpeed, totJI;
        private float baseX, baseY, baseZ;

        public startFeatures(){
            this.frame = 0;
            this.wDist = new double[4]{0,0,0,0};
            this.spineDist = 0;
            this.prevAccel = new double[4] { 0, 0, 0, 0 };

            this.prevSpeed = new double[4] { 0, 0, 0, 0 };
            this.totJI = new double[4] { 0, 0, 0, 0 };

            this.wprev = new _qbit[4];
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

            this.spinePrev.x = -100;
            this.spinePrev.y = -100;
            this.spinePrev.z = -100;

            this.baseX = 0;
            this.baseY = 0;
            this.baseZ = 0;



            //initFeatures is for the actual features while the prev inits are local vars.
            initFeatures();

        }

        private void initFeatures(){

            this.feature.jerkIndex = new double[4];
            this.feature.roundedness = new double[4];
            this.feature.avgAccel = new double[4]; 
            this.feature.peakDec = new double[4];
            this.feature.peakAccel = new double[4];

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
            if (baseZ == 0)
            {   
                this.baseX = s.Joints[JointID.Spine].Position.X  - 0*1;
                this.baseY = s.Joints[JointID.Spine].Position.Y  - 0*2;
                this.baseZ = s.Joints[JointID.Spine].Position.Z  - 0*3;
            }
            
            this.sdata = s;
            
            stableSkele();
            danglingSkele();
            //underBeltSkele();

            ++frame;
            a1.Content = frame;

        }
      
        public void saveFeatures(){
            speed();
            dangSpeed();
            dangQuality();
            writeFeatures();
        }
        
        private void writeFeatures(){
            fileWriter f = new fileWriter(null,"feature.csv");
            f.writeFeatures(feature);
            f.closeFile();
            
        }
        
        private void danglingSkele() {
            dangPos();
            pollDangDistance();
        }
        
        private void stableSkele(){
            updateSpine();
            headAndShoulders();
        }
        
        private void underBeltSkele() { 
            //latter 
        }

        private void dangPos() {
            //rElbow
            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
            if (temp > feature.rElbow.maxx.x)
            {
                feature.rElbow.maxx.x = temp;
                feature.rElbow.maxx.y = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
                feature.rElbow.maxx.z = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
            if (temp > feature.rElbow.maxy.y)
            {
                feature.rElbow.maxy.x = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
                feature.rElbow.maxy.y = temp;
                feature.rElbow.maxy.z = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            if (temp > feature.rElbow.maxz.z)
            {
                feature.rElbow.maxz.x = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
                feature.rElbow.maxz.y = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
                feature.rElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
            if (temp < feature.rElbow.minx.x)
            {
                feature.rElbow.minx.x = temp;
                feature.rElbow.minx.y = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
                feature.rElbow.minx.z = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
            if (temp < feature.rElbow.miny.y)
            {
                feature.rElbow.miny.x = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
                feature.rElbow.miny.y = temp;
                feature.rElbow.miny.z = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z  - 0*3;
            if (temp < feature.rElbow.minz.z)
            {
                feature.rElbow.minz.x = sdata.Joints[JointID.ElbowRight].Position.X  - 0*1;
                feature.rElbow.minz.y = sdata.Joints[JointID.ElbowRight].Position.Y  - 0*2;
                feature.rElbow.minz.z = temp;
            }



            //lElbow
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
            if (temp > feature.lElbow.maxx.x)
            {
                feature.lElbow.maxx.x = temp;
                feature.lElbow.maxx.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
                feature.lElbow.maxx.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
            if (temp > feature.lElbow.maxy.y)
            {
                feature.lElbow.maxy.x = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
                feature.lElbow.maxy.y = temp;
                feature.lElbow.maxy.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            if (temp > feature.lElbow.maxz.z)
            {
                feature.lElbow.maxz.x = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
                feature.lElbow.maxz.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
                feature.lElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
            if (temp < feature.lElbow.minx.x)
            {
                feature.lElbow.minx.x = temp;
                feature.lElbow.minx.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
                feature.lElbow.minx.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
            if (temp < feature.lElbow.miny.y)
            {
                feature.lElbow.miny.x = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
                feature.lElbow.miny.y = temp;
                feature.lElbow.miny.z = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z  - 0*3;
            if (temp < feature.lElbow.minz.z)
            {
                feature.lElbow.minz.x = sdata.Joints[JointID.ElbowLeft].Position.X  - 0*1;
                feature.lElbow.minz.y = sdata.Joints[JointID.ElbowLeft].Position.Y  - 0*2;
                feature.lElbow.minz.z = temp;
            }


            //rHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
            if (temp > feature.rHand.maxx.x)
            {
                feature.rHand.maxx.x = temp;
                feature.rHand.maxx.y = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
                feature.rHand.maxx.z = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
            if (temp > feature.rHand.maxy.y)
            {
                feature.rHand.maxy.x = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
                feature.rHand.maxy.y = temp;
                feature.rHand.maxy.z = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            if (temp > feature.rHand.maxz.z)
            {
                feature.rHand.maxz.x = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
                feature.rHand.maxz.y = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
                feature.rHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
            if (temp < feature.rHand.minx.x)
            {
                feature.rHand.minx.x = temp;
                feature.rHand.minx.y = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
                feature.rHand.minx.z = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
            if (temp < feature.rHand.miny.y)
            {
                feature.rHand.miny.x = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
                feature.rHand.miny.y = temp;
                feature.rHand.miny.z = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z  - 0*3;
            if (temp < feature.rHand.minz.z)
            {
                feature.rHand.minz.x = sdata.Joints[JointID.HandRight].Position.X  - 0*1;
                feature.rHand.minz.y = sdata.Joints[JointID.HandRight].Position.Y  - 0*2;
                feature.rHand.minz.z = temp;
            }



            //lHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
            if (temp > feature.lHand.maxx.x)
            {
                feature.lHand.maxx.x = temp;
                feature.lHand.maxx.y = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
                feature.lHand.maxx.z = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
            if (temp > feature.lHand.maxy.y)
            {
                feature.lHand.maxy.x = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
                feature.lHand.maxy.y = temp;
                feature.lHand.maxy.z = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            if (temp > feature.lHand.maxz.z)
            {
                feature.lHand.maxz.x = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
                feature.lHand.maxz.y = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
                feature.lHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
            if (temp < feature.lHand.minx.x)
            {
                feature.lHand.minx.x = temp;
                feature.lHand.minx.y = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
                feature.lHand.minx.z = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
            if (temp < feature.lHand.miny.y)
            {
                feature.lHand.miny.x = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
                feature.lHand.miny.y = temp;
                feature.lHand.miny.z = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z  - 0*3;
            if (temp < feature.lHand.minz.z)
            {
                feature.lHand.minz.x = sdata.Joints[JointID.HandLeft].Position.X  - 0*1;
                feature.lHand.minz.y = sdata.Joints[JointID.HandLeft].Position.Y  - 0*2;
                feature.lHand.minz.z = temp;
            }


            //rWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
            if (temp > feature.rWrist.maxx.x)
            {
                feature.rWrist.maxx.x = temp;
                feature.rWrist.maxx.y = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
                feature.rWrist.maxx.z = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
            if (temp > feature.rWrist.maxy.y)
            {
                feature.rWrist.maxy.x = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
                feature.rWrist.maxy.y = temp;
                feature.rWrist.maxy.z = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            if (temp > feature.rWrist.maxz.z)
            {
                feature.rWrist.maxz.x = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
                feature.rWrist.maxz.y = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
                feature.rWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
            if (temp < feature.rWrist.minx.x)
            {
                feature.rWrist.minx.x = temp;
                feature.rWrist.minx.y = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
                feature.rWrist.minx.z = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
            if (temp < feature.rWrist.miny.y)
            {
                feature.rWrist.miny.x = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
                feature.rWrist.miny.y = temp;
                feature.rWrist.miny.z = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z  - 0*3;
            if (temp < feature.rWrist.minz.z)
            {
                feature.rWrist.minz.x = sdata.Joints[JointID.WristRight].Position.X  - 0*1;
                feature.rWrist.minz.y = sdata.Joints[JointID.WristRight].Position.Y  - 0*2;
                feature.rWrist.minz.z = temp;
            }



            //lWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
            if (temp > feature.lWrist.maxx.x)
            {
                feature.lWrist.maxx.x = temp;
                feature.lWrist.maxx.y = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
                feature.lWrist.maxx.z = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
            if (temp > feature.lWrist.maxy.y)
            {
                feature.lWrist.maxy.x = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
                feature.lWrist.maxy.y = temp;
                feature.lWrist.maxy.z = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            if (temp > feature.lWrist.maxz.z)
            {
                feature.lWrist.maxz.x = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
                feature.lWrist.maxz.y = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
                feature.lWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
            if (temp < feature.lWrist.minx.x)
            {
                feature.lWrist.minx.x = temp;
                feature.lWrist.minx.y = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
                feature.lWrist.minx.z = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
            if (temp < feature.lWrist.miny.y)
            {
                feature.lWrist.miny.x = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
                feature.lWrist.miny.y = temp;
                feature.lWrist.miny.z = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z  - 0*3;
            if (temp < feature.lWrist.minz.z)
            {
                feature.lWrist.minz.x = sdata.Joints[JointID.WristLeft].Position.X  - 0*1;
                feature.lWrist.minz.y = sdata.Joints[JointID.WristLeft].Position.Y  - 0*2;
                feature.lWrist.minz.z = temp;
            }

        }

        private void pollDangDistance() {

            double[] jIndex = new double[4] { 0,0,0,0 };
            double f0=0;
            if (this.wprev[0].x == -100)
            {
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

                double[] S = new double[4];
                S[0] = Math.Sqrt(Math.Pow((this.wprev[0].x - this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[0].y - this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[0].z - this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 3), 2));
                S[1] = Math.Sqrt(Math.Pow((this.wprev[0].x - this.sdata.Joints[JointID.WristRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[0].y - this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[0].z - this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 3), 2));
                S[2] = Math.Sqrt(Math.Pow((this.wprev[0].x - this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[0].y - this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[0].z - this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 3), 2));
                S[3] = Math.Sqrt(Math.Pow((this.wprev[0].x - this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 1), 2) + Math.Pow((this.wprev[0].y - this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2), 2) + Math.Pow((this.wprev[0].z - this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 3), 2));

                
                
                wDist[0] += S[0];
                wDist[1] += S[1];
                wDist[2] += S[2];
                wDist[3] += S[3];

                //this.feature.dis += S + ",";
                
                a2.Content = wDist[0];//updating label

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

                    if (this.feature.peakAccel[0] < this.prevAccel[0])
                    {
                        this.feature.peakAccel[0] = this.prevAccel[0];
                    }
                    else if (this.feature.peakDec[0] > this.prevAccel[0])
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


                this.wprev[0].x = this.sdata.Joints[JointID.WristLeft].Position.X - 0 * 1;
                this.wprev[0].y = this.sdata.Joints[JointID.WristLeft].Position.Y - 0 * 2;
                this.wprev[0].z = this.sdata.Joints[JointID.WristLeft].Position.Z - 0 * 3;

                this.wprev[1].x = this.sdata.Joints[JointID.WristRight].Position.X - 0 * 1;
                this.wprev[1].y = this.sdata.Joints[JointID.WristRight].Position.Y - 0 * 2;
                this.wprev[1].z = this.sdata.Joints[JointID.WristRight].Position.Z - 0 * 3;

                this.wprev[2].x = this.sdata.Joints[JointID.ElbowLeft].Position.X - 0 * 1;
                this.wprev[2].y = this.sdata.Joints[JointID.ElbowLeft].Position.Y - 0 * 2;
                this.wprev[2].z = this.sdata.Joints[JointID.ElbowLeft].Position.Z - 0 * 3;

                this.wprev[3].x = this.sdata.Joints[JointID.ElbowRight].Position.X - 0 * 1;
                this.wprev[3].y = this.sdata.Joints[JointID.ElbowRight].Position.Y - 0 * 2;
                this.wprev[3].z = this.sdata.Joints[JointID.ElbowRight].Position.Z - 0 * 3;

            }
        }
        
        private void dangSpeed(){

            // this is 30/frame because total distance / total time in second
            feature.lHandSpeedMps = this.wDist[0] * 30 / this.frame; 
            feature.rHandSpeedMps = this.wDist[1] * 30 / this.frame; 
            feature.lElbowSpeedMps = this.wDist[2] * 30 / this.frame; 
            feature.rElbowSpeedMps = this.wDist[3] * 30 / this.frame; 

            feature.avgAccel[0] = this.wDist[0] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[1] = this.wDist[1] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[2] = this.wDist[2] / Math.Pow(this.frame / 30, 2);
            feature.avgAccel[3] = this.wDist[3] / Math.Pow(this.frame / 30, 2);
        }

        private void dangQuality() {
            roundedness();
            jerkIndex();

        }
        
        private void speed(){
            
         //   double distance = Math.Sqrt(Math.Pow((feature.spine.maxz.x - feature.spine.minz.x), 2) + Math.Pow((feature.spine.maxz.y - feature.spine.minz.y), 2) + Math.Pow((feature.spine.maxz.z - feature.spine.minz.z), 2));
            feature.speedMps = (spineDist * 6) / this.frame;
        }
        
        private void updateSpine (){
            float temp = sdata.Joints[JointID.Spine].Position.Z  - 0*3;
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
                double S = Math.Sqrt(Math.Pow((this.spinePrev.x - this.sdata.Joints[JointID.Spine].Position.X - 0 * 1), 2) + Math.Pow((this.spinePrev.y - this.sdata.Joints[JointID.Spine].Position.Y - 0 * 2), 2) + Math.Pow((this.spinePrev.z - this.sdata.Joints[JointID.Spine].Position.Z - 0 * 3), 2));
                spineDist += S;
            }
        }

        private void headAndShoulders(){
            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.Head].Position.X  - 0*1;
            if (temp > feature.head.maxx.x)
            {
                feature.head.maxx.x = temp;
                feature.head.maxx.y = sdata.Joints[JointID.Head].Position.Y  - 0*2;
                feature.head.maxx.z = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.Head].Position.Y  - 0*2;
            if (temp > feature.head.maxy.y)
            {
                feature.head.maxy.x = sdata.Joints[JointID.Head].Position.X  - 0*1;
                feature.head.maxy.y = temp; 
                feature.head.maxy.z = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            } 
            temp = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            if (temp > feature.head.maxz.z)
            {
                feature.head.maxz.x = sdata.Joints[JointID.Head].Position.X  - 0*1;
                feature.head.maxz.y = sdata.Joints[JointID.Head].Position.Y  - 0*2;
                feature.head.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.Head].Position.X  - 0*1;
            if (temp < feature.head.minx.x)
            {
                feature.head.minx.x = temp;
                feature.head.minx.y = sdata.Joints[JointID.Head].Position.Y  - 0*2;
                feature.head.minx.z = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.Head].Position.Y  - 0*2;
            if (temp < feature.head.miny.y)
            {
                feature.head.miny.x = sdata.Joints[JointID.Head].Position.X  - 0*1;
                feature.head.miny.y = temp;
                feature.head.miny.z = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.Head].Position.Z  - 0*3;
            if (temp < feature.head.minz.z)
            {
                feature.head.minz.x = sdata.Joints[JointID.Head].Position.X  - 0*1;
                feature.head.minz.y = sdata.Joints[JointID.Head].Position.Y  - 0*2;
                feature.head.minz.z = temp;
            }

            //lShoulder
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
            if (temp > feature.lShoulder.maxx.x)
            {
                feature.lShoulder.maxx.x = temp;
                feature.lShoulder.maxx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
                feature.lShoulder.maxx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
            if (temp > feature.lShoulder.maxy.y)
            {
                feature.lShoulder.maxy.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
                feature.lShoulder.maxy.y = temp;
                feature.lShoulder.maxy.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            if (temp > feature.lShoulder.maxz.z)
            {
                feature.lShoulder.maxz.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
                feature.lShoulder.maxz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
                feature.lShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
            if (temp < feature.lShoulder.minx.x)
            {
                feature.lShoulder.minx.x = temp;
                feature.lShoulder.minx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
                feature.lShoulder.minx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
            if (temp < feature.lShoulder.miny.y)
            {
                feature.lShoulder.miny.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
                feature.lShoulder.miny.y = temp;
                feature.lShoulder.miny.z = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z  - 0*3;
            if (temp < feature.lShoulder.minz.z)
            {
                feature.lShoulder.minz.x = sdata.Joints[JointID.ShoulderLeft].Position.X  - 0*1;
                feature.lShoulder.minz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y  - 0*2;
                feature.lShoulder.minz.z = temp;
            }


            //rShoulder
            temp = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
            if (temp > feature.rShoulder.maxx.x)
            {
                feature.rShoulder.maxx.x = temp;
                feature.rShoulder.maxx.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
                feature.rShoulder.maxx.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
            if (temp > feature.rShoulder.maxy.y)
            {
                feature.rShoulder.maxy.x = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
                feature.rShoulder.maxy.y = temp;
                feature.rShoulder.maxy.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            if (temp > feature.rShoulder.maxz.z)
            {
                feature.rShoulder.maxz.x = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
                feature.rShoulder.maxz.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
                feature.rShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
            if (temp < feature.rShoulder.minx.x)
            {
                feature.rShoulder.minx.x = temp;
                feature.rShoulder.minx.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
                feature.rShoulder.minx.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
            if (temp < feature.rShoulder.miny.y)
            {
                feature.rShoulder.miny.x = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
                feature.rShoulder.miny.y = temp;
                feature.rShoulder.miny.z = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z  - 0*3;
            if (temp < feature.rShoulder.minz.z)
            {
                feature.rShoulder.minz.x = sdata.Joints[JointID.ShoulderRight].Position.X  - 0*1;
                feature.rShoulder.minz.y = sdata.Joints[JointID.ShoulderRight].Position.Y  - 0*2;
                feature.rShoulder.minz.z = temp;
            }

        
        }

        private void roundedness(){
            //later
        }

        private void jerkIndex(){
            this.feature.jerkIndex[0] = (-1 * this.totJI[0] * 6) / (this.frame * this.feature.lHandSpeedMps);
            this.feature.jerkIndex[1] = (-1 * this.totJI[1] * 6) / (this.frame * this.feature.rHandSpeedMps);
            this.feature.jerkIndex[2] = (-1 * this.totJI[2] * 6) / (this.frame * this.feature.lElbowSpeedMps);
            this.feature.jerkIndex[3] = (-1 * this.totJI[3] * 6) / (this.frame * this.feature.rElbowSpeedMps);
        }

    }
}