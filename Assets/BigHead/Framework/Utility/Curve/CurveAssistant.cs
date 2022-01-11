//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年1月09日   |   曲线方法入口
//

public static partial class CurveAssistant
{
    public static float GetCurveValue(int id, float percent)
    {
        return CsvAssistant.GetCurveCsv().AsPlus<CurveCsvPlus>().GetCurveValue(id, percent);
    }
}