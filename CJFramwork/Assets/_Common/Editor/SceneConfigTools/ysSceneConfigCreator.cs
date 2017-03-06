using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class ysSceneConfigCreator  {
    [MenuItem("GameObject/ysCity/New", false, 20)]
    static void CreateCityRoot()
    {
        CheckHasCityRoot(true);
        CreateConfig("city",CityRoot);
    }
    [MenuItem("GameObject/ysCity/Load", false, 20)]
    static void LoadCityRoot()
    {
        CheckHasCityRoot(true);
        LoadConfig(CityRoot,0);
    }
    [MenuItem("GameObject/ysCity/Save", false, 20)]
    static void SaveCityRoot()
    {
        if (CheckHasCityRoot())
        {
            SaveConfig(CityRoot);
        }
        else
        {
            EditorUtility.DisplayDialog("警告", "场景中不存在可保存物体", "继续");
        }
        EditorApplication.Beep();
    }
    [MenuItem("GameObject/ysLevel/New", false, 21)]
    static void CreateLevelRoot()
    {
        CheckHasLevelRoot(true);
        CreateConfig("level", LevelRoot);
    }
    [MenuItem("GameObject/ysLevel/Load", false, 21)]
    static void LoadLevelRoot()
    {
        CheckHasLevelRoot(true);
        LoadConfig(LevelRoot,1);
    }
    [MenuItem("GameObject/ysLevel/Save", false, 21)]
    static void SaveLevelRoot()
    {
        if (CheckHasLevelRoot())
        {
            SaveConfig(LevelRoot);
        }
        else
        {
            EditorUtility.DisplayDialog("警告", "场景中不存在可保存物体", "继续");
        }
        EditorApplication.Beep();
    }

    static GameObject CityRoot;
    static GameObject LevelRoot;

    static bool CheckHasCityRoot(bool isCrt = false)
    {
        CityRoot = GameObject.Find("city_root");
        if (CityRoot == null && isCrt)
        {
            CityRoot = CreateNewRoot("city_root");
        }
        return CityRoot != null;
    }
    static bool CheckHasLevelRoot(bool isCrt = false)
    {
        LevelRoot = GameObject.Find("level_root");
        if (LevelRoot == null && isCrt)
        {
            LevelRoot = CreateNewRoot("level_root");
        }
        return LevelRoot != null;
    }
    static GameObject CreateNewRoot(string rootName)
    {
        GameObject root = new GameObject(rootName);
        return root;
    }
    static void CreateConfig(string configName, GameObject root)
    {
        GameObject levelConfig = new GameObject(configName+"_" +SceneManager.GetActiveScene().name + "_0" );
        GameObject bornPoint =GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Misc/ScenesConfig/born_point.prefab"));
        GameObject stopWall = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Misc/ScenesConfig/stop_wall.prefab"));
        GameObject trigger = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Misc/ScenesConfig/trigger.prefab"));
        GameObject monsterLayout = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Misc/ScenesConfig/monster_layout.prefab"));

        levelConfig.transform.SetParent(root.transform);
        bornPoint.transform.SetParent(levelConfig.transform);
        stopWall.transform.SetParent(levelConfig.transform);
        trigger.transform.SetParent(levelConfig.transform);
        monsterLayout.transform.SetParent(levelConfig.transform);

        bornPoint.name = bornPoint.name.Remove(bornPoint.name.Length - 7, 7);
        stopWall.name = stopWall.name.Remove(stopWall.name.Length - 7, 7);
        trigger.name = trigger.name.Remove(trigger.name.Length - 7, 7);
        monsterLayout.name = monsterLayout.name.Remove(monsterLayout.name.Length - 7, 7);

    }

    static int selectCount = 0;
    static void SaveConfig(GameObject root)
    {
		
		GameObject item;
		if (Selection.gameObjects == null) 
		{
			item = Selection.activeGameObject;
		} 
		else 
		{
		
			item= Selection.gameObjects[selectCount++];
		}
        if (selectCount == Selection.gameObjects.Length)
        {
            selectCount = 0;
        }
        if (item.name.Contains("city_" + SceneManager.GetActiveScene().name) || item.name.Contains("level_" + SceneManager.GetActiveScene().name))
        {
           
        }
        else
        {
            Debug.LogError(item.name + "无法保存该物体！");
            return;
        }
        GameObject checker = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ScenesConfig/" + item.gameObject.name + ".prefab");
        if (checker == null)
        {
            PrefabUtility.CreatePrefab("Assets/ScenesConfig/" + item.gameObject.name + ".prefab", item.gameObject);
        }
        else
        {
            //if (EditorUtility.DisplayDialog("警告", "已经存在同名预制体:"+ item.gameObject.name, "覆盖", "重命名"))
            //{
            //    PrefabUtility.CreatePrefab("Assets/ScenesConfig/" + item.gameObject.name + ".prefab", item.gameObject);
            //}
			string err = EditorUtility.SaveFilePanelInProject("另存预制体", item.gameObject.name, "prefab", null, "Assets/ScenesConfig/");
			PrefabUtility.CreatePrefab(err, item.gameObject);
        }
    }
    static void LoadConfig(GameObject root, int type)
    {
        string path = EditorUtility.OpenFilePanelWithFilters("选择装载配置预设", Application.dataPath + "/ScenesConfig/", new string[] { "prefab", "prefab" });
        GameObject load = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ScenesConfig/" + Path.GetFileName(path));
        bool isRet = true;
        try
        {
            GameObject instance = GameObject.Find(load.name);
            if (instance != null)
            {
                isRet = EditorUtility.DisplayDialog("提示", "当前预设已经存在，是否覆盖？", "覆盖", "取消");
                if (isRet)
                {
                    GameObject.DestroyImmediate(instance);
                }
            }
            if (isRet)
            {
                GameObject go = GameObject.Instantiate(load);
                if (go.name.Contains("city_"))
                {
                    CheckHasCityRoot(true);
                    root = CityRoot;
                }
                if (go.name.Contains("level_"))
                {
                    CheckHasLevelRoot(true);
                    root = LevelRoot;
                }
                go.transform.SetParent(root.transform);
                go.name = go.name.Remove(go.name.Length - 7, 7);
                Selection.activeGameObject = go;
                //EditorUtility.DisplayDialog("提示", "load操作完成。", "确定");
            }
            else
            {
                //EditorUtility.DisplayDialog("提示", "load操作已取消。", "确定");
            }
        }
        catch (System.Exception ex)
        {
   

        }

    }

    //public class ysSceneConfigLoadWindow : ysEditorWindow
    //{
    //    static List<GameObject> configList = new List<GameObject>();
    //    static List<GameObject> cityConfigList = new List<GameObject>();
    //    static List<GameObject> levelConfigList = new List<GameObject>();
    //    List<GameObject> showList = new List<GameObject>();
    //    static GameObject root;
    //    static List<GameObject> needClean = new List<GameObject>();
    //    static int type;
    //    List<GameObject> loadList = new List<GameObject>();

    //    public static void ShowWindow(GameObject root_,List<GameObject> needClean_,int type_)
    //    {
    //        type = type_;
    //        root = root_;
    //        needClean = needClean_;
    //        ysEditorFunc func = new ysEditorFunc();
    //        configList = func.ReadPrefabList("Assets/ScenesConfig");
    //        for (int i = 0; i < configList.Count; i++)
    //        {
    //            if (configList[i].name.Contains("city_"))
    //            {
    //                cityConfigList.Add(configList[i]);
    //            }
    //            if (configList[i].name.Contains("level_"))
    //            {
    //                levelConfigList.Add(configList[i]);
    //            }
    //        }
    //        window = EditorWindow.GetWindow(typeof(ysSceneConfigLoadWindow), true);
    //    }

    //    void OnGUI()
    //    {

    //        ShowConfigList(type);

    //        DrawButton("完成", () =>
    //        {
    //            LoadConfig(root, needClean,type);
    //            Close();
    //        }
            
    //        );

    //    }

    //    Vector2 showVect = new Vector2();
    //    Vector2 loadVect = new Vector2();
    //    void ShowConfigList(int type)
    //    {
    //        if (type == 0)
    //        {
    //            showList = cityConfigList;
    //        }
    //        else if (type == 1)
    //        {
    //            showList = levelConfigList;
    //        }
    //        EditorGUILayout.BeginHorizontal();
    //        EditorGUILayout.BeginScrollView(showVect);
    //        //EditorGUILayout.BeginVertical();
    //        for (int i = showList.Count-1; i >=0; i--)
    //        {
    //            EditorGUILayout.BeginHorizontal();
    //            EditorGUILayout.ObjectField(showList[i], typeof(GameObject), false);
    //            if (GUILayout.Button("装载"))
    //            {
    //                loadList.Add(showList[i]);
    //                showList.Remove(showList[i]);
    //            }
    //            EditorGUILayout.EndHorizontal();
    //        }
    //        EditorGUILayout.EndScrollView();
    //        //EditorGUILayout.EndVertical();
    //        EditorGUILayout.BeginScrollView(loadVect);
    //        //EditorGUILayout.BeginVertical();
    //        for (int i = loadList.Count - 1; i >= 0; i--)
    //        {
    //            EditorGUILayout.BeginHorizontal();
    //            EditorGUILayout.ObjectField(loadList[i], typeof(GameObject), false);
    //            if (GUILayout.Button("反载"))
    //            {
    //                showList.Add(loadList[i]);
    //                loadList.Remove(showList[i]);
    //            }
    //            EditorGUILayout.EndHorizontal();
    //        }
    //        //EditorGUILayout.EndVertical();
    //        EditorGUILayout.EndScrollView();
    //        EditorGUILayout.EndHorizontal();
    //    }
    //    void CleanConfig(List<GameObject> needClean)
    //    {
    //        for (int i = needClean.Count - 1; i >= 0; i--)
    //        {
    //            GameObject.DestroyImmediate(needClean[i]);
    //        }
    //    }

    //    void LoadConfig(GameObject root, List<GameObject> needClean,int type)
    //    {
    //        CleanConfig(needClean);
    //        string sceneName = SceneManager.GetActiveScene().name;
    //        string rootName = root.name.Split('_')[0] + "_";

    //        //List<GameObject> loadList = new List<GameObject>();
    //        //if (type==0  )
    //        //{
    //        //    loadList = cityConfigList;
    //        //}
    //        //else if (type ==1)
    //        //{
    //        //    loadList = levelConfigList;
    //        //}

    //        for (int i = 0; i < loadList.Count; i++)
    //        {
    //            GameObject go = GameObject.Instantiate(loadList[i]);
    //            go.transform.SetParent(root.transform);
    //            go.name = go.name.Remove(go.name.Length - 7, 7);
    //        }

    //        EditorUtility.DisplayDialog("提示", "操作（Load）成功。", "确定");

    //    }
    //}




































    }