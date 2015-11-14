using BackgroundGps.WinRT.Models;
using Parse;
using System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BackgroundGps.WinRT
{

    public sealed partial class Login : Page
    {
        public Login()
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox.Text;
            string password = passwordBox.Password;

            var query = ParseObject.GetQuery("Usr").WhereEqualTo("Username", username).WhereEqualTo("Password", password);
            ParseObject parseUser = await query.FirstAsync();

            if (parseUser != null)
            {
                Frame.Navigate(typeof(StartMenu), username);
            }
            else
            {
                MessageDialog warningDialog = new MessageDialog("Login error ", "Parse Login Services");
                warningDialog.ShowAsync();

            }
        }
    }
}
