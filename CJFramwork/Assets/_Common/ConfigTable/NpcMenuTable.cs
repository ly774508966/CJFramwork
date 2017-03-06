using UnityEngine;using System.Collections;using System.Collections.Generic;
public class NpcMenuTable: ScriptableObject
{
	public List <NpcMenuTableData> table = new List<NpcMenuTableData>() ;

}
[System.Serializable]
public class NpcMenuTableData
{
	public int id;
	public string name;
	public int type;
	public string value01;
	public string value02;
}
