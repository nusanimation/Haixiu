using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using System.Windows;
using System.IO;

namespace WpfApplication1
{
    public class dynamicDetection : startFeatures
    {
        //private _feature feat;
        //private SkeletonData skeleGrow;
        private fileWriter file = new fileWriter(null, "checking.csv");
        private List<double[]> featureList;     //  protected double[] featureSet;
        private recognizer recog;
        private int iter;
        public int Iteration
        {
            get
            {
                return iter;
            }
        }
        private bool slidingWindow = true;
        public bool SlidingWindowOn
        {
            get
            {
                return slidingWindow;
            }
            set
            {
                slidingWindow = value;
            }
        }

        private int interval = 30;
        public int updateInterval
        {
            get
            {
                return interval/30;
            }
            set
            {
                interval = 30 * value;
            }
                
        }


        public dynamicDetection(String s) 
        {
            
            this.recog = new recognizer(s);
            //this.file = new System.IO.StreamWriter("C:\\Users\\workshop\\Desktop\\11122.csv");


            this.iter = 0;
            this.frame = 0;
            this.feature = new _feature(0.0);

            //this.featureSet = new double[20];

            this.wDist = new double[4];
            this.wDistLeg = new double[4];
            this.prevAccel = new double[4];
            this.prevAccelLeg = new double[4];

            this.prevSpeed = new double[4];
            this.prevSpeedLeg = new double[4];
            this.totJI = new double[4];

            this.wprevLeg = new _qbit[4];

            this.wprev = new _qbit[4];

            this.featureList = new List<double[]>();


            refreshVars();
        }

        public int detect(SkeletonData s) {
           iter++; //frame++;
           addFeatures(s);
           if (slidingWindow == true)
           {
               if (iter % 60 == 0)
               {
                   makeFeatureSet(feature);
                   this.featureList.Add(featureSet);
                   double[] finalFeature = new double [7];
                   for (int i = 0; i < featureSet.Length; i++)
                   {
                       foreach (double[] d in this.featureList)
                       {
                           finalFeature[i] += d[i];
                           //Console.Write(d[i]+", ");
                       }
                       finalFeature[i] /= featureList.Count;
                       Console.WriteLine(finalFeature[i]);
                   }
                   //Console.WriteLine(featureList.Count);
                   if (featureList.Count==interval/30)
                   {
                       //double[] d1 = featureList[0];
                       //double[] d2 = featureList[1];
                       //Console.WriteLine(d1[0]+", "+d2[0]);
                       featureList.RemoveAt(0);                
                   }
                   sendToANN(finalFeature);
                }
             }

           else
           {

               if (iter % interval == 0)
               {
                   makeFeatureSet(feature);
                   sendToANN(featureSet);
               }
           }

            return iter;
        }

        private void makeFeatureSet(_feature f){
                      
            speed();
            dangSpeed();
            dangQuality();
            try
            {
                //this.featureSet[0] = f.speedMps;
                //this.featureSet[1] = f.lHandSpeedMps;
                ////          this.featureSet[0] =  f.peakAccel[0] ; 
                //this.featureSet[2] = f.peakDec[0] / 100;
                //this.featureSet[3] = f.avgAccel[0];
                //this.featureSet[4] = f.jerkIndex[0] / 120;
                //this.featureSet[5] = f.rHandSpeedMps;
                //this.featureSet[6] = f.peakAccel[1] / 100;
                //this.featureSet[7] = f.peakDec[1] / 100;
                //this.featureSet[8] = f.avgAccel[1];
                //this.featureSet[9] = f.jerkIndex[1] / 120;
                //this.featureSet[10] = f.lElbowSpeedMps;
                //this.featureSet[11] = f.peakAccel[2] / 100;
                //this.featureSet[12] = f.peakDec[2] / 100;
                //this.featureSet[13] = f.avgAccel[2];
                //this.featureSet[14] = f.jerkIndex[2] / 120;
                //this.featureSet[15] = f.rElbowSpeedMps;
                //this.featureSet[16] = f.peakAccel[3] / 100;
                //this.featureSet[17] = f.peakDec[3] / 100;
                //this.featureSet[18] = f.avgAccel[3];
                //this.featureSet[19] = f.jerkIndex[3] / 120;

                this.featureSet = new double[7];

                this.featureSet[0] = this.feature.speedMps/2;
                this.featureSet[1] = this.feature.lHandSpeedMps/2;
                this.featureSet[2] = this.feature.rHandSpeedMps/2;
                this.featureSet[3] = this.feature.avgAccel[0]/2;
                //            this.featureSet[4] = this.feature.jerkIndex[0] / 120/2;
                this.featureSet[4] = this.feature.avgAccel[1]/2;
                //            this.featureSet[9] = this.feature.jerkIndex[1] / 120/2;
                this.featureSet[5] = this.feature.avgAccel[2]/2;
                //            this.featureSet[14] = jerkIndex[2] / 120/2;
                this.featureSet[6] = this.feature.avgAccel[3]/2;
                //            this.featureSet[19] = this.feature.f.jerkIndex[3] / 120/2;
                for(int i=0;interval<7;interval++)
                    Console.Write(this.featureSet[i]+"--");
                Console.WriteLine("");
                //double[] dummyset = new double[12];
                //for (int i = 0; i < 7; i++)
                //    dummyset[i] = featureSet[i];
                //dummyset[20] = wDist[0];
                //dummyset[21] = wDist[1];
                //dummyset[22] = wDist[2];
                //dummyset[23] = wDist[3];
                //dummyset[24] = frame;
                //file.WritefeatureSet(dummyset);


            }

            catch { 
                System.Windows.MessageBox.Show("Not Enough Memory or whatever.", "featureset error", 
                    MessageBoxButton.OK, MessageBoxImage.Error); 
            }
        }

        private void sendToANN(double[] feat)
        {

            try
            {

                double[] output;// = new double[1];
                double val = 0;

                output = this.recog.recognizeEmotion(feat);
              
  
                /*
                double[] dummyset = new double[9];
                for (int i = 0; i < 7; i++)
                    dummyset[i] = feat[i];
                dummyset[7] = output[0];
//                dummyset[8] = output[1];

                this.file.WritefeatureSet(dummyset);
                */
/*
                if (Math.Round(output[0]) == 1 && Math.Round(output[1]) == 1)
                    val = 100;
                else if (Math.Round(output[0]) == 1 && Math.Round(output[1]) == 0)
                    val = 75;
                else if (Math.Round(output[0]) == 0 && Math.Round(output[1]) == 1)
                    val = 50;
                else if (Math.Round(output[0]) == 0 && Math.Round(output[1]) == 0)
                    val = 25;
                
                else */
                if (output[0] == double.NaN || output[0] == double.NegativeInfinity || output[0] == double.PositiveInfinity)
                    val = -1.0;
                else
                    val = (output[0] + 1) * 50;
                                

                globalVars.AnnOutput.Content = val + "%";
                if (globalVars.chart != null)
                {

                    globalVars.chart.update(val);
                }

                //preparing for he next iter of detection
                refreshVars();
                feature.initToZero();
            }
            catch
            {
                System.Windows.MessageBox.Show("Detection Module failed for some reason",
                    "detection error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
         }


  //      public void calculateFeature(SkeletonData s)
  //      {
    //        sdata = s;
    //        if (baseZ == 0)
    //        {
    //            this.baseX = this.spineX = s.Joints[JointID.Spine].Position.X;
    //            this.baseY = this.spineY = s.Joints[JointID.Spine].Position.Y;
    //            this.baseZ = this.spineZ = s.Joints[JointID.Spine].Position.Z;
    //        }

    ////here the positions are being normalised wrt spine position
    //        else
    //        {
    //            this.baseX = sdata.Joints[JointID.Head].Position.X + spineX;
    //            this.baseY = sdata.Joints[JointID.Head].Position.Y + spineY;
    //            this.baseZ = sdata.Joints[JointID.Head].Position.Y + spineZ;
    //        }

    //        stableSkele();
    //        danglingSkele();

    //        //underBeltSkele();

    //    }

        public void stopDetection()
        {

            if (this.file != null)
                file.closeFile();
        }

        public void test()
        {
            this.recog.recognizeFeature();
        }
        /* We have too many features like peak accel, spee etc that is a deciding factor .. so fo rnow, update is personcond. 30 frames.
        private void stableSkele() 
        {
            //because this is giving distance covered 5 times in every second. to Meter persond is *5
            this.feature.speedMps = updateSpine()*5;
            headAndShoulders();
        }

        private void danglingSkele()
        {
            dangPos();
            double[][] op = pollDangDistance();
        }
         * */
    //    ~dynamicDetection() {
    //        file.Close();

    //    }
    }
}
