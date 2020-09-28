using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class HousingBarGraph : MonoBehaviour
{

    private Transform bar1, bar2, bar3, bar4, bar5, bar6;
    private Transform marker1, marker2, marker3, marker4, marker5, marker6;


    private void Awake()
    {
       
        bar1 = transform.Find("Bar1");
        bar2 = transform.Find("Bar2");
        bar3 = transform.Find("Bar3");
        bar4 = transform.Find("Bar4");
        bar5 = transform.Find("Bar5");
        bar6 = transform.Find("Bar6");

        marker1 = transform.Find("Marker1");
        marker2 = transform.Find("Marker2");
        marker3 = transform.Find("Marker3");
        marker4 = transform.Find("Marker4");
        marker5 = transform.Find("Marker5");
        marker6 = transform.Find("Marker6");
    }

    public void SetSize(float sizeNormalized, int numberBar = 0) 
    {
        float rateOfMove = 1.0f; //How fast do you want the bar graph to transition between states?
        int sizeOfBar = 600;
        if (numberBar == 0)
        {
            Debug.Log("Error: Must input a number 1-6 to pick a bar graph.");
            return;
        }
        if (numberBar == 1)
        {
            bar1.DOScaleY(sizeNormalized, rateOfMove);
            marker1.transform.DOMoveY(bar1.position.y + (bar1.localScale.y * sizeOfBar), rateOfMove);
        }
        if (numberBar == 2)
        {
            bar2.DOScaleY(sizeNormalized, rateOfMove);
            marker2.transform.DOMoveY(bar2.position.y + (bar2.localScale.y * sizeOfBar), rateOfMove);
            //bar2.localScale = new Vector3(1f, sizeNormalized);
        }
        if (numberBar == 3)
        {
            bar3.DOScaleY(sizeNormalized, rateOfMove);
            marker3.transform.DOMoveY(bar3.position.y + (bar3.localScale.y * sizeOfBar), rateOfMove);
            //bar3.localScale = new Vector3(1f, sizeNormalized);
        }
        if (numberBar == 4)
        {
            bar4.DOScaleY(sizeNormalized, rateOfMove);
            marker4.transform.DOMoveY(bar4.position.y + (bar4.localScale.y * sizeOfBar), rateOfMove);
            //bar4.localScale = new Vector3(1f, sizeNormalized);
        }
        if (numberBar == 5)
        {
            bar5.DOScaleY(sizeNormalized, rateOfMove);
            marker5.transform.DOMoveY(bar5.position.y + (bar5.localScale.y * sizeOfBar), rateOfMove);
            //bar5.localScale = new Vector3(1f, sizeNormalized);
        }
        if (numberBar == 6)
        {
            bar6.DOScaleY(sizeNormalized, rateOfMove);
            marker6.transform.DOMoveY(bar6.position.y + (bar6.localScale.y * sizeOfBar), rateOfMove);
            //bar6.localScale = new Vector3(1f, sizeNormalized);
        }
    }
}
