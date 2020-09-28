using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HouseViewer : MonoBehaviour
{
    public TextMeshPro title;
    public TextMeshPro money;
    public TextMeshPro rooms;
    public TextMeshPro vp;
    public TextMeshPro location;
    public GameObject theHouseIcon;
    public GameObject plusPurpleIcon;


    public Sprite h1;
    public Sprite h2;
    public Sprite h3;
    public Sprite h4;
    public Sprite h5;
    public Sprite h6;


    public void UpdateCardUI(int mon, string name, int rm, int victoryPoints, bool plusPurple, string city)
    {
        theHouseIcon.GetComponent<SpriteRenderer>().sprite = AssignSprite(rm);
        money.text = "$" + mon + "/y";
        title.text = name;
        location.text = city;
        rooms.text = rm.ToString();
        vp.text = victoryPoints.ToString();

        if (plusPurple)
        {
            plusPurpleIcon.SetActive(true);
        }
        else
        {
            plusPurpleIcon.SetActive(false);
        }
    }

    public Sprite AssignSprite(int num)
    {
        if (num == 1)
        {
            return h1;
        }
        if (num == 2)
        {
            return h2;
        }
        if (num == 3)
        {
            return h3;
        }
        if (num == 4)
        {
            return h4;
        }
        if (num == 5)
        {
            return h5;
        }
        if (num == 6)
        {
            return h6;
        }
        return null;
    }

}
