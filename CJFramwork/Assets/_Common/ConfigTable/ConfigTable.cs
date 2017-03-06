using UnityEngine;using System.Collections;using System.Collections.Generic;
public class ConfigTable: ScriptableObject
{
	public List <ConfigTableData> table = new List<ConfigTableData>() ;

}
[System.Serializable]
public class ConfigTableData
{
	public int id;
	public string name;
	public string format;
	public string value;
}
