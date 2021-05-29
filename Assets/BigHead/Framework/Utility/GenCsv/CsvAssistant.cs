//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年2月27日   |   Csv调用入口
//


public static partial class CsvAssistant
{
    public static T AsPlus<T>(this BasicCsv t) where T: BasicCsv => (T) t;
}