using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None, // 无物品类型
    Seed_Carrot, // 胡萝卜种子类型
    Seed_Tomato, // 番茄种子类型
    Hoe, // 耙子工具类型
    灵气,
    齿轮,
    红宝石,
    绿宝石,
    蓝宝石,
    Coins,
    Potion,
    Candle,
    //材料
    木头,
    石头,
    铁矿,
    //武器
    破旧的短剑,
    普通的剑,
    绿宝石剑,
    蓝宝石剑,
    红宝石剑,
    //工具
    破旧的镐子,
    破旧的斧头,
}

// 创建一个ItemData类，继承自ScriptableObject，用于在Unity编辑器中创建物品数据资产
[CreateAssetMenu()]
public class ItemData :ScriptableObject
{
    // 定义物品的类型，初始值为None
    public ItemType type=ItemType.None;
    
    // 定义物品的图标，用于在界面上显示物品的图像
    public Sprite sprite;
    
    // 定义物品的预设，用于在场景中实例化物品对象
    public GameObject prefab;
    
    // 定义物品的最大数量，默认值为1，表示该物品的最大持有数量
    public int maxCount=1;
    
    //武器属性
    [Header("Weapon Properties")]
    public bool isWeapon = false; // 是否是武器
    public int damage = 10; // 武器伤害
    public float attackSpeed = 1.0f; // 攻击速度
    public int maxDurability = 100; // 必须为public
    
    //工具属性
    [Header("Tool Properties")]
    public bool isTool = false;
    public ItemType toolType; // 工具类型(Axe, Pickaxe等)
    public float toolEfficiency = 1f; // 工具效率
    public float toolDurability = 100f; // 工具耐久度
}
