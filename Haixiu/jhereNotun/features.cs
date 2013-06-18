using System;
using Microsoft.Research.Kinect.Nui;

namespace WpfApplication1{
    public class globalVars
    {
        public static JointID[] jid = new JointID[20]{JointID.AnkleLeft, JointID.AnkleRight, JointID.ElbowLeft, JointID.ElbowRight, JointID.FootLeft, JointID.FootRight, JointID.HandLeft, JointID.HandRight, 
                                JointID.Head, JointID.HipCenter, JointID.HipLeft, JointID.HipRight, JointID.KneeLeft, JointID.KneeRight, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ShoulderRight,
                                JointID.Spine, JointID.WristLeft, JointID.WristRight};
        public static bool logFeatures;
        public static startFeatures gFeature;
        public static bool kinectOn, logSkele;
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
        public double speedMps, handSpeedMps;
        public double[] peakAccel, avgAccel, jerkIndex, roundedness;// indexes are [0,1,2,3] =lHand,lWrist,rHand,rWrist
        //refer to initFeatures

    };



    public class startFeatures : globalVars
    {
        protected int frame;

        private _feature feature, feTemp;
        private SkeletonData sdata;
        private double wDist;
        private _qbit wprev;
        private double prevAccel, totAccel, prevSpeed, totJI;


        public startFeatures(){
            this.frame = 0;
            this.wDist = 0;
            this.prevAccel = 0;
            this.totAccel = 0;
            this.prevSpeed = 0;
            this.totJI = 0;
            this.wprev.x = -100;
            this.wprev.y = -100;
            this.wprev.z = -100;
            //initFeatures is for the actual features while the prev inits are local vars.
            initFeatures();

        }

        private void initFeatures(){

            this.feature.jerkIndex = new double[4];
            this.feature.roundedness = new double[4];
            this.feature.avgAccel = new double[4];
            this.feature.peakAccel = new double[4];

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
            this.sdata = s;
            stableSkele();
            this.frame++;
        }
      
        public void saveFeatures(){
            speed();
            dangSpeed();
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
            dangQuality();

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
            float temp = sdata.Joints[JointID.ElbowRight].Position.X;
            if (temp > feature.rElbow.maxx.x)
            {
                feature.rElbow.maxx.x = temp;
                feature.rElbow.maxx.y = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.maxx.z = sdata.Joints[JointID.ElbowRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y;
            if (temp > feature.rElbow.maxy.y)
            {
                feature.rElbow.maxy.x = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.maxy.y = temp;
                feature.rElbow.maxy.z = sdata.Joints[JointID.ElbowRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z;
            if (temp > feature.rElbow.maxz.z)
            {
                feature.rElbow.maxz.x = sdata.Joints[JointID.ElbowRight].Position.X;
                feature.rElbow.maxz.y = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowRight].Position.X;
            if (temp < feature.rElbow.minx.x)
            {
                feature.rElbow.minx.x = temp;
                feature.rElbow.minx.y = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.minx.z = sdata.Joints[JointID.ElbowRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Y;
            if (temp < feature.rElbow.miny.y)
            {
                feature.rElbow.miny.x = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.miny.y = temp;
                feature.rElbow.miny.z = sdata.Joints[JointID.ElbowRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowRight].Position.Z;
            if (temp < feature.rElbow.minz.z)
            {
                feature.rElbow.minz.x = sdata.Joints[JointID.ElbowRight].Position.X;
                feature.rElbow.minz.y = sdata.Joints[JointID.ElbowRight].Position.Y;
                feature.rElbow.minz.z = temp;
            }



            //lElbow
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X;
            if (temp > feature.lElbow.maxx.x)
            {
                feature.lElbow.maxx.x = temp;
                feature.lElbow.maxx.y = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.maxx.z = sdata.Joints[JointID.ElbowLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y;
            if (temp > feature.lElbow.maxy.y)
            {
                feature.lElbow.maxy.x = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.maxy.y = temp;
                feature.lElbow.maxy.z = sdata.Joints[JointID.ElbowLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z;
            if (temp > feature.lElbow.maxz.z)
            {
                feature.lElbow.maxz.x = sdata.Joints[JointID.ElbowLeft].Position.X;
                feature.lElbow.maxz.y = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ElbowLeft].Position.X;
            if (temp < feature.lElbow.minx.x)
            {
                feature.lElbow.minx.x = temp;
                feature.lElbow.minx.y = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.minx.z = sdata.Joints[JointID.ElbowLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Y;
            if (temp < feature.lElbow.miny.y)
            {
                feature.lElbow.miny.x = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.miny.y = temp;
                feature.lElbow.miny.z = sdata.Joints[JointID.ElbowLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ElbowLeft].Position.Z;
            if (temp < feature.lElbow.minz.z)
            {
                feature.lElbow.minz.x = sdata.Joints[JointID.ElbowLeft].Position.X;
                feature.lElbow.minz.y = sdata.Joints[JointID.ElbowLeft].Position.Y;
                feature.lElbow.minz.z = temp;
            }


            //rHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandRight].Position.X;
            if (temp > feature.rHand.maxx.x)
            {
                feature.rHand.maxx.x = temp;
                feature.rHand.maxx.y = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.maxx.z = sdata.Joints[JointID.HandRight].Position.Z;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y;
            if (temp > feature.rHand.maxy.y)
            {
                feature.rHand.maxy.x = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.maxy.y = temp;
                feature.rHand.maxy.z = sdata.Joints[JointID.HandRight].Position.Z;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z;
            if (temp > feature.rHand.maxz.z)
            {
                feature.rHand.maxz.x = sdata.Joints[JointID.HandRight].Position.X;
                feature.rHand.maxz.y = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandRight].Position.X;
            if (temp < feature.rHand.minx.x)
            {
                feature.rHand.minx.x = temp;
                feature.rHand.minx.y = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.minx.z = sdata.Joints[JointID.HandRight].Position.Z;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Y;
            if (temp < feature.rHand.miny.y)
            {
                feature.rHand.miny.x = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.miny.y = temp;
                feature.rHand.miny.z = sdata.Joints[JointID.HandRight].Position.Z;
            }
            temp = sdata.Joints[JointID.HandRight].Position.Z;
            if (temp < feature.rHand.minz.z)
            {
                feature.rHand.minz.x = sdata.Joints[JointID.HandRight].Position.X;
                feature.rHand.minz.y = sdata.Joints[JointID.HandRight].Position.Y;
                feature.rHand.minz.z = temp;
            }



            //lHand
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.HandLeft].Position.X;
            if (temp > feature.lHand.maxx.x)
            {
                feature.lHand.maxx.x = temp;
                feature.lHand.maxx.y = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.maxx.z = sdata.Joints[JointID.HandLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y;
            if (temp > feature.lHand.maxy.y)
            {
                feature.lHand.maxy.x = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.maxy.y = temp;
                feature.lHand.maxy.z = sdata.Joints[JointID.HandLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z;
            if (temp > feature.lHand.maxz.z)
            {
                feature.lHand.maxz.x = sdata.Joints[JointID.HandLeft].Position.X;
                feature.lHand.maxz.y = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.HandLeft].Position.X;
            if (temp < feature.lHand.minx.x)
            {
                feature.lHand.minx.x = temp;
                feature.lHand.minx.y = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.minx.z = sdata.Joints[JointID.HandLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Y;
            if (temp < feature.lHand.miny.y)
            {
                feature.lHand.miny.x = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.miny.y = temp;
                feature.lHand.miny.z = sdata.Joints[JointID.HandLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.HandLeft].Position.Z;
            if (temp < feature.lHand.minz.z)
            {
                feature.lHand.minz.x = sdata.Joints[JointID.HandLeft].Position.X;
                feature.lHand.minz.y = sdata.Joints[JointID.HandLeft].Position.Y;
                feature.lHand.minz.z = temp;
            }


            //rWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristRight].Position.X;
            if (temp > feature.rWrist.maxx.x)
            {
                feature.rWrist.maxx.x = temp;
                feature.rWrist.maxx.y = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.maxx.z = sdata.Joints[JointID.WristRight].Position.Z;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y;
            if (temp > feature.rWrist.maxy.y)
            {
                feature.rWrist.maxy.x = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.maxy.y = temp;
                feature.rWrist.maxy.z = sdata.Joints[JointID.WristRight].Position.Z;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z;
            if (temp > feature.rWrist.maxz.z)
            {
                feature.rWrist.maxz.x = sdata.Joints[JointID.WristRight].Position.X;
                feature.rWrist.maxz.y = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristRight].Position.X;
            if (temp < feature.rWrist.minx.x)
            {
                feature.rWrist.minx.x = temp;
                feature.rWrist.minx.y = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.minx.z = sdata.Joints[JointID.WristRight].Position.Z;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Y;
            if (temp < feature.rWrist.miny.y)
            {
                feature.rWrist.miny.x = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.miny.y = temp;
                feature.rWrist.miny.z = sdata.Joints[JointID.WristRight].Position.Z;
            }
            temp = sdata.Joints[JointID.WristRight].Position.Z;
            if (temp < feature.rWrist.minz.z)
            {
                feature.rWrist.minz.x = sdata.Joints[JointID.WristRight].Position.X;
                feature.rWrist.minz.y = sdata.Joints[JointID.WristRight].Position.Y;
                feature.rWrist.minz.z = temp;
            }



            //lWrist
            //maxx, maxy, maxz
            temp = sdata.Joints[JointID.WristLeft].Position.X;
            if (temp > feature.lWrist.maxx.x)
            {
                feature.lWrist.maxx.x = temp;
                feature.lWrist.maxx.y = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.maxx.z = sdata.Joints[JointID.WristLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y;
            if (temp > feature.lWrist.maxy.y)
            {
                feature.lWrist.maxy.x = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.maxy.y = temp;
                feature.lWrist.maxy.z = sdata.Joints[JointID.WristLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z;
            if (temp > feature.lWrist.maxz.z)
            {
                feature.lWrist.maxz.x = sdata.Joints[JointID.WristLeft].Position.X;
                feature.lWrist.maxz.y = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.WristLeft].Position.X;
            if (temp < feature.lWrist.minx.x)
            {
                feature.lWrist.minx.x = temp;
                feature.lWrist.minx.y = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.minx.z = sdata.Joints[JointID.WristLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Y;
            if (temp < feature.lWrist.miny.y)
            {
                feature.lWrist.miny.x = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.miny.y = temp;
                feature.lWrist.miny.z = sdata.Joints[JointID.WristLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.WristLeft].Position.Z;
            if (temp < feature.lWrist.minz.z)
            {
                feature.lWrist.minz.x = sdata.Joints[JointID.WristLeft].Position.X;
                feature.lWrist.minz.y = sdata.Joints[JointID.WristLeft].Position.Y;
                feature.lWrist.minz.z = temp;
            }

        }

        private void pollDangDistance() {
            double jIndex=0, f0=0, f1=0;
            if (this.wprev.x == -100)
            {
                wDist = 0;
                this.wprev.x = this.sdata.Joints[JointID.WristLeft].Position.X;
                this.wprev.y = this.sdata.Joints[JointID.WristLeft].Position.Y;
                this.wprev.z = this.sdata.Joints[JointID.WristLeft].Position.Z;
            }
            else if (this.frame % 5 == 0 && this.wprev.x != -100)
            {
                double S = Math.Sqrt(Math.Pow((this.wprev.x - this.sdata.Joints[JointID.WristLeft].Position.X), 2) + Math.Pow((this.wprev.y - this.sdata.Joints[JointID.WristLeft].Position.Y), 2) + Math.Pow((this.wprev.z - this.sdata.Joints[JointID.WristLeft].Position.Z), 2));
                wDist += S;
                if (this.prevSpeed == 0 && this.frame == 6)
                {
                    this.prevSpeed = S / 0.2;
                }
                else
                {
                    
                    //acceleration and jerk index calculation
                    f0 = this.prevAccel;
                    f1= this.prevAccel = (2 * S - 2 * this.prevSpeed * 0.2) / 0.04; //s=ut+.5ft^2
                    jIndex = (f1 - f0) / 0.2; //jerk Index = (f1-f0)/dt
                    totJI += jIndex;

                    if (this.feature.peakAccel[0] < this.prevAccel)
                    {
                        this.feature.peakAccel[0] = this.prevAccel;
                    }

                    this.totAccel += this.prevAccel;
                    this.prevSpeed = this.prevSpeed + this.prevAccel * 0.2; //v=u+ft
                }


                this.wprev.x = this.sdata.Joints[JointID.WristLeft].Position.X;
                this.wprev.y = this.sdata.Joints[JointID.WristLeft].Position.Y;
                this.wprev.z = this.sdata.Joints[JointID.WristLeft].Position.Z;


            }
        }
        
        private void dangSpeed(){
            feature.handSpeedMps = this.wDist * 30 / this.frame;
            feature.avgAccel[0] = this.totAccel * 6 / this.frame;
        }

        private void dangQuality() {
            roundedness();
            jerkIndex();

        }
        
        private void speed(){
            
            double distance = Math.Sqrt(Math.Pow((feature.spine.maxz.x - feature.spine.minz.x), 2) + Math.Pow((feature.spine.maxz.y - feature.spine.minz.y), 2) + Math.Pow((feature.spine.maxz.z - feature.spine.minz.z), 2));
            feature.speedMps = (distance * 30) / this.frame;
        }
        
        private void updateSpine (){
            float temp = sdata.Joints[JointID.Spine].Position.Z;
            if (temp > feature.spine.maxz.z)
            {
                feature.spine.maxz.x = sdata.Joints[JointID.Spine].Position.X;
                feature.spine.maxz.y = sdata.Joints[JointID.Spine].Position.Y;
                feature.spine.maxz.z = temp;
            }
            temp = sdata.Joints[JointID.Spine].Position.Z;
            if (temp < feature.spine.minz.z)
            {
                feature.spine.minz.x = sdata.Joints[JointID.Spine].Position.X;
                feature.spine.minz.y = sdata.Joints[JointID.Spine].Position.Y;
                feature.spine.minz.z = temp;
            }
        }

        private void headAndShoulders(){
            //maxx, maxy, maxz
            float temp = sdata.Joints[JointID.Head].Position.X;
            if (temp > feature.head.maxx.x)
            {
                feature.head.maxx.x = temp;
                feature.head.maxx.y = sdata.Joints[JointID.Head].Position.Y;
                feature.head.maxx.z = sdata.Joints[JointID.Head].Position.Z;
            }
            temp = sdata.Joints[JointID.Head].Position.Y;
            if (temp > feature.head.maxy.y)
            {
                feature.head.maxy.x = sdata.Joints[JointID.Head].Position.Y;
                feature.head.maxy.y = temp; 
                feature.head.maxy.z = sdata.Joints[JointID.Head].Position.Z;
            } 
            temp = sdata.Joints[JointID.Head].Position.Z;
            if (temp > feature.head.maxz.z)
            {
                feature.head.maxz.x = sdata.Joints[JointID.Head].Position.X;
                feature.head.maxz.y = sdata.Joints[JointID.Head].Position.Y;
                feature.head.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.Head].Position.X;
            if (temp < feature.head.minx.x)
            {
                feature.head.minx.x = temp;
                feature.head.minx.y = sdata.Joints[JointID.Head].Position.Y;
                feature.head.minx.z = sdata.Joints[JointID.Head].Position.Z;
            }
            temp = sdata.Joints[JointID.Head].Position.Y;
            if (temp < feature.head.miny.y)
            {
                feature.head.miny.x = sdata.Joints[JointID.Head].Position.Y;
                feature.head.miny.y = temp;
                feature.head.miny.z = sdata.Joints[JointID.Head].Position.Z;
            }
            temp = sdata.Joints[JointID.Head].Position.Z;
            if (temp < feature.head.minz.z)
            {
                feature.head.minz.x = sdata.Joints[JointID.Head].Position.X;
                feature.head.minz.y = sdata.Joints[JointID.Head].Position.Y;
                feature.head.minz.z = temp;
            }

            //lShoulder
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X;
            if (temp > feature.lShoulder.maxx.x)
            {
                feature.lShoulder.maxx.x = temp;
                feature.lShoulder.maxx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.maxx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y;
            if (temp > feature.lShoulder.maxy.y)
            {
                feature.lShoulder.maxy.x = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.maxy.y = temp;
                feature.lShoulder.maxy.z = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            if (temp > feature.lShoulder.maxz.z)
            {
                feature.lShoulder.maxz.x = sdata.Joints[JointID.ShoulderLeft].Position.X;
                feature.lShoulder.maxz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderLeft].Position.X;
            if (temp < feature.lShoulder.minx.x)
            {
                feature.lShoulder.minx.x = temp;
                feature.lShoulder.minx.y = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.minx.z = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Y;
            if (temp < feature.lShoulder.miny.y)
            {
                feature.lShoulder.miny.x = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.miny.y = temp;
                feature.lShoulder.miny.z = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderLeft].Position.Z;
            if (temp < feature.lShoulder.minz.z)
            {
                feature.lShoulder.minz.x = sdata.Joints[JointID.ShoulderLeft].Position.X;
                feature.lShoulder.minz.y = sdata.Joints[JointID.ShoulderLeft].Position.Y;
                feature.lShoulder.minz.z = temp;
            }


            //rShoulder
            temp = sdata.Joints[JointID.ShoulderRight].Position.X;
            if (temp > feature.rShoulder.maxx.x)
            {
                feature.rShoulder.maxx.x = temp;
                feature.rShoulder.maxx.y = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.maxx.z = sdata.Joints[JointID.ShoulderRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y;
            if (temp > feature.rShoulder.maxy.y)
            {
                feature.rShoulder.maxy.x = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.maxy.y = temp;
                feature.rShoulder.maxy.z = sdata.Joints[JointID.ShoulderRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z;
            if (temp > feature.rShoulder.maxz.z)
            {
                feature.rShoulder.maxz.x = sdata.Joints[JointID.ShoulderRight].Position.X;
                feature.rShoulder.maxz.y = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.maxz.z = temp;
            }

            //minx, miny, minz
            temp = sdata.Joints[JointID.ShoulderRight].Position.X;
            if (temp < feature.rShoulder.minx.x)
            {
                feature.rShoulder.minx.x = temp;
                feature.rShoulder.minx.y = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.minx.z = sdata.Joints[JointID.ShoulderRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Y;
            if (temp < feature.rShoulder.miny.y)
            {
                feature.rShoulder.miny.x = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.miny.y = temp;
                feature.rShoulder.miny.z = sdata.Joints[JointID.ShoulderRight].Position.Z;
            }
            temp = sdata.Joints[JointID.ShoulderRight].Position.Z;
            if (temp < feature.rShoulder.minz.z)
            {
                feature.rShoulder.minz.x = sdata.Joints[JointID.ShoulderRight].Position.X;
                feature.rShoulder.minz.y = sdata.Joints[JointID.ShoulderRight].Position.Y;
                feature.rShoulder.minz.z = temp;
            }

        
        }

        private void roundedness(){

        }

        private void jerkIndex(){
            this.feature.jerkIndex[0] = (-1 * this.totJI * 6) / (this.frame * this.feature.handSpeedMps);
        }

    }
}