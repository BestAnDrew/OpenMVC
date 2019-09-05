using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Security.Cryptography;

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

            return View();
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
}