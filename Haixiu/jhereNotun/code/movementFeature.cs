using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Controls;
using System.Threading;


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

            feature = new features[20];
            currFeature = new features[20];
            prevFeature = new features[20];
            prevPos = new coOrd[20];
            prevSpeed = new double[20];
            prevAcc = new double[20];

            frame = 0;

            for (int i = 0; i < 7; i++)
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
            foreach (JointID id in globalVars.jid)
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
            if (speedSurge >= surgeThreshold)
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
                    feature[i].Dec += tempAcc;
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
            for (int i = 0; i < 20; i++)
            {
                feature[i].speed /= timeSlice;
                feature[i].Acc /= feature[i].spikePerSecAcc;
                feature[i].Dec /= feature[i].spikePerSecDcc;
                feature[i].spikePerSecAcc /= timeSlice;
                feature[i].spikePerSecDcc /= timeSlice;
            }

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
            foreach(JointID id in jid)
            {
                /*for calcultion efficiency, we're adding 10 to the posiiton coord and dividing by 20*/
               pf[i] =  (s.Joints[id].Position.X - x +10)/20;               
               pf[i+1] =  (s.Joints[id].Position.Y - y+10)/20;               
               pf[i+2] =  (s.Joints[id].Position.Z - z+10)/20;
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

    public class featureExtractor : mFeatureType
    {
        public int frames;
        private int framedelay, getfeaturedelay;
        private movementFeature mfeat;
        private posFeature posfeat;

        features dispFeat;

        double lowerThreshold = 0.005;
        double upperThreshold = 1;
        double surgeThreshold = 10;

        resultViz chart;

        public FeatQueue posQ, mQ;
        public Thread thrd;

        public featureExtractor(double updateFeatureDelay, double getFeatureDelay, double lowerThreshold,
                                double upperThreshold,
                                double surgeThreshold, resultViz chart)
        {

            this.frames = 0;
            this.chart = chart;

            this.framedelay = (int)Math.Round(Math.Abs(updateFeatureDelay) * 30);
            this.getfeaturedelay = (int)Math.Round(Math.Abs(getFeatureDelay) * 30);

            this.lowerThreshold = lowerThreshold;
            this.upperThreshold = upperThreshold;
            this.surgeThreshold = surgeThreshold;

            mfeat = new movementFeature(lowerThreshold, upperThreshold, surgeThreshold, updateFeatureDelay, this.getfeaturedelay);
            posfeat = new posFeature();

            posQ = new FeatQueue();
            mQ = new FeatQueue();

            //this next thread is seeking the posFeature queue and trying to print posfeatures as it arrives.

            thrd = new Thread(() =>
            {
                double[] pData;
                while (globalVars.chartRighthand == true)
                {
                    pData = posQ.Dequeue();
                    if (pData != null)
                    {
                        //for (int i = 0; i < pData.Length; i++)
                        //{
                        //    Console.Write(pData[i] + ",");
                        //}
                        //Console.Write("\n");

                    }
                }
            }
            );
            thrd.Name = "pos feature writer thread";
            thrd.IsBackground = true;
            
            //thrd.Start();

        }

        public void saveFeatures()
        {

            savemFeat();
            saveposFeat();

        }

        private void savemFeat()
        {
            double[] pData;
            fileWriter f = new fileWriter(null, "newMovementFeature.csv");

            do
            {
                pData = mQ.Dequeue();

                if (pData != null)
                {
                    Console.Write(pData + ",");
                    f.WritefeatureSet(pData);
                }
            } while ((pData != null));

            //Console.Write("\n");
            f.closeFile();
        }

        private void saveposFeat()
        {
            double[] pData;
            fileWriter f = new fileWriter(null, "newPosfeature.csv");

            do
            {
                pData = posQ.Dequeue();

                if (pData != null)
                {
                    Console.Write(pData + ",");
                    f.WritefeatureSet(pData);
                }
            } while ((pData != null));

            //Console.Write("\n");
            f.closeFile();
        }

        public void getFeatures(SkeletonData s, Label l)
        {
            this.frames++;

            if (this.frames % (this.framedelay) == 0)
            {
                mfeat.calculateAllFeatures(s, this.frames);
            }
            if (this.frames % (this.getfeaturedelay) == 0)
            {
                this.dispFeat = mfeat.getFeatures(7, 2);
                double[] data = featureToArray(this.dispFeat);
                if(globalVars.chartRighthand==true)
                    this.chart.update(data);
                l.Content = this.dispFeat.speed;
                this.frames = 0;

                if (globalVars.logFeatures == true)
                {
                    mQ.Enqueue(data);
                }

            }

            if (globalVars.logFeatures == true && this.frames % 30 == 0) // 4 times per second
            {
                //posFeature.pDist pfeat;
                //pfeat = posfeat.getPosFeature(s);
                double[] posData = posfeat.getPosFeature(s);
                posQ.Enqueue(posData);

            }

        }

        public double[][] getRawDataStream(SkeletonData s)
        {
            /*calling this function means we only need detection and nothing else.*/
            globalVars.logFeatures = false;
            globalVars.chartRighthand = false;


            this.frames++;
            double[] data=null, posData = null;

            if (this.frames % (this.framedelay) == 0)
            {
                mfeat.calculateAllFeatures(s, this.frames);
            }
            if (this.frames % this.getfeaturedelay == 0)
            {
                this.dispFeat = mfeat.getFeatures(7, 2);
                data = featureToArray(this.dispFeat);
                this.frames = 0;
                
                //posFeature.pDist pfeat;
                //pfeat = posfeat.getPosFeature(s);
                posData = posfeat.getPosFeature(s);
                //posQ.Enqueue(posData);

            }
            double [][] r = new double[][] { data, posData };
            return (r);
        }

        //public double[] featureToArray(posFeature.pDist feature)
        //{
        //    double[] d = new double[7];
        //    d[0] = feature.lsh;
        //    d[1] = feature.rsh;
        //    d[2] = feature.lel;
        //    d[3] = feature.rel;
        //    d[4] = feature.lh;
        //    d[5] = feature.rh;
        //    d[6] = feature.head;

        //    return d;

        //}
        public double[] featureToArray(features feature)
        {
            double[] d = new double[5];
            d[0] = feature.speed;
            d[1] = feature.Acc/10;
            d[2] = feature.Dec/10;
            d[3] = 0;
            d[4] = 0;

            return d;
        }

    }
}
