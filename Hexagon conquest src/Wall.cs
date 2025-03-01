using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour, Damageable
{
    private int _maxWallHealth = 50;
    public SpriteRenderer Renderer()
    { return GetComponent<SpriteRenderer>(); }
    private int _wallHealth;
    private Faction _ownerFaction;
    private int _ID;
    public int ID { get { return _ID; } }
    public Faction Faction { get { return _ownerFaction; } set { _ownerFaction = value; _ID = value.ID; } }

    public int MaxHP()
    {
        return _maxWallHealth;
    }

    public void SetMaxHP(int value)
    {
        _maxWallHealth = value;
        Messenger<Wall>.Broadcast(GameEvents.WHP_CHANGED, this);
    }

    public int HP()
    {
        return _wallHealth;
    }

    public Wall()
    {
        _wallHealth = _maxWallHealth;
    }

    public bool ReceiveDamage(int APower, int MAPower)
    {
        if (_wallHealth >= 0)
        {
            int damage = APower + Mathf.FloorToInt(MAPower / 2);
            _wallHealth = _wallHealth > damage ? _wallHealth - damage : 0;
            Messenger<Wall, int>.Broadcast(GameEvents.WALL_INTERACTION, this, damage);
            Messenger<Wall>.Broadcast(GameEvents.WHP_CHANGED, this);
            return true;
        }
        else return false;
    }

    public void Rebuild(int amount)
    {
        _wallHealth = _wallHealth + amount < _maxWallHealth ? _wallHealth + amount : _maxWallHealth;
        Messenger<Wall, int>.Broadcast(GameEvents.WALL_INTERACTION, this, -amount);
        Messenger<Wall>.Broadcast(GameEvents.WHP_CHANGED, this);
    }

    public Vector3 GetHealthBarPosition()
    {
        return new Vector3(transform.position.x, transform.position.y + 0.6f, 0);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
