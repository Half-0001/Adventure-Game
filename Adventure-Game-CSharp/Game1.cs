using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Adventure_Game_CSharp
{

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
        Menu menu = new Menu();

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
        private bool inMenu = true;
        public bool running = true;
        public bool restart = false;


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
            camera.Zoom = 1f;
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            background = Content.Load<Texture2D>("map");
            spriteFont = Content.Load<SpriteFont>("font");

            player.LoadContent(Content, _resolution, GraphicsDevice);
            enemy.LoadContent(Content, _resolution, GraphicsDevice);
            npc.LoadContent(Content, _resolution, GraphicsDevice);
            menu.LoadContent(Content, GraphicsDevice);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) //Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (inMenu)
            {
                this.camera.Position = new Vector2(450, 450);
                inMenu = menu.Update();
            }

            //debug mode (press G to activate) - shows collision boxes
            kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.G) && !kStateOld.IsKeyDown(Keys.G))
            {
                debugMode = !debugMode;
            }

            if (kState.IsKeyDown(Keys.R) && !kStateOld.IsKeyDown(Keys.R))
            {
                Restart();
            }

            kStateOld = kState;

            if (!inMenu)
            {
                camera.Zoom = 2f;
                if (!player.accessingInventory)
                {
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
                        enemy.AddEnemies2(1, 748, 1966, 3118, 3375);
                        enemy.AddEnemies2(1, 2515, 3196, 1651, 3333);
                    }

                }

                player.PlayerUpdate(gameTime, collidingWith.Count, eventRectName, enemy.enemies);
                modifiedCollisionBoxes = collisionManager.OptimiseCollisions(eventRectName, npc.npcs[0].hitbox);
                if (modifiedCollisionBoxes)
                {
                    collidingWith.Clear();
                    modifiedCollisionBoxes = false;
                }

                eventRectName = "";

                this.camera.Position = player.Position;
            }

            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(this.camera);

            if (!inMenu)
            {
                _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                collisionManager.DrawCollisionBoxes(_spriteBatch, debugMode);
                eventManager.DrawTeleportManager(_spriteBatch, debugMode);

                npc.Draw(_spriteBatch, debugMode);
                enemy.Draw(_spriteBatch, debugMode);
                player.PlayerDraw(_spriteBatch, debugMode, spriteFont);
            }

            if (inMenu)
                menu.Draw(_spriteBatch, debugMode);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Restart()
        {
            player.Restart();
            enemy.Restart(Content, _resolution, GraphicsDevice);
            collisionManager.Restart(GraphicsDevice);
            npc.Restart(Content, _resolution, GraphicsDevice);
        }
    }
}