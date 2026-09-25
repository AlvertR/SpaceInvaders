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
        public Player Player { get; set; }
        public List<Shot> ShotList { get; set; } = new List<Shot>();
        public Sound LaserShot { get; set; }
        public Sound CrashEnemy { get; set; }
        public List<Enemy> EnemyList { get; set; } = new List<Enemy>();
        public bool isEnemyRight { get; set; } = false;
        public float TimerEnd { get; set; } = 0.8f;

        public void LoadGame()
        {
            Raylib.InitWindow(this.WidthWindow, this.HeightWindow, this.NameWindow);
            Raylib.InitAudioDevice();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string fulPathIcon = Path.Combine(basePath, "Resources", "icon.png");
            Image icon = Raylib.LoadImage(fulPathIcon);
            Raylib.ImageFormat(ref icon, PixelFormat.UncompressedR8G8B8A8);
            Raylib.SetWindowIcon(icon);
            Raylib.UnloadImage(icon);

            string laserShotSoundPath = Path.Combine(basePath, "Resources", "laser-gun-shot.mp3");
            string crashSoundPath = Path.Combine(basePath, "Resources", "crash.mp3");
            LaserShot = Raylib.LoadSound(laserShotSoundPath);
            CrashEnemy = Raylib.LoadSound(crashSoundPath);
            Raylib.SetTargetFPS(this.FPS);

            Player = new Player(50,20,(WidthWindow/2) - 25, HeightWindow - 50, 300);
            this.SetEnemiList();

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
                    this.UpdateShotsOutScreanOrImpact();
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
                    Raylib.DrawText(Texts.Title, GetMidelScreanText(Texts.Title, 24), 5, 24, Color.White);
                    Raylib.DrawText(Texts.StartInstruction, GetMidelScreanText(Texts.StartInstruction, 20), 30, 20, Color.White);
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
                        Raylib.DrawText(Texts.PauseInstruction, GetMidelScreanText(Texts.PauseInstruction, 24), HeightWindow/2, 24, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText(Texts.GameOverTitle, GetMidelScreanText(Texts.GameOverTitle, 34), HeightWindow / 2, 34, Color.White);
                    Raylib.DrawText(Texts.ResetInstruction, GetMidelScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText(Texts.EndTitle, GetMidelScreanText(Texts.EndTitle, 34), HeightWindow / 2, 34, Color.White);
                    Raylib.DrawText(Texts.ResetInstruction, GetMidelScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
                    break;
                default:
                    break;
            }
            Raylib.EndDrawing();
        }

        public int GetMidelScreanText(string text, int fontSize)
        {
            int position = 0;
            int textWidth = Raylib.MeasureText(text, fontSize);
            position = (WidthWindow / 2) - (textWidth / 2);
            return position;
        }

        public void AddShot()
        {
            float posX = Player.Position.X + (Player.Width / 2) - 4;
            float posY = Player.Position.Y - 8;
            Shot newShot = new Shot(6,10,posX, posY, Color.Lime, 226);
            ShotList.Add(newShot);
        }

        public void UpdateShotsOutScreanOrImpact()
        {
            ShotList = ShotList.Where(s => s.Status != ShotStatus.Impact && s.Position.Y > 0).ToList();
        }
    
        public void SetEnemiList()
        {
            int cols = 5;
            int rows = 3;
            float enemyWith = 40;
            float enemyHeight = 30;
            float baseSpaceX = ((WidthWindow / cols) / 2) - (enemyWith / 2);
            float baseSpaceY = (((HeightWindow - (HeightWindow/2))/ rows) / 2) - (enemyHeight / 2);
            for (int r = 0; r < rows; r++)
            {
                Color rowColor = r == 0 ? Color.Red : r == 1 ? Color.Magenta : Color.SkyBlue;
                EnemyType type = r == 0 ? EnemyType.Bug : r == 1 ? EnemyType.Skull : EnemyType.Fish;
                for(int c = 0; c < cols; c++)
                {
                    float posX = baseSpaceX + ((WidthWindow / cols) * c);
                    float posY = baseSpaceY + (((HeightWindow - (HeightWindow / 2)) / rows) * r);
                    Enemy newEnemy = new Enemy(enemyWith, enemyHeight, posX, posY, rowColor, type);
                    EnemyList.Add(newEnemy);
                }
            }
        }

        public void MoveEnemys()
        {
            var aliveEnemies = EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList();
            if (aliveEnemies.Count == 0)
                return;

            float move = (isEnemyRight ? 100 : -100) * DeltaTime;

            // Limites de la formacion completa, no de cada enemigo
            float minX = aliveEnemies.Min(e => e.Position.X);
            float maxX = aliveEnemies.Max(e => e.Position.X + e.Width);
            move = Math.Clamp(move, -minX, WidthWindow - maxX);

            bool hitLeft = minX + move <= 0;
            bool hitRight = maxX + move >= WidthWindow;

            foreach (var e in aliveEnemies)
            {
                e.SetPositionX(e.Position.X + move);
                if (hitLeft || hitRight)
                    e.SetPositionY(e.Position.Y + 2);
            }

            if (hitLeft)
                isEnemyRight = true;
            else if (hitRight)
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
            && (e.Position.Y + e.Height) >= Player.Position.Y - 50))
                GameStatus = GameStatus.GameOver;
        }

        public void ResetGame()
        {
            TimerEnd = 0.8f;
            GameStatus = GameStatus.Playing;
            Score = 0;
            Player.SetPositionX((WidthWindow / 2) - 25);
            ShotList = new List<Shot>();
            EnemyList = new List<Enemy>();
            isEnemyRight = false;
            this.SetEnemiList();
        }
    }
}
