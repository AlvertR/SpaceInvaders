using Raylib_cs;
using System.Numerics;

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
                    Raylib.DrawText("Presiona C para continuar", (WidthWindow / 2) - 120, HeightWindow/2, 24, Color.White);
                    break;
                case GameStatus.GameOver:
                    Raylib.DrawText("Fin del juego", (WidthWindow/2) - 50, HeightWindow / 2, 34, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText("Nivel completado", (WidthWindow / 2) - 80, HeightWindow / 2, 34, Color.White);
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
            float move = 0;
            if (isEnemyRight)
                move += 100 * DeltaTime;
            else
                move -= 100 * DeltaTime;

            //EnemyList = EnemyList.Where(e => e.Status != EnemyStatus.Dead).ToList();

            EnemyList.Where(e=> e.Status != EnemyStatus.Dead).ToList().ForEach(e => 
            {
                float newX = e.Position.X + move;
                newX = Math.Clamp(newX, 0, WidthWindow - e.Width);
                e.SetPositionX(newX);
            });

            if (EnemyList.Any(e => e.Position.X <= 0 && e.Status != EnemyStatus.Dead))
            {
                isEnemyRight = true;
                EnemyList.ForEach(e => e.SetPositionY(e.Position.Y + 2));
            }
            if (EnemyList.Any(e => e.Position.X >= WidthWindow - e.Width && e.Status != EnemyStatus.Dead))
            {
                isEnemyRight = false;
                EnemyList.ForEach(e => e.SetPositionY(e.Position.Y + 2));
            }
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
                        //this.DrawExplosion(enemy);
                        enemy.ShowCollision = true;
                        Score++;
                    }
                }
            }
        }

        public void DrawExplosion(Enemy enemy)
        {
            float baseX = enemy.Position.X;
            float baseY = enemy.Position.Y;
            float widthBasePixel = enemy.Width / 7;
            float heightBasePixel = enemy.Height / 5;

            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 2), baseY), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel, baseY - heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 4), baseY), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 5), baseY - heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);

            Raylib.DrawRectangleV(new Vector2(baseX, baseY + (heightBasePixel * 2)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX - widthBasePixel, baseY + heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX - (widthBasePixel * 2), baseY), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 6), baseY + (heightBasePixel * 2)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 7), baseY + heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 8), baseY), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);

            Raylib.DrawRectangleV(new Vector2(baseX - (widthBasePixel * 3), baseY + (heightBasePixel * 3)), new Vector2(widthBasePixel * 2, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 8), baseY + (heightBasePixel * 3)), new Vector2(widthBasePixel * 2, heightBasePixel), Color.Lime);

            Raylib.DrawRectangleV(new Vector2(baseX, baseY + (heightBasePixel * 4)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX - widthBasePixel, baseY + (heightBasePixel * 5)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX - (widthBasePixel * 2), baseY + (heightBasePixel * 6)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 6), baseY + (heightBasePixel * 4)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 7), baseY + (heightBasePixel * 5)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 8), baseY + (heightBasePixel * 6)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);

            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 2), baseY + (heightBasePixel * 5)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel, baseY + (heightBasePixel * 6)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 4), baseY + (heightBasePixel * 5)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel * 5), baseY + (heightBasePixel * 6)), new Vector2(widthBasePixel, heightBasePixel), Color.Lime);
        }

        public void CheckEndGame()
        {
            if (EnemyList.Count(e => e.Status == EnemyStatus.Active) <= 0)
                GameStatus = GameStatus.End;
        }

        public void CheckGameOver()
        {
            if(EnemyList.Any(e => e.Status == EnemyStatus.Active 
            && (e.Position.Y + e.Height) >= Player.Position.Y - 50))
                GameStatus = GameStatus.GameOver;
        }
    }
}
