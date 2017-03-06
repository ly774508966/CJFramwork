using UnityEngine;using System.Collections;using System.Collections.Generic;
public class PlayerInfoTable: ScriptableObject
{
	public List <PlayerInfoTableData> table = new List<PlayerInfoTableData>() ;

}
[System.Serializable]
public class PlayerInfoTableData
{
	public int id;
	public string model;
	public string image;
}
