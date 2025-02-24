public class GamesSchedule
{
    public List<Game> Games { get; set; }
}

public class Game
{
    public int FirstId { get; set; }
    public int SecondId { get; set; }
    public string Id { get; set; }
    public Team Home { get; set; }
    public Team Away { get; set; }
}

public class Team
{
    public string Name { get; set; }
    public string Alias { get; set; }
}
