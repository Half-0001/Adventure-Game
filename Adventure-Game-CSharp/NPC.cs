using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Aseprite.Graphics;
using System.Collections.Generic;
using MonoGame.Aseprite.Documents;
using System;
using System.Diagnostics;

namespace Adventure_Game_CSharp
{
    internal class NPC
    {
        public List<NPC> npcs = new List<NPC>();


        private AnimatedSprite npcSprite;
        public Vector2 position = new Vector2();
        public Rectangle hitbox = new Rectangle();

        Texture2D _texture;

        public NPC(int positionX, int positionY, AsepriteDocument asepritefile, Point _resolution)
        {
            position.X = positionX;
            position.Y = positionY;
            hitbox = new Rectangle(positionX, positionY - 5, 30, 30);

            if (asepritefile != null) //each sprite has its own position variables, if "null" is passed into the creator (at the start of Game1.cs) then it throws an error. 
            {
                npcSprite = new AnimatedSprite(asepritefile);
                npcSprite.Scale = new Vector2(1.0f, 1.0f);
                npcSprite.Y = _resolution.Y - (npcSprite.Height * npcSprite.Scale.Y) - 16;
            }
            else
                npcSprite = null;

        }
        public void LoadContent(ContentManager Content, Point _resolution, GraphicsDevice _graphics)
        {
            //  Load the aseprite file from the content pipeline.
            AsepriteDocument asepritefile = Content.Load<AsepriteDocument>("Male 16-1"); 

            //add npc
            for (int i = 0; i < 1; i++)
                npcs.Add(new NPC(1424, 2742, asepritefile, _resolution));

            //blank texture 
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.White });
        }

        public void Update(GameTime gameTime, int counter, int enemyCount) //TODO: Fix NPC hitbox before all enemies have been killed
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (counter != 1 && enemyCount != 0)
            {
                for (int i = 0; i < npcs.Count; i++)
                {
                    npcs[i].npcSprite.Position = new Vector2(npcs[i].position.X, npcs[i].position.Y);
                    npcs[i].npcSprite.Play("idle");
                    npcs[i].npcSprite.Update(dt);
                }
            }
            if (counter == 1 && enemyCount == 0)
            {
                npcs[0].position = new Vector2(1470, 2620);
                npcs[0].hitbox = new Rectangle((int)npcs[0].position.X, (int)npcs[0].position.Y - 5, 30, 30);
                npcs[0].npcSprite.Position = new Vector2(1470, 2620);
                npcs[0].npcSprite.Play("idle-left");
            }
        }

        public void Draw(SpriteBatch _spriteBatch, bool debugMode)
        {
            for (int i = 0; i < npcs.Count; i++)
            {
                npcs[i].npcSprite.Render(_spriteBatch); //draw sprite
            }

            if (debugMode)
                for (int i = 0; i < npcs.Count; i++)
                {
                    _spriteBatch.Draw(_texture, npcs[i].hitbox, Color.Blue); //draw hitbox
                }
        }
    }
    
}
