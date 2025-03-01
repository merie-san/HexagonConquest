using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneController : MonoBehaviour
{
    [SerializeField] private StatsMenu statsMenu;
    [SerializeField] private Transform tiles;
    [SerializeField] private Castle castleSeed;
    [SerializeField] private Village villageSeed;
    [SerializeField] private Hill hillSeed;
    [SerializeField] private Plane planeSeed;
    [SerializeField] private Forest forestSeed;
    [SerializeField] private Mountain mountainSeed;
    [SerializeField] private Water waterSeed;
    [SerializeField] private Fort fortSeed;
    [SerializeField] private TWall wallSeed;
    [SerializeField] private SpriteRenderer rendererSeed;
    [SerializeField] private Faction playerFaction;
    [SerializeField] private Faction templateEnemyFaction;
    [SerializeField] private Sprite[] colorList;
    [SerializeField] private List<BaseUnit> units;
    [SerializeField] private Wall wallPrefab;
    [SerializeField] private EnemyAI _aiManagement;
    private List<Village> _villagesList = new();
    private List<Castle> _castleList = new();
    private List<Wall> _walls = new();
    private bool _ispaused = false;
    private int _times = 0;
    public List<BaseUnit> Units { get { return units; } }
    public List<Wall> Walls { get { return _walls; } }
    [SerializeField] private BaseUnit[] unitPrefabs = new BaseUnit[12];
    private List<Faction> _enemyFactions = new List<Faction>();
    private BaseUnit _selectedUnit;
    private BaseUnit _selectedAtkUnit;

    public List<Village> VillagesList { get { return _villagesList; } }
    public List<Castle> CastlesList { get { return _castleList; } }

    private void Awake()
    {
        Messenger<Troop, Faction>.AddListener(GameEvents.CREATING_UNIT, OnCreatingUnit);
        Messenger<Wall, bool>.AddListener(GameEvents.REPAIR_WALL, OnRepairWall);
        Messenger<Wall>.AddListener(GameEvents.UPGRADE_WALL, OnUpgradeWall);
        Messenger<bool>.AddListener(GameEvents.GAME_STATUS_CHANGED, OnChangingStatus);
        PrepareScene();
    }

    private void OnChangingStatus(bool isPaused)
    {
        _ispaused = isPaused;
    }

    private void OnRepairWall(Wall wall, bool isRSimple)
    {
        Faction faction = GetFaction(wall.ID);
        if (isRSimple)
        {
            wall.Rebuild(1);
            Messenger<int, Faction>.Broadcast(GameEvents.MONEY_SPENT, 10, faction);
        }
        else
        {
            int maxHP = wall.MaxHP();
            wall.Rebuild(maxHP / 2);
            Messenger<int, Faction>.Broadcast(GameEvents.MONEY_SPENT, maxHP * 4, faction);
        }
    }
    private void OnUpgradeWall(Wall wall)
    {
        Faction faction = GetFaction(wall.ID);
        int maxHP = wall.MaxHP();
        wall.SetMaxHP(maxHP + 5);
        Messenger<int, Faction>.Broadcast(GameEvents.MONEY_SPENT, maxHP * 10, faction);
    }

    private void OnCreatingUnit(Troop unit, Faction faction)
    {
        BaseUnit newUnit = Instantiate(unitPrefabs[(int)unit]);
        bool isPlayer = false;
        if (faction == null)
        {
            faction = playerFaction;
            isPlayer = true;
        }
        newUnit.Color = Instantiate(rendererSeed);
        if (faction.AddUnit(newUnit))
        {
            units.Add(newUnit);
            newUnit.ID = faction.ID;
            newUnit.CurrentTile = faction.Base.GetTile().GetComponent<Tile>();
            newUnit.transform.position = new Vector3(newUnit.CurrentTile.transform.position.x, newUnit.CurrentTile.transform.position.y, -0.3f);
            newUnit.CurrentTile.IsOccupied = true;
            newUnit.CompletedAttack = true;
            newUnit.CompletedMov = true;
            newUnit.TemporaryStats();
            newUnit.AdjustColorPosition();
            int cost = 0;
            switch (unit)
            {
                case Troop.SWD:
                    cost = 50;
                    break;
                case Troop.SPR:
                    cost = 50;
                    break;
                case Troop.ARM:
                    cost = 100;
                    break;
                case Troop.ARC:
                    cost = 80;
                    break;
                case Troop.WIZ:
                    cost = 160;
                    break;
                case Troop.SUP:
                    cost = 150;
                    break;
                case Troop.CVR:
                    cost = 180;
                    break;
                case Troop.SWDCVR:
                    cost = 180;
                    break;
                case Troop.ARMCVR:
                    cost = 280;
                    break;
                case Troop.ARCCVR:
                    cost = 300;
                    break;
                case Troop.CAT:
                    cost = 500;
                    break;
                case Troop.BAL:
                    cost = 500;
                    break;
            }
            _aiManagement.SetStrategy(newUnit, faction, isPlayer);
            Messenger<int, Faction>.Broadcast(GameEvents.MONEY_SPENT, cost, faction);
        }
        else
        {
            Destroy(newUnit.Color);
            Destroy(newUnit.gameObject);
            Debug.Log("Too many units");
        }
    }

    private void OnDestroy()
    {
        Messenger<Troop, Faction>.RemoveListener(GameEvents.CREATING_UNIT, OnCreatingUnit);
        Messenger<Wall, bool>.RemoveListener(GameEvents.REPAIR_WALL, OnRepairWall);
        Messenger<Wall>.RemoveListener(GameEvents.UPGRADE_WALL, OnUpgradeWall);
        Messenger<bool>.RemoveListener(GameEvents.GAME_STATUS_CHANGED, OnChangingStatus);
    }

    private void Update()
    {
        if (_times < 500)
        {
            BindUnitsWithTiles(); _times++;
        }
        //ｨｨ importante chiamare i metodi check siano in update in modo che non si perdano gli eventi keydown
        if (!_ispaused)
        {
            CheckForClicks();
        }
    }

    private void CheckForClicks()
    {
        Camera cam = FindAnyObjectByType<UIController>().Cam;
        if (Input.GetMouseButtonDown(0) && _selectedAtkUnit == null)
        {
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Ray ray = new Ray(point, Vector3.forward);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray);
            if (hit.Length > 0)
            {
                LeftClickMapObject(hit);
            }
        }
        else if (Input.GetMouseButtonDown(1) && _selectedUnit == null)
        {
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Ray ray = new Ray(point, Vector3.forward);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit)
            {
                RightClickMapObject(hit);
            }
        }
        else if ((_selectedUnit != null || _selectedAtkUnit != null) && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            DeselectAllUnit();
        }
    }

    private void RightClickMapObject(RaycastHit2D hit)
    {
        BaseUnit hittedUnit = hit.transform.GetComponent<BaseUnit>();
        CastleTile hittedCastle = hit.transform.GetComponent<CastleTile>();
        if (hittedUnit)
        {
            if (_selectedAtkUnit == null && playerFaction.hasUnit(hittedUnit) && !hittedUnit.CompletedAttack)
            {
                SelectAtkUnit(hittedUnit);
            }
            else if (_selectedAtkUnit != null && hittedUnit.Equals(_selectedAtkUnit) && _selectedAtkUnit.Heal == 0)
            {
                DeselectAtkUnit();
            }
            else if (_selectedAtkUnit != null && !playerFaction.hasUnit(hittedUnit) && _selectedAtkUnit.Heal == 0 && hittedUnit.CurrentTile.IsAttackable)
            {
                CastleTile castleTile = hittedUnit.CurrentTile.GetComponent<CastleTile>();
                if (castleTile != null && castleTile.Wall.ID == hittedUnit.ID && castleTile.Wall.HP() != 0)
                {
                    AttackWall(castleTile);
                }
                else
                {
                    AttackUnit(hittedUnit);
                }
                DeselectAtkUnit();
            }
            else if (_selectedAtkUnit != null && playerFaction.hasUnit(hittedUnit) && _selectedAtkUnit.Heal != 0 && hittedUnit.CurrentTile.IsAttackable)
            {
                HealUnit(hittedUnit);
                DeselectAtkUnit();
            }
            else
            {
                DeselectAtkUnit();
            }
        }
        else
        {
            if (hittedCastle && _selectedAtkUnit != null && _selectedAtkUnit.Heal == 0 && !hittedCastle.Equals(playerFaction.Base.GetTile().GetComponent<CastleTile>()) && hittedCastle.IsAttackable)
            {
                AttackWall(hittedCastle);
            }
            DeselectAtkUnit();
        }
    }

    private void AttackWall(CastleTile hittedCastle)
    {
        if (hittedCastle.Wall.ReceiveDamage(_selectedAtkUnit.AP, _selectedAtkUnit.MAP))
        {
            _selectedAtkUnit.CheckLevelUp(null);
            statsMenu.WriteStats(_selectedAtkUnit, _selectedAtkUnit.Level, _selectedAtkUnit.EXP, _selectedAtkUnit.HP(), _selectedAtkUnit.MaxHP(), _selectedAtkUnit.AP, _selectedAtkUnit.Defense, _selectedAtkUnit.Avoid, _selectedAtkUnit.Hit, _selectedAtkUnit.MAP, _selectedAtkUnit.Heal, _selectedAtkUnit.BonusDef, _selectedAtkUnit.BonusAgility, _selectedAtkUnit.transform.position);
            _selectedAtkUnit.CompletedAttack = true;
        }
    }

    private void HealUnit(BaseUnit hittedUnit)
    {
        if (hittedUnit.ReceiveHeal(_selectedAtkUnit.Heal))
        {
            _selectedAtkUnit.CheckLevelUp(null);
            statsMenu.WriteStats(_selectedAtkUnit, _selectedAtkUnit.Level, _selectedAtkUnit.EXP, _selectedAtkUnit.HP(), _selectedAtkUnit.MaxHP(), _selectedAtkUnit.AP, _selectedAtkUnit.Defense, _selectedAtkUnit.Avoid, _selectedAtkUnit.Hit, _selectedAtkUnit.MAP, _selectedAtkUnit.Heal, _selectedAtkUnit.BonusDef, _selectedAtkUnit.BonusAgility, _selectedAtkUnit.transform.position);
            _selectedAtkUnit.CompletedAttack = true;
        }
    }

    private void AttackUnit(BaseUnit hittedUnit)
    {
        if (hittedUnit.ReceiveDamage(_selectedAtkUnit.AP, _selectedAtkUnit.MAP, _selectedAtkUnit.Hit))
        {
            _selectedAtkUnit.CheckLevelUp(hittedUnit);
            statsMenu.WriteStats(_selectedAtkUnit, _selectedAtkUnit.Level, _selectedAtkUnit.EXP, _selectedAtkUnit.HP(), _selectedAtkUnit.MaxHP(), _selectedAtkUnit.AP, _selectedAtkUnit.Defense, _selectedAtkUnit.Avoid, _selectedAtkUnit.Hit, _selectedAtkUnit.MAP, _selectedAtkUnit.Heal, _selectedAtkUnit.BonusDef, _selectedAtkUnit.BonusAgility, _selectedAtkUnit.transform.position);
            hittedUnit.CurrentTile.IsOccupied = false;
            hittedUnit.Die(GetUnitFaction(hittedUnit));
            units.Remove(hittedUnit);
        }
        _selectedAtkUnit.CompletedAttack = true;
    }

    private void DeselectAtkUnit()
    {
        _selectedAtkUnit = null;
        tiles.GetComponent<Tilemap>().BroadcastMessage("DisableMarks");
    }

    private void DeselectMovUnit()
    {
        _selectedUnit = null;
        tiles.GetComponent<Tilemap>().BroadcastMessage("DisableMarks");
    }

    private void DeselectAllUnit()
    {
        _selectedAtkUnit = null;
        _selectedUnit = null;
        tiles.GetComponent<Tilemap>().BroadcastMessage("DisableMarks");
    }

    private void SelectAtkUnit(BaseUnit hittedUnit)
    {
        _selectedAtkUnit = hittedUnit;
        tiles.GetComponent<Tilemap>().BroadcastMessage("ShowAttackMark",
                new Vector4(_selectedAtkUnit.transform.position.x, _selectedAtkUnit.transform.position.y, _selectedAtkUnit.AttackRange, _selectedAtkUnit.Heal));
    }

    private void LeftClickMapObject(RaycastHit2D[] hit)
    {
        BaseUnit hittedUnit = null;
        Tile hittedTile = null;
        Building hittedBuilding = null;
        for (int i = 0; i < hit.Length; i++)
        {
            BaseUnit tempUnit = hit[i].transform.GetComponent<BaseUnit>();
            Tile tempTile = hit[i].transform.GetComponent<Tile>();
            Building tempBuilding = hit[i].transform.GetComponent<Building>();
            if (tempUnit != null)
            {
                hittedUnit = tempUnit;
                break;
            }
            if (tempTile != null)
            {
                hittedTile = tempTile;
            }
            if (tempBuilding != null)
            {
                hittedBuilding = tempBuilding;
            }
        }
        if (hittedUnit != null)
        {
            statsMenu.WriteStats(hittedUnit, hittedUnit.Level, hittedUnit.EXP, hittedUnit.HP(), hittedUnit.MaxHP(), hittedUnit.AP, hittedUnit.Defense, hittedUnit.Avoid, hittedUnit.Hit, hittedUnit.MAP, hittedUnit.Heal, hittedUnit.BonusDef, hittedUnit.BonusAgility, hittedUnit.transform.position);
            if (playerFaction.hasUnit(hittedUnit) && !hittedUnit.CompletedMov)
            {
                if (_selectedUnit == null)
                {
                    SelectMovUnit(hittedUnit);
                }
                else
                {
                    DeselectMovUnit();
                }
            }
            else if (_selectedUnit != null)
            {
                DeselectMovUnit();
            }
        }
        else if (hittedTile != null)
        {
            MoveUnitInRange(hittedTile, hittedBuilding);
        }
    }

    private void SelectMovUnit(BaseUnit hittedUnit)
    {
        _selectedUnit = hittedUnit;
        tiles.GetComponent<Tilemap>().BroadcastMessage("ShowMarks",
            new Vector4(_selectedUnit.transform.position.x, _selectedUnit.transform.position.y, _selectedUnit.Movement, playerFaction.ID));
    }

    public Faction GetUnitFaction(BaseUnit unit)
    {
        if (playerFaction.hasUnit(unit))
        {
            return playerFaction;
        }
        else
        {
            Faction rightFaction = null;
            foreach (Faction faction in _enemyFactions)
            {
                if (faction.hasUnit(unit))
                {
                    rightFaction = faction;
                    break;
                }
            }
            return rightFaction;
        }
    }

    private void MoveUnitInRange(Tile tile, Building hittedBuilding)
    {
        if (tile.IsMovableTo)
        {
            _selectedUnit.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -0.3f);
            tile.IsOccupied = true;
            _selectedUnit.CurrentTile.IsOccupied = false;
            _selectedUnit.CurrentTile = tile;
            _selectedUnit.TemporaryStats();
            statsMenu.WriteStats(_selectedUnit, _selectedUnit.Level, _selectedUnit.EXP, _selectedUnit.HP(), _selectedUnit.MaxHP(), _selectedUnit.AP, _selectedUnit.Defense, _selectedUnit.Avoid, _selectedUnit.Hit, _selectedUnit.MAP, _selectedUnit.Heal, _selectedUnit.BonusDef, _selectedUnit.BonusAgility, _selectedUnit.transform.position);
            _selectedUnit.AdjustColorPosition();
            _selectedUnit.CompletedMov = true;
            if (hittedBuilding != null)
            {
                playerFaction.BuildingConquered(hittedBuilding);
            }
        }
        DeselectMovUnit();
    }

    private void FixedUpdate()
    {
        CheckGameFinish();

    }

    private void CheckGameFinish()
    {
        if (playerFaction.Base.Owner != playerFaction)
        {
            Lose();
        }
        else
        {
            if (_enemyFactions.Count == 0)
            {
                Win();
            }
        }
    }

    private void Win()
    {
        UIController controller = GetComponent<UIController>();
        controller.Win();
    }

    private void Lose()
    {
        UIController controller = GetComponent<UIController>();
        controller.Lose();
    }

    //questo metodo si occupa di attivare le piastrelle che compongono la mappa, di clonare e posizionare gli edifici e di assegnarli alla faziona corrispondente in base alla loro ID.
    private void PrepareScene()
    {
        int cIndex = 0;
        int firstID = 1;
        int currentId = firstID;
        playerFaction.Base = castleSeed;
        playerFaction.Base.Owner = playerFaction;
        foreach (Transform tile in tiles)
        {
            tile.gameObject.SetActive(true);
            tile.GetComponent<Tile>().DisableMarks();
            CastleTile castleTile = tile.GetComponent<CastleTile>();
            if (castleTile != null)
            {

                if (Mathf.Abs(tile.transform.position.x - castleSeed.transform.position.x) >= 0.05 || Mathf.Abs(tile.transform.position.y - castleSeed.transform.position.y) >= 0.05)
                {
                    cIndex = IstantiateNewFaction(cIndex, castleTile, currentId, out currentId);
                }
                else
                {
                    BuildWall(castleTile, playerFaction);
                    castleSeed.SetTile(tile);
                    castleSeed.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
                    _castleList.Add(castleSeed);
                }
            }
            else if (tile.GetComponent<PlaneTile>() != null)
            {
                InstantiateNewPlane(tile);
            }
            else if (tile.GetComponent<ForestTile>() != null)
            {
                InstantiateNewForest(tile);
            }
            else if (tile.GetComponent<HillTile>() != null)
            {
                InstantiateNewHill(tile);
            }
            else if (tile.GetComponent<MountainTile>() != null)
            {
                InstantiateNewMountain(tile);
            }
            else if (tile.GetComponent<WaterTile>() != null)
            {
                InstantiateNewWater(tile);
            }
            else if (tile.GetComponent<TWallTile>() != null)
            {
                InstantiateNewTWall(tile);
            }
            else if (tile.GetComponent<VillageTile>() != null)
            {
                InstantiateNewVillage(tile);
            }
            else if (tile.GetComponent<FortTile>() != null)
            {
                InstantiateNewFort(tile);
            }

        }
        for (int i = 0; i < playerFaction.LastUnit; i++)
        {
            playerFaction.UnitList[i].Color = Instantiate(rendererSeed);
            playerFaction.UnitList[i].AdjustColorPosition();
        }
        playerFaction.Initialize();
    }

    private void BuildWall(CastleTile castleTile, Faction faction)
    {
        Wall newWall = Instantiate(wallPrefab);
        newWall.Faction = faction;
        castleTile.Wall = newWall;
        newWall.transform.position = castleTile.transform.position;
        _walls.Add(newWall);
    }

    private void InstantiateNewVillage(Transform tile)
    {
        Village newVillage = Instantiate(villageSeed);
        newVillage.SetTile(tile);
        newVillage.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
        _villagesList.Add(newVillage);
    }

    private void InstantiateNewHill(Transform tile)
    {
        Hill newHill = Instantiate(hillSeed);
        newHill.SetTile(tile);
        newHill.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private void InstantiateNewPlane(Transform tile)
    {
        Plane newPlane = Instantiate(planeSeed);
        newPlane.SetTile(tile);
        newPlane.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private void InstantiateNewForest(Transform tile)
    {
        Forest newForest = Instantiate(forestSeed);
        newForest.SetTile(tile);
        newForest.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private void InstantiateNewMountain(Transform tile)
    {
        Mountain newMountain = Instantiate(mountainSeed);
        newMountain.SetTile(tile);
        newMountain.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private void InstantiateNewWater(Transform tile)
    {
        Water newWater = Instantiate(waterSeed);
        newWater.SetTile(tile);
        newWater.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private void InstantiateNewFort(Transform tile)
    {
        Fort newFort = Instantiate(fortSeed);
        newFort.SetTile(tile);
        newFort.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }
    private void InstantiateNewTWall(Transform tile)
    {
        TWall newTWall = Instantiate(wallSeed);
        newTWall.SetTile(tile);
        newTWall.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
    }

    private int IstantiateNewFaction(int colorIndex, CastleTile tile, int factionID, out int nextfactionID)
    {
        Castle newCastle = Instantiate(castleSeed);
        newCastle.id = factionID;
        newCastle.SetTile(tile.transform);
        newCastle.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 0.6f);
        Faction newFaction = Instantiate(templateEnemyFaction);
        newFaction.ID = newCastle.id;
        newFaction.Base = newCastle;
        newCastle.Owner = newFaction;
        if (colorIndex < colorList.Length)
        {
            newFaction.Color = colorList[colorIndex++];
        }
        else
        {
            Debug.Log("Too many factions, insufficient colors");
        }
        foreach (BaseUnit unit in units)
        {
            if (unit.ID == newFaction.ID)
            {
                unit.Color = Instantiate(rendererSeed);
                unit.AdjustColorPosition();
                newFaction.AddUnit(unit);
            }
        }
        newFaction.SetEnemyAI(_aiManagement);
        newFaction.Initialize();
        _enemyFactions.Add(newFaction);
        BuildWall(tile, newFaction);
        _castleList.Add(newCastle);
        nextfactionID = factionID + 1;
        return colorIndex;
    }

    private void BindUnitsWithTiles()
    {
        for (int i = 0; i < units.Count; i++)
        {
            Faction unitFaction = GetFaction(units[i].ID);
            if (unitFaction != null)
            {
                units[i].CastleTarget = unitFaction.Base;
            }
            Ray ray = new Ray(units[i].transform.position, Vector3.forward);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray);
            Tile tile = null;
            Building hitBuilding = null;
            for (int j = 0; j < hit.Length; j++)
            {
                Tile tempTile = hit[j].transform.GetComponent<Tile>();
                Building tempBuilding = hit[j].transform.GetComponent<Building>();
                if (tempTile != null)
                {
                    tile = tempTile;
                }
                if (tempBuilding != null)
                {
                    hitBuilding = tempBuilding;
                }
            }
            if (tile != null)
            {
                Tile occupiedTile = tile;
                if (occupiedTile != null)
                {
                    occupiedTile.IsOccupied = true;
                    units[i].CurrentTile = occupiedTile;
                    units[i].transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, -0.3f);
                    units[i].AdjustColorPosition();
                }
                if (hitBuilding != null)
                {
                    GetFaction(units[i].ID).BuildingConquered(hitBuilding);
                }
            }
        }
    }

    public void EndTurn()
    {
        _selectedAtkUnit = null;
        _selectedUnit = null;
        tiles.GetComponent<Tilemap>().BroadcastMessage("DisableMarks");
        playerFaction.TurnOver();
        int dinamicCount = _enemyFactions.Count;
        int j = 0;
        while (j < dinamicCount)
        {
            Faction currentFaction = _enemyFactions[j];
            currentFaction.MakeDecisions();
            currentFaction.TurnOver();
            dinamicCount = _enemyFactions.Count;
            if (j >= dinamicCount) break;
            else if (currentFaction == _enemyFactions[j])
            {
                j++;
            }
        }
        for (int i = 0; i < units.Count; i++)
        {
            units[i].CompletedMov = false;
            units[i].CompletedAttack = false;
        }
        Messenger<int, int>.Broadcast(GameEvents.TURN_ENDED, playerFaction.Savings, playerFaction.Revenue);
    }

    public int GetPlayerRevenue()
    {
        return playerFaction.Revenue;
    }

    public int GetPlayerSavings()
    {
        return playerFaction.Savings;
    }

    public bool CanPlayerCreateUnits()
    {
        return playerFaction.LastUnit < 20;
    }

    public Wall GetFactionWall(int ID)
    {
        foreach (Wall wall in _walls)
        {
            if (wall.ID == ID)
            {
                return wall;
            }
        }
        return null;
    }

    public Faction GetFaction(int ID)
    {
        if (ID == 0)
        {
            return playerFaction;
        }
        else
        {
            foreach (Faction faction in _enemyFactions)
            {
                if (faction.ID == ID)
                {
                    return faction;
                }
            }
        }
        return null;
    }

    public bool IsBaseOccupied(Faction aFaction)
    {
        if (aFaction == playerFaction || aFaction == null)
        {
            return playerFaction.IsBaseOccupied();
        }
        bool result = true;
        foreach (Faction faction in _enemyFactions)
        {
            if (faction == aFaction)
            {
                result = faction.IsBaseOccupied();
                break;
            }
        }
        return result;
    }

    public void RemoveFaction(Faction faction)
    {
        _enemyFactions.Remove(faction);
    }
}
