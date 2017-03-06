using UnityEngine;using System.Collections;using System.Collections.Generic;
public class TaskGroupTable: ScriptableObject
{
	public List <TaskGroupTableData> table = new List<TaskGroupTableData>() ;

}
[System.Serializable]
public class TaskGroupTableData
{
	public int id;
	public int type;
	public int task;
}
