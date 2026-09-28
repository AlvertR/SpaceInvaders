using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Player : Entity
    {
        private static readonly string[] ExplosionSprite =
        {
            "....#...#....",
            ".#...#.#...#.",
            "..#.......#..",
            "...#.....#...",
            "##.........##",
            "...#.....#...",
            "..#..#.#..#..",
            ".#..#...#..#.",
        };
        public Player() { }
        public Player(float width, float height, float posX, float posY, float speed)
        {
            Bounds = new Rectangle(posX, posY, width, height);
            this.Speed = speed;
        }

        public float Speed { get; set; }
        public int Lifes { get; set; } = 3;
        public bool ShowCollision { get; set; } = false;
        public float Timer { get; set; } = 0.5f;

        public void Draw()
        {
            if (ShowCollision)
            {
                this.DrawSprite(ExplosionSprite, Color.Lime);
            }
            else
            {
                Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.Width, Bounds.Height), Color.Lime);
                Raylib.DrawRectangleV(new Vector2(Bounds.X + (Bounds.Width / 2) - 4, Bounds.Y - 8), new Vector2(8, 8), Color.Lime);
            }
        }

        public void DrawLifes( float screenHeight)
        {
            for (int l = 1; l < Lifes; l++)
            {
                float baseX = (10 + Bounds.Width * (l - 1));
                float baseY = screenHeight - Bounds.Height;
                //Console.WriteLine("x "+baseX.ToString() +" y "+ baseY.ToString() + "window" + screenHeight.ToString());
                Raylib.DrawRectangleV(new Vector2(baseX, baseY), new Vector2(Bounds.Width/2, Bounds.Height/2), Color.Lime);
                Raylib.DrawRectangleV(new Vector2(baseX + (Bounds.Width / 4) - 2, baseY - 4), new Vector2(4, 4), Color.Lime);
            }
        }

        public void Update(int widthWindow)
        {
            float movement = 0;
            if (Raylib.IsKeyDown(KeyboardKey.Left) && Bounds.X >= 0)
                movement -= this.Speed * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.Right) && (Bounds.X + Bounds.Width) <= widthWindow)
                movement += this.Speed * Raylib.GetFrameTime();

            float newX = Bounds.X + movement;
            newX = Math.Clamp(newX, 0, widthWindow - Bounds.Width);
            SetPositionX(newX);
        }

        private void DrawSprite(string[] sprite, Color mainColor)
        {
            int rows = sprite.Length;
            int cols = sprite[0].Length;

            // Tamaño de píxel uniforme para no deformar el dibujo
            float pixel = MathF.Min(Bounds.Width / cols, Bounds.Height / rows);

            // Centrar dentro del rectángulo base
            float offsetX = Bounds.X + (Bounds.Width - pixel * cols) / 2f;
            float offsetY = Bounds.Y + (Bounds.Height - pixel * rows) / 2f;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    char cell = sprite[r][c];
                    if (cell == '.') continue;

                    Color color = cell == 'o' ? Color.Black : mainColor;

                    // Redondear bordes para evitar líneas/huecos entre píxeles
                    int x0 = (int)MathF.Floor(offsetX + c * pixel);
                    int y0 = (int)MathF.Floor(offsetY + r * pixel);
                    int x1 = (int)MathF.Floor(offsetX + (c + 1) * pixel);
                    int y1 = (int)MathF.Floor(offsetY + (r + 1) * pixel);

                    Raylib.DrawRectangle(x0, y0, x1 - x0, y1 - y0, color);
                }
            }
        }

        public void UpdateTimer()
        {
            this.Timer -= Raylib.GetFrameTime();
            if (Timer <= 0)
            {
                ShowCollision = false;
                Timer = 0.5f;
            }
        }
    }
}
