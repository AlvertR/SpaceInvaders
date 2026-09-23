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
        public List<Enemy> EnemyList { get; set; } = new List<Enemy>();
        public bool isEnemyRight { get; set; } = false;

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
            //string hitPaddleSoundPath = Path.Combine(basePath, "Resources", "hit-paddle.mp3");
            LaserShot = Raylib.LoadSound(laserShotSoundPath);
            //HitPaddleSound = Raylib.LoadSound(hitPaddleSoundPath);
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

            //Raylib.UnloadSound(HitPaddleSound);
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
                    this.UpdateShotsOutScreanOrImpact();
                    this.MoveEnemys();
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
                    break;
                default:
                    break;
            }
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            //Raylib.ClearBackground(Color.Black);
            switch (GameStatus)
            {
                case GameStatus.Start:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText(NameWindow, 10, 5, 24, Color.White);
                    Raylib.DrawText("Presiona S para iniciar", 10, 30, 20, Color.White);
                    break;
                case GameStatus.Playing:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText("Puntos: " + Score.ToString(), 10, 5, 14, Color.White);
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
                    break;
                case GameStatus.Paused:
                    Raylib.DrawText("Presiona C para continuar", 10, 5, 20, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText("Fin del juego", 10, 5, 34, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText("Nivel completado", 10, 5, 34, Color.White);
                    break;
                default:
                    break;
            }
            Raylib.EndDrawing();
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
                for(int c = 0; c < cols; c++)
                {
                    float posX = baseSpaceX + ((WidthWindow / cols) * c);
                    float posY = baseSpaceY + (((HeightWindow - (HeightWindow / 2)) / rows) * r);
                    Enemy newEnemy = new Enemy(enemyWith, enemyHeight, posX, posY, rowColor);
                    EnemyList.Add(newEnemy);
                }
            }
        }

        public void MoveEnemys()
        {
            float move = 0;
                if(isEnemyRight)
                    move += 130 * DeltaTime;
                else
                    move -= 130 * DeltaTime;
            foreach(var enemy in EnemyList)
            {
                float newX = enemy.Position.X + move;
                newX = Math.Clamp(newX, 0, WidthWindow - enemy.Width);
                enemy.SetPositionX(newX);
                if (newX <= 0)
                {
                    isEnemyRight = true;
                    //fix move y
                    enemy.SetPositionY(enemy.Position.Y + 10);
                }
                if (newX >= WidthWindow - enemy.Width)
                {
                    isEnemyRight = false;
                    enemy.SetPositionY(enemy.Position.Y + 10);
                }
            }
        }
    }
}
