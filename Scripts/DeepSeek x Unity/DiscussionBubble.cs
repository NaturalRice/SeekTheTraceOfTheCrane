using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 表示游戏中的对话气泡UI元素。
/// 用于显示来自用户或其他角色的消息。
/// </summary>
public class DiscussionBubble : MonoBehaviour
{
    // 用于显示消息和气泡的UI元素。
    [Header(" Elements ")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image bubbleImage;
    [SerializeField] private Sprite userBubbleSprite;

    // 用于自定义用户消息外观的设置。
    [Header(" Settings ")] 
    [SerializeField] private Color userBubbleColor;

    /// <summary>
    /// 使用消息和是否为用户消息的视觉设置来配置对话气泡。
    /// </summary>
    /// <param name="message">要显示的消息文本。</param>
    /// <param name="isUserMessage">指示该消息是否来自用户的布尔值。</param>
    public void Configure(string message, bool isUserMessage)
    {
        // 如果是用户消息，应用特定的视觉设置以区别于其他消息。
        if (isUserMessage)
        {
            bubbleImage.sprite = userBubbleSprite;
            bubbleImage.color = userBubbleColor;
            messageText.color = Color.white;
        }

        // 设置消息文本并更新网格，以确保文本正确显示。
        messageText.text = message;
        messageText.ForceMeshUpdate();
    }
}