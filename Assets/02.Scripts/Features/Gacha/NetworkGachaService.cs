using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkGachaService
{
    private static int InstanceID = 0;

    private GachaViewModel _gachaViewModel;

    public GachaViewModel GetGachaViewModel()
    {
        if(_gachaViewModel == null)
        {
            var gachaViewModel = new GachaViewModel();
            SetGachaViewModel(gachaViewModel);
            _gachaViewModel = gachaViewModel;
        }

        return _gachaViewModel;
    }

    private void SetGachaViewModel(GachaViewModel vm)
    {
        // 뷰모데 초기화
        CollectionViewModel collectionVM = ServiceManager.Instance.CollectionService.GetCollectionViewModel();
        List<string> allHamsterList = collectionVM.AllHamsterIdList;

        foreach(string hamsterId in allHamsterList)
        {
            HamsterData hamsterData = GameDataManager.Instance.GetData<HamsterData>(hamsterId);
            switch (hamsterData.HamsterTier)
            {
                case HamsterTier.SS:
                    AddHamsterIdByTier(vm, HamsterTier.SS, hamsterId);
                    break;
                case HamsterTier.S:
                    AddHamsterIdByTier(vm, HamsterTier.S, hamsterId);
                    break;
                case HamsterTier.A:
                    AddHamsterIdByTier(vm, HamsterTier.A, hamsterId);
                    break;
            }
        }
    }

    private void AddHamsterIdByTier(GachaViewModel vm, HamsterTier tier, string hamsterId)
    {
        if (vm.HamsterIdByTierList.ContainsKey(tier) == false)
        {
            vm.HamsterIdByTierList.Add(tier, new List<string>());
        }
        vm.HamsterIdByTierList[tier].Add(hamsterId);
    }

    public HamsterSave DrawGacha()
    {
        HamsterTier drawTier = DrawTier();

        List<string> hamsterList = _gachaViewModel.HamsterIdByTierList[drawTier];
        if (hamsterList == null)
            return null;

        // 햄스터 몸 랜덤 뽑기
        int hamsterCount = hamsterList.Count;
        int randomIndex = Random.Range(0, hamsterCount);
        string drawHamsterId = hamsterList[randomIndex];

        // 햄스터 얼굴 랜덤 뽑기
        var faceList = ServiceManager.Instance.CollectionService.GetCollectionViewModel().AllFaceIdList;
        int faceCount = faceList.Count;
        randomIndex = Random.Range(0, faceCount);
        string drawFaceId = faceList[randomIndex];

        HamsterSave hamsterSave = new HamsterSave();
        hamsterSave.HamsterUID = InstanceID++;
        hamsterSave.HamsterId = drawHamsterId;
        hamsterSave.FaceId = drawFaceId;

        return hamsterSave;
    }

    private HamsterTier DrawTier()
    {
        int randomValue = Random.Range(0, 100);

        if (randomValue < GachaViewModel.SSProbability)
        {
            return HamsterTier.SS;
        }
        else if(randomValue < GachaViewModel.SSProbability + GachaViewModel.SProbability)
        {
            return HamsterTier.S;
        }
        else
        {
            return HamsterTier.A;
        }
    }
}