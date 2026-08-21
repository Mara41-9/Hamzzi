//게임 전체 흐름 제어 매니저 - 게임 종료, 실행, 로그인 넘어가는 과정 처리 등
using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    private void Start()
    {
        InitMap().Forget();
    }

    // 게임 진입점에서 실행
    private async UniTask InitMap()
    {
        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync("Map", "Prefabs/Map", Vector3.zero);

        bool hasSaveData = false;

        if (hasSaveData)
        {
            ServiceManager.Instance.HousingService.LoadAllHousingData();
        }
        else
        {
            if (prefab.TryGetComponent(out BuildView buildView))
            {
                BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();
                buildView.BindViewModel(buildVM);

                buildVM.InitDefaultRoom(buildView.Transform_DefaultRoom, buildView.Transform_DefaultAisle);
            }
        }

        BuildViewModel build = ServiceManager.Instance.BuildService.GetBuildViewModel();
        HousingViewModel housing = ServiceManager.Instance.HousingService.GetHousingViewModel();

        Camera.main.GetComponent<CameraController>().BindViewModel(housing, build);
    }
}
