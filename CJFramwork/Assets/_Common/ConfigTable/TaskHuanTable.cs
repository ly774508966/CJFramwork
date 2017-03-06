using UnityEngine;using System.Collections;using System.Collections.Generic;
public class TaskHuanTable: ScriptableObject
{
	public List <TaskHuanTableData> table = new List<TaskHuanTableData>() ;

}
[System.Serializable]
public class TaskHuanTableData
{
	public int id;
	public int type;
	public int huan;
	public int[] task_group;
	public int[] target_odds;
	public int[] reward;
	public int[] reward_lead;
}
