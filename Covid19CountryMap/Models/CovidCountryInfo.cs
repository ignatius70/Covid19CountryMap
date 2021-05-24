using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Covid19CountryMap.Models
{
    public class CovidCountryInfo
    {
        public int total_cases { get; set; }
        public int active_cases { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }

    }
}