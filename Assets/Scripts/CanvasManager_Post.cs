using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager_Post : MonoBehaviour
{
    [SerializeField] GameObject[] Pedestals = new GameObject[4];
    [SerializeField] GameObject[] Placement_Texts = new GameObject[4];
    private Vector3[] pedestalLocations = new Vector3[4] {new Vector3(-600,5,0), new Vector3(-200,-115,0), new Vector3(200,-115,0), new Vector3(600,-115, 0)};

    void Start()
    {
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        int n = gameManager.playerCount - 1;

        GetGameObjects();

        foreach(int Player_placement in gameManager.eliminationList)
        {
            Pedestals[n].SetActive(true);
            Placement_Texts[n].SetActive(true);
            GameObject player = GameObject.FindGameObjectWithTag($"Player" + Player_placement);
            player.transform.localPosition = pedestalLocations[n];
            n--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Press_UI"))
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().eliminationList.Clear();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playerReferences = new GameObject[4];
            SceneManager.LoadScene("UI_Menu_Scene");
        }
    }
    void GetGameObjects()
    {
        foreach (GameObject pedestal in Pedestals)
        {
            pedestal.SetActive(false);
        }

        foreach (GameObject text in Placement_Texts)
        {
            text.SetActive(false);
        }
    }
}
