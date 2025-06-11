using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }
    
    [Header("UI Elements")]
    public GameObject craftingPanel;
    public Transform categoryButtonsParent; // 分类按钮的父对象
    public Transform recipeButtonsParent;   // 配方按钮的父对象
    public Transform recipeListParent;
    public GameObject recipeButtonPrefab;
    public Button craftButton;
    public Button closeButton;
    
    [Header("Recipe Details")]
    public Image recipeIcon;
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Transform materialsContainer;
    public GameObject materialSlotPrefab;
    
    private CraftingRecipe selectedRecipe;
    private Dictionary<CraftingRecipe.RecipeCategory, Button> categoryButtons = 
        new Dictionary<CraftingRecipe.RecipeCategory, Button>();
    
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
        
        // 绑定基础按钮
        craftButton.onClick.AddListener(OnCraftButtonClick);
        closeButton.onClick.AddListener(ToggleCraftingUI);
        
        // 绑定动态生成的按钮
        BindCategoryButtons();
    }
    
    private void Start()
    {
        // 改为直接绑定现有按钮
        BindExistingCategoryButtons();
        craftingPanel.SetActive(false);// 确保初始关闭(如果要通过SceneExporter来查找该面板信息时需注释此行代码！)
    }
    
    // 新方法：绑定场景中已有的分类按钮
    private void BindExistingCategoryButtons()
    {
        CategoryButton[] categoryButtons = categoryButtonsParent.GetComponentsInChildren<CategoryButton>(true);
    
        foreach (var button in categoryButtons)
        {
            Button btnComponent = button.GetComponent<Button>();
            btnComponent.onClick.RemoveAllListeners();
            btnComponent.onClick.AddListener(() => {
                ShowCategoryRecipes(button.category);
            });
        }
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
    
    //动态生成配方列表,绑定配方按钮
    public void ShowCategoryRecipes(CraftingRecipe.RecipeCategory category)
    {
        // 清除现有配方列表
        foreach(Transform child in recipeListParent)
        {
            Destroy(child.gameObject);
        }
    
        // 重置滚动位置（如果需要）
        var scrollRect = recipeListParent.GetComponentInParent<ScrollRect>();
        if(scrollRect != null && scrollRect.content != null) 
        {
            scrollRect.normalizedPosition = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("ScrollRect或Content未正确设置");
        }
    
        // 生成新的配方按钮
        var recipes = CraftingManager.Instance.GetRecipesByCategory(category);
        foreach (var recipe in recipes)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListParent); // 注意改为recipeListParent
            Button button = buttonObj.GetComponent<Button>();
    
            // 设置按钮显示
            button.GetComponentInChildren<TextMeshProUGUI>().text = recipe.recipeName;
            button.GetComponent<Image>().sprite = recipe.icon;
    
            // 绑定点击事件
            button.onClick.AddListener(() => SelectRecipe(recipe));
        }
    
        // 强制立即重新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(recipeListParent.GetComponent<RectTransform>());
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
                UpdateMaterialsDisplay(); // 刷新UI
                //PlaySuccessEffect();
            }
        }
    }
    
    //打开/关闭合成界面
    public void ToggleCraftingUI()
    {
        bool active = !craftingPanel.activeSelf;// 背景板无需单独引用，它会随父对象自动激活/禁用
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
    
    //绑定分类按钮
    private void BindCategoryButtons()
    {
        // 获取所有分类按钮
        Button[] categoryButtons = categoryButtonsParent.GetComponentsInChildren<Button>();
    
        foreach (var button in categoryButtons)
        {
            // 从按钮名称获取类别（或使用自定义组件）
            string categoryName = button.name.Replace("Button", "");
            if (System.Enum.TryParse(categoryName, out CraftingRecipe.RecipeCategory category))
            {
                button.onClick.AddListener(() => ShowCategoryRecipes(category));
            }
        }
    }
}