using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using UnityEngine;

public class CurveCsvPlus : CurveCsv
{
    private readonly Dictionary<string, AnimationCurve> _curves = new Dictionary<string, AnimationCurve>();
    
    public override void CustomerRowHandler(string key, CurveRow row)
    {
        base.CustomerRowHandler(key, row);

        var curve = new AnimationCurve();
        for (int i = 0; i < row.Frames.Length; i++)
        {
            var frame = row.Frames[i];
            var data = Array.ConvertAll(frame.Split('、'), float.Parse);
            
            // time, value, inTangent, outTangent
            curve.AddKey(new Keyframe(data[0], data[1], data[2], data[3]));
        }
        _curves.Add(key, curve);
    }

    public float GetCurveValue(int curveId, float percent)
    {
        if (!_curves.TryGetValue(curveId.ToString(), out var curve))
        {
            $"Can't find curve by id - {curveId}, please check exist.".Error();
            return 1f;
        }

        percent = Mathf.Clamp(percent, 0, 1);
        return curve.Evaluate(percent);
    }
}