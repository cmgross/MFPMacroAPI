using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace Scraper
{
    public class FitocracyScraper
    {
        const string BaseUrl = "https://www.fitocracy.com";

        public static List<DateTime> GetLast15WorkoutDates(string userName)
        {
            var loginResponse = Cacheable.Login();
            var userId = Cacheable.GetUserId(userName, loginResponse); //in Dauber, this will be a parameter stored in db when adding client's fito name, and cached tolookup?
            var document = GetActivityFeed(userId, loginResponse);
            //var document = DownloadFeed(userName);

            //<div class="stream-inner">
            //  <div class="stream_item clearfix stream-item-safe" data-ag-type="workout" id="stream_item_41562425">
            //      <div class="stream-item-headline">
            //          <a class="action_time gray_link" href="/entry/41562425/">2015-07-22T20:38:27</a>
            HtmlNodeCollection workouts = document.DocumentNode.SelectNodes("//a[contains(@class,'action_time gray_link')]");
            if (workouts == null) return new List<DateTime>();
            var dates = workouts.Select(workout => DateTime.Parse(workout.InnerHtml.Split('T')[0])).ToList();
            return dates;
        }

        private static HtmlDocument GetActivityFeed(string userId, CookieCollection cookies)
        {
            var cookieJar = new CookieContainer();
            cookieJar.Add(cookies);
            #region GetActivityForUser
            var activityUrl = BaseUrl + "/activity_stream/0/?user_id=" + userId + "&types=WORKOUT";
            var request = (HttpWebRequest)WebRequest.Create(activityUrl);
            request.CookieContainer = cookieJar;
            request.UserAgent = "Dauber";

            var activityPage = new HtmlDocument();

            using (var response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    activityPage.Load(stream);
                }
            }
            #endregion
            return activityPage;
        }

        internal static CookieCollection Login()
        {
            #region GetLogin
            const string loginUrl = BaseUrl + "/accounts/login/";
            var cookieJar = new CookieContainer();
            var request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.CookieContainer = cookieJar;
            request.UserAgent = "Dauber";
            var getResponse = request.GetResponse();
            var tokenCookie = request.CookieContainer.GetCookies(new Uri(BaseUrl))["csrftoken"];
            #endregion

            #region PostLogin
            request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.CookieContainer = cookieJar;
            request.Method = "POST";
            request.Referer = loginUrl;
            request.UserAgent = "Dauber";

            string username = ConfigurationManager.AppSettings["FitocracyUsername"];
            string password = ConfigurationManager.AppSettings["FitocracyPassword"];
            using (var stream = request.GetRequestStream())
            {
                var bytes = Encoding.UTF8.GetBytes(FormatArgs(new { csrfmiddlewaretoken = tokenCookie.Value, username = username, password = password, json = 1, is_username = 1 }));
                stream.Write(bytes, 0, bytes.Length);
            }

            var getLoginResponse = request.GetResponse() as HttpWebResponse;
            #endregion

            return getLoginResponse.Cookies;
        }

        internal static string GetUserId(string userName, CookieCollection cookies)
        {
            var cookieJar = new CookieContainer();
            cookieJar.Add(cookies);
            #region GetProfile - Gets userId
            var profileUrl = BaseUrl + "/profile/" + userName;
            var request = (HttpWebRequest)WebRequest.Create(profileUrl);
            request.CookieContainer = cookieJar;

            request.UserAgent = "Dauber";

            var profilePage = new HtmlDocument();

            using (var response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    profilePage.Load(stream);
                }
            }

            // <form id="duel_form" name="duel_form" method="POST" action="/set_duel/1867535/">
            //var userId = profilePage.DocumentNode
            //    .SelectSingleNode("//input[@type='hidden' and @name='profile_user']")
            //    .Attributes["value"].Value;

            var userId = profilePage.GetElementbyId("duel_form")
                .GetAttributeValue("action", string.Empty)
                .Replace("/set_duel/", string.Empty).Replace("/", string.Empty);
            #endregion

            return userId;
        }

        private static string FormatArgs(object args)
        {
            return string.Join(
                "&",
                args.GetType()
                    .GetProperties()
                    .Select(prop => string.Format("{0}={1}", HttpUtility.UrlEncode(prop.Name), HttpUtility.UrlEncode(prop.GetValue(args).ToString()))));
        }
    }
}
