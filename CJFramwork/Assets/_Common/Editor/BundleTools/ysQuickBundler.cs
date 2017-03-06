using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ysQuickBundleData : ysEditorData
{
    public ysQuickBundleData() : base()
    { }
    public ysQuickBundleData(Platform platform) : base(platform)
    { }
    public void GeneratePath()
    {
        //outputDirectoryPath = Application.dataPath + "/../../../design/build/android/";
        outputDirectoryPath = GetDefaultOutputPath();
    }
}
public class ysQuickBundler:ysEditorFunc
{
    [MenuItem("Assets/ExportQuickBundle #&E", true)]
    private static bool CheckCanUseEditor()
    {
        bool isRet = false;

        if (Selection.assetGUIDs.Length < 1)
        {
            return false;
        }
        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        if (Directory.Exists(path))
        {
            isRet = false;
        }
        else if (File.Exists(path))
        {
            isRet = true;
        }
        return isRet;
    }
    [MenuItem("Assets/ExportQuickBundle #&E")]
    public static void ExportQuickBundle()
    {
        if (Selection.assetGUIDs.Length<1)
        {
            return;
        }
        ysQuickBundleData editorData = new ysQuickBundleData(ysEditorData.Platform.android);
        editorData.buildTarget = ysEditorData.Platform.android;
        editorData.GeneratePath();
        string outputPath = Path.GetFullPath(editorData.outputDirectoryPath);
        string saveName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]));
        string pathx00 =  EditorUtility.SaveFilePanel("快速导出", outputPath, saveName, "unity3d");
        UnityEngine.Object[] outputAssets = new UnityEngine.Object[Selection.assetGUIDs.Length];
        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            outputAssets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
        }
        ysEditorFunc editorFunc = new ysEditorFunc();
        if (!string.IsNullOrEmpty(pathx00))
        {
            editorFunc.ExportUnityObjects(outputAssets, editorData.buildTarget, Path.GetFileNameWithoutExtension(pathx00), Path.GetDirectoryName(pathx00));
        }
    }

}
