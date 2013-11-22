using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge;
using AForge.Controls;
using AForge.Neuro.Learning;
using AForge.Neuro;
using System.Windows;
using System.Threading;
using System.IO;
using System.Collections;

//using System.Windows.Forms;

namespace WpfApplication1
{
    class NeuralNet : globalVars
    {
        private double learningRate = .1;
        private double momentum = 0.0;
        private double sigmoidAlphaValue = 2.0;
        private double learningErrorLimit = 0.1;
        private int iteration = 0;
        // private int sigmoidType = 0;
       // private bool saveStatisticsToFiles = false;
        private double[][] input = null;
        private double[][] output = null;
        private int numberOfOutputs;
        private int inputCount;
        
        
//        private learningWindow l;
        public NeuralNet(String[] data)
        {
//            initANN();
            numberOfOutputs = globalVars.outputCount;
            input = new double[data.Length][];
            output = new double[data.Length][];
            int op;
            needToStopLearning = false;
            int i=0,j=0;
            foreach (String line in data)
            {
                String[] values = line.Split(',');
                input[i] = new double[values.Length - numberOfOutputs];
                j = 0;
                op=0;
                inputCount = values.Length - numberOfOutputs;
                foreach (String asd in values)
                {
                   // System.Windows.MessageBox.Show(i + ", "+j, "meh.", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (j >= values.Length - numberOfOutputs)
                    {
                        if (output[i] == null)
                            output[i] = new double[numberOfOutputs];
                        output[i][op] = Convert.ToDouble(asd);
                        Console.Write(output[i][op] + "");
                        op++;
                    }
                    else
                    {
                        input[i][j] = Convert.ToDouble(asd);
                        Console.Write(input[i][j] + "");
                    }
                    j++;
                    Console.WriteLine("");
                }
//                System.Windows.MessageBox.Show(values.Length + " Lines", "meh.", MessageBoxButton.OK, MessageBoxImage.Information);
                i++;            
             }
//Tests
//          System.Windows.MessageBox.Show(data.Length + " Lines", "meh.", MessageBoxButton.OK, MessageBoxImage.Information);
 /*
            for(i=0; i<input.Length;i++)
                for(j=0; j<input[i].Length;j++)
                //Console.WriteLine(output[i][0]);
                System.Windows.MessageBox.Show(input[i][j].ToString() + " Lines", "meh.", MessageBoxButton.OK, MessageBoxImage.Information);
   */         initANN();
        }


        private void initANN()
        {


            System.Windows.MessageBox.Show("op " + numberOfOutputs + " inp " + inputCount, "detection error", MessageBoxButton.OK, MessageBoxImage.Error);

            ActivationNetwork network = new ActivationNetwork((IActivationFunction)new SigmoidFunction(sigmoidAlphaValue), this.inputCount, globalVars.hiddenCount, numberOfOutputs);

            ActivationNetwork network1 = new ActivationNetwork((IActivationFunction)new BipolarSigmoidFunction(sigmoidAlphaValue), this.inputCount, globalVars.hiddenCount, numberOfOutputs);

            BackPropagationLearning teacher ;
            if (globalVars.typeOfLearning == 1)
            {
               
                teacher = new BackPropagationLearning(network1);
            }
            else if (globalVars.typeOfLearning == 2)
            {
                
                teacher = new BackPropagationLearning(network);
            }
            else
            {
                System.Windows.MessageBox.Show("else", "detection error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
                //teacher = new BackPropagationLearning(network);
            }
//            teacher = new BackPropagationLearning(network1);
    
            // set learning rate and momentum
            teacher.LearningRate = this.learningRate;
            teacher.Momentum = this.momentum;

            // iterations
            iteration = 1;

            // statistic files
            StreamWriter errorsFile = null;

            try
            {
                String desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                // check if we need to save statistics to files
                // open files
                errorsFile = File.CreateText(desktopPath+@"\errors.csv");


                ArrayList errorsList = new ArrayList();

                // loop
                while (!needToStopLearning)
                {
                    // run epoch of learning procedure
                    double error = teacher.RunEpoch(input, output);
                   // errorsList.Add(error);

                    //took me a fucking day for this. http://www.albahari.com/threading/part2.aspx
                    Action action = () => { globalVars.error.Content = error; globalVars.annIter.Content = (iteration ) ;};
                    System.Windows.Application.Current.Dispatcher.Invoke(action);
                    //l.l1.TextChanged += new System.EventHandler(l.textBox1_TextChanged);
//                    learningWindow.l1.Refresh();
  //                  learningWindow.l1.Invalidate();
                    
                    // save current error
                    if (errorsFile != null)
                    {
                    //    errorsFile.WriteLine(error);
                    }

                    iteration++;

                    // check if we need to stop
                    if (error <= learningErrorLimit)
                        break;
                }
/*
                double[,] errors = new double[errorsList.Count, 2];

                for (int i = 0, n = errorsList.Count; i < n; i++)
                {
                    errors[i, 0] = i;
                    errors[i, 1] = (double)errorsList[i];

                }
                */
                //errorChart.RangeX = new DoubleRange(0, errorsList.Count - 1);
                //errorChart.UpdateDataSeries("error", errors);

            }

            catch
            {
                Console.WriteLine("oopsie! bad filename");
            }
            finally
            {
                // close files
                if (errorsFile != null)
                    errorsFile.Close();
            }




            Action action1 = () => { Learner.savedNet = network; globalVars.saveANNbutn.IsEnabled = true; };
            System.Windows.Application.Current.Dispatcher.Invoke(action1);

        }

    }


  /*  class learningWindow : Window
    {
        public static System.Windows.Forms.Label l1;
        public Form f;
        public learningWindow() 
        {
            this.f = new Form();
            l1 = new Label();
            l1.Text = "Hi";
            this.f.Controls.Add(l1);
            this.f.ShowDialog();
           
        }

        public void close()
        {
            this.f.Close();
        }

        public void textBox1_TextChanged(object sender, EventArgs e)
        {
            l1.Text = String.Format("Whatever default text there is {0}", l1.Text);
        }
    }

    */

    public class Learner
    {
        private String filename;
        //private System.IO.StreamReader file;
        private String[] data;
        public static ActivationNetwork savedNet = null;

        public static void saveANNtoFile(String filename = "savedNeuralNet.dat") 
        {
            if (savedNet != null)
                savedNet.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +@"\"+ filename);
        
        }

        public Learner(String str)
        {
            this.filename = str;
            if (readFile() == true)
            {
                globalVars.ANNthread = null;
                globalVars.ANNthread = new Thread(new ThreadStart(startANN));
                globalVars.ANNthread.SetApartmentState(ApartmentState.STA);

                try
                {
                    globalVars.ANNthread.Start();
                }
                catch
                {
                    //Console.WriteLine("oopsie! no threadding");
                    System.Windows.MessageBox.Show("oopsie! no threadding", "Threading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }
        private bool readFile()
        {
            try
            {
               // this.file = new System.IO.StreamReader(this.filename);
               this.data= System.IO.File.ReadAllLines(this.filename);
            }
            catch
            {
                //error
                System.Windows.MessageBox.Show("Couldn't load data file. I'm soO sorry.", "File error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
        private void startANN()
        {
            //learningWindow l = new learningWindow();
            NeuralNet ann = new NeuralNet(data);

        }

    }
    
    
    public class recognizer
    {
        public Network net;
        public recognizer(String n) { 
           //Set ANN
            //_feature f;
            net = Network.Load(n);
            //recognizeFeature();
            
        }
         

        //this now has no fucking use.
        public void recognizeFeature()//_feature f)
        {

            //test temporary input. should output -1
            /*                        
double[] in1 = new double[7] { 0.083761367,0.481607059,0.541926598,0.009443276,0.010626012,0.008413462,0.009360261};
            double[] in2 = new double[7] { 0.063041013,0.407123557,0.381803791,0.006263439,0.005873904,0.005762705,0.005383687 };
            double[] in3 = new double[7] { 0.059386202,0.321555592,0.350544802,0.010372761,0.011307897,0.010550536,0.010332937 };
            */
            double[] in1 = new double[3] { .10, .26, .35 };
            double[] in2 = new double[3] { .71, .85, .92 };
            double[] in3 = new double[3] { .32, .51, .61 };
/*
            double[] in1 = new double[20] { 0.063969761, 0.494024976, -0.130177477, 0.066127301, 0.418330858, 0.35005156, 0.251327007, -0.191292791, 0.04685586, 0.438744597, 0.36954461, 0.160890906, -0.160890906, 0.049465086, 0.446157068, 0.325770039, 0.298227785, -0.298227785, 0.043605677, 0.476020757 };
            double[] in2 = new double[20] { 0.044093227, 0.326825113, -0.149551929, 0.041363803, 0.418807853, 0.277612306, 0.068065802, -0.079305569, 0.035135307, 0.318992807, 0.44454413, 0.126398741, -0.110895964, 0.056262617, 0.437231355, 0.238296542, 0.064586405, -0.047161227, 0.030159406, 0.275350676 };
            double[] in3 = new double[20] { 0.074066872, 0.815675799, -0.204315886, 0.188149218, 0.693660646, 0.722097503, 0.385617688, -0.235079046, 0.166563824, 0.559139432, 0.521563365, 0.07394995, -0.076007914, 0.120307283, 0.249523014, 0.404770909, 0.073427138, -0.108928533, 0.093367156, 0.387029525 };
            */

            double[] out1 = new double[2];
            out1 = net.Compute(in1);
            if (net.Output.Length == 2)
            {

                System.Windows.MessageBox.Show("output: (" + Math.Round(out1[0]) + "," + Math.Round(out1[1]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                out1 = net.Compute(in2);
                System.Windows.MessageBox.Show("output: (" + Math.Round(out1[0]) + "," + Math.Round(out1[1]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                out1 = net.Compute(in3);
                System.Windows.MessageBox.Show("output: (" + Math.Round(out1[0]) + "," + Math.Round(out1[1]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("output: (" +  (out1[0]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                out1 = net.Compute(in2);
                System.Windows.MessageBox.Show("output: (" +  (out1[0]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                out1 = net.Compute(in3);
                System.Windows.MessageBox.Show("output: (" +  (out1[0]) + ")", "Output for test .1,.5", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        public double[] recognizeEmotion(double[] featureVals) 
        {
            double[] out1 = new double[1];
            out1 = net.Compute(featureVals);

            return out1;
        }

    }

}


