using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class StartMenu : Page
    {
        private string username;

        public StartMenu()
        {
            this.InitializeComponent();
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


        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), username);
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            /*
            await ComputeUsersExperiences(username);
            string param = username + "#" + xp;
            Frame.Navigate(typeof(FindPeople), param);
            */
            Frame.Navigate(typeof(FindPeople), username);
        }

        /*
        private async Task ComputeUsersExperiences(string name)
        {
            int easyScore = 0, mediumScore = 0, hardScore = 0;
            var query = ParseObject.GetQuery("Usr").WhereEqualTo("Username", name);
            IEnumerable<ParseObject> results = await query.FindAsync();
            foreach (var user in results)
            {
                easyScore = (user.Get<int>("Easy") % 5) * 1;
                mediumScore = (user.Get<int>("Medium") % 3) * 2;
                hardScore = (user.Get<int>("Hard") % 1) * 3;
            }
            this.xp = easyScore + mediumScore + hardScore;
        }
        */
    }
}
