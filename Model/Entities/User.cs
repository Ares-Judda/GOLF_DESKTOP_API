using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOLF_DESKTOP.Model.Entities
{
    public class User
    {
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string userName { get; set; }
        public string imagen { get; set; }
        public string? phone { get; set; }
        public DateTime? birthDate { get; set; }
        public string? address { get; set; }
        public string? postalCode { get; set; }
    }
}
