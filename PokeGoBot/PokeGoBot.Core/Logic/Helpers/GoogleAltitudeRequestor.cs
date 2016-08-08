using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PokeGoBot.Core.Logic.Helpers
{
    public class GoogleAltitudeRequestor
    {
        public static double GetAltitude(double currentLat, double currentLong)
        {
            try
            {
                var formatedLat = currentLat.ToString().Replace(',', '.');
                var formatedLng = currentLong.ToString().Replace(',', '.');

                var request = (HttpWebRequest)WebRequest.Create(
                string.Format("https://maps.googleapis.com/maps/api/elevation/json?locations={0},{1}", formatedLat,
                    formatedLng));

                var response = (HttpWebResponse)request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream() ?? new MemoryStream()).ReadToEnd();

                var json = JObject.Parse(sr);
                return (double)json.SelectToken("results[0].elevation");
            }
            catch (Exception)
            {
                return 1;
            }
            
        }
    }
}
