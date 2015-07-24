using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Scraper;

namespace MFPMacroAPI.Controllers
{
    public class FitoController : ApiController
    {
        //GET api/fito/chuckgross/ or  //GET api/fito/chuckgross/07-22-2015
        public HttpResponseMessage Get(string userName, string date = null)
        {
            var selectedDate = date == null ? DateTime.Now : DateTime.Parse(date);
            var recentWorkouts = FitocracyScraper.GetLast15WorkoutDates(userName);
            //var workoutsInPastWeek = recentWorkouts.Where(w => w > (selectedDate.AddDays(-7))).ToList();
            //string results = string.Join(",", workoutsInPastWeek);
            var results = recentWorkouts.Count(w => w >= (selectedDate.AddDays(-7)));
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(results.ToString()) };
            return responseMessage;
        }
    }
}
