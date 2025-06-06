using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using OpenAI;
using OpenAI.Chat;

public class NPCDialog : MonoBehaviour
{
    // 用于控制动画的Animator组件
    public Animator animator;
    
    public NPCMovement npcMovement; // 引用NPC的移动脚本
    
    // UI元素
    [SerializeField] private DiscussionBubble bubblePrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Transform bubblesParent;

    // 事件
    public static Action onMessageReceived;

    // 认证信息：提供Deepseek API
    [SerializeField] private string[] apiKey;
    private OpenAIClient api;

    // 设置
    [SerializeField]
    private List<Message> chatPrompts = new List<Message>();

    // NPC属性
    [Header("NPC Settings")]
    [SerializeField] public string npcName = "NPC";
    [SerializeField] private string npcRole = "Generic Role";
    [SerializeField] private string npcTask = "Generic Task";
    [SerializeField] private string npcBackground = "Generic Background";
    [SerializeField] private string npcPersonality = "Generic Personality";

    // 在第一次帧更新之前调用
    void Start()
    {
        // 创建初始消息气泡（该行用于测试）
        //CreateBubble($"你好！我叫{npcName}，是{npcRole}", false);

        if (npcName == "Player") //给玩家介绍游戏操作方法
        {
            CreateBubble($"欢迎来到灵机修真传，目前这个对话面板是游戏系统，当你遇到灵机时会开启新的独立对话面板，按鼠标左键滑动对话框可查看聊天记录", false);
            CreateBubble($"提示：按WSAD上下左右移动，Esc键打开关闭背包，在灵机面前按空格键与其对话，" +
                         $"按鼠标右键退出灵机对话；" +
                         $"按数字键选择物品栏，鼠标左键挪动物品（虽然现在还用不了）；", false);
            CreateBubble($"遇到敌人会开启战斗界面，别慌，如果连这种敌人都打不过那你也太...；" +
                            $"如果你玩累了，按delete退出游戏；" +
                         $"按Tab键打开关闭本面板。", false);
            CreateBubble($"我知道你现在动不了，别急，按一下鼠标右键就可以动了", false);
            CreateBubble($"找到你旁边的灵机小伙伴，在它面前按空格键对话，看看它有什么反应吧！", false);
            CreateBubble($"灵机很胆小，走得很慢，注意多等等它哦(〃'▽'〃)", false);
        }
        
        // 进行认证
        Authenticate();

        // 初始化设置
        Initialize();

        // 确保npcName在Start方法中被正确初始化
        //Debug.Log("NPC Name in Start: " + npcName);
    }

    /// <summary>
    /// 认证OpenAI API密钥。
    /// </summary>
    private void Authenticate()
    {
        api = new OpenAIClient(new OpenAIAuthentication(apiKey[0]));
    }

    /// <summary>
    /// 初始化聊天提示。
    /// </summary>
    private void Initialize()
    {
        Message prompt = new Message(OpenAI.Role.System, $"你是一个名为{npcName}的{npcRole}，" +
                                                         $"你的主要任务是{npcTask}。" +
                                                         $"你的背景是{npcBackground}，" +
                                                         $"性格特点是{npcPersonality}。");
        chatPrompts.Add(prompt);
    }
    
    /// <summary>
    /// 处理用户提问按钮点击事件。
    /// </summary>
    public async void AskButtonCallback()
    {
        // 创建用户消息气泡
        CreateBubble(inputField.text, true);

        Message prompt = new Message(OpenAI.Role.User, inputField.text);
        chatPrompts.Add(prompt);

        inputField.text = "";

        ChatRequest request = new ChatRequest(
            messages: chatPrompts,
            model: OpenAI.Models.Model.GPT3_5_Turbo,
            temperature: 0.2);

        try
        {
            var result = await api.ChatEndpoint.GetCompletionAsync(request);

            Message chatResult = new Message(OpenAI.Role.Assistant, result.FirstChoice.ToString());
            chatPrompts.Add(chatResult);

            // 创建回复消息气泡
            CreateBubble(result.FirstChoice.ToString(), false);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    /// <summary>
    /// 创建消息气泡。
    /// </summary>
    /// <param name="message">要显示的消息文本。</param>
    /// <param name="isUserMessage">是否为用户消息。</param>
    public void CreateBubble(string message, bool isUserMessage)
    {
        DiscussionBubble discussionBubble = Instantiate(bubblePrefab, bubblesParent);
        discussionBubble.Configure(message, isUserMessage);

        onMessageReceived?.Invoke();
    }
    
    /// <summary>
    /// 显示对话内容
    /// </summary>
    public void DisplayDialog()
    {
        // 确保npcName在DisplayDialog方法调用时已经被正确赋值
        //Debug.Log("NPC Name in DisplayDialog: " + npcName);
        UIManager.Instance.ShowDialog(npcName);
        //设置好NPC对话动画后再启用
        //animator.SetTrigger("Talk");
    }
}