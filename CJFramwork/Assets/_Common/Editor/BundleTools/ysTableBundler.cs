using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
public class ysTableBundleData:ysEditorData
{
    public struct Paths
    {
        public string textPath;
        public string classSccriptPath;
        public string scriptableObjectPath;
    }
    public struct AssetDirectoryPaths
    {
        public string textAssetDirectoryPath;
        public string classSccriptAssetDirectoryPath;
        public string scriptableObjectAssetDirectoryPath;
    }
    public class TableInfo:ysISearchble
    {
        public string tableClassName;
        public MonoScript classScript;
        public TextAsset textAsset;
        public ScriptableObject scriptableObject;
        public DataState dataState = ysEditorData.DataState.needUpdate;
        public string GetSearchString()
        {
            return tableClassName;
        }
    }
    public void Initialize()
    {
        GeneratePath();
    }
    public string bundleName = "tableassets";
    public Paths paths;
    public AssetDirectoryPaths assetDirectoryPaths;
    public string[] ignoreStrings = new string[] { "0", "biaoqing", "coordinate"};
    void GeneratePath()
    {
        assetDirectoryPaths.textAssetDirectoryPath = "Assets/_Config/";
        assetDirectoryPaths.classSccriptAssetDirectoryPath = "Assets/_Common/ConfigTable/";
        assetDirectoryPaths.scriptableObjectAssetDirectoryPath = "Assets/TableAssets/";
        paths.textPath = FormatAssetPath2FullPath(assetDirectoryPaths.textAssetDirectoryPath);
        paths.classSccriptPath = FormatAssetPath2FullPath(assetDirectoryPaths.classSccriptAssetDirectoryPath);
        paths.scriptableObjectPath = FormatAssetPath2FullPath(assetDirectoryPaths.scriptableObjectAssetDirectoryPath);
        outputDirectoryPath = GetOutputPath("config");
    }
}
public class ysTableBundleFunc: ysEditorFunc
{
    public ysTableBundleData data = ScriptableObject.CreateInstance<ysTableBundleData>();
    public List<ScriptableObject> scriptableObjectList = new List<ScriptableObject>();
    public ysTableBundleFunc()
    {
        data.Initialize();
        scriptableObjectList = ReadScriptableObjectList(data.paths.scriptableObjectPath);
    }
    public List<ysTableBundleData.TableInfo> ReadTableInfo()
    {
        List<TextAsset> textAssetList = ReadTextAssetList(data.paths.textPath);
        List<ysTableBundleData.TableInfo> tableInfoList = new List<ysTableBundleData.TableInfo>();
        for (int i = 0; i < textAssetList.Count; i++)
        {
            if (CheckHasIgnoreString(textAssetList[i].name))
            {
                continue;
            }
            //string[] tempx00 = textAssetList[i].name.Split('_');
            //string classScriptName="";
            //for (int j = 0; j < tempx00.Length; j++)
            //{
            //    tempx00[j] = tempx00[j].Substring(0, 1).ToUpper() + tempx00[j].Substring(1, tempx00[j].Length - 1);
            //    classScriptName += tempx00[j];
            //}
            //classScriptName += "Table";
            string classScriptName = GenerateClassTableName(textAssetList[i].name);
            ysTableBundleData.TableInfo tableInfo = new ysTableBundleData.TableInfo();
            tableInfo.tableClassName = classScriptName;
            tableInfo.textAsset = textAssetList[i];
            string tempx01 = data.assetDirectoryPaths.classSccriptAssetDirectoryPath + classScriptName + ".cs";
            tableInfo.classScript = AssetDatabase.LoadAssetAtPath<MonoScript>(tempx01);
            string tempx02 = data.assetDirectoryPaths.scriptableObjectAssetDirectoryPath + classScriptName + ".asset";
            tableInfo.scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(tempx02);
            tableInfoList.Add(tableInfo);
        }
        return tableInfoList;  
    }
    public string GenerateClassTableName(string textName)
    {
        string[] tempx00 = textName.Split('_');
        string classScriptName = "";
        for (int j = 0; j < tempx00.Length; j++)
        {
            tempx00[j] = tempx00[j].Substring(0, 1).ToUpper() + tempx00[j].Substring(1, tempx00[j].Length - 1);
            classScriptName += tempx00[j];
        }
        return classScriptName += "Table"; 
    }
    bool CheckHasIgnoreString(string checkString)
    {
        for (int i = 0; i < data.ignoreStrings.Length; i++)
        {
            if (checkString.Contains(data.ignoreStrings[i]))
            {
                return true;
            }
        }
        return false;
    }
    //public bool CheckScriptObjectHasCorrectClass(MonoScript script,ScriptableObject sptObj)
    //{
    //    Type tableClass = script.GetClass();
    //    Type tableDataClass = tableClass.Assembly.GetType(tableClass.Name + "Data");
    //    FieldInfo[] scriptFieldInfos = tableDataClass.GetFields();
    //    int srcCount = scriptFieldInfos.Length;

    //    Type sptObjClass = sptObj.GetType();

    //    Type sptObjDataClass = sptObjClass.GetFields()[0].FieldType.GetGenericArguments()[0];
    //    FieldInfo sptObjFieldInfo = sptObjClass.GetFields()[0];

    //    Debug.Log(sptObjDataClass.Name);

    //    return false;
    //}
    public bool CheckTextAndClassIsMatch(TextAsset textAsset,MonoScript script,bool isDisplayDetials)
    {
        //string textPath = Path.GetFullPath(AssetDatabase.GetAssetPath(textAsset));
        string[][] data = ReadDataFromText(textAsset.text);
        int textFieldLenth = data[1].Length;
       
        if (script==null)
        {
            Debug.Log("提示： " +textAsset.name +" 的类未被创建，导出时该配表将被忽略。");
            return true;
        }
        Type tableClass = script.GetClass();
        Type tableDataClass = tableClass.Assembly.GetType(tableClass.Name + "Data");
        FieldInfo[] scriptFieldInfos = tableDataClass.GetFields();
        int objectFieldLenth = scriptFieldInfos.Length;

        if (isDisplayDetials)
        {
            List<string> textFieldInfoLists = new List<string>();
            List<string> scriptFieldInfoLists = new List<string>();
            for (int i = 0; i < data[1].Length; i++)
            {
                textFieldInfoLists.Add(data[1][i]);
            }
            for (int i = 0; i < scriptFieldInfos.Length; i++)
            {
                scriptFieldInfoLists.Add(scriptFieldInfos[i].Name);
            }
            ysTableBundleWarningBox.ShowFieldInfos(textFieldInfoLists, scriptFieldInfoLists);
        }
        else
        {
            if (textFieldLenth > objectFieldLenth)
            {
                Debug.LogError(textAsset.name + " 配表有新增字段");

            }
            else if (textFieldLenth < objectFieldLenth)
            {
                Debug.LogError(textAsset.name + " 配表有删减字段");
            }
        }
        return textFieldLenth == objectFieldLenth;
    }
    public void CreateAssetByTxt(ysTableBundleData.TableInfo tableInfo)
    {
        Assembly assembly = Assembly.Load("Assembly-CSharp");
        Type tableClassType = assembly.GetType(tableInfo.tableClassName);
        Type dataClassType = assembly.GetType(tableInfo.tableClassName + "Data");
        //Assembly.Load("Assembly-CSharp-Editor").GetType("ysTableBundleFunc")
        this.GetType()
            .GetMethod("CreateAssetFromText")
            .MakeGenericMethod(tableClassType, dataClassType)
            .Invoke(this, new object[] { tableInfo.textAsset.text });
    }
    public string[][] ReadDataFromText(string text)
    {
        char[] p = new char[] { '"', '(', '\\', '\\', '.', '|', '[', '^', '\\', '\\', '"', ']', ')', '*', '"' };
        string pattern = "";
        for (int i = 0; i < p.Length; i++)
        {
            pattern += p[i];
        }
        //Debug.Log(pattern);  //"(\\.|[^\\"])*" ;
        Regex regex = new Regex(pattern);
        MatchCollection results = regex.Matches(text);
        string[] textReplaced = new string[results.Count];
        for (int i = 0; i < textReplaced.Length; i++)
        {
            textReplaced[i] = results[i].Value;
        }
        text = regex.Replace(text, "N/A");
        string[] lines = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].TrimEnd('\r');
        }
        List<int> skipColumes = new List<int>();
        int[] checkLines = new int[] { 3, 4 };
        string[] skipString = new string[] { "note", "\t", "s" };
        for (int i = 0; i < checkLines.Length; i++)
        {
            string[] checkString = lines[checkLines[i] - 1].Split('\t');
            for (int j = 0; j < checkString.Length; j++)
            {
                bool isSkip = false;
                for (int k = 0; k < skipString.Length; k++)
                {
                    isSkip |= checkString[j].Equals(skipString[k]);
                }
                if (isSkip)
                {
                    skipColumes.Add(j - skipColumes.Count);
                }
            }
        }
        int length = lines.Length;
        if (lines.Length > 1 && (lines[lines.Length - 1] == null || lines[lines.Length - 1] == "" || lines[lines.Length - 1] == "\t" || lines[lines.Length - 1] == "\n"))
        {
            length = lines.Length - 1;
        }
        int replaceIndex = 0;
        string[][] data = new string[length][];
        for (int i = 0; i < length; i++)
        {
            string[] tempx00 = lines[i].Split('\t');
            string[] tempx01 = new string[tempx00.Length + 1];
            tempx00.CopyTo(tempx01, 0);
            for (int j = 0; j < skipColumes.Count; j++)
            {
                for (int k = 0; k < tempx00.Length; k++)
                {
                    if (k >= skipColumes[j])
                    {
                        tempx01[k] = tempx01[k + 1];
                    }
                }
            }
            string[] tempx02 = new string[tempx01.Length - skipColumes.Count - 1];
            for (int j = 0; j < tempx02.Length; j++)
            {
                tempx02[j] = tempx01[j];
                if (tempx02[j] == "N/A")
                {
                    tempx02[j] = textReplaced[replaceIndex++];
                }
            }
            data[i] = tempx02;
        }
        return data;
    }
    public void CreateTableClass(string tableName, string text)
    {
        string[][] textData = ReadDataFromText(text);
        Dictionary<string, string> typex00 = new Dictionary<string, string>()
            {
                {"vec","Vector3"},
                {"int","int"},
                {"float","float"},
                {"str","string"},
                {"note","string"},
                {"describe","string"}
            };
        Dictionary<string, string>.KeyCollection keys = typex00.Keys;
        for (int i = 0; i < textData[2].Length; i++)
        {
            bool isArray = textData[2][i].Contains("[]");
            bool isArray2D = isArray ? textData[2][i].Contains("[][]") : false;
            foreach (string str in keys)
            {
                textData[2][i] = textData[2][i].ToLower().Contains(str) ? typex00[str] : textData[2][i];
            }
            textData[2][i] = isArray ? (textData[2][i] + "[]") : textData[2][i];
            textData[2][i] = isArray2D ? (textData[2][i] + "[]") : textData[2][i];
        }
        byte[] byData;
        char[] writeCharData;
        string needStringx00 = "using UnityEngine;using System.Collections;using System.Collections.Generic;\n";
        string needStringx01 = "public class ";
        string className = tableName;
        string classDataName = className + "Data";
        string baseName = ": ScriptableObject";
        string startSymbol = "\n{\n";
        string needStringx02 = "\tpublic List <" + classDataName + "> table = new List<" + classDataName + ">() ;\n";
        string endSymbol = "\n}\n";
        string baseNamex02 = "";
        string needStringx03 = "[System.Serializable]\npublic class " + classDataName + baseNamex02 + startSymbol;
        string properties = "";
        string formater;
        for (int i = 0; i < textData[2].Length; i++)
        {
            string dataType = textData[2][i].TrimEnd('\r');
            string dataOrigin = textData[1][i].TrimEnd('\r');
            if (textData[2][i].Contains("[][]"))
            {
                if (textData[2][i].Contains("int"))
                {
                    formater = "ReadInt32Array2D";
                }
                else if (textData[2][i].Contains("float"))
                {
                    formater = "ReadFloatArray2D";
                }
                else
                {
                    formater = "ReadStringArray2D";
                }
                properties += "\tpublic string s_" + dataOrigin + ";\n"
                    + "\tprivate " + dataType + dataOrigin + "_=null;\n"
                    + "\tpublic " + dataType + " " + dataOrigin + "{get{if(" + dataOrigin + "_==null)" + dataOrigin + "_=ysDataFormater." + formater + "(s_" + dataOrigin + ");return " + dataOrigin + "_;}set{}}\n";
            }
            else
            {
                properties += "\tpublic " + dataType + " " + dataOrigin + ";\n";
            }
        }
        properties.TrimEnd('\n');
        string writeString = needStringx00 + needStringx01 + className + baseName + startSymbol + needStringx02 + endSymbol + needStringx03 + properties + "}\n";

#if !UNITY_EDITOR_OSX
        writeString.Replace("\n", "\r\n");
#endif
        //string s_LastCreateClassText = writeString;
        using (FileStream fs = System.IO.File.Create( data.paths.classSccriptPath + tableName + ".cs"))
        {
            writeCharData = writeString.ToCharArray();
            byData = new byte[writeCharData.Length];
            System.Text.Encoder e = System.Text.Encoding.UTF8.GetEncoder();
            e.GetBytes(writeCharData, 0, writeCharData.Length, byData, 0, true);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(byData, 0, byData.Length);
            fs.Close();
        }
    }
    public void CreateAssetFromText<TClass, TData>(string text) where TClass : ScriptableObject
    {
        
        List<TData> retList = new List<TData>();
        string saveFileName = typeof(TClass).ToString();
        string scriptableObjectPath = data.assetDirectoryPaths.scriptableObjectAssetDirectoryPath;
        if (!Directory.Exists(scriptableObjectPath))
        {
            Directory.CreateDirectory(scriptableObjectPath);
        }
        string[][] tableData = ReadDataFromText(text);
        if (CheckoutRepeatID(tableData, typeof(TClass).FullName))
        {
            return;
        }
        Dictionary<string, string> typex00 = new Dictionary<string, string>()
        {
            {"vec","Vector3"},
            {"int","int"},
            {"float","float"},
            {"str","string"},
        };
        Dictionary<string, string>.KeyCollection keys = typex00.Keys;
        for (int i = 0; i < tableData[2].Length; i++)
        {
            bool isArray = tableData[2][i].Contains("[]");
            bool isArray2D = isArray ? tableData[2][i].Contains("[][]") : false;
            foreach (string str in keys)
            {
                tableData[2][i] = tableData[2][i].ToLower().Contains(str) ? typex00[str] : tableData[2][i];
            }
            tableData[2][i] = isArray ? (tableData[2][i] + "[]") : tableData[2][i];
            tableData[2][i] = isArray2D ? (tableData[2][i] + "[]") : tableData[2][i];
        }
        TClass tableClass = ScriptableObject.CreateInstance<TClass>();
        for (int i = 0 + 4; i < tableData.Length; i++)
        {
            TData itemClass = Activator.CreateInstance<TData>();
            Type classx01 = typeof(TData);
            FieldInfo[] infox01 = classx01.GetFields();
            for (int j = 0; j < infox01.Length; j++)
            {
                try
                {
                    if (tableData[i][j] == " ")
                    {
                        tableData[i][j].Trim();
                        Debug.LogError("第" + i + "行 第" + j + "列");
                    }
                    switch (tableData[2][j])
                    {
                        case "int":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadInt32(tableData[i][j]));
                            break;
                        case "float":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadFloat(tableData[i][j]));
                            break;
                        case "Vector3":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadVector3(tableData[i][j]));
                            break;
                        case "string":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadString(tableData[i][j]));
                            break;

                        case "int[]":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadInt32Array(tableData[i][j]));
                            break;
                        case "float[]":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadFloatArray(tableData[i][j]));
                            break;
                        case "string[]":
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadStringArray(tableData[i][j]));
                            break;

                        //					case "int[][]":
                        //						infox01[j].SetValue(itemClass,ysDataFormater.ReadInt32Array2D(data[i][j]));
                        //						break;
                        //					case "float[][]":
                        //						infox01[j].SetValue(itemClass,ysDataFormater.ReadFloatArray2D(data[i][j]));
                        //						break;
                        //					case "string[][]":
                        //						infox01[j].SetValue(itemClass,ysDataFormater.ReadStringArray2D(data[i][j]));
                        //						break;

                        default:
                            infox01[j].SetValue(itemClass, ysDataFormater.ReadString(tableData[i][j]));
                            break;
                    }

                }
                catch (Exception exx01)
                {
                    try
                    {
                        //Debug.LogError("line:" + (i + 1) + ":cell:" + (j + 1) + "   type:" + tableData[2][j] + "-->  text--> " + tableData[i][j] + " table name -->" + textAsset.ToString() + " -->数据类型不匹配");
                    }
                    catch (System.Exception exx02)
                    {
                        //Debug.LogError(textAsset + " 字段数量不匹配！");
                        Debug.LogError(exx02);
                    }

                    Debug.LogError(exx01);
                }

            }
            retList.Add(itemClass);
        }

        FieldInfo list = tableClass.GetType().GetField("table");
        list.SetValue(tableClass, retList);
        AssetDatabase.CreateAsset(tableClass, scriptableObjectPath + saveFileName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public  List<string> ErrorTableList = new  List<string>();
    private  bool CheckoutRepeatID(string[][] data, string tableName)
    {
        bool isRet = false;
        List<string> repeatIdList = new List<string>();
        List<string> dataList = new List<string>();
        for (int i = 3; i < data.Length; i++)
        {
            if (dataList.Contains(data[i][0]))
            {
                repeatIdList.Add("[id]: " + data[i][0] + "  第" + (i + 1) + "行");
            }
            dataList.Add(data[i][0]);
        }
        if (repeatIdList.Count > 0)
        {
            isRet = true;
            string allErrors = "";

            for (int i = 0; i < repeatIdList.Count; i++)
            {
                allErrors += ('\n' + repeatIdList[i]);
            }
            ErrorTableList.Add("[" + tableName + "]" + " 存在重复[id]" + allErrors);
        }
        return isRet;
    }
    public ScriptableObject ReloadAsset(ysTableBundleData.TableInfo tableInfo)
    {
        string assetPath = data.assetDirectoryPaths.scriptableObjectAssetDirectoryPath + tableInfo.tableClassName + ".asset";
        ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
        for (int i = 0; i < scriptableObjectList.Count; i++)
        {
            if (scriptableObjectList[i]==null&& !scriptableObjectList.Contains(scriptableObject))
            {
                scriptableObjectList[i] = scriptableObject;
            }
        }
        return scriptableObject;
    }
    public static void BatchModeCreateAssets()
    {
        ysTableBundleData editorData = new ysTableBundleData();
        editorData.Initialize();
        ysTableBundleFunc editorFunc = new ysTableBundleFunc();
        editorFunc.data = editorData;
        List<ysTableBundleData.TableInfo> tableInfoList = editorFunc.ReadTableInfo();
        for (int i = 0; i < tableInfoList.Count; i++)
        {
            if (tableInfoList[i].classScript==null)
            {
                continue;
            }
            editorFunc.CreateAssetByTxt(tableInfoList[i]);
        }
    }
    public static void BatchModeExportAssets()
    {
        ysTableBundleData editorData = new ysTableBundleData();
        editorData.Initialize();
        ysTableBundleFunc editorFunc = new ysTableBundleFunc();
        editorFunc.data = editorData;
        editorFunc.scriptableObjectList = editorFunc.ReadScriptableObjectList(editorData.paths.scriptableObjectPath);
        editorFunc.ExportScriptableObjects(
            editorFunc.scriptableObjectList,
            editorFunc.data.buildTarget,
            editorFunc.data.bundleName,
            editorFunc.data.outputDirectoryPath);
    }
}
public class ysTableBundleWindow : ysEditorWindow
{
    [MenuItem("Tools/配表打包 &T", true)]
    private static bool CheckCanUseEditor()
    {
        canShowWindow = CheckCanUseEditorAtCurrentProgram(ProgramType.design);
        return canShowWindow;
    }
    [MenuItem("Tools/配表打包 &T")]
    public static void Initialize()
    {
        func = new ysTableBundleFunc();
        displayTableInfoList = func.ReadTableInfo();
        window = EditorWindow.GetWindow(typeof(ysTableBundleWindow), true);
    }
    static ysTableBundleFunc func;
    static List<ysTableBundleData.TableInfo> displayTableInfoList = new List<ysTableBundleData.TableInfo>();
    static List<ysTableBundleData.TableInfo> searchedTableInfoList = new List<ysTableBundleData.TableInfo>();
    TextAsset inputText = new TextAsset();


    void OnGUI()
    {
        UseAltAndKeyBoardToCloseWindow(KeyCode.T);

        if (DrawSearchBar(() => RegiesterSearchList(displayTableInfoList, 0)))
        {
            DrawHorizental(() => ShowListCount(searchedTableInfoList));
            searchedTableInfoList = GetSearchResult<ysTableBundleData.TableInfo>(0);
            DrawScrollView(() => ShowTableInfo(searchedTableInfoList), 0);
        }
        else
        {
            DrawHorizental(() => ShowListCount(displayTableInfoList));
            DrawScrollView(() => ShowTableInfo(displayTableInfoList), 1);
        }
        
        DoCheckAll();
        ExportAllAssetToBundle();
        DrawHorizental(() => CreateClass());
        //DrawHorizental(() => ChangeInputPath(0));
        EditorGUILayout.Space();
    }
    void ShowTableInfo(List< ysTableBundleData.TableInfo> tableInfoList)
    {
        //Event current = Event.current;
        for (int i = 0; i < tableInfoList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            Rect rect = EditorGUILayout.GetControlRect();
            GetChosenAsset(rect, tableInfoList[i].textAsset);
            EditorGUI.ObjectField(rect, tableInfoList[i].textAsset, typeof(TextAsset), false);
            //TextAsset asset = EditorGUILayout.ObjectField(tableInfoList[i].textAsset, typeof(TextAsset),false)as TextAsset;
            EditorGUILayout.ObjectField(tableInfoList[i].classScript, typeof(MonoScript), true);
            EditorGUILayout.ObjectField(tableInfoList[i].scriptableObject, typeof(ScriptableObject), true);
            if (GUILayout.Button("检查字段"))
            {
                CheckTextAndClassIsMatch(tableInfoList[i].textAsset, tableInfoList[i].classScript,true);
            }
            if (tableInfoList[i].scriptableObject==null)
            {
                tableInfoList[i].dataState = ysEditorData.DataState.missing;
            }
            string assetBtnLabel = "更新asset";
            switch (tableInfoList[i].dataState)
            {
                case ysEditorData.DataState.missing:
                    assetBtnLabel = "缺失";
                    break;
                case ysEditorData.DataState.Updated:
                    assetBtnLabel = "已更新";
                    break;
                default:
                    break;
            }
            if (GUILayout.Button(assetBtnLabel,GUILayout.Width(100f)))
            {
                tableInfoList[i].dataState = ysEditorData.DataState.Updated;
                if (tableInfoList[i].classScript==null)
                {
                    Debug.LogError("配表未被使用，不予更新。");
                    return;
                }
                if (CheckTextAndClassIsMatch(tableInfoList[i].textAsset, tableInfoList[i].classScript, false))
                {
                    CreateScriptableObject(tableInfoList[i]);
                    AssetDatabase.Refresh();
                    if (tableInfoList[i].scriptableObject==null)
                    {
                        tableInfoList[i].scriptableObject = ReloadAsset(tableInfoList[i]);
                    }
                    else
                    {
                        Debug.LogError("配表存在问题，请检查并修改后重试。");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    void CreateScriptableObject(ysTableBundleData.TableInfo tableInfo)
    {
        func.CreateAssetByTxt(tableInfo);
    }
    ScriptableObject ReloadAsset(ysTableBundleData.TableInfo tableInfo)
    {
        return func.ReloadAsset(tableInfo);
    }
    bool CheckTextAndClassIsMatch(TextAsset text,MonoScript script,bool isDisplay)
    {
       return func.CheckTextAndClassIsMatch(text, script, isDisplay);
    }
    bool CheckAllTextAndClassIsMacth(List<ysTableBundleData.TableInfo> tableInfoList)
    {
        bool isRet = true;
        for (int i = 0; i < tableInfoList.Count; i++)
        {
            isRet &= func.CheckTextAndClassIsMatch(tableInfoList[i].textAsset, tableInfoList[i].classScript, false);
        }
        return isRet;
    }
    void DoCheckAll()
    {
        if (GUILayout.Button("检查配表"))
        {
            if (CheckAllTextAndClassIsMacth(displayTableInfoList))
            {
                EditorUtility.DisplayDialog("检查报告", "检查完毕，可以导出文件。", "继续");
            }
            else
            {
                EditorUtility.DisplayDialog("检查报告", "问题严重,请及时修复！", "继续");
            }
        }
        //string errorInfo = "";
        //for (int i = 0; i < displayTableInfoList.Count; i++)
        //{
        //    if (displayTableInfoList[i].scriptableObject==null)
        //    {
        //        errorInfo += displayTableInfoList[i].tableClassName + " ";
        //    }
        //}
        //Debug.LogError(errorInfo + "");
    }
    void ShowListCount<T>(List<T> list)
    {
        EditorGUILayout.LabelField("当前列表总数:" + list.Count);
    }
    //void CheckScriptObjectHasCorrectClass(MonoScript script, ScriptableObject sptObj)
    //{
    //    func.CheckScriptObjectHasCorrectClass(script, sptObj);
    //}
    void ChangeInputPath(int id)
    {
        if (!string.IsNullOrEmpty(DrawDragbleTextField("输入路径:", id)))
        {
            string tempPath = dragbleTextField[id].text;

            func.data.paths.textPath = tempPath;
        }

        if (GUILayout.Button("改变路径"))
        {
            displayTableInfoList = func.ReadTableInfo();
        }
    }
    bool CheckTextHasClass(TextAsset textAsset, List<ysTableBundleData.TableInfo> tableInfoList)
    {
        bool isRet = false;
        for (int i = 0; i < tableInfoList.Count; i++)
        {
            if (tableInfoList[i].textAsset == textAsset)
            {

                return isRet = !(tableInfoList[i].classScript == null);
            }
        }
        return isRet;
    }

    void GetChosenAsset(Rect rect, TextAsset asset)
    {
        Event current = Event.current;
        if (current.type == EventType.MouseDown)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                selectedTextAsset = asset;
            }
        }
    }

    bool isExistClass = false;
    TextAsset selectedTextAsset = null;
    void CreateClass()
    {
        if (selectedTextAsset != null)
        {
            if (AssetDatabase.GetAssetPath(selectedTextAsset).Contains(".txt"))
            {
                inputText = selectedTextAsset;
                isExistClass = CheckTextHasClass(inputText, displayTableInfoList);
                window.Repaint();
            }
        }
        //Selection.selectionChanged += () =>
        //  {
              
        //      if (Selection.activeObject != null)
        //      {
        //          if (AssetDatabase.GetAssetPath(Selection.activeObject).Contains(".txt"))
        //          {
        //              inputText = (TextAsset)Selection.activeObject;
        //              isExistClass = CheckTextHasClass(inputText, displayTableInfoList);
        //              window.Repaint();
        //          }
        //      }
        //  };

        EditorGUILayout.LabelField("拖入文本:",GUILayout.Width(60f));
        inputText = (TextAsset)EditorGUILayout.ObjectField(inputText, typeof(TextAsset), false);

        string btnLabel = isExistClass?"更新字段":"创建类";
        if (GUILayout.Button(btnLabel))
        {
            string name = func.GenerateClassTableName(inputText.name);
            bool isContinue = true;
            if (File.Exists(func.data.paths.classSccriptPath + name + ".cs"))
            {
                isContinue = EditorUtility.DisplayDialog("警告", "该脚本已存在，是否覆盖", "覆盖", "取消");      
            }
            if (!isContinue)
            {
                return;
            }
            //else
            //{
            //    File.Delete(func.data.paths.classSccriptPath + name + ".cs");
            //}
            func.CreateTableClass(name, inputText.text);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<MonoScript>(func.data.assetDirectoryPaths.classSccriptAssetDirectoryPath+name+".cs");
            Debug.Log(name+" 类创建完成...");
            window.Close();
        }
    }
    void ExportAllAssetToBundle()
    {
        if (GUILayout.Button("导出文件"))
        {
            if (CheckAllTextAndClassIsMacth(displayTableInfoList))
            {
                func.ExportScriptableObjects(func.scriptableObjectList, func.data.buildTarget, func.data.bundleName, func.data.outputDirectoryPath);
                //EditorUtility.DisplayDialog("提示", "导出成功！", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("警告", "导出失败！请使用检查功能或者查看输出信息。", "确定");
            }
        }
    }
}
public class ysTableBundleWarningBox : ysEditorWindow
{
    public static string windowTitle = "defaultName";
    public static void ShowWindow()
    { 
        window = (ysTableBundleWarningBox)GetWindow(typeof(ysTableBundleWarningBox), true, windowTitle);
    }
    public static void ShowFieldInfos(List<string> textField, List<string> scriptField)
    {
        textFieldInfos = textField;
        scriptFieldInfos = scriptField;
        canDisplayFieldInfos = true;
        windowTitle = "字段对比";
        ShowWindow();
    }
    static bool canDisplayFieldInfos = false;
    static List<string> textFieldInfos = new List<string>();
    static List<string> scriptFieldInfos = new List<string>();
    void OnGUI()
    {
        if (canDisplayFieldInfos)
        {
            DrawScrollView(() =>
            {
                DrawHorizental(() =>
                {
                    DisplayFieldInfos(textFieldInfos, "文本中字段");
                    DisplayFieldInfos(scriptFieldInfos, "类中字段");
                });
            }, 0);
        }
    }
    void DisplayFieldInfos(List<string> infos,string title)
    {
        GUILayoutOption option = GUILayout.Width(100);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(title, option);
        for (int i = 0; i < infos.Count; i++)
        {
            EditorGUILayout.LabelField(infos[i], option);
        }
        EditorGUILayout.EndVertical();
    }
}