using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asteroids.gameobjects
{
    public class Asteroid
    {
        // Vector 2
        public Vector2 Pos;
        public Vector2 Velocity;
        public Vector2 Center;

        // Int
        int screenHeight;
        int screenWidth;
        int screenMargin;

        // Float
        public float HitboxRadius;
        public float Size;

        // Other
        public Texture2D Tex;
        public Rectangle Hitbox;

        public Asteroid(Texture2D tex, Vector2 pos, Vector2 velocity, float size, int height, int width,int margin)
        {
            Tex = tex;
            Pos = pos;
            Velocity = velocity;
            Size = size;
            screenHeight = height;
            screenWidth = width;
            screenMargin = margin;
            Hitbox = new((int)Pos.X, (int)Pos.Y, (int)(Tex.Width * size), (int)(Tex.Height * size));
            Center = new Vector2(Hitbox.X+Hitbox.Width/2, Hitbox.Y + Hitbox.Height);
            HitboxRadius = Hitbox.X/2;
        }

        public void Update()
        {
            Pos += Velocity;
            ReSpawn();

            Hitbox.X = (int)Pos.X;
            Hitbox.Y = (int)Pos.Y;
            Center = new Vector2(Hitbox.X + Hitbox.Width / 2, Hitbox.Y + Hitbox.Height);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Tex, Pos, null, Color.White,0f, new Vector2(),Size,0f, 0f);
        }

        /// <summary>
        /// Changes asteroid's position if it goes out of bounds
        /// </summary>
        public void ReSpawn()
        {
            if (Pos.X < -screenMargin)
                Pos = new Vector2(screenWidth, Pos.Y);
            if (Pos.X > screenWidth + screenMargin)
                Pos = new Vector2(-screenMargin, Pos.Y);
            if (Pos.Y < -screenMargin)
                Pos = new Vector2(Pos.Y, screenHeight);
            if (Pos.Y > screenHeight + screenMargin)
                Pos = new Vector2(Pos.Y , - screenMargin);
        }
    }
}
