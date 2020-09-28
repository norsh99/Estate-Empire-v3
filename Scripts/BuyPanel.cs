using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;


public class BuyPanel : MonoBehaviour
{

    public GameObject plusButton;
    public GameObject typeMenu;
    public GameObject displayMenu;
    public GameObject assetMenu;

    public GameObject badHighlight;
    public GameObject goodHighlight;

    public TMP_InputField input;
    public TextMeshProUGUI title;
    public TextMeshProUGUI value;
    public TextMeshProUGUI houseIncreaseValue;

    //AssetPanel
    public TextMeshProUGUI titleAsset;
    public TextMeshProUGUI valueAsset;
    public GameObject assetSellButton;


    public HouseData house;
    public int houseID;
    public int houseValue;
    public string houseName;
    public int bidOwner;

    public Sprite noBidIcon;
    public Sprite bidIcon; 
    public Sprite yourBidIcon; 
    public Sprite assetHouseIcon;
    public GameObject bidIconButton;

    private Color yourColor;


    private bool assetHouseType = false;

    private GameMaster gameMasterRef;

    public bool buyScreen; //Hold true if you are on the buyScreen, false if on the sell screen.

    private void Awake()
    {
        gameMasterRef = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        yourColor = gameMasterRef.GetColorOfPlayer(gameMasterRef.GetYourListPosition());
    }


    void Start()
    {

        houseID = 0;
        bidOwner = 0;
    }






    //Switching the state the panel is in. Plus screen, type scree, or view screen
    public void SwitchState(int num)
    {
        if (num == 1)
        {
            plusButton.SetActive(true);
            typeMenu.SetActive(false);
            displayMenu.SetActive(false);
            assetMenu.SetActive(false);
        }
        if (num == 2)
        {
            plusButton.SetActive(false);
            typeMenu.SetActive(true);
            displayMenu.SetActive(false);
            assetMenu.SetActive(false);
            CreateNewPanel();
        }
        if (num == 3)
        {
            plusButton.SetActive(false);
            typeMenu.SetActive(false);
            displayMenu.SetActive(true);
            assetMenu.SetActive(false);
        }
        if (num == 4)
        {
            plusButton.SetActive(false);
            typeMenu.SetActive(false);
            displayMenu.SetActive(false);
            assetMenu.SetActive(true);
        }
    }





    private void CreateNewPanel()
    {
        GameObject spawnPanel = gameMasterRef.buyPanelPrefab;
        GameObject spawnHere = gameMasterRef.contentForBuyPanel;

        GameObject newBuyPanel = Instantiate(spawnPanel, transform.position, Quaternion.identity, spawnHere.transform);
        gameMasterRef.contentForBuyPanel.GetComponent<BuyContent>().listOfPanels.Add(newBuyPanel);
    }

    public void CreateNewPanelWithThisHouse(HouseData updatePanelWithThisHouse)
    {
        SwitchState(3);
        UpdateUI(updatePanelWithThisHouse);
    }

    public void SubmitButton()
    {
        if (input.text != null)
        {
            int id = (int)Convert.ToUInt64(input.text);
            //Debug.Log(houseID);
            if (gameMasterRef.FindHouseID(id) && CheckNoDuplicates(id))
            {
                houseID = id;

                HouseData tempHouse = gameMasterRef.ReturnHouseStats(houseID);

                HouseData newHouse = SetupHouseValue(tempHouse);
                newHouse.SetGameObjectOfSelf_BuyScreen(this.gameObject);
                UpdateUI(newHouse);
                SwitchState(3);
                gameMasterRef.EstablishHouseListing_Send(newHouse);
                gameMasterRef.contentForBuyPanel.GetComponent<BuyContent>().listOfPanels.Add(this.gameObject);
            }
            else
            {
                badHighlight.SetActive(true);
                StartCoroutine(ResetHighlight());
            }
        }
    }

    public void CreatePanelFromPhoton(HouseData theHouse)
    {
        theHouse.SetGameObjectOfSelf_BuyScreen(this.gameObject);
        UpdateUI(theHouse);
        SwitchState(3);
    }





    private HouseData SetupHouseValue(HouseData theHouse)
    {
        if (gameMasterRef.currentHouseBids == null)
        {
            theHouse.SetValue(gameMasterRef.GetValueForHome(theHouse.GetFamNum()));
            return theHouse;
        }
        for (int i = 0; i < gameMasterRef.currentHouseBids.Count; i++)
        {
            if (gameMasterRef.currentHouseBids[i].GetID() == theHouse.GetID())
            {
                return gameMasterRef.currentHouseBids[i];
            }
        }
        theHouse.SetValue(gameMasterRef.GetValueForHome(theHouse.GetFamNum()));
        return theHouse;
    }


    public void UpdateHouseValueText()
    {
        int valOfHomeOnMarket = gameMasterRef.GetValueForHome(house.GetFamNum());

        if (valOfHomeOnMarket > houseValue)
        {
            int dif = valOfHomeOnMarket - houseValue;
            houseIncreaseValue.color = new Color(0, 1, 0.1185064f);
            houseIncreaseValue.text = "+$" + dif;
        }
        else
        {
            int dif = houseValue - valOfHomeOnMarket;
            houseIncreaseValue.color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            houseIncreaseValue.text = "-$" + dif;
        }
    }



    public void UpdateUI(HouseData houseToUpdate, bool isAsset = false)
    {
        //Debug.Log("UI CALLED");
        house = houseToUpdate;

        houseID = house.GetID();
        houseName = house.GetTitle();
        houseValue = house.GetValue();
        bidOwner = house.GetLastPersonToBid();
        value.text = "$"+houseValue.ToString();
        title.text = houseName;

        titleAsset.text = houseName;
        valueAsset.text = "$" + houseValue.ToString();
        if (isAsset)
        {
            //TODO fix this
            UpdateHouseValueText();

            assetHouseType = true;
            assetSellButton.GetComponent<SVGImage>().sprite = assetHouseIcon;
            SwitchState(4);
            return;
        }
        if (house.GetLastPersonToBid() == 0)
        {
            bidIconButton.GetComponent<SVGImage>().sprite = noBidIcon;
            return;
        }
        if (house.GetLastPersonToBid() == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            bidIconButton.GetComponent<SVGImage>().sprite = yourBidIcon;
            bidIconButton.GetComponent<SVGImage>().color = yourColor;
            return;
        }
        if (bidIconButton.GetComponent<SVGImage>().sprite == yourBidIcon)
        {
            gameMasterRef.haveBidOnHouse = false;
        }
        goodHighlight.SetActive(false);
        bidIconButton.GetComponent<SVGImage>().sprite = bidIcon;
        bidIconButton.GetComponent<SVGImage>().color = gameMasterRef.GetColorOfPlayer(gameMasterRef.GetOthersListPosition(house.GetLastPersonToBid()));
    }






    private bool CheckNoDuplicates(int id)
    {

        List<GameObject> theListOfPanels = gameMasterRef.contentForBuyPanel.GetComponent<BuyContent>().listOfPanels;
        if (theListOfPanels == null)
        {
            return true;
        }
        for (int i = 0; i < theListOfPanels.Count; i++)
        {
            if (theListOfPanels[i].GetComponent<BuyPanel>().houseID == id)
            {
                return false;
            }
        }


        return true;
    }

    public void AssetSellButton()
    {
        //Go to second sell screen
        gameMasterRef.UpdateMenuSelection(MenuOptions.Sell);
        gameMasterRef.MoveSellInitialScreen(house.GetFamNum());
        gameMasterRef.currentSale = house;
    }




    public void BidButton()
    {
        if (assetHouseType)
        {
            //Go to second sell screen
            gameMasterRef.UpdateMenuSelection(MenuOptions.Sell);
            gameMasterRef.MoveSellInitialScreen(house.GetFamNum());
            gameMasterRef.currentSale = house;
            return;
        }
        gameMasterRef.Photon_RecieveIdMatch_AllPlayersFinished(PhotonNetwork.LocalPlayer.ActorNumber);
        //If you have enought money
        if (gameMasterRef.playerPhoton.money >= houseValue+gameMasterRef.bidValue && gameMasterRef.haveBidOnHouse == false)
        {
            if (gameMasterRef.playerPhoton.GetBids() > 0 || bidIconButton.GetComponent<SVGImage>().sprite == noBidIcon)
            {
                if (house.GetLastPersonToBid() != PhotonNetwork.LocalPlayer.ActorNumber && house.GetLastPersonToBid() != 0)
                {
                    gameMasterRef.Photon_SendIdNumber_FinishedCheck(house.GetLastPersonToBid());
                }

                if (bidIconButton.GetComponent<SVGImage>().sprite != noBidIcon)
                {
                    gameMasterRef.playerPhoton.RemoveOneBid();
                }
                bidIconButton.GetComponent<SVGImage>().sprite = yourBidIcon;
                bidIconButton.GetComponent<SVGImage>().color = yourColor;
                house.SetLastPersonToBid(PhotonNetwork.LocalPlayer.ActorNumber);
                house.SetValue(house.GetValue() + gameMasterRef.bidValue);

                gameMasterRef.EstablishHouseListing_Send(house);

                //gameMasterRef.CreateHouseListing(house); DEPRICATED
                gameMasterRef.haveBidOnHouse = true;
                goodHighlight.SetActive(true);
                goodHighlight.GetComponent<SVGImage>().color = yourColor;
            }
            else
            {
                badHighlight.SetActive(true);
                StartCoroutine(ResetHighlight());
            }
        }
        else
        {
            badHighlight.SetActive(true);
            StartCoroutine(ResetHighlight());
        }
    }







    public IEnumerator ResetHighlight()
    {
        yield return new WaitForSeconds(1);
        badHighlight.SetActive(false);
    }



    public void Click_GoToHouseViewer()
    {
        gameMasterRef.MenuBarSelection(7);

        if (gameMasterRef.prevMenuSelected == MenuOptions.YourAssets)
        {
            gameMasterRef.houseViewerManager.GetComponent<HouseViewerManager>().EstablishTheListToView(gameMasterRef.playerPhoton.GetAssetList(), houseID);
        }
        else
        {
            gameMasterRef.houseViewerManager.GetComponent<HouseViewerManager>().EstablishTheListToView(gameMasterRef.currentHouseBids, houseID);
        }

        gameMasterRef.theCard.GetComponent<HouseViewer>().UpdateCardUI(house.GetIncome(), house.GetTitle(), house.GetFamNum(), house.GetVictoryPoints(), house.GetExtraPurplePoints(), house.GetLocation());
    }
}
