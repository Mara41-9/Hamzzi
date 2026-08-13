using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildUI : ViewBase
{
    [SerializeField] private GameObject Panel_Vignette;
    [SerializeField] private GameObject Panel_InfoText;
    [SerializeField] private TextMeshProUGUI Text_Info;

    [SerializeField] private Button Button_Build;
    [SerializeField] private Button Button_Exit;
    [SerializeField] private Button Button_Confirm;

    private BuildViewModel _buildVM;

    private void Awake()
    {
        Button_Build.onClick.AddListener(OnClickBuild);
        Button_Exit.onClick.AddListener(OnClickClose);
        Button_Confirm.onClick.AddListener(OnClickConfirm);

        ResetUI();
    }

    public void BindViewModel(BuildViewModel buildVM)
    {
        _buildVM = buildVM;
        _buildVM.PropertyChanged += OnPropertyChanged_View;
    }

    private void OnDestroy()
    {
        _buildVM.PropertyChanged -= OnPropertyChanged_View;
    }

    private void OnPropertyChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_buildVM.SelectType):

                if (_buildVM.SelectType == BuildType.Room)
                {
                    OpenVignette();
                }
                else if (_buildVM.SelectType == BuildType.Aisle)
                {
                    EnterAisleMode();
                }

                break;

            case nameof(_buildVM.CanConfirm):
                Button_Confirm.gameObject.SetActive(_buildVM.CanConfirm);
                break;
        }
    }

    private void OpenVignette()
    {
        Panel_Vignette.SetActive(true);
    }

    private void EnterAisleMode()
    {
        Text_Info.text = "건설한 굴과 연결할 다른 굴을 터치해주세요!";
    }

    private void OnClickBuild()
    {
        if (_buildVM == null)
        {
            BindViewModel(ServiceManager.Instance.BuildService.GetBuildViewModel());
        }

        _buildVM.EnterBuildMode();

        Button_Build.gameObject.SetActive(false);
        Button_Exit.gameObject.SetActive(true);

        Panel_InfoText.SetActive(true);
        Text_Info.text = "땅을 터치해 햄스터의 새로운 보금자리를 만들어주세요!";
    }

    private void OnClickClose()
    {
        _buildVM.CancelBuildMode();
        ResetUI();
    }

    private void OnClickConfirm()
    {
        _buildVM.ConfirmBuild();
        ResetUI();
    }

    private void ResetUI()
    {
        Button_Build.gameObject.SetActive(true);
        Button_Confirm.gameObject.SetActive(false);
        Button_Exit.gameObject.SetActive(false);
        Panel_InfoText.gameObject.SetActive(false);
        Panel_Vignette.SetActive(false);
    }
}
