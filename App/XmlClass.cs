/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Net.Http;

namespace Xml2CSharp
{
    public class xmlway
    {
        public static async Task<CoordAddressResult> GetWeather(String Location, String Location2)
        {
            string link = "http://api.avatardata.cn/CoordAddress/Lookup?key=9268d181665640499b8fa1a0f44ad0da&lat=" + Location + "&dtype=XML&lon=" + Location2;
            //string link = "http://api.avatardata.cn/CoordAddress/Lookup?key=9268d181665640499b8fa1a0f44ad0da&lat=40.0492023635&dtype=XML&lon=116.2955249742";


            var http = new HttpClient();
            //var response = await http.GetAsync("http://api.map.baidu.com/telematics/v3/weather?location=%E6%AD%A6%E6%B1%89&ak=8IoIaU655sQrs95uMWRWPDIa");
            var response = await http.GetAsync(link);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(CoordAddressResult));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (CoordAddressResult)serializer.Deserialize(ms);
            return data;
        }
    }

    [XmlRoot(ElementName = "result")]
    public class Result
    {
        [XmlElement(ElementName = "country")]
        public string Country { get; set; }
        [XmlElement(ElementName = "country_code")]
        public string Country_code { get; set; }
        [XmlElement(ElementName = "province")]
        public string Province { get; set; }
        [XmlElement(ElementName = "city")]
        public string City { get; set; }
        [XmlElement(ElementName = "district")]
        public string District { get; set; }
        [XmlElement(ElementName = "street")]
        public string Street { get; set; }
        [XmlElement(ElementName = "street_number")]
        public string Street_number { get; set; }
    }

    [XmlRoot(ElementName = "CoordAddressResult")]
    public class CoordAddressResult
    {
        [XmlElement(ElementName = "error_code")]
        public string Error_code { get; set; }
        [XmlElement(ElementName = "reason")]
        public string Reason { get; set; }
        [XmlElement(ElementName = "result")]
        public Result Result { get; set; }
    }

}