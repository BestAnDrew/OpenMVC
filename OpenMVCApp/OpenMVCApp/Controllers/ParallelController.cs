using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace OpenMVCApp.Controllers
{
    public class ParallelController : Controller
    {
        // GET: Parallel
        public async Task<ActionResult> Index()
        {

            var timer = new Stopwatch();
            timer.Start();


            for (int i = 0; i < 100; i++)
            {
                await Encrypt();
            }


            timer.Stop();
            ViewData["timer1"] = timer.Elapsed;


            timer.Reset();
            timer.Start();
            Parallel.For(0, 100, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount } , async t =>{
                await Encrypt();
            });
            timer.Stop();
            ViewData["timer2"] = timer.Elapsed;


            var timer3 = new Stopwatch();
            timer3.Start();

            RunDownloadWebSync();

            timer3.Stop();

            ViewData["timer3"] += $"total Execution time:{timer3.ElapsedMilliseconds.ToString()}";

            timer3.Reset();

            timer3.Start();

            await RunDownloadWebAsync();

            timer3.Stop();

            ViewData["timer4"] += $"total Execution time:{timer3.ElapsedMilliseconds.ToString()}";



            return View();
        }


        private void RunDownloadWebSync()
        {
            List<String> websiteCollection = getWebSite();

            foreach (var ws in websiteCollection)
            {
                WebSiteDataModel wsd = getWebSiteData(ws);
                ReportWebSiteInfo(wsd);
            }
        }

        private WebSiteDataModel getWebSiteData(String ws)
        {
            WebSiteDataModel wdsm = new WebSiteDataModel();

            WebClient wc = new WebClient();
            wdsm.WebSiteUri = ws;
            wdsm.WebSiteData = wc.DownloadString(new Uri(ws));

            return wdsm;
        }

        public void ReportWebSiteInfo(WebSiteDataModel wsd)
        {
            ViewData["timer3"] += $"{wsd.WebSiteUri} downloaded :{wsd.WebSiteData.Length} characters long.{Environment.NewLine}";
        }
        private List<String> getWebSite()
        {
            List<String> ls = new List<string>();
            ls.Add("https://www.baidu.com");
            ls.Add("https://www.google.com");
            ls.Add("https://www.Microsoft.com");
            ls.Add("https://www.codeproject.com");
            ls.Add("https://www.yahoo.com");
            ls.Add("https://www.Github.com");
            return ls;
        }

        private async Task RunDownloadWebAsync()
        {
            List<String> websiteCollection = getWebSiteAsync();
            List<Task<WebSiteDataModel>> tastks = new List<Task<WebSiteDataModel>>();

            foreach (var ws in websiteCollection)
            {
                tastks.Add(Task.Run(() => getWebSiteDataAsync(ws)));
            }

            var results = await Task.WhenAll(tastks);

            foreach (var items in results)
            {
                ReportWebSiteInfoAsync(items);
            }
        }

        private async Task<WebSiteDataModel> getWebSiteDataAsync(String ws)
        {
            WebSiteDataModel wdsm = new WebSiteDataModel();

            WebClient wc = new WebClient();
            wdsm.WebSiteUri = ws;
            wdsm.WebSiteData = await wc.DownloadStringTaskAsync(new Uri(ws));

            return wdsm;
        }

        public void ReportWebSiteInfoAsync(WebSiteDataModel wsd)
        {
            ViewData["timer4"] += $"{wsd.WebSiteUri} downloaded :{wsd.WebSiteData.Length} characters long.{Environment.NewLine}";
        }
        private List<String> getWebSiteAsync()
        {
            List<String> ls = new List<string>();
            ls.Add("https://www.baidu.com");
            ls.Add("https://www.google.com");
            ls.Add("https://www.Microsoft.com");
            ls.Add("https://www.codeproject.com");
            ls.Add("https://www.yahoo.com");
            ls.Add("https://www.Github.com");
            return ls;
        }


        public async static Task<String> Encrypt(String input = "TestTestTestTestTestTestTestTestTestTestTestTestTestTestTest")
        {
            
            var Value = "";
            using (Rijndael cryPt = Rijndael.Create())
            {
                cryPt.GenerateKey();
                cryPt.GenerateIV();

                ICryptoTransform transform = cryPt.CreateEncryptor();
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                    {
                        using (var wr = new StreamWriter(cs))
                        {
                           await wr.WriteAsync(input);
                        }
                    }
                    Value = System.Text.Encoding.UTF8.GetString(ms.ToArray() );
                }
            }
            return Value;
        }
    }
    public class WebSiteDataModel
    {
        public String WebSiteUri { get; set; }

        public String WebSiteData { get; set; }

    }
}