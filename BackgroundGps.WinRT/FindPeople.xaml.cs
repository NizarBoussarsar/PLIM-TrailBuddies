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
        private List<User> users;
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
            GetAllUsers();
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
                    users.Add(u);
                }
            }
        }

    }
}
