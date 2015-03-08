using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace MFPMacroAPI.Controllers
{
    public class NutritionValuesController : ApiController
    {
        //GET api/nutritionvalues
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //GET api/nutritionvalues/chuckgross/2015-03-05
        public HttpResponseMessage Get(string userName, string date)
        {
            //todo check parameters
            //call screenscrape library with parameters, check the return type (error, etc)
            var results = Scraper.Scraper.Scrape(userName, date);
            return Request.CreateResponse(HttpStatusCode.OK, GetValues(results));
        }

        //GET api/nutritionvalues/chuckgross/
        public HttpResponseMessage GetToday(string userName)
        {
            //todo check parameters
            //call screenscrape library with parameters, check the return type (error, etc)
            var results = Scraper.Scraper.Scrape(userName);
            //return Request.CreateResponse(HttpStatusCode.OK, GetValues(results));
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(GetValues(results)) };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "results.csv"
            };
            return responseMessage;
        }

        //[HttpPost]
        //public HttpResponseMessage GenerateCSV(FieldParameters fieldParams)
        //{
        //    var output = new byte[] { };
        //    if (fieldParams != null)
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            this.SerializeSetting(fieldParams, stream);
        //            stream.Flush();
        //            output = stream.ToArray();
        //        }
        //    }
        //    var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(output) };
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = "File.csv"
        //    };
        //    return result;
        //}

        private string GetValues(Scraper.Scraper.NutritionRecord nutritrionRecord)
        {
            var values = new StringBuilder(nutritrionRecord.Calories);
            values.Append(",");
            values.Append(nutritrionRecord.Protein);
            values.Append(",");
            values.Append(nutritrionRecord.Fat);
            values.Append(",");
            values.Append(nutritrionRecord.Carbs);
            return values.ToString();
        }
    }
}
