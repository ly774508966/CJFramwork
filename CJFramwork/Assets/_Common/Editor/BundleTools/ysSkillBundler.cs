using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System;
using System.Reflection;

public class ysSkillBundleData :ysEditorData
{
    public enum SkillerType
    {
        hero,
        monster,
    }
    public enum SkillCastType
    {
        attack,
        dodge,
        skill_1,
        skill_2,
        skill_3,
        aura,
    }
    public class SkillCombieInfo:ysISearchble
    {
        public string skillerName;
        public string skillerNameCN;
        public List<GameObject> skillPrefabs;
        public SkillerType skillOwnerType;
        public SkillCastType skillCastType;
        public string GetSearchString()
        {
            return skillerName;
        }
    }
    public class AmmoArtInfo
    {
        public string skillerName;
        public string skillerNameCN;
        public List<GameObject> skillPrefabs;
    }

    public class BuffArtInfo
    {
        public string skillerName;
        public string skillerNameCN;
        public List<GameObject> skillPrefabs;
    }



    public string[] ignoreStrings = new string[] {"271.prefab" };
    public Dictionary<string, string> nameMap = new Dictionary<string, string>()
    {
        {"huzi","虎子"},
        {"shancha","山茶"},
        {"wangyuqian","王羽千"},
        {"daowei","刀卫"},
        {"gongwei","弓卫"},
        {"huangdaxian","黄大仙"},
        {"qixiaoxuan","祁晓轩" },
        {"zhaoxintong","赵馨彤" },
        {"huangshulang","黄鼠狼" },
        {"shanguiwang","山鬼王" },
        {"hongjiaotu","红脚兔" },
        {"nancunmin","男村民" },
        {"nvcunmin","女村民" },
        {"qingmangyao","青蟒妖" },
        {"shangui","山鬼" },
        {"shanyao","山妖" },
        {"shanzhuyao","山猪妖" },

    };
    public List<SkillCombieInfo> skillArtInfoList = new List<SkillCombieInfo>();

    public struct Inputs
    {
        public string ammoArtDirecotyPath;
        public string buffArtDirecotyPath;

    }
    public struct Outputs
    {
        public string ammoArtDirecotyPath;
        public string buffArtDirecotyPath;
    }
    public Inputs inputPaths;
    public Outputs outputPaths;
    public void GeneratePath()
    {
        if (string.IsNullOrEmpty(inputDirectoryPath))
        {
            inputDirectoryPath = Application.dataPath + "/_Resources/Prefab/SkillArt/";
        }
        if (string.IsNullOrEmpty(outputDirectoryPath))
        {
            outputDirectoryPath = Application.dataPath + "/../../../design/build/android/skillart/";
        }
        outputPaths.ammoArtDirecotyPath = Application.dataPath + "/../../../design/build/android/ammoart/";
        outputPaths.buffArtDirecotyPath = Application.dataPath + "/../../../design/build/android/buffart/";

        inputPaths.ammoArtDirecotyPath = Application.dataPath + "/_Resources/Prefab/AmmoArt/";
        inputPaths.buffArtDirecotyPath = Application.dataPath + "/_Resources/Prefab/BuffArt/";
    }
}
public class ysSkillBundleFunc : ysEditorFunc
{
    public ysSkillBundleData data;
    //public static List<ysSkillBundleData.SkillCombieInfo> skillArtInfoList = new List<ysSkillBundleData.SkillCombieInfo>();
    public ysSkillBundleFunc()
    {
        LoadData();
        //skillArtInfoList = ReadSkillArtInfoList();
        CheckOutputPath(data.outputDirectoryPath);
    }
    public void LoadData()
    {
        data = ScriptableObject.CreateInstance<ysSkillBundleData>();
        data.GeneratePath();
    }
    public List<ysSkillBundleData.SkillCombieInfo> ReadSkillArtInfoList()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(data.inputDirectoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();
        List<string> skillerNameList = new List<string>();
        List<ysSkillBundleData.SkillCombieInfo> skillArtInfoList = new List<ysSkillBundleData.SkillCombieInfo>();
        ysSkillBundleData.SkillCombieInfo skillArtInfo = new ysSkillBundleData.SkillCombieInfo();
        for (int i = 0; i < allFiles.Length; i++)
        {
          
            if (allFiles[i].Name.Contains(".prefab") && allFiles[i].Name.Contains("_") && !allFiles[i].Name.Contains(".meta")&&!CheckStringMatch(allFiles[i].Name,data.ignoreStrings))
            {
                string[] tempx00 = allFiles[i].Name.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                string skillerName = tempx00[1];
                string skillerType = tempx00[0];
                if (!skillerNameList.Contains(skillerName))
                {
                    skillerNameList.Add(skillerName);
                    skillArtInfo = new ysSkillBundleData.SkillCombieInfo();
                    skillArtInfo.skillPrefabs = new List<GameObject>();
                    skillArtInfo.skillerName = skillerName;
                    skillArtInfo.skillerNameCN = CheckNameMap(skillerName);
                    skillArtInfoList.Add(skillArtInfo);
                }
                else
                {

                }
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {  Path.DirectorySeparatorChar + "Assets" }, 0)[1];
                skillArtInfo.skillPrefabs.Add(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            }
        }
        return skillArtInfoList;
    }

    public List<ysSkillBundleData.AmmoArtInfo> ReadAmmoArtInfoList()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(data.inputPaths.ammoArtDirecotyPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();
        List<string> skillerNameList = new List<string>();
        List<ysSkillBundleData.AmmoArtInfo> skillArtInfoList = new List<ysSkillBundleData.AmmoArtInfo>();
        ysSkillBundleData.AmmoArtInfo skillArtInfo = new ysSkillBundleData.AmmoArtInfo();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".prefab") && allFiles[i].Name.Contains("_") && !allFiles[i].Name.Contains(".meta") && !CheckStringMatch(allFiles[i].Name, data.ignoreStrings))
            {
                string[] tempx00 = allFiles[i].Name.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                string skillerName = tempx00[2];
                string skillerType = tempx00[0];
                if (!skillerNameList.Contains(skillerName))
                {
                    skillerNameList.Add(skillerName);
                    skillArtInfo = new ysSkillBundleData.AmmoArtInfo();
                    skillArtInfo.skillPrefabs = new List<GameObject>();
                    skillArtInfo.skillerName = skillerName;
                    skillArtInfo.skillerNameCN = CheckNameMap(skillerName);
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

    public List<ysSkillBundleData.BuffArtInfo> ReadBuffArtInfoList()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(data.inputPaths.buffArtDirecotyPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();
        List<string> skillerNameList = new List<string>();
        List<ysSkillBundleData.BuffArtInfo> skillArtInfoList = new List<ysSkillBundleData.BuffArtInfo>();
        ysSkillBundleData.BuffArtInfo skillArtInfo = new ysSkillBundleData.BuffArtInfo();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".prefab") && allFiles[i].Name.Contains("_") && !allFiles[i].Name.Contains(".meta") && !CheckStringMatch(allFiles[i].Name, data.ignoreStrings))
            {
                string[] tempx00 = allFiles[i].Name.Split(new string[] { "_" }, System.StringSplitOptions.RemoveEmptyEntries);
                string skillerName = tempx00[2];
                string skillerType = tempx00[0];
                if (!skillerNameList.Contains(skillerName))
                {
                    skillerNameList.Add(skillerName);
                    skillArtInfo = new ysSkillBundleData.BuffArtInfo();
                    skillArtInfo.skillPrefabs = new List<GameObject>();
                    skillArtInfo.skillerName = skillerName;
                    skillArtInfo.skillerNameCN = CheckNameMap(skillerName);
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

    [System.Obsolete("Use overwrite instead.")]
    public void ExportOneSkillArtPrefab(GameObject prefab)
    {
        string path = AssetDatabase.GetAssetPath(prefab);
        string[] assetnames = { path };
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = prefab.name + ".unity3d";
        buildArray[0].assetNames = assetnames;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (data.buildTarget)
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
        BuildPipeline.BuildAssetBundles(
        data.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);

        Debug.Log(prefab.name + " exports succeed...");
    }
    public void ExportOneSkillCombine(ysSkillBundleData.SkillCombieInfo skillCombine)
    {
       // Debug.Log(Assembly.GetAssembly(this.GetType()));
        //Type typeSkillArtBase = Assembly.Load("Assembly-CSharp").GetType("ysSkillArtBase");
        //Type typeSkillSegmentArt = Assembly.Load("Assembly-CSharp").GetType("ysSkillSegmentArt");

        string[] paths = new string[skillCombine.skillPrefabs.Count];
        for (int i = 0; i < skillCombine.skillPrefabs.Count; i++)
        {
           //GameObject prefab = DeepCopyAsset(skillCombine.skillPrefabs[i]);
           //MonoBehaviour[] monos = prefab.GetComponents<MonoBehaviour>();
           //Type skillArtBase =  monos[0].GetType();
           // skillArtBase.GetFields();
           // FieldInfo[] skillArtBaseFields = skillArtBase.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
           // for (int j = 0; j < skillArtBaseFields.Length; j++)
           // {

           //     Debug.Log(skillArtBaseFields[j].Name);

           // }

            paths[i] = GetCopyAssetPath(skillCombine.skillPrefabs[i]); //AssetDatabase.GetAssetPath(skillCombine.skillPrefabs[i]);

        }
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = FormatOutputName(skillCombine.skillerName);
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (data.buildTarget)
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
        BuildPipeline.BuildAssetBundles(
        data.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        EditorUtility.DisplayDialog("提示", skillCombine.skillerName+"技能导出成功!", "继续");
        Debug.Log(skillCombine.skillerName + " exports succeed...");
    }
    public void ExportAllSkillArtPrefab(List<ysSkillBundleData.SkillCombieInfo> skillArtInfoList,bool isShowDialog = false)
    {
        CheckDirectoryPath(data.outputDirectoryPath);
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
        switch (data.buildTarget)
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
        BuildPipeline.BuildAssetBundles(
        data.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("提示", "文件导出成功!", "继续");
            EditorUtility.OpenFilePanelWithFilters("检查文件", Path.GetFullPath(data.outputDirectoryPath), new string[] { "unity3d", "unity3d" });
        }
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportAllAmmoArtPrefab(List<ysSkillBundleData.AmmoArtInfo> infoList,bool isShowDialog = false)
    {
        CheckDirectoryPath(data.outputPaths.ammoArtDirecotyPath,true);
        AssetBundleBuild[] buildArray = new AssetBundleBuild[infoList.Count];
        for (int i = 0; i < infoList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatAmmoOutputName(infoList[i].skillerName);
            string[] paths = new string[infoList[i].skillPrefabs.Count];
            for (int j = 0; j < infoList[i].skillPrefabs.Count; j++)
            {
                paths[j] = AssetDatabase.GetAssetPath(infoList[i].skillPrefabs[j]);
            }
            buildArray[i].assetNames = paths;
        }
        BuildTarget buildTaget = BuildTarget.Android;
        switch (data.buildTarget)
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
        BuildPipeline.BuildAssetBundles(
        data.outputPaths.ammoArtDirecotyPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        EditorUtility.DisplayDialog("提示", "文件导出成功!", "继续");
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportAllBuffArtPrefab(List<ysSkillBundleData.BuffArtInfo> infoList,bool isShowDialog = false)
    {
        CheckDirectoryPath(data.outputPaths.buffArtDirecotyPath,true);
        AssetBundleBuild[] buildArray = new AssetBundleBuild[infoList.Count];
        for (int i = 0; i < infoList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatBuffOutputName(infoList[i].skillerName);
            string[] paths = new string[infoList[i].skillPrefabs.Count];
            for (int j = 0; j < infoList[i].skillPrefabs.Count; j++)
            {
                paths[j] = AssetDatabase.GetAssetPath(infoList[i].skillPrefabs[j]);
            }
            buildArray[i].assetNames = paths;
        }
        BuildTarget buildTaget = BuildTarget.Android;
        switch (data.buildTarget)
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
        BuildPipeline.BuildAssetBundles(
        data.outputPaths.buffArtDirecotyPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("提示", "文件导出成功!", "继续");
            EditorUtility.OpenFilePanelWithFilters("检查文件", Path.GetFullPath(data.outputPaths.buffArtDirecotyPath), new string[] {"unity3d","unity3d" });
        }
        Debug.Log("All prefab exports succeed...");
    }
    public void CheckOutputPath(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }  
    }
    public void CheckBundleFile()
    {

    }
    public ysSkillBundleData.SkillerType CheckSkillerType(string skillStr)
    {
        switch (skillStr)
        {
            case "hero":
                return ysSkillBundleData.SkillerType.hero;              
            case "monster":
                return ysSkillBundleData.SkillerType.monster;         
            default:
                return ysSkillBundleData.SkillerType.hero;
        }
    }
    public string FormatOutputName(string bundleName)
    {
        return "skillart_" + bundleName + ".unity3d";
    }
    public string FormatAmmoOutputName(string bundleName)
    {
        return "ammoart_" + bundleName + ".unity3d";
    }
    public string FormatBuffOutputName(string bundleName)
    {
        return "buffart_" + bundleName + ".unity3d";
    }
    public string CheckNameMap(string name)
    {
        foreach (string key in data.nameMap.Keys)
        {
            if (name.Contains(key))
            {

                return  name.Replace(key, data.nameMap[key])     ;
            }
        }
        return name;
    }
    public static void BatchModeExportAssets()
    {
        ysSkillBundleFunc func = new ysSkillBundleFunc();
        func.LoadData();
        List<ysSkillBundleData.SkillCombieInfo> skillCombineList = func.ReadSkillArtInfoList();
        List<ysSkillBundleData.AmmoArtInfo> ammoArtList = func.ReadAmmoArtInfoList();
        List<ysSkillBundleData.BuffArtInfo> buffArtList = func.ReadBuffArtInfoList();
        func.ExportAllSkillArtPrefab(skillCombineList);
        func.ExportAllAmmoArtPrefab(ammoArtList);
        func.ExportAllBuffArtPrefab(buffArtList);
    }
}
public class ysSkillBundleWindow : ysEditorWindow
{
    [MenuItem("Tools/技能打包 &S", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.client);
        return canShowWindow;
    } 
    [MenuItem("Tools/技能打包 &S")]
    public static void ShowEditorWindow()
    {
        Init();
        window = EditorWindow.GetWindow(typeof(ysSkillBundleWindow), true);
    }
    void OnEnable()
    {



    }

    void OnGUI()
    {
        UseAltAndKeyBoardToCloseWindow(KeyCode.S);
        DrawTitle("技能列表");
        if (DrawSearchBar(() => RegiesterSearchList(displaySkillCombineList, 0)))
        {
            searchSkillCombineList = GetSearchResult<ysSkillBundleData.SkillCombieInfo>(0);
            DrawScrollView(() => ShowSkillList(searchSkillCombineList), 0);
        }
        else
        {
            DrawScrollView(() => ShowSkillList(displaySkillCombineList), 1);
        }
        //DrawVertical(() => ShowInOutPath());
        DrawHorizental(() => ExportAllSkillArt());
        DrawDragbleTextField("拖拽测试", 0);
    }

    static ysSkillBundleFunc func;

    public static List<ysSkillBundleData.SkillCombieInfo> skillCombineList = new List<ysSkillBundleData.SkillCombieInfo>();
    public static List<ysSkillBundleData.SkillCombieInfo> displaySkillCombineList = new List<ysSkillBundleData.SkillCombieInfo>();
    public static List<ysSkillBundleData.SkillCombieInfo> searchSkillCombineList = new List<ysSkillBundleData.SkillCombieInfo>();

    public static List<ysSkillBundleData.AmmoArtInfo> ammoArtInfoList = new List<ysSkillBundleData.AmmoArtInfo>();
    public static List<ysSkillBundleData.BuffArtInfo> buffArtInfoList = new List<ysSkillBundleData.BuffArtInfo>();

    public static List<GameObject> ammoArtList = new List<GameObject>();
    public static List<GameObject> buffArtList = new List<GameObject>();



    public static void Init()
    {
      
        func = new ysSkillBundleFunc();
        skillCombineList = func.ReadSkillArtInfoList();
        displaySkillCombineList = skillCombineList;
        ammoArtInfoList = func.ReadAmmoArtInfoList();
        buffArtInfoList = func.ReadBuffArtInfoList();

        ammoArtList = func.ReadPrefabList(func.data.inputPaths.ammoArtDirecotyPath);
        buffArtList = func.ReadPrefabList(func.data.inputPaths.buffArtDirecotyPath);

    }
    public void ShowSkillList(List<ysSkillBundleData.SkillCombieInfo> skillCombineList)
    {
        string[] skillerNames = new string[skillCombineList.Count];
        bool[] prefabSelectBtn = new bool[skillCombineList.Count];
        bool[] prefabExportBtn = new bool[skillCombineList.Count];

        for (int i = 0; i < skillCombineList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            skillerNames[i] = EditorGUILayout.TextField(skillCombineList[i].skillerName, GUILayout.Width(180)); //(GameObject)EditorGUILayout.ObjectField(skillCombineList[i].prefab, typeof(GameObject), true, GUILayout.Width(180));
            prefabSelectBtn[i] = GUILayout.Button("选择",GUILayout.Width(100));
            prefabExportBtn[i] = GUILayout.Button("导出",GUILayout.Width(100));
            if (prefabSelectBtn[i])
            {
                Selection.activeGameObject = skillCombineList[i].skillPrefabs[0];
            }
            if (prefabExportBtn[i])
            {
                func.ExportOneSkillCombine(skillCombineList[i]);
                //func.ExportOneSkillArtPrefab(prefab[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    public void ExportAllSkillArt()
    {
        if (GUILayout.Button("导出技能"))
        {
            func.ExportAllSkillArtPrefab(skillCombineList,true);
        }
        if (GUILayout.Button("导出Ammo"))
        {
            // func.ExportAllAmmoArtPrefab(ammoArtInfoList);
            func.ExportPrefabsIndependent(ammoArtList, func.data.buildTarget, func.data.outputPaths.ammoArtDirecotyPath);
        }
        if (GUILayout.Button("导出Buff"))
        {
            // func.ExportAllBuffArtPrefab(buffArtInfoList);
            func.ExportPrefabsIndependent(buffArtList, func.data.buildTarget, func.data.outputPaths.buffArtDirecotyPath);
        }

    }
    public void ShowInOutPath()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输入路径",GUILayout.Width(60));
        func.data.inputDirectoryPath = EditorGUILayout.TextField(func.data.inputDirectoryPath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出路径", GUILayout.Width(60));
        func.data.outputDirectoryPath = EditorGUILayout.TextField(func.data.outputDirectoryPath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("配置路径", GUILayout.Width(60));
        func.configSetting.FullPath = EditorGUILayout.TextField(func.configSetting.FullPath);
        EditorGUILayout.EndHorizontal();
    }
}
