using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MFPMacroAPI.Controllers
{
    public class CaloriesController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //GET api/nutritionvalues/chuckgross/03-08-2015
        public HttpResponseMessage Get(string userName, DateTime date)
        {
            //date format is going to come in as MM/DD/YYYY, reformat it to YYYY-MM-DD
            var formattedDate = date.ToString("yyyy-MM-dd");
            var results = Scraper.Scraper.ScrapeCalories(userName, formattedDate);
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(results.ToString()) };
            return responseMessage;
        }
    }
}
