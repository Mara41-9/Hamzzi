using UnityEngine;
using UnityEngine.UI;

public class DecorUI : ViewBase
{
    [SerializeField] private Button Button_EnterBuild;
    [SerializeField] private Button Button_EnterHousing;
    [SerializeField] private UIButton Button_GoToInGame;

    private HousingViewModel _housingVM;
    private BuildViewModel _buildVM;

    private void Awake()
    {
        Button_EnterBuild.onClick.AddListener(OnClickEnterBuild);
        Button_EnterHousing.onClick.AddListener(OnClickEnterHousing);
    }

    private void OnEnable()
    {
        Button_GoToInGame.BindOnClickButtonEvent(OnClickGoToInGame);
    }

    private void Start()
    {
        _housingVM = ServiceManager.Instance.HousingService.GetHousingViewModel();
        _buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
    }

    private void OnClickEnterBuild()
    {
        _housingVM.TargetRoom = null;
        _housingVM.EnterOverviewMode();

        _buildVM.EnterBuildMode();
        _buildVM.SelectType = BuildType.None;

        UIManager.Instance.OpenBuildUI();
        UIManager.Instance.CloseDecorUI();
    }

    private void OnClickEnterHousing()
    {
        _housingVM.EnterHousingMode();

        UIManager.Instance.OpenHousingUI();
        UIManager.Instance.CloseDecorUI();
    }

    private void OnClickGoToInGame()
    {
        UIManager.Instance.CloseDecorUI();
        UIManager.Instance.OpenInGameUI();
    }
}
