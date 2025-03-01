using Unity.VisualScripting;
using UnityEngine;

public class Water : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;

    public Water()
    {

    }

    public int getMovementCost()
    {
        return 10;
    }
}
