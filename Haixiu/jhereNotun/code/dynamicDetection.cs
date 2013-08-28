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
      //  protected double[] featureSet;
        private recognizer recog;
        private int iter;
        public int Iteration {
            get {
                return iter;
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

            this.featureSet = new double[20];

            this.wDist = new double[4];
            this.wDistLeg = new double[4];
            this.prevAccel = new double[4];
            this.prevAccelLeg = new double[4];

            this.prevSpeed = new double[4];
            this.prevSpeedLeg = new double[4];
            this.totJI = new double[4];

            this.wprevLeg = new _qbit[4];

            this.wprev = new _qbit[4];


            refreshVars();
        }

        public int detect(SkeletonData s) {
            iter++; frame++;
            calculateFeature(s);
           
            if (iter%interval == 0) {
                sendToANN(feature);
            }
            return iter;
        }
        
        private void sendToANN(_feature f) 
        {
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

                this.featureSet[0] = f.speedMps/2;
                this.featureSet[1] = f.lHandSpeedMps/2;
                this.featureSet[2] = f.rHandSpeedMps/2;
                this.featureSet[3] = f.avgAccel[0]/2;
                //            this.featureSet[4] = f.jerkIndex[0] / 120;
                this.featureSet[4] = f.avgAccel[1]/2;
                //            this.featureSet[9] = f.jerkIndex[1] / 120;
                this.featureSet[5] = f.avgAccel[2]/2;
                //            this.featureSet[14] = jerkIndex[2] / 120;
                this.featureSet[6] = f.avgAccel[3]/2;
                //            this.featureSet[19] = f.jerkIndex[3] / 120;

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

            try
            {
                
                double[] output;
                output = this.recog.recognizeEmotion(this.featureSet);
                double[] dummyset = new double[9];
                for (int i = 0; i < 7; i++)
                    dummyset[i] = this.featureSet[i];
                dummyset[7] = output[0];
                dummyset[8] = output[1];

                this.file.WritefeatureSet(dummyset);

                double val = 0;

                if (Math.Round(output[0]) == 1 && Math.Round(output[1]) == 1)
                    val = 100;
                else if (Math.Round(output[0]) == 1 && Math.Round(output[1]) == 0)
                    val = 75;
                else if (Math.Round(output[0]) == 0 && Math.Round(output[1]) == 1)
                    val = 50;
                else if (Math.Round(output[0]) == 0 && Math.Round(output[1]) == 0)
                    val = 25;

                else if (output[0] == double.NaN || output[0] == double.NegativeInfinity || output[0] == double.PositiveInfinity)
                    val = 0.0;

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


        public void calculateFeature(SkeletonData s)
        {
            sdata = s;
            if (baseZ == 0)
            {
                this.baseX = this.spineX = s.Joints[JointID.Spine].Position.X;
                this.baseY = this.spineY = s.Joints[JointID.Spine].Position.Y;
                this.baseZ = this.spineZ = s.Joints[JointID.Spine].Position.Z;
            }

    //here the positions are being normalised wrt spine position
            else
            {
                this.baseX = sdata.Joints[JointID.Head].Position.X + spineX;
                this.baseY = sdata.Joints[JointID.Head].Position.Y + spineY;
                this.baseZ = sdata.Joints[JointID.Head].Position.Y + spineZ;
            }

            stableSkele();
            danglingSkele();
            speed();
            dangSpeed();
            dangQuality();
            //underBeltSkele();

        }

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
