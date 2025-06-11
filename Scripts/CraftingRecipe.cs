using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    [TextArea] public string description;
    public Sprite icon;
    
    [Header("Input Materials")]
    public List<MaterialRequirement> requiredMaterials;
    
    [System.Serializable]
    public class MaterialRequirement
    {
        public ItemType itemType;
        public int amount;
    }
    
    [Header("Output Item")]
    public ItemData outputItem;
    public int outputAmount = 1;
    
    [Header("Category")]
    public RecipeCategory category;
    
    public enum RecipeCategory
    {
        Weapon,
        Equipment,
        Tools,
        Building,
        Material,
        Potion,
        Hp,
        Mp,
    }
    
    // 检查玩家是否有足够材料
    public bool CanCraft(InventoryData inventory)
    {
        foreach(var material in requiredMaterials)
        {
            if(!HasEnoughMaterial(inventory, material.itemType, material.amount))
                return false;
        }
        return true;
    }
    
    private bool HasEnoughMaterial(InventoryData inventory, ItemType itemType, int requiredAmount)
    {
        int totalCount = 0;
        foreach(var slot in inventory.slotList)
        {
            if(slot.item != null && slot.item.type == itemType)
            {
                totalCount += slot.count;
            }
        }
        return totalCount >= requiredAmount;
    }
}