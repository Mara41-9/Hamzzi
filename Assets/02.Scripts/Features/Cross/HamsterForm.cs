using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HamsterForm : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer FaceMesh;
    [SerializeField] private SkinnedMeshRenderer BodyMesh;

    public void SetFaceMesh(string faceId)
    {
        LoadFaceMesh(faceId).Forget();
    }

    private async UniTaskVoid LoadFaceMesh(string faceId)
    {
        var faceData = GameDataManager.Instance.GetData<FaceData>(faceId);
        string facePath = faceData.MaterialPath;

        var faceMarteiral = await ResourceManager.Instance.LoadAsset<Material>(facePath);
        FaceMesh.material = faceMarteiral;
    }

    public void SetBodyMesh(string bodyId)
    {
        LoadBodyMesh(bodyId).Forget();
    }

    private async UniTaskVoid LoadBodyMesh(string bodyId)
    {
        var bodyData = GameDataManager.Instance.GetData<HamsterData>(bodyId);
        string bodyPath = bodyData.MaterialPath;

        var faceMarteiral = await ResourceManager.Instance.LoadAsset<Material>(bodyPath);
        BodyMesh.material = faceMarteiral;
    }
}