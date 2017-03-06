using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class ysEffectBundleData : ysEditorData
{
    public enum EffectType
    {
        ui,
        function,
    }

    public class SkillCombieInfo
    {
        public string skillerName;
        public string skillerNameCN;
        public List<GameObject> skillPrefabs;
 

    }
    public struct Outputs 
    {
        public string skillEffectDirectoryPath;
    }

    public Outputs outputs = new Outputs();
    public string[] ignoreStrings = new string[]{"aaaaa" };
    public string inputDirectoryPathx00;
    public string effectDirectoryPath;
    public string functionBundleName = "functionEffect";
    public string uiEffectBundleName = "uiEffect";
    public void GeneratePath()
    {
        inputDirectoryPath = Application.dataPath + "/Resources/Prefab/Effect/function/";
        inputDirectoryPathx00 = Application.dataPath + "/Resources/Prefab/Effect/ui/";
        effectDirectoryPath = Application.dataPath + "/Resources/Prefab/Effect/skill/";
        outputDirectoryPath = Application.dataPath + "/../../../design/build/android/effect/";
        outputs.skillEffectDirectoryPath = Application.dataPath + "/../../../design/build/android/effect/skill";


    }
}

public class ysEffectBundleFunc : ysEditorFunc
{
    public ysEffectBundleData editorData;

    public static void BatchModeExportAssets()
    {
        ysEffectBundleData data = ScriptableObject.CreateInstance<ysEffectBundleData>();
        data.GeneratePath();
        ysEffectBundleFunc func = new ysEffectBundleFunc();
        List<GameObject> prefabs = func.ReadPrefabList(data.inputDirectoryPath);
        List<GameObject> uiprefabs = func.ReadPrefabList(data.inputDirectoryPathx00);
        func.ExportPrefabs(uiprefabs.ToArray(), data.buildTarget, "uiEffect", data.outputDirectoryPath,false);
        func.ExportPrefabs(prefabs.ToArray(), data.buildTarget, "functionEffect", data.outputDirectoryPath, false);
    }


    public List<ysEffectBundleData.SkillCombieInfo> ReadSkillArtInfoList()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(editorData.effectDirectoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();
        List<string> skillerNameList = new List<string>();
        List<ysEffectBundleData.SkillCombieInfo> skillArtInfoList = new List<ysEffectBundleData.SkillCombieInfo>();
        ysEffectBundleData.SkillCombieInfo skillArtInfo = new ysEffectBundleData.SkillCombieInfo();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".prefab") && allFiles[i].Name.Contains("_") && !allFiles[i].Name.Contains(".meta") && !CheckStringMatch(allFiles[i].Name, editorData.ignoreStrings))
            {
                string[] tempx00 = allFiles[i].Name.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                int tempx01 = tempx00.Length;
                string skillerName = tempx01>4?tempx00[3]:"common";
                string skillerType = tempx01 > 5 ? tempx00[4]:"common";
                if (!skillerNameList.Contains(skillerName))
                {
                    skillerNameList.Add(skillerName);
                    skillArtInfo = new ysEffectBundleData.SkillCombieInfo();
                    skillArtInfo.skillPrefabs = new List<GameObject>();
                    skillArtInfo.skillerName = skillerName;
                    //skillArtInfo.skillerNameCN = CheckNameMap(skillerName);
                    skillArtInfoList.Add(skillArtInfo);
                }
                else
                {

                }
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] { Path.DirectorySeparatorChar + "Assets" }, 0)[1];
                skillArtInfo.skillPrefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            }
        }
        return skillArtInfoList;
    }
    public void ExportAllSkillArtPrefab(List<ysEffectBundleData.SkillCombieInfo> skillArtInfoList,bool isShowDialog = false)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[skillArtInfoList.Count];
        for (int i = 0; i < skillArtInfoList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatOutputName(skillArtInfoList[i].skillerName);
            string[] paths = new string[skillArtInfoList[i].skillPrefabs.Count];
            for (int j = 0; j < skillArtInfoList[i].skillPrefabs.Count; j++)
            {
                paths[j] = AssetDatabase.GetAssetPath(skillArtInfoList[i].skillPrefabs[j]);
            }
            buildArray[i].assetNames = paths;
        }
        BuildTarget buildTaget = BuildTarget.Android;
        switch (editorData.buildTarget)
        {
            case ysEditorData.Platform.android:
                buildTaget = BuildTarget.Android;
                break;
            case ysEditorData.Platform.ios:
                buildTaget = BuildTarget.iOS;
                break;
            default:
                break;
        }
        CheckDirectoryPath(editorData.outputs.skillEffectDirectoryPath);
        BuildPipeline.BuildAssetBundles(
        editorData.outputs.skillEffectDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("提示", "文件导出成功!", "继续");
        }
        Debug.Log("All prefab exports succeed...");
    }
}

public class ysEffectBundleWindow : ysEditorWindow
{
    [MenuItem("Tools/特效打包 &E", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.effect);
        return canShowWindow;
    }
    [MenuItem("Tools/特效打包 &E")]
    public static void ShowEditorWindow()
    {   
        window = EditorWindow.GetWindow(typeof(ysEffectBundleWindow), true);
    }
    ysEffectBundleFunc func;
    ysEffectBundleData data;
    List<GameObject> displayList = new List<GameObject>();
    List<GameObject> funcPrefabs = new List<GameObject>();
    List<GameObject> uiprefabs = new List<GameObject>();
    List<ysEffectBundleData.SkillCombieInfo> skillEffectList = new List<ysEffectBundleData.SkillCombieInfo>();
    public  void Init()
    {
        data = ScriptableObject.CreateInstance<ysEffectBundleData>();
        data.GeneratePath();
        func = new ysEffectBundleFunc();
        func.editorData = data;
        displayList = func.ReadPrefabList(data.inputDirectoryPath);
        //funcPrefabs = func.ReadPrefabList(data.inputDirectoryPath);
        //uiprefabs = func.ReadPrefabList(data.inputDirectoryPathx00);
    }
    void OnEnable()
    {
        Init();
        RegisterTabButtons();
    }
    void OnGUI()
    {
        DrawTabToggleButtons(tabBtns);

        if (tabBtns[2].isSelected)
        {
            DrawScrollView(() => ShowSkillEffectPrefabs(skillEffectList), 1);
        }
        else
        {
            DrawScrollView(() => ShowUiEffectPrefabs(displayList),0);
        }


        DrawButton("打包功能特效", () => ExportFunctionEffect());
        DrawButton("打包UI特效", () => ExportUiEffect());
        DrawButton("打包技能特效", () => ExportSkillEffect());
        DrawButton("打包所有特效", () => ExportAll());
    }
    TabToggleBtn[] tabBtns = new TabToggleBtn[3];
    void RegisterTabButtons()
    {
        tabBtns[0] = new TabToggleBtn("function", () => 
        {
            ReadPrefabList(ref funcPrefabs, ysEffectBundleData.EffectType.function);
        });
        tabBtns[1] = new TabToggleBtn("uiEffect", () =>
        {
            ReadPrefabList(ref uiprefabs, ysEffectBundleData.EffectType.ui);
        });
        tabBtns[2] = new TabToggleBtn("skillEffect", () =>
        {
            ReadSkillEffects(ref skillEffectList);
        });
    }
    void ShowUiEffectPrefabs(List<GameObject> prefabList)
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(prefabList[i], typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
        }
    }
    void ShowSkillEffectPrefabs(List<ysEffectBundleData.SkillCombieInfo> prefabList)
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(prefabList[i].skillPrefabs[0], typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();
        }
    }

    void ReadPrefabList(ref List<GameObject> modelInfoList,ysEffectBundleData.EffectType effectType ,bool isShow = true)
    {
        if (modelInfoList == null || modelInfoList.Count < 1)
        {
            modelInfoList = ReadPrefabList(effectType);
        }
        if (isShow)
        {
            displayList = modelInfoList;
        }
    }

    void ReadSkillEffects(ref List<ysEffectBundleData.SkillCombieInfo> sillEffectList)
    {
        if (sillEffectList == null || sillEffectList.Count < 1)
        {
            sillEffectList = func.ReadSkillArtInfoList();
        }
    
    }
    List<GameObject> ReadPrefabList(ysEffectBundleData.EffectType effectType)
    {
        List<GameObject> prefabList = new List<GameObject>();
        switch (effectType)
        {
            case ysEffectBundleData.EffectType.ui:
                prefabList = func.ReadPrefabList(data.inputDirectoryPathx00);
                break;
            case ysEffectBundleData.EffectType.function:
                prefabList = func.ReadPrefabList(data.inputDirectoryPath);
                break;
            default:
                break;
        }
        return prefabList;
    }
    void ExportFunctionEffect()
    {
        if (funcPrefabs==null|| funcPrefabs.Count<1)
        {
          funcPrefabs = func.ReadPrefabList(data.inputDirectoryPath);
        }
        func.ExportPrefabs(funcPrefabs.ToArray(), data.buildTarget, data.functionBundleName, data.outputDirectoryPath,true);
    }
    void ExportUiEffect()
    {
        if (uiprefabs == null || uiprefabs.Count < 1)
        {
            uiprefabs = func.ReadPrefabList(data.inputDirectoryPathx00);
        }
        func.ExportPrefabs(uiprefabs.ToArray(), data.buildTarget,data.uiEffectBundleName, data.outputDirectoryPath, true);
    }
    void ExportSkillEffect(bool isSingleBundle = true)
    {
        if (isSingleBundle)
        {
            List<GameObject> prefabList = new List<GameObject>();

            prefabList = func.ReadPrefabList(data.effectDirectoryPath);

            func.ExportPrefabsIndependent(prefabList, data.buildTarget, data.outputs.skillEffectDirectoryPath,true);

        }
        else
        {

            if (skillEffectList == null || skillEffectList.Count < 1)
            {
                skillEffectList = func.ReadSkillArtInfoList();
            }
            func.ExportAllSkillArtPrefab(skillEffectList, true);
        }
    
    }
    void ExportAll()
    {
        func.ExportPrefabs(funcPrefabs.ToArray(), data.buildTarget, data.functionBundleName, data.outputDirectoryPath, false);
        func.ExportPrefabs(uiprefabs.ToArray(), data.buildTarget, data.uiEffectBundleName, data.outputDirectoryPath, false);
        func.ExportAllSkillArtPrefab(skillEffectList, true);
        EditorUtility.DisplayDialog("提示", "文件导出成功!", "继续");
    }
}