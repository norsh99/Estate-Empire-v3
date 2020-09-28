using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseViewerManager : MonoBehaviour
{
    List<HouseData> viewThisListOfHouses;

    private GameMaster gameMasterRef;
    public int index;

    void Start()
    {
        index = 0;
        gameMasterRef = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    public void EstablishTheListToView(List<HouseData> theList, int houseID)
    {
        viewThisListOfHouses = theList;
        for (int i = 0; i < viewThisListOfHouses.Count; i++)
        {
            if (houseID == viewThisListOfHouses[i].GetID())
            {
                index = i;
            }
        }
    }


    public void UpdateCardUI(int indexNum)
    {
        int money = viewThisListOfHouses[indexNum].GetIncome();
        string name = viewThisListOfHouses[indexNum].GetTitle();
        int room = viewThisListOfHouses[indexNum].GetFamNum();
        int victoryPoints = viewThisListOfHouses[indexNum].GetVictoryPoints();
        bool purplePoints = viewThisListOfHouses[indexNum].GetExtraPurplePoints();
        string cityName = viewThisListOfHouses[indexNum].GetLocation();

        gameMasterRef.theCard.GetComponent<HouseViewer>().UpdateCardUI(money, name, room, victoryPoints, purplePoints, cityName);
    }

    private void MoveIndex(int num)
    {
        if (num > 0 && index < viewThisListOfHouses.Count-1)
        {
            index += 1;
            return;
        }
        if (num > 0 && index == viewThisListOfHouses.Count-1)
        {
            index = 0;
            return;
        }
        if (num < 0 && index > 0)
        {
            index -= 1;
            return;
        }
        if (num < 0 && index == 0)
        {
            index = viewThisListOfHouses.Count-1;
            return;
        }
    }


    //Click these to bring in the new card
    public void Click_GoToNextCard()
    {
        MoveIndex(1);
        gameMasterRef.MoveScreenOut(gameMasterRef.theCard.transform, 0.1f);
        StartCoroutine(BringInNextCard());
    }
    public void Click_GoToPrevCard()
    {
        MoveIndex(-1);
        gameMasterRef.MoveScreenOut(gameMasterRef.theCard.transform, 0.1f, true);
        StartCoroutine(BringInNextCard(true));
    }


    IEnumerator BringInNextCard(bool reverse = false)
    {
        yield return new WaitForSeconds(0.2f);
        UpdateCardUI(index);
        if (reverse)
        {
            gameMasterRef.MoveScreenIn(gameMasterRef.theCard.transform, 0.1f, true);
        }
        else
        {
            gameMasterRef.MoveScreenIn(gameMasterRef.theCard.transform, 0.1f);
        }
    }

    public void Click_GoBackButton()
    {
        if (gameMasterRef.prevMenuSelected == MenuOptions.YourAssets)
        {
            gameMasterRef.MenuBarSelection(4);
        }
        else
        {
            gameMasterRef.MenuBarSelection(1);
        }
    }
}
