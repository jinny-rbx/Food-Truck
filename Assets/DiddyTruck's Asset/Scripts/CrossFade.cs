using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CrossFade : SceneTransition
{
    public CanvasGroup canvasGroup;

    public override IEnumerator AnimateTransitionIn()
    {
        if (canvasGroup == null) yield break;

        canvasGroup.alpha = 0f;
        // SetUpdate(true) forces DOTween to run even if the game is paused
        yield return canvasGroup.DOFade(1f, 0.5f).SetUpdate(true).WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        if (canvasGroup == null) yield break;

        yield return canvasGroup.DOFade(0f, 0.5f).SetUpdate(true).WaitForCompletion();
    }
}