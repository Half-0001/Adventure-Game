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

namespace Adventure_Game_CSharp
{
    internal class Player
    {
        private Vector2 position = new Vector2(500, 600);
        private int speed = 100;
        private Dir direction = Dir.Down;
        private bool isMoving = false;
        public Rectangle playerRect = new Rectangle(0, 0, 32, 36);
        private Texture2D _texture;
        public List<string> collisionDir = new List<string> { "" };
        private int amountOfCollisions = 0;
        private int amountOfCollisionsOld = 0;
        private string eventTrigger = "";
        float timer = 0;
        private bool mReleased = true;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public void PlayerUpdate(GameTime gameTime, AnimatedSprite _sprite, GraphicsDevice _graphics, List<int> collidingWith, string teleportRectName)
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
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-left");
                        else if (!collisionDir.Contains("left"))  //TODO: Make animations much larger and fix when you can use them
                        {
                            position.X -= speed * dt;
                            _sprite.Play("walkLeft");
                        }
                        break;
                    case Dir.Up:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-up");
                        else if (!collisionDir.Contains("up"))
                        {
                            position.Y -= speed * dt;
                            _sprite.Play("walkUp");
                        }
                        break;
                    case Dir.Right:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-right");
                        else if (!collisionDir.Contains("right"))
                        {
                            position.X += speed * dt;
                            _sprite.Play("walkRight");
                        }
                        break;
                    case Dir.Down:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-down");
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
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-left");
                        else
                            _sprite.Play("idle-left");
                        break;
                    case Dir.Right:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-right");
                        else
                            _sprite.Play("idle-right");
                        break;
                    case Dir.Up:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-up");
                        else     
                            _sprite.Play("idle-up");
                        break;
                    case Dir.Down:
                        if (mState.LeftButton == ButtonState.Pressed && mReleased == true)
                            _sprite.Play("attack-down");
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
            if (eventTrigger == "inside door")
            {
                if (timer <= 5)
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //TODO: Add text along the lines of "the door has locked behind you, the only way forward is down the hole"

        }

            
        

        public void PlayerDraw(SpriteBatch _spriteBatch, AnimatedSprite _sprite, bool debugMode, SpriteFont spriteFont)
        {
            _sprite.Render(_spriteBatch);
            if (debugMode)    
                _spriteBatch.Draw(_texture, playerRect, Color.White); //draw player collision rect for debug

            if (eventTrigger == "inside door")
                if (timer <= 5)
                    _spriteBatch.DrawString(spriteFont, "The door locks behind you, the only way through is down the hole", new Vector2(position.X - 200, position.Y - 200), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0f);
        }
    }
}