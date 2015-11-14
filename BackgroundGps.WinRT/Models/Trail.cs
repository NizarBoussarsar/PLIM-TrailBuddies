using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace BackgroundGps.WinRT.Models
{
    public class Trail
    {
        public string Id { get; set; }
        public int Duration { get; set; }
        public float Distance { get; set; }
        public float Altitude { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdateDate { get; set; }

        public string tmpTest { get; set; }

        public Trail()
        {

        }

        public Trail(int duration, float distance, float altitude)
        {
            this.Duration = duration;
            this.Distance = distance;
            this.Altitude = altitude;
            CreationDate = DateTime.Now.ToString();
            LastUpdateDate = DateTime.Now.ToString();
        }


        public void computeAltitude()
        {

        }


        public async Task GetValues()
        {

            string jsonMessage;
            string address = string.Format(
               "https://maps.googleapis.com/maps/api/elevation/json?locations=39.7391536,-104.9847034&key=AIzaSyAslRmFLPgPTsa39BlN4P_V29l_y4QttTY");
            // Uri.EscapeDataString("" + lat),
            // Uri.EscapeDataString("" + lon));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Method = "GET";
            request.Accept = "application/json";

            try
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        jsonMessage = reader.ReadToEnd();
                        this.tmpTest = jsonMessage;

                        System.Diagnostics.Debug.WriteLine("ALT :" + this.tmpTest);

                    }
                }
            }
            catch (WebException ex)
            {
                tmpTest = ex.Message;
            }
        }

        public virtual User User { get; set; }
    }
}
