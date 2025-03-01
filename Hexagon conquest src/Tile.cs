using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer movMark;
    [SerializeField] private SpriteRenderer atkMark;
    [SerializeField] private SpriteRenderer healMark;
    private bool _isOccupied = false;
    public bool IsOccupied { get { return _isOccupied; } set { _isOccupied = value; } }
    private bool _isMovableTo = false;
    private bool _isReachableForAttack = false;
    public bool IsMovableTo { get => _isMovableTo; }
    public bool IsAttackable { get => _isReachableForAttack; }
    protected int temporaryDefense;
    protected int temporaryAgility;
    protected int hpRegenerationTurn;

    public int TileDefense => temporaryDefense;
    public int TileAgility => temporaryAgility;
    public int TileRegeneration => hpRegenerationTurn;

    public void DisableMarks()
    {
        healMark.enabled = false;
        movMark.enabled = false;
        atkMark.enabled = false;
        _isMovableTo = false;
        _isReachableForAttack = false;
    }

    virtual public bool IsAccessible(int ID)
    {
        return true;
    }

    virtual public void ShowMarks(Vector4 data)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(data.x - transform.position.x, 2) + Mathf.Pow(data.y - transform.position.y, 2));
        if (distance < data.z && !_isOccupied)
        {
            bool isReachable = CheckIfReachableFrom(data.z, new Vector3(data.x, data.y, 0), distance);
            if (isReachable && IsAccessible((int)data.w))
            {
                _isMovableTo = true;
                movMark.enabled = true;
            }
        }
    }

    public bool CheckIfReachableFrom(float range, Vector3 position, float distance)
    {
        Vector2 direction = new Vector2((transform.position.x - position.x) / distance, (transform.position.y - position.y) / distance);
        int rangeConvertito = Mathf.RoundToInt(range / (0.434f * 2));
        Dictionary<string, bool> boolDict = WhichEdgeDirection(direction);
        bool isReachable = false;
        Vector3 initialPosition = new Vector3(position.x, position.y, 0.6f);
        if (boolDict["isRight"] || boolDict["isLeft"] || boolDict["isUpRight"] || boolDict["isUpLeft"] || boolDict["isDownLeft"] || boolDict["isDownRight"])
        {
            isReachable = StepCheckReachable(initialPosition, distance, direction, rangeConvertito, 0.75f);
        }
        else if (boolDict["isUp"] || boolDict["isDown"] || boolDict["isPUpRight"] || boolDict["isPUpLeft"] || boolDict["isPDownLeft"] || boolDict["isPDownRight"])
        {
            isReachable = StepCheckReachable(initialPosition, distance, direction, rangeConvertito, 0.866f);
        }
        else
        {
            isReachable = StepCheckReachable(initialPosition, distance, direction, rangeConvertito, 0.8f);
        }
        return isReachable;
    }

    public Dictionary<string, bool> WhichEdgeDirection(Vector2 direction)
    {
        Dictionary<string, bool> results = new()
        {
            { "isRight", Mathf.Abs(direction.x - 1) <= 0.01 && Mathf.Abs(direction.y) <= 0.01 },
            { "isLeft", Mathf.Abs(direction.x + 1) <= 0.01 && Mathf.Abs(direction.y) <= 0.01 },
            { "isUpRight", Mathf.Abs(direction.x - Mathf.Cos(Mathf.PI / 3)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(Mathf.PI / 3)) <= 0.01},
            { "isUpLeft", Mathf.Abs(direction.x - Mathf.Cos(2 * Mathf.PI / 3)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(2 * Mathf.PI / 3)) <= 0.01 },
            { "isDownLeft",  Mathf.Abs(direction.x - Mathf.Cos(4 * Mathf.PI / 3)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(4 * Mathf.PI / 3)) <= 0.01 },
            { "isDownRight", Mathf.Abs(direction.x - Mathf.Cos(5 * Mathf.PI / 3)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(5 * Mathf.PI / 3)) <= 0.01 },
            { "isUp", Mathf.Abs(direction.x) <= 0.01 && Mathf.Abs(direction.y - 1) <= 0.01 },
            { "isDown", Mathf.Abs(direction.x) <= 0.01 && Mathf.Abs(direction.y + 1) <= 0.01 },
            { "isPUpRight", Mathf.Abs(direction.x - Mathf.Cos(Mathf.PI / 6)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(Mathf.PI / 6)) <= 0.01},
            { "isPUpLeft", Mathf.Abs(direction.x - Mathf.Cos(5 * Mathf.PI / 6)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(5 * Mathf.PI / 6)) <= 0.01},
            { "isPDownRight", Mathf.Abs(direction.x - Mathf.Cos(11 * Mathf.PI / 6)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(11 * Mathf.PI / 6)) <= 0.01 },
            { "isPDownLeft", Mathf.Abs(direction.x - Mathf.Cos(7 * Mathf.PI / 6)) <= 0.01 && Mathf.Abs(direction.y - Mathf.Sin(7 * Mathf.PI / 6)) <= 0.01}
        };
        return results;
    }

    private bool StepCheckReachable(Vector3 initialPosition, float distance, Vector2 direction, int rangeConvertito, float stepLenght)
    {
        int movementConsumed2 = 0;
        int movementConsumed1 = 0;
        int pathMovement = 0;
        Collider2D[] overlaps = new Collider2D[2];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetDepth(0.6f, 0.6f);
        int distanceConvertito = Mathf.RoundToInt(distance / stepLenght);
        Vector2[] originPoints = new Vector2[distanceConvertito];
        for (int step = 0; step < distanceConvertito; step++)
        {
            float xCoordinateStep = initialPosition.x + stepLenght * (step + 1) * Mathf.Cos(Mathf.Atan2(direction.y, direction.x));
            float yCoordinateStep = initialPosition.y + stepLenght * (step + 1) * Mathf.Sin(Mathf.Atan2(direction.y, direction.x));
            originPoints[step] = new Vector2(xCoordinateStep, yCoordinateStep);

            int numHits = Physics2D.OverlapCircle(originPoints[step], 0.05f, contactFilter, overlaps);

            if (numHits == 2)
            {
                movementConsumed2 = overlaps[1].GetComponent<DifficultTerrains>().getMovementCost();
                movementConsumed1 = overlaps[0].GetComponent<DifficultTerrains>().getMovementCost();

                if (movementConsumed1 > movementConsumed2)
                {
                    pathMovement += movementConsumed2;
                }
                else
                {
                    pathMovement += movementConsumed1;
                }
            }
            else if (numHits == 1)
            {
                pathMovement += overlaps[0].GetComponent<DifficultTerrains>().getMovementCost();
            }
            if (pathMovement > rangeConvertito)
            {
                return false;
            }
        }
        return true;
    }

    public void ShowAttackMark(Vector4 data)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(data.x - transform.position.x, 2) + Mathf.Pow(data.y - transform.position.y, 2));
        if (data.w == 0 && distance <= data.z && distance > 0.01)
        {
            atkMark.enabled = true;
            _isReachableForAttack = true;
        }
        else if (distance <= data.z && data.w > 0)
        {
            healMark.enabled = true;
            _isReachableForAttack = true;
        }
    }
}
