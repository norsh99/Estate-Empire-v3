using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using DG.Tweening;

public class PlayerPhotonScript : MonoBehaviour
{

    private PhotonView PV;

    private GameMaster gameMasterRef;

    public int id;
    public int money;
    private int bids;
    private int totalBids;
    private int purplePoints;
    private int victoryPoints;
    private int loans;

    private int extraVictoryPoints;

    private int offers;
    private int totalOffers;
    private List<HouseData> yourHousesBought;



    //For Animation ---------------------------------------------
    private float previousMoney; //For animation purpose
    private float newMoney;      //For animation purpose
    private float currentMoney;  //For animation purpose
    private float animationTime;


    void Start()
    {
        PV = GetComponent<PhotonView>();

        gameMasterRef = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        yourHousesBought = new List<HouseData>();

        money = 7000;

        totalBids = 2;
        bids = totalBids;
        totalOffers = 2;
        offers = totalOffers;

        purplePoints = 0;
        victoryPoints = 0;
        loans = 0;
        extraVictoryPoints = 0;

        UpdateDashboardUI();
        gameMasterRef.offersTextBox.text = ("Offers Left: " + offers);
        gameMasterRef.bidTextBox.text = "Bids: " + bids;
        //UpdateMarketData();
    }

    public int GetExtraVictoryPoints()
    {
        return extraVictoryPoints;
    }

    public void SetExtraVictoryPoints(int val)
    {
        extraVictoryPoints = val;
        UpdateDashboardUI();
    }

    public int GetMoney()
    {
        return money;
    }

    public void AddVictoryPoints(int val)
    {
        victoryPoints += val;
    }

    public void PlusOnePurplePoint()
    {
        purplePoints += 1;
    }

    public int GetTotalVictorypoints()
    {
        return victoryPoints + extraVictoryPoints;
    }

    public void RemoveOnePurplePoint()
    {
        purplePoints -= 1;
    }

    public int GetPurplePoints()
    {
        return purplePoints;
    }

    public void UpdateDashboardUI()
    {
        gameMasterRef.purplePointsTextBox.text = purplePoints.ToString();
        gameMasterRef.victoryPointsTextBox.text = (victoryPoints + extraVictoryPoints).ToString(); //The extra calculation of victory points is key here
        gameMasterRef.loansTextBox.text = loans.ToString();
    }

    private void Update()
    {
        NumAnimation();
    }

    public int GetOffers()
    {
        return offers;
    }

    public void UpgradeTotalBids(int val)
    {
        totalBids += val;
    }

    public void UpgradeTotalOffers(int val)
    {
        totalOffers += val;
    }

    public int GetLoans()
    {
        return loans;
    }

    public void IncreaseLoans(int val)
    {
        loans += val;
    }

    public void ResetOffers()
    {
        offers = totalOffers;
        gameMasterRef.offersTextBox.text = ("Offers Left: " + offers);
    }

    public void RemoveOneOffer()
    {
        offers -= 1;
        gameMasterRef.offersTextBox.text = ("Offers Left: " + offers);
    }

    public void AddOneOffer()
    {
        offers += 1;
        gameMasterRef.offersTextBox.text = ("Offers Left: " + offers);
    }

    public int GetTotalOffer()
    {
        return totalOffers;
    }

    public int GetBids()
    {
        return bids;
    }
    public int GetTotalBids()
    {
        return totalBids;
    }

    public void ResetBids()
    {
        bids = totalBids;
        gameMasterRef.bidTextBox.text = "Bids: "+ bids;
    }

    public void RemoveOneBid()
    {
        bids -= 1;
        gameMasterRef.bidTextBox.text = "Bids: " + bids;
    }
    public void AddOneBid()
    {
        bids += 1;
        gameMasterRef.bidTextBox.text = "Bids: " + bids;
    }

    public void AddMoney(int value)
    {
        animationTime = 0.5f; //Set the intial speed every time a change to money is made
        previousMoney = money;
        currentMoney = money;
        newMoney = (float)(money += value);
        money = (int)newMoney;
    }

    public void NumAnimation()
    {
        if (currentMoney != newMoney)
        {
            if (previousMoney < newMoney)
            {
                currentMoney += (animationTime * Time.deltaTime) * (newMoney - previousMoney);
                if (currentMoney >= newMoney)
                {
                    currentMoney = newMoney;
                }
            }
            else
            {
                currentMoney -= (animationTime * Time.deltaTime) * (previousMoney - newMoney);
                if (currentMoney <= newMoney)
                {
                    currentMoney = newMoney;
                }
            }
            gameMasterRef.moneyTextBox.text = "$"+ currentMoney.ToString("0");
        }
    }

    public void AddHouseToYourList(HouseData theHouse)
    {
        GameObject newAssetPanel = Instantiate(gameMasterRef.buyPanelPrefab, transform.position = new Vector3(0,0,5), Quaternion.identity, gameMasterRef.contentForYourAssets.transform);
        theHouse.SetGameObjectOfSelf(newAssetPanel);
        yourHousesBought.Add(theHouse);
        newAssetPanel.GetComponent<BuyPanel>().UpdateUI(theHouse, true);
    }

    public void RemoveHouseFromList(HouseData theHouse)
    {
        yourHousesBought.Remove(theHouse);
        Destroy(theHouse.GetGameObjectOfSelf());
        SetExtraVictoryPoints(gameMasterRef.CalculateExtraVictoryPoints());
    }

    public List<HouseData> GetAssetList()
    {
        return yourHousesBought;
    }

    public void UpdateAllAssetUI()
    {
        for (int i = 0; i < yourHousesBought.Count; i++)
        {
            yourHousesBought[i].GetGameObjectOfSelf().GetComponent<BuyPanel>().UpdateHouseValueText();
        }
    }



    //Send, You would call this from GameMaster ---------------------------------------------------


    public void UpdateMarketData(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateHousingValues", RpcTarget.AllBuffered, val1, val2, val3, val4, val5, val6);
        }
    }

    public void StartNewYear(int year)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_NewRound", RpcTarget.OthersBuffered, year);
        }
    }

    public void UpdateBuyerCardSelection(int selection)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateHouseBoughtSelection", RpcTarget.AllBuffered, selection);
        }
    }

    public void UpdateBfValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateBfValues", RpcTarget.MasterClient, val1, val2, val3, val4, val5, val6);
        }
    }


    public void UpdateBucketValues(int bucket1, int bucket2, int bucket3, int bucket4, int bucket5, int bucket6)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateBucketValuesInGameMaster", RpcTarget.AllBuffered, bucket1, bucket2, bucket3, bucket4, bucket5, bucket6);
        }
    }

    public void UpdateListOfBids(int id, int val, int lastBid)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateListOfBids", RpcTarget.AllBuffered, id, val, lastBid);
        }
    }

    public void StartGame()
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_StartGame", RpcTarget.AllBuffered);
        }
    }

    public void UpdateIAmReady(int num)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdateIAmReady", RpcTarget.MasterClient, num);
        }
    }

    public void FinishedIdCheck(int id)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_IdFinishedCheck", RpcTarget.AllBuffered, id);
        }
    }

    public void UpdatePlayerInfo(PlayerScoreListing theInfo)
    {
        if (PV.IsMine)
        {
            PV.RPC("RPC_UpdatePlayerInfo", RpcTarget.OthersBuffered, theInfo.GetID(), theInfo.GetVP(), theInfo.GetName(), theInfo.GetMoney(), theInfo.GetCompletedTurn());
        }
    }



    //Receive ------------------------------------------------------------------------------------
    [PunRPC]
    void RPC_UpdateHousingValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        gameMasterRef.Photon_UpdateHousingValues(val1, val2, val3, val4, val5, val6);
    }

    [PunRPC]
    void RPC_NewRound(int year)
    {
        gameMasterRef.Photon_UpdateYear(year);
        gameMasterRef.GoToBoughtScreen();
    }

    [PunRPC]
    void RPC_UpdateHouseBoughtSelection(int selection)
    {
        gameMasterRef.BuyButtonOnBuyScreen(selection);
    }

    [PunRPC]
    void RPC_UpdateBfValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        gameMasterRef.Photon_UpdateBfValues(val1, val2, val3, val4, val5, val6);
    }

    [PunRPC]
    void RPC_UpdateBucketValuesInGameMaster(int bucket1, int bucket2, int bucket3, int bucket4, int bucket5, int bucket6)
    {
        gameMasterRef.Photon_UpdateBucketValues(bucket1, bucket2, bucket3, bucket4, bucket5, bucket6);
    }


    [PunRPC]
    void RPC_UpdateListOfBids(int id, int value, int lastBid)
    {
        gameMasterRef.Photon_UpdateListofBids(id, value, lastBid);
    }

    [PunRPC]
    void RPC_StartGame()
    {
        gameMasterRef.JoinedRoomSuccessfully();
    }

    [PunRPC]
    void RPC_UpdateIAmReady(int num)
    {
        gameMasterRef.Photon_UpdateWhoIsReady(num);
        gameMasterRef.CheckIfAllPlayersAreReady();
    }

    [PunRPC]
    void RPC_IdFinishedCheck(int finishedId)
    {
        gameMasterRef.Photon_RecieveIdMatch_AllPlayersFinished(finishedId);
    }

    [PunRPC]
    void RPC_UpdatePlayerInfo(int id, int vp, string name, int money, bool turn)
    {
        gameMasterRef.Update_PlayerInfo(id, vp, name, money, turn);
    }
}
