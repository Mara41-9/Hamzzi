using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;

public class FriendSlotUI : UIBase
{
    [SerializeField] private TextMeshProUGUI TextMesh_FriendName;
    [SerializeField] private TextMeshProUGUI TextMesh_FriendUid; 
    [SerializeField] private UIButton Button_Visit;

    private long _friendUid = 0;

    public void SetFriendData(FriendInfoData data)
    {
        if (data != null)
        {
            TextMesh_FriendName.text = data.FriendName;
            TextMesh_FriendUid.text = data.FriendUid.ToString();
            _friendUid = data.FriendUid;
        }
    }

    private void OnEnable()
    {
        Button_Visit.BindOnClickButtonEvent(OnClickVisit);
    }

    private void OnDisable()
    {
    }

    private void OnClickVisit()
    {
        var loginVm = ServiceManager.Instance.LoginService.GetViewModel();
        ServiceManager.Instance.UserService.SaveUserAsync(loginVm.UserUID).Forget();

        UIManager.Instance.OpenLoadingUI();

        ServiceManager.Instance.VisitedUserService.CurrentVisitedUid = _friendUid;
        Debug.Log($"친구 방문. 대상 UID: {_friendUid}");

        var visitedUserVm = ServiceManager.Instance.VisitedUserService.GetViewModel();
        if (visitedUserVm == null)
        {
            return;
        }

        visitedUserVm.RequestLoadVisitedInfo();

        ServiceManager.Instance.CollectionService.LoadHamsterCollectionData(_friendUid).Forget();
        ServiceManager.Instance.CollectionService.SetCurrentCollectionViewModel(_friendUid);

        GameManager.Instance.ChangeMap(_friendUid).Forget();
    }
}