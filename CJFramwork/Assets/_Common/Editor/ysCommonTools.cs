using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


public class ysCommonTools : Editor
{
    [MenuItem("Tools/CleanChache")]
    public static void CleanCache()
    {
        Caching.CleanCache();
    }

    [MenuItem("Tools/Test")]
    public static void Test()
    {
        Caching.CleanCache();

        string path = "Assets/_Resources/Prefab/Model/Hero/ch_he_huzi.prefab";
        string[] assetnames = { path };
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = "ch_he_huzi" + ".unity3d";
        buildArray[0].assetNames = assetnames;
        BuildPipeline.BuildAssetBundles(
            Application.dataPath + "/../../../design/build/android/model/",
            buildArray,
            BuildAssetBundleOptions.ForceRebuildAssetBundle,
            BuildTarget.Android);
    }
    protected static string streamingAssets =
#if UNITY_ANDROID
        "/../../../design/build/android";
#elif UNITY_STANDALONE_OSX || UNITY_IPHONE
         "/../../../design/build/ios";
#else
        "/StreamingAssets";
#endif
    protected static BuildTarget buildTarget =
#if UNITY_STANDALONE_OSX || UNITY_IPHONE
        BuildTarget.iPhone;
#elif UNITY_ANDROID
        BuildTarget.Android;
#else 
        BuildTarget.StandaloneWindows;
#endif

    private static string s_luaPath = "/_Resources/Lua";
    public static bool encodeLuaScript = false; // android:luajit, ios:luac
    private static List<Object> luaScript = new List<Object>();
    private static List<string> luaScriptPath = new List<string>();
    

    [MenuItem("Tools/LuaPackage")]
    public static void LuaPackage()
    {
        GenerateLuaByteFiles();
        GenerateLuaGameObjects();
        BuildLuaAssetBundle();
    }

    private static void GenerateLuaByteFiles()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/_Resources/Lua/", "*", SearchOption.AllDirectories);
        string outDir = Application.dataPath + "/LuaPackage";
        DeleteDirectory(outDir);

        outDir += "/Bytes";
        if (!File.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        string pathRoot = Application.dataPath + "/_Resources";
        int rootLen = pathRoot.Length;

        for (int i = 0; i < files.Length; i++)
        {
            //string fname = Path.GetFileName(files[i]);

            if (files[i].IndexOf(".meta") > 0)
            {
                continue;
            }

            string relativePath = files[i].Substring(rootLen);
            string fullPath = outDir + relativePath + ".bytes";
            string strPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            FileUtil.CopyFileOrDirectory(files[i], fullPath);
        }

        AssetDatabase.Refresh();
    }

    private static void GenerateLuaGameObjects()
    {
        luaScript.Clear();
        luaScriptPath.Clear();
        string[] paths = Directory.GetFiles(Application.dataPath + "/LuaPackage/Bytes/Lua", "*.*", SearchOption.AllDirectories);
        foreach (string str in paths)
        {
            if (str.IndexOf(".meta") > 0)
            {
                continue;
            }

            string assetPath = FileUtil.GetProjectRelativePath(str);

            UnityEngine.Object selection = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
            if (selection != null)
            {
                luaScriptPath.Add(str);
                luaScript.Add(selection);
            }
        }
    }

    private static void BuildLuaAssetBundle()
    {
        string S_PackagePath = Application.dataPath + streamingAssets;
        string p = S_PackagePath + "/Lua";
        if (!Directory.Exists(p))
        {
            Directory.CreateDirectory(p);
        }

        //string tempDir = "LuaPackage";
        //string tempPath = Application.dataPath + "/" + tempDir + "/LuaPackage/Bytes/Lua";
        //FileUtil.DeleteFileOrDirectory(tempPath);

        //List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();
        //for (int i = 0; i < luaScript.Count; i++)
        //{
        //    // .meta file
        //    int metaIndex = luaScriptPath[i].LastIndexOf(".meta");
        //    if (metaIndex > 0)
        //    {
        //        continue;
        //    }

        //    string src = Path.GetFullPath(luaScriptPath[i]);
        //    string dst = src.Replace("_Resouces", tempDir);
        //    dst += ".bytes";
        //    int end = dst.LastIndexOf("\\");
        //    if (end < 0)
        //    {
        //        end = dst.LastIndexOf("/");
        //    }
        //    string dstPath = dst.Substring(0, end);
        //    if (!Directory.Exists(dstPath))
        //    {
        //        Directory.CreateDirectory(dstPath);
        //    }

        //    bool isLuaScript = (src.IndexOf(".lua") > 0);
        //    if (encodeLuaScript && isLuaScript)
        //    {
        //        EncodeLuaScript(src, dst);
        //    }
        //    else
        //    {
        //        FileUtil.CopyFileOrDirectory(src, dst);
        //    }

        //    EditorUtility.DisplayProgressBar("lua: " + i + "/" + luaScript.Count, luaScriptPath[i], (float)i / luaScript.Count);
        //}

        //EditorUtility.ClearProgressBar();

        //AssetDatabase.Refresh();

        //string[] paths = Directory.GetFiles(Application.dataPath + "/LuaPackage/Bytes/Lua", "*.*", SearchOption.AllDirectories);
        //string prePath = "Assets/LuaPackage/Bytes/Lua";
        //int preLen = prePath.Length;
        //foreach (string str in paths)
        //{
        //    UnityEngine.Object selection = AssetDatabase.LoadAssetAtPath(str, typeof(UnityEngine.Object));
        //    if (selection != null)
        //    {
        //        string objName = str.Substring(preLen + 1);
        //        objName = objName.Replace('\\', '/');
        //        selection.name = objName;
        //        tempObjects.Add(selection);
        //    }
        //}


        string assetPathRoot = "Assets/LuaPackage/Bytes/Lua/";
        int assetPathRootLen = assetPathRoot.Length;

        foreach (UnityEngine.Object selection in luaScript)
        {
            if (selection != null)
            {
                string objName = selection.name;
                string objPath = AssetDatabase.GetAssetPath(selection);
                selection.name = objPath.Substring(assetPathRootLen);
            }
        }

        p += "/lua.unity3d";
        BuildAssetBundle(null, luaScript.ToArray(), p, 
            BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle, 
            buildTarget);

        string tempRoot = Application.dataPath + "/LuaPackage";
        DeleteDirectory(tempRoot);
        AssetDatabase.Refresh();
    }

    private static void EncodeLuaScript(string inFile, string outFile)
    {
        string luaexe = string.Empty;
        string args = string.Empty;
        string exedir = string.Empty;
        string currDir = Directory.GetCurrentDirectory();
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            luaexe = "luajit.exe";
            args = "-b " + inFile + " " + outFile;
            exedir = Application.dataPath + "/uLua/LuaEncoder/luajit/";
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            luaexe = "./luac";
            args = "-o " + outFile + " " + inFile;
            exedir = Application.dataPath + "/uLua/LuaEncoder/luavm/";
        }
        Directory.SetCurrentDirectory(exedir);
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = luaexe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = true;
        info.ErrorDialog = true;

        Process pro = Process.Start(info);
        pro.WaitForExit();
        Directory.SetCurrentDirectory(currDir);
    }

    protected static bool BuildAssetBundle(UnityEngine.Object mainAsset, UnityEngine.Object[] assets, string pathName, BuildAssetBundleOptions assetBundleOptions, BuildTarget targetPlatform)
    {
        string[] fileNames = new string[assets.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            fileNames[i] = AssetDatabase.GetAssetPath(assets[i]);
        }

        int pos = pathName.LastIndexOf("/");
        string directory = pathName.Substring(0, pos);
        string assetBundleName = Path.GetFileName(pathName);

        AssetBundleBuild[] assetBundle = new AssetBundleBuild[1];
        assetBundle[0].assetBundleName = assetBundleName;
        assetBundle[0].assetNames = fileNames;

        BuildPipeline.BuildAssetBundles(directory, assetBundle, assetBundleOptions, targetPlatform);

        return true;
    }

    private static void DeleteDirectory(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            DirectoryInfo[] childs = dir.GetDirectories();
            foreach (DirectoryInfo child in childs)
            {
                child.Delete(true);
            }
            dir.Delete(true);
        }
    }
}