using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }
    
    [Header("UI Elements")]
    public GameObject craftingPanel;
    public Transform categoryButtonsParent;
    public Transform recipeListParent;
    public GameObject recipeButtonPrefab;
    public Button craftButton;
    
    [Header("Recipe Details")]
    public Image recipeIcon;
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Transform materialsContainer;
    public GameObject materialSlotPrefab;
    
    private CraftingRecipe selectedRecipe;
    private Dictionary<CraftingRecipe.RecipeCategory, Button> categoryButtons = new Dictionary<CraftingRecipe.RecipeCategory, Button>();
    
    private void Awake()//确保单例模式正确初始化
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        InitializeCategoryButtons();
        craftingPanel.SetActive(false);// 确保初始关闭
    }
    //配置分类按钮
    private void InitializeCategoryButtons()
    {
        // 清除现有按钮
        foreach(Transform child in categoryButtonsParent)
        {
            Destroy(child.gameObject);
        }
        
        // 创建类别按钮
        foreach(CraftingRecipe.RecipeCategory category in System.Enum.GetValues(typeof(CraftingRecipe.RecipeCategory)))
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, categoryButtonsParent);
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => ShowCategoryRecipes(category));
            
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = category.ToString();
            
            // 设置按钮文本
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
            
            categoryButtons[category] = button;
        }
    }
    //动态生成配方列表
    public void ShowCategoryRecipes(CraftingRecipe.RecipeCategory category)
    {
        // 清除现有配方列表
        foreach(Transform child in recipeListParent)
        {
            Destroy(child.gameObject);
        }
        
        // 获取当前类别的配方
        List<CraftingRecipe> recipes = CraftingManager.Instance.GetRecipesByCategory(category);
        
        // 创建配方按钮
        foreach(var recipe in recipes)
        {
            GameObject recipeButton = Instantiate(recipeButtonPrefab, recipeListParent);
            recipeButton.GetComponentInChildren<TextMeshProUGUI>().text = recipe.recipeName;
            recipeButton.GetComponent<Image>().sprite = recipe.icon;
            recipeButton.GetComponent<Button>().onClick.AddListener(() => SelectRecipe(recipe));
        }
    }
    //配置详情显示
    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        
        // 更新详情面板
        recipeIcon.sprite = recipe.icon;
        recipeNameText.text = recipe.recipeName;
        recipeDescriptionText.text = recipe.description;
        
        // 更新材料需求
        UpdateMaterialsDisplay();
        
        // 更新合成按钮状态
        craftButton.interactable = recipe.CanCraft(InventoryManager.Instance.backpack);
    }
    
    private void UpdateMaterialsDisplay()
    {
        // 清除现有材料显示
        foreach(Transform child in materialsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 添加新材料显示
        foreach(var material in selectedRecipe.requiredMaterials)
        {
            GameObject materialSlot = Instantiate(materialSlotPrefab, materialsContainer);
            MaterialSlotUI slotUI = materialSlot.GetComponent<MaterialSlotUI>();
            
            ItemData itemData = InventoryManager.Instance.GetItemData(material.itemType);
            if(itemData != null)
            {
                slotUI.SetMaterial(itemData, material.amount);
            }
        }
    }
    //合成按钮逻辑
    public void OnCraftButtonClick()
    {
        if(selectedRecipe != null)
        {
            bool success = CraftingManager.Instance.CraftItem(selectedRecipe);
            if(success)
            {
                // 更新UI
                SelectRecipe(selectedRecipe);
                // 播放成功音效(如有)
                //GameManager.Instance.PlaySound(GameManager.Instance.craftSuccessSound);
            }
            else
            {
                // 播放失败音效
                //GameManager.Instance.PlaySound(GameManager.Instance.craftFailSound);
            }
        }
    }
    //打开/关闭合成界面
    public void ToggleCraftingUI()
    {
        bool active = !craftingPanel.activeSelf;
        craftingPanel.SetActive(active);
        
        if(active)
        {
            // 初始化UI
            ShowCategoryRecipes(CraftingRecipe.RecipeCategory.Equipment);
            GameManager.Instance.canControlLuna = false;
        }
        else
        {
            GameManager.Instance.canControlLuna = true;
        }
    }
}