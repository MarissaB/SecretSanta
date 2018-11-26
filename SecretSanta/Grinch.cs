using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta
{
    public class Grinch
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string RedditUsername { get; set; }
        public string Address { get; set; }

        public Grinch(string name, string email, string reddit, string address)
        {
            Name = name.ToUpper().Trim();
            EmailAddress = email.ToUpper().Trim();
            RedditUsername = SanitizeUsername(reddit);
            Address = address.ToUpper().Trim();
        }

        public string SanitizeUsername(string user)
        {
            string cleaned = string.Empty;
            cleaned = user.ToUpper();
            if (cleaned.Contains("/"))
            {
                cleaned = cleaned.Replace("/U", "");
                cleaned = cleaned.Replace("U/", "");
                cleaned = cleaned.Replace("/", "");
            }
            return cleaned;
        }
    }
}
