using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Collections.ObjectModel;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Xna.Framework;
using System.IO;

using System.Threading;

//using System.Windows.Forms;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        kinectApp app;
        int valueChanged = 0;
        private featureExtractor fExtract;

        private bool learnOn, recordOn, detectOn, settingsOn;

        public MainWindow()
        {

            InitializeComponent();
            // populateListbox();

            globalVars.resultChart = lineChart;
            globalVars.aseries = arousalpoints;
            globalVars.vseries = valencepoints;
            globalVars.Circumplex = vgaCanvas;

            int gpt;
            try { gpt = (int)Convert.ToInt32(graphPt.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); gpt = 30;
            }

            globalVars.chart = new resultViz(lineChart, gpt, 2);
            globalVars.chart2 = new resultViz(featureChart, gpt, 5);

            slider1.Value = (double)-3;
            slider1.Maximum = Camera.ElevationMaximum;
            slider1.Minimum = Camera.ElevationMinimum;

        }

        public void SaveCanvasToFile(String filename = "snap.bmp")
        {
            Canvas surface = canvas1;
            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            double w, h;
            w = surface.Width; h = surface.Height;
            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
            new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);

            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(filename, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }

            // Restore previously saved layout
            surface.LayoutTransform = transform;
            surface.Arrange(new Rect(387, 18, w, h));
        }

        private void populateListbox()
        {
            ObservableCollection<string> list1 = new ObservableCollection<string>();
            list1.Add("BipolarSigmoid");
            list1.Add("BinarySigmoid");
            comboBox1.ItemsSource = list1;
            comboBox1.SelectedItem = 1;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            learnOn = false;
            detectOn = true;
            recordOn = false;
            settingsOn = false;
            //label1.Content = "ahhaaaha";
            //labelFrame.Content = "frame";
            globalVars.screenH = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualHeight;
            globalVars.screenW = ((System.Windows.Controls.Panel)Application.Current.MainWindow.Content).ActualWidth;


            globalVars.a1 = labelFrame;
            //globalVars.a2 = wDistLabel;
            globalVars.error = AnnError;
            globalVars.annIter = iterCount;
            globalVars.saveANNbutn = saveANNbutn;
            //globalVars.jerkLabel = jerkLabel;
            globalVars.ValenceOutput = outputLabel;
            globalVars.ArousalOutput = label14;

            globalVars.typeOfLearning = 1;//= Convert.ToInt32(typeofann.Text.ToString());
            try { globalVars.outputCount = Convert.ToInt32(outputCount.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.outputCount = 1;
            }

            try { globalVars.hiddenCount = Convert.ToInt32(hiddenCount.Text.ToString()); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.hiddenCount = 4;
            }


            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;

            double a, b, c, d, ef;

            try { a = Math.Abs(Convert.ToDouble(lThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); a = 0.005;
            }

            try { b = Math.Abs(Convert.ToDouble(uThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); b = 1;
            }

            if (a >= b)
            {
                System.Windows.MessageBox.Show("Lower Threshold can not be greater or eqal to Upper Threshold. Reverting back to default Values of 0.005 and 1", "you moron.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                lThres.Text = "0.005";
                uThres.Text = "1";
                a = 0.005;
                b = 1;
            }

            try { c = Math.Abs(Convert.ToDouble(surgeThres.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); c = 10;
            }

            try { d = Math.Abs(Convert.ToDouble(ufDelay.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); d = 0.2;
            }

            try { ef = Math.Abs(Convert.ToDouble(gfDelay.Text.ToString())); }
            catch
            {
                System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); ef = 1;
            }

            if (d >= ef)
            {
                System.Windows.MessageBox.Show("UpdateFeature delay can not be greater or eqal to EmotionRecognition delay. Reverting back to default Values of 0.2 and 1", "you moron.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                lThres.Text = "0.2";
                uThres.Text = "1";
                d = 0.2;
                ef = 1;
            }

            //initializig feature extractor and passing the update graph.


            fExtract = new featureExtractor(d, ef, a, b, c, globalVars.chart2);
            globalVars.fExtract = this.fExtract;

            //Thread kinAppThrd = new Thread(() => { app = new kinectApp(canvas1, canvas2, label1, image1, fExtract); });
            //kinAppThrd.Start();

            app = new kinectApp(canvas1, canvas2, label1, image1, fExtract);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //fExtract.thrd.Join();
            globalVars.chartRighthand = false;
            app.stopApp();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            valueChanged = 1;
        }

        private void slider1_MouseLeave(object sender, MouseEventArgs e)
        {
            if (globalVars.kinectOn == true && valueChanged == 1 && (int)slider1.Value >= Camera.ElevationMinimum && (int)slider1.Value <= Camera.ElevationMaximum)
                app.moveCamera((int)slider1.Value);
            valueChanged = 0;
        }

        //record feature

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            //record feature
            if (globalVars.kinectOn == true)
            {
                globalVars.logFeatures = true;
                recordFeature.IsEnabled = false;
                stopRecord.IsEnabled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Kinect is not running. Please Plug in the Kinect and try again when the green light is on.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        //stop recording
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //stop recording
            globalVars.logFeatures = false;
            /*"this is not needed now"*/
            //globalVars.gFeature.saveFeatures();

            fExtract.saveFeatures();

            recordFeature.IsEnabled = true;
            stopRecord.IsEnabled = false;
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            //    if (checkBox1.IsChecked == true)
            //        globalVars.logSkele = true;
            //    else
            //        globalVars.logSkele = false;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            //    if (checkBox1.IsChecked == true)
            //        globalVars.logSkele = true;
            //    else
            //        globalVars.logSkele = false;

        }
        private void button2valence_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog browse1 = new System.Windows.Forms.OpenFileDialog();

            if (browse1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                loadANNValence.Text = browse1.FileName;

        }

        ///Start Emotion Detection
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ///Start Emotion Detection
            if (globalVars.detectorOn == false)
            {
                if (globalVars.kinectOn == false)
                    System.Windows.MessageBox.Show("Kinect is not running. Please Plug in the Kinect and try again when the green light is on.", "Kinect Not running", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    try
                    {


                        //elow, this was for just a test purpose
                        //recognizer R = new recognizer(loadANN.Text.ToString());

                        globalVars.detector = new newDynamicDetection(loadANN.Text.ToString(), loadANNValence.Text.ToString());
                        try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
                        catch
                        {
                            System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval = 10;
                        }


                        /////very experimental. previous detector for arousal

                        //globalVars.detector1 = new dynamicDetection(loadANN.Text.ToString());
                        //try { globalVars.detector1.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
                        //catch
                        //{
                        //    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector1.updateInterval = 10;
                        //}



                        globalVars.detectorOn = true;
                        startDetect.Content = "Stop Emotion Detection";

                        ufDelay.IsEnabled = false;
                        gfDelay.IsEnabled = false;

                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("osme problem with detector moduel.", "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                ufDelay.IsEnabled = true;
                gfDelay.IsEnabled = true;

                globalVars.detector.stopDetection();
                //globalVars.detector1.stopDetection();
                globalVars.detectorOn = false;
                startDetect.Content = "Start Emotion Detection";
            }

        }
        ///Start Learning
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //String[] str =textBox1.Text.ToString().Split(':');
            ///Start Learning
            //System.Windows.MessageBox.Show(textBox1.Text.ToString(), "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            try
            {
                //System.Windows.MessageBox.Show("number " + " " + textBox1.Text.ToString(), "meh.", MessageBoxButton.OK, MessageBoxImage.Error);

                try { globalVars.outputCount = Convert.ToInt32(outputCount.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.outputCount = 1;
                }

                try { globalVars.hiddenCount = Convert.ToInt32(hiddenCount.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.hiddenCount = 4;
                }



                globalVars.typeOfLearning = Convert.ToInt32(((ComboBoxItem)comboBox1.SelectedItem).Tag.ToString());

                startLearning.Content = "Busy Learning";
                startLearning.IsEnabled = false;
                Learner L = new Learner(textBox1.Text.ToString());

            }
            catch
            {
                System.Windows.MessageBox.Show("Enter a filename", "meh.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            //nothing
        }

        private void browsebtn_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog browse = new System.Windows.Forms.OpenFileDialog();

            if (browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox1.Text = browse.FileName;
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog browse1 = new System.Windows.Forms.OpenFileDialog();

            if (browse1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                loadANN.Text = browse1.FileName;
        }

        ///save ANN
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "savedNeuralNetwork"; // Default file name
            dlg.DefaultExt = ".dat"; // Default file extension
            dlg.Filter = "Data Files (.dat)|*.dat"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                Learner.saveANNtoFile(filename);
                //System.Windows.MessageBox.Show(filename, dlg.FileName, MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ArousalChkbx_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ArousalChkbx.IsChecked == true)
                {
                    arousalLev.IsEnabled = true;
                }
                else if (ArousalChkbx.IsChecked == false)
                {
                    arousalLev.IsEnabled = false;
                }
            }
            catch { }

        }

        private void valenceChkbx_Checked(object sender, RoutedEventArgs e)
        {
            valenceLev.IsEnabled = true;
        }

        private void ArousalChkbx_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ArousalChkbx.IsChecked == true)
                {
                    arousalLev.IsEnabled = true;
                }
                else if (ArousalChkbx.IsChecked == false)
                {
                    arousalLev.IsEnabled = false;
                }
            }
            catch { }
        }

        private void valenceChkbx_Unchecked(object sender, RoutedEventArgs e)
        {

            valenceLev.IsEnabled = false;

        }

        private void updateIntervalText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (globalVars.detector != null && updateIntervalText.Text.ToString() != null)
            {
                try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
                catch
                {
                    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval = 10;
                }
            }
        }

        private void reducedFeature_Checked(object sender, RoutedEventArgs e)
        {
            globalVars.reducedRecord = true;
        }

        private void reducedFeature_Unchecked(object sender, RoutedEventArgs e)
        {
            globalVars.reducedRecord = false;
        }

        //saves the detected values. all of them in abstract fileWriter.
        private void button1_Click_2(object sender, RoutedEventArgs e)
        {
            /* This is the small button for check
             it now will save the valence and arousal in file.
             */

            //globalVars.detector = new newDynamicDetection(loadANN.Text.ToString(), loadANNValence.Text.ToString());

            //try { globalVars.detector.updateInterval = Convert.ToInt32(updateIntervalText.Text.ToString()); }
            //catch
            //{
            //    System.Windows.MessageBox.Show("conversion to number failed. Reverting to default value.", "probably your fault.", MessageBoxButton.OK, MessageBoxImage.Exclamation); globalVars.detector.updateInterval=10;
            //}
            //globalVars.detectorOn = true;
            //globalVars.detector.test();


            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ArousalValenceResult"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text Files (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                System.IO.StreamWriter file;
                try
                {
                    file = new System.IO.StreamWriter(filename);

                    while (true)
                    {
                        string s = globalVars.avq.Dequeue();
                        if (s == null)
                            break;
                        else
                            file.WriteLine(s);
                    }

                    file.Close();
                }

                catch
                {
                    System.Windows.MessageBox.Show("Writing the AV results failed", "file creation error or the stack is full", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }


        }

        private void checkBox3_Checked(object sender, RoutedEventArgs e)
        {
            if (globalVars.detector != null)
                globalVars.detector.SlidingWindowOn = true;


        }

        private void checkBox3_Unchecked(object sender, RoutedEventArgs e)
        {
            if (globalVars.detector != null)
                globalVars.detector.SlidingWindowOn = false;

        }

        private void DetectMenu_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void DetectMenu_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void DetectMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            detectOn = !detectOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.DeepSkyBlue;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Visible;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void RecordMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            recordOn = !recordOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.DeepSkyBlue;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;
            recordCanvas.Visibility = System.Windows.Visibility.Visible;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void LearnMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            learnOn = !learnOn;
            LearnMenu.Foreground = Brushes.DeepSkyBlue;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.Gainsboro;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Visible;
            settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
        }

        private void advancedMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            settingsOn = !settingsOn;
            LearnMenu.Foreground = Brushes.Gainsboro;
            RecordMenu.Foreground = Brushes.Gainsboro;
            DetectMenu.Foreground = Brushes.Gainsboro;
            advancedMenu.Foreground = Brushes.DeepSkyBlue;

            detectCanvas.Visibility = System.Windows.Visibility.Hidden;
            recordCanvas.Visibility = System.Windows.Visibility.Hidden;
            learnCanvas.Visibility = System.Windows.Visibility.Hidden;
            settingsCanvas.Visibility = System.Windows.Visibility.Visible;
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            //app.stopApp();

        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //comboBox1.Tag.ToString();
        }

        private void rHandChartingOn_Checked(object sender, RoutedEventArgs e)
        {
            globalVars.chartRighthand = true;
        }

        private void rHandChartingOn_Unchecked(object sender, RoutedEventArgs e)
        {
            globalVars.chartRighthand = false;
        }

        private void snap_Click(object sender, RoutedEventArgs e)
        {
            string fname = "snap.bmp";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
            string filename = sanityChk(path, fname.Split('.')[0], "." + fname.Split('.')[1]);

            SaveCanvasToFile(filename);
            //Canvas.SetLeft(canvas1, 387);
            //Canvas.SetTop(canvas1, 18);
            //this.Show();
        }
        private String sanityChk(String a, String b, String c)
        {
            String temp;
            temp = b;

            for (int i = 1; i <= 10000; ++i)
            {
                if (!File.Exists(a + temp + c))
                    return (a + temp + c);
                temp = b + i;
                if (i >= 9950)
                {
                    string messageBoxText = "You might want to copy your feature files and start again";
                    string caption = "geez. too many feature files";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Exclamation;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }

            }
            return (a + temp + c);
        }

        private void refreshKinect_Click(object sender, RoutedEventArgs e)
        {
            if (globalVars.kinectOn == false)
                globalVars.kinectOn = app.initKinect(image1);
            else
                System.Windows.MessageBox.Show("Kinect is already running buddy. No need to refresh", "Kinect is okay", MessageBoxButton.OK, MessageBoxImage.Information);

        }


    }

}


    /*Achtung: The main kinect app which extracts the kinect output has been shifted to kinectApp.cs*/


