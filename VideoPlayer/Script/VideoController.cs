// 要测试请开启下面预定义
//#define TEST_VIDEO_CONTROLLER

#if UNITY_STANDALONE || UNITY_EDITOR
#define USE_MOVIETEXTURE
#endif

using UnityEngine;
using System;
using System.Collections;
using System.IO;
//using Debug = UnityEngine.Debug;



/**
 * @brief 视频播放控制类(Editor下使用把脚本挂到Plane对象中)
 */
public class VideoController : MonoBehaviour
{
    public Color BackgroundColor = Color.black;
    public Camera targetCamera = null;

    /// 编辑器下视频播放组件，移动端不支持
#if USE_MOVIETEXTURE
    private MovieTexture _movieTexture = null;
    private AudioSource _audio    = null;
#endif

    private Renderer movieRender = null;
    private Coroutine cor = null;
    private Coroutine corDelay = null;
    private static ulong id = 0;
    private WWW www = null;

    /// <summary>
    /// 视频URL
    /// </summary>
    private string VideoUrl = "";

    public event Action PlayBeginAction = null;

    public event Action PlayEndAction = null;

    public bool Loop { get; set; }

    public bool CanUseMovieTexture
    {
        get
        {
#if USE_MOVIETEXTURE
            return true;
#else
            return false;
#endif
        }
    }

    private void CallPlayBegin(float delay)
    {
        if (this.corDelay != null)
            StopCoroutine(this.corDelay);
        this.corDelay = StartCoroutine(this._CallPlayBegin(delay, this.PlayBeginAction));
    }

    private IEnumerator _CallPlayBegin(float delay, System.Action cb)
    {
        yield return new WaitForSeconds(delay);
        if (cb != null)
            cb();
    }

    private void CallPlayEnd(float delay)
    {
        if (this.corDelay != null)
            StopCoroutine(this.corDelay);
        this.corDelay = StartCoroutine(this._CallPlayEnd(delay, this.PlayEndAction));
    }

    private IEnumerator _CallPlayEnd(float delay, System.Action cb)
    {
        yield return new WaitForSeconds(delay);
        if (cb != null)
            cb();
    }

	private void Awake()
	{
	}

    private bool ExitsVideo(string url)
    {
        string fullPath = Application.streamingAssetsPath + "/" + url;
        bool exits = File.Exists(fullPath);
        Debug.Log("fullPath = " + fullPath + "\t exits = " + exits);
        return exits;
    }

    public void Play(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

#if USE_MOVIETEXTURE
        PlayOnPC(url);
#else
        PlayOnMobile(url);
#endif
    }

#if USE_MOVIETEXTURE
    public void Play(MovieTexture mov)
    {
        this.PlayOnPC(mov);
    }
#endif

    public void Stop()
    {
#if USE_MOVIETEXTURE
        StopOnPC();
#else
        StopOnMobile();
#endif
    }

#if USE_MOVIETEXTURE
    private void StopOnPC()
    {
        if (this._movieTexture != null)
            this._movieTexture.Stop();
            //AssetBundle.(this._movieTexture);
        this._movieTexture = null;

        if (this._audio != null)
            this._audio.Stop();
            //Resources.UnloadAsset(this._audio);
        this._audio = null;

        if (this.movieRender != null)
            GameObject.Destroy(this.movieRender.gameObject);
        this.movieRender = null;

        ///GameObject.Destroy(this.gameObject);

        if (this.www != null)
            this.www.Dispose();
        this.www = null;

        Resources.UnloadUnusedAssets();
    }

    private void Prepare()
    {
        // 自适配Camera
        if (this.movieRender == null)
        {
            // TODO: 增加自适配屏幕
            if (this.movieRender == null)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.layer = this.targetCamera.gameObject.layer;
                this.movieRender = obj.renderer;
                this.movieRender.material = new Material(Shader.Find("UI/Default"));
                this.movieRender.transform.localScale = new Vector3(10f, 10f, 1f);
            }
            //this.CallPlayEndAction();
            //return;
        }

        this._audio = this.movieRender.GetComponent<AudioSource>();
        if (this._audio == null)
            this._audio = this.movieRender.gameObject.AddComponent<AudioSource>();
    }

    private void PlayOnPC(string url)
     {
         if (this.VideoUrl != url)
         {
             this.Stop();
         }

         if (!ExitsVideo(url))
         {
             Debug.LogWarning("url = " + url + " no exits");
             return;
         }

         this.Prepare();

        this.VideoUrl = url;
        if (this.cor != null)
            StopCoroutine(this.cor);
        this.cor = StartCoroutine(DownLoadMovie(this.VideoUrl, (MovieTexture texture) =>
            {
                if (texture == null)
                {
                    CallPlayEnd(-1);
                    return;
                }
                this.PlayOnPC(texture); 
            }));
     }

    private void PlayOnPC(MovieTexture mov)
    {
        if (mov == null)
        {
            Invoke("CallPlayEndAction", -1);
            return;
        }

        this.Prepare();

        _audio.clip = mov.audioClip;
        _audio.playOnAwake = false;
        _movieTexture = mov;
        _movieTexture.name = id.ToString();
        ++id;

        this.movieRender.material.mainTexture = _movieTexture;
        mov.loop = false;
        CallPlayBegin(-1);
        mov.Play();
        _audio.Play();
        CallPlayEnd(mov.duration);
    }

    private IEnumerator DownLoadMovie(string path, Action<MovieTexture> loadFinish)
    {
        if (!string.IsNullOrEmpty(path))
        {
            string url = "file://" + Application.streamingAssetsPath + "/" + path;
            this.www = new WWW(url);
            while (www.isDone)
            {
                yield return www;
            }

            MovieTexture mov = www.movie;
            while (!mov.isReadyToPlay)
            {
                yield return www;
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                if (null != loadFinish)
                    loadFinish(null);
                yield break;
            }

            if (null != loadFinish)
            {
                loadFinish(www.movie);
            }
        }
    }

#else
    private void StopOnMobile()
    {

    }

    private void PlayOnMobile(string url)
    {
        //if (!ExitsVideo(url))
      //      return;
        StartCoroutine(PlayVideoCoroutine(url));
    }

    IEnumerator PlayVideoCoroutine(string url)
    {

        CallPlayBegin(-1);
        Handheld.PlayFullScreenMovie(url, BackgroundColor, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
        this.VideoUrl = url;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        CallPlayEnd(-1);
    }
#endif

#if TEST_VIDEO_CONTROLLER
    // 测试代码
    void OnGUI()
    {
        if (GUI.Button (new Rect (20,10,200,50), "PLAY ControlMode.CancelOnTouch")) 
        {
            _controlMode = FullScreenMovieControlMode.CancelOnInput; // 视频播放时触摸屏幕视频关闭
            Play();
        }

        if (GUI.Button (new Rect (20,90,200,25), "PLAY ControlMode.Full")) 
        {
            _controlMode = FullScreenMovieControlMode.Full; // 视频播放时弹出IOS高级控件，控制视频暂停播放 全屏等等
            Play();
        }

        if (GUI.Button (new Rect (20,170,200,25), "PLAY ControlMode.Hidden")) 
        {
            _controlMode = FullScreenMovieControlMode.Hidden; // 视频播放时无法停止，当其播放完一次后自动关闭
            Play();
        }

        if (GUI.Button (new Rect (20,250,200,25), "PLAY ControlMode.Minimal")) 
        {
            _controlMode = FullScreenMovieControlMode.Minimal; // 视频播放时弹出IOS高级控件，可控制播放进度
            Play();
        }
    }
#endif
}

