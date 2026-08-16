using System.Linq;
using UnityEngine;

public class NetworkCollectionService
{
    private CollectionViewModel _collectionViewModel;

    public CollectionViewModel GetCollectionViewModel()
    {
        if(_collectionViewModel == null)
        {
            var collectionViewModel = new CollectionViewModel();
            SetCollectionViewModel(collectionViewModel);
            _collectionViewModel = collectionViewModel;
        }

        return _collectionViewModel;
    }

    private void SetCollectionViewModel(CollectionViewModel vm)
    {
        GameDataManager.Instance.LoadData<HamsterData>();
        LoadHamsterId(vm);
    }

    private void LoadHamsterId(CollectionViewModel vm)
    {
        var allHamsterIds = GameDataManager.Instance.GetAllDataId<HamsterData>();
        vm.AllHamsterIdList = allHamsterIds.ToHashSet<string>();
    }
}
