using UnityEngine;
using UnityEngine.UI;

public class TestUI : ViewBase
{
    [SerializeField] private Button Button_EnterGarden;
    [SerializeField] private Button Button_ExitGarden;
    [SerializeField] private Button Button_EnterBuild;
    [SerializeField] private Button Button_EnterHousing;

    private HousingViewModel _housingVM;
    private BuildViewModel _buildVM;

    private void Awake()
    {
        Button_EnterGarden.onClick.AddListener(OnClickEnterGarden);
        Button_ExitGarden.onClick.AddListener(OnClickExitGarden);
        Button_EnterBuild.onClick.AddListener(OnClickEnterBuild);
        Button_EnterHousing.onClick.AddListener(OnClickEnterHousing);

        Button_ExitGarden.gameObject.SetActive(false);
    }

    private void Start()
    {
        _housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        _buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
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

    private void OnClickEnterBuild()
    {
        _housingVM.EnterOverviewMode();
        _buildVM.EnterBuildMode();
        _buildVM.SelectType = BuildType.None;

        Button_ExitGarden.gameObject.SetActive(false);
        Button_EnterGarden.gameObject.SetActive(true);

        UIManager.Instance.OpenBuildUI();
        UIManager.Instance.CloseTestUI();
    }

    private void OnClickEnterHousing()
    {
        _housingVM.EnterHousingMode();

        Button_ExitGarden.gameObject.SetActive(false);
        Button_EnterGarden.gameObject.SetActive(true);

        UIManager.Instance.OpenHousingUI();
        UIManager.Instance.CloseTestUI();
    }
}
