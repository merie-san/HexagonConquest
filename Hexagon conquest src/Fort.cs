using Unity.VisualScripting;
using UnityEngine;

public class Fort : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;

    public Fort()
    {

    }

    public int getMovementCost()
    {
        return 2;
    }
}