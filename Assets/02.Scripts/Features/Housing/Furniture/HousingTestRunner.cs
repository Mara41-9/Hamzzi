using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class HousingTestRunner : MonoBehaviour
{
    [SerializeField] private HousingView _housingView;
    [SerializeField] private HousingUI _housingUI;

    [Header("기본 방 생성 설정 (BuildView와 동일 좌표)")]
    [SerializeField] private List<Vector2Int> _defaultRoomPos = new List<Vector2Int> { new Vector2Int(0, 0) };
    [SerializeField] private List<Vector2Int> _defaultAislePos = new List<Vector2Int> { new Vector2Int(6, 0) };

    private HousingViewModel _housingVM;
    private BuildViewModel _buildVM;

    private void Start()
    {
        // 1. BuildViewModel 생성 및 이미 만들어두신 기본 방/복도 생성 메서드 호출!
        _buildVM = new BuildViewModel();
        _buildVM.InitDefaultRoom(_defaultRoomPos, _defaultAislePos);

        // 2. HousingViewModel 생성
        _housingVM = new HousingViewModel();

        // 3. 동일한 _buildVM 인스턴스를 HousingView에 전달
        if (_housingView != null)
        {
            _housingView.BindViewModel(_housingVM, _buildVM);
        }

        if (_housingUI != null)
        {
            _housingUI.BindViewModel(_housingVM);
        }

        _housingVM.EnterHousingMode();
        Debug.Log("<color=yellow>[테스트]</color> 하우징 모드 시작! 화면에 생성된 기본 방을 터치해보세요.");
    }
}