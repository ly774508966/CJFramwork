using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class ysSceneConfigBundleData : ysEditorData
{
    public string BundleName = "scenesconfig";
    
    public void GeneratePath()
    {
        inputDirectoryPath = Application.dataPath + "/ScenesConfig/";
        outputDirectoryPath = Application.dataPath + "/../../../design/build/android/scenesconfig/";
    }
}
public class ysSceneConfigBundleFunc : ysEditorFunc
{
    public ysSceneConfigBundleData editorData;
   public void Init()
    {
        editorData = ScriptableObject.CreateInstance<ysSceneConfigBundleData>();
        editorData.GeneratePath();
    }

}
public class ysSceneConfigBundleWindow : ysEditorWindow
{
    ysSceneConfigBundleFunc editorFunc = new ysSceneConfigBundleFunc();
    List<GameObject> configList = new List<GameObject>();

    [MenuItem("Tools/场景配置导出 &G", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.scene);
        return canShowWindow;
    }
    [MenuItem("Tools/场景配置导出 &G")]
    public static void ShowEditorWindow()
    {
        window = EditorWindow.GetWindow(typeof(ysSceneConfigBundleWindow), true) as EditorWindow;
    }

    void OnEnable()
    {
        editorFunc.Init();
        configList = editorFunc.ReadPrefabList(editorFunc.editorData.inputDirectoryPath);

    }
    void OnGUI()
    {
        DrawTitle("场景配置");
        DrawButton("导出场景配置", () => ExportConfigBundle());
    }

    public void ExportConfigBundle()
    {
        editorFunc.ExportPrefabs(configList.ToArray(), editorFunc.editorData.buildTarget, editorFunc.editorData.BundleName, editorFunc.editorData.outputDirectoryPath,true);
    }
}
