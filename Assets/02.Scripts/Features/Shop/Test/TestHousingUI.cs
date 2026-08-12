using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TestHousingUI : ViewBase
{
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private GameObject Prefab_ItemSlot;
    [SerializeField] private Transform Root_ItemSlot;

    private TestHousingViewModel _housingVm;

    private Dictionary<long, TestHousingSlotUI> _housingSlotList = new Dictionary<long, TestHousingSlotUI>();

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        FindShopViewModelAndBind();
    }

    private void OnDisable()
    {
        ClearItemList();

        if(_housingVm != null)
        {
            _housingVm.PropertyChanged -= OnPropChanged_ShopView;
        }
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseUI(UIRootType.ContentUI, UIType.TestHousingUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TestMainUI);
    }

    private void FindShopViewModelAndBind()
    {
        var housingVm = ServiceManager.Instance.HousingService.GetHousingViewModel();
        _housingVm = housingVm;

        _housingVm.PropertyChanged += OnPropChanged_ShopView;
        _housingVm.InvokeOnceOnInit();
    }

    private void OnPropChanged_ShopView(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ShopViewModel.ItemList):
                ResetItemSlotAndCreateAll();
                break;
        }
    }

    private void ResetItemSlotAndCreateAll()
    {
        foreach(var itemKv in _housingVm.ItemList)
        {
            var slotVm = itemKv.Value;

            CreateSlot(slotVm);
        }
    }

    private void CreateSlot(TestHousingSlotViewModel housingSlotVm)
    {
        var gObj = Instantiate(Prefab_ItemSlot, Root_ItemSlot);
        if(gObj == null)
        {
            return;
        }

        var slotView = gObj.GetComponent<TestHousingSlotUI>();
        if(slotView == null)
        {
            return;
        }

        slotView.BindSlotViewModel(housingSlotVm);
        _housingSlotList.Add(housingSlotVm.ItemUniqueId, slotView);
    }

    private void ClearItemList()
    {
        if(_housingSlotList.Count > 0)
        {
            foreach(var itemKv in _housingSlotList)
            {
                var slotView = itemKv.Value;
                Destroy(slotView.gameObject);
            }

            _housingSlotList.Clear();
        }
    }
}
