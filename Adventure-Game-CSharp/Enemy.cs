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
        public List<Enemy> enemies = new List<Enemy>();

        private AnimatedSprite enemySprite;
        private Vector2 position = new Vector2();
        private int health;
        private int speed;
        private Rectangle hitbox = new Rectangle();

        Texture2D _texture;

        public Enemy(int positionX, int positionY, AnimatedSprite sprite)
        {
            health = 100; 
            speed = 90;
            position.X = positionX;
            position.Y = positionY;
            hitbox = new Rectangle(positionX + 11, positionY + 11, 10, 10);
            enemySprite = sprite;
        }
        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            //  Load the asprite file from the content pipeline.
            AsepriteDocument asepritefile = Content.Load<AsepriteDocument>("Enemy 15-1");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            enemySprite = new AnimatedSprite(asepritefile);
            enemySprite.Scale = new Vector2(1.0f, 1.0f);
            enemySprite.Y = _resolution.Y - (enemySprite.Height * enemySprite.Scale.Y) - 16;

            //add enemies to list
            enemies.Add(new Enemy(678, 1768, enemySprite));

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].enemySprite.Update(dt);
                enemies[i].enemySprite.Position = new Vector2(enemies[i].position.X, enemies[i].position.Y);
            }
            
            
        }

        public void Draw(SpriteBatch _spriteBatch, bool debugMode)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].enemySprite.Render(_spriteBatch); //draw sprite
            }

            if (debugMode)
                for (int i = 0; i < enemies.Count; i++)
                {
                    _spriteBatch.Draw(_texture, enemies[i].hitbox, Color.White); //draw hitbox
                }

        }
    }
}
