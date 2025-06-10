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
    
    // 可以缓存常用物品的数量
    private Dictionary<ItemType, int> itemCountCache = new Dictionary<ItemType, int>();

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
    public ItemData GetItemData(ItemType type)
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
    public void AddToBackpack(ItemType type, int amount = 1)
    {
        ItemData item = GetItemData(type);
        if (item == null) return;

        // 遍历背包中的所有槽位，尝试添加物品
        foreach(SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                int addAmount = Mathf.Min(amount, slotData.GetFreeSpace());
                slotData.Add(addAmount);
                amount -= addAmount;
            
                if(amount <= 0) return;
            }
        }

        // 如果没有合适的槽位，尝试找到空槽位添加物品
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.count == 0)
            {
                slotData.AddItem(item, Mathf.Min(amount, item.maxCount));
                amount -= Mathf.Min(amount, item.maxCount);
            
                if(amount <= 0) return;
            }
        }
        // 在AddToBackpack中添加
        Debug.Log($"Added {amount} of {type} to backpack");

        Debug.LogWarning("无法放入仓库，你的背包" + backpack + "已满。");
        
        itemCountCache.Clear(); // 清空缓存
    }
    
    public int GetItemCount(ItemType type)
    {
        if(backpack == null || backpack.slotList == null)
        {
            Debug.LogError("Inventory not initialized!");
            return 0;
        }
        
        if(itemCountCache.ContainsKey(type))
            return itemCountCache[type];
        
        int count = 0;
        foreach(var slot in backpack.slotList)
        {
            if(slot.item != null && slot.item.type == type)
            {
                count += slot.count;
            }
        }
        // 在GetItemCount中添加
        Debug.Log($"Counted {count} of {type} in backpack");
        
        itemCountCache[type] = count;
        return count;
    }
    
    private void OnApplicationQuit()
    {
        ResetWeaponDurability();
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        // 仅在编辑器模式下退出Play Mode时重置
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && 
            !UnityEditor.EditorApplication.isPlaying)
        {
            ResetWeaponDurability();
        }
    }
#endif
    //每次测试完毕后重置武器耐久度
    private void ResetWeaponDurability()
    {
        foreach (SlotData slot in backpack.slotList)
        {
            if (slot?.item?.isWeapon == true)
            {
                slot.durability = slot.item.maxDurability;
                slot.NotifyChange(); // 或 TriggerChange() 根据方案选择
            }
        }
    
        foreach (SlotData slot in toolbarData.slotList)
        {
            if (slot?.item?.isWeapon == true)
            {
                slot.durability = slot.item.maxDurability;
                slot.NotifyChange();
            }
        }
        Debug.Log("武器耐久度已重置");
    }
}
