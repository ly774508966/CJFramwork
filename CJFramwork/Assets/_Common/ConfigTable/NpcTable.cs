using UnityEngine;using System.Collections;using System.Collections.Generic;
public class NpcTable: ScriptableObject
{
	public List <NpcTableData> table = new List<NpcTableData>() ;

}
[System.Serializable]
public class NpcTableData
{
	public int id;
	public string npc_name;
	public string npc_image;
	public string npc_title;
	public string npc_model;
	public int[] npc_menu;
	public string npc_talk;
	public string sound;
	public int show;
	public int[] coordinate;
	public int npc_citymap;
	public int startX;
	public int startY;
	public int startZ;
	public int rotationY;
}
