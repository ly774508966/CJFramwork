using UnityEngine;
using System.Collections;
using System;
public static class ysDataFormater  
{
	//读取三维矢量
	public static Vector3 ReadVector3(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return Vector3.zero;
		}
		string[] vectStr =  _cellValue.ToString().Trim('(',')').Split(',');
		float posX = float.Parse(vectStr[0].Trim());
		float posY = float.Parse(vectStr[1].Trim());
		float posZ = float.Parse(vectStr[2].Trim());
		return new Vector3 (posX,posY,posZ);
	}

	//读取整数
	public static int ReadInt32(object _cellValue)
	{
		return (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r")?0:System.Convert.ToInt32(_cellValue);
	}

	//读取浮点
	public static float ReadFloat(object _cellValue)
	{
		return (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r")? 0f:System.Convert.ToSingle(_cellValue); 
	}

	//读取字符串
	public static string ReadString(object _cellValue)
	{		
		return (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r")? "": _cellValue.ToString();
	}

	static char ArraySplitSymbol = '|';
	static char Array2DSplitSymbol = '#';

	//读取整数
	public static int[] ReadInt32Array(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new int[]{0};
		}
		string[] str = _cellValue.ToString().Split(ArraySplitSymbol);
		int[] retArray = new int[str.Length];
		for (int i = 0; i < str.Length; i++) {
			retArray[i] = Convert.ToInt32(str[i]);
		}
		return retArray;
	}

	//读取浮点
	public static float[] ReadFloatArray(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new float[] {0f};
		}
		string[] str = _cellValue.ToString().Split(ArraySplitSymbol);
		float[] retArray = new float[str.Length];
		for (int i = 0; i < str.Length; i++) {
			retArray[i] = Convert.ToSingle(str[i]);
		}
		return retArray;
	}

	//读取字符串
	public static string[] ReadStringArray(object _cellValue)
	{		
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new string[] {""};
		}
		string[] str = _cellValue.ToString().Split(ArraySplitSymbol);
		string[] retArray = new string[str.Length];
		for (int i = 0; i < str.Length; i++) {
			retArray[i] = (str[i]);
		}
		return retArray;
	}
	public static Vector3[] ReadVector3Array(object _cellValue)
	{		
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new Vector3[] {Vector3.zero};
		}
		string[] str = _cellValue.ToString().Split(ArraySplitSymbol);
		Vector3[] retArray = new Vector3[str.Length];
		for (int i = 0; i < str.Length; i++) {
			retArray[i] = ReadVector3(str[i]);
		}
		return retArray;
	}

	//读取整数
	public static int[][] ReadInt32Array2D(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new int[1][]{new int[]{0}};
		}
		string[] strArray = _cellValue.ToString().Split(Array2DSplitSymbol);
		int[][] retArray2D = new int[strArray.Length][];
		for (int i = 0; i < strArray.Length; i++) {
			string[] temp = strArray[i].Split(ArraySplitSymbol);
			retArray2D[i] = new int[temp.Length] ;
			for (int j = 0; j < temp.Length; j++) {
				retArray2D[i][j] = Convert.ToInt32( temp[j]);
			}
		}
		return retArray2D;
	}

	//读取浮点
	public static float[][] ReadFloatArray2D(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new float[1][]{new float[]{0f}};
		}
		string[] strArray = _cellValue.ToString().Split(Array2DSplitSymbol);
		float[][] retArray2D = new float[strArray.Length][];

		for (int i = 0; i < strArray.Length; i++) {
			string[] temp = strArray[i].Split(ArraySplitSymbol);
			retArray2D[i] = new float[temp.Length] ;
			for (int j = 0; j < temp[i].Length; j++) {
				retArray2D[i][j] = Convert.ToSingle( temp[j]);
			}
		}
		return retArray2D;
	}

	//读取字符串
	public static string[][] ReadStringArray2D(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new string[1][]{new string[]{""}};
		}
		string[] strArray = _cellValue.ToString().Split(Array2DSplitSymbol);
		string[][] retArray2D = new string[strArray.Length][];
		for (int i = 0; i < strArray.Length; i++) {
			string[] temp = strArray[i].Split(ArraySplitSymbol);
			retArray2D[i] = new string[temp.Length] ;
			for (int j = 0; j < temp[i].Length; j++) {
				retArray2D[i][j] = (temp[j]);
			}
		}
		return retArray2D;
	}
	public static Vector3[][] ReadVector3Array2D(object _cellValue)
	{
		if (_cellValue==null||_cellValue.ToString()==""||_cellValue.ToString()=="\r") {
			return new Vector3[1][]{new Vector3[]{Vector3.zero}};
		}
		string[] strArray = _cellValue.ToString().Split(Array2DSplitSymbol);
		Vector3[][] retArray2D = new Vector3[strArray.Length][];
		for (int i = 0; i < strArray.Length; i++) {
			
			string[] temp = strArray[i].Split(ArraySplitSymbol);
			retArray2D[i] = new Vector3[temp.Length] ;
			for (int j = 0; j < temp.Length; j++) {
				retArray2D[i][j] = ReadVector3(temp[j]);
			}
		}
		return retArray2D;
	}

}
