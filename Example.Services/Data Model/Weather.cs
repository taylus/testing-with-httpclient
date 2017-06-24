﻿using Newtonsoft.Json;

namespace Example.Services
{
    //this class generated by pasting a response from https://weathers.co/api
    //(in Visual Studio, Edit -> Paste Special -> Paste JSON as Classes)

    public class Weather
    {
        public string apiVersion { get; set; }
        public WeatherData data { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class WeatherData
    {
        public string location { get; set; }
        public string temperature { get; set; }
        public string skytext { get; set; }
        public string humidity { get; set; }
        public string wind { get; set; }
        public string date { get; set; }
        public string day { get; set; }
    }
}
