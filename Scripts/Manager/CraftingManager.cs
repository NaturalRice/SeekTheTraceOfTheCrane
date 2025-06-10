using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }
    
    [Header("All Recipes")]
    public List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();
    
    private Dictionary<CraftingRecipe.RecipeCategory, List<CraftingRecipe>> categorizedRecipes;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CategorizeRecipes();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void CategorizeRecipes()
    {
        categorizedRecipes = new Dictionary<CraftingRecipe.RecipeCategory, List<CraftingRecipe>>();
        
        // 初始化所有类别
        foreach(CraftingRecipe.RecipeCategory category in System.Enum.GetValues(typeof(CraftingRecipe.RecipeCategory)))
        {
            categorizedRecipes[category] = new List<CraftingRecipe>();
        }
        
        // 分配配方到类别
        foreach(var recipe in allRecipes)
        {
            if(categorizedRecipes.ContainsKey(recipe.category))
            {
                categorizedRecipes[recipe.category].Add(recipe);
            }
        }
    }
    
    public List<CraftingRecipe> GetRecipesByCategory(CraftingRecipe.RecipeCategory category)
    {
        return categorizedRecipes.ContainsKey(category) ? categorizedRecipes[category] : new List<CraftingRecipe>();
    }
    
    // 执行合成
    public bool CraftItem(CraftingRecipe recipe)
    {
        if(recipe == null) return false;
        
        // 检查材料是否足够
        if(!recipe.CanCraft(InventoryManager.Instance.backpack))
            return false;
        
        // 消耗材料
        foreach(var material in recipe.requiredMaterials)
        {
            ConsumeMaterial(material.itemType, material.amount);
        }
        
        // 添加成品
        InventoryManager.Instance.AddToBackpack(recipe.outputItem.type, recipe.outputAmount);
        
        return true;
    }
    
    private void ConsumeMaterial(ItemType itemType, int amount)
    {
        int remaining = amount;
        foreach(var slot in InventoryManager.Instance.backpack.slotList)
        {
            if(remaining <= 0) break;
            
            if(slot.item != null && slot.item.type == itemType)
            {
                int takeAmount = Mathf.Min(slot.count, remaining);
                slot.Reduce(takeAmount);
                remaining -= takeAmount;
            }
        }
    }
}