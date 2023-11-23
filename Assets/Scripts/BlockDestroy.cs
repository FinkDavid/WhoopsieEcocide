using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDestroy : MonoBehaviour
{
    Animator blockAnimator;
    GameObject oilReference;
    bool startedPanicing = false;

    void Start()
    {
        oilReference = GameObject.FindGameObjectWithTag("Oil");
        blockAnimator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if(Utils.Distance(gameObject.transform.position, oilReference.transform.position) < 5f && startedPanicing == false)
        {
            startedPanicing = true;
            blockAnimator.SetBool("isPanicing", true);
        }
    }

    public void OnBlockDestroyed()
    {
        blockAnimator.SetBool("isDead", true);
        Destroy(gameObject, 1.6f);
    }

    public void DestroyBlockPlayer()
    {
        Destroy(gameObject);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
