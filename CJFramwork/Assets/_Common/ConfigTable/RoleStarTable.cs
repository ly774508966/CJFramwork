using UnityEngine;using System.Collections;using System.Collections.Generic;
public class RoleStarTable: ScriptableObject
{
	public List <RoleStarTableData> table = new List<RoleStarTableData>() ;

}
[System.Serializable]
public class RoleStarTableData
{
	public int id;
	public int role_id;
	public int stars;
	public int life_up;
	public int atk_up;
	public int armor_up;
	public int life;
	public int atk;
	public int armor;
	public int crit;
	public int dodge;
	public int critical_ratio;
	public int move;
	public int[] property_evaluate;
}
