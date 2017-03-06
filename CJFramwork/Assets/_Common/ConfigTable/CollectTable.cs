using UnityEngine;using System.Collections;using System.Collections.Generic;
public class CollectTable: ScriptableObject
{
	public List <CollectTableData> table = new List<CollectTableData>() ;

}
[System.Serializable]
public class CollectTableData
{
	public int id;
	public string name;
	public string model;
	public int show;
	public int number;
	public int[] coordinate;
	public int citymap;
	public int startX;
	public int startY;
	public int startZ;
	public int[] rotationY;
	public int progress;
	public int[] reward;
}
