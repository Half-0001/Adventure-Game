using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

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
        Texture2D heartDead;
        SpriteFont spriteFont;
        Texture2D bossTextBox;
        //Texture2D spear;
        Song battleSong;


        //boss fight stuff//
        Random rand = new Random();
        int bossHealth = 100;
        bool bossDisplayingText = true;
        string bossText1 = "You have made a \nterrible mistake \ncoming here today.";
        string bossText2 = "You will not win, \n My power grows \nevery second";
        string bossText3 = "You . . . \n Will . . . \nDIE!";
        string bossText4 = "You may have won \nthe battle, but the \nwar is yet to come";
        bool gameOver;
        
        int textDraw; //variables for displaying text letter by letter whilst it appears
        float textDrawTimer;

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
        bool playerIsDead = false;

        //arena
        Rectangle arenaRectWhite = new Rectangle(295, 445, 0, 0);
        Rectangle arenaRectBlack = new Rectangle(300, 450, 0, 0);

        //list to store bullets in
        private List<Boss> bullets = new List<Boss>();
        
        private Vector2 bulletPosition;
        private Rectangle bulletRect = new Rectangle(0, 0, 10, 10);
        private Vector2 dir;
        private int bulletSpeed = 100;
        private int bulletSpread = -100;

        //spear stuff
        private List<Boss> spears = new List<Boss>();

        private Vector2 spearPosition;
        private Rectangle spearRect = new Rectangle(0, 0, 31, 5);
        private Vector2 spearDir;
        private int spearSpeed = 300;
        //private int spearSpread = -100;

        //timing variables
        private float timer = 0;
        private int stage = 0;
        private bool animateArenaExpand = true;
        private bool animateArenaRetract = false;

        //constructor
        public Boss(float randomX, float randomY, Vector2 position)
        {
            if (randomX != 0 && randomY != 0)
            {
                bulletPosition = position;
                dir = new Vector2(randomX, randomY);
                dir.Normalize();
            }
        }

        public Boss(float X, float Y, Vector2 position, string type)
        {

            if (type == "vertical")
            {
                spearRect = new Rectangle((int)X, (int)Y, 5, 30);
            }
            if (type == "horisontal")
            {
                spearRect = new Rectangle((int)X, (int)Y, 30, 5);
            }

            if (X != 0 && Y != 0)
            {
                spearPosition = position;
                spearDir = new Vector2(X, Y);
                spearDir.Normalize();
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
            bossTextBox = Content.Load<Texture2D>("bossTextBox");

            heart = Content.Load<Texture2D>("sprites/heart");
            heartDead = Content.Load<Texture2D>("sprites/heart-dead");

            //spear = Content.Load<Texture2D>("spear");

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });

            //font
            spriteFont = Content.Load<SpriteFont>("font");

            //boss music
            battleSong = Content.Load<Song>("audio/finalboss");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;
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
                bossSprite.Position = new Vector2(340, 242);
                bossSprite.Scale = new Vector2(0.8f, 0.8f);
                stage = 1;
                MediaPlayer.Play(battleSong);
            }

            if (playerIsDead)
            {
                PlayerDeath(gameTime);
            }

            if (playerIsDead == false)
            {
                if (playerHealth <= 0)
                    {
                        timer = 0;
                        playerIsDead = true;
                        
                    }

                if (playerCanAttack == false && bossHealth > 0)
                    bossSprite.Play("idle-down");


                //set the elapsed game time since last frame 
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                kState = Keyboard.GetState(); //set keyboard state

                if (playerCanAttack)
                    PlayerAttack(gameTime);

                bossSprite.Update(gameTime);

                BossTextManager(gameTime);

                if (playerCanAttack == false && bossHealth > 0)
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

                    //spear movement and rect
                    if (spears.Count != 0)
                    {
                        for (int i = 0; i < spears.Count; i++)
                        {
                            spears[i].spearPosition += spears[i].spearDir * spearSpeed * dt;

                            spears[i].spearRect.X = (int)spears[i].spearPosition.X;
                            spears[i].spearRect.Y = (int)spears[i].spearPosition.Y;
                        }
                    }

                    //getting hit by spears
                    for (int i = 0; i < spears.Count; i++)
                    {
                        if (playerRect.Intersects(spears[i].spearRect)) //getting hit by enemies
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
                            bulletSpeed = 200;
                            Level1(gameTime);
                        }

                        if (stage == 2)
                        {
                            bulletSpeed = 150;
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

                        if (stage == 5)
                        {
                            bulletSpeed = 100;
                            Level5(gameTime);
                        }

                        if (stage == 6)
                        {
                            bulletSpeed = 150;
                            Level6(gameTime);
                        }
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

            if (playerIsDead)
                DrawPlayerDeath(_spriteBatch);


            if (!gameOver && playerIsDead == false)
            {
                //draw boss and health rect
                bossSprite.Render(_spriteBatch);
                if (playerCanAttack)
                {
                    _spriteBatch.DrawString(spriteFont, "Boss Health:", new Vector2(295, 100), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_texture, new Rectangle(295, 145, 310, 40), Color.White);
                    _spriteBatch.Draw(_texture, new Rectangle(300, 150, 300, 30), Color.Black);
                    _spriteBatch.Draw(_texture, new Rectangle(300, 150, bossHealth * 3, 30), Color.DarkRed);
                }

                if (bossDisplayingText)
                {
                    _spriteBatch.Draw(bossTextBox, new Rectangle(530, 270, 207, 90), Color.White);
                    if (stage == 1)
                        _spriteBatch.DrawString(spriteFont, bossText1.Remove(textDraw, bossText1.Length - textDraw), new Vector2(560, 270), Color.Black, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                    if (stage == 3)
                        _spriteBatch.DrawString(spriteFont, bossText2.Remove(textDraw, bossText2.Length - textDraw), new Vector2(560, 270), Color.Black, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                    if (stage == 6)
                        _spriteBatch.DrawString(spriteFont, bossText3.Remove(textDraw, bossText3.Length - textDraw), new Vector2(560, 270), Color.Black, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                    if (stage == 7)
                        _spriteBatch.DrawString(spriteFont, bossText4.Remove(textDraw, bossText4.Length - textDraw), new Vector2(560, 270), Color.Black, 0f, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

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

                    //draw spears
                    for (int i = 0; i < spears.Count; i++)
                    {
                        _spriteBatch.Draw(_texture, spears[i].spearRect, Color.White);
                    }
                }

                if (playerCanAttack)
                {
                    DrawPlayerAttack(_spriteBatch);
                }
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

        private void Level2(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (timer > 2)
            //    while (bullets.Count < 100)
            //        bullets.Add(new Boss(rand.Next(-100, 50), rand.Next(-100, 50), new Vector2(270, 350)));

            if (timer > 2)
            {
                while (bullets.Count < 15)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(0, 400), new Vector2(200, 350)));

                while (spears.Count < 2)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 600), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 600), "horisontal")); //right
                    //spears.Add(new Boss(-0.01f, 100, new Vector2(450, 0), "vertical")); //down
                    //spears.Add(new Boss(0.01f, -100, new Vector2(450, 1200), "vertical")); //up
                }
            }

            if (timer > 4)
            {
                while (bullets.Count < 30)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(0, 400), new Vector2(700, 350)));

                while (spears.Count < 4)
                {
                    //spears.Add(new Boss(100, 0.01f, new Vector2(-130, 600), "horisontal")); //left
                    //spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 600), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(450, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(450, 1200), "vertical")); //up
                }
            }

            if (timer > 6)
            {
                while (bullets.Count < 45)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(0, 400), new Vector2(200, 350)));

                while (spears.Count < 6)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 600), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 600), "horisontal")); //right
                    //spears.Add(new Boss(-0.01f, 100, new Vector2(450, 0), "vertical")); //down
                    //spears.Add(new Boss(0.01f, -100, new Vector2(450, 1200), "vertical")); //up
                }
            }

            if (timer > 8)
            {
                while (bullets.Count < 60)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(0, 400), new Vector2(700, 350)));

                while (spears.Count < 8)
                {
                    //spears.Add(new Boss(100, 0.01f, new Vector2(-130, 600), "horisontal")); //left
                    //spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 600), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(450, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(450, 1200), "vertical")); //up
                }
            }

            if (timer > 12)
            {
                stage++;
                bullets.Clear();
                spears.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
                
            }
        }

        private void Level1(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 5)
            {
                    while (spears.Count < 4)
                    {
                        bossDisplayingText = false;
                        spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                        spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                        spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                        spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                    }
            }
            if (timer > 6)
            {
                while (spears.Count < 8)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }
            }
            if (timer > 7)
            {
                while (spears.Count < 12)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }
            }
            if (timer > 8)
            {
                while (spears.Count < 16)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }
            }
            if (timer > 9)
            {
                while (spears.Count < 20)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }
            }

            if (timer > 12)
            {
                while (bullets.Count < 10)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(-400, 400), new Vector2(270, 600)));
            }

            if (timer > 14)
                while (bullets.Count < 20)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(-400, 400), new Vector2(630, 600)));

            if (timer > 16)
                while (bullets.Count < 30)
                    bullets.Add(new Boss(rand.Next(0, 400), rand.Next(-400, 400), new Vector2(270, 600)));

            if (timer > 18)
                while (bullets.Count < 40)
                    bullets.Add(new Boss(rand.Next(-400, 0), rand.Next(-400, 400), new Vector2(630, 600)));

            if (timer > 22)
            {
                stage++;
                bullets.Clear();
                spears.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void Level3(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer > 5)
            {
                bossDisplayingText = false;
                if (bulletSpread + 1 != 101)
                    bulletSpread += 30;
            }

            if (bullets.Count % 20 == 0)
                bulletSpread = -100;

            if (timer > 5)
            {
                if (bullets.Count < 20)
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                if (spears.Count < 2)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }

                
            }


            if (timer > 7)
            {
                if (bullets.Count < 40)
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));
                if (spears.Count < 4)
                {
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                }

            }



            if (timer > 9)
            {
                if (bullets.Count < 60)
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                if (spears.Count < 6)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(playerPosition.X - 500, playerPosition.Y), "horisontal")); //left
                    spears.Add(new Boss(0.01f, -100, new Vector2(playerPosition.X, playerPosition.Y + 500), "vertical")); //up
                }
            }


            if (timer > 11)
            {
                if (bullets.Count < 80)
                    bullets.Add(new Boss(-100, bulletSpread, new Vector2(630, 600)));
                if (spears.Count < 8)
                {
                    spears.Add(new Boss(-100, -0.01f, new Vector2(playerPosition.X + 500, playerPosition.Y), "horisontal")); //right
                    spears.Add(new Boss(-0.01f, 100, new Vector2(playerPosition.X, playerPosition.Y - 500), "vertical")); //down
                }
            }


            if (timer > 15)
            {
                stage++;
                bullets.Clear();
                spears.Clear();
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
            {
                if (bullets.Count < 40)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread + 15, new Vector2(630, 600)));
                }
                while (spears.Count < 4)
                {
                    spears.Add(new Boss(-0.01f, 100, new Vector2(355, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(355, 1200), "vertical")); //up
                    spears.Add(new Boss(-0.01f, 100, new Vector2(545, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(545, 1200), "vertical")); //up
                }
            }

            if (timer > 4)
            {
                if (bullets.Count < 80)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread + 15, new Vector2(630, 600)));
                }
                while (spears.Count < 8)
                {
                    spears.Add(new Boss(-0.01f, 100, new Vector2(355, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(355, 1200), "vertical")); //up
                    spears.Add(new Boss(-0.01f, 100, new Vector2(545, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(545, 1200), "vertical")); //up
                }
            }

            if (timer > 6)
            {
                if (bullets.Count < 120)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread + 15, new Vector2(630, 600)));
                }
                while (spears.Count < 12)
                {
                    spears.Add(new Boss(-0.01f, 100, new Vector2(355, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(355, 1200), "vertical")); //up
                    spears.Add(new Boss(-0.01f, 100, new Vector2(545, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(545, 1200), "vertical")); //up
                }
            }

            if (timer > 8)
            {
                if (bullets.Count < 160)
                {
                    bullets.Add(new Boss(100, bulletSpread, new Vector2(270, 600)));
                    bullets.Add(new Boss(-100, bulletSpread + 15, new Vector2(630, 600)));
                }
                while (spears.Count < 16)
                {
                    spears.Add(new Boss(-0.01f, 100, new Vector2(355, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(355, 1200), "vertical")); //up
                    spears.Add(new Boss(-0.01f, 100, new Vector2(545, 0), "vertical")); //down
                    spears.Add(new Boss(0.01f, -100, new Vector2(545, 1200), "vertical")); //up
                }
            }

            if (timer > 13)
            {
                stage++;
                bullets.Clear();
                spears.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void PlayerAttack(GameTime gameTime)
        {
            if (selectedButton == 3)
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (bossHealth <= 0)
            {
                MediaPlayer.Stop();
                if (timer > 2)
                {
                    bossDisplayingText = true;
                }

                if (timer > 6)
                {
                    BossDeath(gameTime);
                    return;
                }
            }

            if (playerAttackDelay != true)
            {
                bossSprite.Play("idle-down");
                if (selectedButton == 1)
                {
                    if (kState.IsKeyDown(Keys.D))
                        selectedButton = 2;

                    if (kState.IsKeyDown(Keys.Space))
                    {

                        //playerCanAttack = false;
                        playerAttackDelay = true;
                        selectedButton = 3;
                    }
                }

                else if (selectedButton == 2)
                {
                    if (kState.IsKeyDown(Keys.A))
                        selectedButton = 1;

                    if (kState.IsKeyDown(Keys.Space))
                    {
                        playerIsDead = true;
                    }
                }
            }


            if (playerAttackDelay)
            {
                bossSprite.Play("attacked");
                if (bossSprite.CurrentFrameIndex == 27)
                {
                    bossHealth -= 19;
                    timer = 0;
                    playerAttackDelay = false;
                }
            }
            if (timer > 2)
            {
                if (stage == 3 || stage == 6)
                {
                    bossDisplayingText = true;
                    textDraw = 0;
                    textDrawTimer = 0; //tell the game to display text on stages 3 and 6 and reset the variable for it
                }

                if (bossHealth > 0)
                {
                    animateArenaExpand = true;
                    playerCanAttack = false;
                    playerAttackDelay = false;
                }

                //Debug.WriteLine(timer);

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
                        timer = 0;
                    }
                }

                else
                {
                    timer = 0;
                    arenaRectWhite.Width = 0;
                    arenaRectWhite.Height = 0;
                    selectedButton = 1;
                    textDraw = 0;
                    textDrawTimer = 0;
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

        private void Level5 (GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 2)
            {
                if (bullets.Count < 40)
                {
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(270, 600)));
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(630, 600)));
                }
                while (spears.Count < 2)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 480), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 480), "horisontal")); //right
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 720), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 720), "horisontal")); //right
                }
            }


            if (timer > 4)
            {
                if (bullets.Count < 80)
                {
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(270, 600)));
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(630, 600)));
                }
                while (spears.Count < 4)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 480), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 480), "horisontal")); //right
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 720), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 720), "horisontal")); //right
                }
            }


            if (timer > 6)
            {
                if (bullets.Count < 120)
                {
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(270, 600)));
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(630, 600)));
                }
                while (spears.Count < 6)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 480), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 480), "horisontal")); //right
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 720), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 720), "horisontal")); //right
                }
            }


            if (timer > 8)
            {
                if (bullets.Count < 160)
                {
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(270, 600)));
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(-400, 400), new Vector2(630, 600)));
                }
                while (spears.Count < 8)
                {
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 480), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 480), "horisontal")); //right
                    spears.Add(new Boss(100, 0.01f, new Vector2(-130, 720), "horisontal")); //left
                    spears.Add(new Boss(-100, -0.01f, new Vector2(1000, 720), "horisontal")); //right
                }
            }


            if (timer > 12)
            {
                stage++;
                bullets.Clear();
                spears.Clear();
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void Level6(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer > 5 && timer < 6)
            {
                bossDisplayingText = false;
                while (bullets.Count < 30)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));
            }



            if (timer > 6 && timer < 7)
            {
                int random = rand.Next(1, 3);

                while (bullets.Count < 38)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }


            if (timer > 8)
                while (bullets.Count < 68)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 9 && timer < 10)
            {
                int random = rand.Next(1, 3);
                while (bullets.Count < 76)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }

            if (timer > 10)
                while (bullets.Count < 106)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 11 && timer < 12)
            {
                int random = rand.Next(1, 3);

                while (bullets.Count < 121)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }


            if (timer > 12)
                while (bullets.Count < 151)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 13 && timer < 14)
            {
                int random = rand.Next(1, 3);
                while (bullets.Count < 166)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }

            if (timer > 14)
                while (bullets.Count < 196)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 15 && timer < 16)
            {
                int random = rand.Next(1, 3);

                while (bullets.Count < 204)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }


            if (timer > 16)
                while (bullets.Count < 234)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 17 && timer < 18)
            {
                int random = rand.Next(1, 3);
                while (bullets.Count < 242)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }

            if (timer > 18)
                while (bullets.Count < 272)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 19 && timer < 20)
            {
                int random = rand.Next(1, 3);

                while (bullets.Count < 280)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }


            if (timer > 20)
                while (bullets.Count < 310)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));

            if (timer > 21 && timer < 22)
            {
                int random = rand.Next(1, 3);
                while (bullets.Count < 318)
                {
                    if (random == 1)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(460, 600))));

                    if (random == 2)
                        bullets.Add(new Boss(100, 0.01f, new Vector2(270, rand.Next(600, 730))));
                }
            }

            if (timer > 22)
                while (bullets.Count < 348)
                    bullets.Add(new Boss(rand.Next(-400, 400), rand.Next(0, 400), new Vector2(450, 350)));


            if (timer > 26)
            {
                stage++;
                bullets.Clear();
                //playerCanAttack = true;
                animateArenaRetract = true;
                timer = 0;
            }
        }

        private void BossTextManager(GameTime gameTime)
        {
            if (stage == 1)
            {
                if (timer < 5)
                {
                    if (textDraw < bossText1.Length)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (textDrawTimer > 0.05)
                        {
                            textDraw++;
                            textDrawTimer = 0;
                        }
                    }
                    if (textDraw == bossText1.Length && textDrawTimer < 4)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }

            if (stage == 3)
            {
                if (timer < 5)
                {
                    if (textDraw < bossText2.Length)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (textDrawTimer > 0.05)
                        {
                            textDraw++;
                            textDrawTimer = 0;
                        }
                    }
                    if (textDraw == bossText2.Length && textDrawTimer < 4)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }

            if (stage == 6)
            {
                if (timer < 5)
                {
                    if (textDraw < bossText3.Length)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (textDrawTimer > 0.05)
                        {
                            textDraw++;
                            textDrawTimer = 0;
                        }
                    }
                    if (textDraw == bossText3.Length && textDrawTimer < 4)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }

            if (stage == 7)
            {
                if (timer > 2)
                {
                    if (textDraw < bossText4.Length)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (textDrawTimer > 0.05)
                        {
                            textDraw++;
                            textDrawTimer = 0;
                        }
                    }
                    if (textDraw == bossText4.Length && textDrawTimer < 4)
                    {
                        textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        private void BossDeath(GameTime gameTime)
        {
            //timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            bossSprite.Play("death");
            bossDisplayingText = false;
            if (bossSprite.CurrentFrameIndex == 37)
            {
                //bossSprite.Position = new Vector2(1000, 1000);
                gameOver = true;
            }
        }

        private void PlayerDeath(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            MediaPlayer.Stop();

            if (timer > 4)
            {
                Debug.WriteLine("true");
                bullets.Clear();
                bossDisplayingText = true;
                animateArenaExpand = true;
                arenaRectBlack = new Rectangle(300, 450, 0, 0);
                arenaRectWhite = new Rectangle(295, 445, 0, 0);
                textDraw = 0;
                textDrawTimer = 0;
                stage = 0;
                playerHealth = 100;
                bossHealth = 100;
                //MediaPlayer.Stop();
                MediaPlayer.Play(battleSong);
                playerIsDead = false;

            }
        }

        private void DrawPlayerDeath(SpriteBatch _spriteBatch)
        {
            if (timer < 2)
            {
                _spriteBatch.Draw(heart, playerRect, Color.Red);
            }
            if (timer > 2)
            {
                _spriteBatch.Draw(heartDead, new Rectangle(playerRect.X - 4, playerRect.Y - 4, playerRect.Width + 8, playerRect.Height + 8), Color.Red);
            }
        }
    }
}
