/*
* Copyright (c) CookApps.
* 이진호(jhlee8@cookapps.com)
*/


using System.IO;
using UnityEditor;
using UnityEditor.Presets;

/// <summary>
/// 프리셋 적용
/// 1.에셋과 같은 폴더에 있는 프리셋을 선택
/// 2.폴더에 프리셋이 없는 경우 스크립트에서 부모 폴더를 검색
/// 3.부모 폴더에 프리셋이 없는 경우 Preset 창에서 지정하는 기본 프리셋이 사용
/// </summary>
public class PresetImportPerFolder : AssetPostprocessor
{
    void OnPreprocessAsset()
    {
        // Make sure we are applying presets the first time an asset is imported.
        if (assetImporter.importSettingsMissing)
        {
            // Get the current imported asset folder.
            var path = Path.GetDirectoryName(assetPath);
            while (!string.IsNullOrEmpty(path))
            {
                // Find all Preset assets in this folder.
                var presetGuids = AssetDatabase.FindAssets("t:Preset", new[] { path });
                foreach (var presetGuid in presetGuids)
                {
                    // Make sure we are not testing Presets in a subfolder.
                    string presetPath = AssetDatabase.GUIDToAssetPath(presetGuid);
                    if (Path.GetDirectoryName(presetPath) == path)
                    {
                        // Load the Preset and try to apply it to the importer.
                        var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
                        if (preset.ApplyTo(assetImporter))
                            return;
                    }
                }

                // Try again in the parent folder.
                path = Path.GetDirectoryName(path);
            }
        }
    }
}