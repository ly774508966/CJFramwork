using UnityEngine;using System.Collections;using System.Collections.Generic;
public class SkillTable: ScriptableObject
{
	public List <SkillTableData> table = new List<SkillTableData>() ;

}
[System.Serializable]
public class SkillTableData
{
	public int id;
	public int skill_id;
	public int type;
	public int lv;
	public int skill_group;
	public string collocate;
	public string name;
	public string icon;
	public string description_id;
	public int[] range;
	public int cool_down;
	public int number;
	public int demage_ratio;
	public int[] demage_weight;
	public int treat_ratio;
	public int[] treat_weight;
	public string s_lv_impact;
	private int[][]lv_impact_=null;
	public int[][] lv_impact{get{if(lv_impact_==null)lv_impact_=ysDataFormater.ReadInt32Array2D(s_lv_impact);return lv_impact_;}set{}}
	public string s_lv_impact_ratio_1;
	private int[][]lv_impact_ratio_1_=null;
	public int[][] lv_impact_ratio_1{get{if(lv_impact_ratio_1_==null)lv_impact_ratio_1_=ysDataFormater.ReadInt32Array2D(s_lv_impact_ratio_1);return lv_impact_ratio_1_;}set{}}
	public string s_lv_impact_ratio_2;
	private int[][]lv_impact_ratio_2_=null;
	public int[][] lv_impact_ratio_2{get{if(lv_impact_ratio_2_==null)lv_impact_ratio_2_=ysDataFormater.ReadInt32Array2D(s_lv_impact_ratio_2);return lv_impact_ratio_2_;}set{}}
	public string s_area_impact_id;
	private int[][]area_impact_id_=null;
	public int[][] area_impact_id{get{if(area_impact_id_==null)area_impact_id_=ysDataFormater.ReadInt32Array2D(s_area_impact_id);return area_impact_id_;}set{}}
	public string s_area_impact_ratio_1;
	private int[][]area_impact_ratio_1_=null;
	public int[][] area_impact_ratio_1{get{if(area_impact_ratio_1_==null)area_impact_ratio_1_=ysDataFormater.ReadInt32Array2D(s_area_impact_ratio_1);return area_impact_ratio_1_;}set{}}
	public string s_area_impact_ratio_2;
	private int[][]area_impact_ratio_2_=null;
	public int[][] area_impact_ratio_2{get{if(area_impact_ratio_2_==null)area_impact_ratio_2_=ysDataFormater.ReadInt32Array2D(s_area_impact_ratio_2);return area_impact_ratio_2_;}set{}}
	public string s_area_bad_id;
	private int[][]area_bad_id_=null;
	public int[][] area_bad_id{get{if(area_bad_id_==null)area_bad_id_=ysDataFormater.ReadInt32Array2D(s_area_bad_id);return area_bad_id_;}set{}}
	public string s_area_bad_ratio_1;
	private int[][]area_bad_ratio_1_=null;
	public int[][] area_bad_ratio_1{get{if(area_bad_ratio_1_==null)area_bad_ratio_1_=ysDataFormater.ReadInt32Array2D(s_area_bad_ratio_1);return area_bad_ratio_1_;}set{}}
	public string s_area_bad_ratio_2;
	private int[][]area_bad_ratio_2_=null;
	public int[][] area_bad_ratio_2{get{if(area_bad_ratio_2_==null)area_bad_ratio_2_=ysDataFormater.ReadInt32Array2D(s_area_bad_ratio_2);return area_bad_ratio_2_;}set{}}
	public string s_buff_self_probability;
	private int[][]buff_self_probability_=null;
	public int[][] buff_self_probability{get{if(buff_self_probability_==null)buff_self_probability_=ysDataFormater.ReadInt32Array2D(s_buff_self_probability);return buff_self_probability_;}set{}}
	public string s_buff_id_self;
	private int[][]buff_id_self_=null;
	public int[][] buff_id_self{get{if(buff_id_self_==null)buff_id_self_=ysDataFormater.ReadInt32Array2D(s_buff_id_self);return buff_id_self_;}set{}}
	public string s_buff_all_probability;
	private int[][]buff_all_probability_=null;
	public int[][] buff_all_probability{get{if(buff_all_probability_==null)buff_all_probability_=ysDataFormater.ReadInt32Array2D(s_buff_all_probability);return buff_all_probability_;}set{}}
	public string s_buff_id_all;
	private int[][]buff_id_all_=null;
	public int[][] buff_id_all{get{if(buff_id_all_==null)buff_id_all_=ysDataFormater.ReadInt32Array2D(s_buff_id_all);return buff_id_all_;}set{}}
	public string s_debuff_all_probability;
	private int[][]debuff_all_probability_=null;
	public int[][] debuff_all_probability{get{if(debuff_all_probability_==null)debuff_all_probability_=ysDataFormater.ReadInt32Array2D(s_debuff_all_probability);return debuff_all_probability_;}set{}}
	public string s_debuff_id_all;
	private int[][]debuff_id_all_=null;
	public int[][] debuff_id_all{get{if(debuff_id_all_==null)debuff_id_all_=ysDataFormater.ReadInt32Array2D(s_debuff_id_all);return debuff_id_all_;}set{}}
}
