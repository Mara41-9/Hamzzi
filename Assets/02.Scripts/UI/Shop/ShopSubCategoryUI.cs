using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSubCategoryUI : MonoBehaviour
{
    [SerializeField] private UIButton Button_SubCategory;
    [SerializeField] private TMP_Text Text_CategoryName;

    private string _subCategory;

    public event Action<string> OnClickSubCategory;

    public void OnEnable()
    {
        Button_SubCategory.BindOnClickButtonEvent(OnClick_SubCategory);
    }

    private void OnClick_SubCategory()
    {
        OnClickSubCategory?.Invoke(_subCategory);
    }

    public void SetSubCategory(string subCategory)
    {
        _subCategory = subCategory;
        Text_CategoryName.text = subCategory;
    }

    public void BindSubCategorySelectEvent(Action<string> onClickSubCategory)
    {
        OnClickSubCategory += onClickSubCategory;
    }
}
