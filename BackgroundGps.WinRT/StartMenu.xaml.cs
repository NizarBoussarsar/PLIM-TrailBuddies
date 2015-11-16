using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Navigation;

namespace BackgroundGps.WinRT
{
    public sealed partial class StartMenu : Page
    {
        private string username;

        public StartMenu()
        {
            this.InitializeComponent();

        }

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

            Frame.Navigate(typeof(FindPeople), username);
        }

    }
}