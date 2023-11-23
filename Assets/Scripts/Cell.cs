using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private bool hasBlockPlaced = false;
    public GameObject block;

    public bool GetHasBlockPlaced()
    {
        return hasBlockPlaced;
    }

    public void SetBlock(GameObject block)
    {
        hasBlockPlaced = true;
        this.block = block;
        //Debug.Log($"block placed");
    }

    public void RemoveBlock()
    {
        hasBlockPlaced = false;
        if ( block != null )
        {
            //Object.Destroy(block, 0.5f);
        }
    }
}
