using Raylib_cs;
using System.Numerics;

namespace SpaceInvaders
{
    public class Enemy
    {
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
                        this.DrawBug();
                        break;
                    case EnemyType.Skull:
                        this.DrawSkull();
                        break;
                    case EnemyType.Fish:
                        this.DrawFish();
                        break;
                    default:
                        Raylib.DrawRectangleV(Position, new Vector2(Width, Height), Color);
                        Raylib.DrawRectangleV(new Vector2(Position.X + 8, Position.Y + 8), new Vector2(6, 6), Color.Black);
                        Raylib.DrawRectangleV(new Vector2(Position.X + Width - 14, Position.Y + 8), new Vector2(6, 6), Color.Black);
                        break;
                }
            }
            if (ShowCollision) {
                this.DrawExplosion();
                this.Timer -= Raylib.GetFrameTime();
                if(Timer <= 0)
                    ShowCollision = false;
            }
        }

        public void DrawBug()
        {
            float baseX = this.Position.X;
            float baseY = this.Position.Y;
            float widthBasePixel = this.Width / 7;
            float heightBasePixel = this.Height / 5;

            Raylib.DrawRectangleV(new Vector2(baseX,baseY), new Vector2(this.Width, this.Height), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel,baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*5),baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel,baseY+(heightBasePixel*4)), new Vector2(widthBasePixel*5, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel,baseY-heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX,baseY-(heightBasePixel*2)), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*5),baseY-heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*6),baseY-(heightBasePixel*2)), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-widthBasePixel,baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel*2), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-(widthBasePixel*2),baseY+(heightBasePixel*2)), new Vector2(widthBasePixel, heightBasePixel*3), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*7),baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel*2), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*8),baseY+(heightBasePixel*2)), new Vector2(widthBasePixel, heightBasePixel*3), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel,baseY+(heightBasePixel*5)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*4),baseY+(heightBasePixel*5)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
        }

        public void DrawSkull()
        {
            float baseX = this.Position.X;
            float baseY = this.Position.Y;
            float widthBasePixel = this.Width / 7;
            float heightBasePixel = this.Height / 5;

            Raylib.DrawRectangleV(new Vector2(baseX-widthBasePixel, baseY-heightBasePixel), new Vector2(this.Width+(widthBasePixel*2), this.Height), Color);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel, baseY + heightBasePixel), new Vector2(widthBasePixel*2, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel*5, baseY + heightBasePixel), new Vector2(widthBasePixel*2, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel, baseY-(heightBasePixel*2)), new Vector2(widthBasePixel*5, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-(widthBasePixel*2), baseY), new Vector2(widthBasePixel, heightBasePixel*4), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*8), baseY), new Vector2(widthBasePixel, heightBasePixel*4), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel, baseY+(heightBasePixel*4)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*5), baseY+(heightBasePixel*4)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-widthBasePixel, baseY+(heightBasePixel*5)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*3), baseY+(heightBasePixel*5)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*7), baseY+(heightBasePixel*5)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-(widthBasePixel*3), baseY+(heightBasePixel*6)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*9), baseY+(heightBasePixel*6)), new Vector2(widthBasePixel*2, heightBasePixel), Color);
        }

        public void DrawFish()
        {
            float baseX = this.Position.X;
            float baseY = this.Position.Y;
            float widthBasePixel = this.Width / 7;
            float heightBasePixel = this.Height / 5;

            Raylib.DrawRectangleV(new Vector2(baseX, baseY), new Vector2(this.Width, this.Height), Color);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel, baseY + heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel*5), baseY + heightBasePixel), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX + widthBasePixel, baseY + (heightBasePixel*4)), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX + (widthBasePixel*5), baseY + (heightBasePixel*4)), new Vector2(widthBasePixel, heightBasePixel), Color.Black);
            Raylib.DrawRectangleV(new Vector2(baseX-widthBasePixel, baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel*2), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*7), baseY+heightBasePixel), new Vector2(widthBasePixel, heightBasePixel*2), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+widthBasePixel, baseY-heightBasePixel), new Vector2(widthBasePixel*5, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*2), baseY-(heightBasePixel*2)), new Vector2(widthBasePixel*3, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX-widthBasePixel, baseY+(heightBasePixel*5)), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*7), baseY+(heightBasePixel*5)), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX, baseY+(heightBasePixel*6)), new Vector2(widthBasePixel, heightBasePixel), Color);
            Raylib.DrawRectangleV(new Vector2(baseX+(widthBasePixel*6), baseY+(heightBasePixel*6)), new Vector2(widthBasePixel, heightBasePixel), Color);
        }

        public void DrawExplosion()
        {
            float baseX = this.Position.X;
            float baseY = this.Position.Y;
            float widthBasePixel = this.Width / 7;
            float heightBasePixel = this.Height / 5;

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

        public void SetDeadStatus()
            => Status = EnemyStatus.Dead;
    }
}
