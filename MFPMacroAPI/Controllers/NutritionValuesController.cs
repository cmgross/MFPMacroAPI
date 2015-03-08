using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            return Request.CreateResponse(HttpStatusCode.OK, GetValues(results));
        }

        private List<string> GetValues(Scraper.Scraper.NutritionRecord nutritrionRecord)
        {
            var values = new List<string>
            {
                nutritrionRecord.Calories,
                nutritrionRecord.Protein,
                nutritrionRecord.Fat,
                nutritrionRecord.Carbs
            };
            return values;
        }
    }
}
