using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Scraper
{
    public class Scraper
    {
        public static NutritionRecord Scrape(string userName, string date = null)
        {
            if (date == null) date = DateTime.Today.ToString("yyyy-MM-dd");
            //example: http://www.myfitnesspal.com/food/diary/chuckgross?date=2015-03-05
            var url = "http://www.myfitnesspal.com/food/diary/" + userName + "?date=" + date;
            string results;

            using (var client = new WebClient())
            {
                results = client.DownloadString(url);
            }

            var nutritionTable = GetNutritionTable(results);
            if (!nutritionTable.Any()) return new NutritionRecord { Calories = "0", Protein = "0", Fat = "0", Carbs = "0" };
            return GetNutritionRecord(nutritionTable);
        }

        private static NutritionRecord GetNutritionRecord(Dictionary<string, string> nutritionDictionary)
        {
            var nutritionRecord = new NutritionRecord
            {
                Protein = nutritionDictionary.FirstOrDefault(d => d.Key == "Protein").Value,
                Fat = nutritionDictionary.FirstOrDefault(d => d.Key == "Fat").Value,
                Carbs = nutritionDictionary.FirstOrDefault(d => d.Key == "Carbs").Value,
                Calories = nutritionDictionary.FirstOrDefault(d => d.Key == "Calories").Value
            };

            var calculatedCalories = GetCalculatedCalories(nutritionRecord);
            nutritionRecord.Calories = Math.Max(int.Parse(nutritionRecord.Calories.Replace(",", string.Empty)),
                int.Parse(calculatedCalories)).ToString();
            //always return the higher calories, 
            //this is to account for cases where there are alcohol calories as MFP does not track alcohol
            //as well as calorie counts being rounded down..calculating from macros are more accurate
            return nutritionRecord;
        }

        private static string GetCalculatedCalories(NutritionRecord nutritionRecord)
        {
            var proteinCals = int.Parse(nutritionRecord.Protein) * 4;
            var fatCals = int.Parse(nutritionRecord.Fat) * 9;
            var carbsCals = int.Parse(nutritionRecord.Carbs) * 4;
            var calculatedCalories = proteinCals + fatCals + carbsCals;
            return calculatedCalories.ToString();
        }

        private static Dictionary<string, string> GetNutritionTable(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            if (!IsPublic(document)) return new Dictionary<string, string>();

            //get the table we care about
            HtmlNode primaryTable = document.DocumentNode.Descendants("table").First(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "table0");

            //get the totalsRow
            //<tr class="total">
            //  <td class="first">Totals</td>
            //  <td>0</td>
            //  <td>0</td>
            //  <td>0</td>
            //  <td>0</td>
            //  <td>0</td>
            //<td class="empty"></td>
            //</tr>
            HtmlNode totalsRow = primaryTable.Descendants("tr")
                .First(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "total");
            HtmlNodeCollection totalCells = totalsRow.SelectNodes(".//td[not(@class='first') and not(@class='empty')]");
            //var totalCellsHtml = totalCells.Aggregate(string.Empty, (current, label) => current + label.OuterHtml);

            //get the HeaderRow
            //<tfoot>
            //  <tr>
            //    <td class="first"></td>
            //      <td class="alt">Calories</td>
            //      <td class="alt">Protein</td>
            //      <td class="alt">Carbs</td>
            //      <td class="alt">Fat</td>
            //      <td class="alt">Fiber</td>
            //    <td class="empty"></td>
            //  </tr>
            //</tfoot>
            HtmlNode headerRow = primaryTable.Descendants("tfoot")
                .First()
                .Descendants("tr")
                .First();

            HtmlNodeCollection headerCells = headerRow.SelectNodes(".//td[@class='alt']");
            //var headerCellsHtml = headerCells.Aggregate(string.Empty, (current, label) => current + label.OuterHtml);

            return BuildNutritionDictionary(headerCells, totalCells);
        }

        private static bool IsPublic(HtmlDocument document)
        {
            HtmlNode primaryDiv = document.DocumentNode.Descendants("div").First(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "block-1");
            return !primaryDiv.InnerHtml.Contains("This user is not allowing others to view his or her diary");
        }
        private static Dictionary<string, string> BuildNutritionDictionary(HtmlNodeCollection headerCells,
            HtmlNodeCollection totalCells)
        {
            var nutritionDictionary = new Dictionary<string, string>();
            var dictionaryKeys = new List<string>();
            foreach (var cell in headerCells)
            {
                dictionaryKeys.Add(cell.InnerText);
                nutritionDictionary.Add(cell.InnerText, string.Empty);
            }

            for (int i = 0; i < dictionaryKeys.Count; i++)
            {
                nutritionDictionary[dictionaryKeys[i]] = totalCells[i].InnerText;
            }

            return nutritionDictionary;
        }

        public class NutritionRecord
        {
            public string Calories { get; set; }
            public string Protein { get; set; }
            public string Fat { get; set; }
            public string Carbs { get; set; }
            //TODO compute calories from protein, fat, carbs. return higher number of calories
        }
    }
}
