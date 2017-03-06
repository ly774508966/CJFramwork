using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Babybus.Framework.ExtensionMethods;
public class ysAssetTools : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
public class FindRefrences
{
    [MenuItem("Assets/Find Refrence", false, 1)]
    static void Find()
    {
        EditorSettings.serializationMode = SerializationMode.ForceText;
        string path = AssetDatabase.GetAssetPath(Selection.activeGameObject);
        if (string.IsNullOrEmpty(path)    )
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            List<string> withoutExtrnsions = new List<string>() { ".prefab" };//, ".unity", ".asset", ".mat" };
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(
                s => withoutExtrnsions.Contains(Path.GetExtension(s).ToLower())).ToArray();
            int startIndex = 0;

            EditorApplication.update = delegate ()
            {
                string file = files[startIndex];
                bool isCancel = EditorUtility.DisplayCancelableProgressBar("资源匹配中", file, (float)startIndex / (float)file.Length);
                if (Regex.IsMatch(File.ReadAllText(file), guid))
                {
                    Debug.LogError(file, AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GetRelativeAssetsPath(file)));
                }
                startIndex++;
                if (isCancel||startIndex>=file.Length    )
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                    startIndex = 0;
                    Debug.LogError("匹配结束");
                }
            };  
        }

    }
    [MenuItem("Assets/Find Refrence", true)]
    static bool CanFind()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        return (!string.IsNullOrEmpty(path));
    }




    static string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }


}


namespace Babybus.Framework.ExtensionMethods
{
    public static class TransformExtension
    {
        public static int GetActiveChildCount(this Transform transform)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    count++;
            }

            return count;
        }

        public static T[] GetComponentsInFirstHierarchyChildren<T>(this Transform transform, bool includeInactive = true) where T : Component
        {
            List<T> components = new List<T>();
            foreach (Transform child in transform)
            {
                if (!includeInactive && !child.gameObject.activeSelf)
                    continue;

                T component = child.GetComponent<T>();
                if (component != null)
                    components.Add(component);
            }

            return components.ToArray();
        }

        public static void SetX(this Transform transform, float x)
        {
            Vector3 newPosition = new Vector3(x, transform.position.y, transform.position.z);

            transform.position = newPosition;
        }

        public static void SetY(this Transform transform, float y)
        {
            Vector3 newPosition = new Vector3(transform.position.x, y, transform.position.z);

            transform.position = newPosition;
        }

        public static void SetZ(this Transform transform, float z)
        {
            Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, z);

            transform.position = newPosition;
        }

        public static void SetXY(this Transform transform, Vector3 position)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
        }

        public static void SetXZ(this Transform transform, Vector3 position)
        {
            transform.position = new Vector3(position.x, transform.position.y, position.z);
        }

        public static void SetLocalX(this Transform transform, float x)
        {
            Vector3 newPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);

            transform.localPosition = newPosition;
        }

        public static void SetLocalY(this Transform transform, float y)
        {
            Vector3 newPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);

            transform.localPosition = newPosition;
        }

        public static void SetLocalZ(this Transform transform, float z)
        {
            Vector3 newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);

            transform.localPosition = newPosition;
        }

        public static void SetLocalAngleX(this Transform transform, float x)
        {
            transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        public static void SetLocalAngleY(this Transform transform, float y)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
        }

        public static void SetLocalAngleZ(this Transform transform, float z)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
        }

        public static string GetPath(this Transform transform)
        {
            string path = transform.name;

            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        public static int IndexInParent(this Transform transform)
        {
            if (transform.parent == null)
                return -1;

            for (int i = 0; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                    return i;
            }

            return -1;
        }

        public static void SetActiveInChildren(this Transform transform, bool value)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(value);
            }
        }

        public static void SetLocalScaleX(this Transform transform, float x)
        {
            Vector3 newScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
            transform.localScale = newScale;
        }
        public static void SetLocalScaleY(this Transform transform, float y)
        {
            Vector3 newScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
            transform.localScale = newScale;
        }
        public static void SetLocalScaleZ(this Transform transform, float z)
        {
            Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
            transform.localScale = newScale;
        }

        public static bool EqualsInHierarchy(this Transform transform, Transform other)
        {
            if (transform.childCount != other.childCount)
                return false;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != other.GetChild(i).name)
                    return false;
                else if (!EqualsInHierarchy(transform.GetChild(i), other.GetChild(i)))
                    return false;
            }

            return true;
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
                UnityEngine.Object.Destroy(transform.GetChild(i).gameObject);
        }

        public static T GetComponentInChildrenWithName<T>(this Transform transform, string name) where T : Component
        {
            var array = transform.GetComponentsInChildren<T>();
            return Array.Find(array, (item) =>
            {
                return item.name == name;
            });
        }

        public static void SetChildrenLayer(this Transform transform, Transform transforms, LayerMask layer)
        {
            foreach (Transform childTransform in transform)
            {
                childTransform.gameObject.layer = layer;
                childTransform.SetChildrenLayer(childTransform, layer);
            }
        }
    }
}

