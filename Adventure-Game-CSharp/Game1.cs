﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System;

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

        

        //classes
        private Camera camera;
        Player player = new Player();
        CollisionManager collisionManager = new CollisionManager(0, 0, 0, 0);
        TeleportManager teleportManager = new TeleportManager(0, 0, 0, 0, "");
        Enemy enemy = new Enemy(0, 0, null);

        //textures
        Texture2D background;
        SpriteFont spriteFont;

        //variables
        private List<int> collidingWith = new List<int>();
        private string teleportRectName;
        //private bool insideHouse = false;
        private bool debugMode = false;
        static KeyboardState kState;
        static KeyboardState kStateOld;


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
            
            collisionManager.initCollisions(GraphicsDevice);
            teleportManager.initTeleportManager(GraphicsDevice);

            this.camera = new Camera(_graphics.GraphicsDevice);
            camera.Zoom = 2f;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("map");
            spriteFont = Content.Load<SpriteFont>("font");

            player.LoadContent(Content, _resolution);
            enemy.LoadContent(Content, _resolution, GraphicsDevice);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //manage collisions
            for (int i = 0; i < collisionManager.colliders.Count; i++)
            {
                if (!collidingWith.Contains(i))
                if (player.playerRect.Intersects(collisionManager.colliders[i].rect)) //if player is colliding with a rect
                {
                    collidingWith.Add(i); //stores all the objects that the player is colliding with in a list
                }
                if (player.playerRect.Intersects(collisionManager.colliders[i].rect) == false)
                {
                    if (collidingWith.Contains(i))
                        player.collisionDir.RemoveAt(collidingWith.IndexOf(i));
                        collidingWith.Remove(i);
                }
            }

            //manage teleports
            for (int i = 0; i < teleportManager.teleportColliders.Count; i++)
                if (player.playerRect.Intersects(teleportManager.teleportColliders[i].teleportRect))
                    teleportRectName = teleportManager.teleportColliders[i].rectName;


            //debug mode (press G to activate) - shows collision boxes
            kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.G) && !kStateOld.IsKeyDown(Keys.G))
            {
                debugMode = !debugMode;
            }
            kStateOld = kState;

            player.PlayerUpdate(gameTime, GraphicsDevice, collidingWith, teleportRectName);
            teleportRectName = "";

            enemy.Update(gameTime);

            this.camera.Position = player.Position;
            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(this.camera);

            _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            collisionManager.DrawCollisionBoxes(_spriteBatch, debugMode);
            teleportManager.DrawTeleportManager(_spriteBatch, debugMode);

            enemy.Draw(_spriteBatch, debugMode);
            player.PlayerDraw(_spriteBatch, debugMode, spriteFont);


            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}