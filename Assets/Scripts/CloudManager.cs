using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField]
    Sprite darkCloudsSprite;
    public void ReplaceAllClouds()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = darkCloudsSprite;
        }
    }
}
