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
        // 뷰모델 초기화
    }

    public void LoadHamsterId()
    {
        var allHamsterIds = GameDataManager.Instance.GetAllDataId<HamsterData>();
        _collectionViewModel.AllHamsterIdList = allHamsterIds.ToHashSet<string>();
    }
}
