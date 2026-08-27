using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CrossResultView : MonoBehaviour
{
    [SerializeField] private RectTransform rawImageRect;
    [SerializeField] private CanvasGroup rawImageCanvasGroup;

    public Transform characterModel;
    public Animator characterAnimator;

    private HamsterModelViewModel _hamsterModelViewModel;

    private void OnEnable()
    {
        Debug.Log("dd");
        _hamsterModelViewModel = ServiceManager.Instance.HamsterModelService.GetHamsterModelViewModel();
    }

    public void PlayGachaResult(string hamsterId, string faceId)
    {
        _hamsterModelViewModel.HamsterId = hamsterId;
        _hamsterModelViewModel.FaceId = faceId;

        rawImageRect.localScale = Vector3.zero;
        rawImageCanvasGroup.alpha = 1;

        //characterModel.localEulerAngles = new Vector3(0, 180f, 0);

        Sequence crossSequence = DOTween.Sequence();

        crossSequence.Append(rawImageRect.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack));

        //crossSequence.Join(characterModel.DOLocalRotate(Vector3.zero, 0.7f, RotateMode.FastBeyond360)).SetEase(Ease.OutQuad);
        //crossSequence.AppendCallback(() =>
        //{
        //    if (characterAnimator != null)
        //    {
        //        characterAnimator.SetTrigger("DanceTrigger");
        //    }
        //});

    }
}