using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ysEditorData:Editor
{
    #region 平台相关
    //"Default","Web","Standalone","iPhone","Android"
    public enum Platform
    {
        android,
        ios,
    }
    public Platform buildTarget;
    #endregion

    #region 数据相关
    public enum DataState
    {
        obselete,
        normal,
        needUpdate,
        Updated,
        missing,
    }
    #endregion

    #region 路径相关
    public string bundleName;
    public string inputDirectoryPath;
    public string outputDirectoryPath;
    public string errorLogPath;
    public ysEditorData()
    {

    }
    public ysEditorData(Platform buildTarget)
    {
        this.buildTarget = buildTarget;
    }
    public string FormatAssetPath2FullPath(string assetPath)
    {
        string[] tempx00 = assetPath.Split('/');
        string tempx01 = "";
        for (int i = 0; i < tempx00.Length; i++)
        {
            if (i == 0&& tempx00.Length>1)
            {
                tempx00[i] = tempx00[i].Replace("Assets", null);
            }
            tempx01 += (tempx00[i]+"/");
        }
        return Application.dataPath + tempx01;
    }

    public string FormatFullPath2AssetPath(string fullPath)
    {
        return fullPath.Substring(fullPath.IndexOf("/Assets") + 1);
    }

    protected string GetOutputPath(string dirctory = null)
    {
        string platformDirectory = "android";
        switch (buildTarget)
        {
            case Platform.android:
                platformDirectory = "android";
                break;
            case Platform.ios:
                platformDirectory = "ios";
                break;
            default:
                platformDirectory = "android";
                break;
        }
        string directoryName = dirctory==null?dirctory:(dirctory+"/");
        return Application.dataPath + "/../../../design/build/"+ platformDirectory + "/" + directoryName;
    }
    protected string GetDefaultOutputPath(string dirctory = null)
    {
        string platformDirectory = "android";
        switch (buildTarget)
        {
            case Platform.android:
                platformDirectory = "android";
                break;
            case Platform.ios:
                platformDirectory = "ios";
                break;
            default:
                platformDirectory = "android";
                break;
        }
        string directoryName = dirctory == null ? dirctory : (dirctory + "/");
        return Application.dataPath + "/../../../design/build/" + platformDirectory + "/" + directoryName;
    }
    public string GetErrorLogPath()
    {
        string errPath = Application.dataPath + "/../../../design/build/errorlog/";
        if (!Directory.Exists(Path.GetFullPath(errPath) ))
        {
            Directory.CreateDirectory(Path.GetFullPath(errPath));
        }
        return errPath;
    }
    #endregion
}
public class ysEditorFunc
{
    #region 配置读写

    public struct ConfigSetting
    {
        public string DirectoryPath;
        public string Name;
        public string Extension;
        public string FullPath;
    }
    public ConfigSetting configSetting;
    //public ysEditorData editorData;
    public void ReadConfigData<T>(T data)
    {
        using (FileStream fs = new FileStream(configSetting.FullPath, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            data = (T)formatter.Deserialize(fs);
        }
    }
    public void SaveConfigData<T>(T data)
    {
        using (FileStream fs = new FileStream(configSetting.FullPath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, data);
            fs.Close();
        }
    }
    public static class TextUtility
    {
        public static void Write2File(string filePath, string text)
        {
            filePath = Path.GetFullPath(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            } ;
        }
        public static void Write2File(string directoryPath,string fileName, string text)
        { 
            string filePath = Path.Combine(directoryPath, fileName);
            Write2File(filePath, text);
        }
        public static string ReadAllFile(string filePath)
        {
            return File.ReadAllText(filePath);
            //using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default))
            //{
            //    string text = sr.ReadToEnd();
            //    return text;
            //};
        }
        public static string ReadAllFile(string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            return ReadAllFile(filePath);
        }
    }
    #endregion

    #region 列表读取
    public List<GameObject> ReadPrefabList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();

        List<GameObject> prefabList = new List<GameObject>();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".prefab") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add( AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<UnityEngine.Object> ReadObjectList(string inputDirctoryPath,string extention)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();

        List<UnityEngine.Object> prefabList = new List<UnityEngine.Object>();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(extention) && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<Material> ReadMaterialList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();

        List<Material> prefabList = new List<Material>();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".mat") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<Material>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<MonoScript> ReadMonoScriptList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();

        List<MonoScript> prefabList = new List<MonoScript>();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".cs") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<MonoScript>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<ScriptableObject> ReadScriptableObjectList(string inputDirctoryPath)
    {
        //inputDirctoryPath is a fullPath startfrom C:/user/...;
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles();

        List<ScriptableObject> prefabList = new List<ScriptableObject>();
        for (int i = 0; i < allFiles.Length; i++)
        {

            if (allFiles[i].Name.Contains(".asset") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<Texture> ReadTextureList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*",SearchOption.AllDirectories);

        List<Texture> prefabList = new List<Texture>();
        for (int i = 0; i < allFiles.Length; i++)
        {
            if ((allFiles[i].Name.Contains(".png")||allFiles[i].Name.Contains(".jpg")) && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<Texture>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<TextAsset> ReadTextAssetList(string inputDirctoryPath)
    {
        List<TextAsset> prefabList = new List<TextAsset>();
        if (!inputDirctoryPath.Contains("Assets"))
        {
            Debug.LogError("此路径不在工程内！");
            return prefabList;
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
       
        for (int i = 0; i < allFiles.Length; i++)
        {
            if ((allFiles[i].Name.Contains(".txt")) && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<TextAsset>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<Font> ReadFontList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

        List<Font> prefabList = new List<Font>();
        for (int i = 0; i < allFiles.Length; i++)
        {
            if (allFiles[i].Name.Contains(".ttf") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                prefabList.Add(AssetDatabase.LoadAssetAtPath<Font>(prefabPath));
            }
        }
        return prefabList;
    }
    public List<Shader> ReadShaderList(string inputDirctoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(inputDirctoryPath);
        FileInfo[] allFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

        List<Shader> assetList = new List<Shader>();
        for (int i = 0; i < allFiles.Length; i++)
        {
            if (allFiles[i].Name.Contains(".shader") && !allFiles[i].Name.Contains(".meta"))
            {
                string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] {   Path.DirectorySeparatorChar+"Assets" }, 0)[1];
                assetList.Add(AssetDatabase.LoadAssetAtPath<Shader>(prefabPath));
            }
        }
        return assetList;
    }
    #endregion

    #region 导出文件

    //单独打包一个prefab到一个bundle
    public void ExportPrefab(GameObject prefab, ysEditorData.Platform buildTarget,string outputDirectoryPath)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = FormatOutputName(prefab.name);
        buildArray[0].assetNames =new string[] { AssetDatabase.GetAssetPath(prefab) };
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);

        Debug.Log(prefab.name + " exports succeed...");
    }
    //打包多个prefab到一个bundle
    public void ExportPrefabs(GameObject[] prefabs, ysEditorData.Platform buildTarget, string bundleName, string outputDirectoryPath,bool showResultDialog = false)
    {
        if (!Directory.Exists(outputDirectoryPath))
        {
            Directory.CreateDirectory(outputDirectoryPath);
        }
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(prefabs[i]);
        }
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("All prefab exports succeed...");
        if (showResultDialog)
        {
            EditorUtility.DisplayDialog("提示", "文件导出成功，共" + prefabs.Length + "个文件,共" + buildArray.Length + "个资源包。","继续");
            EditorUtility.OpenFilePanel("导出路径", Path.GetFullPath(outputDirectoryPath), "");
        }
    }
    public void ExportPrefabs(UnityEngine.Object[] prefabs, ysEditorData.Platform buildTarget, string bundleName, string outputDirectoryPath)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];

        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(prefabs[i]);
        }
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportUnityObjects(UnityEngine.Object[] prefabs, ysEditorData editorData)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];

        buildArray[0].assetBundleName = FormatOutputName(editorData.bundleName);
        string[] paths = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(prefabs[i]);
        }
        buildArray[0].assetNames = paths;
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
        BuildPipeline.BuildAssetBundles(
        editorData.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportUnityObjects(UnityEngine.Object[] prefabs, ysEditorData.Platform buildTarget, string bundleName, string outputDirectoryPath,string extention ="unity3d")
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];

        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(prefabs[i]);
        }
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        EditorUtility.OpenFilePanel("输出路径", Path.GetFullPath(outputDirectoryPath), extention);
        //Debug.Log("All prefab exports succeed...");
    }

    public void ExportScriptableObjects(ScriptableObject[] scriptableObjects, ysEditorData.Platform buildTarget, string bundleName, string outputDirectoryPath)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];

        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[scriptableObjects.Length];
        for (int i = 0; i < scriptableObjects.Length; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(scriptableObjects[i]);
        }
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportScriptableObjects(List<ScriptableObject> scriptableObjects, ysEditorData.Platform buildTarget, string bundleName, string outputDirectoryPath,bool isShowDialog = false)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];

        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[scriptableObjects.Count];
        for (int i = 0; i < scriptableObjects.Count; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(scriptableObjects[i]);
        }
        buildArray[0].assetNames = paths;
        BuildTarget buildTaget = BuildTarget.Android;
        switch (buildTarget)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        string tipString = "导出成功，共导出" + scriptableObjects.Count + "个文件，1个资源包，资源包名：" + bundleName + "资源路径：" + outputDirectoryPath;
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("提示", tipString, "继续");
        }
        else
        {
            Debug.Log(tipString);
        }
    }
    //打包多个prefab到多个bundle
    public void ExportPrefabsIndependent(List<GameObject> prefabList, ysEditorData.Platform platform, string outputDirectoryPath, bool isShowDialog = false)
    {
        CheckDirectoryPath(outputDirectoryPath, true);
        AssetBundleBuild[] buildArray = new AssetBundleBuild[prefabList.Count];
        for (int i = 0; i < prefabList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatOutputName(prefabList[i].name); 
            buildArray[i].assetNames = new string[] { AssetDatabase.GetAssetPath(prefabList[i]) };
        }
        BuildTarget buildTaget = BuildTarget.Android;
        switch (platform)
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
        outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        string tipString = "导出成功，共导出" + prefabList.Count + "个文件，1个资源包，资源路径：" + outputDirectoryPath;
        if (isShowDialog)
        {
            EditorUtility.DisplayDialog("提示", tipString, "继续");
        }
        else
        {
            Debug.Log(tipString);
        }
    }
    public bool CheckDirectoryPath(string fullPath,bool isCreate = false)
    {
        bool isRet = false;
        if (!Directory.Exists(Path.GetFullPath(fullPath)))
        {
            if (isCreate)
            {
                Directory.CreateDirectory(Path.GetFullPath(fullPath));
                isRet = true;
            }
            else
            {
                isRet = false;
            }
        }
        else
        {
            isRet = true;
        }

        return isRet;

    }
    public void ExportUnityObjectsIndependent(List<UnityEngine.Object> prefabList, ysEditorData editorData)
    {

        AssetBundleBuild[] buildArray = new AssetBundleBuild[prefabList.Count];
        for (int i = 0; i < prefabList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatOutputName(prefabList[i].name);
            buildArray[i].assetNames = new string[] { AssetDatabase.GetAssetPath(prefabList[i]) };
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
        BuildPipeline.BuildAssetBundles(
        editorData.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("All prefab exports succeed...");
    }
    public string FormatOutputName(string bundleName,string extention = "unity3d")
    {
        return  bundleName + "."+ extention;
    }
    
    #endregion

    #region 深度拷贝
    public GameObject DeepCopyAsset(GameObject asset)
    {
        string srcPath = AssetDatabase.GetAssetPath(asset);
        //string[] srcPathArray = srcPath.Split('/');
        //string upperDirectory = srcPathArray[srcPathArray.Length - 2];
        //string cloneDirectory = upperDirectory + "_clone";
        string dstPath = GetCopyAssetPath(asset);
        string cloneDirectoryPath = Path.GetDirectoryName(dstPath);
        if (!Directory.Exists(cloneDirectoryPath))
        {
            Directory.CreateDirectory(cloneDirectoryPath);
        }
        if (!File.Exists(dstPath))
        {
            AssetDatabase.CopyAsset(srcPath, dstPath);
        }
        return AssetDatabase.LoadAssetAtPath<GameObject>(dstPath);
    }
    public UnityEngine.Object DeepCopyAsset(UnityEngine.Object asset)
    {
        string srcPath = AssetDatabase.GetAssetPath(asset);
        //string[] srcPathArray = srcPath.Split('/');
        //string upperDirectory = srcPathArray[srcPathArray.Length - 2];
        //string cloneDirectory = upperDirectory + "_clone";
        string dstPath = GetCopyAssetPath(asset);
        string cloneDirectoryPath = Path.GetDirectoryName(dstPath);
        if (!Directory.Exists(cloneDirectoryPath))
        {
            Directory.CreateDirectory(cloneDirectoryPath);
        }
        if (!File.Exists(dstPath))
        {
            AssetDatabase.CopyAsset(srcPath, dstPath);
        }
        return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(dstPath);
    }
    //拷贝文件会生成在/xxxx_clone文件夹下
    public string GetCopyAssetPath(UnityEngine.Object asset)
    {
        string srcPath = AssetDatabase.GetAssetPath(asset);
        string[] srcPathArray = srcPath.Split('/');
        string upperDirectory = srcPathArray[srcPathArray.Length - 2];
        string cloneDirectory = upperDirectory + "_clone";
        string dstPath = srcPath.Replace(upperDirectory, cloneDirectory);
        return dstPath;
    }
    public string GetCopyAssetDirctory(UnityEngine.Object asset)
    {
        string dstPath = GetCopyAssetPath(asset);
        string cloneDirectoryPath = Path.GetDirectoryName(dstPath);
        return cloneDirectoryPath;
    }

    #endregion

    #region 字符检查
    protected bool CheckStringMatch(string needCheck,string[] matchArray)
    {
        bool isMatched = false;
        string[] ignoreStrings = matchArray;
        for (int i = 0; i < matchArray.Length; i++)
        {
            isMatched |= needCheck.Contains(matchArray[i]);
        }
        return isMatched;
    }
    protected bool CheckStringEqual(string needCheck, string[] matchArray)
    {
        bool isMatched = false;
        string[] ignoreStrings = matchArray;
        for (int i = 0; i < matchArray.Length; i++)
        {
            isMatched |= needCheck.Equals(matchArray[i]);
        }
        return isMatched;
    }
    #endregion

    #region 算法功能
    public object[] CombineArray(object[] arrayX, object[] arrayY)
    {
        object[] arrayXY = new object[arrayX.Length + arrayY.Length];
        arrayX.CopyTo(arrayXY, 0);
        arrayY.CopyTo(arrayXY, arrayX.Length);
        return arrayXY;
    }

    #endregion
}
public class ysEditorWindow:EditorWindow
{
    #region 结构变量
    protected bool isExtentionMode = false;
    private GUIStyle titleStyle;
    public static EditorWindow window = null;
    protected static bool canShowWindow = false;
    protected Vector2[] scrollViewPos = new Vector2[32];    //最大滚面板
    protected struct DragbleTextField
    {
        private Rect m_area;
        private string m_text;
        public Rect area
        {
            get
            {
                return m_area;
            }

            set
            {
                m_area = value;
            }
        }
        public string text
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
            }
        }
    }
    protected struct TabToggleBtn
    {
        public string label;
        public bool isSelected;
        public Action onBtnClickHandler;
        public TabToggleBtn(string lable,Action onBtnClickHandler)
        {
            this.label = lable;
            this.isSelected = false;
            this.onBtnClickHandler = onBtnClickHandler;
        }
    }
    protected DragbleTextField[] dragbleTextField = new DragbleTextField[32]; //最大可拖拽文本框 

    #endregion

    #region 快捷设置

    //alt-&,ctrl-%,shift-#,LEFT/RIGHT/UP/DOWN/HOME/END/PGUP/PGDN/F1../_*[a~z]

    private static int closeTrigger = 0;
    private static int extentionTrigger = 0;
    private static int keyUpInterval = 0;
    protected void UseAltAndKeyBoardToCloseWindow(KeyCode keyCode)
    {
        if (Event.current.alt && Event.current.keyCode == keyCode)
        {
            closeTrigger++;
            if (keyUpInterval > 0)
            {
                keyUpInterval--;
                return;
            }
            keyUpInterval++;
            if (closeTrigger > 3)
            {
                closeTrigger = 0;
                Close();
            }
        }
        if (Event.current.control && Event.current.alt && Event.current.keyCode == KeyCode.E)
        {
            extentionTrigger++;
            if (keyUpInterval>0)
            {
                keyUpInterval--;
                return;
            }
            keyUpInterval++;
            if (extentionTrigger > 3&& keyUpInterval<5)
            {
                extentionTrigger = 0;
                isExtentionMode = !isExtentionMode;
                window.Repaint();
            }
       
        }
    }

    #endregion

    #region 工程模块

    public enum ProgramType
    {
        common,
        client,
        design,
        character,
        scene,
        effect,
    }

    private static ProgramType CheckCurrentProgramType()
    {
        string appPath = Application.dataPath;
        if (appPath.Contains("client"))
        {
            return ProgramType.client;
        }
        else if (appPath.Contains("design"))
        {
            return ProgramType.design;
        }
        else if (appPath.Contains("character"))
        {
            return ProgramType.character;
        }
        else if (appPath.Contains("effect"))
        {
            return ProgramType.effect;
        }
        else if (appPath.Contains("scene"))
        {
            return ProgramType.scene;
        }
        else
        {
            return ProgramType.common;
        }
    }
    protected static bool CheckCanUseEditorAtCurrentProgram(ProgramType applyType)
    {
        return applyType == CheckCurrentProgramType();
    }
   
    #endregion

    #region 界面模块

    public void DrawHorizental(Action action)
    {
        EditorGUILayout.BeginHorizontal();
        action();
        EditorGUILayout.EndHorizontal();
    }
    public void DrawVertical(Action action)
    {
        EditorGUILayout.BeginVertical();
        action();
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// 滑动显示栏（id用于区分不同位置，可以共享）
    /// </summary>
    /// <param name="action"></param>
    /// <param name="id"></param>
    /// <param name="option"></param>
    public void DrawScrollView(Action action,int id,GUILayoutOption option = null)
    {
        //if (id> scrollViewPos.Length-1)
        //{
        //    return;
        //}
        if (option != null)
        {
            scrollViewPos[id] = EditorGUILayout.BeginScrollView(scrollViewPos[id],option);
        }
        else
        {
            scrollViewPos[id] = EditorGUILayout.BeginScrollView(scrollViewPos[id]);
        }
        action();
        EditorGUILayout.EndScrollView();
    }
    public void DrawTitle(string title)
    {
        titleStyle = new GUIStyle("BoldLabel");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(title,titleStyle);
        EditorGUILayout.EndHorizontal();
    }
    public void DrawButton(string btnLabel, Action doAction)
    {
        if (GUILayout.Button(btnLabel))
        {
            doAction();
        }
    }
    /// <summary>
    /// 搜索框（回调必须使用RegiesterSearchList函数注册搜索结果与id）
    /// </summary>
    /// <param name="Register"></param>
    /// <returns></returns>
    public bool DrawSearchBar(Action Register=null)
    {
        bool isRet = false;
        EditorGUILayout.BeginHorizontal();
        searchString = EditorGUILayout.TextField(searchString, GUI.skin.GetStyle("SearchTextField"));
        if (GUILayout.Button("", GUI.skin.GetStyle("SearchCancelButton")))
        {
            searchString = "";
        }
        EditorGUILayout.EndHorizontal();

        if (searchString!=""&&searchString!=null)
        {
            isRet = true;
            Register();
            if (recordString == searchString)
            {
                
            }
            else
            {
                searchResultList = SearchInputList(searchString, searchList);
                recordString = searchString;
            } 
        }
        return isRet;
    }
    /// <summary>
    /// 拓展模式按钮（控制isExtentionMode布尔变量）
    /// </summary>
    /// <param name="width"></param>
    public void DrawExtentionModeToggle(float width = 80)
    {
        isExtentionMode = EditorGUILayout.ToggleLeft("拓展模式",isExtentionMode,GUILayout.Width(width));
    }
    public string DrawDragbleTextField(string label, int id,float labelWidth = 60f, Action action=null)
    {
        if (dragbleTextField==null||    id >dragbleTextField.Length-1)
        {
            return "";
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
        dragbleTextField[id].area = EditorGUILayout.GetControlRect();
        dragbleTextField[id].text = EditorGUI.TextField(dragbleTextField[id].area, dragbleTextField[id].text);
        EditorGUILayout.EndHorizontal();
        if (    
            (Event.current.type==EventType.DragUpdated||Event.current.type==EventType.DragExited)
            && dragbleTextField[id].area.Contains(Event.current.mousePosition)
            )
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (DragAndDrop.paths!=null&&DragAndDrop.paths.Length>0)
            {
                dragbleTextField[id].text = DragAndDrop.paths[0];
                if (action!=null)
                {
                    action();
                }
            }
        }
        return dragbleTextField[id].text;
    }
    /// <summary>
    /// 顶部单选模式按钮（为变量按钮添加回调函数）
    /// </summary>
    /// <param name="toggleBtns"></param>
    /// <param name="onChangeToggleHandler"></param>
    protected void DrawTabToggleButtons(TabToggleBtn[] toggleBtns,Action onChangeToggleHandler = null)
    {
       int firstIndex = 0;
       int lastIndex = toggleBtns.Length - 1;
       EditorGUILayout.BeginHorizontal();
       for (int i = 0; i < toggleBtns.Length; i++)
       {
            GUIStyle style = GUI.skin.GetStyle("ButtonMid");
            if (i== firstIndex)
            {
                style = GUI.skin.GetStyle("ButtonLeft");
            }
            if (i== lastIndex)
            {
                style = GUI.skin.GetStyle("ButtonRight");
            }
            if (GUILayout.Button(toggleBtns[i].label, style))
            {
                if (!toggleBtns[i].isSelected)
                {
                    for (int j = 0; j < toggleBtns.Length; j++)
                    {
                        toggleBtns[i].isSelected = false;
                    }
                    toggleBtns[i].isSelected = true;
                   
                    if (onChangeToggleHandler!=null)
                    {
                        onChangeToggleHandler();
                    }
                }
                if (toggleBtns[i].onBtnClickHandler!=null)
                {
                    toggleBtns[i].onBtnClickHandler();
                }
            }
       }
        EditorGUILayout.EndHorizontal();
    }
    
    #endregion

    #region 检查模块

    private Dictionary<object, int> listRecordId = new Dictionary<object, int>();

    public bool CheckListChange<T>(List<T> list)
    {
        object key = (object)list;
        if (listRecordId.ContainsKey(key))
        {
            if (list.Count != listRecordId[key])
            {
                listRecordId[key] = list.Count;
                return true;
            }
            else
            {
                listRecordId[key] = list.Count;
                return false;
            }
        }
        else
        {
            listRecordId.Add(key, list.Count);
            return true;
        }
    }
    public bool CheckListChange(List<string> list)
    {
        List<string> key = list;
        if (listRecordId.ContainsKey(key))
        {
            if (list.Count != listRecordId[key])
            {
                listRecordId[key] = list.Count;
                return true;
            }
            else
            {
                listRecordId[key] = list.Count;
                return false;
            }
        }
        else
        {
            listRecordId.Add(key, list.Count);
            return true;
        }
    }

    #endregion

    #region 搜索模块 

    private string recordString = "";
    private string searchString = "";
    private List<string> searchList = new List<string>();
    private bool[] isSearchDictMapRegisted = new bool[8];
    private Dictionary<string, object>[] searchDictMap = new Dictionary<string, object>[8];
    public List<string> searchResultList = new List<string>();
    private List<string> SearchInputList(string scanString, List<string> keyList)
    {
        List<string> scanList = new List<string>();
        for (int i = 0; i < keyList.Count; i++)
        {
            for (int j = 0; j < scanString.Length; j++)
            {
                string[] tempStrx01 = keyList[i].Split('_');
                string tempStrx00 = tempStrx01[tempStrx01.Length > 3 ? 2 : tempStrx01.Length - 1];

                string a = j < tempStrx00.Length ? tempStrx00.Substring(0, j + 1).ToLower() : tempStrx00.Substring(0).ToLower();
                string b = scanString.Substring(0, j + 1).ToLower();

                if (b.Contains(a))
                {
                    if (!scanList.Contains(keyList[i]))
                    {
                        scanList.Add(keyList[i]);
                    }
                }
                else
                {
                    if (scanList.Contains(keyList[i]))
                    {
                        scanList.Remove(keyList[i]);
                    }
                }
            }
        }
        scanList.Sort();
        return scanList;
    }
    /// <summary>
    /// 从已经注册搜索id的链表中获取搜索结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<T> GetSearchResult<T>(int id)
    {
        List<T> resultList = new List<T>();
        if (searchDictMap.Length<1)
        {
            return null;
        }
        for (int i = 0; i < searchResultList.Count; i++)
        {
            if (searchResultList[i] == null|| searchDictMap[id]==null)
            {
                continue;
            }
           
            resultList.Add((T)searchDictMap[id][searchResultList[i]]);
        }

       return resultList;
    }
    /// <summary>
    /// 把链表装填在搜索字典中，通过id区分泛型类型，同时泛型必须基于ysISearchble接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="searchList"></param>
    /// <param name="id"></param>
    public void RegiesterSearchList<T>(List<T> searchList, int id)where T:ysISearchble
    {
        if (isSearchDictMapRegisted[id])
        {
            return;
        }
        Dictionary<string, object> dictMap = new Dictionary<string, object>();
        List<string> recoardList = new List<string>();
        for (int i = 0; i < searchList.Count; i++)
        {
            string tableName = searchList[i].GetSearchString();
            if (!dictMap.ContainsKey(tableName))
            {
                dictMap.Add(tableName, searchList[i]);
            }
            recoardList.Add(tableName);
        }
        searchDictMap[id] = dictMap;
        isSearchDictMapRegisted[id] = true;
        this.searchList = recoardList;
        //resultList.Clear();
        //for (int i = 0; i < searchResultList.Count; i++)
        //{
        //    resultList.Add((T)searchDictMap[id][searchResultList[i]]);
        //}
    }
    public void ShowSearchList()
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            EditorGUILayout.TextField(searchList[i]);
        }
    }

    #endregion
}
public interface ysISearchble
{
    /// <summary>
    /// 获取用于搜索的字符串名称
    /// </summary>
    /// <returns></returns>
   string GetSearchString();
}

