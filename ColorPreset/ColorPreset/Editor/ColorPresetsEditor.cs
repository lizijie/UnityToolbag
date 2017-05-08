using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class ColorPresetsEditWindow: EditorWindow
{
    public const string FORMAT_COLORITEM = "    public static string {0}{1} = \"{2}\";\n";
    public const string BEGIN_GENERATE_CODE = "    // BEGIN_GENERATE_CODE";
    public const string END_GENERATE_CODE = "    // END_GENERATE_CODE";
    public const string COLORPRESET_CODE_PATH = "COLORPRESET_CODE_PATH";
    public const string COLORPRESET_COLOR_PATH = "COLORPRESET_COLOR_PATH";

    private List<string> colorLibs = new List<string>();
    private string[] colorList = null;
    private int colorIndex = -1;
    private string colorPath = string.Empty;
    private string codePath = string.Empty;
    private UnityEngine.Object codeObject = null;

    private static ColorPresetsEditWindow wnd = null;

    [MenuItem("Color/Open")]
    private static void OpenColorPresetsEditWindow()
    {
        wnd = EditorWindow.GetWindow<ColorPresetsEditWindow>();
    }

    [MenuItem("Color/Clear")]
    private static void ClearColorPresets()
    {
        EditorPrefs.DeleteKey(COLORPRESET_CODE_PATH);
        EditorPrefs.DeleteKey(COLORPRESET_COLOR_PATH);
        if (wnd != null)
            wnd.Close();
        OpenColorPresetsEditWindow();
    }

    [MenuItem("Color/Print")]
    private static void PrintColorPresets()
    {
        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("提示", "编译完再试喔", "Ok");
            return;
        }

        ColorPresets.Reflash();
        int count = ColorPresets.Count();
        string[] keys = ColorPresets.GetColorKeys();
        if (keys == null)
            return;

        string ret = "";
        for (int i = 0; i < count; ++i)
        {
            string key = keys[i];
            string value = ColorPresets.GetHexString(key);
            ret += string.Format("{0} {1}\n", key, value);
        }

        Debug.Log(ret);
    }

    private void OnEnable()
    {
        Type typeClassPresetLocaton = System.Type.GetType("UnityEditor.PresetLibraryLocations,UnityEditor");
        MethodInfo _GetAvailableFilesWithExtensionOnTheHDDFunc = typeClassPresetLocaton.GetMethod("GetAvailableFilesWithExtensionOnTheHDD");
        Type typeEnumPresetLocation = System.Type.GetType("UnityEditor.PresetFileLocation,UnityEditor");

        // 获取颜色库的绝对路径
        System.Object objProjectLocation = Enum.Parse(typeEnumPresetLocation, "ProjectFolder");
        System.Object objPreferenceLocation = Enum.Parse(typeEnumPresetLocation, "PreferencesFolder");
        List<string> projectColorList = (List<string>)_GetAvailableFilesWithExtensionOnTheHDDFunc.Invoke(null, new System.Object[2]{objProjectLocation,"colors"});
        List<string> preferenceColorList = (List<string>)_GetAvailableFilesWithExtensionOnTheHDDFunc.Invoke(null, new System.Object[2] { objPreferenceLocation, "colors" });
        this.colorLibs.Clear();
        for (int i = 0, iMax = preferenceColorList.Count; i < iMax; ++i)
            this.colorLibs.Add(preferenceColorList[i]);
        for (int i = 0, iMax = projectColorList.Count; i < iMax; ++i)
            this.colorLibs.Add(System.IO.Path.GetFullPath(projectColorList[i]));
        // 生成颜色选择列表
        List<string> list = new List<string>(this.colorLibs);
        for (int i = 0, iMax = list.Count; i < iMax; ++i)
            list[i] = System.IO.Path.GetFileNameWithoutExtension(list[i]);
        this.colorList = list.ToArray();

        // 缓存的代码路径
        this.codePath = EditorPrefs.GetString(COLORPRESET_CODE_PATH);
        this.codeObject = null; 
        if (File.Exists(this.codePath))
        {
            this.codePath = this.codePath.Replace("\\", "/");
            if (this.codePath.Contains(Application.dataPath))
            {
                string path = this.codePath.Remove(0, Application.dataPath.Length);
                path = "Assets" + path;
                this.codeObject = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            }
            else
            {
                this.codePath = string.Empty;
                this.codeObject = null;
            }
        }
        else
        {
            this.codePath = string.Empty;
            this.codeObject = null;
        }
        Debug.Log("CodePath = " + this.codePath);

        // 缓存的颜色库路径
        this.colorPath = EditorPrefs.GetString(COLORPRESET_COLOR_PATH);
        this.colorIndex = -1;
        if (File.Exists(this.colorPath))
        {
            this.colorIndex = this.colorLibs.IndexOf(this.colorPath);
        }
        else
        {
            this.colorPath = string.Empty;
            this.colorIndex = -1;
        }
        Debug.Log("ColorPath = " + this.colorPath);
    }

    private void OnGUI()
    {
        int index = EditorGUILayout.Popup(this.colorIndex, this.colorList);
        if (this.colorIndex != index)
        {
            this.colorIndex = index;
            if (this.colorIndex != -1)
            {
                this.colorPath = this.colorLibs[this.colorIndex];
                EditorPrefs.SetString(COLORPRESET_COLOR_PATH, this.colorPath); 
            }
        }

        EditorGUILayout.LabelField("代码路径：" + this.codePath);
        UnityEngine.Object obj = EditorGUILayout.ObjectField(this.codeObject, typeof(UnityEngine.Object));
        if (this.codeObject != obj)
        {
            this.codeObject = obj;
            if (this.codeObject != null)
            {
                this.codePath = AssetDatabase.GetAssetPath(this.codeObject);
                this.codePath = Path.GetFullPath(this.codePath);
                EditorPrefs.SetString(COLORPRESET_CODE_PATH, this.codePath); 
            }
        }

        if (!string.IsNullOrEmpty(this.colorPath) 
            && !string.IsNullOrEmpty(this.codePath) 
            && GUILayout.Button("生成颜色码"))
        {
            string msg = string.Format("将{0}\n颜色生成至{1}", this.colorPath, this.codePath);
            if (EditorUtility.DisplayDialog("提示", msg, "确定"))
            {
                this.GenerateColorPreset(this.colorPath, this.codePath);
            }
        }
    }

    public  void GenerateColorPreset(string _colorPath, string _codePath)
    {
        if (string.IsNullOrEmpty(_colorPath) || string.IsNullOrEmpty(_codePath))
            return;

        Debug.Log("_colorPath = " + _colorPath);
        Debug.Log("_codePath = " + _codePath);

        // 获取颜色样式
        System.Object[] objectColorPresetLibrary = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(_colorPath);
        System.Type typeColorPresetLibrary = System.Type.GetType("UnityEditor.ColorPresetLibrary,UnityEditor");
        System.Reflection.MethodInfo _CountFunc = typeColorPresetLibrary.GetMethod("Count");
        System.Reflection.MethodInfo _GetNameFunc = typeColorPresetLibrary.GetMethod("GetName");
        System.Reflection.MethodInfo _GetPresetFunc = typeColorPresetLibrary.GetMethod("GetPreset");

        string genCode = "";
        System.Object obj = objectColorPresetLibrary[0];
        int count = (int)_CountFunc.Invoke(obj, null);
        for (int i = 0; i < count; ++i)
        {
            string name = (string)_GetNameFunc.Invoke(obj, new System.Object[1] { i });
            Color col = (Color)_GetPresetFunc.Invoke(obj, new System.Object[1] { i });
            string hexStr = ColorPresets.ColorToHexString(col);
            genCode += string.Format(FORMAT_COLORITEM, ColorPresets.COLOR_PREFIX, name, hexStr);
        }

        // 插入到目标代码颜色码区域
        string content = System.IO.File.ReadAllText(_codePath);
        int beginIndex = content.IndexOf(BEGIN_GENERATE_CODE);
        int endIndex = content.IndexOf(END_GENERATE_CODE);
        
        string upContent = content.Substring(0, beginIndex + BEGIN_GENERATE_CODE.Length);
        string backContent = content.Substring(endIndex);

        string newStr = upContent + "\n" + genCode + backContent;
        Debug.Log(newStr);
        File.WriteAllText(_codePath, newStr);
    }
}
