using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InterNeuralNet.NetworkView
{
    [Serializable]
    public class SpriteCreator
    {
        [Min(0)] public float pixelPerUnit = 8;

        public Sprite Create(Texture2D tex, string name = "Runtime Sprite")
        {
            var size = new Vector2(tex.width, tex.height);
            Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, size), new Vector2(0.5f, 0.5f), pixelPerUnit);
            sprite.name = name;
            return sprite;
        }

        public static Sprite CreateFixedWidth(Texture2D tex, string name = "Runtime Sprite")
        {
            var size = new Vector2(tex.width, tex.height);
            Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, size), new Vector2(0.5f, 0.5f), tex.width);
            sprite.name = name;
            return sprite;
        }

        public static Sprite CreateFixedHeight(Texture2D tex, string name = "Runtime Sprite")
        {
            var size = new Vector2(tex.width, tex.height);
            Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, size), new Vector2(0.5f, 0.5f), tex.height);
            sprite.name = name;
            return sprite;
        }
    }

}
