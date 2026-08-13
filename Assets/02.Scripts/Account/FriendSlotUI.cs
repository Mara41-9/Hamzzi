using UnityEngine;
using TMPro;

public class FriendSlotUI : UIBase
{
    [SerializeField] private TextMeshProUGUI TextMesh_FriendName;
    [SerializeField] private TextMeshProUGUI TextMesh_FriendId;
    [SerializeField] private UIButton Button_Visit;

    private string _friendId = "";

    public void SetFriendData(FriendInfoData data)
    {
        if (data != null)
        {
            TextMesh_FriendName.text = data.FriendName;
            TextMesh_FriendId.text = data.FriendId;
            _friendId = data.FriendId;
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
        Debug.Log($"방문하기 기능 대기 상태입니다. 대상 ID: {_friendId}");
    }
}