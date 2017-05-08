using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;

namespace U3DFramework
{
	public class MarcoSetting 
	{
        // 包类型
	    public const string BASIC = "BASIC";
	    public const string SDK = "SDK";

        // 发布类型
	    public const string DEBUG = "_DEBUG";
	    public const string RELEASE = "_RELEASE";
	
	    /// <summary>
	    ///  对目标平台的宏组，追加宏
	    /// </summary>
	    /// <param name="targetGroup"></param>
	    /// <param name="define"></param>
	    public static void AddScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string define)
	    {
	        string temp = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
	        if (string.IsNullOrEmpty(temp))
	        {
	            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
	            return;
	        }
	
	        string[] defines = Array.FindAll(temp.Split(';'), (string val) =>
	        {
	            return val != define;
	        });
	
	        string defineString = "";
	        for (int i = 0; i < defines.Length; ++i)
	        {
	            defineString += defines[i] + ";";
	        }
	        defineString += define;
	
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineString);
	    }
	
	    /// <summary>
	    /// 对目标平台的宏组，删除宏
	    /// </summary>
	    /// <param name="targetGroup"></param>
	    /// <param name="define"></param>
	    public static void RemoveScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string define)
	    {
	        string temp = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
	        if (string.IsNullOrEmpty(temp))
	            return;
	
	        string[] defines = Array.FindAll(temp.Split(';'), (string val) =>
	        {
	            return val != define;
	        });
	
	        string defineString = "";
	        if (defines.Length > 0)
	            defineString = defines[0];
	
	        for (int i = 1; i < defines.Length; ++i)
	        {
	            defineString += ";" + defines[i];
	        }
	
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineString);
	    } 
	
	    [MenuItem("U3DFramwork/Marco/SetBasic")]
	    public static void SetBasic()
	    {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
#if UNITY_ANDROID
            group = BuildTargetGroup.Android;
#elif UNITY_IPHONE
            group = BuildTargetGroup.iOS;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            group = BuildTargetGroup.Standalone;
#endif
            MarcoSetting.RemoveScriptingDefineSymbolsForGroup(group, SDK);
            MarcoSetting.AddScriptingDefineSymbolsForGroup(group, BASIC);
	        AssetDatabase.Refresh();
        }
	
	    [MenuItem("U3DFramwork/Marco/SetSdk")]
	    public static void SetSdk()
	    {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
	#if UNITY_ANDROID
            group = BuildTargetGroup.Android;
	#elif UNITY_IPHONE
            group = BuildTargetGroup.iOS;
	#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            group = BuildTargetGroup.Standalone;
	#endif
            MarcoSetting.RemoveScriptingDefineSymbolsForGroup(group, BASIC);
            MarcoSetting.AddScriptingDefineSymbolsForGroup(group, SDK);
	        AssetDatabase.Refresh();
        }

	    [MenuItem("U3DFramwork/Marco/SetDebug")]
	    public static void SetDebug()
	    {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
	#if UNITY_ANDROID
            group = BuildTargetGroup.Android;
	#elif UNITY_IPHONE
            group = BuildTargetGroup.iOS;
	#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            group = BuildTargetGroup.Standalone;
	#endif
            MarcoSetting.RemoveScriptingDefineSymbolsForGroup(group, RELEASE);
            MarcoSetting.AddScriptingDefineSymbolsForGroup(group, DEBUG);
	        AssetDatabase.Refresh();
	    }
	
	    [MenuItem("U3DFramwork/Marco/SetRelease")]
	    public static void SetRelease()
	    {
            BuildTargetGroup group = BuildTargetGroup.Standalone;
	#if UNITY_ANDROID
            group = BuildTargetGroup.Android;
	#elif UNITY_IPHONE
            group = BuildTargetGroup.iOS;
	#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
            group = BuildTargetGroup.Standalone;
	#endif
            MarcoSetting.RemoveScriptingDefineSymbolsForGroup(group, DEBUG);
            MarcoSetting.AddScriptingDefineSymbolsForGroup(group, RELEASE);
	        AssetDatabase.Refresh();
	    }
	
	    [MenuItem("U3DFramwork/Marco/Clear")]
	    public static void Clear()
	    {
	#if UNITY_ANDROID
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
	#elif UNITY_IPHONE
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
	#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
	        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "");
	        AssetDatabase.Refresh();
	#endif
	    }
	}
}
