using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 序列化的类，用于表示库存中的一个槽位数据，包括槽位中的物品数据和数量
/// </summary>
[Serializable]
public class SlotData
{
    /// <summary>
    /// 槽位中包含的物品数据
    /// </summary>
    public ItemData item;
    
    /// <summary>
    /// 槽位中物品的数量，默认为0
    /// </summary>
    public int count = 0;

    /// <summary>
    /// 当槽位数据发生变化时调用的委托
    /// </summary>
    private Action OnChange;

    /// <summary>
    /// 将另一个槽位的数据移动到当前槽位
    /// </summary>
    /// <param name="data">要移动的槽位数据</param>
    public void MoveSlot(SlotData data)
    {
        this.item = data.item;
        this.count = data.count;
        OnChange?.Invoke();
    }

    /// <summary>
    /// 检查槽位是否为空
    /// </summary>
    /// <returns>如果槽位为空返回true，否则返回false</returns>
    public bool IsEmpty()
    {
        return count == 0;
    }

    /// <summary>
    /// 检查槽位是否可以添加更多物品
    /// </summary>
    /// <returns>如果可以添加物品返回true，否则返回false</returns>
    public bool CanAddItem()
    {
        return count < item.maxCount;
    }

    /// <summary>
    /// 获取槽位中还可以添加的物品数量
    /// </summary>
    /// <returns>槽位中的剩余空间</returns>
    public int GetFreeSpace()
    {
        return item.maxCount - count;
    }

    /// <summary>
    /// 向槽位中添加指定数量的物品
    /// </summary>
    /// <param name="numToAdd">要添加的物品数量，默认为1</param>
    public void Add(int numToAdd = 1)
    {
        this.count += numToAdd;
        OnChange?.Invoke();
    }

    /// <summary>
    /// 向槽位中添加一个物品，并指定数量
    /// </summary>
    /// <param name="item">要添加的物品数据</param>
    /// <param name="count">物品的数量，默认为1</param>
    public void AddItem(ItemData item,int count =1)
    {
        this.item = item;
        this.count = count;
        OnChange?.Invoke();
    }

    /// <summary>
    /// 从槽位中减少指定数量的物品
    /// </summary>
    /// <param name="numToReduce">要减少的物品数量，默认为1</param>
    public void Reduce(int numToReduce = 1)
    {
        count -= numToReduce;
        if (count == 0)
        {
            Clear();
        }
        else
        {
            OnChange?.Invoke();
        }
    }

    /// <summary>
    /// 清空槽位中的物品和数量
    /// </summary>
    public void Clear()
    {
        item = null;
        count = 0;
        OnChange?.Invoke();
    }

    /// <summary>
    /// 为槽位数据变化事件添加监听
    /// </summary>
    /// <param name="OnChange">当槽位数据发生变化时调用的委托</param>
    public void AddListener(Action OnChange)
    {
        this.OnChange = OnChange;
    }
    
    [Header("Weapon Properties")]
    public int durability = 100; // 武器耐久度
    
    public void ReduceDurability(int amount)//耐久度有问题，暂时禁用，待修复
    {
        if (item != null && item.isWeapon)
        {
            durability -= amount;
            if (durability <= 0)
            {
                Clear(); // 耐久度为0时销毁武器
            }
            OnChange?.Invoke();
        }
    }
}
