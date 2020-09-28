using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ScorePanel : MonoBehaviour
{
    public TextMeshPro name1;
    public TextMeshPro vp;
    public TextMeshPro money;
    public GameObject highlight;
    public GameObject checkBoxReady;

    private GameMaster gameMasterRef;

    public void Awake()
    {
        gameMasterRef = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }

    public void UpdateUI(string theName, int victoryPoints, int theMoney, int id, bool turn)
    {

        money.text = "$" + theMoney;
        vp.text = victoryPoints.ToString();
        name1.text = theName;
        highlight.GetComponent<SpriteRenderer>().color = gameMasterRef.GetColorOfPlayer(gameMasterRef.GetOthersListPosition(id));

        if (turn)
        {
            checkBoxReady.SetActive(true);
            checkBoxReady.GetComponent<SpriteRenderer>().color = gameMasterRef.GetColorOfPlayer(gameMasterRef.GetOthersListPosition(id));
        }
        else
        {
            checkBoxReady.SetActive(false);
            checkBoxReady.GetComponent<SpriteRenderer>().color = gameMasterRef.GetColorOfPlayer(gameMasterRef.GetOthersListPosition(id));
        }
    }

}
