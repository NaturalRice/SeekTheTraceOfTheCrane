using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // 添加这行

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas;
    [SerializeField] private GameObject videoCanvas0;
    [SerializeField] private GameObject videoCanvas1;
    [SerializeField] private VideoPlayer videoPlayer0;
    [SerializeField] private VideoPlayer videoPlayer1;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private string mainSceneName = "SampleScene";
    
    // 新增音频控制变量
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip mainSceneMusic;
    [SerializeField] private float musicFadeInDuration = 1.0f; // 音乐淡入时间

    private bool isVideoPrepared = false;
    private bool isVideoPlaying = false;
    private bool hasFinished = false;
    private bool canSkipVideo = false; // 新增：控制是否可以跳过

    private void Start()
    {
        startScreenCanvas.SetActive(true);
        videoCanvas0.SetActive(false);
        hasFinished = false;
        canSkipVideo = false; // 初始不可跳过

        if(backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        if (videoPlayer0 == null)
        {
            Debug.LogError("VideoPlayer组件未正确设置！");
            return;
        }

        videoPlayer0.playOnAwake = false;
        videoPlayer0.isLooping = false;

        videoPlayer0.prepareCompleted += OnVideoPrepared;
        videoPlayer0.loopPointReached += OnVideoFinished;
        videoPlayer0.errorReceived += OnVideoError;

        if (rawImage == null)
        {
            Debug.LogError("RawImage组件未正确设置！");
        }
        else
        {
            rawImage.texture = videoPlayer0.targetTexture;
        }
    }

    private void Update()
    {
        if (!hasFinished && !isVideoPrepared && !isVideoPlaying && Input.anyKeyDown)
        {
            StartVideoPlayback();
        }
        
        // 仅在视频播放期间允许跳过
        if (canSkipVideo && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    // 新增专用跳过方法
    private void SkipVideo()
    {
        if (!canSkipVideo) return;
        
        Debug.Log("跳过视频");
        videoPlayer0.Stop();
        canSkipVideo = false; // 立即禁用跳过
        OnVideoFinished(videoPlayer0);
    }

    private void StartVideoPlayback()
    {
        startScreenCanvas.SetActive(false);
        videoCanvas0.SetActive(true);
        videoPlayer0.Prepare();
        Debug.Log("开始准备视频...");
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("视频准备完成，开始播放！");
        isVideoPrepared = true;
        videoPlayer0.Play();
        isVideoPlaying = true;
        canSkipVideo = true; // 视频开始播放后允许跳过
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        Debug.Log("视频播放完毕！");
        hasFinished = true;
        canSkipVideo = false; // 确保跳过功能关闭
        videoCanvas0.SetActive(false);
        
        // 视频结束后开始播放背景音乐
        if(backgroundMusic != null)
        {
            if(mainSceneMusic != null)
            {
                backgroundMusic.clip = mainSceneMusic;
            }
            
            // 使用淡入效果
            StartCoroutine(FadeInMusic());
        }
        
        EnterGame();
    }
    
    // 音乐淡入效果
    private IEnumerator FadeInMusic()
    {
        backgroundMusic.volume = 0f;
        backgroundMusic.Play();
    
        float timer = 0f;
        while(timer < musicFadeInDuration)
        {
            timer += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(0f, 1f, timer / musicFadeInDuration);
            yield return null; // 确保有 yield 语句
        }
    
        backgroundMusic.volume = 1f;
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError($"视频播放错误: {message}");
        hasFinished = true;
        videoCanvas0.SetActive(false);
        EnterGame();
    }

    private void EnterGame()
    {
        Debug.Log("视频播放完毕，进入游戏！");
    
        // 确保GameManager已初始化
        if(GameManager.Instance == null)
        {
            Instantiate(Resources.Load<GameObject>("GameManagerPrefab"));
        }
    
        // 加载场景前确保怪物系统就绪
        GameManager.Instance.TrySpawnMonsters();
    
        SceneManager.LoadScene(mainSceneName);
    }

    private void OnDestroy()
    {
        if (videoPlayer0 != null)
        {
            videoPlayer0.prepareCompleted -= OnVideoPrepared;
            videoPlayer0.loopPointReached -= OnVideoFinished;
            videoPlayer0.errorReceived -= OnVideoError;
        }
    }
    
    public void PlayVideo(System.Action onComplete)
    {
        videoCanvas1.SetActive(true);
        videoPlayer1.Prepare();

        videoPlayer1.prepareCompleted += (source) =>
        {
            videoPlayer1.Play();
            videoPlayer1.loopPointReached += (source) =>
            {
                videoCanvas1.SetActive(false);
                onComplete?.Invoke(); // 调用回调函数
            };
        };
    }
}