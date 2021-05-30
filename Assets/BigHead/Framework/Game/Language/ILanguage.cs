//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月30日   |   多语言接口
//


/*
 * 建议多语言设置：
 * 在Excel中的属性名称中建议使用：CH,EN等标识,并设置EnumLanguage枚举类型，添加全局当前语言环境变量，如：CurrentLanguage。
 * 可以使用 CsvAssistant.GetXXXXXCsv().GetRowByKey(id).GetType().GetProperty(CurrentLanguage.ToString()).GetValue(row) 就可以直接获取到对应的语言文字。
 * 将上述封装成方法如GetLanguage(int index)，在OnLanguageChanged()中添加，则可以在每次在调用该方法。如：Text.text = GetLanguage(xx)。
 */
public interface ILanguage
{
    void OnLanguageChanged();
}