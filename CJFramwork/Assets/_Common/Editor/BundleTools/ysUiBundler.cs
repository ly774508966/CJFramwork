using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class ysUiBundleData : ysEditorData
{
    public struct Paths
    {
        public string UiDirectoryPath;
        public string PrefabDirectoryPath;
        public string AtlasDirectoryPath;
        public string TextureDirectoryPath;
        public string FontDirectoryPath;
    }
    public Paths path;
    public string bundleName;
    public void GeneratePath()
    {
        bundleName = "ui_prefabs";
        path.UiDirectoryPath = FormatAssetPath2FullPath("Assets/Resources/UI");
        path.PrefabDirectoryPath = FormatAssetPath2FullPath("Assets/Resources/UI/Prefab");
        path.AtlasDirectoryPath = FormatAssetPath2FullPath("Assets/Resources/UI/Atlas");
        path.TextureDirectoryPath = FormatAssetPath2FullPath("Assets/Resources/UI/Texture");
        path.FontDirectoryPath = FormatAssetPath2FullPath("Assets/Resources/UI/Font");
        outputDirectoryPath = Application.dataPath + "/../../../design/build/android/ui/";
    }
    public void GenerateIgnore()
    {
        ignoreList.Add("MessagePanel");
        //ignoreList.Add("MessagePanel");
        //ignoreList.Add("MessagePanel");
        //ignoreList.Add("MessagePanel");
    }
    public List<GameObject> uiPrefabs = new List<GameObject>();
    public List<GameObject> atlasPrefabs = new List<GameObject>();
    public List<string> ignoreList = new List<string>();
}
public class ysUiBundleFunc : ysEditorFunc
{
    public ysUiBundleData data;
    public void InitData()
    {
        data = ScriptableObject.CreateInstance<ysUiBundleData>();
        data.GeneratePath();
        data.GenerateIgnore();
        data.uiPrefabs = ReadPrefabList(data.path.PrefabDirectoryPath);
    }
    public void ExportUiPrefab()
    {
        GameObject[] uiPrefabs = CheckIgnorePrefab(data.uiPrefabs,data.ignoreList).ToArray();
        GameObject[] uiClones = new GameObject[uiPrefabs.Length];
        for (int i = 0; i < uiClones.Length; i++)
        {
            uiClones[i] = DeepCopyAsset(uiPrefabs[i]);     
            RecordAssetInfo(uiPrefabs[i], uiClones[i]);
        }
        ExportPrefabs(uiClones, data.buildTarget,data.bundleName,data.outputDirectoryPath,true);
    }
    public void ExportAtlasPrefab()
    {
        List<GameObject> prefabList = ReadPrefabList(data.path.AtlasDirectoryPath);
        ExportPrefabsIndependent(prefabList, data.buildTarget, data.outputDirectoryPath);
    }
    public void ExportTexture()
    {
        List<Texture> textureList = ReadTextureList(data.path.TextureDirectoryPath);
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        string bundleName = "ui_textures";
        buildArray[0].assetBundleName = FormatOutputName(bundleName);
        string[] paths = new string[textureList.Count];
        for (int i = 0; i < textureList.Count; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(textureList[i]);
        }
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
        Debug.Log("All prefab exports succeed...");
    }
    public void ExportFont()
    {
        List<Font> fontList = ReadFontList(data.path.FontDirectoryPath);
        ExportPrefabs(fontList.ToArray(), data.buildTarget, "ui_fonts", data.outputDirectoryPath);
    }
    void RecordAssetInfo(GameObject prefab, GameObject clone)
    {
        //Assembly editorAssembly = Assembly.Load("Assembly-CSharp-Editor");
        Type recordFunc = Assembly.Load("Assembly-CSharp-Editor").GetType("ysAssetRecorderEditorFunc");
        recordFunc.GetMethod("RecordAssetInfo")
            //.MakeGenericMethod(tableClassType, dataClassType)
            .Invoke(Activator.CreateInstance(recordFunc,false), new object[] { prefab, clone });  
    }
    List<GameObject> CheckIgnorePrefab(List<GameObject> prefabList,List<string> ignoreList)
    {
        if (ignoreList!=null||ignoreList.Count>0)
        {
            for (int i = prefabList.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < ignoreList.Count; j++)
                {
                    if (prefabList[i].name.Contains(ignoreList[j]))
                    {
                        prefabList.Remove(prefabList[i]);
                    }
                }
            }
        }
        return prefabList;
    }

    public struct OverSizeInfo
    {
        public Texture texture;
        public float fileMegaBytes;
        public float compressed4AndoridMegaBytes;
        public float compressed4IosMegaBytes;
    }
    public struct AtlasFormatInfo
    {

    }

    public List<OverSizeInfo> overSizeInfo = new List<OverSizeInfo>();
    public List<OverSizeInfo> CheckTextureOverMemoSize(float megaBytes)
    {
        List<OverSizeInfo> overSizeList = new List<OverSizeInfo>();
        System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(data.path.TextureDirectoryPath);
        System.IO.FileInfo[] allFiles = directoryInfo.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
        for (int i = 0; i < allFiles.Length; i++)
        {
            if ((allFiles[i].Name.Contains(".png") || allFiles[i].Name.Contains(".jpg")) && !allFiles[i].Name.Contains(".meta"))
            {
                if (allFiles[i].Length> megaBytes * 1024*1024)
                {
                    string prefabPath = "Assets" + allFiles[i].FullName.Split(new string[] { "\\Assets" }, 0)[1];
                    Texture Texture = AssetDatabase.LoadAssetAtPath<Texture>(prefabPath);
                    TextureImporter m = AssetImporter.GetAtPath(prefabPath) as TextureImporter;
              
                    int size = Texture.height * Texture.width * 4;
                    if (size > 1*1024*1024)
                    {
                        float compressSize = ((float)size) / 1024f / 1024f;
                        switch (m.textureFormat)
                        {
                            case TextureImporterFormat.ETC2_RGB4:
                                compressSize *= 0.125f;
                                break;
                            default:
                                break;
                        }
                        int maxSize = 1024;
                        TextureImporterFormat format = new TextureImporterFormat();
                        m.GetPlatformTextureSettings("Android", out maxSize,out format);
                        switch (format)
                        {
                            #region Formasts

                            case TextureImporterFormat.AutomaticCompressed:
                                break;
                            case TextureImporterFormat.Automatic16bit:
                                break;
                            case TextureImporterFormat.AutomaticTruecolor:
                                break;
                            case TextureImporterFormat.AutomaticCrunched:
                                break;
                            case TextureImporterFormat.DXT1:
                                break;
                            case TextureImporterFormat.DXT5:
                                break;
                            case TextureImporterFormat.RGB16:
                                break;
                            case TextureImporterFormat.RGB24:
                                break;
                            case TextureImporterFormat.Alpha8:
                                break;
                            case TextureImporterFormat.ARGB16:
                                break;
                            case TextureImporterFormat.RGBA32:
                                break;
                            case TextureImporterFormat.ARGB32:
                                break;
                            case TextureImporterFormat.RGBA16:
                                break;
                            case TextureImporterFormat.DXT1Crunched:
                                break;
                            case TextureImporterFormat.DXT5Crunched:
                                break;
                            case TextureImporterFormat.PVRTC_RGB2:
                                break;
                            case TextureImporterFormat.PVRTC_RGBA2:
                                break;
                            case TextureImporterFormat.PVRTC_RGB4:
                                break;
                            case TextureImporterFormat.PVRTC_RGBA4:
                                break;
                            case TextureImporterFormat.ETC_RGB4:
                                break;
                            case TextureImporterFormat.ATC_RGB4:
                                break;
                            case TextureImporterFormat.ATC_RGBA8:
                                break;
                            case TextureImporterFormat.EAC_R:
                                break;
                            case TextureImporterFormat.EAC_R_SIGNED:
                                break;
                            case TextureImporterFormat.EAC_RG:
                                break;
                            case TextureImporterFormat.EAC_RG_SIGNED:
                                break;
                            case TextureImporterFormat.ETC2_RGB4:
                                compressSize *= 0.125f;
                                break;
                            case TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA:
                                break;
                            case TextureImporterFormat.ETC2_RGBA8:
                                break;
                            case TextureImporterFormat.ASTC_RGB_4x4:
                                break;
                            case TextureImporterFormat.ASTC_RGB_5x5:
                                break;
                            case TextureImporterFormat.ASTC_RGB_6x6:
                                break;
                            case TextureImporterFormat.ASTC_RGB_8x8:
                                break;
                            case TextureImporterFormat.ASTC_RGB_10x10:
                                break;
                            case TextureImporterFormat.ASTC_RGB_12x12:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_4x4:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_5x5:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_6x6:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_8x8:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_10x10:
                                break;
                            case TextureImporterFormat.ASTC_RGBA_12x12:
                                break;
                            default:
                                break;
#endregion
                        }
                        //Debug.LogError(prefabPath +"　文件大小："+(((float)allFiles[i].Length)/1024f/1024f).ToString("0.00")+" MB, 压缩大小:"+compressSize.ToString("0.00")+"MB");
                        OverSizeInfo info = new OverSizeInfo();
                        info.texture = Texture;
                        info.fileMegaBytes = ((float)allFiles[i].Length)/1024f/1024f;
                        info.compressed4AndoridMegaBytes = compressSize;
                        overSizeList.Add(info);
                    }                 
                }
            }
        }
        return overSizeList;
    }

    public static void BatchModeExportAssets()
    {
        ysUiBundleFunc editorFunc = new ysUiBundleFunc();
        editorFunc.InitData();
        editorFunc.ExportUiPrefab();
        editorFunc.ExportAtlasPrefab();
        editorFunc.ExportTexture();
        editorFunc.ExportFont();
    }
}
public class ysUiBundleWindow : ysEditorWindow
{
      
    static ysUiBundleFunc func = new ysUiBundleFunc();
    [MenuItem("Tools/UI导出 &U", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.client);
        return canShowWindow;
    }
    [MenuItem("Tools/UI导出 &U")]
    public static void ShowEditorWindow()
    {
        InitData();
        window = EditorWindow.GetWindow(typeof(ysUiBundleWindow), true);
    }
    static void InitData()
    {
        func.InitData();
    }
    void OnGUI()
    {
        UseAltAndKeyBoardToCloseWindow(KeyCode.U);
        DrawButton("导出UI预制体", ()=> ExportUiPrifab());
        DrawButton("导出Atlas图集", () => ExportAtlasPrefab());
        //DrawButton("设置Atlas图集", () => SetAtlasFormat());
        DrawButton("导出Texture图片", () => ExportTextures());
        DrawButton("导出Font字体", () => ExportFonts());
        DrawButton("查询大尺寸图片", () => CheckOverSizedTexture());
        DrawScrollView(() => ShowOverSizeList(),0);
        DrawButton("清空", () => ClearOverSizedTextureList());
    }
    void ExportUiPrifab()
    {
        func.ExportUiPrefab();
    }
    void ExportAtlasPrefab()
    {
        func.ExportAtlasPrefab();
    }
    void ExportTextures()
    {
        func.ExportTexture();
    }
    void ExportFonts()
    {
        func.ExportFont();
    }
    void CheckOverSizedTexture()
    {
        func.overSizeInfo = func.CheckTextureOverMemoSize(0.5f);
    }
    void ClearOverSizedTextureList()
    {
        func.overSizeInfo.Clear();
    }
    void ShowOverSizeList()
    {
        for (int i = 0; i < func.overSizeInfo.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(func.overSizeInfo[i].texture, typeof(Texture), true, GUILayout.Width(150));
            if (GUILayout.Button("选择", GUILayout.Width(40)))
            {
                Selection.activeObject = func.overSizeInfo[i].texture ;
            }
            EditorGUILayout.LabelField("原图大小:", GUILayout.Width(60));
            EditorGUILayout.LabelField(func.overSizeInfo[i].fileMegaBytes.ToString("0.00")+"MB", GUILayout.Width(60));
            EditorGUILayout.LabelField("压缩纹理:", GUILayout.Width(60));
            EditorGUILayout.LabelField(func.overSizeInfo[i].compressed4AndoridMegaBytes.ToString("0.00")+"MB", GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();
        }
    }

    //[MenuItem("Tools2/设置Atlas格式", true)]
    //private static bool CheckCanUseSetAtlasFormat()
    //{
    //    return canShowWindow;
    //}
    [MenuItem("Tools/设置Atlas格式")]
    public static void SetAtlasFormat()
    {
        ysUiBundleFunc func = new ysUiBundleFunc();
        func.InitData();

        List<Texture> prefabList = func.ReadTextureList(func.data.path.AtlasDirectoryPath);
        for (int i = 0; i < prefabList.Count; i++)
        {
            TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(prefabList[i])) as TextureImporter;
            if (importer.textureFormat!= TextureImporterFormat.ETC2_RGBA8)
            {
                importer.SetPlatformTextureSettings("Android", 4096, TextureImporterFormat.ETC2_RGBA8, 100, false);
                importer.SaveAndReimport();
            }
        }

    }
}