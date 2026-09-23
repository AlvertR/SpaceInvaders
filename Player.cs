using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Player
    {
        public Player(float width, float height, float posX, float posY, float speed)
        {
            this.Width = width;
            this.Height = height;
            this.Position = new Vector2(posX, posY);
            this.Speed = speed;
        }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Position {  get; set; }
        public float Speed { get; set; }
        public int Lifes { get; set; } = 3;

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
            Raylib.DrawRectangleV(Position, new Vector2(Width, Height), Color.Lime);
            Raylib.DrawRectangleV(new Vector2(Position.X + (Width / 2) - 4, Position.Y - 8), new Vector2(8, 8), Color.Lime);
        }

        public void Update(int widthWindow)
        {
            float movement = 0;
            if (Raylib.IsKeyDown(KeyboardKey.Left) && Position.X >= 0)
                movement -= this.Speed * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.Right) && (Position.X + Width) <= widthWindow)
                movement += this.Speed * Raylib.GetFrameTime();

            float newX = Position.X + movement;
            newX = Math.Clamp(newX, 0, widthWindow - Width);
            SetPositionX(newX);
        }
    }
}
