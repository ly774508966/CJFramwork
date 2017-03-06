using UnityEngine;using System.Collections;using System.Collections.Generic;
public class BuffTable: ScriptableObject
{
	public List <BuffTableData> table = new List<BuffTableData>() ;

}
[System.Serializable]
public class BuffTableData
{
	public int id;
	public int buff_type;
	public int duration_time;
	public int interval_time;
	public int[] result_id;
	public int[] parameter_1;
	public int[] parameter_2;
	public int[] ratio;
	public string effect;
}
