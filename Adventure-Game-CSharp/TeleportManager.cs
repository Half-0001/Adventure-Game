using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledCS;
using System.Diagnostics;

namespace Adventure_Game_CSharp
{
    internal class TeleportManager
    {
        public Rectangle teleportRect = new Rectangle();
        public string rectName;

        private Texture2D _texture;

        public List<TeleportManager> teleportColliders = new List<TeleportManager>(); //list where colliders are stored
        public TeleportManager(int a, int b, int c, int d, string name)  //collider creator 
        {
            teleportRect = new Rectangle(a, b, c, d);
            rectName = name;
        }
        TiledLayer teleportLayer;


        public void initTeleportManager(GraphicsDevice _graphics) //initialize collision manager
        {
            _texture = new Texture2D(_graphics, 1, 1);
            _texture.SetData(new Color[] { Color.DarkSlateGray });

            var map = new TiledMap("Content/map.tmx");
            for (int i = 0; i < map.Layers.Length; i++)
            {
                //Debug.WriteLine(map.Layers[i].name);
                if (map.Layers[i].name == "Teleport")
                {
                    teleportLayer = map.Layers[i]; //load tilemap and create colliders where collision boxes are marked on the tilemap
                }
            }

            for (int i = 0; i < teleportLayer.objects.Length; i++)
            {
                teleportColliders.Add(new TeleportManager((int)teleportLayer.objects[i].x, (int)teleportLayer.objects[i].y, (int)teleportLayer.objects[i].width, (int)teleportLayer.objects[i].height, teleportLayer.objects[i].name));
                Debug.WriteLine(teleportLayer.objects[i].name);
            }



        }

        public void DrawTeleportManager(SpriteBatch _spriteBatch) //draw collision boxes for debug
        {
            //_spriteBatch.Draw(_texture, new Rectangle(colliders[0].posX, colliders[0].posY, colliders[0].sizeX, colliders[0].sizeY), Color.Blue);
            //for (int i = 0; i < teleportColliders.Count; i++)
                //_spriteBatch.Draw(_texture, teleportColliders[i].teleportRect, Color.Red);
        }
    }
}