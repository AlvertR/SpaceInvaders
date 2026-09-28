namespace SpaceInvaders
{
    public enum GameStatus
    {
        Start,
        Playing,
        Paused,
        GameOver,
        End,
    }

    public enum ShotStatus
    {
        Active,
        Impact,
    }

    public enum ShotType
    {
        Player,
        Enemy,
    }

    public enum EnemyStatus
    {
        Active,
        Dead,
    }

    public enum EnemyType
    {
        Bug,
        Skull,
        Fish,
    }
}
