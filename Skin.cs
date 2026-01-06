using System.Collections.Generic;
using System.IO;
using BepInEx;
using SilksongMultiplayer;
using UnityEngine;

public static class Skin
{
    public static Dictionary<string, Texture2D> keyValuePairs = new Dictionary<string, Texture2D>();
    public static string? GetCurrentTextureName(GameObject go)
    {
        if (go == null) return null;

        tk2dSprite sprite = go.GetComponent<tk2dSprite>();
        if (sprite == null) return null;
        if (sprite != null)
        {
            var r = sprite.GetComponent<Renderer>();
            if (r != null && r.sharedMaterial != null)
            {
                var tex = r.sharedMaterial.mainTexture as Texture2D;
                return $"{sprite.Collection.name}:{tex.name}";
            }
        }

        return null;

    }

    public static void ChangeSkinOnObject(GameObject gameObject,string skinName)
    {
        //Debug.Log("Received a request to change the skin.");
        tk2dBaseSprite sprite = gameObject.GetComponent<tk2dBaseSprite>();
        tk2dSpriteCollectionData cloned = CloneCollection(sprite.Collection);
        var appliedAtlases = new Texture2D[cloned.materials.Length];

        for (int i = 0;i < sprite.Collection.textures.Length;i++)
        {
            string textureName = sprite.Collection.textures[i].name;
            if (string.IsNullOrEmpty(textureName))
            {
                continue;
            }

            string path = Path.Combine(
                "BepInEx",
                "plugins",
                "XvX",
                "skin",
                skinName,
                sprite.Collection.name,
                textureName + ".png"
            );

            cloned.textures[i] = LoadTextureFromGameRoot(path);
            cloned.materials[i].mainTexture = LoadTextureFromGameRoot(path);
            appliedAtlases[i] = LoadTextureFromGameRoot(path);
        }

        var lockComp = gameObject.GetComponent<SkinLock>() ?? gameObject.AddComponent<SkinLock>();
        lockComp.Cloned = cloned;
        lockComp.Atlases = appliedAtlases;
        lockComp.dataName = sprite.Collection.name;

        ApplyClonedCollectionToSprite(sprite, cloned);
    }

    public static int GetCurrentAtlas(tk2dBaseSprite sprite)
    {
        if (sprite == null) return 0;

        var col = sprite.Collection;
        if (col == null) return 0;

        int spriteId = sprite.spriteId;
        if (spriteId < 0 || spriteId >= col.spriteDefinitions.Length)
            return 0;

        var def = col.spriteDefinitions[spriteId];

        int matId = def.materialId;
        if (matId < 0) return 0;

        return matId;
    }

    public static Texture2D? LoadTextureFromGameRoot(string relativePathFromRoot, bool readable = true)
    {
        // 例：relativePathFromRoot = "Mods/Customizer/MySkin/atlas0.png"
        string fullPath = Path.Combine(Paths.GameRootPath, relativePathFromRoot);
        //Debug.Log("图片路径" + fullPath);
        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"PNG not found: {fullPath}");
            return null;
        }

        if(keyValuePairs.TryGetValue(fullPath,out Texture2D value))
        {
            return value;
        }

        byte[] bytes = File.ReadAllBytes(fullPath);

        // TextureFormat usually defaults to RGBA32; the dimensions will be overridden by LoadImage.
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        // Setting `readable=false` makes the pixels unreadable, but saves more video memory; it depends on whether you need to use `GetPixels` later.
        bool ok = ImageConversion.LoadImage(tex, bytes, !readable);
        if (!ok)
        {
            Object.Destroy(tex);
            Debug.LogWarning($"LoadImage failed: {fullPath}");
            return null;
        }
        keyValuePairs.Add(fullPath, tex);
        return tex;
    }

    static void ApplyClonedCollectionToSprite(tk2dBaseSprite sprite, tk2dSpriteCollectionData cloned)
    {
        if (sprite == null || cloned == null) return;

        int id = sprite.spriteId;      // Remember the current spriteId (this is very important).
        cloned.name = sprite.Collection.name;
        sprite.Collection = cloned;    // ✅ Pointing to the cloned collection
        sprite.SetSprite(id);          // ✅ Refresh display
    }

    static tk2dSpriteCollectionData CloneCollection(tk2dSpriteCollectionData original)
    {
        if (original == null) return null;

        // 1) clone collection（ScriptableObject）
        var clone = Object.Instantiate(original);
        clone.name = original.name + "_CLONE";

        // 2) Important: Clone the materials (but do not change the main texture).
        if (clone.materials != null && clone.materials.Length > 0)
        {
            var mats = new Material[clone.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                var src = clone.materials[i];
                if (src == null) continue;

                var m = new Material(src);   // ✅ Independent material
                m.name = src.name + "_CLONE";
                mats[i] = m;
            }
            clone.materials = mats;
        }
        else if (clone.material != null)
        {
            var m = new Material(clone.material);
            m.name = clone.material.name + "_CLONE";
            clone.material = m;
        }

        // 3) If `textures[]` exists, maintain the reference (do not replace it).
        // No action is needed here.

        // 4) Refresh the internal index (call this function if it exists, otherwise it's fine).
        try { clone.InitDictionary(); } catch { }

        return clone;
    }
}
