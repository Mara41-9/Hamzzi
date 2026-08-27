using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HamsterOwnerType
{
    None,
    User,
    Friend,
}

public class CrossView : ViewBase
{
    [Header("버튼")]
    [SerializeField] private UIButton ExitButton;
    [SerializeField] private UIButton MyHamsterSelectButton;
    [SerializeField] private UIButton FriendHamsterSelectButton;
    [SerializeField] private UIButton CrossButton;

    [Header("햄스터 선택")]
    [SerializeField] private CrossHamsterSelectView CrossHamsterSelectView;
    [SerializeField] private Sprite BaseHamsterSprite;
    [SerializeField] private Image MyHamsterImage;
    [SerializeField] private Image FriendHamsterImage;

    [Header("결과")]
    [SerializeField] private CrossResultView CrossResultView;

    private string _userHamsterId;
    private string _friendHamsterId;

    private HamsterViewModel _hamsterViewModel;

    public void OpenUI()
    {
        UIManager.Instance.OpenUI(UIRootType.PopupUI, UIType.CrossUI);
    }

    private void Start()
    {
        _hamsterViewModel = ServiceManager.Instance.CollectionService.GetHamsterViewModel();
    }

    private void OnEnable()
    {
        // 버튼 등록
        ExitButton.BindOnClickButtonEvent(OnClickExitButton);
        MyHamsterSelectButton.BindOnClickButtonEvent(OnClickMyHamsterSelectButton);
        FriendHamsterSelectButton.BindOnClickButtonEvent(OnClickFirendHamsterSelectButton);
        CrossButton.BindOnClickButtonEvent(OnClickCrossButton);

        // 이벤트 등록
        CrossHamsterSelectView.OnSlotSelect += OnSelectHamster;

        // 팝업창들 비활성화 
        CrossHamsterSelectView.gameObject.SetActive(false);
        CrossResultView.gameObject.SetActive(false);

        // 햄스터 선택 초기화
        _userHamsterId = string.Empty;
        _friendHamsterId = string.Empty;

        // 이미지 변경
        MyHamsterImage.sprite = BaseHamsterSprite;
        FriendHamsterImage.sprite = BaseHamsterSprite;

        LockCrossButton();
    }

    private void OnDisable()
    {
        ExitButton.UnBindOnClickButtonEvent(OnClickExitButton);
        MyHamsterSelectButton.UnBindOnClickButtonEvent(OnClickMyHamsterSelectButton);
        FriendHamsterSelectButton.UnBindOnClickButtonEvent(OnClickFirendHamsterSelectButton);
        CrossButton.UnBindOnClickButtonEvent(OnClickCrossButton);

        CrossHamsterSelectView.OnSlotSelect -= OnSelectHamster;
    }

    private void OnClickExitButton()
    {
        UIManager.Instance.CloseUI(UIRootType.PopupUI, UIType.CrossUI);
    }

    private void OnSelectHamster(string hamsterId, HamsterOwnerType ownerType)
    {
        Image iconImage = null;

        switch (ownerType)
        {
            case HamsterOwnerType.User:
                iconImage = MyHamsterImage;
                _userHamsterId = hamsterId;
                break;
            case HamsterOwnerType.Friend:
                iconImage = FriendHamsterImage;
                _friendHamsterId = hamsterId;
                break;
        }

        UpdateIcon(hamsterId, iconImage).Forget();
        LockCrossButton();
    }

    private async UniTask UpdateIcon(string hamsterId, Image iconImage)
    {
        HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
        if (hamsterData == null)
            return;

        var icon = await ResourceManager.Instance.LoadAsset<Sprite>(hamsterData.IconPath);
        iconImage.sprite = icon;
    }

    private void OnClickMyHamsterSelectButton()
    {
        long userUID = ServiceManager.Instance.LoginService.GetViewModel().UserUID;
        CrossHamsterSelectView.OpenSelectView(userUID, HamsterOwnerType.User);
    }

    private void OnClickFirendHamsterSelectButton()
    {
        long friendUID = ServiceManager.Instance.VisitedUserService.CurrentVisitedUid;
        CrossHamsterSelectView.OpenSelectView(friendUID, HamsterOwnerType.Friend);
    }

    private void OnClickCrossButton()
    {
        List<string> faceDataList = _hamsterViewModel.AllFaceIdList;
        int faceCount = faceDataList.Count;

        int randomFace = Random.Range(0, faceCount);
        int randomHamster = Random.Range(0, 2);

        string faceId = faceDataList[randomFace];
        string hamsterId = randomHamster == 0 ? _userHamsterId : _friendHamsterId;

        HamsterSave hamsterSave = new HamsterSave();
        hamsterSave.HamsterUID = GameUtil.GenerateUID();
        hamsterSave.HamsterId = hamsterId;
        hamsterSave.FaceId = faceId;
        hamsterSave.UserUID = ServiceManager.Instance.LoginService.GetViewModel().UserUID;

        CrossResultView.gameObject.SetActive(true);
        CrossResultView.PlayGachaResult(hamsterId, faceId);

        var collectionViewModel = ServiceManager.Instance.CollectionService.GetCollectionViewModel(hamsterSave.UserUID);
        collectionViewModel.AddCollectedHamsterList(hamsterSave);
    }

    private void LockCrossButton()
    {
        // 햄스터를 선택하지 않은 경우
        bool isAllHamsterSelected = _userHamsterId == string.Empty || _friendHamsterId == string.Empty;
        CrossButton.SetInteractable(isAllHamsterSelected == false);

        // 교배 횟수를 다 사용했을 경우

    }
}
