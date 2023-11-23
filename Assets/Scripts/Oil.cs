using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField]
    Sprite waterSprite;

    [SerializeField]
    Sprite oilSprite;

    [SerializeField]
    Sprite oilBackgroundSprite;

    [SerializeField] private Cameramovement _scroller;

    bool isOil = false;
    [SerializeField]
    RuntimeAnimatorController oilController;
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameObject.GetComponent<SpriteRenderer>().sprite = waterSprite;
    }

    void Update()
    {
        if(_scroller.IsMoving)
        {
            if(!isOil)
            {
                ChangeToOilSprite();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if (other.tag == "Player") 
        {
            gameManager.OnPlayerDeath(other.GetComponent<PlayerInformation>().playerID);
		}
        else if(other.tag == "Block")
        {
            other.GetComponent<BlockDestroy>().OnBlockDestroyed();
        }
	}

    public void ChangeToOilSprite()
    {
        GameObject.Find("BackgroundImage").GetComponent<SpriteRenderer>().sprite = oilBackgroundSprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = oilSprite;
        gameObject.GetComponent<Animator>().runtimeAnimatorController = oilController;
        GameObject.FindGameObjectWithTag("CloudManager").GetComponent<CloudManager>().ReplaceAllClouds();
        isOil = true;
    }
}
