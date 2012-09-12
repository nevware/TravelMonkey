using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelMonkey
{
    class Leg
    {
       
        public DateTime departureDate { get; set; } 
       
        public Place start{get; set;} 
        public Place end{get; set;}

        public override string ToString()
        {
            return string.Format("Trip starting {0}, ending {1}, departing {2}", start, end, departureDate); 
        }
    }
}
