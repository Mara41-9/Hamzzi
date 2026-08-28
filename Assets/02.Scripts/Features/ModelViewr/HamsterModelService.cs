using Cysharp.Threading.Tasks;
using UnityEngine;

public class HamsterModelService
{
    private HamsterModelViewModel _modelViewrViewModel;
    private Hamster3DModelView _modelView;

    public HamsterModelViewModel GetHamsterModelViewModel()
    {
        if (_modelViewrViewModel == null)
        {
            var modelViewrViewModel = new HamsterModelViewModel();
            _modelViewrViewModel = modelViewrViewModel;
        }

        return _modelViewrViewModel;
    }

    public async UniTask LoadHamsterModel()
    {
        var modelObject = await GameObjectManager.Instance.CreateObjectAsync(string.Empty, "Hamster3DModel", Vector3.one * 1000);
        var view = modelObject.GetComponent<Hamster3DModelView>();
        _modelView = view;
    }

    public Transform GetModelTransform()
    {
        return _modelView.GetHamsterTransform();
    }

    public void SetHamsterAnimator(string parameter)
    {
        _modelView.SetHamsterAnimator(parameter);
    }
}