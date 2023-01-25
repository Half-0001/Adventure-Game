using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Adventure_Game_CSharp
{
    internal class Boss
    {

        //textures and sprites
        AsepriteDocument asepritefile;
        AnimatedSprite bossSprite;
        AsepriteDocument bossSpriteUpscaled;
        Texture2D _texture;
        Texture2D heart;
        SpriteFont spriteFont;


        //boss fight stuff//
        Random rand = new Random();
        int bossHealth = 100;

        //player 
        Vector2 playerPosition = new Vector2(450, 600);
        Rectangle playerRect = new Rectangle(0, 0, 20, 20);
        KeyboardState kState = new KeyboardState();
        int speed = 70;
        int playerHealth = 100;
        bool canBeAttacked = true;
        float cooldownTimer; //timer for invincibility cooldown before the player can be attacked again
        bool playerCanAttack = false;
        int selectedButton = 1;
        bool playerAttackDelay = false;

        //arena 
        //Rectangle arenaRectWhite = new Rectangle(295, 445, 300, 300);
        //Rectangle arenaRectBlack = new Rectangle(300, 450, 290, 290);
        Rectangle arenaRectWhite = new Rectangle(295, 445, 0, 0);
        Rectangle arenaRectBlack = new Rectangle(300, 450, 0, 0);

        //list to store bullets in
        private List<Boss> bullets = new List<Boss>();
        
        private Vector2 bulletPosition;
        private Rectangle bulletRect = new Rectangle(0, 0, 10, 10);
        private Vector2 dir;
        private int bulletSpeed = 100;
        private int bulletSpread = -100;

        //timing variables
        private float timer = 0;
        private int stage = 0;
        private bool animateArenaExpand = true;
        private bool animateArenaRetract = false;

        //constructor
        public Boss(int randomX, int randomY, Vector2 position)
        {
            if (randomX != 0 && randomY != 0)
            {
                bulletPosition = position;
                dir = new Vector2(randomX, randomY);
                dir.Normalize();
            }
        }

        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            asepritefile = Content.Load<AsepriteDocument>("sprites/Male 09-1");
            bossSpriteUpscaled = Content.Load<AsepriteDocument>("sprites/Male 09-1-Upscaled");
            bossSprite = new AnimatedSprite(asepritefile);
            bossSprite.Scale = new Vector2(1.0f, 1.0f);
            bossSprite.Y = _resolution.Y - (bossSprite.Height * bossSprite.Scale.Y) - 16;
            bossSprite.Position = new Vector2(2477, 760);

            heart = Content.Load<Texture2D>("sprites/heart");

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });

            //font
            spriteFont = Content.Load<SpriteFont>("font");
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
            //boss sprite
            if (stage == 0)
            {
                bossSprite = new AnimatedSprite(bossSpriteUpscaled);
                bossSprite.Position = new Vector2(340, 250);
                bossSprite.Scale = new Vector2(0.8f, 0.8f);
                stage = 1;
            }
            bossSprite.Play("idle-down");
            bossSprite.Update(gameTime);

            //set the elapsed game time since last frame 
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            kState = Keyboard.GetState(); //set keyboard state

            if (playerCanAttack)
                PlayerAttack(gameTime);
                
            if (playerCanAttack == false)
            {
                MovePlayer(dt);

                //bullet movement and rect
                if (bullets.Count != 0)
                {
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        bullets[i].bulletPosition += bullets[i].dir * bulletSpeed * dt;

                        bullets[i].bulletRect.X = (int)bullets[i].bulletPosition.X;
                        bullets[i].bulletRect.Y = (int)bullets[i].bulletPosition.Y;
                    }
                }

                //getting hit by bullets
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (playerRect.Intersects(bullets[i].bulletRect)) //getting hit by enemies
                    {
                        if (canBeAttacked)
                        {
                            playerHealth -= 15;
                            cooldownTimer = 0;
                            canBeAttacked = false;
                            break;
                        }
                    }
                }

                if (canBeAttacked == false)
                {
                    cooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds; //cooldown before the player can be attacked again
                    if (cooldownTimer > 2)
                        canBeAttacked = true;
                }

                if (animateArenaExpand)
                {
                    AnimateArenaExpand(gameTime);
                }

                if (animateArenaRetract)
                {
                    AnimateArenaRetract(gameTime);
                }
                
                if (animateArenaExpand == false && animateArenaRetract == false)
                {
                    if (stage == 1)
                    {
                        bulletSpeed = 150;
                        Level1(gameTime);
                    }

                    if (stage == 2)
                    {
                        bulletSpeed = 100;
                        Level2(gameTime);
                    }

                    if (stage == 3)
                    {
                        bulletSpeed = 150;
                        Level3(gameTime);
                    }

                    if (stage == 4)
                    {
                        bulletSpeed = 80;
                        Level4(gameTime);
                    }
                }

            }

        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            bossSprite.Render(_spriteBatch);
        }

        public void DrawBossBattle(SpriteBatch _spriteBatch, GraphicsDevice graphics)
        {
            graphics.Clear(Color.Black);

            //draw boss and health rect
            bossSprite.Render(_spriteBatch);
            if (playerCanAttack)
            {
                _spriteBatch.DrawString(spriteFont, "Boss Health:", new Vector2(295, 100), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                _spriteBatch.Draw(_texture, new Rectangle(295, 145, 310, 40), Color.White);
                _spriteBatch.Draw(_texture, new Rectangle(300, 150, 300, 30), Color.Black);
                _spriteBatch.Draw(_texture, new Rectangle(300, 150, bossHealth * 3, 30), Color.DarkRed);
            }

            //draw bounding box 
            _spriteBatch.Draw(_texture, arenaRectWhite, Color.White);
            _spriteBatch.Draw(_texture, arenaRectBlack, Color.Black);

            if (playerCanAttack == false)
            {
                //draw player rect and health
                if (animateArenaExpand == false && animateArenaRetract == false)
                {
                    _spriteBatch.Draw(heart, playerRect, Color.Red);
                    _spriteBatch.DrawString(spriteFont, "Health:", new Vector2(295, 760), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_texture, new Rectangle(295, 805, 310, 40), Color.White);
                    _spriteBatch.Draw(_texture, new Rectangle(300, 810, 300, 30), Color.Black);
                    _spriteBatch.Draw(_texture, new Rectangle(300, 810, playerHealth * 3, 30), Color.DarkGreen);
                }


                //draw bullets 
                for (int i = 0; i < bullets.Count; i++)
                {
                    _spriteBatch.Draw(_texture, bullets[i].bulletRect, Color.White);
                }
            }

            if (playerCanAttack)
            {
                DrawPlayerAttack(_spriteBatch);
            }
        }
        private void MovePlayer(float dt)
        {
            //set speed to be half if player is moving diagonally, so the player speed is not doubled
            if ((kState.IsKeyDown(Keys.W) && (kState.IsKeyDown(Keys.A) || kState.IsKeyDown(Keys.D))) || (kState.IsKeyDown(Keys.S) && (kState.IsKeyDown(Keys.A) || kState.IsKeyDown(Keys.D))))
                speed = 55;
            else
                speed = 70;


            if (kState.IsKeyDown(Keys.W)) //player movement and boundries
                if (playerPosition.Y > 460) //top
                    playerPosition.Y -= speed * dt;

            if (kState.IsKeyDown(Keys.S))
                if (playerPosition.Y < 730) //down
                    playerPosition.Y += speed * dt;

            if (kState.IsKeyDown(Keys.A))
                if (playerPosition.X > 310) //left
                    playerPosition.X -= speed * dt;

            if (kState.IsKeyDown(Keys.D))
                if (playerPosition.X < 580) //right
                    playerPosition.X += speed * dt;

            //set player rect to its positions
            playerRect.X = (int)playerPosition.X - 10;
            playerRect.Y = (int)playerPosition.Y - 10;
        }

        private void Level1(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (timer > 2)
            //    while (bullets.Count < 100)
            //        bullets.Add(new Boss(rand.Next(-100, 50), rand.Next(-100, 50), new Vector2(270, 350)));

            if (timer > 5)
                while (bullets.Count < 15)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(0, 400), new Vector2(200, 350)));

            if (timer > 7)
                while (bullets.Count < 30)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(0, 400), new Vector2(700, 350)));

            if (timer > 9)
                while (bullets.Count < 45)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(0, 400), new Vector2(200, 350)));

            if (timer > 11)
                while (bullets.Count < 60)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(0, 400), new Vector2(700, 350)));

            if (timer > 17)
            {
                stage++;
                bullets.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
                
            }
        }

        private void Level2(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 5)
                while (bullets.Count < 10)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(-400, 400), new Vector2(270, 600)));

            if (timer > 8)
                while (bullets.Count < 20)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(-400, 400), new Vector2(630, 600)));

            if (timer > 11)
                while (bullets.Count < 30)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(-400, 400), new Vector2(270, 600)));

            if (timer > 14)
                while (bullets.Count < 40)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(-400, 400), new Vector2(630, 600)));

            if (timer > 20)
            {
                stage++;
                bullets.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void Level3(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer > 2)
                if (bulletSpread + 1 != 101)
                    bulletSpread += 30;

            if (bullets.Count % 20 == 0)
                bulletSpread = -100;

            if (timer > 2)
                if (bullets.Count < 20)
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));

            if (timer > 4)
                if (bullets.Count < 40)
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));


            if (timer > 6)
                if (bullets.Count < 60)
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));

            if (timer > 8)
                if (bullets.Count < 80)
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));

            if (timer > 10)
            {
                stage++;
                bullets.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void Level4(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer > 2)
                if (bulletSpread + 1 != 101)
                    bulletSpread += 30;

            if (bullets.Count % 40 == 0)
                bulletSpread = -100;

            if (timer > 2)
                if (bullets.Count < 40)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));
                }

            if (timer > 4)
                if (bullets.Count < 80)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));
                }

            if (timer > 9)
            {
                stage++;
                bullets.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void PlayerAttack(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (playerAttackDelay != true)
            {
                if (selectedButton == 1)
                {
                    if (kState.IsKeyDown(Keys.D))
                        selectedButton = 2;

                    if (kState.IsKeyDown(Keys.Space))
                    {
                        bossHealth -= 20;
                        timer = 0;
                        //playerCanAttack = false;
                        playerAttackDelay = true;
                    }
                }

                else if (selectedButton == 2)
                {
                    if (kState.IsKeyDown(Keys.A))
                        selectedButton = 1;
                }
            }


            if (playerAttackDelay)
            {
                if (timer > 3)
                {
                    animateArenaExpand = true;
                    playerCanAttack = false;
                }
            }

        }

        private void DrawPlayerAttack(SpriteBatch _spriteBatch)
        {

            if (selectedButton == 1)
            {
                _spriteBatch.Draw(_texture, new Rectangle(230, 550, 140, 40), Color.Yellow);
                _spriteBatch.Draw(_texture, new Rectangle(530, 550, 150, 40), Color.White);

            }
            if (selectedButton == 2)
            {
                _spriteBatch.Draw(_texture, new Rectangle(230, 550, 140, 40), Color.White);
                _spriteBatch.Draw(_texture, new Rectangle(530, 550, 150, 40), Color.Yellow);
            }

            _spriteBatch.Draw(_texture, new Rectangle(235, 555, 130, 30), Color.SlateGray);
            _spriteBatch.Draw(_texture, new Rectangle(535, 555, 140, 30), Color.SlateGray);
            _spriteBatch.DrawString(spriteFont, "Attack", new Vector2(260, 550), Color.White, 0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
            _spriteBatch.DrawString(spriteFont, "Give Up", new Vector2(560, 550), Color.White, 0f, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
        }

        private void AnimateArenaRetract(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (arenaRectWhite.Width > 10)
            {
                if (timer > 0.01)
                {
                    arenaRectWhite.Width -= 6;
                    arenaRectBlack.Width -= 6;
                    
                    timer = 0;

                }
            }

            if (arenaRectWhite.Width <= 10)
            {
                arenaRectWhite.Width = 10;
                arenaRectBlack.Width = 0;

                if (arenaRectWhite.Height > 0)
                {
                    if (timer > 0.01)
                    {
                        arenaRectWhite.Height -= 6;
                        //arenaRectBlack.Height -= 6;
                        timer = 0;
                    }
                }

                else
                {
                    timer = 0;
                    arenaRectWhite.Width = 0;
                    arenaRectWhite.Height = 0;
                    playerCanAttack = true;
                    animateArenaRetract = false;
                }
            }


        }

        private void AnimateArenaExpand(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (arenaRectWhite.Height != 300)
            {
                arenaRectWhite.Width = 10;
                if (timer > 0.01)
                {
                    arenaRectWhite.Height += 6;
                    //arenaRectBlack.Height += 6;
                    timer = 0;
                }
            }

            if (arenaRectWhite.Height == 300)
            {
                arenaRectBlack.Height = 290;

                if (arenaRectWhite.Width < 300)
                {
                    if (timer > 0.01)
                    {
                        arenaRectWhite.Width += 6;
                        arenaRectBlack.Width += 6;
                        timer = 0;
                    }
                }
                else
                {
                    timer = 0;
                    arenaRectBlack.Width = 290;
                    arenaRectWhite.Width = 300;
                    animateArenaExpand = false;
                }
            }
        }
    }
}
