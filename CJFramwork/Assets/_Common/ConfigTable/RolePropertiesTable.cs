using UnityEngine;using System.Collections;using System.Collections.Generic;
public class RolePropertiesTable: ScriptableObject
{
	public List <RolePropertiesTableData> table = new List<RolePropertiesTableData>() ;

}
[System.Serializable]
public class RolePropertiesTableData
{
	public int id;
	public int type;
	public string name;
	public string mark;
	public int grade;
	public int[] skill;
	public string ui_image;
	public string card_image;
	public string model;
	public string[] sound;
	public int ai;
}
