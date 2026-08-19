using DG.Tweening;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector2 _showPosition;
    private Vector2 _hidePosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _showPosition = _rectTransform.anchoredPosition;

        _hidePosition = new Vector2(_showPosition.x, _showPosition.y - 1000f);
    }

    private void OnEnable()
    {
        _rectTransform.DOKill();
        _rectTransform.anchoredPosition = _hidePosition;

        _rectTransform.DOAnchorPos(_showPosition, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        _rectTransform.DOKill();
    }
}
