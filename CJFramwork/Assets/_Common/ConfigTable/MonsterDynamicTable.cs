using UnityEngine;using System.Collections;using System.Collections.Generic;
public class MonsterDynamicTable: ScriptableObject
{
	public List <MonsterDynamicTableData> table = new List<MonsterDynamicTableData>() ;

}
[System.Serializable]
public class MonsterDynamicTableData
{
	public int id;
	public int tyoe;
	public int level;
	public int hp_value;
	public int gj_value;
	public int hj_value;
}
