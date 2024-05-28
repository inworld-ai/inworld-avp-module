using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CombineMetallicSmoothness
{
    [MenuItem("LiquidCity/Combine Metallic + Smoothness Textures")]
    public static void CombineTextures()
    {
        var selection = Selection.objects;
        if (selection.Length != 2) {
            Debug.LogError("Please select exactly two texture assets");
            return;
        }

        Texture2D metallic = null;
        Texture2D alpha = null;
        var isRoughness = false;
        foreach (var obj in Selection.objects) {
            if (!(obj is Texture2D tex)) {
                Debug.LogError("Please select exactly two texture assets");
                return;
            }

            if (obj.name.ToLower().Contains("metal")) {
                metallic = tex;
                continue;
            }

            if (obj.name.ToLower().Contains("smooth")) {
                alpha = tex;
                isRoughness = false;
                continue;
            }

            if (obj.name.ToLower().Contains("rough")) {
                alpha = tex;
                isRoughness = true;
            }
        }

        if (metallic == null) {
            Debug.LogError(
                "Failed to auto-detect metallic texture. Please ensure at least one of the textures contains 'metal' somewhere in the name");
            return;
        }

        if (alpha == null) {
            Debug.LogError(
                "Failed to auto-detect roughness/smoothness texture. Please ensure at least one of the textures contains 'rough' or 'smooth' somewhere in the name");
        }

        var folderName = AssetDatabase.GetAssetPath(Selection.objects[0]);
        var lastSlashPosition = folderName.LastIndexOf("/", StringComparison.Ordinal);
        var fileName =
            folderName.Substring(lastSlashPosition + 1, folderName.Length - lastSlashPosition - 1).Split(".")[0];
        folderName = folderName.Substring(0, lastSlashPosition);
        CombineTextures(metallic, alpha, isRoughness, folderName, fileName);
    }

    private static void CombineTextures(Texture2D metallic, Texture2D alpha, bool isRoughness, string folderName,
        string fileName)
    {
        fileName += "_Combined.png";
        var width = Mathf.Min(metallic.width, alpha.width);
        var height = Mathf.Min(metallic.height, alpha.height);
        var factorM = new Vector2((float) metallic.width / width, (float) metallic.height / height);
        var factorA = new Vector2((float) alpha.width / width, (float) alpha.height / height);
        var newTex = new Texture2D(width, height, TextureFormat.ARGB32, true);
        var metallicPixels = metallic.GetPixels(0, 0, Mathf.FloorToInt(width * factorM.x),
            Mathf.FloorToInt(height * factorM.y));
        var alphaPixels =
            alpha.GetPixels(0, 0, Mathf.FloorToInt(width * factorA.x), Mathf.FloorToInt(height * factorA.y));
        if (metallicPixels.Length != alphaPixels.Length) {
            Debug.LogError("Size mismatch! " + metallicPixels.Length + " vs " + alphaPixels.Length);
            return;
        }

        var finalPixels = new Color[metallicPixels.Length];
        for (var i = 0; i < finalPixels.Length; i++) {
            finalPixels[i] = metallicPixels[i];
            finalPixels[i].a = isRoughness ? 1f - alphaPixels[i].r : alphaPixels[i].r;
        }

        newTex.SetPixels(0, 0, width, height, finalPixels);

        newTex.Apply();
        var png = newTex.EncodeToPNG();
        File.WriteAllBytes(folderName + "/" + fileName, png);
        AssetDatabase.Refresh();
    }
}