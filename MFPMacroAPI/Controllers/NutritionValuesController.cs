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

        //GET api/nutritionvalues/chuckgross/03-08-2015
        public HttpResponseMessage Get(string userName, DateTime date)
        {
            //date format is going to come in as MM/DD/YYYY, reformat it to YYYY-MM-DD
            var formattedDate = date.ToString("yyyy-MM-dd");
            var results = Scraper.Scraper.Scrape(userName, formattedDate);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(results.ToString()) };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "results.csv"
            };
            return responseMessage;
        }

        //GET api/nutritionvalues/chuckgross/
        public HttpResponseMessage GetToday(string userName)
        {
            var results = Scraper.Scraper.Scrape(userName);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(results.ToString()) };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "results.csv"
            };
            return responseMessage;
        }
    }
}
