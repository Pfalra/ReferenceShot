using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisagReferenceShot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Shooter> ShootersList = new List<Shooter>();
        public List<Shooter> ResultList = new List<Shooter>();
        public string CurrentFileName = "";
        public string ReferenceShooterName = "";


        // Keep in mind! "1 div" = 0.01mm 

        public MainWindow()
        {
            InitializeComponent();

            ReferenceShooterList.SelectionChanged += ReferenceShooterList_SelectionChanged;
        }


        private void ReferenceShooterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReferenceShooterList.SelectedItem != null)
            {
                try
                {
                    Shooter selectedReferenceShooter = (Shooter)ReferenceShooterList.SelectedItem;
                    // FIXME: Build a shot selector window instead of taking the best shot of the shooters shots
                    RefreshResultList(selectedReferenceShooter);
                }
                catch (InvalidCastException ex)
                {
                    MessageBox.Show("Invalid XML data");
                }

            }
        }


        private void PathChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Multiselect = false;
            fileDialog.Title = "Choose Data";
            fileDialog.CheckFileExists = true;
            fileDialog.CheckPathExists = true;
            fileDialog.Filter = "XML Files (*.xml)|*.xml";

            if (fileDialog.ShowDialog() == true)
            {
                PathLabel.Content = fileDialog.FileName;
                CurrentFileName = fileDialog.FileName;
                RefreshShooterList(fileDialog.FileName);
            }

        }

        private void ResultListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }




        private async void RefreshShooterList(string filename)
        {
            ReferenceShooterList.IsEnabled = false;
            ShootersList.Clear();
            ReferenceShooterList.Items.Clear();
            ReferenceShooterList.Items.Add("LOADING...");
            ReferenceShooterList.Items.Add("LOADING...");
            ReferenceShooterList.Items.Add("LOADING...");

            await Task.Run(() =>
            {
#pragma warning disable CS8601 // Mögliche Nullverweiszuweisung.
                ShootersList = XmlParser.GetShootersFromXml(filename);
#pragma warning restore CS8601 // Mögliche Nullverweiszuweisung.
            });

            if (ShootersList != null && ShootersList.Count > 0)
            {
                ReferenceShooterList.Items.Clear();
                ReferenceShooterList.ItemsSource = ShootersList;
                ReferenceShooterList.IsEnabled = true;
            }
            else
            {
                ReferenceShooterList.Items.Clear();
                ReferenceShooterList.Items.Add("No shooters detected");
            }
        }


        private void RefreshResultList(Shooter referenceShooter)
        {
            ResultListBox.Items.Clear();
            Shot referenceShot = referenceShooter.FindBestShotByStandardMode();

            foreach (Shooter shooter in ShootersList)
            {
                Shot bestShotByShooter = shooter.FindBestShotByReference(referenceShot, out double distance);

                ResultList.Add(shooter);
            }

            ShotResult.OrderResults(ref ShootersList);

            // Keep place 0 for sanity check
            int count = 0;
            ResultListBox.Items.Clear();
            foreach (Shooter shooter in ShootersList)
            {
                string outputStr = String.Format("P{0}: {1}, {2} ({3} | {4}mm)", 
                                count, 
                                shooter.LastName, 
                                shooter.FirstName, 
                                shooter.BestShotToReference.decimalResult, 
                                shooter.BestShotDistToReference.ToString($"F{3}"));
                ResultListBox.Items.Add(outputStr);
                count++;
            }


            if (ResultListBox.Items.Count <= 0)
            {
                ResultListBox.Items.Add("No results to list");
            }
        }


        private void ReloadShootersButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshShooterList(CurrentFileName);
        }

        private void ReloadResultsButton_Click(object sender, RoutedEventArgs e)
        {
            //RefreshResultList();    
        }
    }
}
