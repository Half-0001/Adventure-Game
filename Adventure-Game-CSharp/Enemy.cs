using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;
using System.Diagnostics;

namespace Adventure_Game_CSharp
{
    internal class Enemy
    {
        public List<Enemy> enemies = new List<Enemy>();

        Random random = new Random();

        private AnimatedSprite enemySprite;
        public Vector2 position = new Vector2();
        public int health;
        private int speed;
        public Rectangle hitbox = new Rectangle();
        public Rectangle attackHitbox = new Rectangle();
        //private float timer = 0;

        AsepriteDocument asepritefile;
        AsepriteDocument asepritefile2;
        Texture2D _texture;

        Point resolution;

        public Enemy(int positionX, int positionY, AsepriteDocument asepritefile, Point _resolution)
        {
            health = 100; 
            speed = 30;
            position.X = positionX;
            position.Y = positionY;
            hitbox = new Rectangle(positionX + 11, positionY + 11, 10, 10);
            attackHitbox = new Rectangle(positionX + 21, positionY + 11, 10, 10);

            if (asepritefile != null) //each sprite has its own position variables, if "null" is passed into the creator (at the start of Game1.cs) then it throws an error. 
            {
                enemySprite = new AnimatedSprite(asepritefile);
                enemySprite.Scale = new Vector2(1.0f, 1.0f);
                enemySprite.Y = _resolution.Y - (enemySprite.Height * enemySprite.Scale.Y) - 16;
            }
            else
                enemySprite = null;

        }
        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            //  Load the aseprite file from the content pipeline.
            asepritefile = Content.Load<AsepriteDocument>("Enemy 15-1");
            asepritefile2 = Content.Load<AsepriteDocument>("Enemy 15-2");
            resolution = _resolution;
            //add enemies to list
            for (int i = 0; i < 10; i++)
                enemies.Add(new Enemy(random.Next(800, 1305), random.Next(1566, 2340), asepritefile, _resolution));

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].hitbox = new Rectangle((int)enemies[i].position.X + 11, (int)enemies[i].position.Y + 11, 10, 10);
                

                enemies[i].enemySprite.Update(dt);
                enemies[i].enemySprite.Position = new Vector2(enemies[i].position.X, enemies[i].position.Y);

                if (enemies[i].health > 0)
                {
                    if (Vector2.Distance(playerPosition, enemies[i].position) < 300)
                    {
                        Vector2 dir = enemies[i].position - playerPosition;
                        dir.Normalize();
                        enemies[i].attackHitbox = new Rectangle(enemies[i].hitbox.X + (int)(dir * 6).X, (int)enemies[i].hitbox.Y + (int)(dir * 6).Y, 10, 10);
                        enemies[i].position -= dir * speed * dt;
                        if (dir.X > 0)
                            enemies[i].enemySprite.Play("walk-left");
                        if (dir.X < 0)
                            enemies[i].enemySprite.Play("walk-right");
                    }
                }


                if (enemies[i].health <= 0)
                {
                    enemies[i].enemySprite.Play("death");
                    enemies[i].attackHitbox = new Rectangle(0, 0, 0, 0);
                    if (enemies[i].enemySprite.CurrentFrameIndex == 21)
                        enemies.RemoveAt(i);
                }
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
                    _spriteBatch.Draw(_texture, enemies[i].attackHitbox, Color.Green); //draw hitbox
                }
        }

        public void AddEnemies(int amount, int xmin, int xmax, int ymin, int ymax)
        {
            for (int i = 0; i < amount; i++)
                enemies.Add(new Enemy(random.Next(xmin, xmax), random.Next(ymin, ymax), asepritefile, resolution));
        }

        public void AddEnemies2(int amount, int xmin, int xmax, int ymin, int ymax)
        {
            for (int i = 0; i < amount; i++)
                enemies.Add(new Enemy(random.Next(xmin, xmax), random.Next(ymin, ymax), asepritefile2, resolution));
        }

        public void Restart(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            enemies.Clear();
            LoadContent(Content, _resolution, _graphics);
        }
    }
}
