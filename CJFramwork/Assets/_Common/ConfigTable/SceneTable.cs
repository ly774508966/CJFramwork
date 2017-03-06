using UnityEngine;using System.Collections;using System.Collections.Generic;
public class SceneTable: ScriptableObject
{
	public List <SceneTableData> table = new List<SceneTableData>() ;

}
[System.Serializable]
public class SceneTableData
{
	public int id;
	public string scene;
	public string sound;
}
