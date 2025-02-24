using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bet;
using HtmlAgilityPack;

namespace Services;

public class StatsServices
{
    public async Task<List<List<string>>> GetStreakAsync(string FirstTeam, string SecondTeam)
    {
        string content = string.Empty;
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = "https://www.espn.com/nba/standings/_/sort/gamesbehind/dir/asc";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        string fistTemaLastFive = await GetLastFiveGames(content, FirstTeam.ToLower());
        string secondTeamLastFive = await GetLastFiveGames(content, SecondTeam.ToLower());

        if (content is null)
            throw new Exception();

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(content);

        string firstTeamName = GetGameName(FirstTeam, document);
        string secondTeamName = GetGameName(SecondTeam, document);

        var numberFirst = FindNumber(firstTeamName, document);
        var numberSecond = FindNumber(secondTeamName, document);

        var nodeFirst = FindNode(document, numberFirst);
        var nodeSecond = FindNode(document, numberSecond);

        List<string> statFirst = GetAllStatsAsync(
            firstTeamName,
            nodeFirst,
            content,
            FirstTeam
        ).Result;
        List<string> statSecond = GetAllStatsAsync(
            secondTeamName,
            nodeSecond,
            content,
            SecondTeam
        ).Result;

        List<List<string>> stats = new() { statFirst, statSecond };

        return stats;
    }

    private static async Task<string> GetLastFiveGames(string content, string team)
    {
        string lastFive = string.Empty;

        using (HttpClient client = new HttpClient())
        {
            try
            {
                string url = $"https://www.espn.com/nba/team/schedule/_/name/{team}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        HtmlDocument document = new HtmlDocument();
        document.LoadHtml(content);

        var res = document.DocumentNode
            .SelectNodes("//tbody[contains(@class, 'Table__TBODY')]")
            .ToList();

        try
        {
            foreach (var item in res)
            {
                string ser = $"<div>{item.InnerHtml}</div>";
                HtmlNode node = HtmlNode.CreateNode(ser);
                var listGameResult = DomToList(node);
                var listGameResultRight = listGameResult.Skip(2).ToList();
                var gamePast = listGameResultRight
                    .TakeWhile(x => x != "")
                    .ToList()
                    .TakeLast(5)
                    .ToList();

                int win = 0;
                int lose = 0;
                for (int i = 0; i < gamePast.Count; i++)
                {
                    if (gamePast[i] == "W")
                        lastFive = $"{++win}-{lose}";
                    else
                        lastFive = $"{win}-{++lose}";
                }

                // Console.WriteLine($"{lastFive}");
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }

        return lastFive;
    }

    private static List<string> DomToList(HtmlNode node)
    {
        var res = new List<string>();

        int number = 0;
        foreach (var item in node.ChildNodes)
        {
            var gameInfo = item.InnerText.Split(" ").ToList();
            int num;
            var str =
                gameInfo.Count > 4 && !string.IsNullOrEmpty(gameInfo[4])
                    ? gameInfo[4].ToString()
                    : string.Empty; // "" C
            var str2 =
                gameInfo.Count > 5 && !string.IsNullOrEmpty(gameInfo[5])
                    ? gameInfo[5].ToString()
                    : string.Empty; // 1 L

            string gameRes = string.Empty;

            if (
                !string.IsNullOrEmpty(str)
                && !string.IsNullOrEmpty(str2)
                && str.GetType() == str2.GetType()
            ) // C L
            {
                bool IsEmpty = string.IsNullOrEmpty(str);
                string value = IsEmpty ? str2 : str;
                var v1 = int.TryParse(value.ElementAt(1).ToString(), out num); // i fales
                if (v1 == true)
                {
                    gameRes = str[0].ToString();
                }
                else
                {
                    gameRes = str2[0].ToString();
                }
            }
            else if (!string.IsNullOrEmpty(str) || !string.IsNullOrEmpty(str2))
            {
                bool IsEmpty = string.IsNullOrEmpty(str);
                string value = IsEmpty ? str2 : str;

                var c = value[0];
                if (c != 'W' && c != 'L')
                    gameRes = value[1].ToString();
                else
                    gameRes = value[0].ToString();
            }

            res.Add(gameRes);
            number++;
        }

        return res;
    }

    private async Task<List<string>> GetAllStatsAsync(
        string team,
        List<HtmlNode> node,
        string content,
        string teamShort
    )
    {
        var statHtml = GetConf(node, team).Last();
        var statClear = statHtml.ChildNodes;

        string wins = statClear[0].InnerText;
        string loses = statClear[1].InnerText;
        string streak = statClear[11].InnerText;
        string lastTeen = statClear[12].InnerText;

        var lastTennGames = lastTeen.Split("-");

        string winRate = GetWinRate(wins, loses);
        string lastTennProcent = GetWinRate(lastTennGames[0], lastTennGames[1]);

        string lastFive = await GetLastFiveGames(content, teamShort.ToLower());
        var lastFiveGames = lastFive.Split("-");
        string lastFiveProcent = GetWinRate(lastFiveGames[0], lastFiveGames[1]);

        return new()
        {
            wins,
            loses,
            streak,
            lastTeen,
            lastTennProcent,
            winRate,
            lastFive,
            lastFiveProcent
        };
    }

    private static string GetWinRate(string wins, string loses)
    {
        double _wins = double.Parse(wins);
        double _loses = double.Parse(loses);
        double _winRate = _wins / (_wins + _loses) * 100;

        return _winRate.ToString("00") + "%";
    }

    private static List<HtmlNode> FindNode(HtmlDocument document, string teamNumber)
    {
        var dataInd = document.DocumentNode
            .SelectNodes("//table[contains(@class, 'Table Table--align-right')]")
            ?.Select(table => table.SelectNodes(".//tr[@data-idx]"))
            .Select(x =>
            {
                var number = Int32.Parse(teamNumber);
                return x[number];
            })
            ?.ToList();
        return dataInd;
    }

    private static string FindNumber(string team, HtmlDocument document)
    {
        var res = document.DocumentNode
            .SelectNodes("//tr[contains(@class, 'Table__TR Table__TR--sm Table__even')]")
            .Where(node => node.InnerText == team)
            .Select(x =>
            {
                HtmlNode node = HtmlNode.CreateNode(x.OuterHtml);
                return node;
            })
            .Select(node =>
            {
                var dataIdx = node.GetAttributes("data-idx").First();
                string number = dataIdx.DeEntitizeValue;
                return number;
            })
            ?.First();

        return res;
    }

    private static string GetGameName(string team, HtmlDocument document)
    {
        var res = document.DocumentNode
            .SelectNodes("//div[contains(@class, 'team-link flex items-center clr-gray-03')]")
            .Select(node =>
            {
                var list = node.ChildNodes;
                var gameAbbreviation = String.Empty;
                var gameName = String.Empty;
                var keyValue = String.Empty;
                var result = String.Empty;
                foreach (var item in list)
                {
                    gameName += item.InnerText;
                    gameAbbreviation += " " + item.InnerText;
                }
                gameAbbreviation = Regex
                    .Replace(gameAbbreviation, @"[\d-]", string.Empty)
                    .TrimStart();

                keyValue = gameAbbreviation.Split(" ").First();

                return (keyValue, gameName);
            })
            .ToDictionary()
            .Where(x => x.Key == team)
            .Select(x => x.Value)
            .ToList()
            .First();

        return res;
    }

    private List<HtmlNode> GetConf(List<HtmlNode> dataInd, string FirstTeam)
    {
        if (dataInd[0].InnerText == FirstTeam)
            return new() { dataInd[0], dataInd[1] };
        else
            return new() { dataInd[2], dataInd[3] };
    }
}
