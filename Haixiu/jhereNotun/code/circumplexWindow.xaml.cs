using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1.code
{
    /// <summary>
    /// Interaction logic for circumplexWindow.xaml
    /// </summary>
    public partial class circumplexWindow : Window
    {
        public circumplexWindow()
        {
            InitializeComponent();
        }
        private void CircumplexWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Show();

        }
        private void CircumplexWindow_Closed(object sender, EventArgs e)
        {
            globalVars.isCircmplexBigOpen = false;

            globalVars.circumplexMaximizeButton.Content = "Open in bigger window";
            //globalVars.mainwin.Show();
        }
    }
}
