using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Enemy
    {
        #region Sprites
        // '#' = color del enemigo, 'o' = negro, '.' = vacío
        private static readonly string[] BugSprite =
        {
            "..#.....#..",
            "...#...#...",
            "..#######..",
            ".##o###o##.",
            "###########",
            "#.#######.#",
            "#.#ooooo#.#",
            "...##.##...",
        };

        private static readonly string[] SkullSprite =
        {
            "....######....",
            "..##########..",
            ".############.",
            ".###oo##oo###.",
            ".############.",
            ".############.",
            "....##..##....",
            "..##..##..##..",
            "##..........##",
        };

        private static readonly string[] FishSprite =
        {
            "...###...",
            "..#####..",
            ".#######.",
            "##o###o##",
            "#########",
            ".#######.",
            ".#o###o#.",
            "#.......#",
            ".#.....#.",
        };

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
        #endregion

        public Enemy(float width, float height, float posX, float posY, Color color, EnemyType type) {
            Width = width; 
            Height = height;
            Position = new Vector2(posX, posY);
            Color = color;
            TypeEnemy = type;
        }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public EnemyStatus Status { get; set; } = EnemyStatus.Active;
        public EnemyType TypeEnemy { get; set; }
        public bool ShowCollision { get; set; } = false;
        public float Timer { get; set; } = 0.5f;

        public void SetPositionX(float position)
        {
            this.Position = new Vector2(position, this.Position.Y);
        }

        public void SetPositionY(float position)
        {
            this.Position = new Vector2(this.Position.X, position);
        }
        public void Draw()
        {
            if (Status == EnemyStatus.Active)
            {
                switch (TypeEnemy)
                {
                    case EnemyType.Bug:
                        this.DrawSprite(BugSprite, Color);
                        break;
                    case EnemyType.Skull:
                        this.DrawSprite(SkullSprite, Color);
                        break;
                    case EnemyType.Fish:
                        this.DrawSprite(FishSprite, Color);
                        break;
                    default:
                        Raylib.DrawRectangleV(Position, new Vector2(Width, Height), Color);
                        Raylib.DrawRectangleV(new Vector2(Position.X + 8, Position.Y + 8), new Vector2(6, 6), Color.Black);
                        Raylib.DrawRectangleV(new Vector2(Position.X + Width - 14, Position.Y + 8), new Vector2(6, 6), Color.Black);
                        break;
                }
            }
            if (ShowCollision) {
                this.DrawSprite(ExplosionSprite, Color.Lime);
            }
        }

        public void UpdateTimer()
        {
            this.Timer -= Raylib.GetFrameTime();
            if (Timer <= 0)
                ShowCollision = false;
        }

        private void DrawSprite(string[] sprite, Color mainColor)
        {
            int rows = sprite.Length;
            int cols = sprite[0].Length;

            // Tamaño de píxel uniforme para no deformar el dibujo
            float pixel = MathF.Min(Width / cols, Height / rows);

            // Centrar dentro del rectángulo base
            float offsetX = Position.X + (Width - pixel * cols) / 2f;
            float offsetY = Position.Y + (Height - pixel * rows) / 2f;

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

        public void SetDeadStatus()
            => Status = EnemyStatus.Dead;
    }
}
