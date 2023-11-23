using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] Canvas _TitleCanvas;
    [SerializeField] Canvas _JoinCanvas;
    [SerializeField] Canvas _PauseCanvas;
    // Start is called before the first frame update
    void Start()
    {
        if(_TitleCanvas && _JoinCanvas)
        {
            _TitleCanvas.enabled = true;
            _JoinCanvas.enabled = false;
        }
        if(_PauseCanvas)
        {
            _PauseCanvas.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickMenuToJoin()
    {
        _TitleCanvas.enabled = false;
        _JoinCanvas.enabled = true;
    }

    public void onClickSGameToSMenu()
    {
        SceneManager.LoadScene("UI_Menu_Scene");
    }

    public void onClickPauseToIngame()
    {
        _PauseCanvas.enabled = false;
        Time.timeScale = 1;
    }

    public void onClickExit()
    {
        Application.Quit();
    }
}
