using UnityEngine;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas;
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private VideoPlayer videoPlayer;
    
    private bool hasSwitched = false;

    private void Start()
    {
        startScreenCanvas.SetActive(true);
        videoCanvas.SetActive(false);
        
        // 添加事件监听
        if(videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    private void Update()
    {
        if (!hasSwitched && Input.anyKeyDown)
        {
            hasSwitched = true;
            startScreenCanvas.SetActive(false);
            videoCanvas.SetActive(true);
            videoPlayer.Prepare();
        }
    }

    // 新增视频准备完成回调
    private void OnVideoPrepared(VideoPlayer source)
    {
        videoPlayer.Play();
    }

    // 新增视频播放完成回调
    private void OnVideoFinished(VideoPlayer source)
    {
        // 这里可以添加视频播放完成后的逻辑
        Debug.Log("视频播放完成");
    }

    private void OnDestroy()
    {
        // 清理视频播放器事件
        if(videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}