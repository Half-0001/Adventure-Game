﻿using Microsoft.Xna.Framework;
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
        public Rectangle rect = new Rectangle();

        private Texture2D _texture;

        public List<CollisionManager> colliders = new List<CollisionManager>(); //list where colliders are stored
        public CollisionManager(int a, int b, int c, int d)  //collider creator 
        {
            rect = new Rectangle(a, b, c, d);
        }


        public void initCollisions(GraphicsDevice _graphics) //initialize collision manager
        {
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray }); 

            var map = new TiledMap("Content/map.tmx"); 

            var objectLayer = map.Layers[2]; //load tilemap and create colliders where collision boxes are marked on the tilemap
            
            for (int i = 0; i < objectLayer.objects.Length; i++)
            {
                colliders.Add(new CollisionManager((int)objectLayer.objects[i].x, (int)objectLayer.objects[i].y, (int)objectLayer.objects[i].width, (int)objectLayer.objects[i].height));
            }
            


            
        }

        public void DrawCollisionBoxes(SpriteBatch _spriteBatch) //draw collision boxes for debug
        {
            //_spriteBatch.Draw(_texture, new Rectangle(colliders[0].posX, colliders[0].posY, colliders[0].sizeX, colliders[0].sizeY), Color.Blue);
            //for (int i = 0; i < colliders.Count; i++)
                //_spriteBatch.Draw(_texture, colliders[i].rect, Color.Blue);
        }
    }
}