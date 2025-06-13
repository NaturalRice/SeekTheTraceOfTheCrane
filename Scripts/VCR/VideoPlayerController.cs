using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 添加场景管理命名空间

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas;
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private string mainSceneName = "MainScene"; // 主场景名称

    private bool isVideoPrepared = false;
    private bool isVideoPlaying = false;
    private bool hasFinished = false; // 新增：标记流程是否已完成

    private void Start()
    {
        // 确保只显示开始界面
        startScreenCanvas.SetActive(true);
        videoCanvas.SetActive(false);
        hasFinished = false;

        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer组件未正确设置！");
            return;
        }

        // 设置视频播放器
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        // 添加事件监听
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
        // 仅在未开始流程且未完成时检测按键
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
        hasFinished = true; // 标记流程已完成
        videoCanvas.SetActive(false);
        EnterGame();
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError($"视频播放错误: {message}");
        hasFinished = true; // 即使出错也标记为已完成
        videoCanvas.SetActive(false);
        EnterGame();
    }

    private void EnterGame()
    {
        Debug.Log("视频播放完毕，进入游戏！");
        
        // 加载主场景
        if (!string.IsNullOrEmpty(mainSceneName))
        {
            SceneManager.LoadScene(mainSceneName);
        }
        
        // 禁用此脚本防止重复执行
        enabled = false;
        
        // 可选：销毁这个控制器对象
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 清理事件监听
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.errorReceived -= OnVideoError;
        }
    }
}