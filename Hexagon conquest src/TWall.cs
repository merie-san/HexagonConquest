using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWall : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;

    public TWall()
    {

    }

    public int getMovementCost()
    {
        return 10;
    }
}
