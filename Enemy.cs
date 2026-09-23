using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Enemy
    {
        public Enemy(float width, float height, float posX, float posY, Color color) {
            Width = width; 
            Height = height;
            Position = new Vector2(posX, posY);
            Color = color;
        }
        //uno = new Enemy(40,30, 10,10, Color.Magenta);
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector2 Position { get; set; }
        public Color Color { get; set; }

        public void Draw()
        {
            // body
            Raylib.DrawRectangleV(Position, new Vector2(Width, Height), Color);
            // eyes
            Raylib.DrawRectangleV(new Vector2(Position.X + 8,Position.Y + 8), new Vector2(6, 6), Color.Black);
            Raylib.DrawRectangleV(new Vector2(Position.X + Width - 14, Position.Y + 8), new Vector2(6, 6), Color.Black);
        }
    }
}
