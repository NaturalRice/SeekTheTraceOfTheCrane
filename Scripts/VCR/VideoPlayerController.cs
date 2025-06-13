using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // 添加这行

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas;
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private string mainSceneName = "SampleScene";
    
    // 新增音频控制变量
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip mainSceneMusic;
    [SerializeField] private float musicFadeInDuration = 1.0f; // 音乐淡入时间

    private bool isVideoPrepared = false;
    private bool isVideoPlaying = false;
    private bool hasFinished = false;

    private void Start()
    {
        startScreenCanvas.SetActive(true);
        videoCanvas.SetActive(false);
        hasFinished = false;

        // 确保背景音乐一开始是停止状态
        if(backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer组件未正确设置！");
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;

        if (rawImage == null)
        {
            Debug.LogError("RawImage组件未正确设置！");
        }
        else
        {
            rawImage.texture = videoPlayer.targetTexture;
        }
    }

    private void Update()
    {
        if (!hasFinished && !isVideoPrepared && !isVideoPlaying && Input.anyKeyDown)
        {
            StartVideoPlayback();
        }
    }

    private void StartVideoPlayback()
    {
        startScreenCanvas.SetActive(false);
        videoCanvas.SetActive(true);
        videoPlayer.Prepare();
        Debug.Log("开始准备视频...");
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("视频准备完成，开始播放！");
        isVideoPrepared = true;
        videoPlayer.Play();
        isVideoPlaying = true;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        Debug.Log("视频播放完毕！");
        hasFinished = true;
        videoCanvas.SetActive(false);
        
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
        videoCanvas.SetActive(false);
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
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.errorReceived -= OnVideoError;
        }
    }
}