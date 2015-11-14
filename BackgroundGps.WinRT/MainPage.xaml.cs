using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Parse;
using BackgroundGps.WinRT.Models;
using System.Threading.Tasks;
using BackgroundGps.WinRT.ViewModel;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace BackgroundGps.WinRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private BackgroundTaskRegistration deviceUseTask;
        private DateTime startTime, endTime;
        private TimeSpan duration;
        private string username;
        private List<string> coordonates;
        private List<Trail> trails;

        public MainPage()
        {
            this.InitializeComponent();

            TrackLocationButton.Click += MainPage_Loaded;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            coordonates = new List<string>();

            //Buton
            StoptrackingButton.IsEnabled = false;

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

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            var promise = await BackgroundExecutionManager.RequestAccessAsync();

            if (promise == BackgroundAccessStatus.Denied)
            {
                MessageDialog warningDialog = new MessageDialog("Background execution is disabled. Please re-enable in the Battery Saver app to allow this app to function", "Background GPS");
                await warningDialog.ShowAsync();
            }
            else
            {
                var defaultSensor = Windows.Devices.Sensors.Accelerometer.GetDefault();
                if (defaultSensor != null)
                {
                    var deviceUseTrigger = new DeviceUseTrigger();

                    deviceUseTask = RegisterBackgroundTask("BackgroundGps.Engine.BackgroundGpsTask", "GpsTask", deviceUseTrigger, null);

                    try
                    {
                        DeviceTriggerResult r = await deviceUseTrigger.RequestAsync(defaultSensor.DeviceId);

                        System.Diagnostics.Debug.WriteLine(r); //Allowed 
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }
            }
        }

        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
                                                                        string taskName,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition)
        {
            //
            // Check for existing registrations of this background task.
            //

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                System.Diagnostics.Debug.WriteLine(cur.Value.Name);

                if (cur.Value.Name == taskName)
                {
                    System.Diagnostics.Debug.WriteLine("Task already registered " + taskName);
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            System.Diagnostics.Debug.WriteLine("Registering new task " + taskName);

            //
            // Register the background task.
            //

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = null;

            try
            {
                task = builder.Register();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return task;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            username = e.Parameter.ToString();
        }

        private void TrackLocationButton_Click(object sender, RoutedEventArgs e)
        {
            TrackLocationButton.IsEnabled = false;
            StoptrackingButton.IsEnabled = true;
            StoptrackingButton.Visibility = Visibility.Visible;
            TrackLocationButton.Visibility = Visibility.Collapsed;

            startTime = DateTime.Now;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("startTime"))
            {
                localSettings.Values.Remove("startTime");
            }
            localSettings.Values.Add("startTime", startTime.ToString());

            progressRing.IsActive = true;

        }

        private async void StoptrackingButton_Click(object sender, RoutedEventArgs e)
        {
            TrackLocationButton.IsEnabled = true;
            StoptrackingButton.IsEnabled = false;

            TrackLocationButton.Visibility = Visibility.Visible;
            StoptrackingButton.Visibility = Visibility.Collapsed;

            deviceUseTask.Unregister(true);


            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey("endTime") == true)
            {
                object tmp = null;
                localSettings.Values.TryGetValue("endTime", out tmp);

                endTime = Convert.ToDateTime(tmp.ToString());

                System.Diagnostics.Debug.WriteLine("endTime Time  " + endTime.ToString());

                duration = endTime - startTime;

                System.Diagnostics.Debug.WriteLine("Duration " + duration.TotalMinutes);

            }

            /////
            if (localSettings.Values.ContainsKey("dist") == true)
            {
                object tmp = null;
                localSettings.Values.TryGetValue("dist", out tmp);

                double dist = (double)tmp;


                System.Diagnostics.Debug.WriteLine("FINAL Dist : " + dist);

                /// PARSE
                /// 
                var trailObject = new ParseObject("Trail");
                trailObject["distance"] = dist;
                trailObject["duration"] = duration.TotalMinutes;
                trailObject["userId"] = username;

                await trailObject.SaveAsync();
                //
                GetAllTrails();
                progressRing.IsActive = false;
            }
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartMenu), username);
        }

        private async void GetX(List<ParseObject> listResult)
        {
            int[] zero = new int[listResult.Count],
                one = new int[listResult.Count],
                two = new int[listResult.Count];
            int i = 0;

            foreach (var item in listResult)
            {
                switch (item.Get<int>("clusterId"))
                {
                    case 0:
                        zero[i]++;
                        break;
                    case 1:
                        one[i]++;
                        break;
                    case 2:
                        two[i]++;
                        break;
                    default:
                        break;
                }
                i++;
            }

            var query = ParseObject.GetQuery("Usr").WhereNotEqualTo("objectId", "toto");
            IEnumerable<ParseObject> results = await query.FindAsync();
            i = 0;
            foreach (var user in results)
            {
                user["Cluster0"] = zero[i];
                user["Cluster1"] = one[i];
                user["Cluster2"] = two[i];
                i++;
                await user.SaveAsync();
            }

            System.Diagnostics.Debug.WriteLine("Zero " + zero + " One " + one + " Two " + two);
        }

    }

}