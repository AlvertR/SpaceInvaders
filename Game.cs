using Raylib_cs;

namespace SpaceInvaders
{
    public class Game
    {
        public Game(int width, int height, string name, int fps) { 
            WidthWindow = width;
            HeightWindow = height;
            NameWindow = name;
            FPS = fps;
        }
        public int HeightWindow { get; set; }
        public int WidthWindow { get; set; }
        public string NameWindow { get; set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public GameStatus GameStatus { get; set; } = GameStatus.Start;
        public int Score { get; set; } = 0;
        public Player Player { get; set; } = new Player();
        public List<Shot> ShotList { get; set; } = new List<Shot>();
        public Sound LaserShot { get; set; }
        public Sound CrashEnemy { get; set; }
        public List<Enemy> EnemyList { get; set; } = new List<Enemy>();
        public bool isEnemyRight { get; set; } = false;
        public float TimerEnd { get; set; } = 0.8f;
        public float EnemySpeed { get; set; } = 100;
        public float EnemyDown { get; set; } = 2;
        public float ScreenEnemyLimit { get; set; }
        public float PlayerX { get; set; }
        public float ShotSpeed { get; set; } = 226;

        public void Run()
        {
            int PlayerWidth = 50;
            int PlayerHeight = 20;
            PlayerX = (WidthWindow / 2) - PlayerWidth / 2;
            int PlayerY = HeightWindow - 50;
            float PlayerSpeed = 300;

            Raylib.InitWindow(this.WidthWindow, this.HeightWindow, this.NameWindow);
            Raylib.InitAudioDevice();
            string BasePath = AppDomain.CurrentDomain.BaseDirectory;

            string FulPathIcon = Path.Combine(BasePath, "Resources", "icon.png");
            Image Icon = Raylib.LoadImage(FulPathIcon);
            Raylib.ImageFormat(ref Icon, PixelFormat.UncompressedR8G8B8A8);
            Raylib.SetWindowIcon(Icon);
            Raylib.UnloadImage(Icon);

            string LaserShotSoundPath = Path.Combine(BasePath, "Resources", "laser-gun-shot.mp3");
            string CrashSoundPath = Path.Combine(BasePath, "Resources", "crash.mp3");
            LaserShot = Raylib.LoadSound(LaserShotSoundPath);
            CrashEnemy = Raylib.LoadSound(CrashSoundPath);
            Raylib.SetTargetFPS(this.FPS);

            Player = new Player(PlayerWidth, PlayerHeight, PlayerX, PlayerY, PlayerSpeed);
            ScreenEnemyLimit = PlayerY - 50;
            this.SetEnemyList();

            while (!Raylib.WindowShouldClose())
            {
                this.DeltaTime = Raylib.GetFrameTime();
                HandleInput();
                Update();
                Draw();
            }

            Raylib.UnloadSound(CrashEnemy);
            Raylib.UnloadSound(LaserShot);
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        public void Update()
        {
            switch (GameStatus)
            {
                case GameStatus.Start:
                    break;
                case GameStatus.Playing:
                    foreach(var shot in ShotList)
                    {
                        shot.SetPositionY(shot.Position.Y - (shot.Speed * DeltaTime));
                    }
                    this.CheckEnemyCollision();
                    this.MoveEnemys();
                    this.UpdateShotsOutScreenOrImpact();
                    this.CheckEndGame();
                    this.CheckGameOver();
                    EnemyList.Where(e => e.Status == EnemyStatus.Dead && e.ShowCollision==true).ToList()
                        .ForEach(e => e.UpdateTimer());
                    break;
                case GameStatus.Paused:
                    break;
                case GameStatus.GameOver:
                case GameStatus.End:
                    break;
                default:
                    break;
            }
        }

        public void HandleInput()
        {
            switch (GameStatus)
            {
                case GameStatus.Start:
                    if (Raylib.IsKeyPressed(KeyboardKey.S))
                        GameStatus = GameStatus.Playing;
                    break;
                case GameStatus.Playing:
                    Player.Update(WidthWindow);
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        this.AddShot();
                        Raylib.PlaySound(LaserShot);
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.P))
                        GameStatus = GameStatus.Paused;
                    break;
                case GameStatus.Paused:
                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                        GameStatus = GameStatus.Playing;
                    break;
                case GameStatus.GameOver:
                case GameStatus.End:
                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                        this.ResetGame();
                    break;
                default:
                    break;
            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            switch (GameStatus)
            {
                case GameStatus.Start:
                    Raylib.DrawText(Texts.Title, GetMidelWidthScreanText(Texts.Title, 24), 5, 24, Color.White);
                    Raylib.DrawText(Texts.StartInstruction, GetMidelWidthScreanText(Texts.StartInstruction, 20), 30, 20, Color.White);
                    break;
                case GameStatus.Paused:
                case GameStatus.Playing:
                    Raylib.DrawText(Texts.PointsTitle+": " + Score.ToString(), 10, 5, 14, Color.White);
                    Player.Draw();
                    foreach(var shot in ShotList)
                    {
                        if(shot.Status == ShotStatus.Active)
                            shot.Draw();
                    }
                    foreach(var enemy in EnemyList)
                    {
                        enemy.Draw();
                    }
                    if(GameStatus == GameStatus.Paused)
                        Raylib.DrawText(Texts.PauseInstruction, GetMidelWidthScreanText(Texts.PauseInstruction, 24), HeightWindow/2, 24, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText(Texts.GameOverTitle, GetMidelWidthScreanText(Texts.GameOverTitle, 34), HeightWindow / 2, 34, Color.White);
                    Raylib.DrawText(Texts.ResetInstruction, GetMidelWidthScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText(Texts.EndTitle, GetMidelWidthScreanText(Texts.EndTitle, 34), HeightWindow / 2, 34, Color.White);
                    Raylib.DrawText(Texts.ResetInstruction, GetMidelWidthScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
                    break;
                default:
                    break;
            }
            Raylib.EndDrawing();
        }

        public int GetMidelWidthScreanText(string text, int fontSize)
        {
            int Position = 0;
            int TextWidth = Raylib.MeasureText(text, fontSize);
            Position = (WidthWindow / 2) - (TextWidth / 2);
            return Position;
        }

        public void AddShot()
        {
            float PosX = Player.Position.X + (Player.Width / 2) - 4;
            float PosY = Player.Position.Y - 8;
            Shot NewShot = new Shot(6,10,PosX, PosY, Color.Lime, ShotSpeed);
            ShotList.Add(NewShot);
        }

        public void UpdateShotsOutScreenOrImpact()
        {
            ShotList = ShotList.Where(s => s.Status != ShotStatus.Impact && s.Position.Y > 0).ToList();
        }
    
        public void SetEnemyList()
        {
            int Cols = 5;
            int Rows = 3;
            float EnemyWith = 40;
            float EnemyHeight = 30;

            float BaseSpaceX = ((WidthWindow / Cols) / 2) - (EnemyWith / 2);
            float BaseSpaceY = (((HeightWindow - (HeightWindow/2))/ Rows) / 2) - (EnemyHeight / 2);

            for (int r = 0; r < Rows; r++)
            {
                Color rowColor = r == 0 ? Color.Red : r == 1 ? Color.Magenta : Color.SkyBlue;
                EnemyType type = r == 0 ? EnemyType.Bug : r == 1 ? EnemyType.Skull : EnemyType.Fish;
                for(int c = 0; c < Cols; c++)
                {
                    float posX = BaseSpaceX + ((WidthWindow / Cols) * c);
                    float posY = BaseSpaceY + (((HeightWindow - (HeightWindow / 2)) / Rows) * r);
                    Enemy newEnemy = new Enemy(EnemyWith, EnemyHeight, posX, posY, rowColor, type);
                    EnemyList.Add(newEnemy);
                }
            }
        }

        public void MoveEnemys()
        {
            var AliveEnemies = EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList();
            if (AliveEnemies.Count == 0)
                return;

            float Move = (isEnemyRight ? EnemySpeed : -EnemySpeed) * DeltaTime;

            // Limites de la formacion completa, no de cada enemigo
            float MinX = AliveEnemies.Min(e => e.Position.X);
            float MaxX = AliveEnemies.Max(e => e.Position.X + e.Width);
            Move = Math.Clamp(Move, -MinX, WidthWindow - MaxX);

            bool HitLeft = MinX + Move <= 0;
            bool HitRight = MaxX + Move >= WidthWindow;

            foreach (var e in AliveEnemies)
            {
                e.SetPositionX(e.Position.X + Move);
                if (HitLeft || HitRight)
                    e.SetPositionY(e.Position.Y + EnemyDown);
            }

            if (HitLeft)
                isEnemyRight = true;
            else if (HitRight)
                isEnemyRight = false;
        }

        public void CheckEnemyCollision()
        {
            foreach (var enemy in EnemyList)
            {
                if (enemy.Status != EnemyStatus.Active)
                    continue;

                Rectangle enemyRec = new Rectangle(enemy.Position.X, enemy.Position.Y, enemy.Width, enemy.Height);
                foreach (var shot in ShotList)
                {
                    if(shot.Status != ShotStatus.Active) 
                        continue;

                    Rectangle shotRec = new Rectangle(shot.Position.X, shot.Position.Y, shot.Width, shot.Height);
                    if(Raylib.CheckCollisionRecs(enemyRec, shotRec))
                    {
                        shot.SetImpactStatus();
                        enemy.SetDeadStatus();
                        Raylib.PlaySound(CrashEnemy);
                        enemy.ShowCollision = true;
                        Score++;
                        break;
                    }
                }
            }
        }

        public void CheckEndGame()
        {
            if (EnemyList.Count(e => e.Status == EnemyStatus.Active) <= 0)
            {
                this.TimerEnd -= DeltaTime;
                if (TimerEnd <= 0)
                    GameStatus = GameStatus.End;
            }
        }

        public void CheckGameOver()
        {
            if(EnemyList.Any(e => e.Status == EnemyStatus.Active 
            && (e.Position.Y + e.Height) >= ScreenEnemyLimit))
                GameStatus = GameStatus.GameOver;
        }

        public void ResetGame()
        {
            TimerEnd = 0.8f;
            GameStatus = GameStatus.Playing;
            Score = 0;
            Player.SetPositionX(PlayerX);
            ShotList = new List<Shot>();
            EnemyList = new List<Enemy>();
            isEnemyRight = false;
            this.SetEnemyList();
        }
    }
}
