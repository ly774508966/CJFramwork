using UnityEngine;using System.Collections;using System.Collections.Generic;
public class MultiLanguageTable: ScriptableObject
{
	public List <MultiLanguageTableData> table = new List<MultiLanguageTableData>() ;

}
[System.Serializable]
public class MultiLanguageTableData
{
	public int id;
	public string bianma;
	public string Simplified;
}
