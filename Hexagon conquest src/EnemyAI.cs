//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;

public class EnemyAI : MonoBehaviour
{
    public void SetStrategy(BaseUnit unit, Faction faction, bool isPlayer)
    {
        if (isPlayer)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.playerCharacter;
        }
        else if (unit.Heal != 0)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.healerBehaviour;
        }
        else if (faction.Revenue < 50)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.conquestVillages;
        }
        else
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.moveTowardsEnemyCastle;
        }
    }

    public void AIChoice(BaseUnit unit, Faction unitFaction, Wall walls)
    {
        if (unit.CurrentAI == BaseUnit.AIBehaviour.healerBehaviour)
        {
            TargetTroopsHealer(unit, unitFaction);
        }
        else if (unit.CurrentAI == BaseUnit.AIBehaviour.stationaryAttack)
        {
            StationaryBehaviour(unit, unitFaction, walls);
        }
        else if (unit.CurrentAI == BaseUnit.AIBehaviour.stationaryReactiveAttack)
        {
            StationaryBehaviour(unit, unitFaction, walls);
        }
        else if (unit.CurrentAI == BaseUnit.AIBehaviour.moveTowardsEnemyCastle)
        {
            MoveTowardsCastle(unit, unitFaction, walls);
        }
        else if (unit.CurrentAI == BaseUnit.AIBehaviour.conquestVillages)
        {
            MoveTowardsVillages(unit, unitFaction, walls);
        }
        else if (unit.CurrentAI == BaseUnit.AIBehaviour.defendFactionCastle)
        {
            RetreatToDefend(unit, unitFaction, walls);
        }
        else
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.stationaryReactiveAttack;
        }
    }

    private void StationaryBehaviour(BaseUnit unit, Faction unitFaction, Wall walls)
    {
        Castle factionCastle = unitFaction.Base;
        float distanceFromBase = Mathf.Sqrt(Mathf.Pow(factionCastle.transform.position.x - unit.transform.position.x, 2) + Mathf.Pow(factionCastle.transform.position.y - unit.transform.position.y, 2));
        bool atkd = DefaultEnemyAttack(unit, unitFaction);
        if (!atkd && unit.CurrentAI == BaseUnit.AIBehaviour.stationaryReactiveAttack)
        {
            Vector3 dest = LookAroundForUnits(unit, unitFaction, true);
            if (dest.z == -0.3f)
            {
                MoveInDirectionOf(unit, unitFaction, dest);
            }
            DefaultEnemyAttack(unit, unitFaction);
        }
        if (walls.HP() <= walls.MaxHP() / 2 && distanceFromBase <= 8.68f) //10 esagoni
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.defendFactionCastle;
        }
    }

    private void MoveTowardsVillages(BaseUnit unit, Faction unitFaction, Wall walls)
    {
        Castle factionCastle = unitFaction.Base;
        float distanceFromBase = Mathf.Sqrt(Mathf.Pow(factionCastle.transform.position.x - unit.transform.position.x, 2) + Mathf.Pow(factionCastle.transform.position.y - unit.transform.position.y, 2));
        bool atkd = DefaultEnemyAttack(unit, unitFaction);
        if (!atkd)
        {
            Vector3 dest = LookAroundForUnits(unit, unitFaction, true);
            if (dest.z != -0.3f)
            {
                List<Village> villagesList = FindAnyObjectByType<SceneController>().VillagesList;
                float villageDistance = 0f;
                float shortestDistance = 9999999f;
                foreach (Village village in villagesList)
                {
                    if (village.Owner != unitFaction)
                    {
                        villageDistance = Mathf.Sqrt(Mathf.Pow(village.transform.position.x - unit.transform.position.x, 2) + Mathf.Pow(village.transform.position.y - unit.transform.position.y, 2));
                        if (villageDistance < shortestDistance)
                        {
                            shortestDistance = villageDistance;
                            dest = new Vector3(village.transform.position.x, village.transform.position.y, -0.3f);
                            unitFaction.HasEveryVillage = false;
                        }
                    }
                }
                if (shortestDistance == 9999999f || unitFaction.Revenue >= 150)
                {
                    unitFaction.HasEveryVillage = true;
                    unit.CurrentAI = BaseUnit.AIBehaviour.moveTowardsEnemyCastle;
                    MakeRandomMove(unit, unitFaction);
                }
                else
                {
                    if (!MoveInDirectionOf(unit, unitFaction, dest))
                    {
                        MakeRandomMove(unit, unitFaction);
                    }
                }
            }
            else
            {
                if (!MoveInDirectionOf(unit, unitFaction, dest))
                {
                    MakeRandomMove(unit, unitFaction);
                }
            }
            DefaultEnemyAttack(unit, unitFaction);
        }
        if (walls.HP() <= walls.MaxHP() / 2 && distanceFromBase <= 8.68f)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.defendFactionCastle;
        }
        else if (unitFaction.Revenue >= 60)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.moveTowardsEnemyCastle;
        }
    }

    private void MoveTowardsCastle(BaseUnit unit, Faction unitFaction, Wall walls)
    {
        Castle factionCastle = unitFaction.Base;
        float distanceFromBase = Mathf.Sqrt(Mathf.Pow(factionCastle.transform.position.x - unit.transform.position.x, 2) + Mathf.Pow(factionCastle.transform.position.y - unit.transform.position.y, 2));
        if (unit.CastleTarget.Owner == unitFaction)
        {
            UpdateTargetedCastle(unit, unitFaction);
        }
        bool cstlatkd = CastleAttack(unit, unitFaction);
        if (!cstlatkd)
        {
            bool atkd = DefaultEnemyAttack(unit, unitFaction);
            if (!atkd)
            {
                Vector3 dest = LookAroundForUnits(unit, unitFaction, true);
                if (dest.z != -0.3f)
                {
                    if (!MoveInDirectionOf(unit, unitFaction, unit.CastleTarget.transform.position))
                    {
                        MakeRandomMove(unit, unitFaction);
                    }
                }
                else
                {
                    if (!MoveInDirectionOf(unit, unitFaction, dest))
                    {
                        MakeRandomMove(unit, unitFaction);
                    }
                }
                cstlatkd = CastleAttack(unit, unitFaction);
                if (!cstlatkd)
                {
                    DefaultEnemyAttack(unit, unitFaction);
                }
            }
        }
        if (walls.HP() <= walls.MaxHP() / 2 && distanceFromBase <= 8.68f)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.defendFactionCastle;
        }
        else if (unitFaction.Revenue <= 30 && unitFaction.HasEveryVillage == false)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.conquestVillages;
        }
    }

    private void UpdateTargetedCastle(BaseUnit unit, Faction unitFaction)
    {
        List<Castle> castlesList = new(FindAnyObjectByType<SceneController>().CastlesList);
        for (int indexList = 0; indexList < castlesList.Count; indexList++)
        {
            if (castlesList[indexList].Owner == unitFaction)
            {
                castlesList.Remove(castlesList[indexList]);
            }
        }
        int castleChoice = Random.Range(0, castlesList.Count - 1);
        unit.CastleTarget = castlesList[castleChoice];
    }

    private bool CastleAttack(BaseUnit unit, Faction unitFaction)
    {
        Vector3 castleCoor = unit.CastleTarget.transform.position;
        float distanceFromTarget = Mathf.Sqrt(Mathf.Pow(unit.transform.position.x - castleCoor.x, 2) + Mathf.Pow(unit.transform.position.y - castleCoor.y, 2));
        if (distanceFromTarget <= unit.AttackRange)
        {
            Ray ray = new Ray(new Vector3(castleCoor.x, castleCoor.y, -0.3f), Vector3.forward);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray);
            CastleTile castleTile = null;
            Building hitBuilding = null;
            BaseUnit hitUnit = null;
            for (int i = 0; i < hit.Length; i++)
            {
                CastleTile tempTile = hit[i].transform.GetComponent<CastleTile>();
                Building tempBuilding = hit[i].transform.GetComponent<Building>();
                BaseUnit tempUnit = hit[i].transform.GetComponent<BaseUnit>();
                if (tempTile != null)
                {
                    castleTile = tempTile;
                }
                if (tempBuilding != null)
                {
                    hitBuilding = tempBuilding;
                }
                if (tempUnit != null)
                {
                    hitUnit = tempUnit;
                }
            }
            if (castleTile != null)
            {
                if (castleTile.Wall.HP() > 0)
                {
                    if (castleTile.Wall.ReceiveDamage(unit.AP, unit.MAP))
                    {
                        unit.CheckLevelUp(null);
                        return true;
                    }
                }
                else if (hitUnit != null)
                {
                    if (hitUnit.ReceiveDamage(unit.AP, unit.MAP, unit.Hit))
                    {
                        unit.CheckLevelUp(hitUnit);
                        Faction hitUnitFaction = FindObjectOfType<SceneController>().GetUnitFaction(hitUnit);
                        List<BaseUnit> units = FindAnyObjectByType<SceneController>().Units;
                        hitUnit.CurrentTile.IsOccupied = false;
                        hitUnit.Die(hitUnitFaction);
                        units.Remove(hitUnit);
                    }
                    return true;
                }
            }

        }
        return false;
    }

    private void RetreatToDefend(BaseUnit unit, Faction unitFaction, Wall walls)
    {
        Castle factionCastle = unitFaction.Base;
        bool atkd = DefaultEnemyAttack(unit, unitFaction);
        Vector3 dest = new Vector3(factionCastle.transform.position.x, factionCastle.transform.position.y, -0.3f);
        if (!MoveInDirectionOf(unit, unitFaction, dest))
        {
            MakeRandomMove(unit, unitFaction);
        }
        if (!atkd)
        {
            DefaultEnemyAttack(unit, unitFaction);
        }
        if (walls.HP() >= walls.MaxHP() / 2 && unitFaction.Revenue <= 30)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.conquestVillages;
        }
        else if (walls.HP() >= walls.MaxHP() / 2 && unitFaction.Revenue >= 60)
        {
            unit.CurrentAI = BaseUnit.AIBehaviour.moveTowardsEnemyCastle;
        }
    }

    private void TargetTroopsHealer(BaseUnit unit, Faction unitFaction)
    {
        bool hldSelf = unit.TryToHealSelf();
        if (!hldSelf)
        {
            bool hldAlly = DefaultEnemyHealing(unit, unitFaction);
            if (!hldAlly)
            {
                Vector3 dest = LookAroundForUnits(unit, unitFaction, false);
                if (dest.z != -0.3f)
                {
                    TryToEscape(unit, unitFaction);
                }
                else
                {
                    MoveInDirectionOf(unit, unitFaction, dest);
                }
            }
            else
            {
                TryToEscape(unit, unitFaction);
            }
        }
        else
        {
            TryToEscape(unit, unitFaction);
        }
    }

    private void TryToEscape(BaseUnit unit, Faction unitFaction)
    {
        Vector3 avoid = LookAroundForUnits(unit, unitFaction, true);
        bool random = Random.Range(0, 4) == 0;
        if (avoid.z != -0.3f || random)
        {
            MakeRandomMove(unit, unitFaction);
        }
        else
        {
            Vector3 dest = new Vector3(3 * unit.transform.position.x - 2 * avoid.x, 3 * unit.transform.position.y - 2 * avoid.y, -0.3f);
            MoveInDirectionOf(unit, unitFaction, dest);
        }
    }

    public void MakeRandomMove(BaseUnit unit, Faction unitFaction)
    {
        bool hasNotMoved = true;
        Tilemap tilemap = FindObjectOfType<Tilemap>();
        int times = 0;
        while (hasNotMoved && times < 20)
        {
            times++;
            int seed;
            int rangeInt = Mathf.RoundToInt(unit.Movement / (0.433f * 2));
            seed = Random.Range(0, 3 * rangeInt * (rangeInt + 1));
            Vector3Int currentCoor = tilemap.WorldToCell(new Vector3(unit.transform.position.x, unit.transform.position.y, 0));
            int chosenOrientation = seed;
            int chosenRange = 6;
            while (chosenOrientation - chosenRange >= 0)
            {
                chosenOrientation -= chosenRange;
                chosenRange += 6;
            }
            chosenRange /= 6;
            Vector3Int destCell;
            if (currentCoor.y % 2 == 0)
            {
                destCell = GetTargetCell(chosenOrientation, chosenRange, currentCoor, true);
            }
            else
            {
                destCell = GetTargetCell(chosenOrientation, chosenRange, currentCoor, false);
            }
            Vector3 worldPoint = tilemap.CellToWorld(destCell);
            Ray ray = new Ray(new Vector3(worldPoint.x, worldPoint.y, -0.2f), Vector3.forward);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray);
            if (hit.Length > 0)
            {
                hasNotMoved = HasNotMovedToDestination(unit, unitFaction, hit);
            }
        }
    }

    private Vector3Int GetTargetCell(int orientation, int range, Vector3Int center, bool evenY)
    {
        int x = center.x + range;
        int y = center.y;
        for (int i = 0; i < orientation; i++)
        {
            switch (i / range)
            {
                case 0:
                    if (evenY)
                    {
                        x--;
                    }
                    y++;
                    evenY = !evenY;
                    break;
                case 1:
                    x--;
                    break;
                case 2:
                    if (evenY)
                    {
                        x--;
                    }
                    y--;
                    evenY = !evenY;
                    break;
                case 3:
                    if (!evenY)
                    {
                        x++;
                    }
                    y--;
                    evenY = !evenY;
                    break;
                case 4:
                    x++;
                    break;
                case 5:
                    if (!evenY)
                    {
                        x++;
                    }
                    y++;
                    evenY = !evenY;
                    break;
            }
        }
        return new Vector3Int(x, y, 0);
    }

    private bool HasNotMovedToDestination(BaseUnit unit, Faction unitFaction, RaycastHit2D[] hit)
    {
        Tile destTile = null;
        Building destBuilding = null;
        for (int i = 0; i < hit.Length; i++)
        {
            Tile tempTile = hit[i].transform.GetComponent<Tile>();
            Building tempBuilding = hit[i].transform.GetComponent<Building>();
            if (tempTile != null)
            {
                destTile = tempTile;
            }
            if (tempBuilding != null)
            {
                destBuilding = tempBuilding;
            }
        }
        if (destTile != null)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(destTile.transform.position.x - unit.transform.position.x, 2) + Mathf.Pow(destTile.transform.position.y - unit.transform.position.y, 2));
            if (destTile.CheckIfReachableFrom(unit.Movement, unit.transform.position, distance) && !destTile.IsOccupied && destTile.IsAccessible(unit.ID))
            {
                unit.transform.position = new Vector3(destTile.transform.position.x, destTile.transform.position.y, -0.3f);
                unit.AdjustColorPosition();
                unit.CurrentTile.IsOccupied = false;
                unit.CurrentTile = destTile;
                unit.TemporaryStats();
                destTile.IsOccupied = true;
                if (destBuilding != null)
                {
                    unitFaction.BuildingConquered(destBuilding);
                }
                return false;
            }
        }
        return true;
    }

    public bool MoveInDirectionOf(BaseUnit unit, Faction unitFaction, Vector3 destination)
    {
        int weight = Random.Range(3, 10);
        float devAngle, devModule;
        float xDest = (weight * destination.x + unit.transform.position.x) / (weight + 1);
        float yDest = (weight * destination.y + unit.transform.position.y) / (weight + 1);
        for (int i = 0; i < 3; i++)
        {
            devAngle = Random.Range(0, 2 * Mathf.PI);
            devModule = Random.Range(0, 0.2f);
            Ray ray = new Ray(new Vector3(xDest + devModule * Mathf.Cos(devAngle), yDest + devModule * Mathf.Sin(devAngle), -0.2f), Vector3.forward);
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray);
            if (HasNotMovedToDestination(unit, unitFaction, hit))
            {
                xDest = (xDest + unit.transform.position.x) / 2;
                yDest = (yDest + unit.transform.position.y) / 2;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    internal bool DefaultEnemyHealing(BaseUnit unit, Faction unitFaction)
    {
        int rangeInt = Mathf.RoundToInt(unit.AttackRange / (0.434f * 2));
        List<BaseUnit> unitsInRange = RaycastAround(unit, 6 * rangeInt, unitFaction, false, unit.AttackRange);
        float maxHLEffectiveness = 0;
        int maxHLEffInd = 0;
        if (unitsInRange.Count == 0)
        {
            return false;
        }
        for (int i = 0; i < unitsInRange.Count; i++)
        {
            float effHL = unitsInRange[i].HP() + unit.Heal > unitsInRange[i].MaxHP() ? unitsInRange[i].MaxHP() - unitsInRange[i].HP() : unit.Heal;
            float HLEff = effHL / unitsInRange[i].HP();
            if (HLEff > maxHLEffectiveness)
            {
                maxHLEffectiveness = HLEff;
                maxHLEffInd = i;
            }
        }
        BaseUnit chosenUnit = unitsInRange[maxHLEffInd];
        unit.CompletedAttack = true;
        if (chosenUnit.ReceiveHeal(unit.Heal))
        {
            unit.CheckLevelUp(null);
            return true;
        }
        return false;
    }

    private List<BaseUnit> RaycastAround(BaseUnit unit, int directions, Faction unitFaction, bool findEnemy, float scanRange)
    {
        float offset = directions / 6 % 2 == 0 ? 0 : Mathf.PI / directions;
        float deltaAngle = Mathf.PI * 2 / directions;
        int startInd = Random.Range(0, directions);
        float curInd = startInd;
        int rotDir = Random.Range(0, 1) == 0 ? -1 : 1;
        List<RaycastHit2D> results = new();
        float curAngle;
        int numHits = 0;
        List<BaseUnit> suitableUnits = new();
        do
        {
            curAngle = curInd * deltaAngle + offset;
            Vector2 direction = new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
            Vector2 startingPoint = new Vector2(unit.transform.position.x + direction.x * 0.434f, unit.transform.position.y + direction.y * 0.434f);
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetDepth(-0.4f, -0.2f);
            numHits = Physics2D.Raycast(startingPoint, direction, filter, results, scanRange - 0.433f);
            if (numHits != 0)
            {
                for (int i = 0; i < numHits; i++)
                {
                    BaseUnit target = results[i].collider.GetComponent<BaseUnit>();
                    if (findEnemy)
                    {
                        if (!unitFaction.hasUnit(target))
                        {
                            suitableUnits.Add(target);
                        }
                    }
                    else
                    {
                        if (unitFaction.hasUnit(target) && !Equals(target))
                        {
                            suitableUnits.Add(target);
                        }
                    }
                }
            }
            curInd += rotDir;
        }
        while ((curInd + directions) % directions != startInd);

        return suitableUnits;
    }

    public Vector3 LookAroundForUnits(BaseUnit unit, Faction unitFaction, bool findEnemy)
    {
        float visibleRange = unit.AttackRange > unit.Movement ? unit.AttackRange : unit.Movement;
        List<BaseUnit> suitableUnits = RaycastAround(unit, 12, unitFaction, findEnemy, visibleRange);
        return suitableUnits.Count > 0 ? suitableUnits[0].transform.position : new Vector3(0, 0, 100);
    }

    public bool DefaultEnemyAttack(BaseUnit unit, Faction unitFaction)
    {
        int rangeInt = Mathf.RoundToInt(unit.AttackRange / (0.434f * 2));
        List<BaseUnit> unitsInRange = RaycastAround(unit, 6 * rangeInt, unitFaction, true, unit.AttackRange);
        if (unitsInRange.Count == 0)
        {
            return false;
        }
        float maxATKEffectiveness = 0;
        int maxATKEffInd = 0;
        for (int i = 0; i < unitsInRange.Count; i++)
        {
            float ATKEff = Mathf.Clamp(unit.AP - unitsInRange[i].DEF, 0, 100) * Mathf.Clamp(unit.Hit - unitsInRange[i].EV, 0, 100) / (unitsInRange[i].HP() * 100f);
            if (ATKEff > maxATKEffectiveness)
            {
                maxATKEffectiveness = ATKEff;
                maxATKEffInd = i;
            }
        }
        BaseUnit target = unitsInRange[maxATKEffInd];
        unit.CompletedAttack = true;
        CastleTile castleTile = target.CurrentTile.GetComponent<CastleTile>();
        if (castleTile != null && castleTile.Wall.ID == target.ID && castleTile.Wall.HP() != 0)
        {
            castleTile.Wall.ReceiveDamage(unit.AP, unit.MAP);
        }
        else
        {
            if (target.ReceiveDamage(unit.AP, unit.MAP, unit.Hit))
            {
                unit.CheckLevelUp(target);
                Faction targetFaction = FindObjectOfType<SceneController>().GetUnitFaction(target);
                List<BaseUnit> units = FindAnyObjectByType<SceneController>().Units;
                target.CurrentTile.IsOccupied = false;
                target.Die(targetFaction);
                units.Remove(target);
            }
        }
        return true;
    }

}
