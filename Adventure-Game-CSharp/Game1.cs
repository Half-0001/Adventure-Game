using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;

namespace Adventure_Game_CSharp
{
    enum Dir
    {
        Down,
        Up,
        Left,
        Right,
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Point _resolution;

        private AnimatedSprite _sprite;

        //classes
        Player player = new Player();
        private Camera camera;

        //textures
        Texture2D background;

        //variables
        const int playerRadius = 20;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _resolution = new Point(900, 900);
            _graphics.PreferredBackBufferWidth = _resolution.X;
            _graphics.PreferredBackBufferHeight = _resolution.Y;
            _graphics.ApplyChanges();
            this.camera = new Camera(_graphics.GraphicsDevice);
            camera.Zoom = 2f;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("map");

            //  Load the asprite file from the content pipeline.
            AsepriteDocument aseprite = Content.Load<AsepriteDocument>("Male 01");

            //  Create a new aniamted sprite instance using the aseprite doucment loaded.
            _sprite = new AnimatedSprite(aseprite);
            _sprite.Scale = new Vector2(1.0f, 1.0f);
            _sprite.Y = _resolution.Y - (_sprite.Height * _sprite.Scale.Y) - 16;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.PlayerUpdate(gameTime, _sprite, GraphicsDevice);

            this.camera.Position = player.Position;
            this.camera.Update(gameTime);

            _sprite.Update(dt);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(this.camera);
            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            player.PlayerDraw(_spriteBatch, _sprite);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}