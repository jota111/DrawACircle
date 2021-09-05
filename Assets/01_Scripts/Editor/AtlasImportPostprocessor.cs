/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/

using System.Linq;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 아틀라스로 묶이는 텍스쳐는 Uncompressed, 플렛폼 오버라이드 사용안함
/// </summary>
public class AtlasImportPostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        // SpriteAtlas 변경 감지 전용 
        var list = importedAssets.Where (c => c.EndsWith ( ".spriteatlas" )). ToArray ();
        if (list.Length <= 0 )
            return;
        
        foreach (var path in list)
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if(atlas == null)
                continue;
            
            if (atlas.spriteCount == 0)
                return;

            var serializedObject = new SerializedObject (atlas);
            var sprites = serializedObject.FindProperty ( "m_PackedSprites" );
            foreach (SerializedProperty sprite in sprites)
            {
                var s = sprite.objectReferenceValue as Sprite;
                if (s != null)
                {
                    NonCompress(s);
                }
            }
        }
    }

    private static readonly string[] Platform = {"Standalone", "iPhone", "Android"};
    private static void NonCompress(Sprite sprite)
    {
        var path = AssetDatabase.GetAssetPath(sprite);
        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if(textureImporter == null)
            return;

        var hasPlatform = Platform.Select(p => textureImporter.GetPlatformTextureSettings(p) != null).All(has => !has);

        if (hasPlatform || textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
        {
            Platform.ForEach(textureImporter.ClearPlatformTextureSettings);
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.SaveAndReimport();   
        }
    }
}