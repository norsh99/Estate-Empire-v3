using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextValueBoxAnimation : MonoBehaviour
{


    void Start()
    {
        MoveOnY();
        //StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(this.gameObject);
    }
    private void MoveOnY()
    {
        this.transform.DOMoveY(this.transform.position.y + 300, 2.0f).OnComplete(DespawnObject);
    }
    private void DespawnObject()
    {
        Destroy(this.gameObject);
    }
}
