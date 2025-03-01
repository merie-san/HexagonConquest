using Unity.VisualScripting;
using UnityEngine;

public class Hill : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;

    public Hill()
    {

    }

    public int getMovementCost()
    {
        return 2;
    }

}