using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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

        public void LoadGame()
        {
            Raylib.InitWindow(this.WidthWindow, this.HeightWindow, this.NameWindow);
            //Raylib.InitAudioDevice();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string fulPathIcon = Path.Combine(basePath, "Resources", "icon.png");
            Image icon = Raylib.LoadImage(fulPathIcon);
            Raylib.ImageFormat(ref icon, PixelFormat.UncompressedR8G8B8A8);
            Raylib.SetWindowIcon(icon);
            Raylib.UnloadImage(icon);

            //string hitBrickSoundPath = Path.Combine(basePath, "Resources", "hit-brick.mp3");
            //string hitPaddleSoundPath = Path.Combine(basePath, "Resources", "hit-paddle.mp3");
            //HitBrickSound = Raylib.LoadSound(hitBrickSoundPath);
            //HitPaddleSound = Raylib.LoadSound(hitPaddleSoundPath);
            Raylib.SetTargetFPS(this.FPS);

            while (!Raylib.WindowShouldClose())
            {
                this.DeltaTime = Raylib.GetFrameTime();
                //HandleInput();
                //Update();
                Draw();
            }

            //Raylib.UnloadSound(HitPaddleSound);
            //Raylib.UnloadSound(HitBrickSound);
            //Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        public void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText(NameWindow, 10, 5, 24, Color.White);
            Raylib.EndDrawing();
        }
    }
}
