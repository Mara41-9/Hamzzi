using UnityEngine;
using UnityEngine.UI;

public class TestUI : ViewBase
{
    [SerializeField] private Button Button_EnterGarden;
    [SerializeField] private Button Button_ExitGarden;

    private HousingViewModel _housingVM;

    private void Awake()
    {
        Button_EnterGarden.onClick.AddListener(OnClickEnterGarden);
        Button_ExitGarden.onClick.AddListener(OnClickExitGarden);

        Button_ExitGarden.gameObject.SetActive(false);
    }

    private void Start()
    {
        _housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
    }

    private void OnClickEnterGarden()
    {
        Button_ExitGarden.gameObject.SetActive(true);
        Button_EnterGarden.gameObject.SetActive(false);

        _housingVM.EnterGardenMode();
    }

    private void OnClickExitGarden()
    {
        Button_ExitGarden.gameObject.SetActive(false);
        Button_EnterGarden.gameObject.SetActive(true);

        _housingVM.EnterOverviewMode();
    }
}
