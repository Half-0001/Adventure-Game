using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;

namespace Adventure_Game_CSharp
{
    internal class Player
    {
        private Vector2 position = new Vector2(500, 500);
        private int speed = 100;
        private Dir direction = Dir.Down;
        private bool isMoving = false;
        private KeyboardState kStateOld = Keyboard.GetState();
        public Rectangle playerRect = new Rectangle(0, 0, 32, 32);
        Texture2D _texture;




        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public void PlayerUpdate(GameTime gameTime, AnimatedSprite _sprite, GraphicsDevice _graphics)
        {
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });

            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _sprite.Position = position;
            isMoving = false;
            playerRect.X = (int)position.X;
            playerRect.Y = (int)position.Y;

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
            if (kState.IsKeyDown(Keys.Space))
                isMoving = false;

            if (isMoving)
            {

                switch (direction)
                {
                    case Dir.Left:
                        position.X -= speed * dt;
                        _sprite.Play("walkLeft");
                        break;
                    case Dir.Up:
                        position.Y -= speed * dt;
                        _sprite.Play("walkUp");
                        break;
                    case Dir.Right:
                        position.X += speed * dt;
                        _sprite.Play("walkRight");
                        break;
                    case Dir.Down:
                        position.Y += speed * dt;
                        _sprite.Play("walkDown");
                        break;
                }
            }

            if (!isMoving)
            {
                _sprite.Play("idle");
            }
        }

        public void PlayerDraw(SpriteBatch _spriteBatch, AnimatedSprite _sprite)
        {
            _sprite.Render(_spriteBatch);
            _spriteBatch.Draw(_texture, playerRect, Color.Purple);
        }
    }
}