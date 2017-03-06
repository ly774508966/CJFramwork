using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MonsterRefreshGroupTable: ScriptableObject {
	public List<MonsterRefreshGroupData> sc_ruyuncun_group = new List<MonsterRefreshGroupData>(); 
    public List<MonsterRefreshGroupData> sc_jiuqvgoushandao_group = new List<MonsterRefreshGroupData>();
}
[System.Serializable]
public class MonsterRefreshGroupData
{
	public int ID;                    			
	public int ActivateCondition;               
	public int[] ActivateParam;                   
	public int LoopCount;                      
	public int FirstWaveNum;                    
	public int NextConditionType;                   
	public int[] NextParam;
	public int[] MonsterIDList;

	public int PositionSelectType;
	public int PositionListID;          		                 
	public string s_PositionList;
	private Vector3[][] PositionList_ = null ;    
	public Vector3[][] PositionList{get{if(PositionList_==null)PositionList_=ysDataFormater.ReadVector3Array2D(s_PositionList);return PositionList_;}set{}}            
	public string s_RotationList;				
	private Vector3[][] RotationList_ = null ;          
	public Vector3[][] RotationList{get{if(RotationList_==null)RotationList_=ysDataFormater.ReadVector3Array2D(s_RotationList);return RotationList_;}set{}}

	public List<MonsterConfigObjectData> monsterConfigObjectData = new List<MonsterConfigObjectData>();

}
[System.Serializable]
public class MonsterConfigObjectData  {

	public int id;
	public int groupId;
	public int[] npcId;
	public int[] npcRate;
	public int npcType;
	public int deadRoam;
	public int[] hateGroup;
	public int[] hateValue;
	public int lockRotation;

}