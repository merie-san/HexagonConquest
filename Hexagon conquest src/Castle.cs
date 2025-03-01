using Unity.VisualScripting;
using UnityEngine;

public class Castle : MonoBehaviour, DifficultTerrains, Building
{
    [SerializeField] private int _id = 0;
    private Transform _tile;
    private int _revenue;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;
    public int id { get => _id; set { _id = value; } }
    private Faction _owner;
    public Faction Owner { get => _owner; set => _owner = value; }

    public Castle()
    {
        _revenue = 25;
    }

    public void AddFactionColor(Sprite factionColor)
    {
        SpriteRenderer renderer = this.AddComponent<SpriteRenderer>();
        renderer.sprite = factionColor;
        renderer.sortingOrder = 1;
    }

    public SpriteRenderer GetRenderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public int GetRevenue()
    {
        return _revenue;
    }

    public int getMovementCost()
    {
        return 2;
    }
}
