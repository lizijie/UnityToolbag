using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

public class ColorPresetDemo
{
    [MenuItem("Color/Test")]
    private static void Test()
    {
        List<string> paths = ColorPresetPaths;
        for (int i = 0, iMax = paths.Count; i < iMax; ++i)
        {
            string colorPath = paths[i];
            Debug.Log("ColorPath = " + colorPath);
            List<string> pairs = GetColorPairs(colorPath);
            Print(pairs);
        }
    }

    /// <summary>
    /// 获得颜色库名称列表
    /// </summary>
    private static List<string> ColorPresetPaths
    {
        get
        {
            Type typeClassPresetLocaton = System.Type.GetType("UnityEditor.PresetLibraryLocations,UnityEditor");
            MethodInfo _GetAvailableFilesWithExtensionOnTheHDDFunc = typeClassPresetLocaton.GetMethod("GetAvailableFilesWithExtensionOnTheHDD");
            Type enumPresetLocation = System.Type.GetType("UnityEditor.PresetFileLocation,UnityEditor");

            // 全局路径。多个Unity项目间可以共享
            System.Object parmPreferenceLocation = Enum.Parse(enumPresetLocation, "PreferencesFolder");
            List<string> preferenceColorList = (List<string>)_GetAvailableFilesWithExtensionOnTheHDDFunc.Invoke(null, new System.Object[2] { parmPreferenceLocation, "colors" });
            // 本地项目路径。仅被当前Unity项目使用。
            System.Object parmProjectLocation = Enum.Parse(enumPresetLocation, "ProjectFolder");
            List<string> projectColorList = (List<string>)_GetAvailableFilesWithExtensionOnTheHDDFunc.Invoke(null, new System.Object[2] { parmProjectLocation, "colors" });

            // 收集所有颜色库文件路径
            List<string> colorLibs = new List<string>();
            for (int i = 0, iMax = preferenceColorList.Count; i < iMax; ++i)
                colorLibs.Add(preferenceColorList[i]);
            for (int i = 0, iMax = projectColorList.Count; i < iMax; ++i)
                colorLibs.Add(System.IO.Path.GetFullPath(projectColorList[i]));

            return colorLibs;
            // 生成颜色选择列表
            List<string> fileNames = new List<string>(colorLibs);
            for (int i = 0, iMax = fileNames.Count; i < iMax; ++i)
                fileNames[i] = System.IO.Path.GetFileNameWithoutExtension(fileNames[i]);

            return fileNames;
        }
    }

    /// <summary>
    /// 反序列path路径保存的颜色对
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static List<string> GetColorPairs(string path)
    {
        List<string> pairs = new List<string>();
        if (string.IsNullOrEmpty(path))
            return pairs;

        System.Type typeColorPresetLibrary = System.Type.GetType("UnityEditor.ColorPresetLibrary,UnityEditor");
        System.Reflection.MethodInfo _CountFunc = typeColorPresetLibrary.GetMethod("Count");
        System.Reflection.MethodInfo _GetNameFunc = typeColorPresetLibrary.GetMethod("GetName");
        System.Reflection.MethodInfo _GetPresetFunc = typeColorPresetLibrary.GetMethod("GetPreset");

        System.Object[] instanceArray = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
        System.Object colorLibIntance = instanceArray[0];
        int count = (int)_CountFunc.Invoke(colorLibIntance, null);
        for (int i = 0; i < count; ++i)
        {
            string name = (string)_GetNameFunc.Invoke(colorLibIntance, new System.Object[1] { i });
            Color col = (Color)_GetPresetFunc.Invoke(colorLibIntance, new System.Object[1] { i });
            string hexStr = ColorToHexString(col);
            string val = string.Format("{0}:{1}", name, hexStr);
            pairs.Add(val);
        }

        return pairs;
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

    /// <summary>
    /// 打印颜色对
    /// </summary>
    /// <param name="pairs"></param>
    private static void Print(List<string> pairs)
    {
        string temp = "";
        for (int i = 0, iMax = pairs.Count; i < iMax; ++i)
        {
            temp += pairs[i] + "\n";
        }

        Debug.Log(temp);
    }
}
