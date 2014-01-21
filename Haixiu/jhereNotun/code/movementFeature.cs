using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Controls;
using System.Threading;
using System.Windows;
using System.IO;

namespace WpfApplication1
{
    public abstract class mFeatureType
    {

        public struct coOrd
        {
            public double x, y, z;
        }
        public struct _pos
        {
            public coOrd minX, maxX;
            public coOrd minY, maxY;
            public coOrd minZ, maxZ;

        }
        public struct features
        {
            public _pos pos;
            public double speed, peakAcc, Acc, Dec, jerk;
            public double spikePerSecAcc, spikePerSecDcc;
        }
        public struct mFeature
        {
            public features[] node;
            //AnkleLeft, AnkleRight, ElbowLeft, ElbowRight, FootLeft, FootRight, HandLeft, HandRight, Head, HipCenter, HipLeft, HipRight, KneeLeft, KneeRight, ShoulderCenter, ShoulderLeft, ShoulderRight, Spine, WristLeft, WristRight;
        }

    }

    class movementFeature : mFeatureType
    {
        private double baseX, baseY, baseZ = 0;

        features[] feature, currFeature, prevFeature;
        private coOrd[] prevPos;
        private double[] prevSpeed, prevAcc;

        private int frame;//, interval;
        private double lowerThreshold, upperThreshold, surgeThreshold, interval;
        JointID[] joints = new JointID[11]{ JointID.ElbowLeft, JointID.ElbowRight, JointID.HandLeft, JointID.HandRight, JointID.Head, JointID.ShoulderCenter, JointID.ShoulderLeft, JointID.ShoulderRight,
 JointID.Spine, JointID.WristLeft, JointID.WristRight};
        public movementFeature()
        {
            this.interval = 0.2;
            lowerThreshold = 0.005;
            upperThreshold = 1;
            surgeThreshold = 10;

            initFeature();
        }
        public movementFeature(double lowerThreshold,
                                double upperThreshold,
                                double surgeThreshold, double updateInterval, int featureSaveInterval)
        {
            this.interval = updateInterval;
            this.lowerThreshold = lowerThreshold;
            this.upperThreshold = upperThreshold;
            this.surgeThreshold = surgeThreshold;

            initFeature();
        }


        private void initFeature()
        {

            feature = new features[11];
            currFeature = new features[11];
            prevFeature = new features[11];
            prevPos = new coOrd[11];
            prevSpeed = new double[11];
            prevAcc = new double[11];

            frame = 0;

            for (int i = 0; i < 11; i++)
            {
                feature[i].pos.maxX.x = -100;
                feature[i].pos.maxY.y = -100;
                feature[i].pos.maxZ.z = -100;
                feature[i].pos.minX.x = 100;
                feature[i].pos.minY.y = 100;
                feature[i].pos.minZ.z = 100;
                feature[i].speed = 0;
                feature[i].peakAcc = 0;
                feature[i].Acc = 0;
                feature[i].jerk = 0;
                feature[i].spikePerSecAcc = 0;
                feature[i].spikePerSecDcc = 0;

                currFeature[i].pos.maxX.x = -100;
                currFeature[i].pos.maxY.y = -100;
                currFeature[i].pos.maxZ.z = -100;
                currFeature[i].pos.minX.x = 100;
                currFeature[i].pos.minY.y = 100;
                currFeature[i].pos.minZ.z = 100;
                currFeature[i].speed = 0;
                currFeature[i].peakAcc = 0;
                currFeature[i].Acc = 0;
                currFeature[i].jerk = 0;
                currFeature[i].spikePerSecAcc = 0;
                currFeature[i].spikePerSecDcc = 0;


                prevFeature[i].pos.maxX.x = -100;
                prevFeature[i].pos.maxY.y = -100;
                prevFeature[i].pos.maxZ.z = -100;
                prevFeature[i].pos.minX.x = 100;
                prevFeature[i].pos.minY.y = 100;
                prevFeature[i].pos.minZ.z = 100;
                prevFeature[i].speed = 0;
                prevFeature[i].peakAcc = 0;
                prevFeature[i].Acc = 0;
                prevFeature[i].jerk = 0;
                prevFeature[i].spikePerSecAcc = 0;
                prevFeature[i].spikePerSecDcc = 0;

                prevPos[i].x = -100;
                prevPos[i].y = -100;
                prevPos[i].z = -100;

                prevSpeed[i] = 0;
                prevAcc[i] = 0;

            }

        }

        public void calculateAllFeatures(SkeletonData s, int frame)
        {


            this.frame = frame;
            //here the positions are being normalised wrt spine position
            if (baseZ == 0)
            {
                this.baseX = s.Joints[JointID.Spine].Position.X;
                this.baseY = s.Joints[JointID.Spine].Position.Y;
                this.baseZ = s.Joints[JointID.Spine].Position.Z;
            }

            int i = 0;

            foreach (JointID id in this.joints)
            {
                //if(i==7)
                calculateNodeFeatures(s.Joints[id], i);
                i++;
            }

        }

        private void calculateNodeFeatures(Joint j, int i)
        {
            position(j, i);
            speedAccJerk(j, i);

        }

        private void position(Joint j, int i)
        {

            double temp = j.Position.X - this.baseX;

            if (temp > feature[i].pos.maxX.x)
            {
                feature[i].pos.maxX.x = temp;
                feature[i].pos.maxX.y = j.Position.Y - this.baseY;
                feature[i].pos.maxX.z = j.Position.Z - this.baseZ;
            }
            temp = j.Position.Y - this.baseY;
            if (temp > feature[i].pos.maxY.y)
            {
                feature[i].pos.maxY.x = j.Position.X - this.baseX;
                feature[i].pos.maxY.y = temp;
                feature[i].pos.maxY.z = j.Position.Z - this.baseZ;
            }
            temp = j.Position.Z - this.baseZ;
            if (temp > feature[i].pos.maxZ.z)
            {
                feature[i].pos.maxZ.x = j.Position.X - this.baseX;
                feature[i].pos.maxZ.y = j.Position.Y - this.baseY;
                feature[i].pos.maxZ.z = temp;
            }

            //minX, minY, minZ
            temp = j.Position.X - this.baseX;
            if (temp < feature[i].pos.minX.x)
            {
                feature[i].pos.minX.x = temp;
                feature[i].pos.minX.y = j.Position.Y - this.baseY;
                feature[i].pos.minX.z = j.Position.Z - this.baseZ;
            }
            temp = j.Position.Y - this.baseY;
            if (temp < feature[i].pos.minY.y)
            {
                feature[i].pos.minY.x = j.Position.X - this.baseX;
                feature[i].pos.minY.y = temp;
                feature[i].pos.minY.z = j.Position.Z - this.baseZ;
            }
            temp = j.Position.Z - this.baseZ;
            if (temp < feature[i].pos.minZ.z)
            {
                feature[i].pos.minZ.x = j.Position.X - this.baseX;
                feature[i].pos.minZ.y = j.Position.Y - this.baseY;
                feature[i].pos.minZ.z = temp;
            }
        }

        private void speedAccJerk(Joint j, int i)
        {
            //            Console.Write(i);
            double dist = 0, speed = 0;
            if (this.prevPos[i].x == -100)
            {
                //Console.Write("\n"+i);
                // System.Windows.MessageBox.Show("ping.", "featureset error",  MessageBoxButton.OK, MessageBoxImage.Error); 
                feature[i].speed = 0;
                this.prevPos[i].x = j.Position.X;
                this.prevPos[i].y = j.Position.Y;
                this.prevPos[i].z = j.Position.Z;
            }
            else if (/*this.frame % interval == 0 &&*/ this.prevPos[i].x != -100)
            {

                //if (((this.prevPos[i].x - j.Position.X) >= lowerThreshold || (this.prevPos[i].y - j.Position.Y) >= lowerThreshold || (this.prevPos[i].z - j.Position.Z) >= lowerThreshold)  && ((this.prevPos[i].x - j.Position.X) <= upperThreshold || (this.prevPos[i].y - j.Position.Y) <= upperThreshold || (this.prevPos[i].z - j.Position.Z) <= upperThreshold))
                {
                    dist = Math.Sqrt(Math.Pow((this.prevPos[i].x - j.Position.X), 2) + Math.Pow((this.prevPos[i].y - j.Position.Y), 2) + Math.Pow((this.prevPos[i].z - j.Position.Z), 2));

                    this.prevPos[i].x = j.Position.X;
                    this.prevPos[i].y = j.Position.Y;
                    this.prevPos[i].z = j.Position.Z;
                }
                feature[i].speed += dist;
                //Console.WriteLine("*#*" + dist + " and frame " + this.frame);
            }
            
            /***Logic for Acclaration and Jerk***/

            speed = dist / interval;
            double speedSurge = (Math.Abs(speed - prevSpeed[i]) / prevSpeed[i]) * 100;
            //Console.WriteLine("speed: " + speed + "prevspeed: " + prevSpeed[i] + " surge: " + speedSurge);
            if (speedSurge >= this.surgeThreshold)
            {
                //calculate acceleration and jerk
                double tempAcc = (speed - prevSpeed[i]) / interval;
                if (tempAcc > 0)
                {
                    feature[i].Acc += tempAcc;
                    feature[i].spikePerSecAcc++;
                    //Console.WriteLine("acc: " + feature[i].Acc);
                }
                else
                {
                    feature[i].Dec += - tempAcc;
                    feature[i].spikePerSecDcc++;
              //      Console.WriteLine("dcc: " + feature[i].Dec);
                }
            }
            //Console.WriteLine("prevSpeed=," + prevSpeed[i] + ", Speed= ," + speed+ ", f= ," + feature[i].Acc+ ", -f= ," + feature[i].Dec );

            prevSpeed[i] = speed;
        }

        public features getFeatures(int index, double timeSlice)
        {

            if (index < 0 || index > 20)
            {
                Console.WriteLine("give proper index for getFeatures. -1 means all 0-19 means individual nodes");
                index = 0;
            }
            /*
            //Console.WriteLine("**" + feature[index].speed);
            feature[index].speed /= timeSlice;
            feature[index].Acc /= feature[index].spikePerSecAcc;
            feature[index].Dec /= feature[index].spikePerSecDcc;
            Console.WriteLine("\nv=" + feature[index].speed + " f= " + feature[index].Acc);

            feature[index].spikePerSecAcc /= timeSlice;
            feature[index].spikePerSecDcc /= timeSlice;

            features f = feature[index];
            initFeature();
            */
            currFeature[index].speed = feature[index].speed - prevFeature[index].speed;
            currFeature[index].Acc = feature[index].Acc - prevFeature[index].Acc;
            currFeature[index].Dec = feature[index].Dec - prevFeature[index].Dec;
            currFeature[index].spikePerSecAcc = feature[index].spikePerSecAcc - prevFeature[index].spikePerSecAcc;
            currFeature[index].spikePerSecDcc = feature[index].spikePerSecDcc - prevFeature[index].spikePerSecDcc;
            currFeature[index].jerk= feature[index].jerk- prevFeature[index].jerk;

            prevFeature[index] = feature[index];

            currFeature[index].speed /= timeSlice;
            currFeature[index].Acc /= currFeature[index].spikePerSecAcc;
            currFeature[index].Dec /= currFeature[index].spikePerSecDcc;
            //Console.WriteLine("\nv=" + currFeature[index].speed + " f= " + currFeature[index].Acc + " index= " + currFeature[index].spikePerSecAcc +  " -f= " + currFeature[index].Dec+" dcc= " + currFeature[index].spikePerSecDcc);

            currFeature[index].spikePerSecAcc /= timeSlice;
            currFeature[index].spikePerSecDcc /= timeSlice;

            return currFeature[index];

        }


        public features[] getFeatures(double timeSlice)
        {
            for (int i = 0; i < 11; i++)
            {
                feature[i].speed /= timeSlice;
                if (double.IsNaN(feature[i].speed)) feature[i].speed = 0;
                feature[i].Acc /= feature[i].spikePerSecAcc;
                if (double.IsNaN(feature[i].Acc)) feature[i].Acc = 0;
                feature[i].Dec /= feature[i].spikePerSecDcc;
                if (double.IsNaN(feature[i].Dec)) feature[i].Dec = 0;
                feature[i].spikePerSecAcc /= timeSlice;
                if (double.IsNaN(feature[i].spikePerSecAcc)) feature[i].spikePerSecAcc = 0;
                feature[i].spikePerSecDcc /= timeSlice;
                if (double.IsNaN(feature[i].spikePerSecDcc)) feature[i].spikePerSecDcc = 0;
            }

            //for (int index = 0; index < 11; index++)
            //{
            //    currFeature[index].speed = feature[index].speed - prevFeature[index].speed;
            //    currFeature[index].Acc = feature[index].Acc - prevFeature[index].Acc;
            //    currFeature[index].Dec = feature[index].Dec - prevFeature[index].Dec;
            //    currFeature[index].spikePerSecAcc = feature[index].spikePerSecAcc - prevFeature[index].spikePerSecAcc;
            //    currFeature[index].spikePerSecDcc = feature[index].spikePerSecDcc - prevFeature[index].spikePerSecDcc;
            //    currFeature[index].jerk = feature[index].jerk - prevFeature[index].jerk;

            //    prevFeature[index] = feature[index];

            //    currFeature[index].speed /= timeSlice;
            //    currFeature[index].Acc /= currFeature[index].spikePerSecAcc;
            //    currFeature[index].Dec /= currFeature[index].spikePerSecDcc;
            //    //Console.WriteLine("\nv=" + currFeature[index].speed + " f= " + currFeature[index].Acc + " index= " + currFeature[index].spikePerSecAcc +  " -f= " + currFeature[index].Dec+" dcc= " + currFeature[index].spikePerSecDcc);

            //    currFeature[index].spikePerSecAcc /= timeSlice;
            //    currFeature[index].spikePerSecDcc /= timeSlice;
            //}


//            features[] f = currFeature;
            features[] f = feature;
            initFeature();

            return f;
        }

    }


    public class posFeature
    {
        public struct pFeature
        {
            public coOrd lsh, rsh, lel, rel, lh, rh, head;

            //pDist distances;
            //pos angles;
        }
        public struct coOrd
        {
            public double x, y, z;
        }
        public struct pDist
        {
            //public coOrd lsh, rsh, lel, rel, lh, rh, head;

        }
        //public struct angle
        //{
        //    public double xy, z;
        //}
        //public struct pos
        //{
        //    public angle lsh, rsh, lel, rel, lh, rh, head;

        //}

        private double x, y, z;
        private pFeature pf;
        public posFeature()
        {
            x = 0; y = 0; z = 0;
            this.pf = new pFeature();

        }
        public double[] getPosFeature(SkeletonData s)
        {
            //pDist pf = getDistance(s);
            double[] pf = getPos(s);
            return pf;
        }
        private double[] getPos(SkeletonData s) {
            //pFeature pf = new pFeature();
            double[] pf = new double [21];
            int i = 0;
            x = s.Joints[JointID.Spine].Position.X;
            y = s.Joints[JointID.Spine].Position.Y;
            z = s.Joints[JointID.Spine].Position.Z;

            //pf.lh.x = s.Joints[JointID.HandLeft].Position.X - x;
            //pf.lh.y = s.Joints[JointID.HandLeft].Position.Y - y;
            //pf.lh.z = s.Joints[JointID.HandLeft].Position.Z - z;
            JointID[] jid = new JointID[7]{JointID.ElbowLeft, JointID.ElbowRight, JointID.WristLeft, JointID.WristRight, 
                                JointID.Head, JointID.ShoulderLeft, JointID.ShoulderRight};
            //Console.WriteLine(" \n");
            foreach (JointID id in jid)
            {
                /*for calcultion efficiency, we're adding 10 to the posiiton coord and dividing by 20*/
                pf[i] = (s.Joints[id].Position.X - x + 10) / 20;
                pf[i + 1] = (s.Joints[id].Position.Y - y + 10) / 20;
                pf[i + 2] = (s.Joints[id].Position.Z - z + 10) / 20;
                //Console.WriteLine(pf[i] + " " + pf[i + 1] + " " + pf[i + 2] + " ");
                i += 3;
            }   

            return pf;
        }

        private pDist getDistance(SkeletonData s)
        {

            pDist pf = new pDist();
            x = s.Joints[JointID.Spine].Position.X;
            y = s.Joints[JointID.Spine].Position.Y;
            z = s.Joints[JointID.Spine].Position.Z;

            /*
            //calculating distances
            pf.lsh = Math.Sqrt(Math.Pow((s.Joints[JointID.ShoulderLeft].Position.X - x), 2) + Math.Pow((s.Joints[JointID.ShoulderLeft].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.ShoulderLeft].Position.Z - z), 2));

            pf.rsh = Math.Sqrt(Math.Pow((s.Joints[JointID.ShoulderRight].Position.X - x), 2) + Math.Pow((s.Joints[JointID.ShoulderRight].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.ShoulderRight].Position.Z - z), 2));

            pf.lel = Math.Sqrt(Math.Pow((s.Joints[JointID.ElbowLeft].Position.X - x), 2) + Math.Pow((s.Joints[JointID.ElbowLeft].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.ElbowLeft].Position.Z - z), 2));

            pf.rel = Math.Sqrt(Math.Pow((s.Joints[JointID.ElbowRight].Position.X - x), 2) + Math.Pow((s.Joints[JointID.ElbowRight].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.ElbowRight].Position.Z - z), 2));

            pf.lh = Math.Sqrt(Math.Pow((s.Joints[JointID.HandLeft].Position.X - x), 2) + Math.Pow((s.Joints[JointID.HandLeft].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.HandLeft].Position.Z - z), 2));

            pf.rh = Math.Sqrt(Math.Pow((s.Joints[JointID.HandRight].Position.X - x), 2) + Math.Pow((s.Joints[JointID.HandRight].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.HandRight].Position.Z - z), 2));

            pf.head = Math.Sqrt(Math.Pow((s.Joints[JointID.Head].Position.X - x), 2) + Math.Pow((s.Joints[JointID.Head].Position.Y - y), 2) + Math.Pow((s.Joints[JointID.Head].Position.Z - z), 2));

            */
            return pf;
        }
        /*
      private pos getAngles(SkeletonData s)
      {
          pos position = new pos();
          angle a;

            
          a = calculatePositionAngles(s, JointID.HandLeft);
            
          return position;
   
      }
    private angle calculatePositionAngles(SkeletonData s, JointID id)
      {
          angle a = new angle();
          double cX, cY, cZ;
          double bX, bY, bZ;
          double aX, aY, aZ;
          double baseL, heightL, hypL;

          cX = s.Joints[JointID.Spine].Position.X;
          cY = s.Joints[JointID.Spine].Position.Y;
          cZ = s.Joints[JointID.Spine].Position.Z;

          bX = s.Joints[JointID.ShoulderRight].Position.X;
          bY = s.Joints[JointID.Spine].Position.Y;
          bZ = s.Joints[JointID.ShoulderRight].Position.Z;

          aX = s.Joints[id].Position.X;
          aY = s.Joints[id].Position.Y;
          aZ = s.Joints[id].Position.Z;

          baseL = Math.Sqrt(Math.Pow(cX - bX, 2) + Math.Pow(cY - bY, 2) + Math.Pow(cZ - bZ, 2));
          heightL = Math.Sqrt(Math.Pow(cX - bX, 2) + Math.Pow(cY - bY, 2) + Math.Pow(cZ - bZ, 2));
          hypL = Math.Sqrt(Math.Pow(aX - bX, 2) + Math.Pow(aY - bY, 2) + Math.Pow(aZ - bZ, 2));


          return a;

      }
*/
    }


    /*
     * *
     * *
     * *FeatureExtractor**
     * 
     */

  // AT FeatureExtractor.cs

}
