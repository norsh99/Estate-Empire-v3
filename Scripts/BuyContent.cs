using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyContent : MonoBehaviour
{

    public List<GameObject> listOfPanels;
    public GameObject spawnPanel;


    void Start()
    {
        listOfPanels = new List<GameObject>();
        GameObject newBuyPanel = Instantiate(spawnPanel, transform.position, Quaternion.identity, this.transform);
        listOfPanels.Add(newBuyPanel);
    }


    //Destroy the list of gameobjects when a new round begins
    public void DestroyList()
    {
        for (int i = 0; i < listOfPanels.Count; i++)
        {
            Destroy(listOfPanels[i]);
        }
        listOfPanels = new List<GameObject>();
        GameObject newBuyPanel = Instantiate(spawnPanel, transform.position, Quaternion.identity, this.transform);
        listOfPanels.Add(newBuyPanel);
    }

}
