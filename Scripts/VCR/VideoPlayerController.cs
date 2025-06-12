using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas; // 引用开始界面的Canvas
    [SerializeField] private GameObject videoCanvas; // 引用视频播放界面的Canvas
    [SerializeField] private VideoPlayer videoPlayer; // 引用VideoPlayer组件
    [SerializeField] private RawImage rawImage; // 引用RawImage组件

    private bool isVideoPlaying = false;

    private void Start()
    {
        // 确保开始界面在游戏开始时显示
        startScreenCanvas.SetActive(true);
        videoCanvas.SetActive(false);

        // 确保VideoPlayer组件已正确配置
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer组件未正确设置！");
        }
        else
        {
            videoPlayer.prepareCompleted += OnVideoPrepared; // 添加准备完成事件
        }

        // 确保RawImage组件已正确配置
        if (rawImage == null)
        {
            Debug.LogError("RawImage组件未正确设置！");
        }
        else
        {
            rawImage.texture = videoPlayer.targetTexture; // 设置RawImage的Texture
        }
    }

    private void Update()
    {
        // 检测玩家是否按下任意键
        if (Input.anyKeyDown && !isVideoPlaying)
        {
            // 隐藏开始界面
            startScreenCanvas.SetActive(false);
            // 显示视频播放界面
            videoCanvas.SetActive(true);

            // 准备并播放视频
            videoPlayer.Prepare();
            isVideoPlaying = true;
        }

        // 检测视频是否播放完毕
        if (isVideoPlaying && !videoPlayer.isPlaying)
        {
            // 隐藏视频播放界面
            videoCanvas.SetActive(false);
            // 进入游戏主场景
            EnterGame();
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        Debug.Log("视频准备完成，开始播放！");
        videoPlayer.Play();
    }

    private void EnterGame()
    {
        // 这里可以添加进入游戏主场景的逻辑
        Debug.Log("视频播放完毕，进入游戏！");
        // 例如加载主场景
        // SceneManager.LoadScene("MainScene");
    }
}