using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Adventure_Game_CSharp
{
    internal class Player
    {
        private Vector2 position = new Vector2(500, 500);
        private int speed = 300;
        private Dir direction = Dir.Down;
        private bool isMoving = false;
        private KeyboardState kStateOld = Keyboard.GetState();
        

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public void PlayerUpdate(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            isMoving = false;

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
                        break;
                    case Dir.Up:
                        position.Y -= speed * dt;
                        break;
                    case Dir.Right:
                        position.X += speed * dt;
                        break;
                    case Dir.Down:
                        position.Y += speed * dt;
                        break;
                }
            }
        }

        public void PlayerDraw(SpriteBatch _spriteBatch)
        {

        }
    }
}