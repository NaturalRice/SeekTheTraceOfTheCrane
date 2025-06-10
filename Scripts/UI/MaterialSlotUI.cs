using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI nameText;
    public Color sufficientColor = Color.white;
    public Color insufficientColor = Color.red;
    
    public void SetMaterial(ItemData itemData, int requiredAmount)
    {
        icon.sprite = itemData.sprite;
        nameText.text = itemData.name;
        
        // 获取玩家拥有的数量
        int playerAmount = InventoryManager.Instance.GetItemCount(itemData.type);
        
        // 设置数量和颜色
        amountText.text = $"{playerAmount}/{requiredAmount}";
        amountText.color = playerAmount >= requiredAmount ? sufficientColor : insufficientColor;
    }
}