using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSettingsUI : ViewBase
{
    [SerializeField] private UIButton Button_BackgroundClose;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private UIButton Button_Logout;

    private void OnEnable()
    {
        Button_BackgroundClose.BindOnClickButtonEvent(OnClick_Close);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        Button_Logout.BindOnClickButtonEvent(LogoutInGame);
    }

    private void OnDisable()
    {
        Button_BackgroundClose.UnBindOnClickButtonEvent(OnClick_Close);
        Button_Close.UnBindOnClickButtonEvent(OnClick_Close);

        Button_Logout.UnBindOnClickButtonEvent(LogoutInGame);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseTitleSettingsUI();
    }

    private void LogoutInGame()
    {
        // 씨앗 저장
        var loginService = ServiceManager.Instance.LoginService;
        var userService = ServiceManager.Instance.UserService;

        long userUID = loginService.GetViewModel().UserUID;
        int seedCount = userService.GetUserViewModel().SeedCount;

        UserSaveData saveData = new UserSaveData();
        saveData.GoldCount = seedCount;

        userService.SaveUserAsync(userUID, saveData).Forget();

        // 씬 리로드
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
