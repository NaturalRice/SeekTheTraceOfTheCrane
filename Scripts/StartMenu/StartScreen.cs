using UnityEngine;
using UnityEngine.Video;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private GameObject startScreenCanvas; // 引用开始界面的Canvas
    [SerializeField] private GameObject videoCanvas; // 引用视频播放界面的Canvas
    [SerializeField] private VideoPlayer videoPlayer; // 引用VideoPlayer组件

    private void Start()
    {
        // 确保开始界面在游戏开始时显示
        startScreenCanvas.SetActive(true);
        videoCanvas.SetActive(false);
    }

    private void Update()
    {
        // 检测玩家是否按下任意键
        if (Input.anyKeyDown)
        {
            // 隐藏开始界面
            startScreenCanvas.SetActive(false);
            // 显示视频播放界面
            videoCanvas.SetActive(true);

            // 准备并播放视频
            videoPlayer.Prepare();
            videoPlayer.Play();
        }
    }
}