using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shape
{
    iShape,
    lShape,
    zShape,
    tShape,
    oShape
}

public enum Material
{
    seal,
    turtle,
    crab,
    fish,
    bird
}

public enum PowerupState
{
    None,
    JumpBoost,
    Pushable,
    Push,
    Swap,
    Stunned,
    Freeze,
    Destroy
}