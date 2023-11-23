using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager_In_Game : MonoBehaviour
{
    enum direction
    {
        Left, Right, Up, Down
    }

    [SerializeField] Canvas Pause_Canvas;
    [SerializeField] Canvas Game_Canvas;
    [SerializeField] Canvas Game_SnE_Canvas;

    [SerializeField] TextMeshProUGUI CountdownText_Move;
    [SerializeField] TextMeshProUGUI CountdownText_Game;
    [SerializeField] TextMeshProUGUI GameEnd_Text;

    [SerializeField] GameObject _Selected_Button;
    [SerializeField] Sprite[] _Button_Images;

    [SerializeField] Image BlackBar;

    GameManager gameManager;

    private float _secondCounter = 0;
    [SerializeField] float _MovementCountdown = 3;
    [SerializeField] float _GamestartCountdown = 10;
    [SerializeField] float _GameEndCountdown = 3;

    public bool GameStart = false;
    public bool MoveStart = false;
    public int alivePlayers = 4;

    // Start is called before the first frame update
    void Start()
    {
        Pause_Canvas.enabled = false;
        Game_Canvas.enabled = true;
        Game_SnE_Canvas.enabled = true;
        CountdownText_Game.enabled = false;
        GameEnd_Text.enabled = false;
        _Selected_Button.GetComponent<Image>().sprite = SetNextImage(_Selected_Button.GetComponent<Image>().sprite);
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        foreach (GameObject player in gameManager.playerReferences)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _secondCounter += Time.deltaTime;

        _MovementCountdown -= (int)math.floor(_secondCounter);

        if (_MovementCountdown == 0)
        {
            MoveStart = true;
            CountdownText_Move.enabled = false;
            CountdownText_Game.enabled = true;
            BlackBar.enabled = false;

            foreach (GameObject player in gameManager.playerReferences)
            {
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
        else
        {
            if(CountdownText_Move.enabled == true)
            {
                CountdownText_Move.text = $"{_MovementCountdown}";
            }
        }
        if(CountdownText_Game.enabled == true) 
        {
            _GamestartCountdown -= (int)_secondCounter;
            CountdownText_Game.text = $"{_GamestartCountdown}";
            if( _GamestartCountdown == 0) 
            {
                GameStart = true;
                CountdownText_Game.enabled = false;
            }
        }

        if(GameEnd_Text.enabled == true)
        {
            _GameEndCountdown -= (int)_secondCounter;
            if(_GameEndCountdown == 0)
            {
                LoadPostgameScene();
            }
        }
        CheckInput();

        if(_secondCounter / 1 >= 1) 
        {

            _secondCounter = 0;
        }
    }

    public void GameEnd()
    {
        MoveStart = false;
        BlackBar.enabled = true;
        GameEnd_Text.enabled = true;
        foreach (GameObject player in gameManager.playerReferences)
        {
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void CheckInput()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !Pause_Canvas.enabled)
        {
            Pause_Canvas.enabled = true;
            Time.timeScale = 0;
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1;
                Pause_Canvas.enabled = false;
            }
            if(Input.GetAxis("Vertical1") < 0)
            {
                GetNearestButton(direction.Up);
            }
            if( Input.GetAxis("Vertical1") > 0)
            {
                GetNearestButton(direction.Down);
            }
            if (Input.GetButtonDown("Press_UI"))
            {
                Button buttonToExecute = _Selected_Button.GetComponent<Button>();
                buttonToExecute.onClick.Invoke();
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
                foreach (GameObject Button in Buttons)
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

    void LoadPostgameScene()
    {
        SceneManager.LoadScene("UI_Post_Game");
    }
    Sprite SetNextImage(Sprite Current_sprite)
    {
        for (int i = 0; i < _Button_Images.Length; i++)
        {
            if (Current_sprite == _Button_Images[i])
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
