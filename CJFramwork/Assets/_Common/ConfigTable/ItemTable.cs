using UnityEngine;using System.Collections;using System.Collections.Generic;
public class ItemTable: ScriptableObject
{
	public List <ItemTableData> table = new List<ItemTableData>() ;

}
[System.Serializable]
public class ItemTableData
{
	public int id;
	public string name;
	public string miaoshu;
	public string icon;
	public int bigtype;
	public int smalltype;
	public int card_star;
	public int yuqidao_exp;
}
