using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dailive.Helpers
{
    public class WeatherApi
    {
        public class Weather
        {
            /// <summary>
            /// 温度
            /// </summary>
            public string tmp;
            /// <summary>
            /// 风向
            /// </summary>
            public string Wind;
            /// <summary>
            /// 风的等级
            /// </summary>
            public string WindLevel;
            /// <summary>
            /// 描述  sunny rainy windy
            /// </summary>
            public string txt;
            /// <summary>
            /// 代号
            /// </summary>
            public string code;
            /// <summary>
            /// 空气质量 指数和等级
            /// </summary>
            public string AqiValue, AqiLevel;
            /// <summary>
            /// 紫外线强度
            /// </summary>
            public string UV;
            /// <summary>
            /// 相对湿度
            /// </summary>
            public string hum;
            /// <summary>
            /// 小语
            /// </summary>
            public string desc;
            public List<DailyWeather> Daily;
            public List<HourWeather> Hours;

        }
        public class HourWeather
        {
            public string Date;
            public string code;
            public string Tmp;
            public string wind;
        }
        public class DailyWeather
        {
            public string Date;
            public string code;
            public string Tmp;
            public string uv;
            public string wind;
        }
        public class City {
            public string name;
            public string cid;
            public string hid;
            public string admin_area;
        }

        public static string GetImgUrl(string code)=> "https://a.hecdn.net/img/plugin/190516/icon/c/"+code+"d.png";
        public static async Task<City> GetPositionByIpAsync() {
            string url = "https://restapi.amap.com/v3/ip?key=d1a7a5151bc3b5f7de34c34f824da3fe&s=rsv3&platform=JS&logversion=2.0&appname=heweather&csid=C6F0C296-EE9F-4D99-AC19-9C23B05445E9&sdkversion=1.4.15";
            string json = await HttpHelper.GetWebWithHeaderAsync(url);
            JObject obj = JObject.Parse(json);
            string adcode = obj["adcode"].ToString();
            JObject find = JObject.Parse(await HttpHelper.GetWebWithHeaderAsync("https://search.heweather.net/find?key=b130450ee32047d882cabe9929e7ec22&group=cn&lang=zh&location=" + adcode));
            var data = find["HeWeather6"][0]["basic"][0];
            string cid = data["cid"].ToString();
            string hid = await HttpHelper.GetWebWithHeaderAsync("https://www.heweather.com/v1/current/hid/"+cid+".html");
            return new City(){
                name = data["location"].ToString(),
                cid = cid,
                admin_area = data["admin_area"].ToString(),
                hid = hid
            };
        }

        public static async Task<List<City>> SearchCitiesByNameAsync(string name) {
            string json = await HttpHelper.GetWebWithHeaderAsync($"https://search.heweather.net/find?key=500e448f9e6c440aab8aa50f14220331&location={HttpUtility.UrlDecode(name)}&group=overseas,cn&lang=zh");
            var data = JObject.Parse(json)["HeWeather6"][0]["basic"];
            List<City> c = new List<City>();
            foreach (var a in data) {
                City city = new City() { 
                name=a["location"].ToString(),
                cid=a["cid"].ToString(),
                admin_area=a["admin_area"].ToString(),
                hid=null
                };
                c.Add(city);
            }
            return c;
        }
        public static async Task<Weather> GetWeatherAsync(City c) {
            c.hid ??= await HttpHelper.GetWebWithHeaderAsync("https://www.heweather.com/v1/current/hid/" + c.cid + ".html");
            var o = JObject.Parse(await HttpHelper.GetWebWithHeaderAsync("https://www.heweather.com/v1/current/condition/" + c.hid + ".html"));
            Weather data = new Weather()
            {
                code = o["data"]["code"].ToString(),
                txt= o["data"]["txt"].ToString(),
                tmp= o["data"]["tmp"].ToString(),
                Wind= o["data"]["wdir"].ToString(),
                WindLevel = o["data"]["wsc"].ToString(),
                UV= o["data"]["uv"].ToString(),
                hum= o["data"]["hum"].ToString(),
                AqiValue= o["data"]["aqi"].ToString(),
                AqiLevel= o["data"]["aqiLevel"].ToString(),
                desc= o["data"]["phrase"].ToString(),
            };
            var hours = o["data"]["hours"];
            var hdata = new List<HourWeather>();
            foreach (var h in hours) {
                hdata.Add(new HourWeather() {
                    Date = h["time"].ToString(),
                    code = h["code"].ToString(),
                    Tmp = h["tmp"].ToString(),
                    wind=h["sc"].ToString()
                });
            }
            data.Hours = hdata;
            JObject days = JObject.Parse(await HttpHelper.GetWebWithHeaderAsync($"https://devapi.heweather.net/v7/weather/3" +
                $"d?location={c.cid}&key=500e448f9e6c440aab8aa50f14220331",true));
            List<DailyWeather> Daily = new List<DailyWeather>();
            var d = days["daily"];
            foreach (var a in d) {
                Daily.Add(new DailyWeather() {
                    code = a["iconDay"].ToString(),
                    Date = a["fxDate"].ToString(),
                    Tmp = a["tempMin"] + " - " + a["tempMax"],
                    uv=a["uvIndex"].ToString(),
                    wind=a["windDirDay"]+"  "+a["windScaleDay"]+"级"
                });
            }
            data.Daily = Daily;
            return data;
        }
    }
}
