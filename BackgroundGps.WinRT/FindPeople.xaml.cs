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
        private List<Trail> trails;
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

            GetAllTrails();


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

        private async Task GetAllTrails()
        {
            trails = new List<Trail>();

            var query = ParseObject.GetQuery("Trail").WhereNotEqualTo("objectId", "toto");
            IEnumerable<ParseObject> results = await query.FindAsync();

            foreach (var item in results)
            {
                Trail trail = new Trail();
                trail.Distance = item.Get<float>("distance");
                trail.Duration = (int)item.Get<double>("duration");
                trail.Id = item.ObjectId;
                trails.Add(trail);
            }

            Kmeans.Init(trails);
            int[] clusters = Kmeans.Cluster(Kmeans.rawData, 3);

            List<ParseObject> listResult = new List<ParseObject>();

            for (int i = 0; i < results.Count(); i++)
            {
                ParseObject pObject = results.ElementAt(i);
                pObject["clusterId"] = clusters[i];
                listResult.Add(pObject);
                await pObject.SaveAsync();
            }

            GetX(listResult);
        }

        private void GetX(List<ParseObject> listResult)
        {
            int zero = 0, one = 0, two = 0;
            foreach (var item in listResult)
            {
                if (item.Get<string>("userId") == username)
                {
                    switch (item.Get<int>("clusterId"))
                    {
                        case 0:
                            zero++;
                            break;
                        case 1:
                            one++;
                            break;
                        case 2:
                            two++;
                            break;
                        default:
                            break;
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine("Zero " + zero + " One " + one + " Two " + two);
        }





    }
}
