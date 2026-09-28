using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Shot : Entity
    {
        public Shot(float width, float height, float posX, float posY, Color color, float speed, ShotType type) { 
            Bounds = new Rectangle(posX, posY, width, height);
            Color = color;
            Speed = speed;
            Type = type;
        }

        public Color Color { get; set; }
        public ShotStatus Status { get; set; } = ShotStatus.Active;
        public float Speed { get; set; }
        public ShotType Type { get; set; }

        public void Draw()
        {
            if (Status == ShotStatus.Active)
                switch (this.Type) {
                    case ShotType.Enemy:
                        Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.Width, Bounds.Height), Color);
                        Raylib.DrawRectangleV(new Vector2(Bounds.X - (Bounds.Height / 6), Bounds.Y- Bounds.Width), new Vector2(Bounds.Height, Bounds.Width), Color);
                        break;
                    case ShotType.Player:
                    default:
                        Raylib.DrawRectangleV(new Vector2(Bounds.X, Bounds.Y), new Vector2(Bounds.Width, Bounds.Height), Color);
                        break;
                }
        }

        public void SetImpactStatus()
            => Status = ShotStatus.Impact;
    }
}
