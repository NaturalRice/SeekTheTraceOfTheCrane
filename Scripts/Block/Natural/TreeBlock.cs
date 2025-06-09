using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBlock : InteractableBlock
{
    [Header("Tree Specific")]
    public GameObject stumpPrefab; // 树桩预制体
    public float woodDropMultiplier = 1.5f; // 木材掉落倍率
    
    protected override void Start()
    {
        base.Start();
        requiredTool = ItemType.破旧的斧头; // 需要斧头
    }
    
    protected override void Break()
    {
        // 生成树桩
        if(stumpPrefab) Instantiate(stumpPrefab, transform.position, transform.rotation);
        
        // 增加木材掉落率
        foreach(var drop in possibleDrops)
        {
            if(drop.item.type == ItemType.木头)
            {
                drop.minAmount = Mathf.RoundToInt(drop.minAmount * woodDropMultiplier);
                drop.maxAmount = Mathf.RoundToInt(drop.maxAmount * woodDropMultiplier);
            }
        }
        
        base.Break();
    }
}