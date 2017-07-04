using UnityEngine;
using System.Collections;

public class TestCase : MonoBehaviour {
   // public MovieTexture mov = null;

    public static VideoController video = null;
    public const string C_VIDEOLOG_NAME = "PlayLogoVideo";

	// Use this for initialization
	void Start () {
	    
	}

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Height(50f), GUILayout.Width(100f));
        if (GUILayout.Button("load mp4", GUILayout.Height(50f), GUILayout.Width(100f)))
        {
            PlayVideo("mp4.mp4");
        }

        if (GUILayout.Button("load raw mp4", GUILayout.Height(50f), GUILayout.Width(100f)))
        {
          //  PlayVideo(mov);
        }

        if (GUILayout.Button("load ogv",GUILayout.Height(50f), GUILayout.Width(100f)))
        {
            PlayVideo("ogv.ogv");
        }

        if (GUILayout.Button("Not Exits", GUILayout.Height(50f), GUILayout.Width(100f)))
        {
            PlayVideo("noExit.ogg");
        }

        if (GUILayout.Button("Stop",GUILayout.Height(50f), GUILayout.Width(100f)))
        {
            StopVideo();
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 播放Logo视频
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    public void PlayVideo(string path, System.Action cb = null)
    {
        if (video == null)
        {
            GameObject obj = new GameObject();
            GameObject.DontDestroyOnLoad(obj);
            obj.name = C_VIDEOLOG_NAME;
            video = obj.AddComponent<VideoController>();
        }

        video.targetCamera = this.gameObject.camera;
        video.BackgroundColor = Color.black;
        video.PlayEndAction += () =>
        {
            if (cb != null)
                cb();
        };
        video.Play(path);
    }

    /// 播放Logo视频
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
//    public void PlayVideo(MovieTexture mov, System.Action cb = null)
//    {
//        if (video == null)
//        {
//            GameObject obj = new GameObject();
//            GameObject.DontDestroyOnLoad(obj);
//            obj.name = C_VIDEOLOG_NAME;
//            video = obj.AddComponent<VideoController>();
//        }

//        video.targetCamera = this.gameObject.camera;
//        video.BackgroundColor = Color.black;
//        video.PlayEndAction += () =>
//        {
//            if (cb != null)
//                cb();
//        };
//        video.Play(mov);
//    }

    /// <summary>
    /// 停止Logo视频
    /// </summary>
    public void StopVideo()
    {
        if (video != null)
            video.Stop();
    }
}
