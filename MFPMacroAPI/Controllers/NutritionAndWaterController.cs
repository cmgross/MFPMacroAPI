using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MFPMacroAPI.Controllers
{
    public class NutritionAndWaterController : ApiController
    {
        //GET api/nutritionandwater
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //GET api/nutritionandwater/chuckgross/01-03-2016
        public HttpResponseMessage Get(string userName, DateTime date)
        {
            //date format is going to come in as MM/DD/YYYY, reformat it to YYYY-MM-DD
            var formattedDate = date.ToString("yyyy-MM-dd");
            var results = Scraper.Scraper.ScrapeMacrosWithWater(userName, formattedDate);
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
