using SpaceInvaders;

public class Program
{
    public Program() { }

    public static void Main(string[] args)
    {
        Game game = new Game(800, 600, Texts.Title, 60);
        game.LoadGame();
    }
}