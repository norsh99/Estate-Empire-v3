using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuBarAnimation : MonoBehaviour
{

    public Transform bar1, bar2, bar3, bar4, bar5, bar6, bar7, bar8, bar9, bar10;
    public Transform logo;
    public Transform canvasLoadIn;




    void Start()
    {
        canvasLoadIn.transform.localScale = new Vector3(0, 0, 0);
        LaunchBarIntroAnimation();
    }



    private void LaunchBarIntroAnimation()
    {
        float rateOfMove = 1.5f;

        bar1.DOScaleY(1.0f, rateOfMove+0.1f).SetEase(Ease.InOutBack);
        bar2.DOScaleY(0.79f, rateOfMove - 0.1f).SetEase(Ease.InOutBack);
        bar3.DOScaleY(0.48f, rateOfMove + 0.12f).SetEase(Ease.InOutBack);
        bar4.DOScaleY(0.65f, rateOfMove - 0.12f).SetEase(Ease.InOutBack);
        bar5.DOScaleY(0.35f, rateOfMove + 0.11f).SetEase(Ease.InOutBack);
        bar6.DOScaleY(1.0f, rateOfMove - 0.15f).SetEase(Ease.InOutBack);
        bar7.DOScaleY(0.79f, rateOfMove + 0.14f).SetEase(Ease.InOutBack);
        bar8.DOScaleY(0.48f, rateOfMove - 0.11f).SetEase(Ease.InOutBack);
        bar9.DOScaleY(0.65f, rateOfMove + 0.12f).SetEase(Ease.InOutBack);
        bar10.DOScaleY(0.35f, rateOfMove).SetEase(Ease.InOutBack);
        logo.DOMoveY(196.0f, rateOfMove).OnComplete(LoadInButtons);
    }
    private void LoadInButtons()
    {
        canvasLoadIn.DOScale(0.4518764f, 0.5f);
    }
}
