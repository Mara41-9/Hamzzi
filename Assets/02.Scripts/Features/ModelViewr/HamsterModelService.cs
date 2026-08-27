using Cysharp.Threading.Tasks;
using UnityEngine;

public class HamsterModelService
{
    private HamsterModelViewModel _modelViewrViewModel;
    private Transform _modelTransform;

    public HamsterModelService()
    {
        LoadHamsterModel().Forget();
    }

    public HamsterModelViewModel GetHamsterModelViewModel()
    {
        if (_modelViewrViewModel == null)
        {
            var modelViewrViewModel = new HamsterModelViewModel();
            _modelViewrViewModel = modelViewrViewModel;
        }

        return _modelViewrViewModel;
    }

    private async UniTask LoadHamsterModel()
    {
        var modelObject = await GameObjectManager.Instance.CreateObjectAsync(string.Empty, "Hamster3DModel", Vector3.one * 1000);
        var view = modelObject.GetComponent<Hamster3DModelView>();
        _modelTransform = view.GetHamsterTransform();
    }

    public Transform GetModelTransform()
    {
        return _modelTransform;
    }
}