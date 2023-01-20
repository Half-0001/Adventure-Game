using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
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
        CollisionManager collisionManager = new CollisionManager(0, 0, 0, 0, "");
        EventManager eventManager = new EventManager(0, 0, 0, 0, "");
        Enemy enemy = new Enemy(0, 0, null, Point.Zero);
        NPC npc = new NPC(0, 0, null, Point.Zero);

        //textures
        Texture2D background;
        SpriteFont spriteFont;

        //variables
        private List<int> collidingWith = new List<int>();
        private string eventRectName;
        private int counter = 0;

        private bool debugMode = false;
        static KeyboardState kState;
        static KeyboardState kStateOld;

        private bool modifiedCollisionBoxes = false;


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
            eventManager.initTeleportManager(GraphicsDevice);

            this.camera = new Camera(_graphics.GraphicsDevice);
            camera.Zoom = 2f;
            base.Initialize();

            collisionManager.colliders.Add(new CollisionManager(npc.npcs[0].hitbox.X, npc.npcs[0].hitbox.Y, npc.npcs[0].hitbox.Size.X, npc.npcs[0].hitbox.Size.Y, "Level3"));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("map");
            spriteFont = Content.Load<SpriteFont>("font");

            player.LoadContent(Content, _resolution, GraphicsDevice);
            enemy.LoadContent(Content, _resolution, GraphicsDevice);
            npc.LoadContent(Content, _resolution, GraphicsDevice);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); 
            if (!player.accessingInventory)
            {
                //manage collisions TODO: Move to player class
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

                //manage teleports TODO: Move to player class
                for (int i = 0; i < eventManager.teleportColliders.Count; i++)
                    if (player.playerRect.Intersects(eventManager.teleportColliders[i].teleportRect))
                        eventRectName = eventManager.teleportColliders[i].rectName;


                enemy.Update(gameTime, player.Position);
                npc.Update(gameTime, counter, enemy.enemies.Count);

                //add more enemies when player talks to npc
                if (eventRectName == "NPC1" && counter == 0)
                {
                    enemy.AddEnemies(1, 284, 1139, 1554, 2324);
                    counter++;
                }
                if (counter == 1 && enemy.enemies.Count == 0)
                {
                    collisionManager.colliders.RemoveAt(collisionManager.colliders.Count - 1);
                    collisionManager.colliders.Add(new CollisionManager(npc.npcs[0].hitbox.X, npc.npcs[0].hitbox.Y, npc.npcs[0].hitbox.Size.X, npc.npcs[0].hitbox.Size.Y, "Level3"));
                }
            }

            //debug mode (press G to activate) - shows collision boxes
            kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.G) && !kStateOld.IsKeyDown(Keys.G))
            {
                debugMode = !debugMode;
            }
            kStateOld = kState;

            player.PlayerUpdate(gameTime, collidingWith.Count, eventRectName, enemy.enemies);
            modifiedCollisionBoxes = collisionManager.OptimiseCollisions(eventRectName);
            if (modifiedCollisionBoxes)
            {
                collidingWith.Clear();
                modifiedCollisionBoxes = false;
            }
                
            eventRectName = "";

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
            eventManager.DrawTeleportManager(_spriteBatch, debugMode);

            npc.Draw(_spriteBatch, debugMode);
            enemy.Draw(_spriteBatch, debugMode);
            player.PlayerDraw(_spriteBatch, debugMode, spriteFont);


            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}