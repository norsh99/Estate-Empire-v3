using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{

    public Transform doorL;
    public Transform doorR;

    private void Start()
    {
        doorR.position = new Vector3(153, 0, 0);
        doorL.position = new Vector3(-153, 0, 0);
        MoveDoorsOut();
    }

    public void PlayGame()
    {
        MoveDoors();
    }


    private void MoveDoors()
    {
        doorR.transform.DOMove(new Vector3(153, 0, 0), 0.5f).SetEase(Ease.InOutBounce);
        doorL.transform.DOMove(new Vector3(-153, 0, 0), 0.5f).SetEase(Ease.InOutBounce).OnComplete(GoToMainGame);
    }

    private void GoToMainGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    private void MoveDoorsOut()
    {
        doorR.transform.DOMove(new Vector3(437, 0, 0), 0.6f).SetEase(Ease.InCirc);
        doorL.transform.DOMove(new Vector3(-437, 0, 0), 0.6f).SetEase(Ease.InCirc);
    }

}
