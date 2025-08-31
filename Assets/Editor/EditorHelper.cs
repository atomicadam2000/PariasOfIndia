// 31.08.2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Collections.Generic;

public class EditorHelper : MonoBehaviour
{
    [MenuItem("EditorHelper/SliceSprites")]
    static void SliceSprites()
    {
        // Change the below for the width and height dimensions of each sprite within the spritesheets
        int sliceWidth = 64;
        int sliceHeight = 64;

        // Change the below for the path to the folder containing the sprite sheets
        // Ensure the folder is within 'Assets/Resources/' (e.g., 'Assets/Resources/ToSlice')
        string folderPath = "ToSlice";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        Debug.Log("spriteSheets.Length: " + spriteSheets.Length);

        foreach (var spriteSheetObj in spriteSheets)
        {
            Debug.Log("Processing: " + spriteSheetObj);

            string path = AssetDatabase.GetAssetPath(spriteSheetObj);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter == null)
            {
                Debug.LogWarning($"Could not find TextureImporter for {path}");
                continue;
            }

            textureImporter.isReadable = true;
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;

            // Initialize the ISpriteEditorDataProvider
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);
            dataProvider.InitSpriteEditorDataProvider();

            List<SpriteRect> spriteRects = new List<SpriteRect>();

            Texture2D spriteSheet = spriteSheetObj as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth)
            {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight)
                {
                    SpriteRect spriteRect = new SpriteRect
                    {
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (SpriteAlignment)(int)SpriteAlignment.Custom,
                        name = $"{(spriteSheet.height - j) / sliceHeight}, {i / sliceWidth}",
                        rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight),
                        spriteID = GUID.Generate()
                    };

                    spriteRects.Add(spriteRect);
                }
            }

            // Set the sprite rects to the data provider
            dataProvider.SetSpriteRects(spriteRects.ToArray());

            // Apply the changes
            dataProvider.Apply();

            // Reimport the asset to apply changes
            textureImporter.SaveAndReimport();
        }

        Debug.Log("Done Slicing!");
    }
}