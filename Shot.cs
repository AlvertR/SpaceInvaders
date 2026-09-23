using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Shot
    {
        public Shot(float width, float height, float posX, float posY, Color color, float speed) { 
            Width = width;
            Height = height;
            Position = new Vector2(posX, posY);
            Color = color;
            Speed = speed;
        }
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public ShotStatus Status { get; set; } = ShotStatus.Active;
        public float Speed { get; set; }

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
            Raylib.DrawRectangleV(Position, new Vector2(Width, Height), Color);
        }

        public void SetImpactStatus()
            => Status = ShotStatus.Impact;
    }
}
