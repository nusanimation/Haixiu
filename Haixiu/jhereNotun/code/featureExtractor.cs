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
    public class featureExtractor : mFeatureType
    {
        public int frames;
        private int framedelay, getfeaturedelay;
        private movementFeature mfeat;
        private posFeature posfeat;

        features dispFeat;
        features[] completeMovementFeatures;
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
//              this.dispFeat = mfeat.getFeatures(7, this.getfeaturedelay);
//              double[] data = featureToArray(this.dispFeat);

                this.completeMovementFeatures = mfeat.getFeatures(this.getfeaturedelay/30);
                double[] data = featureToArray(this.completeMovementFeatures);

//                if (globalVars.chartRighthand == true)
//                    this.chart.update(data);
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
            double[] data = null, posData = null;

            if (this.frames % (this.framedelay) == 0)
            {
                mfeat.calculateAllFeatures(s, this.frames);

                this.dispFeat = mfeat.getFeatures(7, 2);
                data = featureToArray(this.dispFeat);
                this.frames = 0;

                //posFeature.pDist pfeat;
                //pfeat = posfeat.getPosFeature(s);
                posData = posfeat.getPosFeature(s);
                //posQ.Enqueue(posData);

            }
            double[][] r = new double[2][];// { data, posData };
            r[0] = data;
            r[1] = posData;
            //if (data != null && posData != null) 
            //System.Windows.MessageBox.Show("Ans 0 " + r[0][0] + " ans 1 " + r[1][0], "chk", MessageBoxButton.OK, MessageBoxImage.Error);
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
            d[1] = feature.Acc / 10;
            d[2] = feature.Dec / 10;
            d[3] = 0;
            d[4] = 0;

            return d;
        }
        public double[] featureToArray(features[] feature)
        {
            double[] d = new double[4*feature.Length];
            int counter=0;
            for (int i = 0; i < feature.Length; i++)
            {
                d[counter++] = feature[i].speed;
                d[counter++] = feature[i].Acc;
                d[counter++] = feature[i].Dec;
                //d[counter++] = feature[i].jerk;
                d[counter++] = (feature[i].spikePerSecAcc + feature[i].spikePerSecDcc);

            }

            return d;
        }

    }
}