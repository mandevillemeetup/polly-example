using System;
using System.Net;
using System.Windows;
using Humanizer;
using Polly;

namespace WpfApplication1
{

    internal class GitHubGateway
    {
        public string GetData()
        {
            using (var wc = new WebClient())
            {
                return wc.DownloadString(
                        "https://raw.githubusercontent.com/jason-roberts/PSPolDemoFiles/master/SomeRemoteData.txt");                
            }
        }
    }



    public partial class MainWindow : Window
    {
        private readonly GitHubGateway _gateway;
        private readonly Policy _policy;

        public MainWindow()
        {
            InitializeComponent();

            _gateway = new GitHubGateway();

            _policy = Policy.Handle<WebException>()
                            .CircuitBreaker(2, 30.Seconds());
        }

        
        private void Go_OnClick(object sender, RoutedEventArgs e)
        {           
            try
            {
                var downloadedData = _policy.Execute(() => _gateway.GetData());                

                Output.Text += DateTime.Now + " " + downloadedData + Environment.NewLine;
            }
            catch (Exception ex)
            {                
                Output.Text += DateTime.Now + " " + ex.Message + Environment.NewLine;
            }       
        }         
    }
}

