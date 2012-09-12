using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.Text.RegularExpressions;

// Import log4net classes.
using log4net;
using log4net.Config;


namespace TravelMonkey
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            // Set up a simple configuration that logs on the console.
            BasicConfigurator.Configure();

            log.Info("Entering application.");

            getMail();

            log.Info("App is finished");
        }

        private static void getMail()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);

            service.Credentials = new System.Net.NetworkCredential( "neville.kuyt", "0bvious!", "EMEA" );

            service.AutodiscoverUrl("neville.kuyt@akqa.com");

            log.Debug("Connecting to email server" + service.ServerInfo);

            FindItemsResults<Item> findTravelResults = service.FindItems(WellKnownFolderName.Inbox, "System.Message.FromAddress:=*@chamberstravel.com AND System.Subject:Ticketed itinerary for*", new ItemView(10));

            service.LoadPropertiesForItems(findTravelResults, PropertySet.FirstClassProperties);

            log.Debug("Found " + findTravelResults.Items.Count + " mails");

            foreach (Item item in findTravelResults.Items)
            {
                log.Debug(item.Subject);

                parseMail(item);

            }

        }

        private static void parseMail(Item item)
        {
            //Is there air travel?
            List<Leg> legs = getLegsFromMail(item);

            foreach (Leg leg in legs)
            {
                log.Debug("Leg: " + leg);

            }
                
            
        }

        private static List<Leg> getLegsFromMail(Item item)
        {
            
            string body = item.Body;
           
            List<string> airSections = getAirSectionFromMail(body);
            List<Leg> legs = new List<Leg>();
            foreach (string section in airSections)
            {
                Leg t = new Leg();

                t.start = getStartfromBody(body);
                t.end = getTofromBody(body);
                DateTime departureDate = DateTime.Parse(getDepartureDateFromBody(section) + getDepartureTimeFromBody(body)
                    );
                t.departureDate = departureDate;

                legs.Add(t);
            }
            return legs;
        }

        private static string getDepartureTimeFromBody(string body)
        {
            string pattern = "(?<=^|Depart</b>:)[^><]+?(?=<|$)";
            foreach (Match match in Regex.Matches(body, pattern, RegexOptions.IgnoreCase))
            {
                return match.Value;
            }
            throw new InvalidOperationException();
        }

        private static List<string> getAirSectionFromMail(string body)
        {
            string airPattern = "<strong>AIR</strong>(.*)";
            
            List<string> sections = new List<string>();

            foreach (Match match in Regex.Matches(body, airPattern, RegexOptions.IgnoreCase))
            {
                sections.Add(match.Value);

            }
            return sections;
        }

        private static Place getStartfromBody(string body)
        {
            
            string toPattern = "(?<=^|From</b>:)[^><]+?(?=<|$)";
            foreach (Match match in Regex.Matches(body, toPattern, RegexOptions.IgnoreCase))
            {
                Place p = new Place(match.Value);

                return p;
            }
            throw new InvalidOperationException();
        }
        private static Place getTofromBody(string body)
        {

            string pattern = "(?<=^|To</b>:)[^><]+?(?=<|$)";
            foreach (Match match in Regex.Matches(body, pattern, RegexOptions.IgnoreCase))
            {
                Place p = new Place(match.Value);

                return p;
            }
            throw new InvalidOperationException();
        }
        
        private static string getDepartureDateFromBody(string p)
        {
            string datePattern = "(?<=^|<strong>)[^><]+?[0-9](?=<|$)";

            foreach (Match match in Regex.Matches(p, datePattern, RegexOptions.IgnoreCase))
            {
                return match.Value;
            }
            throw new InvalidOperationException();
            
        }
    }
}
