using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Faction : MonoBehaviour
{
    [SerializeField] private BaseUnit[] unitList = new BaseUnit[20];
    [SerializeField] private int id;
    [SerializeField] private Sprite factionColor;
    [SerializeField] private int savings = 0;
    private EnemyAI enemyAI;
    public int Savings { get => savings; set => savings = value; }
    public BaseUnit[] UnitList { get { return unitList; } }
    public Sprite Color { get => factionColor; set { factionColor = value; } }
    public int ID { get => id; set { id = value; } }
    private List<Building> _buildings = new List<Building>();
    [SerializeField] private Castle castleBase;
    public Castle Base { get => castleBase; set { castleBase = value; } }
    [SerializeField] private int _lastUnit = 0;
    public int LastUnit { get => _lastUnit; }
    private int _revenue = 25;
    public int Revenue => _revenue;

    private bool hasEveryVillage = false;
    public bool HasEveryVillage { get => hasEveryVillage; set { hasEveryVillage = value; } }

    public void Initialize()
    {
        for (int i = 0; i < _lastUnit; i++)
        {
            unitList[i].Color.sprite = factionColor;
        }
        _buildings.Add(castleBase);
        foreach (Building building in _buildings)
        {
            building.AddFactionColor(factionColor);
        }
    }

    public void MakeDecisions()
    {
        Wall walls = castleBase.GetTile().GetComponent<CastleTile>().Wall;
        for (int i = 0; i < _lastUnit; i++)
        {
            enemyAI.AIChoice(unitList[i], this, walls);
        }
        if (savings >= walls.MaxHP() * 50)
        {
            Messenger<Wall>.Broadcast(GameEvents.UPGRADE_WALL, walls);
        }
        else
        if (walls.HP() < walls.MaxHP() / 2 && savings * _revenue >= walls.MaxHP() * 40)
        {
            RebuildWall(walls, walls.MaxHP() * 4, false);
        }
        else if (walls.HP() <= 5)
        {
            RebuildWall(walls, walls.MaxHP() * 4, true);
        }
        if (_lastUnit < 20)
        {
            UnitSpam();
        }
    }

    private void DissolveFaction()
    {
        castleBase.Owner.Savings += savings;
        SceneController controller = FindAnyObjectByType<SceneController>();
        for (int i = 0; i < _lastUnit; i++)
        {
            if (!castleBase.Owner.AddUnit(unitList[i]))
            {
                unitList[i].CurrentTile.IsOccupied = false;
                controller.Units.Remove(unitList[i]);
                unitList[i].Die(this);
            }
            RemoveUnit(unitList[i]);
        }
        for (int i = 0; i < _buildings.Count; i++)
        {
            castleBase.Owner.BuildingConquered(_buildings[i]);
        }

        controller.RemoveFaction(this);
        Destroy(gameObject);
    }

    private bool CheckIfAlive()
    {
        return castleBase.Owner == this;
    }

    private void RebuildWall(Wall wall, int costs, bool isSimple)
    {
        if (!isSimple)
        {
            if (savings >= costs)
            {
                Messenger<Wall, bool>.Broadcast(GameEvents.REPAIR_WALL, wall, false);
            }
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                if (savings >= 10)
                {
                    Messenger<Wall, bool>.Broadcast(GameEvents.REPAIR_WALL, wall, true);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public bool AddUnit(BaseUnit unit)
    {
        if (_lastUnit < 20 && unit != null)
        {
            unit.Color.sprite = factionColor;
            unit.ID = id;
            unitList[_lastUnit++] = unit;
            unit.CastleTarget = castleBase;
            return true;
        }
        return false;
    }

    public void RemoveUnit(BaseUnit unit)
    {
        int ind = _lastUnit;
        int maxUnits = unitList.Length - 1;
        for (int i = 0; i < _lastUnit; i++)
        {
            if (unitList[i] == unit)
            {
                ind = i;
                break;
            }
        }
        ShiftUnits(ind, maxUnits);
    }

    private void ShiftUnits(int ind, int maxUnits)
    {
        for (int i = ind; i < _lastUnit; i++)
        {

            if (i == maxUnits)
            {
                unitList[i] = null;
            }
            else
            {
                unitList[i] = unitList[i + 1];
            }
        }
        unitList[_lastUnit - 1] = null;
        _lastUnit--;
    }

    public void BuildingConquered(Building building)
    {
        SpriteRenderer buildingRenderer = building.GetRenderer();
        if (buildingRenderer != null)
        {
            buildingRenderer.sprite = factionColor;
        }
        else
        {
            building.AddFactionColor(factionColor);
        }
        _buildings.Add(building);
        _revenue += building.GetRevenue();
        Faction previousOwner = building.Owner;
        building.Owner = this;
        if (previousOwner != null)
        {
            previousOwner.BuildingLost(building);
        }
    }

    public void BuildingLost(Building building)
    {
        _revenue -= building.GetRevenue();
        _buildings.Remove(building);
        if (!CheckIfAlive())
        {
            DissolveFaction();
        }
    }

    public bool hasUnit(BaseUnit unit)
    {
        for (int i = 0; i < _lastUnit; i++)
        {
            if (unitList[i] == unit) return true;
        }
        return false;
    }

    internal void TurnOver()
    {
        for (int unit = 0; unit < _lastUnit; unit++)
        {
            if (unitList[unit] == null)
            {
                ShiftUnits(unit, 19);
            }
            else
                unitList[unit].Regeneration();
        }
        savings += _revenue;
    }

    public bool IsBaseOccupied()
    {
        return castleBase.GetTile().GetComponent<Tile>().IsOccupied;
    }

    private void UnitSpam()
    {
        Wall walls = castleBase.GetTile().GetComponent<CastleTile>().Wall;
        if (!castleBase.GetTile().GetComponent<Tile>().IsOccupied)
        {
            if (savings > 500)
            {
                if (walls.HP() == walls.MaxHP())
                {
                    int choice = Random.Range(0, 4);
                    if (choice == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.CAT, this);
                    }
                    else if (choice == 1)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.BAL, this);
                    }
                    else if (choice == 2)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SUP, this);
                    }
                    else if (choice == 3)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARMCVR, this);
                    }
                    else if (choice == 4)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARCCVR, this);
                    }
                }
                else if (walls.HP() >= walls.MaxHP() * 0.75)
                {
                    int choice = Random.Range(0, 9);
                    if (choice == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARC, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.WIZ, this);
                    }
                }
                else
                {
                    int choice = Random.Range(0, 17);
                    if (choice % 3 == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARMCVR, this);
                    }
                    else if (choice % 3 == 1 && choice != 1)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SWDCVR, this);
                    }
                    else if (choice % 3 == 2 && choice != 2)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.CVR, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARM, this);
                    }
                }
            }
            else if (savings > 250)
            {
                if (walls.HP() == walls.MaxHP())
                {
                    bool choice = Random.Range(0, 1) == 0;
                    if (choice)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARM, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.WIZ, this);
                    }
                }
                else if (walls.HP() >= walls.MaxHP() * 0.75)
                {
                    int choice = Random.Range(0, 4);
                    if (choice == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARC, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.WIZ, this);
                    }
                }
                else
                {
                    int choice = Random.Range(0, 14);
                    if (choice % 3 == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARMCVR, this);
                    }
                    else if (choice % 3 == 1 && choice != 1)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SWDCVR, this);
                    }
                    else if (choice % 3 == 2 && choice != 2)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.CVR, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARM, this);
                    }
                }

            }
            else if (savings > 100)
            {
                if (walls.HP() == walls.MaxHP())
                {
                    int choice = Random.Range(0, 2);
                    if (choice == 0)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.ARC, this);
                    }
                    else if (choice == 1)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SWD, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SPR, this);
                    }
                }
                else
                {
                    bool choice = Random.Range(0, 1) == 0;
                    if (choice)
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SWD, this);
                    }
                    else
                    {
                        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SPR, this);
                    }
                }
            }
            else if (savings > 50)
            {
                bool choice = Random.Range(0, 1) == 0;
                if (choice)
                {
                    Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SWD, this);
                }
                else
                {
                    Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, Troop.SPR, this);
                }
            }
        }
    }

    public void SetEnemyAI(EnemyAI enemyAI)
    {
        this.enemyAI = enemyAI;
    }
}
