using UnityEngine;using System.Collections;using System.Collections.Generic;
public class ExpTable: ScriptableObject
{
	public List <ExpTableData> table = new List<ExpTableData>() ;

}
[System.Serializable]
public class ExpTableData
{
	public int id;
	public int level;
	public int exp;
}
