using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelMonkey
{
    class Place
    {
        
        private string city;
        private string country;

        public Place(string p)
        {
            string[] elements = p.Split(new char[] {','});
            city = elements[0];
            country = elements[1];
        }

        public string City
        {
            get
            {
                return this.city;
            }
        }
        public string Country
        {
            get{
                return this.country;
            }
        }
        public override string ToString()
        {
            return city + " - " + country;
        }
    }
}
