using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelUI : ViewBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_Confirm;
    [SerializeField] private Button Button_Skip;

    [SerializeField] private Image Image_PrevHamster;
    [SerializeField] private Image Image_NextHamster;
    [SerializeField] private TextMeshProUGUI Text_PrevInfo;
    [SerializeField] private TextMeshProUGUI Text_NextInfo;
    [SerializeField] private TextMeshProUGUI Text_PrevDescription;
    [SerializeField] private TextMeshProUGUI Text_NextDescription;

    [SerializeField] private Transform Parent_Hamsters;

    private void Start()
    {
        
    }
}
