using UnityEngine;using System.Collections;using System.Collections.Generic;
public class PveTable: ScriptableObject
{
	public List <PveTableData> table = new List<PveTableData>() ;

}
[System.Serializable]
public class PveTableData
{
	public int xulie;
	public int[] scene;
	public int type;
	public int[] people_number;
	public int[] startX;
	public int[] startY;
	public int[] startZ;
	public int[] rotationW;
	public int[] type_win;
	public int[] win_value;
	public int time;
	public int npc_info;
	public int[] reward;
}
