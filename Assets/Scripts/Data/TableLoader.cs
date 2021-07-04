using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLoader : MonoBehaviour
{
    private const string sheetURL =
        "https://docs.google.com/spreadsheets/d/1J197-uPXrTqBckx4qJGQ0_KAHuiZ5XPPgq7-2LgwOaQ/export?format=tsv&gid=";
    private readonly Dictionary<TableType, string> gidDict = new Dictionary<TableType, string>()
    {
        {TableType.Stage, "0"},
        {TableType.Wave, "1388325185"},
        {TableType.Character, "1303884673"},
        {TableType.Status, "451083748"},
    };

    private bool _isFinished;
    public bool IsFinished => _isFinished;

    public void LoadTable()
    {
        _isFinished = false;
        StartCoroutine(LoadTable_C());
    }

    private IEnumerator LoadTable_C()
    {
        var iter = gidDict.GetEnumerator();
        var _tableDataDict = new Dictionary<TableType, List<TableData>>();
        while (iter.MoveNext())
        {
            WWW www = new WWW(string.Concat(sheetURL, iter.Current.Value));
            yield return www;

            while (www.isDone == false)
                yield return null;

            if (string.IsNullOrEmpty(www.error))
            {
                var content = www.text;
                var lines = content.Split('\n');
                for (int i = 1; i < lines.Length; ++i)
                {
                    var tableData = CreateTableData(iter.Current.Key);
                    tableData.ParsingData(lines[i].Trim());

                    if (!_tableDataDict.ContainsKey(iter.Current.Key))
                        _tableDataDict[iter.Current.Key] = new List<TableData>();
                    _tableDataDict[iter.Current.Key].Add(tableData);
                }
            }
            else
            {
                DebugEx.LogError($"[Failed] table load error: {www.error}");
                yield break;
            }
        }

        DataManager.Instance.SetTableData(_tableDataDict);
        _isFinished = true;
    }

    private TableData CreateTableData(TableType type)
    {
        switch (type)
        {
            case TableType.Stage:
                return new StageTable();
            case TableType.Character:
                return new CharacterTable();
            case TableType.Wave:
                return new WaveTable();
            case TableType.Status:
                return new StatusTable();
        }
        return null;
    }
}
