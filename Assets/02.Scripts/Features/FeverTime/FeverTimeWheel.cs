// 쳇바퀴에 부착되어 일정 시간마다 피버타임 트리거를 발생시키는 컴포넌트
using UnityEngine;
using UnityEngine.EventSystems;

public class FeverTimeWheel : MonoBehaviour
{
    private const float TEMP_TRIGGER_INTERVAL_SEC = 20f; // TODO: HAM-68 데이터테이블 완성되면 등급별 값으로 교체

    private float _elapsedTime; //쳇바퀴가 마지막으로 리셋된 뒤부터 지금까지 흐른 시간을 초단위로 저장
    private bool _isReady;

    private void Update()
    {
        if (_isReady == false)
        {
            UpdateTimer();
            return;
        }

        CheckTouchInput();
    }

    private void UpdateTimer()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= TEMP_TRIGGER_INTERVAL_SEC)
        {
            SetReadyState();
        }
    }

    private void SetReadyState()
    {
        _isReady = true;

#if UNITY_EDITOR
        Debug.Log("쳇바퀴 반짝임 트리거 (TODO: 실제 반짝임 이펙트 연결되면 이 로그를 교체)");
#endif
    }

    private void CheckTouchInput()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        if (touch.phase != TouchPhase.Began)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(touch.position);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            OnWheelTouched();
        }
    }

    private void OnWheelTouched()
    {
        _isReady = false;
        _elapsedTime = 0f;

        FeverTimeManager.Instance.SetState(FeverTimeState.CutscenePlaying);
    }
}