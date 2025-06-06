using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理游戏中的物品库存系统。
/// </summary>
public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// 获取 InventoryManager 的单例实例。
    /// </summary>
    public static InventoryManager Instance { get; private set; }

    /// <summary>
    /// 存储物品类型和物品数据的字典。
    /// </summary>
    private Dictionary<ItemType, ItemData> itemDataDict = new Dictionary<ItemType, ItemData>();

    /// <summary>
    /// 背包的库存数据。
    /// </summary>
    [HideInInspector]
    public InventoryData backpack;

    /// <summary>
    /// 工具栏的库存数据。
    /// </summary>
    [HideInInspector]
    public InventoryData toolbarData;

    /// <summary>
    /// 在对象初始化时调用，设置 InventoryManager 的单例实例并初始化数据。
    /// </summary>
    private void Awake()
    {
        Instance = this;
        Init();
    }

    /// <summary>
    /// 初始化物品数据和库存数据。
    /// </summary>
    private void Init()
    {
        // 从资源中加载所有物品数据并填充字典
        ItemData[] itemDataArray = Resources.LoadAll<ItemData>("Data");
        foreach(ItemData data in itemDataArray)
        {
            itemDataDict.Add(data.type, data);
        }

        // 加载背包和工具栏的库存数据
        backpack = Resources.Load<InventoryData>("Data/Backpack");
        toolbarData = Resources.Load<InventoryData>("Data/Toolbar");
    }

    /// <summary>
    /// 根据物品类型获取物品数据。
    /// </summary>
    /// <param name="type">物品类型。</param>
    /// <returns>物品数据，如果找不到则返回 null。</returns>
    private ItemData GetItemData(ItemType type)
    {
        ItemData data;
        bool isSuccess = itemDataDict.TryGetValue(type, out data);
        if (isSuccess)
        {
            return data;
        }
        else
        {
            Debug.LogWarning("你传递的type：" + type + "不存在，无法得到物品信息。");
            return null;
        }
    }

    /// <summary>
    /// 将物品添加到背包中。
    /// </summary>
    /// <param name="type">要添加的物品类型。</param>
    public void AddToBackpack(ItemType type)
    {
        ItemData item = GetItemData(type);
        if (item == null) return;

        // 遍历背包中的所有槽位，尝试添加物品
        foreach(SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                slotData.Add();
                return;
            }
        }

        // 如果没有合适的槽位，尝试找到空槽位添加物品
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.count == 0)
            {
                slotData.AddItem(item);
                return;
            }
        }

        Debug.LogWarning("无法放入仓库，你的背包" + backpack + "已满。");
    }
}
