using UnityEngine;using System.Collections;using System.Collections.Generic;
public class TaskDuihuaTable: ScriptableObject
{
	public List <TaskDuihuaTableData> table = new List<TaskDuihuaTableData>() ;

}
[System.Serializable]
public class TaskDuihuaTableData
{
	public int id;
	public string name;
	public string people_now;
	public string level;
	public string yuyin;
}
