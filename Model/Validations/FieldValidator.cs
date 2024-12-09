using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOLF_DESKTOP.Model.Validations {
    public static class FieldValidator {
        public static bool AreFieldsValid(string email, string name, string username, string lastName, string password, object userType, bool hasImage) {
            return !string.IsNullOrWhiteSpace(email) &&
                   !string.IsNullOrWhiteSpace(name) &&
                   !string.IsNullOrWhiteSpace(username) &&
                   !string.IsNullOrWhiteSpace(lastName) &&
                   !string.IsNullOrWhiteSpace(password) &&
                   userType != null &&
                   hasImage &&
                   IsValidEmail(email);
        }

        public static bool IsValidEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) return false;
            int atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@')) return false;
            if (email.Contains(" ")) return false;
            string domain = email.Substring(atIndex + 1).ToLower();
            string[] allowedDomains = { "gmail.com", "hotmail.com", "outlook.com" };
            return Array.Exists(allowedDomains, d => domain == d);
        }
    }
}
