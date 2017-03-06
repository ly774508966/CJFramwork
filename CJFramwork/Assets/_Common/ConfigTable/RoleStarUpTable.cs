using UnityEngine;using System.Collections;using System.Collections.Generic;
public class RoleStarUpTable: ScriptableObject
{
	public List <RoleStarUpTableData> table = new List<RoleStarUpTableData>() ;

}
[System.Serializable]
public class RoleStarUpTableData
{
	public int id;
	public int aim_star;
	public int role_yuqidao_id;
	public int cost_star_claim;
	public int cost_number;
	public int cost_gold;
}
