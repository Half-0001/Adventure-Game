using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using MonoGame.Aseprite.Documents;
using System.Reflection.Metadata;

namespace Adventure_Game_CSharp
{
    internal class Player
    {
        private Vector2 position = new Vector2(500, 600);
        private int speed = 100;
        private Dir direction = Dir.Down;
        private bool isMoving = false;
        public Rectangle playerRect = new Rectangle(0, 0, 32, 36);
        private Rectangle coverScreen = new Rectangle(0, 0, 500, 500);
        private Texture2D _texture;
        public List<string> collisionDir = new List<string> { "" };
        private int amountOfCollisions = 0;
        private int amountOfCollisionsOld = 0;
        private string eventTrigger = "";
        private float timer = 0;
        private bool fallenDown = false;
        private AnimatedSprite _sprite;


        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public void LoadContent(ContentManager Content, Point _resolution)
        {
            //  Load the asprite file from the content pipeline.
            AsepriteDocument aseprite = Content.Load<AsepriteDocument>("Male 01");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            _sprite = new AnimatedSprite(aseprite);
            _sprite.Scale = new Vector2(1.0f, 1.0f);
            _sprite.Y = _resolution.Y - (_sprite.Height * _sprite.Scale.Y) - 16;
        }
        public void PlayerUpdate(GameTime gameTime, GraphicsDevice _graphics, List<int> collidingWith, string teleportRectName)
        {
            amountOfCollisions = collidingWith.Count;
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Position = new Vector2(position.X - 20, position.Y - 20);
            isMoving = false;
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;
            coverScreen.X = (int)position.X - 250;
            coverScreen.Y = (int)position.Y - 250;


            if (kState.IsKeyDown(Keys.D)) //set direction depending on what keys are pressed
            {
                direction = Dir.Right;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.A))
            {
                direction = Dir.Left;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.W))
            {
                direction = Dir.Up;
                isMoving = true;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                direction = Dir.Down;
                isMoving = true;
            }


            if (isMoving)
            {

                switch (direction) //move player and play animations
                {
                    case Dir.Left:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-left");
                            //mReleased = false;
                        }
                        else if (!collisionDir.Contains("left"))  //TODO: Make animations much larger and fix when you can use them
                        {
                            position.X -= speed * dt;
                            _sprite.Play("walkLeft");
                        }
                        break;
                    case Dir.Up:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-up");
                            //mReleased = false;
                        }
                        else if (!collisionDir.Contains("up"))
                        {
                            position.Y -= speed * dt;
                            _sprite.Play("walkUp");
                        }
                        break;
                    case Dir.Right:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-right");
                            //mReleased = false;
                        }
                        else if (!collisionDir.Contains("right"))
                        {
                            position.X += speed * dt;
                            _sprite.Play("walkRight");
                        }
                        break;
                    case Dir.Down:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-down");
                            //mReleased = false;
                        }
                        else if (!collisionDir.Contains("down"))
                        {
                            position.Y += speed * dt;
                            _sprite.Play("walkDown");
                        }
                        break;
                }
            }

            if (!isMoving)
            {
                switch (direction)
                {
                    case Dir.Left: //play idle animations depending on which direction the player was last moving
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-left");
                            //mReleased = false;
                        }
                        else
                            _sprite.Play("idle-left");
                        break;
                    case Dir.Right:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-right");
                            //mReleased = false;
                        }
                        else
                            _sprite.Play("idle-right");
                        break;
                    case Dir.Up:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-up");
                            //mReleased = false;
                        }
                        else     
                            _sprite.Play("idle-up");
                        break;
                    case Dir.Down:
                        if (mState.LeftButton == ButtonState.Pressed && fallenDown == true)
                        {
                            _sprite.Play("attack-down");
                            //mReleased = false;
                        }
                        else
                            _sprite.Play("idle-down");
                        break;

                }
            }

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
            if (teleportRectName == "House outside")
            {
                position = new Vector2(1730, 1006); //inside of house co-ords
            }


            if (teleportRectName == "House inside")
            {
                position = new Vector2(520, 550); //outside of coords
            }

            if (teleportRectName == "inside door")
            {
                position = new Vector2(1665, 230); //tunnel
                eventTrigger = "inside door";
                
            }

            if (teleportRectName == "Hole")
            {
                position = new Vector2(403, 1650); //through hole
                eventTrigger = "through hole";
                timer = 0;
                fallenDown = true;
            }

            if (eventTrigger == "inside door")
            {
                if (timer <= 5)
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (eventTrigger == "through hole")
            {
                if (timer <= 10)
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            _sprite.Update(dt);

        }

            
        

        public void PlayerDraw(SpriteBatch _spriteBatch, bool debugMode, SpriteFont spriteFont)
        {
            _sprite.Render(_spriteBatch);
            if (debugMode)    
                _spriteBatch.Draw(_texture, playerRect, Color.White); //draw player collision rect for debug

            if (eventTrigger == "inside door")
                if (timer <= 5)
                    _spriteBatch.DrawString(spriteFont, "The door locks behind you, the only way through is down the hole", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            if (eventTrigger == "through hole")
            {
                if (timer < 10)
                {
                    _spriteBatch.Draw(_texture, coverScreen, Color.Black);
                } 
                if (timer > 2 && timer < 4.6)
                    _spriteBatch.DrawString(spriteFont, "After what feels like forver, you finally reach the bottom", new Vector2(position.X - (spriteFont.MeasureString("After what feels like forver, you finally reach the bottom").Length()*0.35f) / 2, position.Y - 50), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
                if (timer > 4.6 && timer < 7.2)
                    _spriteBatch.DrawString(spriteFont, "A sword lies on the ground beside you. You pick it up", new Vector2(position.X - (spriteFont.MeasureString("A sword lies on the ground beside you. You pick it up").Length() * 0.35f) / 2, position.Y), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
                if (timer > 7.2 && timer < 10)
                    _spriteBatch.DrawString(spriteFont, "(Press left click to swing the sword)", new Vector2(position.X - (spriteFont.MeasureString("(Press left click to swing the sword)").Length() * 0.35f) / 2, position.Y + 50), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
            }
        }
    }
}