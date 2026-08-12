using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSubCategoryUI : MonoBehaviour
{
    [SerializeField] private UIButton Button_SubCategory;
    [SerializeField] private TMP_Text Text_CategoryName;

    public string SubCategory { get; private set; }

    private Color _defaultTextColor;

    public event Action<string> OnClickSubCategory;

    public void Awake()
    {
        _defaultTextColor = Text_CategoryName.color;
    }

    public void OnEnable()
    {
        Button_SubCategory.BindOnClickButtonEvent(OnClick_SubCategory);
    }

    private void OnClick_SubCategory()
    {
        OnClickSubCategory?.Invoke(SubCategory);
    }

    public void SetSubCategory(string subCategory)
    {
        SubCategory = subCategory;
        Text_CategoryName.text = subCategory;
    }

    public void BindSubCategorySelectEvent(Action<string> onClickSubCategory)
    {
        OnClickSubCategory += onClickSubCategory;
    }

    public void SetSelected(bool isSelected)
    {
        if(isSelected == true)
        {
            Text_CategoryName.color = Color.black;
        }
        else
        {
            Text_CategoryName.color = _defaultTextColor;
        }
    }
}
