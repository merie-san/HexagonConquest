using Unity.VisualScripting;
using UnityEngine;

public class Plane : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;


    public Plane()
    {

    }

    public int getMovementCost()
    {
        return 1;
    }
}