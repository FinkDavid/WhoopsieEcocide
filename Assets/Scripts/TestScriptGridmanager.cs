using System;
using UnityEngine;

public class TestScriptGridmanager : MonoBehaviour
{
    private int count = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Tuple<Block,Block> t = DrawBlocks.DrawBlock(count%4);
        Destroy(t.Item1);

        Destroy(t.Item2);
        count++;
    }
}
