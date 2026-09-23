using SpaceInvaders;

public class Program
{
    public Program() { }

    public static void Main(string[] args)
    {
        Game game = new Game(800, 600, "Space Invaders", 60);
        game.LoadGame();
    }
}