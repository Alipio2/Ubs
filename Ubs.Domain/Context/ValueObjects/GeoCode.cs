using System;
using System.Collections.Generic;
using System.Text;

namespace Ubs.Domain.Context.ValueObjects
{
    public class GeoCode
    {
        public decimal Lat { get;  private set; }
        public decimal Log { get; private set; }

        
        public GeoCode(decimal lat, decimal log)
        {
            Lat = lat;
            Log = log;
        }
    }
}
