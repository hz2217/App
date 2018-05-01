using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    class OpenGeoProxy
    {
        public async static Task<RootObject> GetGeoInformation(String str)
        {
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri("http://api.avatardata.cn/Weather/Query?key=fa848569705941a1abdf5c83a6e30f6c&cityname=" + str);
            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

            var serializer = new DataContractJsonSerializer(typeof(RootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(httpResponseBody));
            RootObject data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }

    [DataContract]
    public class Wind
    {
        [DataMember]
        public string windspeed { get; set; }
        [DataMember]
        public string direct { get; set; }
        [DataMember]
        public string power { get; set; }
        [DataMember]
        public string offset { get; set; }
    }
    [DataContract]
    public class Weather
    {
        [DataMember]
        public string humidity { get; set; }
        [DataMember]
        public string img { get; set; }
        [DataMember]
        public string info { get; set; }
        [DataMember]
        public string temperature { get; set; }
    }
    [DataContract]
    public class Realtime
    {
        [DataMember]
        public Wind wind { get; set; }
        [DataMember]
        public string time { get; set; }
        [DataMember]
        public Weather weather { get; set; }
        [DataMember]
        public string dataUptime { get; set; }
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public string city_code { get; set; }
        [DataMember]
        public string city_name { get; set; }
        [DataMember]
        public string week { get; set; }
        [DataMember]
        public string moon { get; set; }
    }
    [DataContract]
    public class Info
    {
        [DataMember]
        public List<string> kongtiao { get; set; }
        [DataMember]
        public List<string> yundong { get; set; }
        [DataMember]
        public List<string> ziwaixian { get; set; }
        [DataMember]
        public List<string> ganmao { get; set; }
        [DataMember]
        public List<string> xiche { get; set; }
        [DataMember]
        public object wuran { get; set; }
        [DataMember]
        public List<string> chuanyi { get; set; }
    }
    [DataContract]
    public class Life
    {
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public Info info { get; set; }
    }
    [DataContract]
    public class Info2
    {
        [DataMember]
        public List<string> dawn { get; set; }
        [DataMember]
        public List<string> day { get; set; }
        [DataMember]
        public List<string> night { get; set; }
    }
    [DataContract]
    public class Weather2
    {
        [DataMember]
        public string date { get; set; }
        [DataMember]
        public string week { get; set; }
        [DataMember]
        public string nongli { get; set; }
        [DataMember]
        public Info2 info { get; set; }
    }
    [DataContract]
    public class Pm252
    {
        [DataMember]
        public string curPm { get; set; }
        [DataMember]
        public string pm25 { get; set; }
        [DataMember]
        public string pm10 { get; set; }
        [DataMember]
        public string level { get; set; }
        [DataMember]
        public string quality { get; set; }
        [DataMember]
        public string des { get; set; }
    }
    [DataContract]
    public class Pm25
    {
        [DataMember]
        public string key { get; set; }
        [DataMember]
        public object show_desc { get; set; }
        [DataMember]
        public Pm252 pm25 { get; set; }
        [DataMember]
        public string dateTime { get; set; }
        [DataMember]
        public string cityName { get; set; }
    }
    [DataContract]
    public class Result
    {
        [DataMember]
        public Realtime realtime { get; set; }
        [DataMember]
        public Life life { get; set; }
        [DataMember]
        public List<Weather2> weather { get; set; }
        [DataMember]
        public Pm25 pm25 { get; set; }
        [DataMember]
        public int isForeign { get; set; }
    }
    [DataContract]
    public class RootObject
    {
        [DataMember]
        public Result result { get; set; }
        [DataMember]
        public int error_code { get; set; }
        [DataMember]
        public string reason { get; set; }
    }
}
