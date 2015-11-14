using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundGps.WinRT.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int NbrTrailsCluster0 { get; set; }
        public int NbrTrailsCluster1 { get; set; }
        public int NbrTrailsCluster2 { get; set; }
        public string CreationDate { get; set; }
        public string LastUpdateDate { get; set; }
        public int Xp { get; set; }
        public int nbTrails { get; set; }

        public User()
        {

        }

        public string toString()
        {
            return this.Name + " Tel. " + this.PhoneNumber;
        }

        public User(string name, string phoneNumber)
        {
            this.Name = name;
            this.PhoneNumber = phoneNumber;
            this.NbrTrailsCluster0 = 0;
            this.NbrTrailsCluster1 = 0;
            this.NbrTrailsCluster2 = 0;
            CreationDate = DateTime.Now.ToString();
            LastUpdateDate = DateTime.Now.ToString();
        }

    }
}
