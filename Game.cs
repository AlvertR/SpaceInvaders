using Raylib_cs;
using System.Text.Json;

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
        public int HeightWindow { get; private set; }
        public int WidthWindow { get; private set; }
        public string NameWindow { get; private set; } = string.Empty;
        public int FPS { get; set; }
        public float DeltaTime { get; set; } = 0;
        public GameStatus GameStatus { get; private set; } = GameStatus.Start;
        public int Score { get; private set; } = 0;
        public Player Player { get; private set; } = new Player();
        public List<Shot> ShotList { get; private set; } = new List<Shot>();
        public List<Shot> EnemyShotList { get; private set; } = new List<Shot>();
        public Sound LaserShot { get; set; }
        public Sound CrashEnemy { get; set; }
        public List<Enemy> EnemyList { get; private set; } = new List<Enemy>();
        public bool isEnemyRight { get; private set; } = false;
        public float TimerEnd { get; private set; } = 0.8f;
        public float EnemySpeed { get; private set; } = 100;
        public float EnemyDown { get; private set; } = 10;
        public float ScreenEnemyLimit { get; private set; }
        public float PlayerX { get; private set; }
        public float ShotSpeed { get; private set; } = 226;
        public float TimerEnemyShot { get; private set; } = 0.8f;

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
                        shot.SetPositionY(shot.Bounds.Y - (shot.Speed * DeltaTime));
                    }
                    foreach (var shotE in EnemyShotList)
                    {
                        shotE.SetPositionY(shotE.Bounds.Y + (shotE.Speed * DeltaTime));
                    }
                    this.CheckEnemyCollision();
                    this.CheckPLayerCollision();
                    this.MoveEnemys();
                    this.AddEnemyShot();
                    this.UpdateShotsOutScreenOrImpact();
                    this.CheckEndGame();
                    this.CheckGameOver();
                    EnemyList.Where(e => e.Status == EnemyStatus.Dead && e.ShowCollision==true).ToList()
                        .ForEach(e => e.UpdateTimer());
                    if(Player.ShowCollision)
                        Player.UpdateTimer();
                    break;
                case GameStatus.Paused:
                    break;
                case GameStatus.GameOver:
                    break;
                case GameStatus.End:
                    {
                        this.TimerEnd -= DeltaTime;
                        if (TimerEnd <= 0)
                            this.NewLevel();
                    }
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
                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                        this.ResetGame();
                    break;
                case GameStatus.End:
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
                    foreach (var shotE in EnemyShotList)
                    {
                        if (shotE.Status == ShotStatus.Active)
                            shotE.Draw();
                    }
                    foreach (var enemy in EnemyList)
                    {
                        enemy.Draw();
                    }
                    Player.DrawLifes(HeightWindow);
                    if(GameStatus == GameStatus.Paused)
                        Raylib.DrawText(Texts.PauseInstruction, GetMidelWidthScreanText(Texts.PauseInstruction, 24), HeightWindow/2, 24, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText(Texts.GameOverTitle, GetMidelWidthScreanText(Texts.GameOverTitle, 34), HeightWindow / 2, 34, Color.White);
                    Raylib.DrawText(Texts.ResetInstruction, GetMidelWidthScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText(Texts.EndTitle, GetMidelWidthScreanText(Texts.EndTitle, 34), HeightWindow / 2, 34, Color.White);
                    //Raylib.DrawText(Texts.ResetInstruction, GetMidelWidthScreanText(Texts.ResetInstruction, 30), (HeightWindow / 2) + 30, 30, Color.White);
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
            float PosX = Player.Bounds.X + (Player.Bounds.Width / 2) - 4;
            float PosY = Player.Bounds.Y - 8;
            Shot NewShot = new Shot(6,10,PosX, PosY, Color.Lime, ShotSpeed, ShotType.Player);
            ShotList.Add(NewShot);
        }

        public void AddEnemyShot()
        {
            TimerEnemyShot -= DeltaTime;
            if (EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList().Count > 0 && TimerEnemyShot <= 0)
            {
                int index = Random.Shared.Next(EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList().Count);
                var enemyRandom = EnemyList[index];
                float PosX = enemyRandom.Bounds.X + (enemyRandom.Bounds.Width / 2) - 4;
                float PosY = enemyRandom.Bounds.Y - 8;
                Shot NewShot = new Shot(6, 10, PosX, PosY, enemyRandom.Color, ShotSpeed, ShotType.Enemy);
                EnemyShotList.Add(NewShot);
                TimerEnemyShot = 0.8f;
            }
        }

        public void UpdateShotsOutScreenOrImpact()
        {
            ShotList.RemoveAll(s => s.Status == ShotStatus.Impact && s.Bounds.Y > 0);
            EnemyShotList.RemoveAll(s => s.Status == ShotStatus.Impact && s.Bounds.Y >= HeightWindow);
            EnemyList.RemoveAll(e => e.Status == EnemyStatus.Dead && !e.ShowCollision);
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
            if (EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList().Count == 0)
                return;
            int enemys = EnemyList.Count(e => e.Status == EnemyStatus.Active);
            float updateSpeed = enemys >= 10 ? EnemySpeed
                : (enemys < 10 && enemys >= 5) ? EnemySpeed * 1.5f
                : EnemySpeed * 2f;

            float Move = (isEnemyRight ? updateSpeed : -updateSpeed) * DeltaTime;

            // Limites de la formacion completa, no de cada enemigo
            float MinX = EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList().Min(e => e.Bounds.X);
            float MaxX = EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList().Max(e => e.Bounds.X + e.Bounds.Width);
            Move = Math.Clamp(Move, -MinX, WidthWindow - MaxX);

            bool HitLeft = MinX + Move <= 0;
            bool HitRight = MaxX + Move >= WidthWindow;

            foreach (var e in EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList())
            {
                e.SetPositionX(e.Bounds.X + Move);
                if (HitLeft || HitRight)
                {
                    float dawnSpeed = enemys >= 10 ? EnemyDown
                        : (enemys < 10 && enemys >= 5) ? EnemyDown * 1.5f
                        : EnemyDown * 2f;

                    e.SetPositionY(e.Bounds.Y + dawnSpeed);
                }
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

                foreach (var shot in ShotList)
                {
                    if(shot.Status != ShotStatus.Active) 
                        continue;

                    if(Raylib.CheckCollisionRecs(enemy.Bounds, shot.Bounds))
                    {
                        shot.SetImpactStatus();
                        enemy.SetDeadStatus();
                        Raylib.PlaySound(CrashEnemy);
                        enemy.ShowCollision = true;
                        if(enemy.TypeEnemy == EnemyType.Bug)
                            Score += 30;
                        if (enemy.TypeEnemy == EnemyType.Skull)
                            Score += 20;
                        if (enemy.TypeEnemy == EnemyType.Fish)
                            Score += 10;
                        break;
                    }
                }
            }
        }

        public void CheckPLayerCollision()
        {
            foreach (var shotE in EnemyShotList)
            {
                if (shotE.Status != ShotStatus.Active) 
                    continue;

                if(Raylib.CheckCollisionRecs(Player.Bounds, shotE.Bounds))
                {
                    shotE.SetImpactStatus();
                    Raylib.PlaySound(CrashEnemy);
                    Player.ShowCollision = true;
                    Player.Lifes--;
                }
            }
        }

        public void CheckEndGame()
        {
            if (EnemyList.Count(e => e.Status == EnemyStatus.Active) <= 0)
            {
                this.TimerEnd -= DeltaTime;
                if (TimerEnd <= 0)
                {
                    GameStatus = GameStatus.End;
                    TimerEnd = 0.8f;
                }
            }
        }

        public void CheckGameOver()
        {
            if(EnemyList.Any(e => e.Status == EnemyStatus.Active 
            && (e.Bounds.Y + e.Bounds.Height) >= ScreenEnemyLimit))
                GameStatus = GameStatus.GameOver;

            if(Player.Lifes <= 0)
                GameStatus = GameStatus.GameOver;
        }

        public void ResetGame()
        {
            TimerEnd = 0.8f;
            GameStatus = GameStatus.Playing;
            Score = 0;
            Player.SetPositionX(PlayerX);
            Player.Lifes = 3;
            ShotList = new List<Shot>();
            EnemyList = new List<Enemy>();
            EnemyShotList = new List<Shot>();
            isEnemyRight = false;
            this.SetEnemyList();
        }

        public void NewLevel()
        {
            TimerEnd = 0.8f;
            GameStatus = GameStatus.Playing;
            ShotList = new List<Shot>();
            EnemyList = new List<Enemy>();
            EnemyShotList = new List<Shot>();
            isEnemyRight = false;
            this.SetEnemyList();
        }
    }
}
