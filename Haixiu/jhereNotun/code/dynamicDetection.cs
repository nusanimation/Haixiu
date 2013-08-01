using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using System.Windows;

namespace WpfApplication1.code
{
    class dynamicDetection : startFeatures
    {
        //private _feature feat;
        //private SkeletonData skeleGrow;
        double[] featureSet;
        private recognizer recog;
        private int iter;
        public int Iteration {
            get {
                return iter;
            }
        }

        public dynamicDetection(String s) 
        {
            //init motherfucking neural net
            this.recog = new recognizer(s);

            this.iter = 0;
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
            iter++;
            calculateFeature(s);
           
            if (iter == 30) {
                sendToANN(feature);
            }
            return iter;
        }
        
        private void sendToANN(_feature f) 
        {
            try
            {
                this.featureSet[0] = f.speedMps;
                this.featureSet[1] = f.lHandSpeedMps;
                //          this.featureSet[0] =  f.peakAccel[0] ; 
                this.featureSet[2] = f.peakDec[0] / 100;
                this.featureSet[3] = f.avgAccel[0];
                this.featureSet[4] = f.jerkIndex[0] / 120;
                this.featureSet[5] = f.rHandSpeedMps;
                this.featureSet[6] = f.peakAccel[1] / 100;
                this.featureSet[7] = f.peakDec[1] / 100;
                this.featureSet[8] = f.avgAccel[1];
                this.featureSet[9] = f.jerkIndex[1] / 120;
                this.featureSet[10] = f.lElbowSpeedMps;
                this.featureSet[11] = f.peakAccel[2] / 100;
                this.featureSet[12] = f.peakDec[2] / 100;
                this.featureSet[13] = f.avgAccel[2];
                this.featureSet[14] = f.jerkIndex[2] / 120;
                this.featureSet[15] = f.rElbowSpeedMps;
                this.featureSet[16] = f.peakAccel[3] / 100;
                this.featureSet[17] = f.peakDec[3] / 100;
                this.featureSet[18] = f.avgAccel[3];
                this.featureSet[19] = f.jerkIndex[3] / 120;
            }
            catch { 
                System.Windows.MessageBox.Show("Not Enough Memory.", "error", 
                    MessageBoxButton.OK, MessageBoxImage.Error); 
            }

            this.recog.recognizeEmotion(featureSet);

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
            dangQuality();
            //underBeltSkele();

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

    }
}
