using static BigHead.Csv.Core.CsvGenerateType;
using UnityEngine;
using System.Collections.Generic;
using System;


public static partial class CsvAssistant
{
    private static CurveCsv _instanceCurveCsv { get; set;}

    public static CurveCsv GetCurveCsv()
    {
        if(Equals(null, _instanceCurveCsv)) _instanceCurveCsv = CsvFunctions.GetCsv<CurveCsv>("CurveCsv");
        return _instanceCurveCsv;
    }
}

public class CurveCsv : BasicCsv
{
    protected Dictionary<string, CurveRow> _dict = new Dictionary<string, CurveRow>();
    protected override string Path { get; set;} = "CsvData/ConstCsv/Curve.csv";

    protected override void AnalysisCsv(List<string> list, Action callback)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i].Split(',');
            var csvClass = new CurveRow();
            csvClass.ID = ToInt(item[0]);
            csvClass.Desc = item[1];
            csvClass.Frames = ToStringArray(item[2]);
            CustomerRowHandler(item[0], csvClass);
        }
        callback?.Invoke();
    }
    public CurveRow GetRowByKey(string key)
    {
        _dict.TryGetValue(key, out var value);
        return value;
    }
    public CurveRow GetRowByKey(int id)
    {
        var key = id.ToString();
        _dict.TryGetValue(key, out var value);
        return value;
    }
    public virtual void CustomerRowHandler(string key, CurveRow row)
    {
        _dict.Add(key, row);
    }
}