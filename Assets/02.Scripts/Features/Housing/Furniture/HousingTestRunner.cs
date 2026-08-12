using UnityEngine;

public class HousingTestRunner : MonoBehaviour
{
    [SerializeField] private BuildView _buildView;
    [SerializeField] private HousingView _housingView;
    [SerializeField] private HousingUI _housingUI;
    [SerializeField] private CameraController _cameraController;

    private HousingViewModel _housingVM;

    private void Start()
    {
        BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
        _buildView.BindViewModel(buildVM);

        _housingVM = new HousingViewModel();

        if (_housingView != null)
        {
            _housingView.BindViewModel(_housingVM, buildVM);
        }

        if (_housingUI != null)
        {
            _housingUI.BindViewModel(_housingVM);
        }

        if (_cameraController != null)
        {
            _cameraController.BindViewModel(_housingVM);
        }

        _housingVM.EnterHousingMode();
    }
}