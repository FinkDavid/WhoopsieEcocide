using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerupManager : MonoBehaviour
{

    System.Random r = new();

    private Cameramovement oil;
    private Cameramovement camera1;
    // Start is called before the first frame update
    void Start()
    {
        oil=GameObject.FindGameObjectWithTag("Oil").GetComponent<Cameramovement>();
        camera1=GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Cameramovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     Stun();
        // }
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     Freeze();
        // }
    }
    public void Stun()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float maxY=-999999;
        int highestPlayer=-1;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].transform.position.y > maxY)
            {
                highestPlayer = i;
                maxY = players[i].transform.position.y;
            }
        }
        //players[highestPlayer].GetComponent<PlayerMovement>().SetStuned();
    }


   

    public void Freeze()
    {
        GameObject.Find("Scroller").GetComponent<Cameramovement>().SetFreeze();
        //oil.SetFreeze();
    }

    public void Swap()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 2)
        {
            (players[0].transform.position, players[1].transform.position) = (players[1].transform.position, players[0].transform.position);
            return;
        }
        // Fisher-Yates shuffle algorith
        
        for (int i = players.Length - 1; i > 0; i--)
        {
            int j = r.Next(i + 1);
            // Swap positions
            (players[i].transform.position, players[j].transform.position) = (players[j].transform.position, players[i].transform.position);
        }
     
    }
   
}