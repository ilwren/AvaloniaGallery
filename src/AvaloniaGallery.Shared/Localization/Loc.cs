using System.Globalization;
using Avalonia.Data;

namespace AvaloniaGallery.Localization;

/// <summary>
/// The gallery's translation table and the current language.
/// <para>
/// Deliberately hand rolled rather than backed by .resx: switching language in a .resx
/// setup means restarting the app or rebuilding every view, because the generated
/// accessors return a <see cref="string"/> captured at load time. Here every translated
/// string is surfaced through <see cref="LocExtension"/>, which is an
/// <see cref="IObservable{T}"/>, so changing <see cref="Language"/> updates live bindings
/// in place and the whole UI re-letters itself without a reload.
/// </para>
/// </summary>
public static class Loc
{
    /// <summary>Raised whenever <see cref="Language"/> changes.</summary>
    public static event Action? LanguageChanged;

    /// <summary>
    /// Languages offered in the title bar picker.
    /// <para>
    /// Declared before <see cref="_language"/> on purpose: static field initializers run in
    /// source order, so reading this list from an earlier initializer would see null.
    /// </para>
    /// </summary>
    public static IReadOnlyList<LanguageInfo> SupportedLanguages { get; } = new[]
    {
        new LanguageInfo("en", "English", "English"),
        new LanguageInfo("zh-Hans", "Chinese (Simplified)", "简体中文"),
    };

    private static LanguageInfo _language = SupportedLanguages[0];

    /// <summary>
    /// Applies the initial language's culture.
    /// <para>
    /// A field initializer sets <see cref="_language"/> without running the property setter,
    /// so the culture was previously left at whatever the OS supplied. On a Chinese Windows
    /// that meant the gallery reported English while <see cref="CultureInfo.CurrentCulture"/>
    /// was still zh-CN, and every culture-driven control — Calendar, DatePicker, TimePicker —
    /// rendered Chinese month and meridiem names under an English UI.
    /// </para>
    /// </summary>
    static Loc() => ApplyCulture(_language);

    public static LanguageInfo Language
    {
        get => _language;
        set
        {
            if (_language == value)
                return;

            _language = value;
            ApplyCulture(value);

            LanguageChanged?.Invoke();
        }
    }

    /// <summary>
    /// Keeps date, number and calendar formatting in the samples consistent with the chosen
    /// language, not just the gallery's own chrome.
    /// </summary>
    private static void ApplyCulture(LanguageInfo language)
    {
        var culture = CultureInfo.GetCultureInfo(language.Code);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    /// <summary>
    /// Picks the closest supported language for the OS locale, falling back to English.
    /// </summary>
    public static void UseSystemLanguage()
    {
        var name = CultureInfo.CurrentUICulture.Name;

        var match = SupportedLanguages.FirstOrDefault(
                        l => name.Equals(l.Code, StringComparison.OrdinalIgnoreCase))
                    ?? SupportedLanguages.FirstOrDefault(
                        l => name.StartsWith(l.Code.Split('-')[0], StringComparison.OrdinalIgnoreCase));

        if (match is not null)
            Language = match;
    }

    /// <summary>
    /// Looks up <paramref name="key"/> for the current language.
    /// Missing keys return the key itself, which makes an omission obvious in the UI
    /// rather than silently rendering an empty label.
    /// </summary>
    public static string Get(string key)
    {
        var table = _language.Code == "zh-Hans" ? Zh : En;

        if (table.TryGetValue(key, out var value))
            return value;

        // Fall back to English before giving up, so a partial translation degrades to a
        // readable UI instead of a wall of raw keys.
        return En.TryGetValue(key, out var fallback) ? fallback : key;
    }

    /// <summary>English strings, and the fallback for any key a translation is missing.</summary>
    private static readonly Dictionary<string, string> En = new(StringComparer.Ordinal)
    {
        ["App.Title"] = "Avalonia Gallery",
        ["App.Search"] = "Search controls…",
        ["App.Home"] = "Back to the home page",
        ["App.Theme"] = "Switch between light and dark",
        ["App.Language"] = "Change language",

        ["Home.Title"] = "Avalonia Gallery",
        ["Home.Intro"] = "Every control Avalonia ships for free, with runnable examples and the exact markup that produced them. Each sample is parsed at runtime, so the code you copy is the code you just saw running.",
        ["Home.Controls"] = "controls documented",
        ["Home.Samples"] = "runnable examples",
        ["Home.Categories"] = "categories",
        ["Home.New"] = "new in Avalonia 12",
        ["Home.Browse"] = "Browse by category",
        ["Home.ControlsCount"] = "{0} controls",
        ["Home.AiNotice"] = "Built by AI — every sample, test and document in this project was written by an AI agent, without human code review.",

        ["Page.Source"] = "Source",
        ["Page.Copy"] = "Copy",
        ["Page.Copied"] = "Copied",
        ["Page.SourceTip"] = "Show or hide the code for this sample",
        ["Page.CopyTip"] = "Copy the code to the clipboard",
        ["Page.Docs"] = "Documentation",
        ["Page.NewBadge"] = "New in 12",
        ["Page.PageSource"] = "Page source",
        ["Page.PageSourceTip"] = "Show the gallery's own C# for this page",
        ["Page.Properties"] = "Properties",
        ["Page.PropertiesTip"] = "Change this sample's properties live",

        ["Sample.Failed"] = "This sample failed to load",

        ["Platform.DesktopOnly"] = "Desktop only",
        ["Platform.NotSupported"] = "This control is not available on the current platform ({0}). The code is shown so you can still read it.",

        ["Tray.Enable"] = "Show tray icon",
        ["Tray.Disable"] = "Hide tray icon",
        ["Tray.Active"] = "The tray icon is live — look in your system notification area.",
        ["Tray.Inactive"] = "No tray icon is registered right now.",
        ["Tray.Unavailable"] = "Tray icons need a desktop session, so this button is disabled here.",
        ["Tray.MenuShow"] = "Show gallery",
        ["Tray.MenuHide"] = "Hide gallery",
        ["Tray.MenuQuit"] = "Quit",
        ["Tray.ToolTip"] = "Avalonia Gallery",

        ["Cat.BasicInput"] = "Basic input",
        ["Cat.Text"] = "Text",
        ["Cat.Collections"] = "Collections",
        ["Cat.Layout"] = "Layout",
        ["Cat.Navigation"] = "Navigation",
        ["Cat.Menus"] = "Menus and toolbars",
        ["Cat.DateTime"] = "Date and time",
        ["Cat.Graphics"] = "Graphics and media",
        ["Cat.Color"] = "Color",
        ["Cat.Status"] = "Status and info",
        ["Cat.System"] = "System",
    };

    /// <summary>Simplified Chinese strings.</summary>
    private static readonly Dictionary<string, string> Zh = new(StringComparer.Ordinal)
    {
        ["App.Title"] = "Avalonia 控件库",
        ["App.Search"] = "搜索控件…",
        ["App.Home"] = "返回首页",
        ["App.Theme"] = "切换明暗主题",
        ["App.Language"] = "切换语言",

        ["Home.Title"] = "Avalonia 控件库",
        ["Home.Intro"] = "涵盖 Avalonia 免费提供的全部控件，每个示例都可运行，并附上生成它的原始标记。示例在运行时解析，因此你复制的代码就是你刚刚看到运行的代码。",
        ["Home.Controls"] = "个控件",
        ["Home.Samples"] = "个可运行示例",
        ["Home.Categories"] = "个分类",
        ["Home.New"] = "个 Avalonia 12 新增",
        ["Home.Browse"] = "按分类浏览",
        ["Home.ControlsCount"] = "{0} 个控件",
        ["Home.AiNotice"] = "由 AI 实现 —— 本项目的每个示例、测试与文档均由 AI 代理编写，未经过人工代码评审。",

        ["Page.Source"] = "源代码",
        ["Page.Copy"] = "复制",
        ["Page.Copied"] = "已复制",
        ["Page.SourceTip"] = "显示或隐藏此示例的代码",
        ["Page.CopyTip"] = "将代码复制到剪贴板",
        ["Page.Docs"] = "官方文档",
        ["Page.NewBadge"] = "12 新增",
        ["Page.PageSource"] = "页面源码",
        ["Page.PageSourceTip"] = "查看本页在 Gallery 中的 C# 实现",
        ["Page.Properties"] = "属性",
        ["Page.PropertiesTip"] = "实时调整该示例的属性",

        ["Sample.Failed"] = "示例加载失败",

        ["Platform.DesktopOnly"] = "仅桌面端",
        ["Platform.NotSupported"] = "当前平台（{0}）不支持此控件，下面仅展示代码供参考。",

        ["Tray.Enable"] = "显示托盘图标",
        ["Tray.Disable"] = "隐藏托盘图标",
        ["Tray.Active"] = "托盘图标已启用 —— 请查看系统通知区域。",
        ["Tray.Inactive"] = "当前未注册托盘图标。",
        ["Tray.Unavailable"] = "托盘图标需要桌面环境，此处按钮不可用。",
        ["Tray.MenuShow"] = "显示主窗口",
        ["Tray.MenuHide"] = "隐藏主窗口",
        ["Tray.MenuQuit"] = "退出",
        ["Tray.ToolTip"] = "Avalonia 控件库",

        ["Cat.BasicInput"] = "基础输入",
        ["Cat.Text"] = "文本",
        ["Cat.Collections"] = "集合",
        ["Cat.Layout"] = "布局",
        ["Cat.Navigation"] = "导航",
        ["Cat.Menus"] = "菜单与工具栏",
        ["Cat.DateTime"] = "日期与时间",
        ["Cat.Graphics"] = "图形与媒体",
        ["Cat.Color"] = "颜色",
        ["Cat.Status"] = "状态与信息",
        ["Cat.System"] = "系统",
    };

    /// <summary>Sanity check used by the tests: every English key must exist in every table.</summary>
    public static IReadOnlyCollection<string> Keys => En.Keys;

    internal static IReadOnlyDictionary<string, string> TableFor(string code) =>
        code == "zh-Hans" ? Zh : En;
}

/// <summary>One entry in the language picker.</summary>
public sealed record LanguageInfo(string Code, string EnglishName, string NativeName)
{
    public override string ToString() => NativeName;
}
