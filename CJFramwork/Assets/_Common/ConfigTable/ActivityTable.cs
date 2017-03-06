using UnityEngine;using System.Collections;using System.Collections.Generic;
public class ActivityTable: ScriptableObject
{
	public List <ActivityTableData> table = new List<ActivityTableData>() ;

}
[System.Serializable]
public class ActivityTableData
{
	public int id;
	public string function;
	public string name;
	public int table_page;
	public string table_text;
	public int level;
	public string icon;
	public string jiaobiao;
	public int[] weeks;
	public int[] reset;
	public int number;
	public int active;
	public string s_time_open;
	private int[][]time_open_=null;
	public int[][] time_open{get{if(time_open_==null)time_open_=ysDataFormater.ReadInt32Array2D(s_time_open);return time_open_;}set{}}
	public string xianshi;
	public string miaoshu;
	public int[] reward;
	public int kuaijie_type;
	public string kuaijie_value;
	public int team_table;
	public string team_table_name;
	public int tuisong;
	public string tuisong_shuoming;
}
