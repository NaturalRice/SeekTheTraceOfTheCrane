using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 单例模式的物品移动处理器，用于处理游戏中的物品移动逻辑
public class ItemMoveHandler : MonoBehaviour
{
    // 单例实例属性，提供全局访问点
    public static ItemMoveHandler Instance { get; private set; }

    // 用于显示物品图标的图像组件
    private Image icon;
    // 当前选中的槽位数据
    private SlotData selectedSlotData;

    // 玩家对象，用于执行丢弃物品等操作
    private Player player;

    // 控制键是否按下的状态
    private bool isCtrlDown = false;

    // 初始化单例实例和组件引用
    private void Awake()
    {
        Instance = this;
        icon = GetComponentInChildren<Image>();
        HideIcon();
        player = GameObject.FindAnyObjectByType<Player>();
    }

    // 更新方法，用于处理物品移动、丢弃等逻辑
    private void Update()
    {
        // 更新物品图标位置
        if (icon.enabled)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), Input.mousePosition,
                null,
                out position);
            icon.GetComponent<RectTransform>().anchoredPosition = position;
        }

        // 鼠标左键点击事件处理
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ThrowItem();
            }
        }

        // 控制键按下和释放事件处理
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCtrlDown = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCtrlDown = false;
        }

        // 鼠标右键点击事件处理
        if (Input.GetMouseButtonDown(1))
        {
            ClearHandForced();
        }
    }

    // 槽位点击事件处理方法
    public void OnSlotClick(SlotUI slotui)
    {
        // 判断手上是否为空
        if (selectedSlotData != null)
        {
            // 手上不为空时的逻辑
            if (slotui.GetData().IsEmpty())
            {
                MoveToEmptySlot(selectedSlotData, slotui.GetData());
            }
            else
            {
                // 手上不为空且点击的槽位也不为空时的逻辑
                if (selectedSlotData == slotui.GetData()) return;
                else
                {
                    // 类型一致和类型不一致的逻辑
                    if (selectedSlotData.item == slotui.GetData().item)
                    {
                        MoveToNotEmptySlot(selectedSlotData, slotui.GetData());
                    }
                    else
                    {
                        SwitchData(selectedSlotData, slotui.GetData());
                    }
                }
            }
        }
        else
        {
            // 手上为空时的逻辑
            if (slotui.GetData().IsEmpty()) return;
            selectedSlotData = slotui.GetData();
            ShowIcon(selectedSlotData.item.sprite);
        }
    }

    // 隐藏物品图标的方法
    void HideIcon()
    {
        icon.enabled = false;
    }

    // 显示物品图标的方法
    public void ShowIcon(Sprite sprite)
    {
        if (icon == null || icon.gameObject == null) 
        {
            icon = CreateNewIcon(); // 动态重建图标
            return;
        }
    
        icon.sprite = sprite;
        icon.enabled = true;
    }
    
    private Image CreateNewIcon()
    {
        GameObject iconObj = new GameObject("DragIcon");
        iconObj.transform.SetParent(transform);
        // 复制原有RectTransform设置
        return iconObj.AddComponent<Image>();
    }

    // 清空手上的物品的方法
    void ClearHand()
    {
        if (selectedSlotData.IsEmpty())
        {
            HideIcon(); 
            selectedSlotData = null;
        }
    }

    // 强制清空手上的物品的方法
    void ClearHandForced()
    {
        HideIcon();
        if (icon != null && icon.gameObject != null) 
        {
            icon.enabled = false;
        }
        selectedSlotData = null;
    }

    // 丢弃物品的方法
    private void ThrowItem()
    {
        if (selectedSlotData != null)
        {
            GameObject prefab = selectedSlotData.item.prefab;
            int count = selectedSlotData.count;
            if (isCtrlDown)
            {
                player.ThrowItem(prefab, 1);
                selectedSlotData.Reduce();
            }
            else
            {
                player.ThrowItem(prefab, count);
                selectedSlotData.Clear();
            }
            ClearHand();
        }
    }

    // 移动物品到空槽位的方法
    private void MoveToEmptySlot(SlotData fromData, SlotData toData)
    {
        if (isCtrlDown)
        {
            toData.AddItem(fromData.item);
            fromData.Reduce();
        }
        else
        {
            toData.MoveSlot(fromData);
            fromData.Clear();
        }
        ClearHand();
    }

    // 移动物品到非空槽位的方法
    private void MoveToNotEmptySlot(SlotData fromData, SlotData toData)
    {
        if (isCtrlDown)
        {
            if (toData.CanAddItem())
            {
                toData.Add();
                fromData.Reduce();
            }
        }
        else
        {
            int freespace = toData.GetFreeSpace();
            if (fromData.count > freespace)
            {
                toData.Add(freespace);
                fromData.Reduce(freespace);
            }
            else
            {
                toData.Add(fromData.count);
                fromData.Clear();
            }
        }
        ClearHand();
    }

    // 交换两个槽位数据的方法
    private void SwitchData(SlotData data1, SlotData data2)
    {
        ItemData item = data1.item;
        int count = data1.count;
        data1.MoveSlot(data2);

        data2.AddItem(item, count);

        ClearHandForced();
    }
}
