/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SH.Util
{
    public class SpriteAtlasCache
    {
        private readonly SpriteAtlas _spriteAtlas;
        private readonly Dictionary<string, Sprite> _dictTable = new Dictionary<string, Sprite>();

        public SpriteAtlasCache(SpriteAtlas spriteAtlas)
        {
            _spriteAtlas = spriteAtlas;
        }

        public Sprite GetSprite(string name)
        {
            if (_spriteAtlas == null)
                return null;

            if (_dictTable.TryGetValue(name, out var sprite))
                return sprite;

            sprite = _spriteAtlas.GetSprite(name);
            _dictTable.Add(name, sprite);
            return sprite;
        }
    }
}