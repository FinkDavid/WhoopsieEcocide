using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager_Post : MonoBehaviour
{
    [SerializeField] GameObject PlayerPosition_Holder;
    [SerializeField] GameObject Pedestal_Holder;
    [SerializeField] GameObject Placement_Holder;
    [SerializeField] GameObject[] Pedestals = new GameObject[4];
    [SerializeField] GameObject[] Placement_Texts = new GameObject[4];

    private Dictionary<string, Vector3> PedestalLocations = new()
    {
        {"P0",new Vector3(-600,-10,0)},
        {"P1",new Vector3(-200,-90,0)},
        {"P2",new Vector3(200,-90,0)},
        {"P3",new Vector3(600,-90, 0)},
    };


    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        int n = 0;

        GetGameObjects();

        foreach (int Player_placement in gameManager.eliminationList)
        {
            Pedestals[n].SetActive(true);
            Placement_Texts[n].SetActive(true);
            GameObject player = GameObject.FindGameObjectWithTag($"Player" + Player_placement);
            player.transform.position = (PedestalLocations["P" + (n)] + PlayerPosition_Holder.transform.position);
                n++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("UI_Menu_Scene");
        }
        if(Input.GetButtonDown("Press_UI"))
        {
            SceneManager.LoadScene("UI_Menu_Scene");
        }
    }
    void GetGameObjects()
    {
        for (int i = 0; i < Pedestal_Holder.transform.childCount; i++)
        {
            Transform pedestal = Pedestal_Holder.transform.GetChild(i);
            Pedestals[i] = pedestal.gameObject;
        }

        foreach (GameObject pedestal in Pedestals)
        {
            pedestal.SetActive(false);
        }

        for(int i = 0; i < Placement_Holder.transform.childCount;i++)
        {
            Transform Placement = Placement_Holder.transform.GetChild(i);
            Placement_Texts[i] = Placement.gameObject;
        }
        foreach (GameObject text in Placement_Texts)
        {
            text.SetActive(false);
        }
    }
}
