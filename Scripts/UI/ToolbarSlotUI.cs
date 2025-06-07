using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 表示工具栏槽位的UI元素，继承自SlotUI类。
/// 该类负责管理工具栏槽位在高亮和取消高亮时的视觉效果。
/// </summary>
public class ToolbarSlotUI : SlotUI
{
    /// <summary>
    /// 用于显示槽位处于高亮状态时的Sprite。
    /// </summary>
    public Sprite slotLight;

    /// <summary>
    /// 用于显示槽位处于非高亮状态时的Sprite。
    /// </summary>
    public Sprite slotDark;

    /// <summary>
    /// 引用当前UI元素的Image组件，用于更改其外观。
    /// </summary>
    private Image image;

    /// <summary>
    /// 在对象创建时初始化必要的组件。
    /// </summary>
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// 将槽位的外观更改为高亮状态。
    /// </summary>
    public void Highlight()
    {
        image.sprite = slotDark;
    }

    /// <summary>
    /// 将槽位的外观更改为非高亮状态。
    /// </summary>
    public void UnHighlight()
    {
        image.sprite = slotLight;
    }
    
    public Image cooldownOverlay;
    
    //攻击冷却指示器
    /*private void Update()
    {
        if (cooldownOverlay != null && GetData() != null && GetData().item != null && GetData().item.isWeapon)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                float cooldownPercent = Mathf.Clamp01((Time.time - player.lastAttackTime) / player.attackCooldown);
                cooldownOverlay.fillAmount = 1 - cooldownPercent;
            }
        }
    }*/
}