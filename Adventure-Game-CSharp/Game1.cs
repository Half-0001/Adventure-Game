using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Comora;
using System.Collections.Generic;
using System.Diagnostics;

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
        Boss boss = new Boss(0, 0, new Vector2(0, 0));

        //textures
        Texture2D background;
        SpriteFont spriteFont;

        //variables
        private  List<int> collidingWith = new List<int>();
        private string eventRectName;
        private int counter = 0;

        private bool debugMode = false;
        static KeyboardState kState;
        static KeyboardState kStateOld;

        private bool modifiedCollisionBoxes = false;
        private string inMenu = "true";
        //public bool running = true;
        //public bool restart = false;


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
            boss.LoadContent(Content, _resolution, GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) //Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (inMenu == "true")
            {
                this.camera.Position = new Vector2(450, 450);
                inMenu = menu.Update();
                if (inMenu == "false but also skip boss fight")
                {
                    player.level = 4;
                    inMenu = "false";
                }
            }

            if (player.restart || debugMode)
            {
                if (kState.IsKeyDown(Keys.R))
                { 
                    Restart();
                }
            }

            //debug mode (press G to activate) - shows collision boxes
            kState = Keyboard.GetState();
            if (kState.IsKeyDown(Keys.G) && !kStateOld.IsKeyDown(Keys.G))
            {
                debugMode = !debugMode;
            }

            kStateOld = kState;

            if (inMenu == "false" && player.level != 4)
            {
                camera.Zoom = 2f;
                if (!player.accessingInventory || !player.restart)
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

                    boss.Update(player.eventTrigger, gameTime);
                    enemy.Update(gameTime, player.Position);
                    npc.Update(gameTime, counter, enemy.enemies.Count);

                    //add more enemies when player talks to npc
                    if (eventRectName == "NPC1" && counter == 0)
                    {
                        enemy.AddEnemies(10, 284, 1139, 1554, 2324);
                        counter++;
                    }
                    if (counter == 1 && enemy.enemies.Count == 0) //when player has killed the extra enemies, remove the npc collision box
                    {
                        collisionManager.colliders.RemoveAt(collisionManager.colliders.Count - 1);
                        collisionManager.colliders.Add(new CollisionManager(npc.npcs[0].hitbox.X, npc.npcs[0].hitbox.Y, npc.npcs[0].hitbox.Size.X, npc.npcs[0].hitbox.Size.Y, "Level3"));
                        enemy.AddEnemies2(10, 748, 1966, 3118, 3375);
                        enemy.AddEnemies2(10, 2470, 3196, 1651, 3333);
                    }

                }

                player.PlayerUpdate(gameTime, collidingWith.Count, eventRectName, enemy.enemies, debugMode);
                modifiedCollisionBoxes = collisionManager.OptimiseCollisions(eventRectName, npc.npcs[0].hitbox);
                if (modifiedCollisionBoxes)
                {
                    collidingWith.Clear();
                    modifiedCollisionBoxes = false;
                }

                eventRectName = "";

                this.camera.Position = player.Position;
            }

            if (player.level == 4)
            {
                camera.Zoom = 1f;
                camera.Position = new Vector2(450, 450);
                boss.BossBattle(gameTime);
            }


            this.camera.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(this.camera);

            if (inMenu == "false" && player.level != 4)
            {
                _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                collisionManager.DrawCollisionBoxes(_spriteBatch, debugMode);
                eventManager.DrawTeleportManager(_spriteBatch, debugMode);

                npc.Draw(_spriteBatch, debugMode);
                enemy.Draw(_spriteBatch, debugMode);
                player.PlayerDraw(_spriteBatch, debugMode, spriteFont);
                boss.Draw(_spriteBatch);
            }

            if (inMenu == "true")
                menu.Draw(_spriteBatch, debugMode);

            if (player.level == 4)
            {
                boss.DrawBossBattle(_spriteBatch, GraphicsDevice);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Restart()
        {
            player.Restart();
            counter = 0;
            enemy.Restart(Content, _resolution, GraphicsDevice);
            collisionManager.Restart(GraphicsDevice);
            npc.Restart(Content, _resolution, GraphicsDevice);
        }
    }
}