using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance { get; private set; }
    
    // 保留对持久化对象的引用
    [Header("持久化引用")]
    public GameObject craftingPanel;
    public Transform categoryButtonsParent;
    public Transform recipeButtonsParent; // Content对象
    public GameObject recipeButtonPrefab;
    public Button craftButton;
    public Button closeButton;
    
    [Header("详情区域引用")]
    public Image recipeIcon;
    public TextMeshProUGUI recipeNameText;
    public TextMeshProUGUI recipeDescriptionText;
    public Transform materialsContainer;
    public GameObject materialSlotPrefab;
    // 不再保存动态生成的按钮引用
    private CraftingRecipe selectedRecipe;
    private Dictionary<CraftingRecipe.RecipeCategory, Button> categoryButtons = 
        new Dictionary<CraftingRecipe.RecipeCategory, Button>();
    
    // 添加初始化标志
    private bool isInitialized = false;
    
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
        
        // 改为延迟初始化
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
    
    private void InitializeIfNeeded()
    {
        if(isInitialized) return;
        
        // 确保所有必要引用有效
        if(recipeButtonsParent == null || materialsContainer == null)
        {
            Debug.LogError("关键UI引用丢失！");
            return;
        }
        
        // 绑定分类按钮
        BindExistingCategoryButtons();
        isInitialized = true;
    }
    
    //动态生成配方列表,绑定配方按钮
    public void ShowCategoryRecipes(CraftingRecipe.RecipeCategory category)
    {
        if(!ValidateReferences()) return;
        
        // 安全检查
        if(recipeButtonsParent == null)
        {
            Debug.LogError("recipeButtonsParent未设置！");
            return;
        }

        // 清除现有配方按钮（使用安全方式）
        for(int i = recipeButtonsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(recipeButtonsParent.GetChild(i).gameObject);
        }

        // 生成新按钮
        var recipes = CraftingManager.Instance.GetRecipesByCategory(category);
        foreach (var recipe in recipes)
        {
            var buttonObj = Instantiate(recipeButtonPrefab, recipeButtonsParent);
            var button = buttonObj.GetComponent<Button>();
        
            // 安全设置按钮属性
            var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if(text != null) text.text = recipe.recipeName;
        
            var image = buttonObj.GetComponent<Image>();
            if(image != null && recipe.icon != null) image.sprite = recipe.icon;
        
            button.onClick.AddListener(() => 
            {
                if(recipe != null) SelectRecipe(recipe);
            });
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
        
        // 方法实现
        Debug.Log("Selected recipe: " + recipe.recipeName);
    }
    
    private void UpdateMaterialsDisplay()
    {
        // 安全清除
        if(materialsContainer == null) return;
    
        for(int i = materialsContainer.childCount - 1; i >= 0; i--)
        {
            var child = materialsContainer.GetChild(i);
            if(child != null) Destroy(child.gameObject);
        }

        // 安全生成新材料显示
        if(selectedRecipe == null) return;
    
        foreach(var material in selectedRecipe.requiredMaterials)
        {
            if(materialSlotPrefab == null) continue;
        
            var slot = Instantiate(materialSlotPrefab, materialsContainer);
            var slotUI = slot.GetComponent<MaterialSlotUI>();
            if(slotUI == null) continue;
        
            var itemData = InventoryManager.Instance.GetItemData(material.itemType);
            if(itemData != null)
            {
                slotUI.SetMaterial(itemData, material.amount);
            }
        }
    }
    
    //合成按钮逻辑
    public void OnCraftButtonClick()
    {
        Debug.Log("合成按钮被点击"); // 确认按钮响应
    
        if(selectedRecipe == null)
        {
            Debug.LogWarning("没有选中的配方");
            return;
        }

        Debug.Log($"尝试合成: {selectedRecipe.recipeName}");
        Debug.Log($"需要材料: {string.Join(",", selectedRecipe.requiredMaterials.Select(m => $"{m.itemType}x{m.amount}"))}");

        bool success = CraftingManager.Instance.CraftItem(selectedRecipe);
    
        if(success)
        {
            Debug.Log("合成成功！");
            UpdateMaterialsDisplay();
        }
        else
        {
            Debug.Log("合成失败，请检查材料是否足够");
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
            InitializeIfNeeded();
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
    //错误预防机制，添加安全校验方法：
    private bool ValidateReferences()
    {
        bool isValid = true;
    
        if(recipeButtonsParent == null)
        {
            Debug.LogError("recipeButtonsParent未分配！");
            isValid = false;
        }
    
        if(materialsContainer == null)
        {
            Debug.LogError("materialsContainer未分配！");
            isValid = false;
        }
    
        if(recipeButtonPrefab == null)
        {
            Debug.LogError("配方按钮预制件未分配！");
            isValid = false;
        }
    
        return isValid;
    }
}