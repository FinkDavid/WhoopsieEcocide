using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool roundOver = false;
    public string gameSceneName;
    public string menuSceneName;
    public Vector2[] playerSpawns = new Vector2[4];
    public int playerCount = 2;
    public GameObject[] playerReferences;
    // I used here RuntimeAnimatorController to support AnimatorController and AnimatorOverrideController
    public RuntimeAnimatorController[] playerAnimatorController = new RuntimeAnimatorController[4];
    public List<int> eliminationList;

    public List<InputDevice> inputDevices = new List<InputDevice>();

    //[SerializeField] private SoundManager _soundManager;

    void Awake()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") == this.gameObject)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == gameSceneName) 
		{
            //_soundManager.StopMenu();
            //_soundManager.StartCountdown();

            //For starting in the in game scene immediatly
            foreach (var device in InputSystem.devices)
            {
                if (device is Gamepad gamepad)
                {
                    if(!inputDevices.Contains(device))
                        inputDevices.Add(device);
                }
            }

            playerCount = inputDevices.Count;
            // Debug.Log(inputDevices.Length);
            // foreach (InputDevice device in inputDevices)
            // {
            //     Debug.Log("Device name: " + device.name);
            //     Debug.Log("Device type: " + device.GetType());
            //     // You can access other properties and methods of the InputDevice class here
            // }
            playerReferences = new GameObject[playerCount];
			for(int i = 0; i < playerCount; i++)
            {   
                gameObject.GetComponent<PlayerInputManager>().JoinPlayer(i, -1, null, inputDevices[i]);
            }

            playerReferences = GameObject.FindGameObjectsWithTag("Player");

            for(int i = 0; i < playerCount; i++)
            {
                playerReferences[i].transform.position = playerSpawns[i];
                playerReferences[i].GetComponent<PlayerInformation>().playerID = i;
                playerReferences[i].GetComponent<Animator>().runtimeAnimatorController = playerAnimatorController[i];
            }
		}
        else if (scene.name == menuSceneName)
        {
            //_soundManager.PlayMenu();
        }
    }

    public void OnPlayerDeath(int playerID)
    {
        //_soundManager.PlaySoundEffect(_soundManager.SoundEffects.PlayerDeath, .5f);
        eliminationList.Add(playerID);
        playerReferences[playerID].SetActive(false);

        if(playerCount - eliminationList.Count == 1)
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    void GameOver()
    {
        //_soundManager.StopSoundtrack();
        roundOver = true;  
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        eliminationList.Add(p.GetComponent<PlayerInformation>().playerID);
        GameObject.Find("Scriptholder").GetComponent<CanvasManager_In_Game>().GameEnd();
        //_soundManager.PlayWinning();
    }


}
