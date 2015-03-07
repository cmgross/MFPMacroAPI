using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MFPMacroAPI.Controllers
{
    public class NutritionController : ApiController
    {
        //GET api/nutrition
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //GET api/nutrition/chuckgross/2015-03-05
        public HttpResponseMessage Get(string userName, string date)
        {
            //todo check parameters
            //call screenscrape library with parameters, check the return type (error, etc)
            var results = Scraper.Scraper.Scrape(userName, date);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        //GET api/nutrition/chuckgross/
        public HttpResponseMessage GetToday(string userName)
        {
            //todo check parameters
            //call screenscrape library with parameters, check the return type (error, etc)
            var results = Scraper.Scraper.Scrape(userName);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        //todo get methods for calories/protein/fat/carbs, cache entire result for 1 hr and return from the cache
    }
}
