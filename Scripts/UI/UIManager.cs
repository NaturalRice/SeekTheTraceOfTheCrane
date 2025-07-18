using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Image hpMaskImage;
    public Image mpMaskImage;
    private float originalSize; // 血条原始宽度
    public GameObject battlePanelGo;
    
    public GameObject TalkPanelGo0;//玩家
    public GameObject TalkPanelGo1;//灵机1
    public GameObject TalkPanelGo2;//参观者1
    public GameObject TalkPanelGo3;//参观者2
    public GameObject TalkPanelGo4;//参观者3
    public GameObject TalkPanelGo5;//参观者4
    public GameObject TalkPanelGo6;//程老
    
    public Image hpColorImage; // 新增-用于颜色变化的血条图
    
    // 面板引用
    public GameObject questPanel;
    public GameObject guidePanel;
    // 按钮引用
    public Button questButton;
    public Button guideButton;
    public Button questPanelCloseButton;
    public Button guidePanelCloseButton;

    void Awake()
    {
        // 确保所有对话面板初始关闭
        TalkPanelGo0.SetActive(false);
        
        // 默认隐藏面板
        questPanel.SetActive(false);
        guidePanel.SetActive(false);
        
        originalSize = hpMaskImage.rectTransform.rect.width;
        SetHPValue(1);
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 显示任务面板
    public void ShowQuestPanel()
    {
        questPanel.SetActive(true);
    }

    // 隐藏任务面板
    public void HideQuestPanel()
    {
        questPanel.SetActive(false);
    }

    // 显示指南面板
    public void ShowGuidePanel()
    {
        guidePanel.SetActive(true);
    }

    // 隐藏指南面板
    public void HideGuidePanel()
    {
        guidePanel.SetActive(false);
    }
    
    private void Start()
    {
        // 绑定按钮事件
        questButton.onClick.AddListener(ShowQuestPanel);
        guideButton.onClick.AddListener(ShowGuidePanel);

        // 绑定关闭按钮事件
        questPanelCloseButton.onClick.AddListener(HideQuestPanel);
        guidePanelCloseButton.onClick.AddListener(HideGuidePanel);
    }

    /// <summary>
    /// 血条UI填充显示
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void SetHPValue(float fillPercent)
    {
        // 1. 更新血条长度
        hpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, 
            fillPercent * originalSize
        );
        
        // 2. 动态调整血条颜色
        UpdateHPColor(fillPercent);
    }
    
    private void UpdateHPColor(float fillPercent)
    {
        Color targetColor;
        
        if (fillPercent > 0.5f)
        {
            // 血量>50%：绿→黄渐变
            float lerpValue = (fillPercent - 0.5f) * 2; // 映射到0-1
            targetColor = Color.Lerp(Color.yellow, Color.green, lerpValue);
        }
        else
        {
            // 血量≤50%：黄→红渐变
            float lerpValue = fillPercent * 2; // 映射到0-1
            targetColor = Color.Lerp(Color.red, Color.yellow, lerpValue);
        }

        hpColorImage.color = targetColor;
    }

    /// <summary>
    /// 蓝条UI填充显示
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void SetMPValue(float fillPercent)
    {
        mpMaskImage.rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }

    public void ShowOrHideBattlePanel(bool show)
    {
        battlePanelGo.SetActive(show);
    }
    
    void Update()
    {
        // 检查玩家是否按下了Esc键
        if (Input.GetKeyDown(KeyCode.Tab))
            if(GameManager.Instance.canControlLuna)
                TalkPanelGo0.SetActive(!TalkPanelGo0.activeSelf);//玩家面板可随时打开关闭

        // 检查玩家是否按下了Delete键
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            QuitGame();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            // 关闭对话面板
            TalkPanelGo1.SetActive(false);
            TalkPanelGo2.SetActive(false);
            TalkPanelGo3.SetActive(false);
            TalkPanelGo4.SetActive(false);
            TalkPanelGo5.SetActive(false);
            TalkPanelGo6.SetActive(false);
            // 允许玩家继续控制角色
            GameManager.Instance.canControlLuna = true;
            //允许NPC活动
            GameManager.Instance.canWalkingNPC = true;
        }
    }
    
    public bool IsAnyPanelOpen()
    {
        // 检查所有可能的面板是否开启
        return TalkPanelGo0.activeSelf || 
               TalkPanelGo1.activeSelf ||
               TalkPanelGo2.activeSelf ||
               TalkPanelGo3.activeSelf ||
               TalkPanelGo4.activeSelf ||
               TalkPanelGo5.activeSelf ||
               TalkPanelGo6.activeSelf ||
               (BackpackUI.Instance != null && BackpackUI.Instance.parentUI.activeSelf);
    }

    /// <summary>
    /// 显示对话内容（包含人物的切换，名字的更换，对话内容的更换）
    /// </summary>
    /// <param name="name"></param>
    public void ShowDialog(string name)
    {
        switch (name)
        {
            case "灵机1":
                TalkPanelGo1.SetActive(true);
                break;
            case "参观者1":
                TalkPanelGo2.SetActive(true);
                break;
            case "参观者2":
                TalkPanelGo3.SetActive(true);
                break;
            case "参观者3":
                TalkPanelGo4.SetActive(true);
                break;
            case "参观者4":
                TalkPanelGo5.SetActive(true);
                break;
            case "程老":
                TalkPanelGo6.SetActive(true);
                break;
            default:
                TalkPanelGo0.SetActive(true);
                break;
        }
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器模式下停止播放
#else
    Application.Quit(); // 在发布的游戏中退出
#endif
    }
}