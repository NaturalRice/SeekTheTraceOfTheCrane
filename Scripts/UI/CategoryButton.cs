using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryButton : MonoBehaviour
{
    public CraftingRecipe.RecipeCategory category;
    
    void Start()
    {
        // 可选：自动设置按钮文本
        GetComponentInChildren<TextMeshProUGUI>().text = category.ToString();
    }
}