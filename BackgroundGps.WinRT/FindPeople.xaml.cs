using BackgroundGps.WinRT.Models;
using BackgroundGps.WinRT.ViewModel;
using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
// Pour en savoir plus sur le modèle d’élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkID=390556

namespace BackgroundGps.WinRT
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class FindPeople : Page
    {
        private List<User> users, similarUsers;

        private User userConnected;
        private string username;

        public FindPeople()
        {
            this.InitializeComponent();


            try
            {
                ParseClient.Initialize("tFQtC1M0IhpZCWBBRRmqXCCE3SdUHO76f1RSNDOD", "wX0h5aUBInXq1NzNZIVx5b04kdidb4iGHKPLKidf");
            }
            catch (Exception)
            {
                MessageDialog warningDialog = new MessageDialog("We couldn't get Parse to work, please try to connect to a Wifi", "Parse BaaS");
                warningDialog.ShowAsync();
            }
            //System.Diagnostics.Debug.WriteLine("XP : " + xp);

            users = new List<User>();
            similarUsers = new List<User>();
            GetAllUsers();
            System.Diagnostics.Debug.WriteLine("SIZE : " + users.Count);
            //  similarUsers = new List<User>();
            ///  this.GetSimilarUsers();

        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            username = e.Parameter.ToString();
        }

        private async void GetAllUsers()
        {
            var query = ParseObject.GetQuery("Usr").WhereNotEqualTo("objectId", "toto");
            IEnumerable<ParseObject> results = await query.FindAsync();
            foreach (ParseObject item in results)
            {
                User u = new User()
                {
                    Id = item.ObjectId,
                    Name = item.Get<string>("Username"),
                    NbrEasyTrails = item.Get<int>("Easy"),
                    NbrMediumTrails = item.Get<int>("Medium"),
                    NbrHardTrails = item.Get<int>("Hard"),
                    PhoneNumber = item.Get<string>("Phone"),
                };

                int easyScore = (u.NbrEasyTrails % 5) * 1;
                int mediumScore = (u.NbrMediumTrails % 3) * 2;
                int hardScore = (u.NbrHardTrails % 1) * 3;

                u.nbTrails = u.NbrEasyTrails + u.NbrMediumTrails + u.NbrHardTrails;
                u.Xp = easyScore + mediumScore + hardScore;

                System.Diagnostics.Debug.WriteLine("XP : " + u.Xp + " NB : " + u.nbTrails);


                if (u.Name == username)
                {
                    userConnected = u;
                }
                else
                {
                    this.users.Add(u);
                }
            }

            foreach (User user in this.users)
            {
                if (CompareUsers(this.userConnected, user))
                {
                    this.similarUsers.Add(user);
                }
            }
            int i = 0;
            foreach (User usr in this.similarUsers)
            {
                txtSimilar.Text += i + "- " + usr.Name + " Tel. " + usr.PhoneNumber + " \n";
            }
        }


        private List<User> GetItemsForListView()
        {
            System.Diagnostics.Debug.WriteLine("LEN : " + users.Count());
            return users;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), username);
        }

        private bool CompareUsers(User connectedUser, User otherUser)
        {
            double xpMargin = connectedUser.Xp * 0.05; // 5% de XP 
            double nbMargin = connectedUser.nbTrails * 0.05; // 5% de XP 

            /// TEST XP
            if (Math.Abs(connectedUser.Xp - otherUser.Xp) > xpMargin) // Not Equal Bc. XP diffrent 
            {
                return false;
            }

            if (Math.Abs(connectedUser.nbTrails - otherUser.nbTrails) > nbMargin) // Not Equal Bc. XP diffrent 
            {
                return false;
            }

            // if no test fails then equal
            return true;
        }


    }
}
