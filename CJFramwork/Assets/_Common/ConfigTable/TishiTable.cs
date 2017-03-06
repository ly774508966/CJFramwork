using UnityEngine;using System.Collections;using System.Collections.Generic;
public class TishiTable: ScriptableObject
{
	public List <TishiTableData> table = new List<TishiTableData>() ;

}
[System.Serializable]
public class TishiTableData
{
	public int id;
	public string bianma;
	public int type;
	public string npc;
	public string image;
	public string text;
}
