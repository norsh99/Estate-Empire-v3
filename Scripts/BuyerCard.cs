using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyerCard : GameMaster
{
    public BuyerCard(int money, int rooms, bool plusOneCard)
    {

        this.money = money;
        this.rooms = rooms;
        this.plusOneCard = plusOneCard;

    }

    private int money;
    private int rooms;
    private bool plusOneCard;

    public bool GetPlusOneCard()
    {
        return plusOneCard;
    }
    public int GetMoney()
    {
        return money;
    }
    public int GetRooms()
    {
        return rooms;
    }

    public void AdjustMoney(int howMuch)
    {
        money += howMuch;
    }




}
