using UnityEngine;using System.Collections;using System.Collections.Generic;
public class ProgressTable: ScriptableObject
{
	public List <ProgressTableData> table = new List<ProgressTableData>() ;

}
[System.Serializable]
public class ProgressTableData
{
	public int id;
	public string name;
	public string icon;
	public int time;
}
