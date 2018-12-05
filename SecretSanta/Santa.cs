using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Diagnostics;

namespace SecretSanta
{
    public class Santa
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string RedditUsername { get; set; }
        public string Wishlist { get; set; }
        public bool Rematcher { get; set; }
        public bool ShipInternationally { get; set; }
        public bool ShipOverseas { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public DateTime CreationDate { get; set; }
        public bool NeedsManualReview { get; set; }
        public List<string> ProblemFields { get; set; }
        public string OutputString { get; set; }

        public Santa() { }

        public Santa (string firstName) // used only for special vendor pairs on singles
        {
            FirstName = firstName;
        }

        public Santa(string firstName, string lastName, string email, string username, string wishlist, string rematcher, string international, string overseas, string country, string address)
        {
            FirstName = firstName.ToUpper().Trim();
            LastName = lastName.ToUpper().Trim();
            EmailAddress = email.ToUpper().Trim();
            RedditUsername = SanitizeUsername(username).Trim();
            Wishlist = wishlist;
            Rematcher = CastStringToBool(rematcher);
            ShipOverseas = CastStringToBool(overseas);
            if (ShipOverseas)
            {
                ShipInternationally = true; // Clearly if you can ship overseas, you're shipping internationally already...
            }
            else
            {
                ShipInternationally = CastStringToBool(international); // ...but you could be land-locked if you're not cool with overseas.
            }
            Country = country.ToUpper().Trim();
            Address = address.ToUpper().Trim();
            NeedsManualReview = false;
            ProblemFields = new List<string>();
            OutputString = Stringify();
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

        public bool CastStringToBool(string input)
        {
            bool result = false;
            if(input.Contains("Yes"))
            {
                result = true;
            }
            return result;

        }

        public void GetAccountCreationDate()
        {
            DateTime creationDate = DateTime.Today;
            string accountOverviewURL = BuildAccountURL(RedditUsername);
            string timeHTML = string.Empty;

            //get the page
            var web = new HtmlWeb();
            var document = web.Load(accountOverviewURL);
            var page = document.DocumentNode;

            //loop through all span tags with age css class
            foreach (var item in page.QuerySelectorAll("span.age"))
            {
               timeHTML = item.QuerySelector("time").OuterHtml;
            }
            creationDate = ParseCreationHTML(timeHTML);
            CreationDate = creationDate;
        }

        public string BuildAccountURL(string username)
        {
            string url = string.Empty;

            url = "https://www.reddit.com/user/" + username + "/overview";

            return url;
        }

        public DateTime ParseCreationHTML(string html)
        {
            DateTime date = DateTime.Today;
            if (html.Length > 0)
            {
                int dividerIndex = html.IndexOf("datetime");
                string parsed = html.Substring(dividerIndex + 10, 10);

                date = DateTime.Parse(parsed);
            }
            else
            {
                NeedsManualReview = true;
                ProblemFields.Add("CreationDate not found. Please manually check their Reddit user page.");
            }

            return date;
        }

        public bool AmIAGrinch(List<Grinch> grinches)
        {
            bool isGrinch = false;
            foreach (Grinch grinch in grinches)
            {
                if (grinch.Name.Contains(FirstName) && grinch.Name.Contains(LastName))
                {
                    isGrinch = true;
                    ProblemFields.Add("Grinch! " + FirstName + " " + LastName);
                    return isGrinch;
                }
                if (grinch.RedditUsername.Contains(RedditUsername))
                {
                    isGrinch = true;
                    ProblemFields.Add("Grinch! " + RedditUsername);
                    return isGrinch;
                }
                if (grinch.EmailAddress.Contains(EmailAddress))
                {
                    isGrinch = true;
                    ProblemFields.Add("Grinch! " + EmailAddress);
                    return isGrinch;
                }
                if (grinch.Address.Contains(Address))
                {
                    isGrinch = true;
                    ProblemFields.Add("Grinch! " + Address);
                    return isGrinch;
                }

            }

            return isGrinch;
        }

        public void ValidateSanta()
        {
            if (FirstName.Length == 0 || LastName.Length == 0)
            {
                NeedsManualReview = true;
                ProblemFields.Add("Missing name.");
            }

            if (!EmailAddress.Contains("@") || !EmailAddress.Contains("."))
            {
                NeedsManualReview = true;
                ProblemFields.Add("Invalid email.");
            }

            if (EmailAddress.Contains("..")) // fix stupid email typos
            {
                EmailAddress = EmailAddress.Replace("..", ".");
            }

            if(Country.Length == 0 || Address.Length == 0)
            {
                NeedsManualReview = true;
                ProblemFields.Add("Invalid country or address.");
            }
        }

        /* Goal columns (starting on row 3)
        * First Name
        * Last Name
        * Email
        * Username
        * What they want
        * Country
        * Address */

        public string Stringify()
        {
            string text = string.Empty;

            text = FirstName + "\t" + LastName + "\t" + EmailAddress + "\t" +
                RedditUsername + "\t" + Wishlist + "\t" + Country + "\t" + Address;

            return text;
        }
    }
}
