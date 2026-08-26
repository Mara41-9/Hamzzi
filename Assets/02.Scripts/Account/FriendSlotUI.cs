using UnityEngine;
using TMPro;

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
        ServiceManager.Instance.VisitedUserService.CurrentVisitedUid = _friendUid;
        Debug.Log($"방문하기 기능 대기 상태입니다. 대상 UID: {_friendUid}");
    }
}