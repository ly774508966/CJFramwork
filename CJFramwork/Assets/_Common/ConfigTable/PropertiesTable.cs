using UnityEngine;using System.Collections;using System.Collections.Generic;
public class PropertiesTable: ScriptableObject
{
	public List <PropertiesTableData> table = new List<PropertiesTableData>() ;

}
[System.Serializable]
public class PropertiesTableData
{
	public int id;
	public int type;
	public string name;
	public float fight;
	public string describe;
}
