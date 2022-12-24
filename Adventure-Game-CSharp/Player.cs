using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Position = position;
            isMoving = false;
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;

            
            foreach (var item in collisionDir)
                Debug.WriteLine(item);
            foreach (var item in collidingWith)
                Debug.WriteLine(item);

            if (kState.IsKeyDown(Keys.D))
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

            if (!isMoving)
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

            //if (amountOfCollisions == amountOfCollisionsOld - 1)
            //{
            //    collisionDir.RemoveAt(0);
            //    if (collisionDir.Count == 0)
            //        collisionDir.Add("");
            //}


            if (teleportRectName == "House outside")
            {
                position = new Vector2(1730, 1036); //inside of house co-ords
                //camera.Position = new Vector2(position.X, 1336);
            }


            if (teleportRectName == "House inside")
            {
                position = new Vector2(524, 590);
                //camera.Position = position;
            }

            amountOfCollisionsOld = amountOfCollisions;
            //return camera;
        }

            
        

        public void PlayerDraw(SpriteBatch _spriteBatch, AnimatedSprite _sprite, bool debugMode)
        {
            _sprite.Render(_spriteBatch);
            if (debugMode)    
                _spriteBatch.Draw(_texture, playerRect, Color.White); //draw player collision rect for debug
        }
    }
}