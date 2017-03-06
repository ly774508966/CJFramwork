using UnityEngine;using System.Collections;using System.Collections.Generic;
public class MonsterTable: ScriptableObject
{
	public List <MonsterTableData> table = new List<MonsterTableData>() ;

}
[System.Serializable]
public class MonsterTableData
{
	public int id;
	public int type;
	public int role_properties_id;
	public string title;
	public int dynamic_ability;
	public int life;
	public int atk;
	public int armor;
	public int crit;
	public int dodge;
	public int criticalratio;
	public int move;
	public int super_armor;
	public int lifebars;
	public int sight_scope;
}
