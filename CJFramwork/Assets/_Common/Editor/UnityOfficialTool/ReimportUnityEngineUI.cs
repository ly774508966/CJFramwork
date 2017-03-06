using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

public class ReimportUnityEngineUI
{
    [MenuItem("Assets/Reimport UI Assemblies", false, 100)]
    public static void ReimportUI()
    {
#if UNITY_4_6
        var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{0}/{1}";  
        var version = Regex.Match(Application.unityVersion, @"^[0-9]+\.[0-9]+\.[0-9]+").Value;  
#elif UNITY_4_7
        var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{0}/{1}";  
        var version = Regex.Match(Application.unityVersion, @"^[0-9]+\.[0-9]+\.[0-9]+").Value;  
#else
        var path = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/GUISystem/{1}";
        var version = string.Empty;
#endif
        string engineDll = string.Format(path, version, "UnityEngine.UI.dll");
        string editorDll = string.Format(path, version, "Editor/UnityEditor.UI.dll");
        ReimportDll(engineDll);
        ReimportDll(editorDll);

        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/Advertisements/Editor/UnityEditor.Advertisements.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/TreeEditor/Editor/UnityEditor.TreeEditor.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/EditorTestsRunner/Editor/UnityEditor.EditorTestsRunner.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/EditorTestsRunner/Editor/nunit.framework.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/Networking/Editor/UnityEditor.Networking.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/UnityPurchasing/UnityEngine.Purchasing.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/UnityAnalytics/UnityEngine.Analytics.dll");
        ReimportDll(EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/Networking/UnityEngine.Networking.dll");
    }
    static void ReimportDll(string path)
    {
        if (File.Exists(path))
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer);
        else
            Debug.LogError(string.Format("DLL not found {0}", path));
    }
}