using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    public int HP();
    public int MaxHP();
    public SpriteRenderer Renderer();
    public Vector3 GetHealthBarPosition();
    public Transform GetTransform();
}
