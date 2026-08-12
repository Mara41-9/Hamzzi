using UnityEngine;
using UnityEngine.UI;

public class GachaView : ViewBase
{
    [Header("UI Base")]
    [SerializeField] private Button ExitButton;

    [SerializeField] private Button DrawOneButton;
    [SerializeField] private Button DrawTenButton;

    private CollectionViewModel _collectionViewModel;

    private void Start()
    {
        _collectionViewModel = NetworkManager_YMH.Instance.CollectionService.GetCollectionViewModel();
    }

    private void OnEnable()
    {
        ExitButton.onClick.AddListener(CloseCollectionUI);

        DrawOneButton.onClick.AddListener(DrawHamster);
        DrawTenButton.onClick.AddListener(DrawTenHamster);
    }

    private void OnDisable()
    {
        ExitButton.onClick.RemoveListener(CloseCollectionUI);

        DrawOneButton.onClick.RemoveListener(DrawHamster);
        DrawTenButton.onClick.RemoveListener(DrawTenHamster);
    }

    private void CloseCollectionUI()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.GachaUI);
    }

    private void DrawHamster()
    {
        string hamsterId = NetworkManager_YMH.Instance.GachaService.DrawGacha();
        Debug.Log(hamsterId);

        if (_collectionViewModel.CollectedHamsterIdList.Contains(hamsterId) == false)
        {
            _collectionViewModel.CollectedHamsterIdList.Add(hamsterId);
        }
    }

    private void DrawTenHamster()
    {
        for(int i = 0; i < 10; i++)
        {
            DrawHamster();
        }
    }
}
