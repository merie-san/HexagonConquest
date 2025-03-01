using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseUnit : MonoBehaviour, Damageable
{
    /// <STATS>
    private float _movement = 1.736f;
    private float _attackRange = 0.868f;
    public float Movement { get => _movement; set { _movement = value * 0.434f * 2.0f; } }
    public float AttackRange { get => _attackRange; set { _attackRange = value * 0.434f * 2.0f; } }
    protected int _maxHP = 5;
    protected int _defense = 0;
    protected int _agility = 0;
    protected int _precision = 100;
    protected int _health;
    protected int _attack = 0;
    protected int _magicAttack = 0;
    protected int _healingPower = 0;
    private int _EXP = 0;
    private string _level = "Recruit";
    private int tempDef = 0;
    private int tempAgility = 0;
    private int tileRegeneration = 0;
    public int HP()
    {
        return _health;
    }
    public void SetHP(int value)
    {
        _health = value;
    }
    public int DEF { get => _defense; }
    public int EV { get => _agility; }

    public int MaxHP()
    {
        return _maxHP;
    }
    public int AP { get => _attack; }
    public int MAP { get => _magicAttack; }
    public int Hit { get => _precision; }
    public int Heal { get => _healingPower; }
    public int Defense { get => _defense; }
    public int Avoid { get => _agility; }
    public int BonusDef { get => tempDef; }
    public int BonusAgility { get => tempAgility; }
    public int EXP { get => _EXP; }
    public string Level { get => _level; }
    /// </STATS>
    [SerializeField] private AIBehaviour _AIBehaviour;
    public AIBehaviour CurrentAI { get => _AIBehaviour; set => _AIBehaviour = value; }

    private Castle _currentCastleTarget;
    public Castle CastleTarget { get => _currentCastleTarget; set => _currentCastleTarget = value; }

    private SpriteRenderer factionColorRenderer;
    private bool _completedAttack = false;
    public bool CompletedAttack { get => _completedAttack; set => _completedAttack = value; }
    [SerializeField] private int factionID;
    public int ID { get => factionID; set => factionID = value; }
    public SpriteRenderer Color { get => factionColorRenderer; set => factionColorRenderer = value; }
    private Tile _currentTile;
    public Tile CurrentTile { get { return _currentTile; } set { _currentTile = value; } }
    private bool _completedMov = false;
    public bool CompletedMov { get { return _completedMov; } set { _completedMov = value; } }

    public SpriteRenderer spriteRenderer;
    public Sprite[] frames;
    public float refreshTime = 0.8f;
    public int frameIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("NextFrame", refreshTime, refreshTime);
    }

    // Update is called once per frame
    void NextFrame()
    {
        if (spriteRenderer.enabled)
        {
            frameIndex = (frameIndex + 1) % frames.Length;
            spriteRenderer.sprite = frames[frameIndex];
        }
    }
    public enum AIBehaviour
    {
        stationaryAttack,
        stationaryReactiveAttack,
        moveTowardsEnemyCastle,
        conquestVillages,
        defendFactionCastle,
        healerBehaviour,
        playerCharacter
    }

    public void TemporaryStats()
    {
        tempDef = CurrentTile.TileDefense;
        tempAgility = CurrentTile.TileAgility;
    }

    public void Regeneration()
    {
        if (CurrentTile.TileRegeneration != 0)
        {
            tileRegeneration = Mathf.Clamp(_health + CurrentTile.TileRegeneration, 0, _maxHP);
            _health = tileRegeneration;
            Messenger<BaseUnit, bool>.Broadcast(GameEvents.HP_CHANGED, this, false);
            Messenger<BaseUnit, int, bool>.Broadcast(GameEvents.UNIT_INTERACTION, this, -_currentTile.TileRegeneration, false);
        }
    }

    public bool ReceiveDamage(int rAP, int rMAP, int Hit)
    {
        int damage = 0;
        bool missed = true;
        if (Random.Range(1, 100) <= (Hit - _agility - tempAgility))
        {
            int physicalDmg = rAP - _defense - tempDef;
            damage = Mathf.Clamp(physicalDmg, 0, 100) + rMAP;
            missed = false;
        }
        _health = _health - damage;
        bool result = _health <= 0;
        Messenger<BaseUnit, bool>.Broadcast(GameEvents.HP_CHANGED, this, result);
        Messenger<BaseUnit, int, bool>.Broadcast(GameEvents.UNIT_INTERACTION, this, damage, missed);
        return result;
    }

    public void CheckLevelUp(BaseUnit typeOfUnit)
    {
        if (typeOfUnit != null)
        {
            switch (typeOfUnit)
            {
                case Swordman:
                    _EXP += 18;
                    break;
                case Spearman:
                    _EXP += 18;
                    break;
                case HeavyInfantry:
                    _EXP += 30;
                    break;
                case Archer:
                    _EXP += 25;
                    break;
                case Wizard:
                    _EXP += 45;
                    break;
                case Support:
                    _EXP += 43;
                    break;
                case SpearCavalry:
                    _EXP += 50;
                    break;
                case SwordCavalry:
                    _EXP += 50;
                    break;
                case HeavyCavarly:
                    _EXP += 75;
                    break;
                case HorseArcher:
                    _EXP += 80;
                    break;
                case Catapult:
                    _EXP += 130;
                    break;
                case Ballista:
                    _EXP += 130;
                    break;
            }
        }
        else
        {
            if (_healingPower > 0 || _precision <= 70)
            {
                _EXP += 10;
            }

        }
        if (_EXP >= 100)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        _EXP = _EXP % 100;
        if (_level == "Recruit")
        {
            _level = "Veteran";
            _maxHP += 1;
            _health += 1;
            if (_magicAttack == 0 && _healingPower == 0)
            {
                _attack += 1;
            }
            _precision += 5;
            _agility += 5;
            if (_healingPower > 0)
            {
                _healingPower++;
            }
        }
        else if (_level == "Veteran")
        {
            _level = "Hero";
            _maxHP += 2;
            _health += 2;
            if (_magicAttack == 0 && _healingPower == 0)
            {
                _attack += 1;
            }
            _precision += 5;
            _agility += 5;
            _defense += 1;
            _movement += 1;
            if (_magicAttack > 0)
            {
                _magicAttack++;
            }
        }
    }

    public bool ReceiveHeal(int rHeal)
    {
        bool result = false;
        if (_health < _maxHP)
        {
            int healedHP = rHeal + _health;
            SetHP(Mathf.Clamp(healedHP, 0, _maxHP));
            result = true;
            Messenger<BaseUnit, int, bool>.Broadcast(GameEvents.UNIT_INTERACTION, this, -rHeal, false);
        }
        Messenger<BaseUnit, bool>.Broadcast(GameEvents.HP_CHANGED, this, false);
        return result;
    }

    internal bool TryToHealSelf()
    {
        return ReceiveHeal(_healingPower);
    }

    public SpriteRenderer Renderer()
    {
        return GetComponent<SpriteRenderer>();
    }

    public Vector3 GetHealthBarPosition()
    {
        return new Vector3(transform.position.x, transform.position.y - 0.6f, 0);
    }

    internal void Die(Faction faction)
    {
        Destroy(factionColorRenderer);
        faction.RemoveUnit(this);
        Destroy(gameObject);
    }

    public void AdjustColorPosition()
    {
        factionColorRenderer.transform.position = transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
