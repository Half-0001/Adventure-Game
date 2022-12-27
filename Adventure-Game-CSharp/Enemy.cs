using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;

namespace Adventure_Game_CSharp
{
    internal class Enemy
    {
        private AnimatedSprite enemySprite;
        private Vector2 position = new Vector2(678, 1768);
        public void LoadContent(ContentManager Content, Point _resolution)
        {
            //  Load the asprite file from the content pipeline.
            AsepriteDocument asepritefile = Content.Load<AsepriteDocument>("Enemy 15-1");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            enemySprite = new AnimatedSprite(asepritefile);
            enemySprite.Scale = new Vector2(1.0f, 1.0f);
            enemySprite.Y = _resolution.Y - (enemySprite.Height * enemySprite.Scale.Y) - 16;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            enemySprite.Position = new Vector2(position.X, position.Y);
            enemySprite.Update(dt);
        }

        public void Draw(SpriteBatch _spritebatch)
        {
            enemySprite.Render(_spritebatch);
        }
    }
}
