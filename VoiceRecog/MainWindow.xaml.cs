using System.Diagnostics;
using System.Globalization;
using System.Speech.Recognition;
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

namespace VoiceRecog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
        {
        private SpeechRecognitionEngine recognizer;
        private bool isRecognitionRunning = true; // Track whether recognition is currently running
        public SimConnectImplementer mysimconnect;

        public MainWindow()
        {
            InitializeComponent();

            // Create a Choices object containing a list of choices.
            CultureInfo culture = new CultureInfo("en-US");

            Choices commands = new Choices();
            commands.Add(new string[] { "gear up", "gear down", "flaps down", "flaps up" });
            // Create a GrammarBuilder object and append the Choices object.
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commands);
            gb.Culture = culture;    
            
            // Create the Grammar instance and load it into the recognizer.
            Grammar grammar = new Grammar(gb);
            



            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            recognizer.SetInputToDefaultAudioDevice();

            //Grammar grammar = new DictationGrammar();
            
            recognizer.LoadGrammar(grammar);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(VoiceRecognizer);
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread connectSimConnect = new Thread(ConnectSimConnect);
            connectSimConnect.IsBackground = true;
            connectSimConnect.Start();

            Debug.WriteLine("Window Loaded");
        }

        public void ConnectSimConnect()
        {
            mysimconnect = new SimConnectImplementer();
            mysimconnect.LogResult += OnAddResult;

            //Debug.WriteLine($"Simconnect started");
            bool localbSimConnected = false;
            bool title_registred = false;
            while (true)
            {
                //Debug.Write($"Start loop");
                Thread.Sleep(1000);
                localbSimConnected = mysimconnect.bSimConnected;
                if (localbSimConnected == false)
                {
                    //Debug.WriteLine($"Start inner if ");
                    Thread.Sleep(1000);
                    this.Dispatcher.Invoke(() =>
                    {
                        //Debug.WriteLine($"Just before simconnect call");
                        mysimconnect.Connect();
                        Thread.Sleep(200);
                        localbSimConnected = mysimconnect.bSimConnected;
                        Debug.WriteLine($"Simconnect status loop: {localbSimConnected}");
                        if (localbSimConnected)
                        {
                            //SimconnectStatusEllipse.Fill = Brushes.Green;
                            //int i = mysimconnect.RegisterSimVar("TITLE", "unit", "string", "NOT_USED");
                            //title_registred = true;
                            //Debug.WriteLine($"Title registred {i}");

                        }
                        else
                        {
                            //SimconnectStatusEllipse.Fill = Brushes.Red;
                            TextBox1.Text += "Looking for simulator...\r\n";
                            Thread.Sleep(200);
                        }
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        //SimconnectStatusEllipse.Fill = Brushes.Green;
                    });
                    

                }
            }
        }


        private void OnAddResult(object sender, string sResult)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!sResult.Contains("|"))
                {
                    TextBox1.AppendText(sResult + "\r\n");
                    TextBox1.ScrollToEnd();
                }
                //Debug.WriteLine(sResult);

            });

            
        }




        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (isRecognitionRunning)
            {
                recognizer.RecognizeAsyncCancel();
                Button1.Content = "Start Speech Recognition";
            }
            else
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                Button1.Content = "Stop Speech Recognition";
            }

            isRecognitionRunning = !isRecognitionRunning; // Toggle the recognition state

        }

        private void VoiceRecognizer(object sender, SpeechRecognizedEventArgs e)
        {
            TextBox1.Text += e.Result.Text + "\r\n";
            if (e.Result.Text == "gear up")
            {
                mysimconnect.SendEvent("Mobiflight.GEAR_UP", 1);
                Debug.WriteLine("GEAR UP SEND!!!!!!!!!!!!");
            }
            if (e.Result.Text == "gear down")
            {
                mysimconnect.SendEvent("Mobiflight.GEAR_DOWN", 1);
                Debug.WriteLine("GEAR UP DOWN!!!!!!!!!!!!");
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Clear();
        }
    }
}