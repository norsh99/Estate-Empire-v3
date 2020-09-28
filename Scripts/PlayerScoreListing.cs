using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreListing: IComparable<PlayerScoreListing>
{

    public PlayerScoreListing(int id, int vp, int money, string name, bool turnComplete)
    {
        this.id = id;
        this.vp = vp;
        this.money = money;
        this.name = name;
        this.completedTurn = turnComplete;
    }

    private bool completedTurn;
    private int id;
    private int vp;
    private int money;
    private string name;
    private GameObject thePanel;

    public int CompareTo(PlayerScoreListing score)
    {
        return score.vp.CompareTo(this.vp);
    }


    public bool GetCompletedTurn()
    {
        return completedTurn;
    }

    public int GetID()
    {
        return id;
    }
    public int GetVP()
    {
        return vp;
    }
    public int GetMoney()
    {
        return money;
    }
    public string GetName()
    {
        return name;
    }
    public GameObject GetThePanel()
    {
        return thePanel;
    }

    public void SetCompletedTurn(bool turn)
    {
        completedTurn = turn;
    }

    public void SetID(int val)
    {
        id = val;
    }
    public void SetVP(int val)
    {
        vp = val;
    }
    public void SetMoney(int val)
    {
        money = val;
    }
    public void SetName(string nameString)
    {
        name = nameString;
    }
    public void SetThePanel(GameObject panel)
    {
        thePanel = panel;
    }

    public void UpdateUI(PlayerScoreListing thePlayer)
    {
        thePanel.GetComponent<ScorePanel>().UpdateUI(thePlayer.GetName(), thePlayer.GetVP(), thePlayer.GetMoney(), thePlayer.GetID(), thePlayer.GetCompletedTurn());
    }

}
