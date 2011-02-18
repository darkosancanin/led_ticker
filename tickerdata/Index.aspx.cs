using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Ticker
{
    public partial class _Index : System.Web.UI.Page
    {
        private static int MAX_CHARS = 500;

        protected void Page_Load(object sender, EventArgs e)
        {
            string data;
            try
            {
                data = GetData();
                if(data.Length > MAX_CHARS)
                {
                    data = data.Substring(0, MAX_CHARS - 6) + "...   ";
                }
            }
            catch (Exception ex)
            {
                if (Request["showerror"] == "true")
                    data = String.Format("Error retrieving data.<br/>{0}<br/>{1}", ex.Message, ex.StackTrace);
                else
                    data = "Error retrieving data.";
            }
            Response.Write(data);
        }

        private string GetData()
        {
            var option = Request["option"];
            if (option == "0" || option == "menu")
                return GetMenu();
            if (option == "1")
                return GetNBAScores();
            if (option == "2")
                return GetNBAHeadlines();
            if (option == "3")
                return GetSydneyWeather();
            if (option == "4")
                return GetCampbelltownWeather();
            if (option == "5")
                return GetFinanceData();
            if (option == "6")
                return GetAustralianSportsNews();

            throw new ApplicationException(String.Format("Unknown option requested: {0}", option));
        }

        private static string GetAustralianSportsNews()
        {
            var html = GetHtml("http://www.foxsports.com.au");
            var latestNews = Regex.Match(html, "<h3 class=\"heading\">Latest News</h3>(.*?)<!-- // .module-content -->", RegexOptions.Singleline).ToString();
            var newsLines = Regex.Matches(latestNews, "<li(.*?)</li>", RegexOptions.Singleline);
            var data = "";
            foreach (var newsLine in newsLines)
            {
                var sport = Regex.Match(newsLine.ToString(), "<strong>(?<val>(.*?))</strong>", RegexOptions.Singleline).Groups["val"].ToString();
                var headline = Regex.Match(newsLine.ToString(), "</strong>(?<val>(.*?))</a>", RegexOptions.Singleline).Groups["val"].ToString();
                headline = headline.Replace("&#039;", "").Replace("&#39;", "").Replace("&#133;", "");
                data += sport + headline + "    ";
            }
            return data;
        }

        private static string GetMenu()
        {
            return "1.NBA Scores   2.NBA Headlines   3.Sydney Weather   4.Campbelltown Weather   5.Finance   6.Australian Sports News";
        }

        private static string GetFinanceData()
        {
            var html = GetHtml("http://au.finance.yahoo.com");
            var allords = Regex.Match(html, "id=\"yfs_l10_\\^aord\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var allords_point_difference = Regex.Match(html, "id=\"yfs_c10_\\^aord\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var allords_percentage_difference = Regex.Match(html, "id=\"yfs_pp0_\\^aord\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var sp200 = Regex.Match(html, "id=\"yfs_l10_\\^axjo\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var sp200_point_difference = Regex.Match(html, "id=\"yfs_c10_\\^axjo\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var sp200_percentage_difference = Regex.Match(html, "id=\"yfs_pp0_\\^axjo\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var aud_to_usd = Regex.Match(html, "id=\"yfs_l10_audusd=x\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var aud_to_usd_change = Regex.Match(html, "id=\"yfs_c10_audusd=x\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var aud_to_gbp = Regex.Match(html, "id=\"yfs_l10_audgbp=x\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var aud_to_gbp_change = Regex.Match(html, "id=\"yfs_c10_audgbp=x\">(?<val>(.*?))</span>", RegexOptions.Singleline).Groups["val"].ToString();
            var data = "All Ords  " + allords + "  " + allords_point_difference + "  " + allords_percentage_difference  + "    ";
            data += "S&P ASX 200  " + sp200 + "  " + sp200_point_difference + "  " + sp200_percentage_difference + "    ";
            data += "AUD to USD  " + aud_to_usd + "  " + aud_to_usd_change + "    ";
            data += "AUD to GBP  " + aud_to_gbp + "  " + aud_to_gbp_change + "    ";
            return data;
        }

        private static string GetCampbelltownWeather()
        {
            return "Sydney Weather - " + GetWeather("http://www.weather.com.au/nsw/sydney");
        }

        private static string GetSydneyWeather()
        {
            return "Campbelltown Weather - " + GetWeather("http://www.weather.com.au/nsw/campbelltown");
        }

        private static string GetWeather(string url)
        {
            var html = GetHtml(url);
            var weatherData = Regex.Match(html, "<tbody>(.*?)</tbody>", RegexOptions.Singleline).ToString();
            var weatherlines = Regex.Matches(weatherData, "<tr>(.*?)</tr>", RegexOptions.Singleline);
            var data = "";
            foreach (var weatherline in weatherlines)
            {
                var day = Regex.Match(weatherline.ToString(), "<td class=\"day\">(?<val>(.*?))</td>", RegexOptions.Singleline).Groups["val"].ToString();
                var forecast = Regex.Match(weatherline.ToString(), "<td class=\"forecast\">(?<val>(.*?))</td>", RegexOptions.Singleline).Groups["val"].ToString();
                var min = Regex.Match(weatherline.ToString(), "<td class=\"min\">(?<val>(.*?))</td>", RegexOptions.Singleline).Groups["val"].ToString();
                var max = Regex.Match(weatherline.ToString(), "<td class=\"max\">(?<val>(.*?))</td>", RegexOptions.Singleline).Groups["val"].ToString();
                data += day + " " + forecast + " min " + min + " max " + max + "   ";
            }
            return data;
        }

        private static string GetNBAHeadlines()
        {
            var html = GetHtml("http://sports.yahoo.com/nba");
            var headlineData = Regex.Match(html, "Top Stories(.*?)</ul>", RegexOptions.Singleline).ToString();
            var headlines = Regex.Matches(headlineData, "<li>(.*?)</li>", RegexOptions.Singleline);
            var data = "";
            foreach (var headline in headlines)
            {
                var headlineTitle = Regex.Match(headline.ToString(), "...\" >(?<val>(.*?))</a>", RegexOptions.Singleline).Groups["val"].ToString();
                headlineTitle = headlineTitle.Replace("&#039;", "").Replace("&#39;", "").Replace("&#133;", "");
                data += headlineTitle + "    ";
            }
            return data;
        }

        private static string GetNBAScores()
        {
            var html = GetHtml("http://www.nba.com/games/game_component/dynamic/simple_scoreboard.xml");
            var games = Regex.Matches(html, "<game(.*?)</game>", RegexOptions.Singleline);
            var data = "";
            foreach (var game in games)
            {
                var period = Regex.Match(game.ToString(), "gstattxt=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString();
                var gametime = Regex.Match(game.ToString(), "clk=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString();
                var hometeamline = Regex.Match(game.ToString(), "<htm(.*?)/>", RegexOptions.Singleline).ToString();
                var hometeamname = GetLastElementFromNBAScoreLine(Regex.Match(hometeamline, "tm=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString());
                var hometeamscore = GetLastElementFromNBAScoreLine(Regex.Match(hometeamline, "scr=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString());
                var visitingteamline = Regex.Match(game.ToString(), "<vtm(.*?)/>", RegexOptions.Singleline).ToString();
                var visitingteamname = GetLastElementFromNBAScoreLine(Regex.Match(visitingteamline, "tm=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString());
                var visitingteamscore = GetLastElementFromNBAScoreLine(Regex.Match(visitingteamline, "scr=\"(?<val>(.*?))\"", RegexOptions.Singleline).Groups["val"].ToString());
                data += hometeamname + " " + hometeamscore + " " + visitingteamname + " " + visitingteamscore + " " + period + " ";
                if (!String.IsNullOrEmpty(gametime))
                    data += gametime + " ";
                data += "    ";
            }
            return data;
        }

        //returns the last element in the nba score line, eg. from "123|CHA|343|TUR" it returns "TUR"
        private static string GetLastElementFromNBAScoreLine(string val)
        {
            return val.Substring(val.LastIndexOf('|') + 1, val.Length - val.LastIndexOf('|') - 1);
        }

        private static string GetHtml(string url)
        {
            return new WebClient().DownloadString(url);
        }
    }
}
