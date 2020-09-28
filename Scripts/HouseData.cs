using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseData
{
    public HouseData(int id, bool extraPurple, int vp, int income, int famNum, string location, string title)
    {
        this.id = id;
        this.value = 0;
        this.famNum = famNum;
        this.title = title;
        this.bidValue = 0;
        this.lastPersonToBid = 0;
        this.victoryPoints = vp;
        this.income = income;
        this.extraPurple = extraPurple;
        this.location = location;


    }
    private int id;
    private int value;
    private int famNum;
    private string title;
    private int bidValue;
    private int owner;
    private int lastPersonToBid;
    private int victoryPoints;
    private int income;
    private bool extraPurple;
    private string location;



    private GameObject asset_selfGameObjectInList;
    private GameObject buy_selfGameObject;

    public string GetLocation()
    {
        return location;
    }

    public bool GetExtraPurplePoints()
    {
        return extraPurple;
    }

    public int GetVictoryPoints()
    {
        return victoryPoints;
    }

    public void SetVictoryPoints(int val)
    {
        victoryPoints = val;
    }


    public int GetID()
    {
        return id;
    }
    public int GetValue()
    {
        return value;
    }
    public int GetFamNum()
    {
        return famNum;
    }
    public string GetTitle()
    {
        return title;
    }
    public int GetBidValue()
    {
        return bidValue;
    }
    public int GetOwner()
    {
        return owner;
    }
    public int GetLastPersonToBid()
    {
        return lastPersonToBid;
    }

    public GameObject GetGameObjectOfSelf()
    {
        return asset_selfGameObjectInList;
    }

    public GameObject GetGameObjectOfSelf_BuyScreen()
    {
        return buy_selfGameObject;
    }

    public int GetIncome()
    {
        return income;
    }

    public void SetValue(int val)
    {
        value = val;
    }
    public void SetBidValue(int bidVal)
    {
        bidValue = bidVal;
    }
    public void SetOwner(int theOwner)
    {
        owner = theOwner;
    }
    public void SetLastPersonToBid(int person)
    {
        lastPersonToBid = person;
    }

    public void SetGameObjectOfSelf(GameObject theObject)
    {
        asset_selfGameObjectInList = theObject;
    }

    public void SetGameObjectOfSelf_BuyScreen(GameObject theObject)
    {
        buy_selfGameObject = theObject;
    }
}
