using UnityEngine;using System.Collections;using System.Collections.Generic;
public class TaskTable: ScriptableObject
{
	public List <TaskTableData> table = new List<TaskTableData>() ;

}
[System.Serializable]
public class TaskTableData
{
	public int id;
	public int paixu;
	public int type;
	public int level;
	public string name;
	public string target;
	public int auto_qishi;
	public int type_qishi;
	public int[] type_qishi_value;
	public int progress;
	public int[] task_duihua;
	public int type_target;
	public int type_target_value;
	public string[] icon;
	public int[] reward;
	public int[] next_task;
}
