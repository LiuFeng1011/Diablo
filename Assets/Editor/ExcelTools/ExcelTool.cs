using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Excel;
using System.Data;
using LitJson;

public class ExcelTool {



//	void Start () 
//	{		
//		Debuger.EnableLog = true;
//		//ReadConfigFileTest();
//		HandleXLSX();
//	}

	//生成配置表文件
	void HandleXLSX(){
		LoadData("test");
	}


	/// <summary>
	/// 载入一个excel文件 Loads the data.
	/// </summary>
	/// <param name="filename">Filename.</param>
	public static string LoadData(string filename)
	{
		FileStream stream = File.Open(GameConst.GetExcelFilePath(filename), FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
		
		DataSet result = excelReader.AsDataSet();

		string ret = "";
		//处理所有的子表
		for(int i = 0 ; i < result.Tables.Count; i++){
            Debug.Log(result.Tables[i].TableName);
			bool issuccess = HandleATable(result.Tables[i]);
			if(issuccess )
				ret += result.Tables[i].TableName + "\n";
		}
		return ret;
	}

	public static string GetClassNameByName(string tablename){

		if(tablename.Substring(0,6) == "string"){
			return "GameStringConf";
		}
		return tablename;
	}

	/// <summary>
	/// 处理一张表 Handle A table.
	/// </summary>
	/// <param name="result">Result.</param>
	public static bool HandleATable(DataTable result){
		Debug.Log(result.TableName);

		//创建这个类
		Type t = Type.GetType(GetClassNameByName(result.TableName));
		if(t == null){
            
			Debug.Log("the type is null  : " + result.TableName);
			return false;
		}

		int columns = result.Columns.Count;
		int rows = result.Rows.Count;

		//行数从0开始  第0行为注释
		int fieldsRow = 1;//字段名所在行数
		int contentStarRow = 2;//内容起始行数
		
		//获取所有字段
		string[] tableFields = new string[columns];
		
		for(int j =0; j < columns; j++)
		{
			tableFields[j] = result.Rows[fieldsRow][j].ToString();
			//Debuger.Log(tableFields[j]);
		}

		//存储表内容的字典
        List<object> dataList = new List<object>();

		//遍历所有内容
		for(int i = contentStarRow;  i< rows; i++)
		{
            object o = Activator.CreateInstance(t);

			for(int j =0; j < columns; j++)
			{
				System.Reflection.FieldInfo info = o.GetType().GetField(tableFields[j]);

				if(info == null){
					continue;
				}

				string val = result.Rows[i][j].ToString();

				if(info.FieldType ==  typeof(int)){
					info.SetValue(o,int.Parse(val));
				}else if(info.FieldType ==  typeof(float)){
					info.SetValue(o,float.Parse(val));
				}else{
					info.SetValue(o,val);
				}
			}
			//o.toString();

            //Debug.Log(JsonMapper.ToJson(o));
            dataList.Add(o);
		}	

        SaveTableData(dataList,result.TableName + GameConst.CONF_FILE_NAME);
		return true;
	}

	/// <summary>
	/// 把Dictionary序列化为byte数据
	/// Saves the table data.
	/// </summary>
	/// <param name="dic">Dic.</param>
	/// <param name="tablename">Tablename.</param>
    public static void SaveTableData(List<object> list ,string tablename){

        byte[] data = GameCommon.SerializeObject(list);
        byte[] gzipData = GameCommon.CompressGZip(data);
		//WriteByteToFile(gzipData,tablename);
        Debug.Log(GameConst.SaveConfigFilePath(tablename));
		GameCommon.WriteByteToFile(gzipData,GameConst.SaveConfigFilePath(tablename));
	}



}
