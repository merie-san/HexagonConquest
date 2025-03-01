using System;
using Unity.VisualScripting;
using UnityEngine;

public class Village : MonoBehaviour, DifficultTerrains, Building
{
    private Transform _tile;
    private int _revenue;
    public Transform GetTile() => _tile;
    public void SetTile(Transform value) => _tile = value;
    private Faction _owner = null;
    public Faction Owner { get => _owner; set => _owner = value; }

    public Village()
    {
        _revenue = 12;
    }
    public void AddFactionColor(Sprite factionColor)
    {
        SpriteRenderer renderer = this.AddComponent<SpriteRenderer>();
        renderer.sprite = factionColor;
        renderer.sortingOrder = 1;
    }

    public SpriteRenderer GetRenderer()
    {
        return this.GetComponent<SpriteRenderer>();
    }

    public int GetRevenue()
    {
        return _revenue;
    }

    public int getMovementCost()
    {
        return 1;
    }
}
