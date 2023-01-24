using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;
using System.Diagnostics;

namespace Adventure_Game_CSharp
{
    internal class Boss
    {

        //pre battle
        AsepriteDocument asepritefile;
        AnimatedSprite bossSprite;
        Texture2D _texture;

        //boss fight stuff//
        Random rand = new Random();

        //player 
        Vector2 playerPosition = new Vector2(450, 600);
        Rectangle playerRect = new Rectangle(0, 0, 20, 20);
        KeyboardState kState = new KeyboardState();
        int speed = 50;
        

        //lists to store bullets in
        private List<Boss> bullets = new List<Boss>();
        private Vector2 bulletPosition;
        private Rectangle bulletRect = new Rectangle(0, 0, 10, 10);
        private Vector2 dir;

        //constructor
        public Boss(int randomX, int randomY)
        {
            if (randomX != 0 && randomY != 0)
            {
                bulletPosition = new Vector2(300, 300);
                dir = new Vector2(randomX, randomY);
                dir.Normalize();
            }
        }

        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            asepritefile = Content.Load<AsepriteDocument>("sprites/Male 09-1");
            bossSprite = new AnimatedSprite(asepritefile);
            bossSprite.Scale = new Vector2(1.0f, 1.0f);
            bossSprite.Y = _resolution.Y - (bossSprite.Height * bossSprite.Scale.Y) - 16;
            bossSprite.Position = new Vector2(2477, 760);

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public void Update(string eventTrigger, GameTime gameTime)
        {
            if (eventTrigger == "Boss" || eventTrigger == "Boss text 2")
            {
                if (eventTrigger == "Boss")
                {
                    if (bossSprite.Position != new Vector2(2477, 845))
                    {
                        bossSprite.Position = new Vector2(bossSprite.Position.X, bossSprite.Position.Y + 0.25f);
                        bossSprite.Play("walk-down");
                    }
                    if (bossSprite.Position == new Vector2(2477, 845))
                    {
                        bossSprite.Play("idle-down");
                    }

                }

                if (eventTrigger == "Boss text 2")
                {
                    bossSprite.Play("idle-up");
                }
            }
            else
                bossSprite.Play("idle-up");

            bossSprite.Update(gameTime);
        }

        public void BossBattle(GameTime gameTime)
        {
            //set the elapsed game time since last frame 
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            kState = Keyboard.GetState();
            //set speed to be half if player is moving diagonally, so the player speed is not doubled
            if ((kState.IsKeyDown(Keys.W) && (kState.IsKeyDown(Keys.A) || kState.IsKeyDown(Keys.D))) || (kState.IsKeyDown(Keys.S) && (kState.IsKeyDown(Keys.A) || kState.IsKeyDown(Keys.D))))
                speed = 35;
            else
                speed = 50;


            if (kState.IsKeyDown(Keys.W)) //player movement and boundries
                if (playerPosition.Y > 460) 
                    playerPosition.Y -= speed * dt;

            if (kState.IsKeyDown(Keys.S))
                if (playerPosition.Y < 740)
                    playerPosition.Y += speed * dt;

            if (kState.IsKeyDown(Keys.A))
                if (playerPosition.X > 310)
                    playerPosition.X -= speed * dt;

            if (kState.IsKeyDown(Keys.D))
                if (playerPosition.X < 590)
                    playerPosition.X += speed * dt;

            //set player rect to its positions
            playerRect.X = (int)playerPosition.X - 10;
            playerRect.Y = (int)playerPosition.Y - 10;

            Debug.WriteLine(bullets.Count);

            //bullet movement and rect
            if (bullets.Count != 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].bulletPosition += bullets[i].dir;

                    bullets[i].bulletRect.X = (int)bullets[i].bulletPosition.X;
                    bullets[i].bulletRect.Y = (int)bullets[i].bulletPosition.Y;
                }
            }

            Level1();


        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            bossSprite.Render(_spriteBatch);
        }

        public void DrawBossBattle(SpriteBatch _spriteBatch, GraphicsDevice graphics)
        {
            graphics.Clear(Color.Black);

            //draw bounding box 
            _spriteBatch.Draw(_texture, new Rectangle(295, 445, 310, 310), Color.White);
            _spriteBatch.Draw(_texture, new Rectangle(300, 450, 300, 300), Color.Black);

            //draw player rect
            _spriteBatch.Draw(_texture, playerRect, Color.Red);

            //draw bullets 
            for (int i = 0; i < bullets.Count; i++)
            {
                _spriteBatch.Draw(_texture, bullets[i].bulletRect, Color.White);
            }
        }

        public void Level1()
        {
            while (bullets.Count < 10)
                bullets.Add(new Boss(rand.Next(0, 360), rand.Next(0, 360)));
        }
    }
}
