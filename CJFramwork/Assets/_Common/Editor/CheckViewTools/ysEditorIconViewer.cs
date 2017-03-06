using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class ysEditorIconViewer  : EditorWindow
{
    static string[] text;
    [MenuItem("Tools/GUI图标查看器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ysEditorIconViewer));

       // TextAsset data = Resources.Load<TextAsset>("t.txt");//Assets/_Editors/BundleEditor/Resources/

        TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/_Common/Editor/CheckViewTools/iconNames.txt");
        text = data.ToString().Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries);
    }
    public Vector2 scrollPosition;
    void OnGUI()
    {

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //鼠标放在按钮上的样式
        foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
        {
            GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
            GUILayout.Space(10);
        }

        //内置图标
        if (text==null||text.Length<1)
        {
            return;
        }
        for (int i = 0; i < text.Length; i += 8)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 8; j++)
            {
                int index = i + j;
                if (index < text.Length)
                {
                   // EditorGUILayout.BeginVertical();
                    GUILayout.Button(EditorGUIUtility.IconContent(text[index].TrimEnd('\r')), GUILayout.Width(50), GUILayout.Height(30));
                    //EditorGUILayout.SelectableLabel(text[index]);
                    //EditorGUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}
