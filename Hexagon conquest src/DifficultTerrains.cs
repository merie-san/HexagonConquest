using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DifficultTerrains
{
    Transform GetTile();
    void SetTile(Transform value);
    int getMovementCost();
}
