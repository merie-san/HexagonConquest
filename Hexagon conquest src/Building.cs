using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Building
{
    void AddFactionColor(Sprite factionColor);
    SpriteRenderer GetRenderer();
    Transform GetTile();
    void SetTile(Transform value);
    int GetRevenue();
    Faction Owner { get; set; }
}
