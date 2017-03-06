using UnityEngine;using System.Collections;using System.Collections.Generic;
public class RewardTable: ScriptableObject
{
	public List <RewardTableData> table = new List<RewardTableData>() ;

}
[System.Serializable]
public class RewardTableData
{
	public int id;
	public int reward_dynamic;
	public string s_reward;
	private int[][]reward_=null;
	public int[][] reward{get{if(reward_==null)reward_=ysDataFormater.ReadInt32Array2D(s_reward);return reward_;}set{}}
}
