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
                    NbrTrailsCluster0 = item.Get<int>("Cluster0"),
                    NbrTrailsCluster1 = item.Get<int>("Cluster1"),
                    NbrTrailsCluster2 = item.Get<int>("Cluster2"),
                    PhoneNumber = item.Get<string>("Phone"),
                };

                u.nbTrails = u.NbrTrailsCluster0 + u.NbrTrailsCluster1 + u.NbrTrailsCluster2;

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

        private bool CompareUsersTrailFrq(User connectedUser, User otherUser)
        {
            float frqMargin = 0.05f;

            float connectedfrq0, connectedfrq1, connectedfrq2;
            float frq0, frq1, frq2;

            connectedfrq0 = connectedUser.NbrTrailsCluster0 / connectedUser.nbTrails;
            connectedfrq1 = connectedUser.NbrTrailsCluster1 / connectedUser.nbTrails;
            connectedfrq2 = connectedUser.NbrTrailsCluster2 / connectedUser.nbTrails;

            frq0 = otherUser.NbrTrailsCluster0 / otherUser.nbTrails;
            frq1 = otherUser.NbrTrailsCluster1 / otherUser.nbTrails;
            frq2 = otherUser.NbrTrailsCluster2 / otherUser.nbTrails;

            if (Math.Abs(connectedfrq0 - frq0) > frqMargin)
            {
                return false;
            }

            if (Math.Abs(connectedfrq1 - frq1) > frqMargin)
            {
                return false;
            }

            if (Math.Abs(connectedfrq2 - frq2) > frqMargin)
            {
                return false;
            }


            return true;
        }

        private bool CompareUsers(User connectedUser, User otherUser)
        {
            double nbMargin = connectedUser.nbTrails * 0.05; // 5% de XP 

            if (Math.Abs(connectedUser.nbTrails - otherUser.nbTrails) > nbMargin) // Not Equal Bc. XP diffrent 
            {
                return false;
            }

            return CompareUsersTrailFrq(connectedUser, otherUser);

        }


    }
}
