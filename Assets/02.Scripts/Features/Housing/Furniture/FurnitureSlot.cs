using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureSlot : MonoBehaviour
{
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Button Button_Select;
    [SerializeField] private TextMeshProUGUI Text_Count;

    private ItemData _data;
    private HousingViewModel _housingVM;

    private void Awake()
    {
        Button_Select.onClick.AddListener(OnClickSelect);
    }

    public async UniTask Bind(ItemData data, HousingViewModel housingVM)
    {
        _data = data;
        _housingVM = housingVM;

        Image_Icon.sprite = await ResourceManager.Instance.LoadAsset<Sprite>(_data.IconPath);
        
        //인벤토리 이후에 추가
        //Text_Count.text = $"{}";
    }

    private void OnClickSelect()
    {
        if (_housingVM.CurrentViewMode ==HousingViewMode.Garden)
        {
            _housingVM.SelectGardenFurniture(_data.Id, Vector2Int.one);
        }
        else if (_housingVM.TargetRoom != null)
        {
            _housingVM.SelectFurniture(_data.Id, Vector2Int.one, _housingVM.TargetRoom);
        }
    }
}
