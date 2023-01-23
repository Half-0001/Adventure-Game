using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using System.Threading;

namespace Adventure_Game_CSharp
{
    internal class Menu
    {
        Texture2D menuBackground;
        Texture2D playWhite;
        Texture2D playYellow;
        Texture2D _texture;
        MouseState mState;
        Rectangle playButton = new Rectangle(350, 425, 200, 50);
        Rectangle mousePos = new Rectangle(0, 0, 10, 10);
        bool selected = false;

        public void LoadContent(ContentManager Content, GraphicsDevice _graphics)
        {
            menuBackground = Content.Load<Texture2D>("menu");
            playWhite = Content.Load<Texture2D>("play_white");
            playYellow = Content.Load<Texture2D>("play_yellow");

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public bool Update()
        {
            mState = Mouse.GetState();
            mousePos.X = (int)mState.X;
            mousePos.Y = (int)mState.Y;
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                return false;
            
            else if (mousePos.Intersects(playButton))
            {
                selected = true;
                if (mState.LeftButton == ButtonState.Pressed)
                    return false;
                else
                    return true;
            }

            else
            {
                selected = false;
                return true;
            }


        }
        public void Draw(SpriteBatch _spriteBatch, bool debugMode)
        {
            _spriteBatch.Draw(menuBackground, new Vector2(0, 0), Color.White);
            if (!selected)
                _spriteBatch.Draw(playWhite, playButton, Color.White);
            if (selected)
                _spriteBatch.Draw(playYellow, playButton, Color.White);

            if (debugMode)
            {
                _spriteBatch.Draw(_texture, playButton, Color.White);
                _spriteBatch.Draw(_texture, mousePos, Color.White);
            }
        }
    }
}
