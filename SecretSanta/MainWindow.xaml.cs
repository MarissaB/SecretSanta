using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SecretSanta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<Santa> santas = new List<Santa>();
            public List<Santa> validSantas = new List<Santa>();
            public List<Santa> manualReviewSantas = new List<Santa>();
            public List<Santa> tooYoungSantas = new List<Santa>();
            public List<Santa> caughtGrinches = new List<Santa>();
            public List<Santa> internationalSantas = new List<Santa>();

        public List<Grinch> grinches = new List<Grinch>();

        #region Santa Stuff
        private void ImportRawFileButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".tsv",
                Filter = "TSV Files (*.tsv)|*.tsv"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                try
                {
                    IEnumerable<Santa> enumerableSantas = ReadSantas(filename);
                    ParseTheSantaData(enumerableSantas);
                    RawFileLinesLabel.Content = enumerableSantas.Count();
                }
                catch
                {
                    MessageBox.Show("Failed to import Santas file.", "ERROR: INVALID FILE PARSE", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public IEnumerable<Santa> ReadSantas(string fileName)
        {
            // We change file extension here to make sure it's a .tsv file.
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".tsv"));


            // lines.Select allows me to project each line as a Santa. 
            // This will give me an IEnumerable<Santa> back.
            return lines.Select(line =>
            {
                string[] data = line.Split('\t');
                // We return a santa with the data in order.
                return new Santa(data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10]);
            });
        }

        public void ParseTheSantaData(IEnumerable<Santa> rawSantaImport)
        {
            santas = rawSantaImport.ToList();
            List<Santa> removeTheseSantas = new List<Santa>();
            int counter = 0;
            foreach (Santa user in santas)
            {
                if (user.FirstName.Contains("FIRST NAME")) // cut the header row if it was included in the raw file
                {
                    removeTheseSantas.Add(user);
                }
                else
                {
                    counter++;
                    HowManySantasParsedLabel.Content = counter;
                }
            }
            santas.RemoveAll(x => removeTheseSantas.Contains(x));
        }
        #endregion


        #region Grinch Stuff
        private void ImportGrinches_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                // Set filter for file extension and default file extension 
                DefaultExt = ".tsv",
                Filter = "TSV Files (*.tsv)|*.tsv"
            };

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                try
                {
                    IEnumerable<Grinch> enumerableGrinches = ReadGrinches(filename);
                    ParseTheGrinchData(enumerableGrinches);
                    GrinchFileLinesLabel.Content = enumerableGrinches.Count();
                }
                catch
                {
                    MessageBox.Show("Failed to import Grinches file.", "ERROR: INVALID FILE PARSE", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public IEnumerable<Grinch> ReadGrinches(string fileName)
        {
            // We change file extension here to make sure it's a .tsv file.
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".tsv"));


            // lines.Select allows me to project each line as a Grinch. 
            // This will give me an IEnumerable<Grinch> back.
            return lines.Select(line =>
            {
                string[] data = line.Split('\t');
                // We return a grinch with the data in order.
                return new Grinch(data[0], data[1], data[2], data[4]);
            });
        }

        public void ParseTheGrinchData(IEnumerable<Grinch> grinchImport)
        {
            grinches = grinchImport.ToList();
            List<Grinch> removeTheseGrinches = new List<Grinch>();
            int counter = 0;
            foreach (Grinch user in grinches)
            {
                if (user.Name.Contains("NAME")) // cut the header row if it was included in the raw file
                {
                    removeTheseGrinches.Add(user);
                }
                else
                {
                    counter++;
                    HowManyGrinchesParsedLabel.Content = counter;
                }
            }
            grinches.RemoveAll(x => removeTheseGrinches.Contains(x));
        }
        #endregion

        #region Parsing Stuff
        private void ParseAccounts_Click(object sender, RoutedEventArgs e)
        {

            if (santas.Count == 0 || grinches.Count == 0)
            {
                MessageBox.Show("Santas and Grinches files are not imported.", "ERROR: MISSING FILES", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ParseTheData();

                ManualReviewText.Text = "The following Santas need manual review and correction before they can be valid for matching.\r\n\r\n";
                ManualReviewText.Text += LogParsing(manualReviewSantas);
                ManualReviewDataGrid.ItemsSource = manualReviewSantas;

                BootedAccountsText.Text = "The following Santas were booted and are not valid for matching.\r\n\r\n";
                BootedAccountsText.Text += LogParsing(tooYoungSantas) + "\r\n";
                BootedAccountsText.Text += LogParsing(caughtGrinches);
                BootedDataGrid.ItemsSource = tooYoungSantas.Concat(caughtGrinches);

                ValidSantaDataGrid.ItemsSource = validSantas;
            }
            LoadingLabel.Visibility = Visibility.Collapsed;

            if (manualReviewSantas.Count > 0)
            {
                ManuallyAddSantas.IsEnabled = true;
            }
            else
            {
                ManuallyAddSantas.IsEnabled = false;
            }
        }

        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke
            (
            DispatcherPriority.Background,
            (SendOrPostCallback)delegate (object arg)
            {
                var f = arg as DispatcherFrame;
                f.Continue = false;
            },
            frame
            );
            Dispatcher.PushFrame(frame);
        }


        public async void ParseTheData()
        {
            santas.Select(o => o.RedditUsername).Distinct();
            int validCount = 0;
            int badDate = 0;
            int manualReviewCount = 0;
            int grinchCount = 0;

            foreach (Santa user in santas)
            {

                user.GetAccountCreationDate();
                user.ValidateSanta();

                bool isAGrinch = user.AmIAGrinch(grinches);
                if (isAGrinch) // oh god kill it!
                {
                    grinchCount++;
                    caughtGrinches.Add(user);
                    await Dispatcher.BeginInvoke(new Action(() => BlacklistedLabel.Content = grinchCount.ToString()));
                    continue;
                }

                if (user.NeedsManualReview) // Something needs to be reviewed manually.
                {
                    manualReviewCount++;
                    manualReviewSantas.Add(user);
                }
                else // Nothing needs manual review, so check the date.
                {
                    DateTime cutOffDate = DateTime.Today.AddDays(-90);
                    if (cutOffDate < user.CreationDate)
                    {
                        badDate++;
                        user.ProblemFields.Add("Removed due to < 90 day old account.");
                        tooYoungSantas.Add(user);
                    }
                    else
                    {
                        validCount++;
                        validSantas.Add(user);
                    }
                }
                Thread.Sleep(10);  // wait for 0.1 second
                BlacklistedLabel.Content = grinchCount;
                ManualReviewLabel.Content = manualReviewCount;
                ValidSantasLabel.Content = validCount;
                YoungAccountLabel.Content = badDate;
                LoadingLabel.Visibility = Visibility.Visible;
                DoEvents();
            }
        }

        public string LogParsing(List<Santa> santaList)
        {
            string log = string.Empty;
            foreach (Santa santa in santaList)
            {
                log += "Reddit Username: " + santa.RedditUsername + "\r\n";
                foreach (string s in santa.ProblemFields)
                {
                    log += "---- " + s + "\r\n";
                }
            }
            return log;

        }

        #endregion

        #region Manually Add Names

        private void AddManuals_Click(object sender, RoutedEventArgs e)
        {
            List<Santa> santasToRemove = new List<Santa>();
            foreach (Santa row in ManualReviewDataGrid.ItemsSource)
            {
                if (row.NeedsManualReview == false)
                {
                    santasToRemove.Add(row);
                    row.ProblemFields.Clear();
                    validSantas.Add(row);
                }
            }

            foreach (Santa remove in santasToRemove)
            {
                manualReviewSantas.Remove(remove);
            }

            ManualReviewDataGrid.ItemsSource = null;
            ManualReviewDataGrid.ItemsSource = manualReviewSantas;
            ValidSantasLabel.Content = validSantas.Count;
            ManualReviewLabel.Content = manualReviewSantas.Count;

            if (manualReviewSantas.Count > 0)
            {
                ManuallyAddSantas.IsEnabled = true;
            }
            else
            {
                ManuallyAddSantas.IsEnabled = false;
            }
        }

        #endregion

        #region Sorting and Pairing Valid Santas

        private void PairSantas_Click(object sender, RoutedEventArgs e)
        {
            List<List<Santa>> separatedByCountry = BreakDownSantaList(validSantas);

            foreach (List<Santa> countryList in separatedByCountry)
            {
                List<Tuple<Santa, Santa>> pairedSantas = PairSantas(countryList);
            }

            if (internationalSantas.Count > 0)
            {
                ManualPairButton.IsEnabled = true;
                ManualPairGridUpper.ItemsSource = internationalSantas;
                ManualPairGridLower.ItemsSource = internationalSantas;
            }
        }

        public List<List<Santa>> BreakDownSantaList(List<Santa> unfilteredList)
        {
            List<List<Santa>> brokenDownList = new List<List<Santa>>();

            List<string> countries = unfilteredList.Select(o => o.Country).Distinct().ToList();
            foreach (string country in countries)
            {
                List<Santa> countryOfSantas = unfilteredList.Where(c => c.Country == country).ToList();

                if (countryOfSantas.Count % 2 != 0) // Number of santas in this batch is odd! Grab an international santa
                {
                    Santa specialPick = new Santa();
                    try // to get an overseas international santa
                    {
                        specialPick = countryOfSantas.Where(s => s.ShipOverseas == true).First();
                    }
                    catch // international shipping alone is okay too if there are no overseas shipper santas
                    {
                        specialPick = countryOfSantas.Where(s => s.ShipInternationally == true).FirstOrDefault();
                    }
                    internationalSantas.Add(specialPick);
                    countryOfSantas.Remove(specialPick);
                }

                if (countryOfSantas.Count > 0) // Because if there's only one they're in the international pool already
                {
                    brokenDownList.Add(countryOfSantas);
                }
            }

            return brokenDownList;
        }

        public List<Tuple<Santa, Santa>> PairSantas(List<Santa> inputList)
        {
            List<Tuple<Santa, Santa>> pairedSantas = new List<Tuple<Santa, Santa>>();
            List<Santa> recipientSantas = new List<Santa>(inputList); // santas we pull from - this will go down to empty

            foreach (Santa santa in inputList)
            {
                Santa recipient = GetRandomSanta(santa, recipientSantas);
                Tuple<Santa, Santa> pair = new Tuple<Santa, Santa>(santa, recipient);
                pairedSantas.Add(pair);
                recipientSantas.Remove(recipient);
            }

            return pairedSantas;
        }

        public Santa GetRandomSanta(Santa giver, List<Santa> recipients)
        {
            Random random = new Random();
            int randomNumber;
            Santa pick = new Santa();

            do
            {
                if (recipients.Count != 2)
                {
                    randomNumber = random.Next(0, recipients.Count - 1);
                    pick = recipients[randomNumber];
                }
                else
                {
                    pick = recipients[1]; // When there are only 2 left, pair 'em up directly to avoid loop issues
                }
            }
            while (pick.RedditUsername == giver.RedditUsername);

            return pick;

        }

        #endregion

        #region Manual Pairs (UN of Santas)
        /*TODO: Use two lists to pair santas manually.
         * Username validation to avoid pairing to themselves.
         * Add pairs under a manual bunch.
         */

        private void PairManualSantas_Click(object sender, RoutedEventArgs e)
        {
            
        }

        #endregion
    }
}
