using UnityEngine;

public class HamsterModelService
{
    private HamsterModelViewModel _modelViewrViewModel;

    public HamsterModelViewModel GetHamsterModelViewModel()
    {
        if (_modelViewrViewModel == null)
        {
            var modelViewrViewModel = new HamsterModelViewModel();
            SetHamsterModelViewModel();
            _modelViewrViewModel = modelViewrViewModel;
        }

        return _modelViewrViewModel;
    }

    private void SetHamsterModelViewModel()
    {
        GameObjectManager.Instance.CreateObject(string.Empty, "Hamster3DModel", Vector3.one * 1000);
    }
}