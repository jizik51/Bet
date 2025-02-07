using HtmlAgilityPack;
using PuppeteerSharp;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Text.Json;
using Bet.Models;

namespace Bet
{
    public class FindGamesController : Controller
    {
        static string sourceTimeZoneId = "Eastern Standard Time";
        static string targetTimeZoneId = "Russian Standard Time";
        TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneId);
        List<string> teams =
            new()
            {
                "bkn",
                "lac",
                "phi",
                "hou",
                "atl",
                "lal",
                "utah",
                "gs",
                "mil",
                "por",
                "cha",
                "det",
                "ind",
                "tor",
                "wsh",
                "chi",
                "bos",
                "cle",
                "mia",
                "den",
                "ny",
                "sac",
                "dal",
                "no",
                "sa",
                "min",
                "phx",
                "okc",
                "orl",
                "mem"
            };

        public async Task<List<Games>> GetGameList()
        {
            string content = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://www.espn.com/nba/schedule";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    content = await response.Content.ReadAsStringAsync();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }

            if (content is null)
                throw new Exception();

            string day = GetDay();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);

            var res = document.DocumentNode
                .SelectNodes("//div[contains(@class, 'ResponsiveTable')]")
                .Where(node => node.FirstChild.InnerText == day)
                ?.ToList();

            // var listGames = new List<List<string>>();
            List<Games> listGames = new List<Games>();

            try
            {
                var str = res?[0].InnerHtml;
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(str);

                var teamName = htmlDocument.DocumentNode
                    .SelectNodes("//span[contains(@class, 'Table__Team')]")
                    .Select(x => x.InnerText)
                    ?.ToList();

                var time = htmlDocument.DocumentNode
                    .SelectNodes("//td[contains(@class, 'date__col Table__TD')]")
                    .Select(x => x.InnerText)
                    .ToList();

                List<string> lTime = new() { };

                foreach (var item in time)
                {
                    DateTime usTime = DateTime.ParseExact(
                        item.Replace(" ", ""),
                        "h:mmtt",
                        CultureInfo.InvariantCulture
                    );

                    DateTime usTimeWithZone = TimeZoneInfo.ConvertTime(
                        usTime,
                        sourceTimeZone,
                        targetTimeZone
                    );

                    lTime.Add($"{usTimeWithZone.ToString("h:mm tt")}AM");
                }

                for (var i = 0; i < teamName.Count; i += 2)
                {
                    string teamNameFirst = teamName[i].Replace(" ", "");
                    string teamNameSecond = teamName[i + 1].Replace(" ", "");

                    var teamFirst = (int)Enum.Parse<GamesEnum>(teamNameFirst);
                    var teamSecond = (int)Enum.Parse<GamesEnum>(teamNameSecond);

                    listGames.Add(
                        new Games(
                            teamNameFirst,
                            teamNameSecond,
                            null,
                            teams[teamFirst],
                            teams[teamSecond]
                        )
                    );
                }

                //fillTime
                for (int i = 0; i < listGames.Count; i++) // 0 1 2 3..
                {
                    for (int j = i; j <= i; j++)
                    {
                        listGames[i].Time = lTime[j];
                    }
                }

                WriteInJson(listGames);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            //Send
            return listGames;
        }

        private static string GetDay()
        {
            var day = DateTime.Today; // .AddDays(-1)
            string res = string.Empty;
            try
            {
                DateTime dt = DateTime.ParseExact(
                    day.Date.ToString(),
                    "dd.MM.yyyy H:mm:ss",
                    CultureInfo.CreateSpecificCulture("en-US")
                );

                res = dt.Date.ToString("dddd, MMMM d, yyyy ", new CultureInfo("en-US"));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }

            if (res is null)
                throw new Exception();

            return res;
        }

        private void WriteInJson(List<Games> listGames)
        {
            int game = 0;

            System.IO.File.WriteAllText("C:/Users/troll/source/repos/Bet/wwwroot/game.json", "");
            foreach (var item in listGames)
            {
                string json = JsonSerializer.Serialize<Games>(item);
                string newJson = $"\"game{game}\" : {json},";
                game++;
                System.IO.File.AppendAllText(
                    "C:/Users/troll/source/repos/Bet/wwwroot/game.json",
                    newJson
                );
            }
            string js = System.IO.File.ReadAllText(
                "C:/Users/troll/source/repos/Bet/wwwroot/game.json"
            );

            js = js.Insert(0, "{");
            js = js.Remove(js.Length - 1, 1);
            js += "}";

            System.IO.File.WriteAllText("C:/Users/troll/source/repos/Bet/wwwroot/game.json", js);
        }
    }

    // public class GameInfo
    // {
    //     public string? TeamFirst { get; private set; }
    //     public string? TeamFirstLogoSrc { get; private set; }
    //     public string? TeamSecond { get; private set; }
    //     public string? TeamSecondLogoSrc { get; private set; }
    //     public string? Time { get; set; }

    //     public GameInfo(
    //         string teamFirst,
    //         string teamSecond,
    //         string? time,
    //         string? teamFirstLogoSrc,
    //         string? teamSecondLogoSrc
    //     )
    //     {
    //         TeamFirst = teamFirst;
    //         TeamSecond = teamSecond;
    //         Time = time;
    //         TeamFirstLogoSrc = teamFirstLogoSrc;
    //         TeamSecondLogoSrc = teamSecondLogoSrc;
    //     }
    // }

    enum GamesEnum
    {
        Brooklyn = 0,
        LA = 1,
        Philadelphia = 2,
        Houston = 3,
        Atlanta = 4,
        LosAngeles = 5,
        Utah = 6,
        GoldenState = 7,
        Milwaukee = 8,
        Portland = 9,
        Charlotte = 10,
        Detroit = 11,
        Indiana = 12,
        Toronto = 13,
        Washington = 14,
        Chicago = 15,
        Boston = 16,
        Cleveland = 17,
        Miami = 18,
        Denver = 19,
        NewYork = 20,
        Sacramento = 21,
        Dallas = 22,
        NewOrleans = 23,
        SanAntonio = 24,
        Minnesota = 25,
        Phoenix = 26,
        OklahomaCity = 27,
        Orlando = 28,
        Memphis = 29
    }
}
