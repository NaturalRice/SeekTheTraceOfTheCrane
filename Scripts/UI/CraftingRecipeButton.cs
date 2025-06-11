using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingRecipeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    
    private CraftingRecipe recipe;
    private CraftingUI craftingUI;

    public void Initialize(CraftingRecipe recipe, CraftingUI ui)
    {
        this.recipe = recipe;
        craftingUI = ui;
        
        iconImage.sprite = recipe.icon;
        nameText.text = recipe.recipeName;
        
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        craftingUI.SelectRecipe(recipe);
    }
}