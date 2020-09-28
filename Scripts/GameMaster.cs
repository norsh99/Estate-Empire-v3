using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Photon.Pun;
using System;

public class GameMaster : MonoBehaviour
{
    [SerializeField] private HousingBarGraph housingBarGraph;
    public TextMeshPro yearTextField;

    private int b1, b2, b3, b4, b5, b6; //Actual value for the houses
    private int b1f, b2f, b3f, b4f, b5f, b6f; //Values for how much extra a house will cost
    private int bucket1, bucket2, bucket3, bucket4, bucket5, bucket6; //how much buyers are there in the deck
    private int bucketMax1, bucketMax2, bucketMax3, bucketMax4, bucketMax5, bucketMax6;//Max you can add to a bucket
    private int prevBucket1, prevBucket2, prevBucket3, prevBucket4, prevBucket5, prevBucket6; //What were the buyers like last round

    //The menu currently selected
    public MenuOptions menuSelected;
    public MenuOptions prevMenuSelected;


    //Relevant game varibales.
    private int inflation;
    private int year;
    private int totalPlayers;

    //Menu Variable
    public Transform buyScreen;
    public Transform sellScreen;
    public Transform upgradesScreen;
    public Transform homeScreen;
    public Transform buyerCard;
    public Transform openingScreen;
    public Transform boughtScreen;
    public Transform yourAssetsScreen;
    public Transform topAgentScreen;
    public Transform houseViewer;
    public GameObject menuButtons; //For disabling so as to not accidentially be clicked.
    public GameObject textBoxAnimation;

    //OpeningScreenVariables
    public GameObject choosePlayersPanel;
    public GameObject exitPanel;
    public GameObject waitingToStartScreen;
    public GameObject startButton;
    public TextMeshProUGUI playersInGame;
    public Image playersButton2;
    public Image playersButton3;
    public Image playersButton4;
    public TextMeshProUGUI code;
    public TMP_InputField codeInputField;
    public SVGImage JoinButtonSVGImage;

    //Buy Screen Variables
    public Sprite selectedNumberButton;
    public Sprite deselectedNumberButton;
    public Image button1, button2, button3, button4, button5, button6;
    public TextMeshPro textBox1, textBox2, textBox3, textBox4, textBox5, textBox6;
    public GameObject buyButton;
    private int selectionNumberButton;
    public GameObject buyPanelPrefab;
    public GameObject contentForBuyPanel;
    public int bidValue;
    public TextMeshProUGUI bidValueBox;
    public bool haveBidOnHouse;
    public TextMeshProUGUI bidTextBox;


    //Your Assets Screen
    public GameObject contentForYourAssets;

    //Sell Screen First Window Variables
    private int sellButton1, sellButton2, sellButton3, sellButton4, sellButton5, sellButton6;
    private NumberOfRoomsSelected numberOfRoomsSelected;
    public GameObject sellScreenInitialScreen;
    public Canvas SellScreenCanvas;

    //Sell Screen Variables
    private float currentBuyerCardMoney;
    private bool isThereABuyerCardOut;
    private List<BuyerCard> selectedBuyerCardList;
    private List<BuyerCard> buyerCardList1;
    private List<BuyerCard> buyerCardList2;
    private List<BuyerCard> buyerCardList3;
    private List<BuyerCard> buyerCardList4;
    private List<BuyerCard> buyerCardList5;
    private List<BuyerCard> buyerCardList6;
    private int cardNumber; //The number that marks how many buyers we have gone through. This resets to zero of new year.
    public TextMeshPro buyerCardMoney;
    public TextMeshPro buyerCardHousing;
    public Sprite famSprite1;
    public Sprite famSprite2;
    public Sprite famSprite3;
    public Sprite famSprite4;
    public Sprite famSprite5;
    public Sprite famSprite6;
    public GameObject spriteOnBuyerCard;
    public GameObject soldTag;
    public GameObject sellButtonOnBuyerCard;
    private bool isAnimatingSoldButton;
    public TextMeshPro numberSelectedTextBox;
    public GameObject plusOneCard;
    public HouseData currentSale;
    public TextMeshProUGUI offersTextBox;
    private int salePrice;


    //Market Insight Variables
    public Transform insightScreen;


    public TextMeshPro value1, value2, value3, value4, value5, value6;
    public TextMeshPro vp1, vp2, vp3, vp4, vp5, vp6;

    //Player Photon
    public PhotonLobby isMasterClient;
    public PlayerPhotonScript playerPhoton;

    //All the Houses Data
    public List<HouseData> allHouseData;
    public List<HouseData> currentHouseBids;


    public TextMeshProUGUI moneyTextBox;
    public Color[] playerColors;



    //NavBar
    private bool finishBoxSelected;
    public Image finishCheckBoxGameObject;
    public int playersThatAreReady;

    //Bought Screen
    public GameObject boughtAHouseEnable;
    public GameObject didNotBuyAHouseEnable;
    public TextMeshPro titleForHouse;
    public TextMeshProUGUI purplePointsTextBox;
    public TextMeshProUGUI victoryPointsTextBox;
    public TextMeshProUGUI loansTextBox;

    //Upgrade Screen
    public TextMeshPro totalBidsTextBox;
    public TextMeshPro totalOffersTextBox;

    //Input Name Screen
    public SVGImage SubmitButton;
    public TMP_InputField nameInputField;
    public string userName;
    public GameObject nameSubmitScreen;
    public GameObject createAndJoinScreen;

    //Top Agents Screen
    public GameObject scorePanelPrefab;
    public GameObject topAgentContent;
    public List<PlayerScoreListing> playerScoreList;

    //House Viewer
    public GameObject theCard;
    public GameObject houseViewerManager;

    void Start()
    {
        menuSelected = MenuOptions.CreateAndJoin;
        currentHouseBids = new List<HouseData>();
        playerScoreList = new List<PlayerScoreListing>();

        LoadInHouseData();
        //BringInChoosePlayersPanel(); Moved to Photon Lobby
    }

    //Reset the buckets back to 0.
    private void ResetBuckets()
    {
        bucket1 = 1; bucket2 = 1; bucket3 = 1; bucket4 = 1; bucket5 = 1; bucket6 = 1;
    }
    //Updates prevBucket variables to remember the last rounds numbers.
    private void UpdatePrevBucket()
    {
        prevBucket1 = bucket1; prevBucket2 = bucket2; prevBucket3 = bucket3; prevBucket4 = bucket4; prevBucket5 = bucket5; prevBucket6 = bucket6;
    }

    //Fill buckets of buyers randomly. Don't go above max. This should be used after SetBucketMax();
    private void RandomlyDistributeBuyersToBuckets()
    {
        UpdatePrevBucket();
        ResetBuckets();
        int howManyBuyers = (10 * totalPlayers) + 5;
        for (int i = 0; i < howManyBuyers; i++)
        {
            while (true)
            {
                int t = 1; //A clicker to see if we reach 6 skips. If so that means there are no possible buckets open.
                int whichBucket = UnityEngine.Random.Range(1, 7);
                if (whichBucket == 1)
                {
                    if (bucket1 < bucketMax1)
                    {
                        bucket1 += 1;
                        break;
                    }
                } 
                t += 1;
                if (whichBucket == 2)
                {
                    if (bucket2 < bucketMax2)
                    {
                        bucket2 += 1;
                        break;
                    }
                }
                t += 1;
                if (whichBucket == 3)
                {
                    if (bucket3 < bucketMax3)
                    {
                        bucket3 += 1;
                        break;
                    }
                }
                t += 1;
                if (whichBucket == 4)
                {
                    if (bucket4 < bucketMax4)
                    {
                        bucket4 += 1;
                        break;
                    }
                }
                t += 1;
                if (whichBucket == 5)
                {
                    if (bucket5 < bucketMax5)
                    {
                        bucket5 += 1;
                        break;
                    }
                }
                t += 1;
                if (whichBucket == 6)
                {
                    if (bucket6 < bucketMax6)
                    {
                        bucket6 += 1;
                        break;
                    }
                }
                if (t >= 6)
                {
                    break;
                }
            }
        }
    }

    //Setting the maximum potential number of people to a group. This should be updated everyone year
    private void SetBucketMax()
    {
        //maximumNumber = ((valueOfHouseAboveCurrentHouse - valueOfCurrentHouse) / 100.0f) - 2.0f;
        bucketMax1 = (int)(((b2 - b1) / 100) + 5);
        bucketMax2 = (int)(((b3 - b2) / 100) + 5);
        bucketMax3 = (int)(((b4 - b3) / 100) + 5);
        bucketMax4 = (int)(((b5 - b4) / 100) + 5);
        bucketMax5 = (int)(((b6 - b5) / 100) + 5);
        bucketMax6 = 15;
        if (bucketMax1 < 0)
        {
            bucketMax1 = 1;
        }
        if (bucketMax2 < 0)
        {
            bucketMax2 = 1;
        }
        if (bucketMax3 < 0)
        {
            bucketMax3 = 1;
        }
        if (bucketMax4 < 0)
        {
            bucketMax4 = 1;
        }
        if (bucketMax5 < 0)
        {
            bucketMax5 = 1;
        }
    }

    //Does math to convert the value of a house into the coordinate system of the bar graph.
    public void AdjustMarketValueToBarGragh()
    {
        bool relativeNumberRating = false;
        float num1 = 0.0f;
        float num2 = 0.0f;
        float num3 = 0.0f;
        float num4 = 0.0f;
        float num5 = 0.0f;
        float num6 = 0.0f;
        if (relativeNumberRating)
        {
            num1 = (float)b1 / (float)b2;
            num2 = (float)b2 / (float)b3;
            num3 = (float)b3 / (float)b4;
            num4 = (float)b4 / (float)b5;
            num5 = (float)b5 / (float)b6;
            num6 = (float)b6 / 10000.0f;
        }
        else
        {
            num1 = (float)b1 / 10000.0f;
            num2 = (float)b2 / 10000.0f;
            num3 = (float)b3 / 10000.0f;
            num4 = (float)b4 / 10000.0f;
            num5 = (float)b5 / 10000.0f;
            num6 = (float)b6 / 10000.0f;
        }


        if (num1 > 1)
        {
            num1 = 1;
        }
        if (num2 > 1)
        {
            num2 = 1;
        }
        if (num3 > 1)
        {
            num3 = 1;
        }
        if (num4 > 1)
        {
            num4 = 1;
        }
        if (num5 > 1)
        {
            num5 = 1;
        }
        if (num6 > 1)
        {
            num6 = 1;
        }

        SetBarGraphSize(
            num1,
            num2,
            num3,
            num4,
            num5,
            num6);
    }

    //Visually adjusts the graph on screen with inputed values
    private void SetBarGraphSize(float sb1, float sb2, float sb3, float sb4, float sb5, float sb6)
    {
        housingBarGraph.SetSize(sb1, 1);
        housingBarGraph.SetSize(sb2, 2);
        housingBarGraph.SetSize(sb3, 3);
        housingBarGraph.SetSize(sb4, 4);
        housingBarGraph.SetSize(sb5, 5);
        housingBarGraph.SetSize(sb6, 6);
    }

    //Comparing the demand of people this round to last round determines if the price should go up or down.
    private void FluxMarketValues()
    {
        int rateOfAdjustment = 50; //This value can be adjusted to determine how much of a flux happens.
        float sufferRate = 0.5f; //When 0 buyers show up that means the price is too high for them. The market will suffer for it.
        if (bucket1 < prevBucket1) //Meaning less demand this round compared to last round.
        {
            b1 -= bucket1 * rateOfAdjustment;
            if (bucket1 == 0)
            {
                b1 -= (int)((b1*sufferRate)+200);
            }
        }
        else
        {
            b1 += prevBucket1 * rateOfAdjustment;
        }

        if (bucket2 < prevBucket2) 
        {
            b2 -= bucket2 * rateOfAdjustment;
            if (bucket2 == 0)
            {
                b2 -= (int)((b2 * sufferRate) + 200);
            }
        }
        else
        {
            b2 += prevBucket2 * rateOfAdjustment;
        }

        if (bucket3 < prevBucket3)
        {
            b3 -= bucket3 * rateOfAdjustment;
            if (bucket3 == 0)
            {
                b3 -= (int)((b3 * sufferRate) + 200);
            }
        }
        else
        {
            b3 += prevBucket3 * rateOfAdjustment;
        }

        if (bucket4 < prevBucket4)
        {
            b4 -= bucket4 * rateOfAdjustment;
            if (bucket4 == 0)
            {
                b4 -= (int)((b4 * sufferRate) + 200);
            }
        }
        else
        {
            b4 += prevBucket4 * rateOfAdjustment;
        }

        if (bucket5 < prevBucket5)
        {
            b5 -= bucket5 * rateOfAdjustment;
            if (bucket5 == 0)
            {
                b5 -= (int)((b5 * sufferRate) + 200);
            }
        }
        else
        {
            b5 += prevBucket5 * rateOfAdjustment;
        }

        int randCrash = 0;
        int crash = UnityEngine.Random.Range(0, 2);
        //Debug.Log("Crash: " + crash);
        if (b6 > 7000)
        {
            if (crash == 1)
            {
                Debug.Log("Crashed!!!");
                randCrash = -3000;
            }
        }
        if (bucket6 < prevBucket6)
        {

            b6 -= bucket6 * rateOfAdjustment;
            if (bucket6 == 0)
            {
                b6 -= (int)((b6 * sufferRate) + 200);
            }
        }
        else
        {
            b6 += (prevBucket6 * rateOfAdjustment) + randCrash;
        }

        //The final calculations for updating the values of each house.
        int randMarketFlux = HousingValueCalculation();
        b1 += randMarketFlux + b1f;
        b2 += randMarketFlux + b2f;
        b3 += randMarketFlux + b3f;
        b4 += randMarketFlux + b4f;
        b5 += randMarketFlux + b5f;
        b6 += randMarketFlux + b6f;

        b1 = RandomFluxPerHouseCatagory(1, b1);
        b2 = RandomFluxPerHouseCatagory(2, b2);
        b3 = RandomFluxPerHouseCatagory(3, b3);
        b4 = RandomFluxPerHouseCatagory(4, b4);
        b5 = RandomFluxPerHouseCatagory(5, b5);
        b6 = RandomFluxPerHouseCatagory(6, b6);

        b1 = RoundMarketValueTo100s(CurveTheMarket(b1));
        b2 = RoundMarketValueTo100s(CurveTheMarket(b2));
        b3 = RoundMarketValueTo100s(CurveTheMarket(b3));
        b4 = RoundMarketValueTo100s(CurveTheMarket(b4));
        b5 = RoundMarketValueTo100s(CurveTheMarket(b5));
        b6 = RoundMarketValueTo100s(CurveTheMarket(b6));

        b1 = CheckIfZero(b1);
        b2 = CheckIfZero(b2);
        b3 = CheckIfZero(b3);
        b4 = CheckIfZero(b4);
        b5 = CheckIfZero(b5);
        b6 = CheckIfZero(b6);

    }

    private int CheckIfZero(int theNum)
    {

        if (theNum <= 0)
        {
            return 200;
        }
        return theNum;
    }


    //Resets the button numbers to grey default
    private void ClearNumberButtonSelection()
    {
        Color whiteColor = new Color(0.9058824f, 0.9058824f, 0.9058824f);

        selectionNumberButton = 0;

        button1.sprite = deselectedNumberButton;
        button2.sprite = deselectedNumberButton;
        button3.sprite = deselectedNumberButton;
        button4.sprite = deselectedNumberButton;
        button5.sprite = deselectedNumberButton;
        button6.sprite = deselectedNumberButton;
        textBox1.color = whiteColor;
        textBox2.color = whiteColor;
        textBox3.color = whiteColor;
        textBox4.color = whiteColor;
        textBox5.color = whiteColor;
        textBox6.color = whiteColor;
    }

    private void UpdateBuyScreenTextMarketValues()
    {
        textBox1.text = "$" + b1;
        textBox2.text = "$" + b2;
        textBox3.text = "$" + b3;
        textBox4.text = "$" + b4;
        textBox5.text = "$" + b5;
        textBox6.text = "$" + b6;
    }

    //Animates the buyer card moving out of the screen space.
    private void MoveBuyerCardOut(bool autoMoveInNextBuyerCard)
    {
        if (autoMoveInNextBuyerCard)
        {
            buyerCard.DOMoveX(564.0f, 0.25f).OnComplete(MoveBuyerCardIn);
        }
        else
        {
            //Debug.Log("No trigger?");
            MenuScreenActivationTrue();
            buyerCard.DOMoveX(564.0f, 0.25f);
            isThereABuyerCardOut = false;
            isAnimatingSoldButton = false;
        }

    }

    //Animates the buyer card moving in to the screen space
    private void MoveBuyerCardIn()
    {
        soldTag.SetActive(false);
        sellButtonOnBuyerCard.SetActive(true);
        ResetSoldTagPosition();

        UpdateBuyerCardInfo(selectedBuyerCardList[cardNumber].GetRooms(), selectedBuyerCardList[cardNumber].GetMoney(), selectedBuyerCardList[cardNumber].GetPlusOneCard());
        salePrice = selectedBuyerCardList[cardNumber].GetMoney();
        buyerCard.position = new Vector3(-556.0f, 144f, 10);
        buyerCard.DOMoveX(0.0f, 0.25f).OnComplete(MenuScreenActivationFalse); //May not be using anymore
        isThereABuyerCardOut = true;
        cardNumber += 1;
        if (cardNumber >= selectedBuyerCardList.Count)
        {
            cardNumber = 0;
        }
    }

    //Every round(year) this will be called to create the new list of buyers that the buyer cards will display and cycle through.
    private void GenerateBuyerCardList()
    {
        buyerCardList1 = new List<BuyerCard>();
        buyerCardList2 = new List<BuyerCard>();
        buyerCardList3 = new List<BuyerCard>();
        buyerCardList4 = new List<BuyerCard>();
        buyerCardList5 = new List<BuyerCard>();
        buyerCardList6 = new List<BuyerCard>();
        //int totalNumberOfBuyers = bucket1 + bucket2 + bucket3 + bucket4 + bucket5 + bucket6;

        //for (int i = 0; i < totalNumberOfBuyers; i++)
        //{
        //    int theNum = 0;
        //    if (i < bucket1 + bucket2 + bucket3 + bucket4 + bucket5 + bucket6)
        //    {
        //        theNum = 6;
        //    }
        //    if (i < bucket1 + bucket2 + bucket3 + bucket4 + bucket5)
        //    {
        //        theNum = 5;
        //    }
        //    if (i < bucket1 + bucket2 + bucket3 + bucket4)
        //    {
        //        theNum = 4;
        //    }
        //    if (i < bucket1 + bucket2 + bucket3)
        //    {
        //        theNum = 3;
        //    }
        //    if (i < bucket1 + bucket2)
        //    {
        //        theNum = 2;
        //    }
        //    if (i < bucket1)
        //    {
        //        theNum = 1;
        //    }
        //    BuyerCard bc = new BuyerCard(0, theNum);
        //    buyerCardList1.Add(bc);
        //}
        int numFlux1 = LowestValueForBuyerCardGeneration(b1);
        int numFlux2 = LowestValueForBuyerCardGeneration(b2);
        int numFlux3 = LowestValueForBuyerCardGeneration(b3);
        int numFlux4 = LowestValueForBuyerCardGeneration(b4);
        int numFlux5 = LowestValueForBuyerCardGeneration(b5);
        int numFlux6 = LowestValueForBuyerCardGeneration(b6);
        int rateOfAdjustment = 100;
        for (int i = 0; i < bucket1; i++)
        {
            int assignAmountOfMoney = numFlux1 + b1;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux1 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }

            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 1, givePlusOneCard);
            buyerCardList1.Add(bc);
            numFlux1 += rateOfAdjustment;
        }
        for (int i = 0; i < bucket2; i++)
        {
            int assignAmountOfMoney = numFlux2 + b2;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux2 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }
            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 2, givePlusOneCard);
            buyerCardList2.Add(bc);
            numFlux2 += rateOfAdjustment;
        }
        for (int i = 0; i < bucket3; i++)
        {
            int assignAmountOfMoney = numFlux3 + b3;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux3 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }
            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 3, givePlusOneCard);
            buyerCardList3.Add(bc);
            numFlux3 += rateOfAdjustment;
        }
        for (int i = 0; i < bucket4; i++)
        {
            int assignAmountOfMoney = numFlux4 + b4;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux4 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }
            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 4, givePlusOneCard);
            buyerCardList4.Add(bc);
            numFlux4 += rateOfAdjustment;
        }
        for (int i = 0; i < bucket5; i++)
        {
            int assignAmountOfMoney = numFlux5 + b5;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux5 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }
            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 5, givePlusOneCard);
            buyerCardList5.Add(bc);
            numFlux5 += rateOfAdjustment;
        }
        for (int i = 0; i < bucket6; i++)
        {
            int assignAmountOfMoney = numFlux6 + b6;
            bool givePlusOneCard = false;
            int randomChance = UnityEngine.Random.Range(0, 2);
            if (numFlux6 <= 0 && randomChance == 1)
            {
                givePlusOneCard = true;
            }
            BuyerCard bc = new BuyerCard(assignAmountOfMoney, 6, givePlusOneCard);
            buyerCardList6.Add(bc);
            numFlux6 += rateOfAdjustment;
        }
        buyerCardList1 = ShuffleList(buyerCardList1);
        buyerCardList2 = ShuffleList(buyerCardList2);
        buyerCardList3 = ShuffleList(buyerCardList3);
        buyerCardList4 = ShuffleList(buyerCardList4);
        buyerCardList5 = ShuffleList(buyerCardList5);
        buyerCardList6 = ShuffleList(buyerCardList6);
    }

    private int LowestValueForBuyerCardGeneration(int houseCatagoryValue)
    {
        int lowestValue = -500;
        if (houseCatagoryValue < (lowestValue * -1))
        {
            lowestValue = 0;
        }
        return lowestValue;
    }



    private List<BuyerCard> ShuffleList(List<BuyerCard> theList)
    {
        //Time to shuffle the deck!
        for (int i = 0; i < 200; i++)
        {
            int pullCardFromHere = UnityEngine.Random.Range(0, theList.Count);
            BuyerCard tempHoldBuyerCard = theList[pullCardFromHere];
            theList.RemoveAt(pullCardFromHere);
            theList.Add(tempHoldBuyerCard);
        }

        return theList;
    }
    //Will pull info from the buyer list for the round and update the visuals on the buyer card.
    private void UpdateBuyerCardInfo(int houseingNum, int buyerVal, bool plusOneCard2)
    {
        //int randomMoneyDistibution = Random.Range(0, 4); //Some Patrons will give more for a house, randomly.
        if (houseingNum == 1)
        {
            //currentBuyerCardMoney = b1 + randomMoneyDistibution * 100;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite1;

        }
        if (houseingNum == 2)
        {
            //currentBuyerCardMoney = b2 + randomMoneyDistibution * 100;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite2;
        }
        if (houseingNum == 3)
        {
            //currentBuyerCardMoney = b3 + randomMoneyDistibution * 100;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite3;
        }
        if (houseingNum == 4)
        {
            //currentBuyerCardMoney = b4 + randomMoneyDistibution * 200;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite4;
        }
        if (houseingNum == 5)
        {
            //currentBuyerCardMoney = b5 + randomMoneyDistibution * 200;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite5;
        }
        if (houseingNum == 6)
        {
            //currentBuyerCardMoney = b6 + randomMoneyDistibution * 200;
            spriteOnBuyerCard.GetComponent<SpriteRenderer>().sprite = famSprite6;
        }

        //buyerCardMoney.text = currentBuyerCardMoney.ToString();
        buyerCardMoney.text = buyerVal.ToString();
        buyerCardHousing.text = houseingNum.ToString();
        plusOneCard.SetActive(plusOneCard2);
    }

    //Returns the market value excluding indidual housing values. This function should consider inflation and the random value.
    private int HousingValueCalculation()
    {
        int theMoney = 0;
        int marketCrashChance = UnityEngine.Random.Range(0, 6);
        int marketRandFluxValue = 0;

        if (marketCrashChance == 1)
        {
            marketRandFluxValue = -300 - (year * 100); //How much the market will decrease if the market crashes.
            //Debug.Log("Market Crashed");
        }
        if (marketCrashChance == 2)
        {
            marketRandFluxValue = 200 - (year * 100); 
            //Debug.Log("Market Crashed");
        }
        if (marketCrashChance == 3)
        {
            marketRandFluxValue = 200 + (year * 100); 
            //Debug.Log("Market Crashed");
        }
        if (b6 > 6500)
        {
            if (marketCrashChance == 4 || marketCrashChance == 5)
            {
                marketRandFluxValue += -400;
            }
        }
        if (b6 > 7000)
        {
            if (marketCrashChance == 4 || marketCrashChance == 5)
            {
                marketRandFluxValue += -400;
            }
        }
        if (b6 > 7500)
        {
            if (marketCrashChance == 4 || marketCrashChance == 5)
            {
                marketRandFluxValue += -400;
            }
        }

        theMoney += inflation + marketRandFluxValue;
        //CreateTextBoxAnimation(theMoney);
        return theMoney;
    }

    //Reset the values that store if a house has been sold or bought this round.
    private void ResetBuyAndSellVariables()
    {
        b1f = 0;
        b2f = 0;
        b3f = 0;
        b4f = 0;
        b5f = 0;
        b6f = 0;
    }

    private void AnimateSaleTagIn()
    {
        soldTag.SetActive(true);
        isAnimatingSoldButton = true;
        sellButtonOnBuyerCard.SetActive(false);
        soldTag.transform.DORotate(new Vector3(0.0f, 0.0f, 24.0f), 0.25f);
        soldTag.transform.DOScale(1.0f, 0.25f);
        soldTag.transform.DOMove(new Vector3(0, 0, 0), 1.0f).OnComplete(MoveSellScreenOut);
    }


    private void ResetSoldTagPosition()
    {
        soldTag.transform.DORotate(new Vector3(0.0f, 0.0f, -21.671f), 0.0f);
        soldTag.transform.DOScale(2.105784f, 0.0f);
    }

    private void StartTheGame()
    {
        year = 1;
        inflation = 0;

        numberOfRoomsSelected = NumberOfRoomsSelected.None;
        isThereABuyerCardOut = false;
        cardNumber = 0;
        isAnimatingSoldButton = false;
        haveBidOnHouse = false;
        finishBoxSelected = false; 
        finishCheckBoxGameObject.color = new Color(1, 1, 1, 1);
        playersThatAreReady = 0;

        b1 = 1000; b2 = 2000; b3 = 3000; b4 = 4000; b5 = 5000; b6 = 6000; //Actual value for the houses
        b1f = 0; b2f = 0; b3f = 0; b4f = 0; b5f = 0; b6f = 0; //Values for how much extra a house will cost

        AdjustMarketValueToBarGragh();

        SetBucketMax();
        RandomlyDistributeBuyersToBuckets();
        GenerateBuyerCardList();

        CreatePlayerInfo();

        //UpdateYourInfoLocaly();
        //SetFinishBoxToTrue(false);
        //SetFinishBoxToFalse();

        //SetFinishBoxToFalse(); //TODO This might not work

    }

    public void BringInChoosePlayersPanel()
    {
        choosePlayersPanel.transform.DOMove(new Vector3(0.0f, 11.0f, 6.93f), 0.5f).SetEase(Ease.InOutBack);
    }

    private void MoveOpeningScreenOut()
    {
        MenuScreenActivationTrue();
        openingScreen.transform.DOMove(new Vector3(0, -1130, -6.93f), 0.5f).SetEase(Ease.InCirc);
    }

    private void MoveOpeningScreenIn()
    {
        openingScreen.transform.DOMove(new Vector3(0, 0, -6.93f), 0.5f).OnComplete(MenuScreenActivationFalse);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void MoveSellScreenOut()
    {
        MenuScreenActivationTrue();
        UpdateMenuSelection(MenuOptions.Home);
        //sellScreen.DOMoveX(564.0f, 0.25f).OnComplete(ResetInitialSellScreenTrue);
        //ResetInitialSellScreenTrue();
        MoveBuyerCardOut(false);
    }

    private void ResetInitialSellScreenTrue()
    {
        SellScreenCanvas.gameObject.SetActive(true);
        sellScreenInitialScreen.transform.position = new Vector3(564f, 0, 10f);
    }

    //Make sure that you can't drop below zero or go over 10,000
    private int CurveTheMarket(int theMoney)
    {
        if (theMoney > 10000)
        {
            theMoney = 10000;
        }
        if (theMoney < 0)
        {
            theMoney = 200;
        }

        return theMoney;
    }

    private void MenuScreenActivationTrue()
    {
        //menuButtons.SetActive(true);
    }
    private void MenuScreenActivationFalse()
    {
        //menuButtons.SetActive(false);
    }

    //Move in the Market Insight Screen
    private void MoveInsightScreenIn()
    {
        UpdateMarketInsightScreen();
        insightScreen.transform.DOMove(new Vector3(0, 0, -6.93f), 0.5f).OnComplete(MenuScreenActivationFalse);
    }

    //Move out the Market Insight Screen
    private void MoveInsightScreenOut()
    {
        MenuScreenActivationTrue();
        insightScreen.DOMoveY(-1107.0f, 0.25f);
    }

    private float ConvertToFloatDivision(int firstNum, int totalNum)
    {
        float calculation = 0.0f;
        float num1 = firstNum * 1.0f;
        float num2 = totalNum * 1.0f;
        calculation += num1 / num2;
        calculation = Mathf.Round(calculation*100.0f);
        return calculation;
    }

    //Update the insight screen full of data like percentages of buyers and values of each house on the market.
    private void UpdateMarketInsightScreen()
    {
        int totalBuyersInDeck = (bucket1 + bucket2 + bucket3 + bucket4 + bucket5 + bucket6);

        value1.text = "$" + b1;
        value2.text = "$" + b2;
        value3.text = "$" + b3;
        value4.text = "$" + b4;
        value5.text = "$" + b5;
        value6.text = "$" + b6;

        float v1 = ConvertToFloatDivision(bucket1, totalBuyersInDeck);
        float v2 = ConvertToFloatDivision(bucket2, totalBuyersInDeck);
        float v3 = ConvertToFloatDivision(bucket3, totalBuyersInDeck);
        float v4 = ConvertToFloatDivision(bucket4, totalBuyersInDeck);
        float v5 = ConvertToFloatDivision(bucket5, totalBuyersInDeck);
        float v6 = ConvertToFloatDivision(bucket6, totalBuyersInDeck);

        vp1.text = bucket1.ToString();
        vp2.text = bucket2.ToString();
        vp3.text = bucket3.ToString();
        vp4.text = bucket4.ToString();
        vp5.text = bucket5.ToString();
        vp6.text = bucket6.ToString();
    }


    private void CreateTextBoxAnimation(int theValue)
    {
        GameObject a = Instantiate(textBoxAnimation);
        a.transform.position = new Vector3(0,-275,-6f);
        //a.transform.localScale = a.transform.localScale;
        a.GetComponentInChildren<TextMeshPro>().text = "$" + theValue.ToString();
        if (theValue > 0)
        {
            a.GetComponentInChildren<TextMeshPro>().color = new Color(0.31f, 0.89f, 0.27f); //green
        }
        if (theValue < 0)
        {
            a.GetComponentInChildren<TextMeshPro>().color = new Color(0.89f, 0.37f, 0.27f); //red
        }
        if (theValue == 0)
        {
            a.GetComponentInChildren<TextMeshPro>().color = new Color(0.36f, 0.36f, 0.36f); //grey
        }
    }

    private int RoundMarketValueTo100s(int value)
    {
        float newValue = (float)value / 100.0f;
        int newIntValue = (int)Mathf.Round(newValue);
        newIntValue = newIntValue * 100;

        return newIntValue;
    }

    //Randomly increase or decrease catagories if they are above or below there market value significantly
    private int RandomFluxPerHouseCatagory(int numOfRooms, int value)
    {
        int swingValue = 1000 + (year+100);
        int randomChance = UnityEngine.Random.Range(0, 2); //First num is inclusive, second number is not.
        if (numOfRooms == 1)
        {
            if (value < 500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 1500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        if (numOfRooms == 2)
        {
            if (value < 1500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 2500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        if (numOfRooms == 3)
        {
            if (value < 2500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 3500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        if (numOfRooms == 4)
        {
            if (value < 3500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 4500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        if (numOfRooms == 5)
        {
            if (value < 4500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 5500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        if (numOfRooms == 6)
        {
            if (value < 5500 + inflation && randomChance == 0)
            {
                return value + swingValue + 300;
            }
            if (value > 6500 + inflation && randomChance == 0)
            {
                return value - swingValue;
            }
        }
        return value;
    }

    public void MoveScreenOut(Transform screen, float speed = 0.75f, bool doReverse = false)
    {
        if (doReverse)
        {
            screen.DOMove(new Vector3(-551, 0, 0), speed);
            return;
        }
        screen.DOMove(new Vector3(551, 0, 0), speed);
    }

    public void MoveScreenIn(Transform screen, float speed = 0.75f, bool doReverse = false)
    {
        if (doReverse)
        {
            screen.transform.position = new Vector3(551, 0, 0);
            screen.DOMove(new Vector3(0, 0, 0), speed);
            return;
        }
        screen.transform.position = new Vector3(-551, 0, 0);
        screen.DOMove(new Vector3(0, 0, 0), speed);
    }

    private Transform FindTransformFromMenuSelection(MenuOptions menu)
    {
        Transform theTransform = homeScreen;
        if (menu == MenuOptions.Buy)
        {
            theTransform = buyScreen;
        }
        if (menu == MenuOptions.Upgrades)
        {
            theTransform = upgradesScreen;
        }
        if (menu == MenuOptions.Home)
        {
            theTransform = homeScreen;
        }
        if (menu == MenuOptions.Sell)
        {
            theTransform = sellScreen;
        }
        if (menu == MenuOptions.CreateAndJoin)
        {
            theTransform = openingScreen;
        }
        if (menu == MenuOptions.BoughtScreen)
        {
            theTransform = boughtScreen;
        }
        if (menu == MenuOptions.YourAssets)
        {
            theTransform = yourAssetsScreen;
        }
        if (menu == MenuOptions.TopAgentScreen)
        {
            theTransform = topAgentScreen;
        }
        if (menu == MenuOptions.HouseViewer)
        {
            theTransform = houseViewer;
        }
        return theTransform;
    }

    public void UpdateMenuSelection(MenuOptions selected)
    {
        if (selected != menuSelected)
        {
            MenuOptions prevMenu = menuSelected;
            prevMenuSelected = menuSelected; //Global awareness
            MenuOptions goToMenu = selected;

            menuSelected = selected;

            MoveScreenOut(FindTransformFromMenuSelection(prevMenu));
            MoveScreenIn(FindTransformFromMenuSelection(selected));
        }

    }

    public HouseData ReturnHouseStats(int id)
    {
        for (int i = 0; i < allHouseData.Count; i++)
        {
            if (id == allHouseData[i].GetID())
            {
                return allHouseData[i];
            }
        }
        return null;
    }

    public bool FindHouseID(int id)
    {
        for (int i = 0; i < allHouseData.Count; i++)
        {
            if (id == allHouseData[i].GetID())
            {
                return true;
            }
        }
        return false;
    }

    public bool FindHouseAndReplace(HouseData house)
    {
        for (int i = 0; i < currentHouseBids.Count; i++)
        {
            if (house.GetID() == currentHouseBids[i].GetID())
            {
                Debug.Log("FOUND HOUSEE");
                currentHouseBids[i].SetValue(house.GetValue());
                currentHouseBids[i].SetLastPersonToBid(house.GetLastPersonToBid());

                house.GetGameObjectOfSelf_BuyScreen().GetComponent<BuyPanel>().UpdateUI(house);
                return true;
            }
        }
        return false;
    }

    public void EstablishHouseListing_Send(HouseData house)
    {
        if (!FindHouseAndReplace(house))
        {
            currentHouseBids.Add(house);
        }
        Photon_SendListOfBids(house.GetID(), house.GetValue(), house.GetLastPersonToBid());
    }

    public void EstablishHouseListing_Recieve(HouseData house)
    {
        if (!FindHouseAndReplace(house))
        {
            currentHouseBids.Add(house);
            CreateNewListing(house); //Create the Panel visual //TODO this is new hope it works
        }
    }

    private HouseData PackageNewHouse(int id, int val, int lastBid)
    {
        HouseData newHouse = ReturnHouseStats(id);
        newHouse.SetValue(val);
        newHouse.SetLastPersonToBid(lastBid);
        return newHouse;
    }

    public void UpdateBidValue(int value, bool incriment)
    {
        if (incriment)
        {
            bidValue += value;
            if (bidValue < 100)
            {
                bidValue = 100;
            }
        }
        else
        {
            bidValue = value;
        }
        //Update bid value UI
        bidValueBox.text = bidValue.ToString();

    }

    private void SetFinishBoxToFalse(bool passRPC = true)
    {

        finishBoxSelected = false;
        UpdateYourInfoLocaly();
        finishCheckBoxGameObject.color = new Color(1, 1, 1, 1);
        if (passRPC)
        {
            UpdateIAmNotReady();
        }

    }

    private void SetFinishBoxToTrue(bool passRPC = true)
    {

        finishBoxSelected = true;
        UpdateYourInfoLocaly();
        finishCheckBoxGameObject.color = new Color(0.2156863f, 0.827451f, 0.4039216f, 1);
        if (passRPC)
        {
            UpdateIAmReady();
        }

    }

    public void UpdateIAmReady()
    {
        Photon_SendYouAreReady(1);
    }

    public void UpdateIAmNotReady()
    {
        Photon_SendYouAreReady(-1);
    }

    public void CheckIfAllPlayersAreReady()
    {
        if (playersThatAreReady == PhotonNetwork.PlayerList.Length)
        {
            GoToBoughtScreen();
        }
    }

    //Searches the BuyContent.listOfPanels list for a house you bid on. 
    public HouseData DidPurchaseHome()
    {

        //List<GameObject> listOfPanels = contentForBuyPanel.GetComponent<BuyContent>().listOfPanels;
        if (currentHouseBids != null)
        {
            for (int i = 0; i < currentHouseBids.Count; i++)
            {
                if (currentHouseBids[i].GetLastPersonToBid() == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //Debug.Log("FOUND HOUSE!");
                    return currentHouseBids[i];
                }
            }
        }

        return null;
    }

    //When everyone agrees to finish there round sned them through here
    public void GoToBoughtScreen()
    {

        UpdateMenuSelection(MenuOptions.BoughtScreen);

        HouseData houseBought = DidPurchaseHome();

        if (houseBought == null)
        {
            boughtAHouseEnable.SetActive(false);
            didNotBuyAHouseEnable.SetActive(true);
            StartCoroutine(PauseCalcPay(CalculateTotalPay(), 0f));
            StartCoroutine(PauseCalcPay(CalculateYearlyLoans(), 3.4f));
        }
        else
        {
            playerPhoton.AddHouseToYourList(houseBought); // Adds the bought house to your list
            BuyButtonOnBuyScreen(houseBought.GetFamNum());
            boughtAHouseEnable.SetActive(true);
            didNotBuyAHouseEnable.SetActive(false);
            titleForHouse.text = houseBought.GetTitle();
            //Animate spending cash in money varibale
            playerPhoton.AddMoney(-houseBought.GetValue());
            StartCoroutine(PauseCalcPay(CalculateTotalPay(), 1.6f));
            StartCoroutine(PauseCalcPay(CalculateYearlyLoans(), 5.0f));
            playerPhoton.AddVictoryPoints(houseBought.GetVictoryPoints());
            playerPhoton.PlusOnePurplePoint();

            playerPhoton.SetExtraVictoryPoints(CalculateExtraVictoryPoints());

            CreateTextBoxAnimation(-houseBought.GetValue());

            if (houseBought.GetExtraPurplePoints())
            {
                playerPhoton.PlusOnePurplePoint();
            }
            playerPhoton.UpdateDashboardUI();
        }
        StartNewYear();
        playerPhoton.ResetBids();
        playerPhoton.ResetOffers();
        contentForBuyPanel.GetComponent<BuyContent>().DestroyList();
        currentHouseBids = new List<HouseData>();

    }

    IEnumerator PauseCalcPay(int val, float time)
    {
        yield return new WaitForSeconds(time);
        playerPhoton.AddMoney(val);
        CreateTextBoxAnimation(val);
        UpdateYourInfoLocaly();
    }

    public int CalculateTotalPay()
    {
        int totalMoney = 0;
        List<HouseData> theList = playerPhoton.GetAssetList();
        for (int i = 0; i < theList.Count; i++)
        {
            if (theList != null)
            {
                totalMoney += theList[i].GetIncome();
            }
        }
        return totalMoney;
    }

    public int CalculateYearlyLoans()
    {
        int loans = playerPhoton.GetLoans();
        int rate = -100;
        int amountToPay = loans * rate;
        //playerPhoton.AddMoney(amountToPay);
        return amountToPay;
    }

    public int GetValueForHome(int homeVal)
    {
        if (homeVal == 1)
        {
            return b1;
        }
        if (homeVal == 2)
        {
            return b2;
        }
        if (homeVal == 3)
        {
            return b3;
        }
        if (homeVal == 4)
        {
            return b4;
        }
        if (homeVal == 5)
        {
            return b5;
        }
        if (homeVal == 6)
        {
            return b6;
        }
        return 0;
    }

    public int CalculateExtraVictoryPoints()
    {
        int teamLisle = 0;
        int teamDownersGrove = 0;
        int teamNaperville = 0;
        int teamDavie = 0;
        int teamWeston = 0;

        int finalVa = 0;

        List<HouseData> theList = playerPhoton.GetAssetList();

        for (int i = 0; i < theList.Count; i++)
        {
            if (theList[i].GetLocation() == "Lisle")
            {
                teamLisle += 1;
            }
            if (theList[i].GetLocation() == "Downers Grove")
            {
                teamDownersGrove += 1;
            }
            if (theList[i].GetLocation() == "Naperville")
            {
                teamNaperville += 1;
            }
            if (theList[i].GetLocation() == "Davie")
            {
                teamDavie += 1;
            }
            if (theList[i].GetLocation() == "Weston")
            {
                teamWeston += 1;
            }

        }

        finalVa = GetVP(teamLisle) + GetVP(teamDownersGrove) + GetVP(teamNaperville) + GetVP(teamDavie) + GetVP(teamWeston);

        return finalVa;
    }

    private int GetVP(int val)
    {
        if (val == 2)
        {
            return 1;
        }
        if (val == 3)
        {
            return 3;
        }
        if (val == 4)
        {
            return 6;
        }
        if (val == 5)
        {
            return 10;
        }
        if (val == 6)
        {
            return 15;
        }
        if (val >= 7)
        {
            return 21;
        }
        return 0;
    }

    //The visual panel instantiation through Photon
    public void CreateNewListing(HouseData theHouse)
    {
        GameObject newBuyPanel = Instantiate(buyPanelPrefab, transform.position = new Vector3(0,0,-9), Quaternion.identity, contentForBuyPanel.transform);
        contentForBuyPanel.GetComponent<BuyContent>().listOfPanels.Add(newBuyPanel);
        newBuyPanel.transform.SetSiblingIndex(0);

        newBuyPanel.GetComponent<BuyPanel>().CreatePanelFromPhoton(theHouse);
    }

    //Oganizes all players victory points from top to bottom in the list
    //This gets launched everytime the user clicks on the button to go to the Top Agents screen
    public void RankPlayers()
    {
        playerScoreList.Sort(); //Class needs to be Icomparable so as to use the sort function
        for (int i = 0; i < playerScoreList.Count; i++)
        {
            playerScoreList[i].GetThePanel().transform.SetSiblingIndex(i);
        }
    }

    //Gets called only once at the beginning of the game
    public void CreatePlayerInfo()
    {
        GameObject newPanel = Instantiate(scorePanelPrefab,  transform.localPosition = new Vector3(0, 0, 2), Quaternion.identity, topAgentContent.transform);
        PlayerScoreListing newPlayer = new PlayerScoreListing(PhotonNetwork.LocalPlayer.ActorNumber, playerPhoton.GetTotalVictorypoints(), playerPhoton.GetMoney(), userName, finishBoxSelected);
        newPlayer.SetThePanel(newPanel);
        playerScoreList.Add(newPlayer);
        newPlayer.UpdateUI(newPlayer);
        Send_PlayerInfo(newPlayer);
    }

    //Send the info
    public void Send_PlayerInfo(PlayerScoreListing theInfo)
    {
        playerPhoton.UpdatePlayerInfo(theInfo);
    }

    public void UpdateYourInfoLocaly()
    {
        PlayerScoreListing newPlayer = new PlayerScoreListing(PhotonNetwork.LocalPlayer.ActorNumber, playerPhoton.GetTotalVictorypoints(), playerPhoton.GetMoney(), userName, finishBoxSelected);
        Update_PlayerInfo(newPlayer.GetID(), newPlayer.GetVP(), newPlayer.GetName(), newPlayer.GetMoney(), newPlayer.GetCompletedTurn());
        Send_PlayerInfo(newPlayer);
    }

    //receiving data can use this
    public void Update_PlayerInfo(int id, int vp, string name, int money, bool turn)
    {
        for (int i = 0; i < playerScoreList.Count; i++)
        {
            if (playerScoreList[i].GetID() == id)
            {
                playerScoreList[i].SetMoney(money);
                playerScoreList[i].SetVP(vp);
                playerScoreList[i].SetCompletedTurn(turn);
                playerScoreList[i].UpdateUI(playerScoreList[i]);
                RankPlayers();
                return;
            }
        }
        GameObject newPanel = Instantiate(scorePanelPrefab, transform.position = new Vector3(0, 0, 2), Quaternion.identity, topAgentContent.transform);
        PlayerScoreListing newPlayer = new PlayerScoreListing(id, vp, money, name, turn);
        newPlayer.SetThePanel(newPanel);
        playerScoreList.Add(newPlayer);
        newPlayer.UpdateUI(newPlayer);
    }

    public int GetYourListPosition()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                //Debug.Log("Id slot you: " + i);
                return i;
            }
        }
        return 0;
    }

    public int GetOthersListPosition(int numId)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (numId == PhotonNetwork.PlayerList[i].ActorNumber)
            {
                //Debug.Log("Id slot others: " + i);
                //Debug.Log("FOUND THE OTHER PLAYERS COLOR");
                return i;
            }
        }
        return 0;
    }


    public Color GetColorOfPlayer(int playerNum)
    {
        Color newColor = new Color();
        if (playerNum == 0)
        {
            newColor = playerColors[0];
        }
        if (playerNum == 1)
        {
            newColor = playerColors[1];
        }
        if (playerNum == 2)
        {
            newColor = playerColors[2];
        }
        if (playerNum == 3)
        {
            newColor = playerColors[3];
        }
        if (playerNum == 4)
        {
            newColor = playerColors[4];
        }
        return newColor;
    }


    //TODO
    //------------------------------PHOTON---------------------------------------------------------------------------------

    public bool IsPhotonMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            return true;
        }
        return false;
    }

    //RECEIVE-----------------------------
    public void Photon_UpdateHousingValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        b1 = val1;
        b2 = val2;
        b3 = val3;
        b4 = val4;
        b5 = val5;
        b6 = val6;
    }

    public void Photon_UpdateYear(int y1)
    {
        year = y1;
        yearTextField.text = "Year " + year;
        cardNumber = 0;

        numberOfRoomsSelected = NumberOfRoomsSelected.None;
        isThereABuyerCardOut = false;
        isAnimatingSoldButton = false;
        haveBidOnHouse = false;
        finishBoxSelected = false;
        finishCheckBoxGameObject.color = new Color(1, 1, 1, 1);
        playersThatAreReady = 0;
        GenerateBuyerCardList(); 
    }

    public void Photon_UpdateBfValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        b1f = val1;
        b2f = val2;
        b3f = val3;
        b4f = val4;
        b5f = val5;
        b6f = val6;
    }

    public void Photon_UpdateBucketValues(int val1, int val2, int val3, int val4, int val5, int val6)
    {
        bucket1 = val1;
        bucket2 = val2;
        bucket3 = val3;
        bucket4 = val4;
        bucket5 = val5;
        bucket6 = val6;
    }


    public void Photon_UpdateListofBids(int id, int value, int lastBid)
    {
        EstablishHouseListing_Recieve(PackageNewHouse(id, value, lastBid));
    }

    public void Photon_UpdateWhoIsReady(int num) 
    {
        playersThatAreReady += num;
    }

    public void Photon_RecieveIdMatch_AllPlayersFinished(int id)
    {
        if (id == PhotonNetwork.LocalPlayer.ActorNumber && finishBoxSelected)
        {
            SetFinishBoxToFalse(true);
        }
    }

    //SEND---------------------------------
    public void Photon_SendYear()
    {
        playerPhoton.UpdateMarketData(b1, b2, b3, b4, b5, b6);
        Photon_SendBucketValues();
        playerPhoton.StartNewYear(year);
    }

    public void Photon_SendBfValues()
    {
        playerPhoton.UpdateBfValues(b1f, b2f, b3f, b4f, b5f, b6f);
    }

    public void Photon_SendBucketValues()
    {
        playerPhoton.UpdateBucketValues(bucket1, bucket2, bucket3, bucket4, bucket5, bucket6);
    }

    public void Photon_SendListOfBids(int id, int value, int lastBid)
    {
        playerPhoton.UpdateListOfBids(id, value, lastBid);
    }

    public void Photon_SendYouAreReady(int num)
    {
        playerPhoton.UpdateIAmReady(num);
    }

    public void Photon_SendIdNumber_FinishedCheck(int id)
    {
        playerPhoton.FinishedIdCheck(id);
    }


    //------------------------------BUTTONS-------------------------------------------------------------------------------




    //This function, when clicked, will move the year up by one and generate
    //a new group of buyers in the market. The market for each of the houses will adjust.
    public void StartNewYear()
    {
        if (IsPhotonMasterClient())
        {
            year += 1;
            inflation += 100;
            yearTextField.text = "Year " + year;
            cardNumber = 0;
            haveBidOnHouse = false;


            numberOfRoomsSelected = NumberOfRoomsSelected.None;
            isThereABuyerCardOut = false;
            isAnimatingSoldButton = false;
            SetFinishBoxToFalse(false);
            playersThatAreReady = 0;

            SetBucketMax();
            RandomlyDistributeBuyersToBuckets();
            FluxMarketValues();
            //AdjustMarketValueToBarGragh(); Moved to Bought Screen
            GenerateBuyerCardList();
            ResetBuyAndSellVariables();

            //PHOTON
            Photon_SendYear();
        }
        else
        {
            Debug.Log("Not the master client! Can't progress the year");
        }
    }

    public void MenuBarSelection(int numSelected)
    {
        if (menuSelected != MenuOptions.BoughtScreen)
        {
            if (isThereABuyerCardOut)
            {
                MoveBuyerCardOut(false);
            }

            if (numSelected == 1)
            {
                //Buy Button
                UpdateMenuSelection(MenuOptions.Buy);
                UpdateBidValue(100, false);
            }
            if (numSelected == 2)
            {
                //Upgrades Button
                UpdateMenuSelection(MenuOptions.Upgrades);
            }
            if (numSelected == 3)
            {
                //Home Button
                UpdateMenuSelection(MenuOptions.Home);
            }
            if (numSelected == 4)
            {
                //Your Asset Button 
                UpdateMenuSelection(MenuOptions.YourAssets);
                playerPhoton.UpdateAllAssetUI();
            }
            if (numSelected == 5)
            {

                //Finished Button
                if (finishBoxSelected)
                {
                    SetFinishBoxToFalse();
                    Debug.Log("Finished Button Not Selected");
                }
                else
                {
                    SetFinishBoxToTrue();
                    Debug.Log("Finished Button Selected");
                }
            }
            if (numSelected == 6)
            {
                //Top Agent Screen 
                UpdateMenuSelection(MenuOptions.TopAgentScreen);
                RankPlayers();
            }
            if (numSelected == 7)
            {
                //House Viewer
                UpdateMenuSelection(MenuOptions.HouseViewer);
            }
        }

    }



    //The button on the Sellscreen that moves the buyer card in and out.
    public void BuyerCardMove()
    {
        if (!isAnimatingSoldButton && playerPhoton.GetOffers() > 0) {
            if (!isThereABuyerCardOut)
            {
                MoveBuyerCardIn();

            }
            else
            {
                MoveBuyerCardOut(true);

            }
            playerPhoton.RemoveOneOffer();
        }
    }

    //When you want to actually purchase a home click this button on the buy screen.
    public void BuyButtonOnBuyScreen(int photonSelect = 0)
    {
        int valueInflux = 700; //How much money is added to the market when a house is bought

        if (selectionNumberButton == 1 || photonSelect == 1)
        {
            b1f += valueInflux - 200;
        }
        if (selectionNumberButton == 2 || photonSelect == 2)
        {
            b2f += valueInflux - 100;
        }
        if (selectionNumberButton == 3 || photonSelect == 3)
        {
            b3f += valueInflux;
        }
        if (selectionNumberButton == 4 || photonSelect == 4)
        {
            b4f += valueInflux;
        }
        if (selectionNumberButton == 5 || photonSelect == 5)
        {
            b5f += valueInflux + 100;
        }
        if (selectionNumberButton == 6 || photonSelect == 6)
        {
            b6f += valueInflux + 200;
        }

        //PHOTON
        Photon_SendBfValues();
    }

    public void BuyScreenExitButton()
    {
        MenuScreenActivationTrue(); 
        buyScreen.DOMoveX(-556.0f, 0.25f);
        ClearNumberButtonSelection();
        buyButton.SetActive(false);
    }

    public void NumberButtonSelection(int num)
    {
        Color greenColor = new Color(0, 1, 0.1185064f);
        ClearNumberButtonSelection();
        buyButton.SetActive(true);
        if (num == 1)
        {
            selectionNumberButton = 1;
            button1.sprite = selectedNumberButton;
            textBox1.color = greenColor;
        }
        if (num == 2)
        {
            selectionNumberButton = 2;
            button2.sprite = selectedNumberButton;
            textBox2.color = greenColor;
        }
        if (num == 3)
        {
            selectionNumberButton = 3;
            button3.sprite = selectedNumberButton;
            textBox3.color = greenColor;
        }
        if (num == 4)
        {
            selectionNumberButton = 4;
            button4.sprite = selectedNumberButton;
            textBox4.color = greenColor;
        }
        if (num == 5)
        {
            selectionNumberButton = 5;
            button5.sprite = selectedNumberButton;
            textBox5.color = greenColor;
        }
        if (num == 6)
        {
            selectionNumberButton = 6;
            button6.sprite = selectedNumberButton;
            textBox6.color = greenColor;
        }
    }

    public void SellScreenExitButton()
    {
        if (!isAnimatingSoldButton)
        {
            MoveSellScreenOut();
        }

    }
    public void SellScreenSellButton()
    {
        int howManyRoomsSale = selectedBuyerCardList[cardNumber].GetRooms();

        int sale = salePrice;

        if (selectedBuyerCardList[cardNumber].GetPlusOneCard())
        {
            playerPhoton.PlusOnePurplePoint();
            playerPhoton.UpdateDashboardUI();
        }
                             
        playerPhoton.AddMoney(sale);
        playerPhoton.AddVictoryPoints(-currentSale.GetVictoryPoints()); //Removing Victory Points
        playerPhoton.RemoveHouseFromList(currentSale);
        playerPhoton.SetExtraVictoryPoints(CalculateExtraVictoryPoints());
        playerPhoton.UpdateDashboardUI();
        UpdateYourInfoLocaly();

        int valueInflux = 700; //How much money is removed from the market when someone sells a home.

        if (howManyRoomsSale == 1)
        {
            b1f -= valueInflux - 200;
        }
        if (howManyRoomsSale == 2)
        {
            b2f -= valueInflux - 100;
        }
        if (howManyRoomsSale == 3)
        {
            b3f -= valueInflux;
        }
        if (howManyRoomsSale == 4)
        {
            b4f -= valueInflux ;
        }
        if (howManyRoomsSale == 5)
        {
            b5f -= valueInflux + 100;
        }
        if (howManyRoomsSale == 6)
        {
            b6f -= valueInflux + 200;
        }
        AnimateSaleTagIn();
    }

    public void JoinedRoomAsMasterSuccessfully()
    {
        Debug.Log("Joined Master Transition Successfully Check");
        totalPlayers = 4;
        StartTheGame();
        UpdateMenuSelection(MenuOptions.Home);
        //openingScreen.transform.DOMove(new Vector3(0, 0, -6.93f), 0.5f).OnComplete(MoveOpeningScreenOut);
    }

    public void JoinedRoomSuccessfully()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Joined Room Transition Successfully Check");
            totalPlayers = 4;
            StartTheGame();
            UpdateMenuSelection(MenuOptions.Home);
            //openingScreen.transform.DOMove(new Vector3(0, 0, -6.93f), 0.5f).OnComplete(MoveOpeningScreenOut);
        }
    }

    //Interaction with the Opening scene panel is done here.
    public void OpeningScreenButton(int selection)
    {
        if (selection == 2)
        {
            playersButton2.sprite = selectedNumberButton;
            totalPlayers = 2; //How many players are playing the game. Set this before launching the game.
        }
        if (selection == 3)
        {
            playersButton3.sprite = selectedNumberButton;
            totalPlayers = 3;
        }
        if (selection == 4)
        {
            playersButton4.sprite = selectedNumberButton;
            totalPlayers = 4; 
        }
        //StartTheGame();
        //openingScreen.transform.DOMove(new Vector3(0, 0, -6.93f), 0.5f).OnComplete(MoveOpeningScreenOut);
    }

    public void ExitExitScreen()
    {
        MoveOpeningScreenOut();
    }
    public void CompletelyExitTheGame()
    {
        exitPanel.transform.DOMove(new Vector3(0, -1130, -6.93f), 0.5f).SetEase(Ease.InCirc).OnComplete(LoadMainMenu);
    }

    public void ExitTheGame()
    {
        choosePlayersPanel.SetActive(false);
        exitPanel.SetActive(true);
        MoveOpeningScreenIn();
    }

    public void InsightScreenButton()
    {
        MoveInsightScreenIn();
    }

    public void ExitInsightScreen()
    {
        MoveInsightScreenOut();
    }

    //The Sell Button on the Game Screen.
    public void MoveSellInitialScreen(int houseTypeSelected)
    {
        //sellScreenInitialScreen.transform.DOMoveY(-1130.0f, 0.25f);
        SellScreenCanvas.gameObject.SetActive(true);
        cardNumber = 0;
        if (houseTypeSelected == 1)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.One;
            numberSelectedTextBox.text = "1";
            ShuffleList(buyerCardList1);
            selectedBuyerCardList = buyerCardList1;
        }
        if (houseTypeSelected == 2)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.Two;
            numberSelectedTextBox.text = "2";
            ShuffleList(buyerCardList2);
            selectedBuyerCardList = buyerCardList2;
        }
        if (houseTypeSelected == 3)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.Three;
            numberSelectedTextBox.text = "3";
            ShuffleList(buyerCardList3);
            selectedBuyerCardList = buyerCardList3;
        }
        if (houseTypeSelected == 4)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.Four;
            numberSelectedTextBox.text = "4";
            ShuffleList(buyerCardList4);
            selectedBuyerCardList = buyerCardList4;
        }
        if (houseTypeSelected == 5)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.Five;
            numberSelectedTextBox.text = "5";
            ShuffleList(buyerCardList5);
            selectedBuyerCardList = buyerCardList5;
        }
        if (houseTypeSelected == 6)
        {
            numberOfRoomsSelected = NumberOfRoomsSelected.Six;
            numberSelectedTextBox.text = "6";
            ShuffleList(buyerCardList6);
            selectedBuyerCardList = buyerCardList6;
        }
    }
    public void Click_IncreaseBid()
    {
        UpdateBidValue(100, true);
    }

    public void Click_DecreaseBid()
    {
        UpdateBidValue(-100, true);
    }

    public void StartNewYearFromBoughtScreen()
    {
        AdjustMarketValueToBarGragh();
        UpdateMenuSelection(MenuOptions.Home);
    }

    public void UpgradeScreenButtons(int button)
    {
        if (button == 1 && playerPhoton.GetPurplePoints() > 0)
        {
            playerPhoton.RemoveOnePurplePoint();
            playerPhoton.UpgradeTotalBids(1);
            playerPhoton.AddOneBid();
            totalBidsTextBox.text = "Bids: " + playerPhoton.GetTotalBids();
        }
        if (button == 2 && playerPhoton.GetPurplePoints() > 0)
        {
            playerPhoton.RemoveOnePurplePoint();
            playerPhoton.UpgradeTotalOffers(1);
            playerPhoton.AddOneOffer();
            totalOffersTextBox.text = "Offer: " + playerPhoton.GetTotalOffer();
        }
        if (button == 3 && playerPhoton.GetLoans() < 10 && playerPhoton.GetTotalVictorypoints() >= 2)
        {
            playerPhoton.AddMoney(1000);
            playerPhoton.IncreaseLoans(1);
            playerPhoton.AddVictoryPoints(-2);
            UpdateYourInfoLocaly();
        }
        if (button == 4 && playerPhoton.GetLoans() > 0 && playerPhoton.GetMoney() >= 1100 && !haveBidOnHouse)
        {
            playerPhoton.AddMoney(-1100);
            playerPhoton.IncreaseLoans(-1);
            playerPhoton.AddVictoryPoints(+2);
            UpdateYourInfoLocaly();
        }
        playerPhoton.UpdateDashboardUI();
    }

    public void SubmitNameButton()
    {
        if (nameInputField.text != "")
        {
            userName = nameInputField.text;
            nameSubmitScreen.SetActive(false);
            createAndJoinScreen.SetActive(true);
        }
        else
        {
            SubmitButton.color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            nameInputField.text = "";
            StartCoroutine(ResetSubmitNameButtonColor());
        }
    }

    IEnumerator ResetSubmitNameButtonColor()
    {
        yield return new WaitForSeconds(1.0f);
        SubmitButton.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }







    private void LoadInHouseData()
    {
        if (allHouseData == null)
        {
            allHouseData = new List<HouseData>();
        }

        allHouseData.Add(new HouseData(1, false, 4, 400, 4, "Davie", "Kerridge Home"));
        allHouseData.Add(new HouseData(2, false, 5, 500, 5, "Downers Grove", "Moshiri Home"));
        allHouseData.Add(new HouseData(3, false, 6, 600, 6, "Weston", "Vance Home"));
        allHouseData.Add(new HouseData(4, true, 5, 500, 5, "Weston", "Costas Residence"));
        allHouseData.Add(new HouseData(5, false, 1, 100, 1, "Lisle", "Bishop Home"));
        allHouseData.Add(new HouseData(6, false, 5, 500, 5, "Lisle", "Fayette Home"));
        allHouseData.Add(new HouseData(7, true, 5, 600, 5, "Davie", "Hyman Residence"));
        allHouseData.Add(new HouseData(8, false, 1, 200, 1, "Lisle", "Moore Home"));
        allHouseData.Add(new HouseData(9, true, 8, 400, 6, "Weston", "Ferry Residence"));
        allHouseData.Add(new HouseData(10, false, 1, 100, 1, "Lisle", "Krichmar Home"));
        allHouseData.Add(new HouseData(11, true, 2, 100, 1, "Lisle", "Johnson Residence"));
        allHouseData.Add(new HouseData(12, true, 5, 500, 5, "Davie", "Lankes Residence"));
        allHouseData.Add(new HouseData(13, false, 4, 400, 4, "Davie", "Vautin Home"));
        allHouseData.Add(new HouseData(14, false, 3, 200, 2, "Downers Grove", "Witmer Home"));
        allHouseData.Add(new HouseData(15, true, 2, 200, 2, "Downers Grove", "Ortega Residence"));
        allHouseData.Add(new HouseData(16, false, 1, 400, 2, "Downers Grove", "Bailey Home"));
        allHouseData.Add(new HouseData(17, true, 6, 400, 5, "Naperville", "Frenk Residence"));
        allHouseData.Add(new HouseData(18, true, 1, 200, 1, "Lisle", "Saperstein Residence"));
        allHouseData.Add(new HouseData(19, false, 1, 100, 1, "Lisle", "Orazi Home"));
        allHouseData.Add(new HouseData(20, true, 1, 0, 1, "Lisle", "Jasper Residence"));
        allHouseData.Add(new HouseData(21, false, 1, 0, 1, "Lisle", "Fidler Home"));
        allHouseData.Add(new HouseData(22, false, 4, 400, 5, "Lisle", "Marruco Home"));
        allHouseData.Add(new HouseData(23, true, 1, 100, 1, "Lisle", "Calgary Residence"));
        allHouseData.Add(new HouseData(24, false, 4, 400, 4, "Davie", "Rosenberger Home"));
        allHouseData.Add(new HouseData(25, false, 6, 600, 6, "Weston", "Takemoto Home"));
        allHouseData.Add(new HouseData(26, true, 3, 400, 3, "Naperville", "Pruitt Residence"));
        allHouseData.Add(new HouseData(27, false, 5, 400, 6, "Weston", "Challenor Home"));
        allHouseData.Add(new HouseData(28, true, 6, 700, 6, "Weston", "Roussos Residence"));
        allHouseData.Add(new HouseData(29, false, 4, 300, 4, "Davie", "Pacholok Home"));
        allHouseData.Add(new HouseData(30, true, 2, 200, 2, "Downers Grove", "Leduc Residence"));
        allHouseData.Add(new HouseData(31, false, 3, 300, 3, "Naperville", "Charles Home"));
        allHouseData.Add(new HouseData(32, true, 3, 400, 3, "Naperville", "Wesson Residence"));
        allHouseData.Add(new HouseData(33, true, 2, 600, 4, "Davie", "Overman Residence"));
        allHouseData.Add(new HouseData(34, false, 2, 200, 2, "Downers Grove", "Lyngley Home"));
        allHouseData.Add(new HouseData(35, true, 4, 200, 3, "Naperville", "Poholsky Residence"));
        allHouseData.Add(new HouseData(36, false, 5, 100, 4, "Davie", "Wengret Home"));
        allHouseData.Add(new HouseData(37, true, 6, 700, 6, "Weston", "Renshaw Residence"));
        allHouseData.Add(new HouseData(38, true, 3, 300, 3, "Naperville", "Bashir Residence"));
        allHouseData.Add(new HouseData(39, true, 5, 500, 4, "Davie", "Kurtzman Residence"));
        allHouseData.Add(new HouseData(40, false, 6, 500, 6, "Weston", "Wrocklage Home"));
        allHouseData.Add(new HouseData(41, false, 6, 600, 6, "Weston", "Kimmel Home"));
        allHouseData.Add(new HouseData(42, true, 6, 200, 4, "Davie", "Lidano Residence"));
        allHouseData.Add(new HouseData(43, false, 2, 200, 3, "Naperville", "Osborne Home"));
        allHouseData.Add(new HouseData(44, false, 2, 200, 2, "Downers Grove", "Coover Home"));
        allHouseData.Add(new HouseData(45, false, 1, 500, 3, "Naperville", "Baumann Home"));
        allHouseData.Add(new HouseData(46, true, 7, 600, 6, "Weston", "Parroco Residence"));
        allHouseData.Add(new HouseData(47, false, 3, 300, 3, "Naperville", "Delaney Home"));
        allHouseData.Add(new HouseData(48, false, 5, 500, 5, "Downers Grove", "Gossard Home"));
        allHouseData.Add(new HouseData(49, true, 2, 300, 2, "Downers Grove", "Ridley Residence"));
        allHouseData.Add(new HouseData(50, false, 6, 500, 5, "Naperville", "Marron Home"));
        allHouseData.Add(new HouseData(51, true, 3, 200, 2, "Downers Grove", "Connor Residence"));
        allHouseData.Add(new HouseData(52, false, 1, 200, 2, "Downers Grove", "Cavett Home"));
        allHouseData.Add(new HouseData(53, false, 3, 300, 3, "Naperville", "Erdos Home"));
        allHouseData.Add(new HouseData(54, true, 4, 400, 4, "Davie", "Mandler Residence"));
    }







}

public enum NumberOfRoomsSelected
{
    None,
    One,
    Two,
    Three,
    Four,
    Five,
    Six
}

public enum MenuOptions
{
    CreateAndJoin,
    Buy,
    Upgrades,
    Home,
    Sell,
    Close,
    BoughtScreen,
    YourAssets,
    TopAgentScreen,
    HouseViewer
}



//TODO 1. Build a view players screen to see their stats