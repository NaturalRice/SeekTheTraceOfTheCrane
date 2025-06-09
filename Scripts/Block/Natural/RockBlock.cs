using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBlock : InteractableBlock
{
    [Header("Ore Specific")]
    public MeshRenderer oreRenderer;
    public Color[] healthLevelColors; // 根据血量变化的颜色
    
    protected override void Start()
    {
        base.Start();
        requiredTool = ItemType.破旧的镐子; // 需要镐
    }
    
    protected override void UpdateBlockState()
    {
        if(oreRenderer && healthLevelColors.Length > 0)
        {
            int level = Mathf.FloorToInt((currentHealth / maxHealth) * healthLevelColors.Length);
            oreRenderer.material.color = healthLevelColors[Mathf.Clamp(level, 0, healthLevelColors.Length-1)];
        }
    }
}