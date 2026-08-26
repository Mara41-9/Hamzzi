//게임 전체 흐름 제어 매니저 - 게임 종료, 실행, 로그인 넘어가는 과정 처리 등
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : SingletonBase<GameManager>
{
    // 게임 진입점에서 실행
    public async UniTask InitMap(long userUID)
    {
        if (userUID == 0)
        {
            return;
        }

        GameObject prefab = await GameObjectManager.Instance.CreateObjectAsync("Map", "Prefabs/Map", Vector3.zero);
        BuildView buildView = prefab.GetComponent<BuildView>();
        BuildViewModel buildVM = ServiceManager.Instance.BuildService.GetBuildViewModel();

        buildView.BindViewModel(buildVM);

        bool hasSaveData = await ServiceManager.Instance.NetworkBuildService.HasUserRoomData(userUID);

        if (hasSaveData)
        {
            await ServiceManager.Instance.NetworkBuildService.LoadBuildAndFurnitureData(userUID);
            buildView.SpawnAllLoadBuilds();

            foreach (var pair in buildVM.Builds)
            {
                if (pair.Value.BuildType == BuildType.Aisle)
                {
                    buildVM.UpdateConnection(pair.Key);
                }
                else
                {
                    buildVM.UpdateRoomConnection(pair.Value);
                }
            }

            RoomViewModel topAisle = null;
            int maxY = int.MinValue;

            foreach (var pair in buildVM.Builds)
            {
                if (pair.Value.BuildType == BuildType.Aisle)
                {
                    if (pair.Value.OriginPos.y > maxY)
                    {
                        maxY = pair.Value.OriginPos.y;
                        topAisle = pair.Value;
                    }
                }
            }

            if (topAisle != null)
            {
                topAisle.SetWallActive(0, true);
                topAisle.Refresh();
            }

            ServiceManager.Instance.BuildService.RefreshAisleNavMesh(buildVM.Builds);
        }
        else
        {
            if (buildView != null)
            {
                buildVM.InitDefaultRoom(buildView.Transform_DefaultRoom, buildView.Transform_DefaultAisle);
            }
        }

        HousingViewModel housing = ServiceManager.Instance.HousingService.GetHousingViewModel();

        Camera.main.GetComponent<CameraController>().BindViewModel(housing, buildVM);

        await UniTask.DelayFrame(2);

        NavigationManager.Instance.BuildNav();
    }
}
