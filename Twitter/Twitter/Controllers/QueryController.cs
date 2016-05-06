using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Twitter.Models;
namespace Twitter.Controllers
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    public class QueryController : Controller
    {
        // GET: Query

        public ActionResult Index()
        {
            return View();
        }
        string result;
        string tw;

        public async Task<ActionResult> Welcome(string lname)
        {
            tw = lname;
            result = await InvokeRequestResponseService(lname).ConfigureAwait(false); ;
            if (result != "")
            {
                ViewBag.Error = false;
                var resultss = JsonConvert.DeserializeObject<RootObject>(result);
                if (resultss.Results.output1.value.Values[0][0] == "negative")
                {
                    resultss.Results.output1.value.Values[0][1] =( 1 - Convert.ToDouble( resultss.Results.output1.value.Values[0][1])).ToString();
                }
                ViewBag.Tweet = tw;
                ViewBag.Response = string.Format("{0} with Probability {1:0.0000}", resultss.Results.output1.value.Values[0][0], resultss.Results.output1.value.Values[0][1]);
            }else
            {
                ViewBag.Error = true;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is About Artificial Intelligence Cource Project, Twitter Sentiment Data Analysis";

            return View();
        }
        async Task<string> InvokeRequestResponseService(string tweet)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"sentiment_label", "tweet_text"},
                                Values = new string[,] {  { "0", tweet } }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "5insn0xEIFCYXaFhfWmpPZ4Yt60DXgXtoW73KurxlkZ98H5/jWPiHKcuDnuw3fnlBs8JAZEs1Y15ZYMLb53ZCw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/d8b7f0bf4d2e4a7f95fc241fcd2792fd/services/7f8db9ba819b4935ab8418c9c8d10090/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    
                    return result;
                    //Console.ReadKey();
                }
                else
                {
                    //Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                   // Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return "Fail";
                }
            }
        }
    }
}