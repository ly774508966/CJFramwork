using UnityEngine;using System.Collections;using System.Collections.Generic;
public class RoleYuqidaoTable: ScriptableObject
{
	public List <RoleYuqidaoTableData> table = new List<RoleYuqidaoTableData>() ;

}
[System.Serializable]
public class RoleYuqidaoTableData
{
	public int id;
	public int layer;
	public int stage;
	public int star;
	public int exp;
	public int sum_exp;
	public int[] properties_id;
	public int[] properties_value;
	public int yuqidao_exp;
}
