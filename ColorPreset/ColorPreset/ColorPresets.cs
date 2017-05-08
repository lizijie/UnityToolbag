using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public static partial class ColorPresets
{
    // --- 以下2行注释，用于标记颜色代码插入起始和结束位置
    // BEGIN_GENERATE_CODE
    public static string Color_2324234 = "314D795";
    public static string Color_45 = "314D795";
    // END_GENERATE_CODE

    // 颜色项前缀
    public const string COLOR_PREFIX = "Color_";
    private static Dictionary<string, string> colors = new Dictionary<string,string>();

    static ColorPresets()
    {
        Reflash();
    }

    /// <summary>
    /// 刷新颜色列表
    /// </summary>
    public static void Reflash()
    {
        colors.Clear();
        Type t = typeof(ColorPresets);
        FieldInfo[] list = t.GetFields(BindingFlags.Static | BindingFlags.Public);
        for (int i = 0, iMax = list.Length; i < iMax; ++i)
        {
            FieldInfo info = list[i];
            if (info.Name.Contains(COLOR_PREFIX))
            {
                colors.Add(info.Name, (string)info.GetValue(null));
            }
        }
    }

    /// <summary>
    /// 获取颜色列表长度
    /// </summary>
    /// <returns></returns>
    public static int Count()
    {
        return colors.Count;
    }

    /// <summary>
    /// 获取颜色列表Keys数组
    /// </summary>
    /// <returns></returns>
    public static string[] GetColorKeys()
    {
        return new List<string>(colors.Keys).ToArray();
    }

    /// <summary>
    /// 获取颜色Key对应的十六进制颜色
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetHexString(string key)
    {
        if (colors == null || !colors.ContainsKey(key))
            return string.Empty;
        return colors[key];
    }

    /// <summary>
    /// 获取颜色Key对应的十六进制颜色
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns
    //public static string GetHexString(int num)
    //{
    //    string key = COLOR_PREFIX + num;
    //    return GetHexString(key);
    //}

    /// <summary>
    /// 将内部定义色号, 转换为Color对象
    /// </summary>
    /// <param name="colorNum"></param>
    /// <returns></returns>
    //public static Color FromNum(int num)
    //{
    //    string key = COLOR_PREFIX + num;
    //    if (colors == null || !colors.ContainsKey(key))
    //        return Color.white;
    //    return HexStringToColor(colors[key]);
    //}

    /// <summary>
    /// 由[#1]表示转换为[FFF1AE]实际色值
    /// 
    /// 如:      [#11]xx伤害后有[-][#6]30%[-][#11]概率可触发小喷泉[-]
    /// 转换为:  [B4A78C]xx伤害后有[-][60D92B]30%[-][B4A78C]概率可触发小喷泉[-] 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string FormatText(string src)
    {
        if (string.IsNullOrEmpty(src)) return src;
        string ret = src;
        Regex regex = new Regex(@"\[#\d+\]", RegexOptions.IgnoreCase);
        MatchCollection matches = regex.Matches(src);
        foreach (Match match in matches)
        {
            string strRex = match.Value;
            string num = strRex.Substring(2, strRex.Length - 3).Trim().ToUpper();
            string key = COLOR_PREFIX + num;
            if (colors.ContainsKey(key))
            {
                string val = colors[key];
                string str = string.Format("[{0}]", val);
                ret = ret.Replace(strRex, str);
            }
        }
        return ret;
    }

    /// <summary>
    /// 将十六进制字符串转换成Color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public static Color HexStringToColor(string hex)
    {
        if (hex.Length < 6)
            return Color.black;
 
        byte r = 0;
        byte g = 0;
        byte b = 0;
        byte a = 0;

        byte.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
        byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out g);
        byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out b);
        byte.TryParse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, null, out a);

        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// 颜色转换为十六进制字符串
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static string ColorToHexString(Color c)
    {
        string ret = "";
        ret += Mathf.RoundToInt(c.r * 255f).ToString("X2");
        ret += Mathf.RoundToInt(c.g * 255f).ToString("X2");
        ret += Mathf.RoundToInt(c.b * 255f).ToString("X2");
        ret += Mathf.RoundToInt(c.a * 255f).ToString("X2");
        return ret;
    }
}
