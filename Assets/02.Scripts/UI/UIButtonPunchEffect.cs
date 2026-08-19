// 버튼 클릭 시 순간적으로 커졌다가 줄어드는 펀치 스케일 이펙트 (공용 컴포넌트, 아무 버튼에나 부착)
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonPunchEffect : MonoBehaviour
{
    private const float PunchScaleMultiplier = 1.15f;
    private const float ScaleUpDurationSeconds = 0.08f;
    private const float ScaleDownDurationSeconds = 0.12f;

    private Button _button;
    private RectTransform _rectTransform;
    private Vector3 _originalScale;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClickButton);
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnClickButton);
        }

        if (_rectTransform != null)
        {
            _rectTransform.DOKill();
            _rectTransform.localScale = _originalScale;
        }
    }

    private void OnClickButton()
    {
        PlayPunchEffect();
    }

    private void PlayPunchEffect()
    {
        _rectTransform.DOKill();
        _rectTransform.localScale = _originalScale;

        Sequence punchSequence = DOTween.Sequence();
        punchSequence.Append(_rectTransform.DOScale(_originalScale * PunchScaleMultiplier, ScaleUpDurationSeconds).SetEase(Ease.Linear));
        punchSequence.Append(_rectTransform.DOScale(_originalScale, ScaleDownDurationSeconds).SetEase(Ease.Linear));
    }
}