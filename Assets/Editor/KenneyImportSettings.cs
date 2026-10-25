using UnityEditor;
using UnityEngine;

// Applies pixel-art import settings to every image in Assets/Art/Kenney,
// so tiles are crisp and each tile is exactly 1 Unity unit (same size as the player).
//   Tiles/                    -> 18 px tiles  -> 18 pixels per unit
//   Backgrounds/, Characters/ -> 24 px sprites -> 24 pixels per unit
public class KenneyImportSettings : AssetPostprocessor
{
    const string KenneyFolder = "Assets/Art/Kenney/";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(KenneyFolder)) return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;

        // Full Rect meshes avoid tiny gaps between tiles on a Tilemap
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        importer.spritePixelsPerUnit = assetPath.Contains("/Tiles/") ? 18 : 24;
        importer.filterMode = FilterMode.Point;                              // no blur on pixel art
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
    }
}
