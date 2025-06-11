using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingCategoryButton : MonoBehaviour
{
    public CraftingRecipe.RecipeCategory category;
    private CraftingUI craftingUI;

    public void Initialize(CraftingUI ui)
    {
        craftingUI = ui;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        craftingUI.ShowCategoryRecipes(category);
    }
}