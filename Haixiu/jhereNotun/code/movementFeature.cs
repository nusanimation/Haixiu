using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

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
            public double speed, peakAcc, Acc, jerk;
            public double spikePerSec;
        }
        public struct mFeature
        {
            public features[] node;
            //AnkleLeft, AnkleRight, ElbowLeft, ElbowRight, FootLeft, FootRight, HandLeft, HandRight, Head, HipCenter, HipLeft, HipRight, KneeLeft, KneeRight, ShoulderCenter, ShoulderLeft, ShoulderRight, Spine, WristLeft, WristRight;
        }

    }

    class movementFeature : mFeatureType
    {
        private double baseX, baseY, baseZ=0;

        features[] feature;
        private coOrd[] prevPos;
        private double[] prevSpeed, prevAcc;

        private int frame, interval;
        private double lowerThreshold, upperThreshold, surgeThreshold;

        public  movementFeature() 
        {
            this.interval = 10;
            initFeature();
        }

        private void initFeature()
        { 
            lowerThreshold = 0.005;
            upperThreshold = 1;
            surgeThreshold = 10;

            feature = new features[20];
            prevPos = new coOrd[20];
            prevSpeed = new double[20];
            prevAcc = new double[20];

            for (int i = 0; i < 20; i++)
            {
                feature[i].pos.maxX.x = -100;
                feature[i].pos.maxY.y = -100;
                feature[i].pos.maxZ.z = -100;
                feature[i].pos.minX.x = 100;
                feature[i].pos.minY.y = 100;
                feature[i].pos.minZ.z = 100;

                feature[i].spikePerSec = 0;

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
            foreach (JointID id in globalVars.jid)
            {
                calculateNodeFeatures(s.Joints[id],i);
                i++;
            }

        }

        private void calculateNodeFeatures(Joint j, int i)
        {
            position(j,i);
            speedAccJerk(j,i);

        }
        
        private void position(Joint j,int i){
                
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

            double dist=0, speed = 0;
            
            if (this.prevPos[0].x == -100)
            {
                // System.Windows.MessageBox.Show("ping.", "featureset error",  MessageBoxButton.OK, MessageBoxImage.Error); 
                feature[i].speed = 0;
                this.prevPos[0].x = j.Position.X;
                this.prevPos[0].y = j.Position.Y;
                this.prevPos[0].z = j.Position.Z;
            }
            else if (this.frame % interval == 0 && this.prevPos[0].x != -100)
            {

                if (((this.prevPos[i].x - j.Position.X) >= lowerThreshold || (this.prevPos[i].y - j.Position.Y) >= lowerThreshold || (this.prevPos[i].z - j.Position.Z) >= lowerThreshold)  && ((this.prevPos[i].x - j.Position.X) <= upperThreshold || (this.prevPos[i].y - j.Position.Y) <= upperThreshold || (this.prevPos[i].z - j.Position.Z) <= upperThreshold))
                {
                    dist =  Math.Sqrt(Math.Pow((this.prevPos[i].x - j.Position.X), 2) + Math.Pow((this.prevPos[i].y - j.Position.Y), 2) + Math.Pow((this.prevPos[i].z - j.Position.Z), 2));

                    this.prevPos[i].x = j.Position.X;
                    this.prevPos[i].y = j.Position.Y;
                    this.prevPos[i].z = j.Position.Z;
                }
                feature[i].speed += dist;
            }
          speed = dist /(interval/30);

          double speedSurge = (Math.Abs(speed - prevSpeed[i])/prevSpeed[i]) * 100 ;
          if (speedSurge >= surgeThreshold)
            {
                //calculate acceleration and jerk
                feature[i].Acc += (speed - prevSpeed[i]) / (interval / 30);
                feature[i].spikePerSec++;
            }
        }

        public features getFeatures(int index)
        {
           

            if (index < 0 || index > 20)
            {
                Console.WriteLine("give proper index for getFeatures. -1 means all 0-19 means individual nodes");
                index = 0;
            }
            feature[index].speed /= (this.frame / 30);
            feature[index].Acc /= feature[index].spikePerSec;
            feature[index].spikePerSec /= (this.frame / 30);
            
            return feature[index];

        }

        public features[] getFeatures()
        {
            for (int i = 0; i < 20; i++)
            {
                feature[i].speed /= (this.frame / 30);
                feature[i].Acc /= feature[i].spikePerSec;
                feature[i].spikePerSec /= (this.frame / 30);
            }

            return feature;
        }

    }


}
