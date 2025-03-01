using Unity.VisualScripting;
using UnityEngine;

public class Mountain : MonoBehaviour, DifficultTerrains
{
    private Transform _tile;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;

    public Mountain()
    {

    }

    public SpriteRenderer AddRenderer()
    {
        return this.AddComponent<SpriteRenderer>();
    }

    public SpriteRenderer GetRenderer()
    {
        return this.GetComponent<SpriteRenderer>();
    }

    public int getMovementCost()
    {
        return 3;
    }
}