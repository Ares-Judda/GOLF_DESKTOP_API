using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOLF_DESKTOP.Model.Utilities {
    public class UserSingleton {
        private static UserSingleton _instance;

        public string IdUser { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        private UserSingleton() { }

        public static UserSingleton GetInstance() {
            if (_instance == null) {
                _instance = new UserSingleton();
            }
            return _instance;
        }
    }
}
