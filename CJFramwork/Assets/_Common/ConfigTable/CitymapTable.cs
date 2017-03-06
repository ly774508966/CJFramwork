using UnityEngine;using System.Collections;using System.Collections.Generic;
public class CitymapTable: ScriptableObject
{
	public List <CitymapTableData> table = new List<CitymapTableData>() ;

}
[System.Serializable]
public class CitymapTableData
{
	public int id;
	public string map_name;
	public string map_image;
	public int map_scene;
	public int[] startX;
	public int[] startY;
	public int[] startZ;
	public int[] rotationY;
}
