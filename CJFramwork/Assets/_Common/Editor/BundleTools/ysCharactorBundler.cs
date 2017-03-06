using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;
using System;

public class ysCharactorBundleData : ysEditorData
{
    public enum ListType
    {
        hero,
        monster,
        collection,
        npc,
        role,
    }
    public enum ErrorType
    {
        shaderError,
        textureFormatError,
        missingCityMode,
        missingAnimationController,
        clipNotLoop,
        positionLockXz,
        modelImportError,

    }
    public struct ErrorInfo
    {
        public UnityEngine.Object errorSrc;
        public string errorMsg;
        public ModelInfo modelInfo;
        public ErrorType errorType;
    }
    public class ModelInfo:ysISearchble
    {
        public GameObject modelPrefab;
        public AnimatorOverrideController modelAnimator;
        public Material modelMatirial;
        public Texture2D modelTexture;
        public GameObject modelPrifabInCity;
        public AnimatorOverrideController modelPrifabInCityAnimator;
        public string modelAnimatorPath;
        public AnimatorController baseController;
        public bool hasCityMode;

        string ysISearchble.GetSearchString()
        {
            return modelPrefab.name.Split('_')[2];
        }
    }
    public class RoleInfo : ysISearchble
    {
        public ModelInfo roleModel;
        public GameObject[] cosModels;
        public GameObject[] weapenModels;

        string ysISearchble.GetSearchString()
        {
            return roleModel.modelPrefab.name.Split('_')[2];
        }
    }


    public class RepairInfo
    {
        public List<Shader> shaders = new List<Shader>();
        public Shader shaderDiffuse;
        public Shader shaderAlpha;
        public int textureSize;
        public bool isGenerateMipMap;
        public TextureImporterFormat textureFormatDiffuse;
        public TextureImporterFormat textureFormatAlpha;
        public WrapMode wrapMode;
        public FilterMode filterMode;
    }
    public struct Input
    {
        public string heroModelPath;
        public string monsterModelPath;
        public string collectionModelPath;
        public string npcModelPath;
        public string roleModelPath;
        public string shaderPath;

        public string baseAnimatorControllerAssetPath;
        public string npcAnimationControllerAssetPath;
        public string actorAnimationControllerAssetPath;
        public string collectAnimationControllerAssetPath;
    }
    public struct Output
    {
        public string bundleName;
    }
    public Input input;
    public RepairInfo repairInfo = new RepairInfo();
    public void UseDefaultData()
    {
        GeneratePath();
    }
    void GeneratePath()
    {
        input.heroModelPath = FormatAssetPath2FullPath("Assets/_Prefab/Hero");
        input.monsterModelPath = FormatAssetPath2FullPath("Assets/_Prefab/Monster");
        input.collectionModelPath = FormatAssetPath2FullPath("Assets/_Prefab/Collect");
        input.npcModelPath = FormatAssetPath2FullPath("Assets/_Prefab/Npc");
        input.roleModelPath = FormatAssetPath2FullPath("Assets/_Prefab/Role");
        input.shaderPath = FormatAssetPath2FullPath("Assets/Resources/Shader");
        input.baseAnimatorControllerAssetPath = "Assets/Resources/Model";
        input.npcAnimationControllerAssetPath = "Assets/Resources/Model/Npc.controller";
        input.actorAnimationControllerAssetPath = "Assets/Resources/Model/Actor.controller";
        input.collectAnimationControllerAssetPath = "Assets/Resources/Model/Collect.controller";
        outputDirectoryPath = Application.dataPath + "/../../../design/build/android/model/";
    }

}
public class ysCharactorBundleFunc : ysEditorFunc
{
    public ysCharactorBundleData editorData;

    string[] ignorePrefabNameStings = new string[] { "Role", "_city","test"};
    string[] loopClipNameStings = new string[] { "breath", "run", "coma" };
    string[] xzSettingString = new string[] { "damage1"};
    string[] textureWithAlphaStrings = new string[] { "_al" };
    string[] shaderWithAlphaStrings = new string[] { "cutout",};
    string[] shaderUsedName = new string[] {"character","hxh_role","hxh_role_cutout"};
    string shaderDiffuse = "hxh/role";
    string shaderAlpha = "hxh/role cutout";
    public void LoadData()
    {
        editorData = ScriptableObject.CreateInstance<ysCharactorBundleData>();
        editorData.UseDefaultData();
        GenerateRepairInfo(editorData);
    }
    void GenerateRepairInfo(ysCharactorBundleData editorData)
    {
        editorData.repairInfo.shaders = ReadShaderList(editorData.input.shaderPath);
        for (int i = 0; i < editorData.repairInfo.shaders.Count; i++)
        {
            if (editorData.repairInfo.shaders[i].name == shaderDiffuse)
            {
                editorData.repairInfo.shaderDiffuse = editorData.repairInfo.shaders[i];
            }
            if (editorData.repairInfo.shaders[i].name == shaderAlpha)
            {
                editorData.repairInfo.shaderAlpha = editorData.repairInfo.shaders[i];
            }
        }
        editorData.repairInfo.textureSize = 512;
        editorData.repairInfo.isGenerateMipMap = false;
        editorData.repairInfo.wrapMode = WrapMode.Default;
        editorData.repairInfo.filterMode = FilterMode.Bilinear;
        switch (editorData.buildTarget)
        {
            case ysEditorData.Platform.android:
                editorData.repairInfo.textureFormatDiffuse = TextureImporterFormat.ETC2_RGB4;
                editorData.repairInfo.textureFormatAlpha = TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA;
                break;
            case ysEditorData.Platform.ios:
                editorData.repairInfo.textureFormatDiffuse = TextureImporterFormat.PVRTC_RGB4;
                editorData.repairInfo.textureFormatAlpha = TextureImporterFormat.PVRTC_RGBA4;
                break;
            default:
                break;
        }
    }
    public List<ysCharactorBundleData.ModelInfo> ReadModelInfoList(ysCharactorBundleData.ListType listType)
    {
        List<ysCharactorBundleData.ModelInfo> modelInfoList = new List<ysCharactorBundleData.ModelInfo>();
        string loadPath = loadPath = editorData.input.heroModelPath;
        string directoryName = "Hero";
        bool hasCityMode = false;
        AnimatorController baseController = new AnimatorController();
        switch (listType)
        {
            case ysCharactorBundleData.ListType.hero:
                loadPath = editorData.input.heroModelPath;
                directoryName = "Hero";
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);
                hasCityMode = true;
                break;
            case ysCharactorBundleData.ListType.monster:
                loadPath = editorData.input.monsterModelPath;
                directoryName = "Monster";
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);
                hasCityMode = true;
                break;
            case ysCharactorBundleData.ListType.collection:
                loadPath = editorData.input.collectionModelPath;
                directoryName = "Collect";
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.collectAnimationControllerAssetPath);
                break;
            case ysCharactorBundleData.ListType.npc:
                loadPath = editorData.input.npcModelPath;
                directoryName = "Npc";
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);
                hasCityMode = true;
                break;
            //case ysCharactorBundleData.ListType.role:
            //    loadPath = editorData.input.roleModelPath;
            //    directoryName = "Role";
            //    baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);
            //    hasCityMode = true;
            //    break;
            default:
                loadPath = editorData.input.heroModelPath;
                directoryName = "Hero";
                baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);
                break;
        }
        List<GameObject> prefabList = ReadPrefabList(loadPath);
        for (int i = 0; i < prefabList.Count; i++)
        {
            if (!CheckStringMatch(prefabList[i].name,ignorePrefabNameStings))
            {
                ysCharactorBundleData.ModelInfo modelInfo = new ysCharactorBundleData.ModelInfo();
                modelInfo.modelPrefab = prefabList[i];
                string prefabPath = AssetDatabase.GetAssetPath(prefabList[i]);
                string prefabCityPath = prefabPath.Replace(".prefab", "_city.prefab");
                string prefabAnimatorPath = "Assets/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/" + prefabList[i].name + ".overrideController";
                string prefabCityAnimatorPath = "Assets/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/" + prefabList[i].name + "_city.overrideController";
                string prefabMatDirectory = Application.dataPath + "/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/Materials/";
                FileInfo[] mats = new DirectoryInfo(prefabMatDirectory).GetFiles("*.mat");
                if (mats != null && mats.Length > 0)
                {
                    FileInfo mat = mats[0];
                    string prefabMatirialPath = "Assets/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/Materials/" + mat.Name;
                    modelInfo.modelMatirial = AssetDatabase.LoadAssetAtPath<Material>(prefabMatirialPath);
                }
                string prefabTexDirectory = Application.dataPath + "/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/";
                FileInfo[] texs = new DirectoryInfo(prefabTexDirectory).GetFiles("*.tga");
                if (texs != null && texs.Length > 0)
                {
                    FileInfo tex = texs[0];
                    string prefabTexturePath = "Assets/Resources/Model/" + directoryName + "/" + prefabList[i].name + "/" + tex.Name;
                    modelInfo.modelTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(prefabTexturePath);
                }
                if (hasCityMode)
                {
                    modelInfo.hasCityMode = true;
                    modelInfo.modelPrifabInCity = AssetDatabase.LoadAssetAtPath<GameObject>(prefabCityPath);
                    modelInfo.modelPrifabInCityAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(prefabCityAnimatorPath);
                }
                modelInfo.modelAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(prefabAnimatorPath);
                modelInfo.modelAnimatorPath = prefabAnimatorPath;
                modelInfo.baseController = baseController;
                modelInfoList.Add(modelInfo);
            }
        }

        return modelInfoList;
    }
    public List<ysCharactorBundleData.RoleInfo> ReadRoleInfoList()
    {
        List<ysCharactorBundleData.ModelInfo> modelInfoList = new List<ysCharactorBundleData.ModelInfo>();
        string loadPath  = editorData.input.roleModelPath;
        string directoryName = "Hero";
        bool hasCityMode = false;
        AnimatorController baseController = new AnimatorController();
        directoryName = "Role";
        baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(editorData.input.actorAnimationControllerAssetPath);

       List<ysCharactorBundleData.RoleInfo> rolePrefabInfoList = new List<ysCharactorBundleData.RoleInfo>();

        DirectoryInfo directoryInfo = new DirectoryInfo(loadPath);
        DirectoryInfo[] roleDirectorys = directoryInfo.GetDirectories();
        for (int i = 0; i < roleDirectorys.Length; i++)
        {
            ysCharactorBundleData.RoleInfo roleInfo = new ysCharactorBundleData.RoleInfo();
            string roleDirectoryName = roleDirectorys[i].Name;
            string rolePrefabPath = editorData.FormatFullPath2AssetPath(loadPath)+ roleDirectoryName+"/"+ roleDirectoryName+".prefab";
            GameObject rolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rolePrefabPath);
            List<GameObject> roleCosPrefabList = new List<GameObject>();
            GameObject roleCosPrefab;
            int cosCount = 0;
            do 
            {
                cosCount++;
                rolePrefabPath = editorData.FormatFullPath2AssetPath(loadPath) +  roleDirectoryName + "/" + roleDirectoryName+"_"+ cosCount.ToString("000") + ".prefab";
                roleCosPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rolePrefabPath);
                if (roleCosPrefab!=null)
                {
                    roleCosPrefabList.Add(roleCosPrefab);
                }
            } while (roleCosPrefab!=null);

            cosCount = 0;
            List<GameObject> weaponPrefabList = new List<GameObject>();
            GameObject weaponPrefab;
            do
            {
                cosCount++;
                rolePrefabPath = editorData.FormatFullPath2AssetPath(loadPath) + roleDirectoryName + "/" + roleDirectoryName + "_we_" + cosCount.ToString("000") + ".prefab";
                weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(rolePrefabPath);
                if (weaponPrefab != null)
                {
                    weaponPrefabList.Add(weaponPrefab);
                }
            } while (weaponPrefab != null);

            ysCharactorBundleData.ModelInfo rolemodel = new ysCharactorBundleData.ModelInfo();
            rolemodel.modelPrefab = rolePrefab;
            //
            //
            roleInfo.roleModel = rolemodel;
            roleInfo.cosModels = roleCosPrefabList.ToArray();
            roleInfo.weapenModels = weaponPrefabList.ToArray();
            rolePrefabInfoList.Add(roleInfo);
        }
       

        return rolePrefabInfoList;
    }
    public bool CheckHasCityModel(ysCharactorBundleData.ModelInfo modelInfo,bool tryRepair = false)
    {
        bool isRet = true;
        if (modelInfo.hasCityMode&& modelInfo.modelPrifabInCity == null)
        {
            isRet &= false;
            if (tryRepair)
            {
                TryRepairCityModel(modelInfo);
            }
            else
            {
                string errorInfoMsg = modelInfo.modelPrefab.name + "缺少city模型";
                CollectErrorInfo(ysCharactorBundleData.ErrorType.missingCityMode, modelInfo.modelPrefab, errorInfoMsg, modelInfo);
            }
        }
        return isRet;
    }
    public void TryRepairCityModel(ysCharactorBundleData.ModelInfo modelInfo)
    {
        string pathx00 = AssetDatabase.GetAssetPath(modelInfo.modelPrefab);
        string pathx01 = pathx00.Replace(".prefab", "_city.prefab");
        AssetDatabase.CopyAsset(pathx00, pathx01);
        string pathx02 = AssetDatabase.GetAssetPath(modelInfo.modelAnimator);
        string pathx03 = pathx02.Replace(".overrideController", "_city.overrideController");
        AnimatorOverrideController controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(pathx03);
        if (controller==null)
        {
            AssetDatabase.CopyAsset(pathx02, pathx03);
            controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(pathx03);
            controller.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(editorData.input.npcAnimationControllerAssetPath);
            EditorUtility.SetDirty(controller);
        }
        UnityEditor.Animations.AnimatorController baseController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(editorData.input.npcAnimationControllerAssetPath);
        GameObject cityModel = AssetDatabase.LoadAssetAtPath<GameObject>(pathx01);
        cityModel.GetComponent<Animator>().runtimeAnimatorController = controller;
       
        MatchAnimationClips(controller, baseController);
        EditorUtility.SetDirty(cityModel);
        AssetDatabase.SaveAssets();
        modelInfo.modelPrifabInCity = AssetDatabase.LoadAssetAtPath<GameObject>(pathx01);
    }
    public void MatchAnimationClips(AnimatorOverrideController overrideController, UnityEditor.Animations.AnimatorController baseController)
    {
        string overridePath = AssetDatabase.GetAssetPath(overrideController);
        string clipPath = Path.GetDirectoryName(editorData.FormatAssetPath2FullPath(overridePath).TrimEnd(Path.AltDirectorySeparatorChar));
        string[] clipNames = TraverseFbxAniationClip(clipPath);
        string[] originClipNames = new string[baseController.animationClips.Length];
        string[] sortedClipNames = new string[baseController.animationClips.Length];
        for (int i = 0; i < originClipNames.Length; i++)
        {
            originClipNames[i] = overrideController.clips[i].originalClip.name;
        }
        sortedClipNames = MatchAnimatorClipString(originClipNames, clipNames);
        for (int i = 0; i < overrideController.animationClips.Length; i++)
        {
            AnimationClip newClip = AssetDatabase.LoadAssetAtPath(Path.GetDirectoryName(overridePath) + Path.AltDirectorySeparatorChar + sortedClipNames[i], typeof(AnimationClip)) as AnimationClip;
            if (newClip != null)
            {
                overrideController[overrideController.clips[i].overrideClip.name] = newClip;
            }
            else
            {
                if (overrideController.clips[i].originalClip!=null)
                {
                overrideController[overrideController.clips[i].overrideClip.name] = overrideController.clips[i].originalClip;
                }
            }
        }
    }
    string[] MatchAnimatorClipString(string[] originClipNames, string[] clipNames)
    {
        string[] sortedClipNames = new string[originClipNames.Length];
        for (int i = 0; i < originClipNames.Length; i++)
        {
            string tempx00 = originClipNames[i].ToLower().TrimStart("none_".ToCharArray());

            for (int j = 0; j < clipNames.Length; j++)
            {
                string tempx01 = clipNames[j].Split('@', '.')[1].ToLower();
                bool isContain = tempx00.Contains(tempx01) || tempx00.Contains(tempx01.Replace('s', 'c'));
                if (isContain)
                {
                    sortedClipNames[i] = clipNames[j];
                    clipNames[j] = "a@matched.fbx";
                    break;
                }
            }
        }
        return sortedClipNames;
    }
    string[] TraverseFbxAniationClip(string modelFilePath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(modelFilePath);
        FileInfo[] allFiles = directoryInfo.GetFiles();
        List<string> clipFiles = new List<string>();
        for (int i = 0; i < allFiles.Length; i++)
        {
            if (allFiles[i].Name.Contains("@") && !allFiles[i].Name.Contains(".meta"))
            {
                clipFiles.Add(allFiles[i].Name);
            }
        }
        return clipFiles.ToArray();
    }
    public bool CheckHasAnimatorController(ysCharactorBundleData.ModelInfo modelInfo,bool tryRepair =false)
    {
        bool isRet = true;
        if (modelInfo.modelAnimator==null)
        {
            if (tryRepair)
            {
                TryCreateAnimator(modelInfo);
                isRet &= true;
            }
            else
            {
                if (EditorUtility.DisplayDialog("警告", "动作控制器不存在！", "尝试生成", " 取消"))
                {
                    TryCreateAnimator(modelInfo);
                    isRet &= true;
                }
                else
                {
                    isRet &= false;
                }
            }
            return isRet;
        }


        if (modelInfo.modelPrefab.GetComponent<Animator>().runtimeAnimatorController == null)
        {
            isRet = false;
            string errorInfoMsg = modelInfo.modelPrefab.name + "丢失AnimatorController";
            CollectErrorInfo(ysCharactorBundleData.ErrorType.missingAnimationController,modelInfo.modelPrefab, errorInfoMsg, modelInfo);
            if (tryRepair )
            {
                TryRepairAnimatorController(modelInfo);
            }
        }
        return isRet;
    }
    void TryRepairAnimatorController(ysCharactorBundleData.ModelInfo modelInfo)
    {
        if (modelInfo.modelAnimator==null)
        {
            modelInfo.modelAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(modelInfo.modelAnimatorPath);
            if (modelInfo.modelAnimator==null)
            {
                Debug.LogError(modelInfo.modelPrefab.name +"动画文件不存在，请重新生成...");
                if (EditorUtility.DisplayDialog("警告", "动作控制器不存在！", "尝试生成", " 取消"))
                {
                    TryCreateAnimator(modelInfo);
                }
            }
        }
        else
        {
            modelInfo.modelPrefab.GetComponent<Animator>().runtimeAnimatorController = modelInfo.modelAnimator;
            EditorUtility.SetDirty(modelInfo.modelPrefab);
            AssetDatabase.SaveAssets();
        }
    }
    public bool CheckLoopedAnimationClip(ysCharactorBundleData.ModelInfo modelInfo, bool tryRepair = false)
    {
        bool isRet = true;
        AnimationClip[] clips = modelInfo.modelAnimator.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (CheckStringMatch(clips[i].name,loopClipNameStings))
            {
                if (clips[i].isLooping == false)
                {
                  
                    if (tryRepair)
                    {
                        TryRepairLoopClip(clips[i]);
                    }
                    else
                    {
                        string errorInfoMsg = modelInfo.modelPrefab.name + "的" + clips[i].name + "动作没有设置循环";
                        CollectErrorInfo(ysCharactorBundleData.ErrorType.clipNotLoop, clips[i], errorInfoMsg, modelInfo);
                    }
                }
            }
        }
        return isRet;
    }
    void TryRepairLoopClip(AnimationClip clip)
    {
        ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;
        importer.defaultClipAnimations[0].loop = true;
        importer.SaveAndReimport();
    }
    public bool CheckAnimationClipXzSet(ysCharactorBundleData.ModelInfo modelInfo, bool tryRepair = false)
    {
        bool isRet = true;
        AnimationClip[] clips = modelInfo.modelAnimator.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (CheckStringMatch(clips[i].name, xzSettingString))
            {
                ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clips[i])) as ModelImporter;

                if (importer.clipAnimations[0].lockRootPositionXZ == false)
                {
                    isRet = false;
                    if (tryRepair)
                    {
                        TryRepairXzLock(clips[i]);
                    }
                    else
                    {
                        string errorInfoMsg = modelInfo.modelPrefab.name + "的" + clips[i].name + "动作设置XZ选项";
                        CollectErrorInfo(ysCharactorBundleData.ErrorType.positionLockXz, importer, errorInfoMsg, modelInfo);
                    }

                }
            }
        }
        return isRet;
    }
    public void TryRepairXzLock(AnimationClip clip)
    {
        ModelImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;
        ModelImporterClipAnimation[] defaultclips = importer.defaultClipAnimations;
        ModelImporterClipAnimation[] clips = new ModelImporterClipAnimation[defaultclips.Length];
        importer.animationType = ModelImporterAnimationType.Generic;
        for (int i = 0; i < defaultclips.Length; i++)
        {
            clips[i] = defaultclips[i];
            clips[i].lockRootHeightY = true;
            clips[i].lockRootRotation = true;
            clips[i].lockRootPositionXZ = true;
            clips[i].keepOriginalPositionY = true;
            clips[i].keepOriginalOrientation = true;
            clips[i].keepOriginalPositionXZ = true;
        }
        importer.clipAnimations = clips;
        importer.SaveAndReimport();
    }
    public bool CheckModelImportSetting(ysCharactorBundleData.ModelInfo modelInfo, bool tryRepair = false)
    {
        bool isRet = true;

        //string pa = Path.GetDirectoryName(AssetDatabase.GetAssetPath(modelInfo.modelAnimator));
        //string p2 = modelInfo.modelPrefab.name + ".FBX";
        string fbxPath =Path.Combine( Path.GetDirectoryName(modelInfo.modelAnimatorPath), modelInfo.modelPrefab.name+ ".FBX");
        ModelImporter importer = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (importer.animationType == ModelImporterAnimationType.Generic)// && importer.motionNodeName == "Bone_root01")
        {
        }
        else
        {
            if (tryRepair)
            {
                EditorUtility.DisplayDialog("警告", "需要手动设置模型导入属性，并手动更新所有子动作模型的Avatar！", "继续");
            }
            isRet &= false;
            string errorInfoMsg = modelInfo.modelPrefab.name + "的模型没有设置为Generec或者根骨设置为"+ importer.motionNodeName;
            CollectErrorInfo(ysCharactorBundleData.ErrorType.modelImportError, importer, errorInfoMsg, modelInfo);
        }
        return isRet;
    }
    void TryCreateAnimator(ysCharactorBundleData.ModelInfo modelInfo)
    {
        AnimatorOverrideController baseController = new AnimatorOverrideController();
        baseController.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(editorData.input.actorAnimationControllerAssetPath);
        AssetDatabase.CreateAsset(baseController, modelInfo.modelAnimatorPath);
        baseController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(modelInfo.modelAnimatorPath);
        modelInfo.modelPrefab.GetComponent<Animator>().runtimeAnimatorController = baseController;


        EditorUtility.SetDirty(modelInfo.modelPrefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public bool CheckShaderSet(ysCharactorBundleData.ModelInfo modelInfo, bool tryRepair = false)
    {
        bool tempx00 = CheckStringMatch(modelInfo.modelTexture.name, textureWithAlphaStrings);
        bool tempx01 = CheckStringMatch(modelInfo.modelMatirial.shader.name, shaderWithAlphaStrings);
        if (tempx00 ^ tempx01)
        {

            if (tryRepair)
            {
                TryRepairShader(modelInfo);
            }
            else
            {
                string errorInfoMsg = modelInfo.modelPrefab.name + "的Shader设置错误";
                CollectErrorInfo(ysCharactorBundleData.ErrorType.shaderError, modelInfo.modelMatirial, errorInfoMsg, modelInfo);
            }
        }
        return !tempx00 ^ tempx01;
    }
    void TryRepairShader(ysCharactorBundleData.ModelInfo modelInfo)
    {
        if (CheckStringMatch(modelInfo.modelTexture.name, textureWithAlphaStrings))
        {
            modelInfo.modelMatirial.shader = editorData.repairInfo.shaderAlpha;
        }
        else
        {
            modelInfo.modelMatirial.shader = editorData.repairInfo.shaderDiffuse;
        }
        EditorUtility.SetDirty(modelInfo.modelMatirial);
        AssetDatabase.SaveAssets();
    }
    public bool CheckTextureFormat(ysCharactorBundleData.ModelInfo modelInfo, bool tryRepair = false)
    {
        bool isRet = true;
        string errorString = "";
        if (modelInfo.modelTexture.height>editorData.repairInfo.textureSize|| modelInfo.modelTexture.width> editorData.repairInfo.textureSize)
        {
            isRet &= false;
            string errorInfox00 = modelInfo.modelPrefab.name.ToString() + "的贴图格式设置错误：没有设置成"+ editorData.repairInfo.textureSize + "x"+ editorData.repairInfo.textureSize + "。";
            errorString += errorInfox00;
            //CollectErrorInfo(ysCharactorBundleData.ErrorType.textureFormatError, modelInfo.modelTexture, errorInfox00, modelInfo);
        }
        if (modelInfo.modelTexture.mipmapCount>1)
        {
            isRet &= false;
            string errorInfox01 = modelInfo.modelPrefab.name.ToString() + "的贴图格式设置错误:存在mipmap。";
            errorString += errorInfox01;
            //CollectErrorInfo(ysCharactorBundleData.ErrorType.textureFormatError, modelInfo.modelTexture, errorInfox01, modelInfo);
        }
        //贴图带alpha通道时
        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(modelInfo.modelTexture)) as TextureImporter;
        if (CheckStringMatch(modelInfo.modelTexture.name, textureWithAlphaStrings))
        {

            if (importer.textureFormat != editorData.repairInfo.textureFormatAlpha)//&& modelInfo.modelTexture.format != TextureFormat.PVRTC_RGBA2)
            {
                isRet &= false;
                string errorInfox02 = modelInfo.modelPrefab.name + "的贴图格式设置错误：不应该为" + modelInfo.modelTexture.format+"。";
                errorString += errorInfox02;
                //CollectErrorInfo(ysCharactorBundleData.ErrorType.textureFormatError, modelInfo.modelTexture, errorInfox02, modelInfo);
            }
        }
        else
        {
            if (importer.textureFormat != editorData.repairInfo.textureFormatDiffuse )//&& modelInfo.modelTexture.format != TextureFormat.PVRTC_RGB2)
            {
                isRet &= false;
                string errorInfox03 = modelInfo.modelPrefab.name + "的贴图格式设置错误：不应该为" + modelInfo.modelTexture.format + "。";
                errorString += errorInfox03;
                //CollectErrorInfo(ysCharactorBundleData.ErrorType.textureFormatError, modelInfo.modelTexture, errorInfox03, modelInfo);
            }
        }
        if (!isRet)
        {
            if (tryRepair)
            {
                TryRepairTextureFormat(modelInfo, CheckStringMatch(modelInfo.modelTexture.name, textureWithAlphaStrings));
            }
            else
            {
                CollectErrorInfo(ysCharactorBundleData.ErrorType.textureFormatError, modelInfo.modelTexture, errorString, modelInfo);
            }
        }
        return isRet;
    }

    void TryRepairTextureFormat(ysCharactorBundleData.ModelInfo modelInfo,bool isWithAlpha)
    {
        TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(modelInfo.modelTexture)) as TextureImporter;
        TextureImporterFormat format;
        if (isWithAlpha)
        {
            importer.textureFormat = editorData.repairInfo.textureFormatAlpha;
            format = editorData.repairInfo.textureFormatAlpha;
        }
        else
        {
            importer.textureFormat = editorData.repairInfo.textureFormatDiffuse;
            format = editorData.repairInfo.textureFormatDiffuse;
        }
        string platForm = "";
        switch (editorData.buildTarget)
        {
            case ysEditorData.Platform.android:
                platForm = "Android";
                break;
            case ysEditorData.Platform.ios:
                platForm = "IOS";
                break;
            default:
                break;
        }
        importer.mipmapEnabled = false;
        importer.SetPlatformTextureSettings(platForm, editorData.repairInfo.textureSize, format);
        importer.SaveAndReimport();
    }

    public List<ysCharactorBundleData.ErrorInfo> errorInfoList = new List<ysCharactorBundleData.ErrorInfo>();
    public void CollectErrorInfo(ysCharactorBundleData.ErrorType errorType,UnityEngine.Object src,string msg,ysCharactorBundleData.ModelInfo modelInfo)
    {
        ysCharactorBundleData.ErrorInfo errorInfo = new ysCharactorBundleData.ErrorInfo();
        errorInfo.errorType = errorType;
        errorInfo.errorSrc = src;
        errorInfo.errorMsg = msg;
        errorInfo.modelInfo = modelInfo;
        errorInfoList.Add(errorInfo);
    }
    public void ExportCharacters(List<ysCharactorBundleData.ModelInfo> modelInfoList)
    {
        if (!EditorUtility.DisplayDialog("警告", "处理资源需要较长时间，是否继续？", "继续", "取消")) return;
        AssetBundleBuild[] buildArray = new AssetBundleBuild[modelInfoList.Count];
        for (int i = 0; i < modelInfoList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatOutputName(modelInfoList[i].modelPrefab.name);
            string pathx00 = AssetDatabase.GetAssetPath(modelInfoList[i].modelPrefab);
            string pathx01 = AssetDatabase.GetAssetPath(modelInfoList[i].modelPrifabInCity);
            string[] paths = new string[] { pathx00, pathx01 };
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
        BuildPipeline.BuildAssetBundles(
        editorData.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
        Debug.Log("导出成功，共导出" + modelInfoList.Count + "个文件，"+ modelInfoList.Count + "个资源包，资源路径：" + editorData.outputDirectoryPath);
    }
    public void ExportCharacters()
    {
        List<ysCharactorBundleData.ModelInfo> modelInfoList = new List<ysCharactorBundleData.ModelInfo>();
        modelInfoList.AddRange(ReadModelInfoList(ysCharactorBundleData.ListType.hero));
        modelInfoList.AddRange(ReadModelInfoList(ysCharactorBundleData.ListType.collection));
        modelInfoList.AddRange(ReadModelInfoList(ysCharactorBundleData.ListType.monster));
        modelInfoList.AddRange(ReadModelInfoList(ysCharactorBundleData.ListType.npc));
        modelInfoList.AddRange(ReadModelInfoList(ysCharactorBundleData.ListType.role));
        AssetBundleBuild[] buildArray = new AssetBundleBuild[modelInfoList.Count];
        for (int i = 0; i < modelInfoList.Count; i++)
        {
            buildArray[i].assetBundleName = FormatOutputName(modelInfoList[i].modelPrefab.name);
            string pathx00 = AssetDatabase.GetAssetPath(modelInfoList[i].modelPrefab);
            string pathx01 = AssetDatabase.GetAssetPath(modelInfoList[i].modelPrifabInCity);
            string[] paths = new string[] { pathx00, pathx01 };
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
        BuildPipeline.BuildAssetBundles(
        editorData.outputDirectoryPath,
        buildArray,
        BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
        buildTaget);
    }

    public void ExportCharacterOne(ysCharactorBundleData.ModelInfo modelInfo)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = FormatOutputName(modelInfo.modelPrefab.name);
        string pathx00 = AssetDatabase.GetAssetPath(modelInfo.modelPrefab);
        string pathx01 = AssetDatabase.GetAssetPath(modelInfo.modelPrifabInCity);
        string[] paths = new string[] { pathx00, pathx01 };

        if (string.IsNullOrEmpty(pathx01))
        {
            paths = new string[] { pathx00 };
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
    }
    public void ExportRoleOne(ysCharactorBundleData.RoleInfo modelInfo)
    {
        AssetBundleBuild[] buildArray = new AssetBundleBuild[1];
        buildArray[0].assetBundleName = FormatOutputName(modelInfo.roleModel.modelPrefab.name);
        string pathx00 = AssetDatabase.GetAssetPath(modelInfo.roleModel.modelPrefab);
        string[] pathx01 = new string[modelInfo.cosModels.Length];
        string[] pathx02 = new string[modelInfo.weapenModels.Length];
        for (int i = 0; i < pathx01.Length; i++)
        {
            pathx01[i] = AssetDatabase.GetAssetPath(modelInfo.cosModels[i]);
        }
        for (int i = 0; i < pathx02.Length; i++)
        {
            pathx02[i] = AssetDatabase.GetAssetPath(modelInfo.weapenModels[i]);
        }
        string[] paths = new string[] { pathx00 };

        string[] pathsAll = new string[paths.Length + pathx01.Length + pathx02.Length];
        paths.CopyTo(pathsAll, 0);
        pathx01.CopyTo(pathsAll, paths.Length);
        pathx02.CopyTo(pathsAll, paths.Length+pathx01.Length);

        buildArray[0].assetNames = pathsAll;

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
        EditorUtility.OpenFilePanel("查看文件", Path.GetFullPath(editorData.outputDirectoryPath), "unity3d");
    }
    public static void BatchModeExportAssets()
    {
        ysCharactorBundleData editorData = ScriptableObject.CreateInstance<ysCharactorBundleData>();
        editorData.UseDefaultData();
        ysCharactorBundleFunc editorFunc = new ysCharactorBundleFunc();
        editorFunc.editorData = editorData;
        editorFunc.ExportCharacters();
    }
}

public class ysCharactorBundleWindow : ysEditorWindow
{
    [MenuItem("Tools/角色导出 &C", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.character);
        return canShowWindow;
    }
    //public static ysCharactorBundleWindow charactorBundleWindow;
    [MenuItem("Tools/角色导出 &C")]
    public static void ShowEditorWindow()
    {
        window = GetWindow(typeof(ysCharactorBundleWindow), true) ;
    }
    ysCharactorBundleFunc editorFunc = new ysCharactorBundleFunc();

    List<ysCharactorBundleData.ModelInfo> displayList = new List<ysCharactorBundleData.ModelInfo>();
    List<ysCharactorBundleData.ModelInfo> searchList = new List<ysCharactorBundleData.ModelInfo>();
    List<ysCharactorBundleData.ModelInfo> heroList = new List<ysCharactorBundleData.ModelInfo>();
    List<ysCharactorBundleData.ModelInfo> monsterList = new List<ysCharactorBundleData.ModelInfo>();
    List<ysCharactorBundleData.ModelInfo> collectionList = new List<ysCharactorBundleData.ModelInfo>();
    List<ysCharactorBundleData.ModelInfo> npcList = new List<ysCharactorBundleData.ModelInfo>();


    List<ysCharactorBundleData.RoleInfo> displayRoleList = new List<ysCharactorBundleData.RoleInfo>();
    List<ysCharactorBundleData.RoleInfo> searchRoelList = new List<ysCharactorBundleData.RoleInfo>();
    List<ysCharactorBundleData.RoleInfo> roleList = new List<ysCharactorBundleData.RoleInfo>();

    TabToggleBtn[] tabBtns = new TabToggleBtn[5];
    bool tryRepair = false;
    void OnEnable()
    {
        editorFunc = new ysCharactorBundleFunc();
        editorFunc.LoadData();
        RegisterTabButtons();
        ReadModelInfoList(ref displayList, ysCharactorBundleData.ListType.hero);
    }

    GameObject exportPrefab;
    void OnGUI()
    {
        UseAltAndKeyBoardToCloseWindow(KeyCode.C);

        DrawTabToggleButtons(tabBtns);

        if (!tabBtns[4].isSelected)
        {

            if (DrawSearchBar(() => RegiesterSearchList(displayList, 0)))
            {
                searchList.Clear();
                searchList.AddRange(GetSearchResult<ysCharactorBundleData.ModelInfo>(0));
                searchList.AddRange(GetSearchResult<ysCharactorBundleData.ModelInfo>(1));
                searchList.AddRange(GetSearchResult<ysCharactorBundleData.ModelInfo>(2));
                searchList.AddRange(GetSearchResult<ysCharactorBundleData.ModelInfo>(3));
                //searchList.AddRange(GetSearchResult<ysCharactorBundleData.ModelInfo>(4));
                DrawScrollView(() => ShowModelInfoList(searchList), 3);
            }
            else
            {
                DrawScrollView(() => ShowModelInfoList(displayList), 0);
            }
        }
        else
        {
            if (DrawSearchBar(() => RegiesterSearchList(displayRoleList, 1)))
            {
                searchRoelList.Clear();
                searchRoelList.AddRange(GetSearchResult<ysCharactorBundleData.RoleInfo>(4));
                DrawScrollView(() => ShowRoelInfoList(searchRoelList), 3);
            }
            else
            {
                DrawScrollView(() => ShowRoelInfoList(displayRoleList), 0);
            }

        }
        DrawHorizental(() => { DrawExtentionModeToggle(); tryRepair = EditorGUILayout.ToggleLeft("检查时尝试修复", tryRepair); });

        DrawButton("检查资源", () => { editorFunc.errorInfoList.Clear(); CheckAll(tryRepair); });

        DrawButton("资源配置", () => ShowCheckSettingWindow());


        DrawErrorList(editorFunc.errorInfoList);

        if (editorFunc.errorInfoList == null || editorFunc.errorInfoList.Count < 1)
        {
            DrawButton("导出所有角色", () => ExportAllCharactor());
        }
        DrawHorizental(() => ShowListCount(displayList.Count, heroList.Count + monsterList.Count + collectionList.Count + npcList.Count + roleList.Count));

        DragExport();

        EditorGUILayout.LabelField(editorFunc.editorData.outputDirectoryPath);
        DrawDragbleTextField("test:", 0);
    }
    void RegisterTabButtons()
    {
        tabBtns[0] = new TabToggleBtn("Hero", () => { ReadModelInfoList(ref heroList, ysCharactorBundleData.ListType.hero); RegiesterSearchList(heroList, 0); });

        tabBtns[1] = new TabToggleBtn("Monster", () => { ReadModelInfoList(ref monsterList, ysCharactorBundleData.ListType.monster); RegiesterSearchList(monsterList, 1);
    });

        tabBtns[2] = new TabToggleBtn("Collection", () => { ReadModelInfoList(ref collectionList, ysCharactorBundleData.ListType.collection); RegiesterSearchList(collectionList, 2); });

        tabBtns[3] = new TabToggleBtn("Npc", () => { ReadModelInfoList(ref npcList, ysCharactorBundleData.ListType.npc); RegiesterSearchList(npcList, 3); });

        tabBtns[4] = new TabToggleBtn("Role", () => { ReadRoleInfoList(ref roleList); RegiesterSearchList(roleList, 4); });
    }
    void ReadModelInfoList(ref List<ysCharactorBundleData.ModelInfo> modelInfoList, ysCharactorBundleData.ListType listType, bool isShow = true)
    {
        if (modelInfoList == null || modelInfoList.Count < 1)
        {
            modelInfoList = editorFunc.ReadModelInfoList(listType);
        }
        if (isShow)
        {
            displayList = modelInfoList;
        }
    }
    void ReadRoleInfoList(ref List<ysCharactorBundleData.RoleInfo> roleInfoList, bool isShow = true)
    {
        if (roleInfoList == null || roleInfoList.Count < 1)
        {
            roleInfoList = editorFunc.ReadRoleInfoList();
        }
        if (isShow)
        {
 
            displayRoleList = roleInfoList;
        }
    }
    List<ysCharactorBundleData.ModelInfo> GetRoleModelList(List<ysCharactorBundleData.RoleInfo> roleInfoList)
    {
        List<ysCharactorBundleData.ModelInfo> modelList = new List<ysCharactorBundleData.ModelInfo>();
        for (int i = 0; i < roleInfoList.Count; i++)
        {
            ysCharactorBundleData.ModelInfo modelInfo = new ysCharactorBundleData.ModelInfo();
            modelInfo = roleInfoList[i].roleModel;
            modelList.Add(modelInfo);
        }
        return modelList;
    }


    void ShowModelInfoList(List<ysCharactorBundleData.ModelInfo> modelInfoList)
    {
        if (isExtentionMode)
        {
            ShowModelInfoListExtention(modelInfoList);
        }
        else
        {
            ShowModelInfoListSimple(modelInfoList);
        }
    }
    void ShowRoelInfoList(List<ysCharactorBundleData.RoleInfo> roleInfoList)
    {
        if (isExtentionMode)
        {
            ShowRoleInfoListSimple(roleInfoList);
        }
        else
        {
            ShowRoleInfoListSimple(roleInfoList);
        }
    }


    void ShowModelInfoListSimple(List<ysCharactorBundleData.ModelInfo> modelInfoList)
    {
        for (int i = 0; i < modelInfoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(modelInfoList[i].modelPrefab, typeof(GameObject), true);
            if (GUILayout.Button("材质设置", GUILayout.Width(100f)))
            {
                editorFunc.CheckShaderSet(modelInfoList[i], true);
                editorFunc.CheckTextureFormat(modelInfoList[i], true);
                EditorUtility.DisplayDialog("提示", "材质设置完成。", "继续");
            }

            if (GUILayout.Button("动画设置", GUILayout.Width(100f)))
            {
                if (editorFunc.CheckHasAnimatorController(modelInfoList[i]))
                {
                    if (modelInfoList[i].modelAnimator==null)
                    {
                        modelInfoList[i].modelAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(modelInfoList[i].modelAnimatorPath);
                    }
                }
                else
                {
                    return;
                }
                if (editorFunc.CheckModelImportSetting(modelInfoList[i], true))
                {
                    editorFunc.CheckLoopedAnimationClip(modelInfoList[i], true);
                    editorFunc.CheckAnimationClipXzSet(modelInfoList[i], true);
                    RematchAnimationClips(modelInfoList[i]);
                    EditorUtility.DisplayDialog("提示", "动画设置完成。", "继续");
                }
            }
            if (GUILayout.Button("导出",GUILayout.Width(100f)))
            {
                editorFunc.ExportCharacterOne(modelInfoList[i]);
                EditorUtility.DisplayDialog("提示", "导出完成。", "继续");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    void ShowModelInfoListExtention(List<ysCharactorBundleData.ModelInfo> modelInfoList)
    {
        for (int i = 0; i < modelInfoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(modelInfoList[i].modelPrefab, typeof(GameObject), true);
            EditorGUILayout.ObjectField(modelInfoList[i].modelPrifabInCity, typeof(GameObject), false);
            EditorGUILayout.ObjectField(modelInfoList[i].modelAnimator, typeof(AnimatorOverrideController), true);
            EditorGUILayout.ObjectField(modelInfoList[i].modelPrifabInCityAnimator, typeof(AnimatorOverrideController), true);
            EditorGUILayout.ObjectField(modelInfoList[i].modelMatirial, typeof(Material), true);
            EditorGUILayout.ObjectField(modelInfoList[i].modelTexture, typeof(Texture2D), true);
            if (GUILayout.Button("检查", GUILayout.Width(100f)))
            {
                CheckOne(modelInfoList[i]);
            }
            if (GUILayout.Button("分配动作", GUILayout.Width(100f)))
            {
                RematchAnimationClips(modelInfoList[i]);
            }
            if (GUILayout.Button("导出",GUILayout.Width(100f)))
            {
                editorFunc.ExportCharacterOne(modelInfoList[i]);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    void ShowRoleInfoListSimple(List<ysCharactorBundleData.RoleInfo> roleInfoList)
    {
        for (int i = 0; i < roleInfoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(roleInfoList[i].roleModel.modelPrefab, typeof(GameObject), true);
            //if (GUILayout.Button("材质设置", GUILayout.Width(100f)))
            //{
            //    editorFunc.CheckShaderSet(modelInfoList[i], true);
            //    editorFunc.CheckTextureFormat(modelInfoList[i], true);
            //    EditorUtility.DisplayDialog("提示", "材质设置完成。", "继续");
            //}

            //if (GUILayout.Button("动画设置", GUILayout.Width(100f)))
            //{
            //    if (editorFunc.CheckHasAnimatorController(modelInfoList[i]))
            //    {
            //        if (modelInfoList[i].modelAnimator == null)
            //        {
            //            modelInfoList[i].modelAnimator = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(modelInfoList[i].modelAnimatorPath);
            //        }
            //    }
            //    else
            //    {
            //        return;
            //    }
            //    if (editorFunc.CheckModelImportSetting(modelInfoList[i], true))
            //    {
            //        editorFunc.CheckLoopedAnimationClip(modelInfoList[i], true);
            //        editorFunc.CheckAnimationClipXzSet(modelInfoList[i], true);
            //        RematchAnimationClips(modelInfoList[i]);
            //        EditorUtility.DisplayDialog("提示", "动画设置完成。", "继续");
            //    }
            //}
            if (GUILayout.Button("导出", GUILayout.Width(100f)))
            {
                editorFunc.ExportRoleOne(roleInfoList[i]);
                EditorUtility.DisplayDialog("提示", "导出完成。", "继续");
            }
            EditorGUILayout.EndHorizontal();
        }

    }
    void RematchAnimationClips(ysCharactorBundleData.ModelInfo modelInfo)
    {
        AnimatorOverrideController overrideController = modelInfo.modelAnimator;
        AnimatorController baseController = modelInfo.baseController;
        editorFunc.MatchAnimationClips(overrideController, baseController);
    }
    void DragExport()
    {
        if (!isExtentionMode)
        {
            return;
        }
        EditorGUILayout.BeginHorizontal();

        exportPrefab = EditorGUILayout.ObjectField(exportPrefab, typeof(GameObject), true) as GameObject;
        if (GUILayout.Button("导出")   )
        {
            editorFunc.ExportPrefab(exportPrefab, editorFunc.editorData.buildTarget, editorFunc.editorData.outputDirectoryPath);
        }
        EditorGUILayout.EndHorizontal();

    }


    bool CheckOne(ysCharactorBundleData.ModelInfo modelInfo)
    {
        bool isRet = true;
        isRet &= editorFunc.CheckModelImportSetting(modelInfo, tryRepair);
        isRet &= editorFunc.CheckHasAnimatorController(modelInfo, tryRepair);
        isRet &= editorFunc.CheckLoopedAnimationClip(modelInfo, tryRepair);
        isRet &= editorFunc.CheckAnimationClipXzSet(modelInfo, tryRepair);
        isRet &= editorFunc.CheckShaderSet(modelInfo, tryRepair);
        isRet &= editorFunc.CheckTextureFormat(modelInfo, tryRepair);
        isRet &= editorFunc.CheckHasCityModel(modelInfo, tryRepair);
        if (isRet)
        {
            EditorUtility.DisplayDialog("检查报告", "没有异常，可以导出资源。", "继续");
        }
        return isRet;
    }

    bool CheckAll(bool tryRepair = false)
    {
        bool isRet = true;
        List<ysCharactorBundleData.ModelInfo> checkList = new List<ysCharactorBundleData.ModelInfo>();

        ReadModelInfoList(ref heroList, ysCharactorBundleData.ListType.hero, false);
        ReadModelInfoList(ref monsterList, ysCharactorBundleData.ListType.monster, false);
        ReadModelInfoList(ref collectionList, ysCharactorBundleData.ListType.collection, false);
        ReadModelInfoList(ref npcList, ysCharactorBundleData.ListType.npc, false);
        ReadRoleInfoList(ref roleList, false);

        checkList.AddRange(heroList);
        checkList.AddRange(monsterList);
        checkList.AddRange(collectionList);
        checkList.AddRange(npcList);
        //checkList.AddRange(roleList);

        for (int i = 0; i < checkList.Count; i++)
        {
            isRet&= editorFunc.CheckHasAnimatorController(checkList[i], tryRepair);
            isRet&= editorFunc.CheckLoopedAnimationClip(checkList[i], tryRepair);
            isRet&= editorFunc.CheckAnimationClipXzSet(checkList[i], tryRepair);
            isRet&= editorFunc.CheckShaderSet(checkList[i], tryRepair);
            isRet&= editorFunc.CheckTextureFormat(checkList[i], tryRepair);
            isRet&= editorFunc.CheckHasCityModel(checkList[i], tryRepair);
        }
        if (isRet == false)
        {
            //Debug.LogError("导出进程将被终止！");
        }
        else
        {
            if (editorFunc.errorInfoList.Count < 1)
            {
                EditorUtility.DisplayDialog("检查报告", "没有异常，可以导出资源。", "继续");
            }
        }
        return isRet;
    }


    void ShowErrorList(List<ysCharactorBundleData.ErrorInfo> errorList)
    {
        if (errorList.Count>0)
        {
            for (int i = errorList.Count-1; i >=0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(errorList[i].errorMsg,GUI.skin.GetStyle("Wizard Error"));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("定位"))
                {
                    Selection.activeObject = errorList[i].errorSrc;
                }
                if (GUILayout.Button("修复"))
                {
                    if (RepairError(errorList[i]))
                    {
                        editorFunc.errorInfoList.Remove(errorList[i]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    bool RepairError(ysCharactorBundleData.ErrorInfo errorInfo)
    {
        bool isRet = true;
        switch (errorInfo.errorType)
        {
            case ysCharactorBundleData.ErrorType.shaderError:
                editorFunc.CheckShaderSet(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.textureFormatError:
                editorFunc.CheckTextureFormat(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.missingCityMode:
                editorFunc.CheckHasCityModel(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.missingAnimationController:
                editorFunc.CheckHasAnimatorController(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.clipNotLoop:
                editorFunc.CheckLoopedAnimationClip(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.positionLockXz:
                editorFunc.CheckAnimationClipXzSet(errorInfo.modelInfo, true);
                break;
            case ysCharactorBundleData.ErrorType.modelImportError:
                isRet &= false;
                EditorUtility.DisplayDialog("警告", "需要手动设置模型导入属性，并手动更新所有子动作模型的Avatar！", "继续");
                break;
            default:
                break;
        }
        return isRet;
    }

    void DrawErrorList(List<ysCharactorBundleData.ErrorInfo> errorList)
    {
        if (errorList.Count > 0)
        {
            DrawScrollView(() => ShowErrorList(errorList), 1, GUILayout.Height(100f));
            DrawButton("清空", () => errorList.Clear());
        }
        else
        {

        }
    }

    void ShowCheckSettingWindow()
    {
        //Rect rect = new Rect(this.position.x, this.position.y, 380f, 400f);
        ysCharacterBundleCheckSettingWindow settingWindow = GetWindow<ysCharacterBundleCheckSettingWindow>(true);
        settingWindow.editorData = editorFunc.editorData;
    }

    void ExportAllCharactor()
    {
        List<ysCharactorBundleData.ModelInfo> modelInfoList = new List<ysCharactorBundleData.ModelInfo>();
        ReadModelInfoList(ref heroList, ysCharactorBundleData.ListType.hero, false);
        ReadModelInfoList(ref monsterList, ysCharactorBundleData.ListType.monster, false);
        ReadModelInfoList(ref collectionList, ysCharactorBundleData.ListType.collection, false);
        ReadModelInfoList(ref npcList, ysCharactorBundleData.ListType.npc, false);
        ReadRoleInfoList(ref roleList, false);
        modelInfoList.AddRange(heroList);
        modelInfoList.AddRange(monsterList);
        modelInfoList.AddRange(collectionList);
        modelInfoList.AddRange(npcList);
        //modelInfoList.AddRange(roleList);
        editorFunc.ExportCharacters(modelInfoList);
    }
    void ShowListCount(int count,int allCount)
    {
        EditorGUILayout.LabelField("当前 "+  count.ToString()+" 个",GUILayout.Width(80));
        EditorGUILayout.LabelField("共 " + allCount.ToString() + " 个", GUILayout.Width(80));
    }
}

public class ysCharacterBundleCheckSettingWindow : ysEditorWindow
{
    public ysCharactorBundleData editorData;

    //ysCharactorBundleData settingData;
    enum TextureSize
    {
        _128x128,
        _256x256,
        _512x512,
        _1024x1024,
    }
    TextureSize textureSize = TextureSize._512x512;
    int width = 100;
    void OnGUI()
    {
        DrawTitle("资源配置：");
        DrawHorizental(() => ShowFilterModeInfo(editorData));
        DrawHorizental(() => ShowWrapModeInfo(editorData));
        DrawHorizental(() => ShowTextureSize(editorData));
        DrawHorizental(() => ShowIsGenerateMipMap(editorData));
        DrawHorizental(() => ShowTextureFormatDiffuse(editorData));
        DrawHorizental(() => ShowTextureFormatAlpha(editorData));
        DrawHorizental(() => ShowShaderDiffuse(editorData));
        DrawHorizental(() => ShowShaderAlpha(editorData));
        //width = EditorGUILayout.IntField(width);
        DrawButton("确定", () => Close());
    }

    void ShowFilterModeInfo(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("过滤模式：");
        editorData.repairInfo.filterMode = (FilterMode)EditorGUILayout.EnumPopup(editorData.repairInfo.filterMode);
    }

    void ShowTextureSize(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("最大尺寸：");
        textureSize = (TextureSize)EditorGUILayout.EnumPopup(textureSize);
        switch (textureSize)
        {
            case TextureSize._128x128:
                editorData.repairInfo.textureSize = 128;
                break;
            case TextureSize._256x256:
                editorData.repairInfo.textureSize = 256;
                break;
            case TextureSize._512x512:
                editorData.repairInfo.textureSize = 512;
                break;
            case TextureSize._1024x1024:
                editorData.repairInfo.textureSize = 1024;
                break;
            default:
                editorData.repairInfo.textureSize = 512;
                break;
        }
    }
    void ShowWrapModeInfo(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("循环模式：");
        editorData.repairInfo.wrapMode = (WrapMode)EditorGUILayout.EnumPopup(editorData.repairInfo.wrapMode);
    }
    
    void ShowIsGenerateMipMap(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("MipMap：");
        editorData.repairInfo.isGenerateMipMap = EditorGUILayout.Toggle(editorData.repairInfo.isGenerateMipMap);
    }
    void ShowTextureFormatDiffuse(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("贴图格式：");
        editorData.repairInfo.textureFormatDiffuse = (TextureImporterFormat)EditorGUILayout.EnumPopup(editorData.repairInfo.textureFormatDiffuse);
    }
    void ShowTextureFormatAlpha(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("透贴格式：");
        editorData.repairInfo.textureFormatAlpha = (TextureImporterFormat)EditorGUILayout.EnumPopup(editorData.repairInfo.textureFormatAlpha);
    }
    enum ShaderType
    {
        character,
        hxh_role,
        hxh_role_cutout,
    }
    ShaderType shaderType = ShaderType.hxh_role;
    bool isBrowseShader = false;
    bool isBrowseShaderAlpah = false;
    void ShowShaderDiffuse(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("贴图渲染：");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(editorData.repairInfo.shaderDiffuse, typeof(Shader), false);
        if (GUILayout.Button("浏览"))
        {
            isBrowseShader = !isBrowseShader;
        }
        EditorGUILayout.EndHorizontal();
        if (isBrowseShader)
        {

            for (int i = 0; i < editorData.repairInfo.shaders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                UnityEngine.Object shader = EditorGUILayout.ObjectField(editorData.repairInfo.shaders[i], typeof(Shader), false);

                if (GUILayout.Button("选择"))
                {
                    editorData.repairInfo.shaderDiffuse = (Shader)shader;
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndVertical();
    }
    void ShowShaderAlpha(ysCharactorBundleData editorData)
    {
        EditorGUILayout.LabelField("透贴渲染：");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ObjectField(editorData.repairInfo.shaderAlpha, typeof(Shader), false);
        if (GUILayout.Button("浏览"))
        {
            isBrowseShaderAlpah = !isBrowseShaderAlpah;
        }
        EditorGUILayout.EndHorizontal();
        if (isBrowseShaderAlpah)
        {

            for (int i = 0; i < editorData.repairInfo.shaders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                UnityEngine.Object shader = EditorGUILayout.ObjectField(editorData.repairInfo.shaders[i], typeof(Shader), false);

                if (GUILayout.Button("选择"))
                {
                    editorData.repairInfo.shaderAlpha = (Shader)shader;
                }

                EditorGUILayout.EndHorizontal();
            }
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndVertical();
    }
}

