using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包UI管理类，负责背包界面的显示和交互逻辑
/// </summary>
public class BackpackUI : MonoBehaviour
{
    /// <summary>
    /// 背包界面的父级UI对象
    /// </summary>
    public GameObject parentUI;

    /// <summary>
    /// 存储所有槽位UI组件的列表
    /// </summary>
    public List<SlotUI> slotuiList;
    
    public static BackpackUI Instance { get; private set; }

    /// <summary>
    /// 在Awake阶段初始化背包UI管理器
    /// </summary>
    private void Awake()
    {
        // 查找并缓存当前GameObject下的ParentUI对象
        parentUI = transform.Find("ParentUI").gameObject;
        
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
    /// 在Start阶段初始化背包UI
    /// </summary>
    private void Start()
    {
        // 初始化UI
        InitUI();
        parentUI.SetActive(false); // 确保初始关闭
    }

    /// <summary>
    /// 每帧调用一次，处理输入和更新逻辑
    /// </summary>
    void Update()
    {
        // 如果按下Esc键，则切换背包界面的显示状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleUI();
        }
    }

    /// <summary>
    /// 初始化背包UI，包括初始化槽位列表和更新UI显示
    /// </summary>
    void InitUI()
    {
        // 初始化slotuiList，长度为24
        slotuiList = new List<SlotUI>(new SlotUI[24]);  
        // 获取当前GameObject下所有的SlotUI组件
        SlotUI[] slotuiArray = transform.GetComponentsInChildren<SlotUI>();

        // 遍历所有找到的SlotUI组件，并将其添加到slotuiList中
        foreach(SlotUI slotUI in slotuiArray)
        {
            slotuiList[slotUI.index] = slotUI;
        }

        // 更新UI显示
        UpdateUI();
    }

    /// <summary>
    /// 根据InventoryManager中的数据更新背包UI显示
    /// </summary>
    public void UpdateUI()
    {
        // 从InventoryManager中获取背包槽位数据列表
        List<SlotData> slotdataList = InventoryManager.Instance.backpack.slotList;
        
        // 遍历槽位数据列表，更新对应的槽位UI组件
        for(int i = 0; i < slotdataList.Count; i++)
        {
            slotuiList[i].SetData(slotdataList[i]);
        }
    }

    /// <summary>
    /// 切换背包界面的显示状态
    /// </summary>
    private void ToggleUI()
    {
        // 设置parentUI对象的激活状态为其当前状态的反值
        parentUI.SetActive(!parentUI.activeSelf);
    }

    /// <summary>
    /// 当关闭按钮被点击时调用此方法，切换背包界面的显示状态
    /// </summary>
    public void OnCloseClick()
    {
        // 切换背包界面的显示状态
        ToggleUI();
    }
}
