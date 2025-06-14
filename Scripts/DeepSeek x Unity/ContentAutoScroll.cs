using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ContentAutoScroll 类用于自动将内容滚动到底部。
// 通常用于聊天窗口或消息框等 UI 元素，以确保新消息始终可见。
public class ContentAutoScroll : MonoBehaviour
{
    // 内容要滚动的 RectTransform 组件。
    private RectTransform rt;
    
    // Start 方法在第一次帧更新之前被调用。
    // 它初始化 RectTransform 组件并订阅 DiscussionManager 的 onMessageReceived 事件。
    void Start()
    {
        // 获取当前 GameObject 的 RectTransform 组件。
        rt = GetComponent<RectTransform>();

        // 订阅 DiscussionManager 的 onMessageReceived 事件，在收到新消息时触发 DelayScrollDown 方法。
        NPCDialog.onMessageReceived += DelayScrollDown;
    }

    // OnDestroy 方法在对象即将被销毁时调用。
    // 它取消订阅 DiscussionManager 的 onMessageReceived 事件。
    private void OnDestroy()
    {
        // 取消订阅 DiscussionManager 的 onMessageReceived 事件，防止内存泄漏。
        NPCDialog.onMessageReceived -= DelayScrollDown;
    }

    // DelayScrollDown 方法用于延迟滚动内容到底部。
    // 它使用 Invoke 方法在 0.3 秒后调用 ScrollDown 方法。
    private void DelayScrollDown()
    {
        // 在 0.3 秒后调用 ScrollDown 方法。
        Invoke("ScrollDown", .3f);
    }

    // ScrollDown 方法用于将内容滚动到底部。
    // 它调整 RectTransform 的 anchoredPosition 的 y 坐标，确保内容滚动到底部。
    private void ScrollDown()
    {
        // 获取当前的 anchoredPosition。
        Vector2 anchoredPosition = rt.anchoredPosition;
        // 计算新的 y 坐标，确保其不小于 0。
        anchoredPosition.y = Mathf.Max(0, rt.sizeDelta.y);
        // 设置新的 anchoredPosition。
        rt.anchoredPosition = anchoredPosition;
    }
}