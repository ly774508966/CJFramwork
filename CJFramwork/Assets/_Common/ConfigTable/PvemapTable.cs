using UnityEngine;using System.Collections;using System.Collections.Generic;
public class PvemapTable: ScriptableObject
{
	public List <PvemapTableData> table = new List<PvemapTableData>() ;

}
[System.Serializable]
public class PvemapTableData
{
	public int id;
	public int[] scene;
	public int type;
	public int[] people_number;
	public int[] startX;
	public int[] startY;
	public int[] startZ;
	public int[] rotationY;
	public int[] type_win;
	public int[] win_value;
	public int time;
	public int[] reward;
	public string talk_qian;
	public string talk_hou;
	public int[] scene_config_id;
}
