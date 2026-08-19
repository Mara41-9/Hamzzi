using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaView : ViewBase
{
    [Header("UI Base")]
    [SerializeField] private Button ExitButton;

    [Header("가챠 버튼")]
    [SerializeField] private Button DrawOneButton;
    [SerializeField] private Button DrawTenButton;

    [Header("가챠 결과창 UI")]
    [SerializeField] private GachaResultView GachaResultView;

    private CollectionViewModel _collectionViewModel;

    private void Start()
    {
        _collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
    }

    private void OnEnable()
    {
        ExitButton.onClick.AddListener(CloseCollectionUI);

        DrawOneButton.onClick.AddListener(DrawOneHamster);
        DrawTenButton.onClick.AddListener(DrawTenHamster);
    }

    private void OnDisable()
    {
        ExitButton.onClick.RemoveListener(CloseCollectionUI);

        DrawOneButton.onClick.RemoveListener(DrawOneHamster);
        DrawTenButton.onClick.RemoveListener(DrawTenHamster);
    }

    private void CloseCollectionUI()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.GachaUI);
    }

    private string DrawHamster()
    {
        HamsterSave hamsterSave = ServiceManager.Instance.GachaService.DrawGacha();
        _collectionViewModel.AddCollectedHamsterList(hamsterSave);
        Debug.Log($"{hamsterSave.HamsterId}, {hamsterSave.FaceId} ");

        return hamsterSave.HamsterId;
    }

    private void DrawOneHamster()
    {
        List<string> drawHamsterIdList = new List<string>();

        string hamsterId = DrawHamster();
        drawHamsterIdList.Add(hamsterId);

        GachaResultView.gameObject.SetActive(true);
        GachaResultView.ShowGachaResult(drawHamsterIdList);
    }

    private void DrawTenHamster()
    {
        List<string> drawHamsterIdList = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            string hamsterId = DrawHamster();
            drawHamsterIdList.Add(hamsterId);
        }

        GachaResultView.gameObject.SetActive(true);
        GachaResultView.ShowGachaResult(drawHamsterIdList);
    }
}
