using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.gameobjects
{
    public class Spaceship
    {
        // Int
        int screenHeight;
        int screenWidth;
        int screenMargin;

        // Float
        public float HitboxRadius;
        float Size;

        // Other
        Texture2D Tex;
        Vector2 Pos;
        //public Rectangle Hitbox;

        public Spaceship(Texture2D tex, Vector2 pos, float size, int height, int width, int margin)
        {
            Tex = tex;
            Pos = pos;
            Size = size;
            screenHeight = height;
            screenWidth = width;
            screenMargin = margin;
            //Hitbox = new((int)Pos.X, (int)Pos.Y, (int)(Tex.Width * size), (int)(Tex.Height * size));
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Tex, Pos, null, Color.White, 0f, new Vector2(), Size, 0f, 0f);
        }

    }
}
