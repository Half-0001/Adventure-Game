using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;

namespace Adventure_Game_CSharp
{
    internal class Player
    {
        enum Dir
        {
            Down,
            Up,
            Left,
            Right,
        }

        private KeyboardState kStateOld;
        private Vector2 position = new Vector2(500, 600); //new Vector2(1792, 2042);
        private int speed = 100;
        private Dir direction = Dir.Down;
        public Rectangle playerRect = new Rectangle(0, 0, 32, 36);
        private Rectangle coverScreen = new Rectangle(0, 0, 500, 500);
        private Rectangle attackRect = new Rectangle(0, 0, 0, 0);
        private Texture2D _texture;
        public List<string> collisionDir = new List<string> { "" };
        private int amountOfCollisionsOld = 0;
        private string eventTrigger = "";
        private float timer = 0;
        private AnimatedSprite _sprite;
        private int health = 100;
        private bool canBeAttacked = true;
        private bool attacking = false;
        private List<string> inventory = new List<string> { "Bottle 'O Pop ", "Bones" };
        public bool accessingInventory = false;
        private int level = 1;
        private bool isMoving;

        private string npcText = "In order for me to allow you passage you must first \nslay all the ghosts in this area";
        private int textDraw;
        private float textDrawTimer;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            //  Load the asprite file from the content pipeline.
            AsepriteDocument aseprite = Content.Load<AsepriteDocument>("Male 01");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            _sprite = new AnimatedSprite(aseprite);
            _sprite.Scale = new Vector2(1.0f, 1.0f);
            _sprite.Y = _resolution.Y - (_sprite.Height * _sprite.Scale.Y) - 16;

            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }
        public void PlayerUpdate(GameTime gameTime, int amountOfCollisions, string eventRectName, List<Enemy> enemies)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Position = new Vector2(position.X - 20, position.Y - 20);
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;
            coverScreen.X = (int)position.X - 250;
            coverScreen.Y = (int)position.Y - 250;

            //check if player is accessing inventory 
            if (kState.IsKeyDown(Keys.Tab) && !kStateOld.IsKeyDown(Keys.Tab))
            {
                accessingInventory = !accessingInventory;
            }

            if (!accessingInventory)
            {
                if (!attacking)
                    isMoving = SetDirection(kState);
                
                if (isMoving && !attacking) //move player and play animations
                {
                    MovePlayer(dt);
                }

                if (!isMoving && !attacking)
                {
                    PlayIdleAnimations();
                }

                if (attackRect != new Rectangle(0, 0, 0, 0)) //reset the attack rect
                    attackRect = new Rectangle(0, 0, 0, 0);

                if (kState.IsKeyDown(Keys.E) && level == 3) //player attacking
                {
                    attacking = true;
                }
            
                if (attacking)
                {
                    Attack();
                }

                if (level == 3)
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (attackRect.Intersects(enemies[i].hitbox)) //attacking enemies with sword
                        {
                            enemies[i].health -= 100;
                        }
                        if (enemies[i].attackHitbox.Intersects(playerRect)) //getting hit by enemies
                        {
                            if (canBeAttacked)
                            {
                                health -= 15;
                                timer = 0;
                                canBeAttacked = false;
                                break;
                            }
                        }
                    }


                    if (canBeAttacked == false)
                    {
                        timer += (float)gameTime.ElapsedGameTime.TotalSeconds; //cooldown before the player can be attacked again
                        if (timer > 2)
                            canBeAttacked = true;
                    }
                }

                //collisions
                if (amountOfCollisions != 0 && amountOfCollisions != amountOfCollisionsOld) //if a new item is added to the collisions list the player is stopped from moving in the current direction
                {
                    if (collisionDir.Contains("") || amountOfCollisions > amountOfCollisionsOld)
                    {
                        switch (direction)
                        {
                            case Dir.Left:
                                collisionDir.Add("left");
                                collisionDir.Remove("");
                                break;
                            case Dir.Right:
                                collisionDir.Add("right");
                                collisionDir.Remove("");
                                break;
                            case Dir.Up:
                                collisionDir.Add("up");
                                collisionDir.Remove("");
                                break;
                            case Dir.Down:
                                collisionDir.Add("down");
                                collisionDir.Remove("");
                                break;
                        }
                    }

                }
                if (amountOfCollisions == 0)
                {
                    collisionDir.Clear();
                    collisionDir.Add("");
                }
                amountOfCollisionsOld = amountOfCollisions;

                //section to manage teleporting around the map

                if (eventRectName != "")
                {
                    TeleportPlayer(eventRectName);
                }

                if (eventTrigger == "inside door")
                {
                    if (timer <= 5)
                        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (eventTrigger == "through hole")
                {

                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timer >= 10)
                        eventTrigger = "";
                }

                if (eventTrigger == "house 4 text 2")
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (eventTrigger == "house 4 text 1")
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (eventTrigger == "Chest")
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            //text drawing logic
            if (eventTrigger == "display text 1")
            {
                if (textDraw < npcText.Length)
                {
                    textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (textDrawTimer > 0.05)
                    {
                        textDraw++;
                        textDrawTimer = 0;
                    }
                }
                if (textDraw == npcText.Length && textDrawTimer < 4)
                {
                    textDrawTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
            _sprite.Update(dt);
            kStateOld = kState;
        }

        public void PlayerDraw(SpriteBatch _spriteBatch, bool debugMode, SpriteFont spriteFont)
        {
            if (!accessingInventory)
            {
                _sprite.Render(_spriteBatch);

                //event drawing
                if (eventTrigger == "inside door")
                    if (timer <= 5)
                        _spriteBatch.DrawString(spriteFont, "The door locks behind you, the only way through is down the hole", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);

                if (eventTrigger == "house 4 text 2")
                    if (timer <= 5)
                        _spriteBatch.DrawString(spriteFont, "You use the key to enter the house, The rust on the lock prevents you from retrieving the key", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);

                if (eventTrigger == "house 4 text 1")
                    if (timer <= 2)
                        _spriteBatch.DrawString(spriteFont, "the door to this house seems to be locked, perhaps you can find a key somewhere", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
                
                if (eventTrigger == "Chest")
                    if (timer <= 2)
                        _spriteBatch.DrawString(spriteFont, "In the chest lies a rusty key, you pick it up", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);


                //draw health
                _spriteBatch.DrawString(spriteFont, "Health: " + health.ToString(), new Vector2(position.X - (spriteFont.MeasureString("Health: " + health.ToString()).Length() * 0.5f) / 2, position.Y + 200), Color.Red, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);


                //draw text box
                if (eventTrigger == "display text 1" && textDrawTimer < 4)
                {
                    _spriteBatch.Draw(_texture, new Rectangle((int)position.X - 120, (int)position.Y + 140, 280, 80), Color.White);
                    _spriteBatch.Draw(_texture, new Rectangle((int)position.X - 115, (int)position.Y + 145, 270, 70), Color.Black);
                    _spriteBatch.DrawString(spriteFont, npcText.Remove(textDraw, npcText.Length - textDraw), new Vector2(position.X - 110, position.Y + 145), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
                }
            }

            if (accessingInventory)
                InventoryDraw(inventory, _spriteBatch, spriteFont);

            //if (eventTrigger == "through hole")
            //{
            //    if (timer < 10)
            //    {
            //        _spriteBatch.Draw(_texture, coverScreen, Color.Black);
            //    }
            //    if (timer > 2 && timer < 4.6)
            //        _spriteBatch.DrawString(spriteFont, "After what feels like forver, you finally reach the bottom", new Vector2(position.X - (spriteFont.MeasureString("After what feels like forver, you finally reach the bottom").Length() * 0.35f) / 2, position.Y - 50), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            //    if (timer > 4.6 && timer < 7.2)
            //        _spriteBatch.DrawString(spriteFont, "A sword lies on the ground beside you. You pick it up", new Vector2(position.X - (spriteFont.MeasureString("A sword lies on the ground beside you. You pick it up").Length() * 0.35f) / 2, position.Y), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            //    if (timer > 7.2 && timer < 10)
            //        _spriteBatch.DrawString(spriteFont, "(Press E to swing the sword)", new Vector2(position.X - (spriteFont.MeasureString("(Press E to swing the sword)").Length() * 0.35f) / 2, position.Y + 50), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            //}

            if (debugMode)
            {
                _spriteBatch.Draw(_texture, playerRect, Color.White); //draw player collision rect for debug
                _spriteBatch.DrawString(spriteFont, "X: " + MathF.Round(position.X, 0).ToString(), new Vector2(position.X - 220, position.Y - 230), Color.White);
                _spriteBatch.DrawString(spriteFont, "Y: " + MathF.Round(position.Y, 0).ToString(), new Vector2(position.X - 220, position.Y - 200), Color.White);
                _spriteBatch.Draw(_texture, attackRect, Color.Yellow);
            }
        }

        private void InventoryDraw(List<string> inventory, SpriteBatch _spriteBatch, SpriteFont spriteFont)
        {
            _spriteBatch.Draw(_texture, coverScreen, Color.Black); //make the screen background black

            _spriteBatch.DrawString(spriteFont, "Inventory:", new Vector2(position.X - (spriteFont.MeasureString("Inventory").Length() * 0.35f) / 2, position.Y - 150), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);

            for (int i = 0; i < inventory.Count; i++)
            {
                _spriteBatch.DrawString(spriteFont, inventory[i], new Vector2(position.X - (spriteFont.MeasureString(inventory[i]).Length() * 0.35f) / 2, position.Y - 100 + (i * 20)), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            }
            
        }

        private bool SetDirection(KeyboardState kState)
        {
            if (kState.IsKeyDown(Keys.D)) //set direction depending on what keys are pressed
            {
                direction = Dir.Right;
                return true;
            }

            else if (kState.IsKeyDown(Keys.A))
            {
                direction = Dir.Left;
                return true;
            }

            else if (kState.IsKeyDown(Keys.W))
            {
                direction = Dir.Up;
                return true;
            }

            else if (kState.IsKeyDown(Keys.S))
            {
                direction = Dir.Down;
                return true;
            }
            else
                return false;
        }

        private void MovePlayer(float dt)
        {
            switch (direction)
            {
                case Dir.Left:
                    if (!collisionDir.Contains("left"))
                    {
                        position.X -= speed * dt;
                        _sprite.Play("walkLeft");
                    }
                    break;
                case Dir.Up:
                    if (!collisionDir.Contains("up"))
                    {
                        position.Y -= speed * dt;
                        _sprite.Play("walkUp");
                    }
                    break;
                case Dir.Right:
                    if (!collisionDir.Contains("right"))
                    {
                        position.X += speed * dt;
                        _sprite.Play("walkRight");
                    }
                    break;
                case Dir.Down:
                    if (!collisionDir.Contains("down"))
                    {
                        position.Y += speed * dt;
                        _sprite.Play("walkDown");
                    }
                    break;
            }
        }

        private void TeleportPlayer(string eventRectName)
        {
            if (eventRectName == "House outside")
            {
                position = new Vector2(1730, 1006); //inside of house co-ords
            }


            if (eventRectName == "House inside")
            {
                position = new Vector2(520, 550); //outside of coords
            }

            if (eventRectName == "inside door")
            {
                position = new Vector2(1427, 230); //tunnel
                eventTrigger = "inside door";
                level = 2;

            }

            if (eventRectName == "Hole")
            {
                if (!inventory.Contains("Sword"))
                    inventory.Add("Sword");
                position = new Vector2(403, 1650); //through hole
                eventTrigger = "through hole";
                timer = 0;
                level = 3;
            }

            if (eventRectName == "NPC1")
            {
                eventTrigger = "display text 1";
            }

            if (eventRectName == "Dungeon house 1")
            {
                position = new Vector2(2488, 310);
            }

            if (eventRectName == "Dungeon house 1 inside")
            {
                position = new Vector2(624, 2705);
            }

            if (eventRectName == "Dungeon house 2")
            {
                position = new Vector2(3129, 310);
            }

            if (eventRectName == "Dungeon house 2 inside")
            {
                position = new Vector2(2029, 2190);
            }

            if (eventRectName == "Dungeon house 3")
            {
                position = new Vector2(3129, 994);
            }

            if (eventRectName == "Dungeon house 3 inside")
            {
                position = new Vector2(2091, 1820);
            }

            if (eventRectName == "Dungeon house 4")
            {
                if (!inventory.Contains("Key"))
                {
                    eventTrigger = "house 4 text 1";
                    timer = 0;
                }
                else
                {
                    eventTrigger = "house 4 text 2";
                    timer = 0;
                    position = new Vector2(2488, 994);
                }
                    
            }

            if (eventRectName == "Chest")
            {
                timer = 0;
                eventTrigger = "Chest";
            }

            if (eventRectName == "Dungeon house 4 inside")
            {
                position = new Vector2(1648, 1935);
            }
            
            if (eventRectName == "Chest")
            {
                if (!inventory.Contains("Key"))
                {
                    inventory.Add("Key");
                }
            }
        }

        private void PlayIdleAnimations()
        {
            switch (direction)
            {
                case Dir.Left: //play idle animations depending on which direction the player was last moving
                    _sprite.Play("idle-left");
                    break;
                case Dir.Right:
                    _sprite.Play("idle-right");
                    break;
                case Dir.Up:
                    _sprite.Play("idle-up");
                    break;
                case Dir.Down:
                    _sprite.Play("idle-down");
                    break;

            }
        }

        private void Attack()
        {
            if (direction == Dir.Down)
            {
                _sprite.Play("attack-down");
                if (_sprite.CurrentFrameIndex == 24)
                {
                    attackRect = new Rectangle((int)position.X, (int)position.Y, 32, 72);
                    attacking = false;
                }
            }

            if (direction == Dir.Up)
            {

                _sprite.Play("attack-up");
                if (_sprite.CurrentFrameIndex == 41)
                {
                    attackRect = new Rectangle((int)position.X, (int)position.Y - 36, 32, 72);
                    attacking = false;
                }
            }

            if (direction == Dir.Left)
            {
                _sprite.Play("attack-left");
                if (_sprite.CurrentFrameIndex == 36)
                {
                    attackRect = new Rectangle((int)position.X - 32, (int)position.Y, 64, 36);
                    attacking = false;
                }
            }

            if (direction == Dir.Right)
            {
                _sprite.Play("attack-right");
                if (_sprite.CurrentFrameIndex == 30)
                {
                    attackRect = new Rectangle((int)position.X, (int)position.Y, 64, 36);
                    attacking = false;
                }
            }
        }
    }
}