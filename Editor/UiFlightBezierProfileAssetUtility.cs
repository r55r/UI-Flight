/*
 * UI Flight のベジェ profile asset を ProjectWindow 直下へ素早く作成する。
 * package 導入直後に inspector 設定へ入れるよう、最小限の Editor 補助だけを提供する。
 */

using System.IO;
using UnityEditor;
using UnityEngine;

public static class UiFlightBezierProfileAssetUtility
{
    [MenuItem("Assets/Create/Flying Gorilla/UI Flight Default Profile", priority = 310)]
    public static void CreateDefaultProfileAsset()
    {
        var asset = ScriptableObject.CreateInstance<UiFlightBezierProfile>();
        string targetDirectory = ResolveTargetDirectory();
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(targetDirectory, "UiFlightBezierProfile.asset")
        );

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private static string ResolveTargetDirectory()
    {
        Object activeObject = Selection.activeObject;
        if (activeObject == null)
        {
            return "Assets";
        }

        string selectedPath = AssetDatabase.GetAssetPath(activeObject);
        if (string.IsNullOrWhiteSpace(selectedPath))
        {
            return "Assets";
        }

        return AssetDatabase.IsValidFolder(selectedPath)
            ? selectedPath
            : Path.GetDirectoryName(selectedPath)?.Replace('\\', '/') ?? "Assets";
    }
}
