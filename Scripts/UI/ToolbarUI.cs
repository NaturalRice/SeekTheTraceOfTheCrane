using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ToolbarUI 类负责管理工具栏的用户界面
/// </summary>
public class ToolbarUI : MonoBehaviour
{
    /// <summary>
    /// 工具栏槽位 UI 列表
    /// </summary>
    public List<ToolbarSlotUI> slotuiList;

    /// <summary>
    /// 当前选中的工具栏槽位 UI
    /// </summary>
    private ToolbarSlotUI selectedSlotUI;
    
    public static ToolbarUI Instance { get; private set; }

    /// <summary>
    /// 在第一次帧更新之前调用
    /// </summary>
    void Start()
    {
        // 初始化工具栏 UI
        InitUI();
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 每帧调用一次
    /// </summary>
    private void Update()
    {
        // 处理工具栏槽位选择控制
        ToolbarSelectControl();
    }

    /// <summary>
    /// 获取当前选中的槽位 UI
    /// </summary>
    /// <returns>返回当前选中的 ToolbarSlotUI 对象</returns>
    public ToolbarSlotUI GetSelectedSlotUI()
    {
        return selectedSlotUI;
    }

    /// <summary>
    /// 初始化工具栏 UI
    /// </summary>
    void InitUI()
    {
        // 初始化槽位 UI 列表，大小为 9
        slotuiList = new List<ToolbarSlotUI>(new ToolbarSlotUI[9]);
        // 获取所有子对象中类型为 ToolbarSlotUI 的组件
        ToolbarSlotUI[] slotuiArray = transform.GetComponentsInChildren<ToolbarSlotUI>();

        // 遍历获取到的槽位 UI 数组并将其添加到列表中
        foreach (ToolbarSlotUI slotUI in slotuiArray)
        {
            slotuiList[slotUI.index] = slotUI;
        }

        // 更新 UI 显示
        UpdateUI();
    }

    /// <summary>
    /// 更新工具栏 UI 显示
    /// </summary>
    public void UpdateUI()
    {
        // 从工具栏数据中获取槽位数据列表
        List<SlotData> slotdataList = InventoryManager.Instance.toolbarData.slotList;

        // 遍历槽位数据列表并更新对应的 UI
        for (int i = 0; i < slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }

    /// <summary>
    /// 处理工具栏槽位选择
    /// </summary>
    void ToolbarSelectControl()
    {
        // 遍历数字键 1-9 对应的键码
        for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
        {
            // 检查对应数字键是否被按下
            if (Input.GetKeyDown((KeyCode)i))
            {
                // 如果有选中的槽位 UI，则取消其高亮状态
                if (selectedSlotUI != null)
                {
                    selectedSlotUI.UnHighlight();
                }
                // 计算按下的数字键对应的索引
                int index = i - (int)KeyCode.Alpha1;
                // 更新选中的槽位 UI
                selectedSlotUI = slotuiList[index];
                // 高亮新选中的槽位 UI
                selectedSlotUI.Highlight();
            }
        }
    }
    
    // 添加方法获取当前选中武器
    public ItemData GetSelectedWeapon()
    {
        if (selectedSlotUI != null && 
            selectedSlotUI.GetData() != null && 
            selectedSlotUI.GetData().item != null && 
            selectedSlotUI.GetData().item.isWeapon)
        {
            return selectedSlotUI.GetData().item;
        }
        return null;
    }
}
