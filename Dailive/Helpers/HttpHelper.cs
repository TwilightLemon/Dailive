using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dailive.Helpers
{
    public static class HttpHelper
    {
        public static async Task<string> GetWebAsync(string url,Encoding e=null) {
            e ??= Encoding.UTF8;
            HttpWebRequest hw = (HttpWebRequest)WebRequest.Create(url);
            using var st = await hw.GetResponseAsync();
            using StreamReader sr =new StreamReader(st.GetResponseStream(),e);
            string sx= await sr.ReadToEndAsync();
            return sx;
        }
        public static async Task<string> GetWebGZIPAsync(string url, Encoding e = null)
        {
            e ??= Encoding.UTF8;
            HttpWebRequest hw = (HttpWebRequest)WebRequest.Create(url);
            using var st = await hw.GetResponseAsync();
            using GZipStream decodeStream = new GZipStream(st.GetResponseStream(), CompressionMode.Decompress);
            using StreamReader sr = new StreamReader(decodeStream, e);
            string sx = await sr.ReadToEndAsync();
            return sx;
        }
        /// 带上简单Header的Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetWebWithHeaderAsync(string url,bool useGZIP=false)
        {
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            hwr.Headers.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            hwr.Headers.Add("cookie", "__cfduid=dbb44318d9a3e9b739a80230a206d3d8f1592021136; io=xGIaLcCN2fsSC37pADUn");
            hwr.Headers.Add("sec-ch-ua", "\"\\\\Not\\\"A;Brand\";v=\"99\", \"Chromium\";v=\"84\", \"Microsoft Edge\";v=\"84\"");
            hwr.Headers.Add("sec-ch-ua-mobile", "?0");
            if (useGZIP)
                hwr.Headers.Add("accept-encoding", "gzip, deflate, br");
            hwr.Headers.Add("sec-fetch-dest", "document");
            hwr.Headers.Add("sec-fetch-mode", "navigate");
            hwr.Headers.Add("sec-fetch-site", "none");
            hwr.Headers.Add("sec-fetch-user", "?1");
            hwr.Headers.Add("upgrade-insecure-requests", "1");
            hwr.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.38 Safari/537.36 Edg/84.0.522.15";
            using WebResponse res = await hwr.GetResponseAsync();
            string st;
            if (useGZIP) {
                using GZipStream decodeStream = new GZipStream(res.GetResponseStream(), CompressionMode.Decompress);
                using StreamReader sr = new StreamReader(decodeStream, Encoding.UTF8);
                st = await sr.ReadToEndAsync();
            }
            else
            {
                using StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
                st = await sr.ReadToEndAsync();
            }
            return st;
        }
    }
}
