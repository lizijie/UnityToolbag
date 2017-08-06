using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Animation))]
public class AnimationShower : Editor
{
    private int _curIndex = 0;
    private List<string> animsName = new List<string>();
    public void Awake()
    {
        UpdateAnimations();
    }

    public override void OnInspectorGUI ()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            UpdateAnimations();
        }
        Animation anim = target as Animation;

        GUILayout.BeginHorizontal();
        GUILayout.Label("动画: ");
        _curIndex = EditorGUILayout.Popup(_curIndex, animsName.ToArray());
        if (GUILayout.Button("播放"))
        {
            if (_curIndex < animsName.Count)
            {
                anim.Stop();
                anim.Play(animsName[_curIndex]);
            }
        }
        GUILayout.EndHorizontal();
    }

    protected void UpdateAnimations()
    {
        Animation anim = target as Animation;
        var enumerator = anim.GetEnumerator();
        while(enumerator.MoveNext())
        {
            AnimationState state = enumerator.Current as AnimationState;
            if(state.clip != null) animsName.Add(state.clip.name);
        }
    }
}