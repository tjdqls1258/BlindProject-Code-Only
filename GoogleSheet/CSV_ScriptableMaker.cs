using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public abstract class ScriptableObject_CSV : ScriptableObject
{
    public abstract string GetName();
}

public class CSV_ScriptableMaker<DATA> where DATA : ScriptableObject_CSV, new()
{
    static string CsvPath = $"{Application.dataPath}/GoogleSheet/Editor/CSVData/{{}}.csv";
    static string scriptableData = $"{Application.dataPath}/GoogleSheet/Editor/Data/";

    public void ChangeData(string csvName)
    {
        File.ReadAllText(string.Format(CsvPath, csvName));
        string[] datas = File.ReadAllLines(CsvPath);
        foreach (string data in datas)
        {
            MakeScriptableData(GetData(data));
        }

        SaveData();
    }

    private void MakeScriptableData(DATA data)
    {
        DATA d = data;
        AssetDatabase.AddObjectToAsset(d, $"{scriptableData}{data.GetName()}.asset");
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(data));
        Debug.Log(AssetDatabase.GetAssetPath(data));
    }

    private void SaveData()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    protected DATA GetData(string str)
    {
        DATA data = new();

        List<string> list = str.Split(',').ToList();
        FieldInfo[] fields = typeof(DATA).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        UnityEngine.Assertions.Assert.IsTrue(list.Count == fields.Length, $"{this.GetType()} Script Field Data Not Match !!!!");

        for (int i = 0; i < fields.Length; ++i)
        {
            FieldInfo field = fields[i];
            Type fieldType = fields[i].FieldType;

            object valueToSet;
            object value = list[i];

            try
            {
                if (fieldType.IsEnum)
                {
                    if (int.TryParse(list[i], out int enumNumber))
                        valueToSet = Enum.ToObject(fieldType, enumNumber);
                    else
                        valueToSet = Enum.Parse(fieldType, list[i]);
                }
                else
                {
                    if (fieldType == typeof(string))//안드로이드 경우 \r가 붙어서 제거.
                        valueToSet = ((string)(Convert.ChangeType(value, fieldType))).Replace("\r", "") ?? string.Empty;
                    else
                        valueToSet = Convert.ChangeType(value, fieldType);
                }

                field.SetValue(data, valueToSet);
            }
            catch (Exception e)
            {
                Debug.LogError("ex : " + e.Message);
                Debug.LogError($"i : {i}, fieldType : {fieldType}, list[i] : {list[i]}");
            }
        }

        return data;
    }
}
