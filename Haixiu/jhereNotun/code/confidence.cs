using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WpfApplication1
{
    public class FeatQueue
    {
        /// <summary>Used as a lock target to ensure thread safety.</summary>
        //private readonly Locker _Locker = new Locker();

        private readonly System.Collections.Generic.Queue<double[]> _Queue = new System.Collections.Generic.Queue<double[]>();

        /// <summary></summary>
        public void Enqueue(double[] item)
        {
            lock (this)
            {
                _Queue.Enqueue(item);
            }
        }
        public double[] Dequeue()
        {
            double[] data;
             
            lock (this)
            {
                if (_Queue.Count > 0)
                {
                    data = _Queue.Dequeue();
                    return data;
                }
                else
                    return null;
            }
        }
    }

    class confidence
    {
        int emoCategory;
        double[] output;
        double emoThreshold, ConfidenceLowThreshold, ConfidenceHighThreshold;
        recognizer confiRecog;
        public confidence(String filename = "confidenceNet.dat") 
        {
            this.emoCategory = 10;
            this.emoThreshold = 0.1;
            this.ConfidenceLowThreshold = 0.4;
            this.ConfidenceHighThreshold = 0.6;
            this.confiRecog = new recognizer(filename);
        }

        public void evaluateConfidence(double[] data) 
        {
            output = this.confiRecog.recognizeEmotion(data);
            List<KeyValuePair<double, int>> lst = new List<KeyValuePair<double,int>>();

            //checking for multiple emo category:
            for (int i = 0; i < this.emoCategory; i++)
            {
                if (output[i] > this.emoThreshold)
                {
                    lst.Add(new KeyValuePair<double, int>(output[i], i));
                }
            }
            getConfidence(lst);

        }

        private double getConfidence(List<KeyValuePair<double, int>> lst) {
            KeyValuePair<double, double> ans;
            if (lst.Count == 1)
            {

            }
            else if (lst.Count == 0)
            {

            }
            else
            {
            }

            return .1; }

    }
}
