using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
public class ysCommandLineBundler : Editor {



    public static void BuildAndorid()
    {
        Build(BuildTarget.Android);
    }
    public static void BuildIOS()
    {
        Build(BuildTarget.iOS);
    }

    const string OPT_PLATFORM = "-qmplatform";
    const string OPT_BUNDLE_VERSION = "-qmbundle_version";
    const string OPT_SHOW_VERSION = "-qmshow_version";
    const string OPT_VERSION_CODE = "-qmversion_code";
    /// <summary>
    /// 根据参数配置Unity ProjectSetting
    /// </summary>
    static void ProjectSetting()
    {
        Dictionary<string, string> settings = new Dictionary<string, string>();
        settings[OPT_PLATFORM] = "";
        settings[OPT_BUNDLE_VERSION] = "";
        settings[OPT_SHOW_VERSION] = "";
        settings[OPT_VERSION_CODE] = "";
        string[] args = System.Environment.GetCommandLineArgs();
        ParseArgs(settings, args);//解析参数

        PlayerSettings.companyName = "****";
        PlayerSettings.bundleVersion = settings[OPT_BUNDLE_VERSION];
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
        PlayerSettings.aotOptions = "nimt-trampolines=512,ntrampolines=2048";
        //PlayerSettings.targetGlesGraphics = TargetGlesGraphics.Automatic;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel9;
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.Android.forceSDCardPermission = true;
        PlayerSettings.Android.bundleVersionCode = int.Parse(settings[OPT_VERSION_CODE]);
        PlayerSettings.Android.useAPKExpansionFiles = true;//是否使用obb分离模式
        PlayerSettings.productName = "******";
        PlayerSettings.bundleIdentifier = "*****";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "宏定义");//宏定义的设置
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
    }

    static void ParseArgs(Dictionary<string, string> settings, string[] args)
    {
        foreach (var key in settings.Keys)
        {
            for (int i = 0; i < args.Length >> 1; i++)
            {
                if (settings.ContainsKey(args[i * 2 - 1]))
                {
                    settings[args[i * 2 - 1]] = args[i * 2];
                }
            }
        }

    }
    //[PostProcessBuild(90)]
    //public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    //{
    //    string[]  files = Directory.GetFiles(pathToBuiltProject, "*", SearchOption.AllDirectories);
    //    if (files != null)
    //    {
    //        foreach (string fileName in files)
    //        {
    //            if (fileName.Contains(".obb"))
    //            {
    //                int bundleVersion = PlayerSettings.Android.bundleVersionCode;
    //                string bundlePackageName = PlayerSettings.bundleIdentifier;
    //                Directory.Move(fileName, pathToBuiltProject + "/main." + bundleVersion + "." + bundlePackageName + ".obb");
    //            }
    //        }
    //    }
    //}
    static void Build(BuildTarget target)
    {
        string android_project_path = "C:/Users/xuchengjie/Desktop/HxH_test.apk";//目标目录
        target = BuildTarget.Android;
        string[] outScenes = GetBuildScenes();//需要打包的scene名字数组
        BuildPipeline.BuildPlayer(outScenes, android_project_path, target, BuildOptions.None);
    }
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
            {
                names.Add(e.path);
                Debug.Log(e.path);
            }
        }
        return names.ToArray();
    }
}
