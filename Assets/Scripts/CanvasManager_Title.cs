using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    enum direction
    {
        Left,Right, Up, Down
    }

    [SerializeField] Canvas _TitleCanvas;
    [SerializeField] Canvas _JoinCanvas;
    [SerializeField] bool[] _ReadyStatus = { false, false, false, false };
    [SerializeField] GameObject _Selected_Button;
    [SerializeField] Sprite[] _Button_Images;
    [SerializeField] GameObject[] playerBanners;
    [SerializeField] GameObject startText;
    List<InputDevice> list = new List<InputDevice>();
    int joinedPlayers = 0;

    // Start is called before the first frame update
    void Start()
    {
        _Selected_Button.GetComponent<Image>().sprite = SetNextImage(_Selected_Button.GetComponent<Image>().sprite);
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlayMenu();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().roundOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_JoinCanvas.enabled == true)
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is Gamepad gamepad && gamepad.buttonWest.isPressed)
                {
                    InputDevice xPressedDevice = device;

                    if(!list.Contains(xPressedDevice))
                    {
                        list.Add(xPressedDevice);
                        playerBanners[joinedPlayers].transform.GetChild(0).gameObject.SetActive(false);
                        playerBanners[joinedPlayers].transform.GetChild(1).gameObject.SetActive(true);
                        joinedPlayers++;
                    }
                }
                
                if(list.Count > 0)
                {
                    _ReadyStatus[list.Count - 1] = true;
                }

                if(list.Count >= 2)
                {
                    startText.SetActive(true);
                }

                if(list.Count >= 2 && Input.GetButtonDown("Press_UI"))
                {
                    GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().StopMenu();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().inputDevices = list;
                    SceneManager.LoadScene("TestSceneDavid");
                }
            }

            if(_ReadyStatus[0] && _ReadyStatus[1] && _ReadyStatus[2] && _ReadyStatus[3])
            {
                if(Input.GetButtonDown("Press_UI"))
                {
                    GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().StopMenu();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().inputDevices = list;
                    SceneManager.LoadScene("TestSceneDavid");
                }
            }
        }

        if (Input.GetAxis("Vertical1") < 0)
        {
            GetNearestButton(direction.Up);
        }
        if (Input.GetAxis("Vertical1") > 0)
        {
            GetNearestButton(direction.Down);
        }
        if (Input.GetButtonDown("Press_UI"))
        {
            Button buttonToExecute = _Selected_Button.GetComponent<Button>();
            buttonToExecute.onClick.Invoke();
        }

        if(Input.GetButtonDown("Go_Back"))
        {
            if(_JoinCanvas.enabled == true)
            {
                _JoinCanvas.enabled = false;
                _TitleCanvas.enabled = true;

                for(int i = 0; i < _ReadyStatus.Length;i++)
                { 
                    _ReadyStatus[i] = false;    
                    playerBanners[i].transform.GetChild(0).gameObject.SetActive(true);
                    playerBanners[i].transform.GetChild(1).gameObject.SetActive(false);
                }
                list.Clear();
                joinedPlayers = 0;
                startText.SetActive(false);
            }
        }

    }
    private void GetNearestButton(direction direction)
    {
        GameObject NextButton = _Selected_Button;
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag("Button");
        switch (direction)
        {
            case direction.Left:
                foreach(GameObject Button in Buttons)
                {
                    if (NextButton == _Selected_Button)
                    {
                        if (Button.transform.position.x < NextButton.transform.position.x)
                        {
                            NextButton = Button;
                        }
                    }
                    else
                    {
                        if (Button.transform.position.x > NextButton.transform.position.x)
                        {
                            NextButton = Button;
                        }
                    }
                    
                }
                _Selected_Button.GetComponent<Image>().sprite = SetPrevImage(_Selected_Button.GetComponent<Image>().sprite);
                _Selected_Button = NextButton;
                _Selected_Button.GetComponent<Image>().sprite = SetNextImage(_Selected_Button.GetComponent<Image>().sprite);
                break;
            case direction.Right:
                break;
            case direction.Up:
                foreach (GameObject Button in Buttons)
                {
                    if (NextButton == _Selected_Button)
                    {
                        if (Button.transform.position.y > NextButton.transform.position.y)
                        {
                            NextButton = Button;
                        }
                    }
                    else
                    {
                        if (Button.transform.position.y < NextButton.transform.position.y && Button != _Selected_Button)
                        {
                            NextButton = Button;
                        }
                    }
                }
                _Selected_Button.GetComponent<Image>().sprite = SetPrevImage(_Selected_Button.GetComponent<Image>().sprite);
                _Selected_Button = NextButton;
                _Selected_Button.GetComponent<Image>().sprite = SetNextImage(_Selected_Button.GetComponent<Image>().sprite);
                break;
            case direction.Down:
                foreach (GameObject Button in Buttons)
                {
                    if (NextButton == _Selected_Button)
                    {
                        if (Button.transform.position.y < NextButton.transform.position.y)
                        {
                            NextButton = Button;
                        }
                    }
                    else
                    {
                        if (Button.transform.position.y > NextButton.transform.position.y)
                        {
                            NextButton = Button;
                        }
                    }
                }
                _Selected_Button.GetComponent<Image>().sprite = SetPrevImage(_Selected_Button.GetComponent<Image>().sprite);
                _Selected_Button = NextButton;
                _Selected_Button.GetComponent<Image>().sprite = SetNextImage(_Selected_Button.GetComponent<Image>().sprite);
                break;
            default:
                Debug.Log("No valid direction given");
                break;
        }
    }
    Sprite SetNextImage(Sprite Current_sprite)
    {
        for(int i = 0; i < _Button_Images.Length; i++)
        {
            if(Current_sprite == _Button_Images[i])
            {
                return _Button_Images[i + 1];
            }
        }
        return null;
    }
    Sprite SetPrevImage(Sprite Current_sprite)
    {
        for (int i = 0; i < _Button_Images.Length; i++)
        {
            if (Current_sprite == _Button_Images[i])
            {
                return _Button_Images[i - 1];
            }
        }
        return null;
    }
}