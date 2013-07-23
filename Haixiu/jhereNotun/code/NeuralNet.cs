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

        private int inputCount;


//        private learningWindow l;
        public NeuralNet(String[] data)
        {
//            initANN();
            input = new double[data.Length][];
            output = new double[data.Length][];
            int op;
            needToStopLearning = false;
            int i=0,j=0;
            foreach (String line in data)
            {
                String[] values = line.Split(',');
                input[i] = new double[values.Length-6];
                j = 0;
                op=0;
                foreach (String asd in values)
                {
                   // System.Windows.MessageBox.Show(i + ", "+j, "meh.", MessageBoxButton.OK, MessageBoxImage.Information);
                    inputCount = values.Length-6;
                    if (j >= values.Length - 6)
                    {
                        if (output[i] == null)
                            output[i] = new double[6];
                        output[i][op] = Convert.ToDouble(asd);
                        op++;
                    }
                    else
                        input[i][j] = Convert.ToDouble(asd)/3;

                    j++;
                    
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
            
            ActivationNetwork network = new ActivationNetwork((IActivationFunction)new SigmoidFunction(sigmoidAlphaValue), this.inputCount, 6, 6);
            BackPropagationLearning teacher = new BackPropagationLearning(network);
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
                    Console.WriteLine("oopsie! no threadding");
                    System.Windows.MessageBox.Show("oopsie! no threadding", "Threading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }
        private bool readFile()
        {
            try
            {
               // this.file = new System.IO.StreamReader(this.filename);
               data= System.IO.File.ReadAllLines(this.filename);
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
        Network net;
        public recognizer(String n) { 
           //Set ANN
            _feature f;
            net = Network.Load(n);
            recognizeFeature();
            
        }

        private void recognizeFeature()//_feature f)
        {
            double[] in1 = new double[4] {0,0,.1,.5};
            double[] out1 = new double[6];
            out1 = net.Compute(in1);
            System.Windows.MessageBox.Show("output: (" + Math.Round(out1[0]) + "-" + 
                Math.Round(out1[1]) + "-" + Math.Round(out1[2]) + "), (" +
                Math.Round(out1[3]) + "-" + Math.Round(out1[4]) + "-" +
                Math.Round(out1[5])+")",
                "Output for test .1,.5", MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

}


