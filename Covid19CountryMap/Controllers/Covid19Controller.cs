using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Covid19CountryMap.Models;

namespace Covid19CountryMap.Controllers
{
    public class Covid19Controller : Controller
    {
        // GET: Covid19
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Map()
        {
            return View();
        }

        public JsonResult Info(string LatLng)
        {

            string result = string.Empty;
            string countryName = string.Empty;
            CovidCountryInfo covidInfo = new CovidCountryInfo();
            //Not elegant...
            decimal lat = decimal.Parse(LatLng.Substring(7, LatLng.IndexOf(",") - 9));
            decimal lng = decimal.Parse(LatLng.Substring(LatLng.IndexOf(",")+2, LatLng.Length-(LatLng.IndexOf(",") + 2)-1));
            //Get Country Name
            countryName = CountryName(lat, lng);

            //Get Covid info
            covidInfo = GetCountryInfo(countryName);

            return Json(covidInfo, JsonRequestBehavior.AllowGet);

        }

        public CovidCountryInfo GetCountryInfo(string Country)
        {
            CovidCountryInfo covidCountryInfo = new CovidCountryInfo();
            string url = string.Format("{0}{1}", Properties.Settings.Default.UrlCovid, Country);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("x-rapidapi-key", "eddcd98038mshda75c2806509ac2p1ce17bjsn0f76aaf64056");
            request.Headers.Add("x-rapidapi-host", "coronavirus-map.p.rapidapi.com");
            request.ContentType = "application/json";
            request.Accept = "application/json";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();

                            dynamic jsonResponse = JsonConvert.DeserializeObject(JToken.Parse(responseBody).ToString());
                            covidCountryInfo.total_cases = jsonResponse["data"]["summary"]["total_cases"];
                            covidCountryInfo.active_cases = jsonResponse["data"]["summary"]["active_cases"];
                            covidCountryInfo.deaths = jsonResponse["data"]["summary"]["deaths"];
                            covidCountryInfo.recovered = jsonResponse["data"]["summary"]["recovered"];

                        }
                    }
                }
            }
            catch
            {
                // Handle error
            }

            return covidCountryInfo;

        }

        public string CountryName(decimal Lat, decimal Lng)
        {
            string result = string.Empty;
            string url = string.Format("{0}latitude={1}&longitude={2}&localityLanguage=en", Properties.Settings.Default.UrlCountry, Lat, Lng);

            Dictionary<string, string> convertCountry = new Dictionary<string, string>();
            convertCountry.Add("united states of america", "usa");
            convertCountry.Add("united kingdom", "uk");

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {                        
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();

                            dynamic jsonResponse = JsonConvert.DeserializeObject(JToken.Parse(responseBody).ToString());
                            result = jsonResponse["countryName"];

                            result = result.ToLower();

                            if (convertCountry.ContainsKey(result))
                            {
                                convertCountry.TryGetValue(result, out result);
                            }                            

                        }
                    }
                }
            }
            catch
            {
                // Handle error
            }

            return result;
        }

    }
}