#if UNITY_EDITOR
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GoogleSheetReder : EditorWindow
{
    private static readonly string ClientName = "user";
    private static readonly string dataPath = $"{UnityEngine.Application.dataPath.Replace("Assets", "")}client_secret_633291316510-ao56irbicvfhrm2m9n1k0scean980ufl.apps.googleusercontent.com.json";

    public static SheetsService CreateService()
    {
        var scopes = new string[] { SheetsService.Scope.SpreadsheetsReadonly };

        Debug.Log(dataPath);
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //pass, 
            GoogleClientSecrets.FromFile($"{dataPath}").Secrets,
            scopes,
            ClientName,
            CancellationToken.None).Result;

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
        });

        return service;
    }

    public static void TestCSV02(UnityAction<IList<IList<object>>> callback, string ID, string Name)
    {
        CreateService_CSVObject(CreateService(), callback, Name, ID);
    }

    private static void CreateService_CSVObject(SheetsService service, UnityAction<IList<IList<object>>> callback, string ID, string Name)
    {
        ValueRange value;
        try
        {
            value = service.Spreadsheets.Values.Get(Name, ID).Execute();
        }
        catch (Exception ex)
        {
            Debug.LogError($"{Name} request Error : {ex.Message}");
            return;
        }

        callback.Invoke(value.Values);
    }

    private static string CreateService_CSV(SheetsService service, string sheetName, string spreadSheetID)
    {
        ValueRange value;
        try
        {
            value = service.Spreadsheets.Values.Get(spreadSheetID, sheetName).Execute();
        }
        catch (Exception ex)
        {
            Debug.LogError($"{sheetName} request Error : {ex.Message}");
            return "";
        }

        var values = value.Values;
        int columnCount = values[0].Count;
        List<int> ignoreIndex = new();
        for (int i = 0; i < values[0].Count; i++)
        {
            string title = values[0][i].ToString();
            if (string.IsNullOrEmpty(values[1][i].ToString()))
            {
                continue;
            }
            if (title.IndexOf("(") != -1 || title.IndexOf(")") != -1 ||
                title.IndexOf("[") != -1 || title.IndexOf("]") != -1)
            {
                ignoreIndex.Add(i);
            }
        }

        StringBuilder csvInfo = new StringBuilder();
        for (int x = 1; x < values.Count; x++)
        {
            if (x != 1)
                csvInfo.AppendLine();
            for (int y = 0; y < columnCount; y++)
            {
                if (ignoreIndex.Exists(i => i == y)) continue;

                if (y != 0)
                {
                    csvInfo.Append(",");
                }
                if (values[x].Count <= y)
                    continue;

                string str = values[x][y].ToString();

                if (str.IndexOf(",") != -1)
                {
                    csvInfo.Append("\"");
                    csvInfo.Append(str);
                    csvInfo.Append("\"");
                }
                else
                {
                    csvInfo.Append(str);
                }
            }
        }
        Debug.Log(csvInfo);
        return csvInfo.ToString();
    }

    #region Window

    private static string CSVSettingPath = $"{UnityEngine.Application.dataPath}/GoogleSheet/Editor/CSVSettingJson.json";
    private static string CSVSavePath = $"{UnityEngine.Application.dataPath}/GoogleSheet/CSVData/{{0}}.csv";
    private static List<CSVData> CSVDataList = new();
    private static List<bool> toggleList = new();
    private Vector2 scrollPos = Vector2.zero;
    [Serializable]
    public class CSVData
    {
        public string Name;
        public string SheetName;
        public string SheetID;
    }

    [MenuItem("Tools/CSV Loader")]
    public static void ShowMyEditor()
    {
        LoadSetting();
        EditorWindow wnd = GetWindow<GoogleSheetReder>();
        wnd.titleContent = new GUIContent("Google Sheet Loader");
    }

    void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < CSVDataList.Count; i++)
        {
            toggleList[i] = EditorGUILayout.BeginToggleGroup($"{CSVDataList[i].Name}" , toggleList[i]);
            GUILayout.Space(1);
            CSVDataList[i].Name = EditorGUILayout.TextField("Name", CSVDataList[i].Name);
            CSVDataList[i].SheetID = EditorGUILayout.TextField("Sheet ID", CSVDataList[i].SheetID);
            CSVDataList[i].SheetName = EditorGUILayout.TextField("Sheet Name", CSVDataList[i].SheetName);
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("Remove"))
            {
                Remove(i);
                SaveSetting();
            }

        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add")) 
        {
            AddData();
            SaveSetting();
        }

        if (GUILayout.Button("ConvartCSV"))
        {
            SheetsService service = CreateService();
            for (int i = 0; i < CSVDataList.Count; i++)
            {
                if (toggleList[i])
                {
                    string csvData = CreateService_CSV(service, 
                        CSVDataList[i].SheetName, 
                        CSVDataList[i].SheetID);

                    File.WriteAllText(string.Format(CSVSavePath, CSVDataList[i].Name), csvData);
                    SaveSetting();
                }
            }
        }


        void AddData()
        {
            CSVDataList.Add(new());
            toggleList.Add(false);
        }

        void Remove(int index)
        {
            CSVDataList.RemoveAt(index);
            toggleList.RemoveAt(index);
        }
    }

    public static void LoadSetting()
    {
        if(!File.Exists(CSVSettingPath))
        {
            string data = "";
            File.WriteAllText(CSVSettingPath, data);
        }

        string Json =  File.ReadAllText(CSVSettingPath);
        CSVDataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CSVData>>(Json);

        if (CSVDataList == null)
        {
            CSVDataList = new();
            return;
        }

        foreach (CSVData data in CSVDataList)
        {
            toggleList.Add(false);
        }
    }

    public static void SaveSetting()
    {
        string data = Newtonsoft.Json.JsonConvert.SerializeObject(CSVDataList);
        File.WriteAllText(CSVSettingPath, data);
    }
    #endregion

}
#endif
