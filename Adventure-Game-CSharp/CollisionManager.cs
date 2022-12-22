using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledCS;
using System.Diagnostics;

namespace Adventure_Game_CSharp
{
    internal class CollisionManager
    {
        //public int posX;
        //public int posY;
        //public int sizeX;
        //public int sizeY;
        public Rectangle rect = new Rectangle();

        private Texture2D _texture;

        public List<CollisionManager> colliders = new List<CollisionManager>();
        public CollisionManager(int a, int b, int c, int d)
        {
            //posX = a;
            //posY = b;
            //sizeX = c;
            //sizeY = d;
            //rect = new Rectangle(posX, posY, sizeX, sizeY);
            rect = new Rectangle(a, b, c, d);
        }


        public void initCollisions(GraphicsDevice _graphics)
        {
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });

            var map = new TiledMap("Content/map.tmx");

            // Retrieving objects or layers can be done using Linq or a for loop
            //var objectLayer = map.Layers.First(l => l.id == 4);
            var objectLayer = map.Layers[2];
            
            for (int i = 0; i < objectLayer.objects.Length; i++)
            {
                colliders.Add(new CollisionManager((int)objectLayer.objects[i].x, (int)objectLayer.objects[i].y, (int)objectLayer.objects[i].width, (int)objectLayer.objects[i].height));
            }
            


            
        }

        public void DrawCollisionBoxes(SpriteBatch _spriteBatch)
        {
            //_spriteBatch.Draw(_texture, new Rectangle(colliders[0].posX, colliders[0].posY, colliders[0].sizeX, colliders[0].sizeY), Color.Blue);
            //for (int i = 0; i < colliders.Count; i++)
                //_spriteBatch.Draw(_texture, colliders[i].rect, Color.Blue);
        }
    }
}
