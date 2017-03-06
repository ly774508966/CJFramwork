using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ysWorldPositionTag))]
public class ysWorldPositionEditor : Editor
{
    void OnSceneGUI()
    {
        ysWorldPositionTag scene = target as ysWorldPositionTag;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        Handles.Label(scene.transform.position + scene.offset,
        "位置：" + scene.transform.position.ToString() +
        "\n旋转：" + scene.transform.rotation.ToString(), style);
    }
}
