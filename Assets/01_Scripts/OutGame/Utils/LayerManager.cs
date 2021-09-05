using System.Collections;
using System.Collections.Generic;
using OutGameCore;
using UnityEngine;

namespace OutGameCore
{
    public class LayerManager
    {
        public static int ModelLayer = LayerMask.NameToLayer("GameModels");
        public static int ObjectLayer = LayerMask.NameToLayer("GameObjects");
        public static int GroundLayer = LayerMask.NameToLayer("GameGround");
        // public static int ObstaclesLayer = LayerMask.NameToLayer("GameObstacles");
        // public static int TileObjectLayer = LayerMask.NameToLayer("GameTileObject");
        // public static int TransparentLayer = LayerMask.NameToLayer("TransparentFX");

        public static int[] GetLayersWithoutObstacle => new int[]
        {
            ObjectLayer, GroundLayer
        };

        public static int GroundSpriteOrder = -1;
        public static int ShadowSpriteOrder = 0;
        public static int ObjectSpriteOrder = 0;
        public static int ObstaclesSpriteOrder = 0;
        public static int ModelSpriteOrder = 0;
        

        public static void SetLayerRecursively(GameObject go, bool prechecking = false)
        {
            if (prechecking == true)
            {
                if (go.layer == LayerManager.ObjectLayer)
                {
                    return;
                }
            }

            GameUtils.SetLayerRecursively(go, LayerManager.ObjectLayer);
        }

        public static void SetModelLayerRecursively(GameObject go)
        {
            GameUtils.SetLayerRecursively(go, LayerManager.ModelLayer);
        }
    }
}
