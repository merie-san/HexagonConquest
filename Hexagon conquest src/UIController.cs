using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

public class UIController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public Camera Cam { get => cam; }
    [SerializeField] private float camSpeed = 0.05f;
    private Vector2 _camDirection = Vector2.zero;
    [SerializeField] private Zoom camZoom = Zoom.Zoom0;
    private enum Zoom { Zoom0, Zoom1, Zoom2, Zoom3 };
    [SerializeField] private TMP_Text turn;
    [SerializeField] private TMP_Text earnings;
    [SerializeField] private TMP_Text savings;
    [SerializeField] private Popup popupWindow;
    [SerializeField] private MainMenuPopup popupMenu;
    [SerializeField] private StatsMenu statsMenu;
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private HealthBar healthBarPrefabWalls;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform bottomRight;
    [SerializeField] private TextPopup textPopupPrefab;
    [SerializeField] private Button simpleRepairButton;
    [SerializeField] private Button halfRepairButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button buyUnitsButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button statsToggleButton;
    private Dictionary<Damageable, HealthBar> _activeHealthBars = new Dictionary<Damageable, HealthBar>();
    private int _turnNumber = 0;

    private void Start()
    {
        popupWindow.Close();
        popupMenu.Close();
        statsMenu.Close();
        statsMenu.DefaultStatsMenu();
    }

    private void Awake()
    {
        Messenger<int, int>.AddListener(GameEvents.TURN_ENDED, OnEndTurn);
        Messenger<int, Faction>.AddListener(GameEvents.MONEY_SPENT, OnMoneySpent);
        Messenger<BaseUnit, bool>.AddListener(GameEvents.HP_CHANGED, UpdateHealthBars);
        Messenger<Wall>.AddListener(GameEvents.WHP_CHANGED, UpdateHealthBars);
        Messenger<BaseUnit, int, bool>.AddListener(GameEvents.UNIT_INTERACTION, ShowTextPopup);
        Messenger<Wall, int>.AddListener(GameEvents.WALL_INTERACTION, ShowTextPopup);
        Messenger<bool>.AddListener(GameEvents.GAME_STATUS_CHANGED, OnChangingStatus);
    }

    private void OnUnitDeath(BaseUnit arg1)
    {
        _activeHealthBars.Remove(arg1);
    }

    public void Win()
    {
        int completedLevels = LevelController.completedLevels;
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
        if (completedLevels < currentLevel)
        { LevelController.completedLevels = currentLevel; }
        popupMenu.Open();
        popupMenu.SetButtonsWinLev();
    }

    public void Lose()
    {
        popupMenu.Open();
        popupMenu.SetButtonsLoseLev();
    }

    private void OnChangingStatus(bool paused)
    {
        if (paused)
        {
            popupWindow.Close();
            statsMenu.Close();
            simpleRepairButton.interactable = false;
            halfRepairButton.interactable = false;
            upgradeButton.interactable = false;
            buyUnitsButton.interactable = false;
            endTurnButton.interactable = false;
            statsToggleButton.interactable = false;
        }
        else
        {
            SetWallButtons();
            buyUnitsButton.interactable = true;
            endTurnButton.interactable = true;
            statsToggleButton.interactable = true;
        }
    }

    private void UpdateHealthBars(BaseUnit unit, bool died)
    {
        HealthBar healthbar;
        if (_activeHealthBars.TryGetValue(unit, out healthbar))
        {
            if (died)
            {
                _activeHealthBars.Remove(unit);
                Destroy(healthbar.gameObject);
            }
            else
            {
                healthbar.ChangeFill(unit.HP());
            }
        }
    }

    private void OnMoneySpent(int cost, Faction faction)
    {
        faction.Savings -= cost;
        if (faction.ID == 0)
        {
            savings.text = "" + faction.Savings;
        }
        popupWindow.SetButtons();
        SetWallButtons();
    }

    private void ShowTextPopup(Wall wall, int damage)
    {
        TextPopup newPopup = Instantiate(textPopupPrefab, canvas.transform);
        newPopup.transform.SetSiblingIndex(0);
        newPopup.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y + 0.85f, 0);
        SetText(damage, newPopup, false);
    }

    private void OnDestroy()
    {
        Messenger<int, int>.RemoveListener(GameEvents.TURN_ENDED, OnEndTurn);
        Messenger<int, Faction>.RemoveListener(GameEvents.MONEY_SPENT, OnMoneySpent);
        Messenger<BaseUnit, bool>.RemoveListener(GameEvents.HP_CHANGED, UpdateHealthBars);
        Messenger<Wall>.RemoveListener(GameEvents.WHP_CHANGED, UpdateHealthBars);
        Messenger<BaseUnit, int, bool>.RemoveListener(GameEvents.UNIT_INTERACTION, ShowTextPopup);
        Messenger<Wall, int>.RemoveListener(GameEvents.WALL_INTERACTION, ShowTextPopup);
        Messenger<bool>.RemoveListener(GameEvents.GAME_STATUS_CHANGED, OnChangingStatus);
    }

    private void ShowTextPopup(BaseUnit unit, int damage, bool missed)
    {
        TextPopup newPopup = Instantiate(textPopupPrefab, canvas.transform);
        newPopup.transform.SetSiblingIndex(0);
        newPopup.transform.position = new Vector3(unit.transform.position.x, unit.transform.position.y + 0.70f, 0);
        SetText(damage, newPopup, missed);
    }

    private void SetText(int damage, TextPopup newPopup, bool missed)
    {
        newPopup.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Pow(1.2f, 3 - (int)camZoom);
        if (missed)
        {
            newPopup.Text = "Missed";
        }
        else if (damage >= 0)
        {
            newPopup.Text = "- " + damage + " HP";
        }
        else if (damage < 0)
        {
            int healAmount = -damage;
            newPopup.Text = "+ " + healAmount + " HP";
            newPopup.SetColor(false);
        }

        newPopup.FadeOut();
    }

    private void UpdateHealthBars(Wall wall)
    {
        HealthBar healthbar;
        if (_activeHealthBars.TryGetValue(wall, out healthbar))
        {
            healthbar.MaxHealth = wall.MaxHP();
            healthbar.ChangeFill(wall.HP());
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckCamMovBegan();
        CheckCamMovStopped();
        KeyboardZoom();
    }

    private void FixedUpdate()
    {
        MoveCamera();
        CheckHPBars();
    }

    private void CheckHPBars()
    {
        List<BaseUnit> units = GetComponent<SceneController>().Units;
        List<Wall> walls = GetComponent<SceneController>().Walls;
        int j = 0;
        int dinamicCount = units.Count;
        foreach (KeyValuePair<Damageable, HealthBar> entry in _activeHealthBars)
        {
            entry.Value.MaxHealth = entry.Key.MaxHP();
            entry.Value.ChangeFill(entry.Key.HP());
        }
        while (j < dinamicCount)
        {
            BaseUnit currentUnit = units[j];
            if (currentUnit != null)
            {
                CheckHpBar(units[j], healthBarPrefab);
            }
            if (currentUnit == units[j])
            {
                j++;
            }
            dinamicCount = units.Count;
        }
        foreach (Damageable wall in walls)
        {
            CheckHpBar(wall, healthBarPrefabWalls);
        }
        foreach (KeyValuePair<Damageable, HealthBar> element in _activeHealthBars)
        {
            element.Value.GetComponent<RectTransform>().transform.position = element.Key.GetHealthBarPosition();
        }
    }

    private void CheckHpBar(Damageable obj, HealthBar prefab)
    {
        HealthBar newHeathBar, healthBar;
        if (obj.Renderer().isVisible)
        {
            if (!_activeHealthBars.ContainsKey(obj))
            {
                newHeathBar = Instantiate(prefab, canvas.transform);
                newHeathBar.transform.SetSiblingIndex(0);
                newHeathBar.MaxHealth = obj.MaxHP();
                newHeathBar.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Pow(1.2f, 3 - (int)camZoom) * 2, Mathf.Pow(1.2f, 3 - (int)camZoom) * 2, 1);
                newHeathBar.Initialize(obj.HP(), obj.HP());
                _activeHealthBars.Add(obj, newHeathBar);
            }
        }
        else
        {
            if (_activeHealthBars.ContainsKey(obj))
            {
                _activeHealthBars.TryGetValue(obj, out healthBar);
                Destroy(healthBar.gameObject);
                _activeHealthBars.Remove(obj);
            }
        }
    }

    public void OnEndTurn(int playerSavings, int playerRevenue)
    {
        _turnNumber++;
        turn.text = "" + _turnNumber;
        earnings.text = "" + playerRevenue;
        savings.text = "" + playerSavings;
        SetWallButtons();
    }

    public void UpdateTexts()
    {
        SceneController controller = GetComponent<SceneController>();
        earnings.text = "" + controller.GetPlayerRevenue();
        savings.text = "" + controller.GetPlayerSavings();
    }

    private void CheckCamMovStopped()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            _camDirection.y = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            _camDirection.x = 0;
        }
    }
    private void CheckCamMovBegan()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _camDirection.y = 0;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _camDirection.y = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _camDirection.y = -1;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _camDirection.x = 0;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _camDirection.x = 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _camDirection.x = -1;
        }
    }

    private void MoveCamera()
    {
        float newXValue = _camDirection.x * camSpeed + cam.transform.position.x;
        float newYValue = _camDirection.y * camSpeed + cam.transform.position.y;
        float offsetX = cam.pixelWidth * ((int)camZoom + 1) / 800;
        float offsetY = cam.pixelHeight * ((int)camZoom + 1) / 800;
        if (newXValue - offsetX > topLeft.position.x && newXValue + offsetX < bottomRight.position.x && newYValue + offsetY < topLeft.position.y && newYValue - offsetY > bottomRight.position.y)
        {
            cam.transform.position = new Vector3(newXValue, newYValue, -100);
        }
        if (newXValue - offsetX <= topLeft.position.x && newXValue > cam.transform.position.x)
        {
            cam.transform.position = new Vector3(newXValue, cam.transform.position.y, -100);
        }
        if (newXValue + offsetX >= bottomRight.position.x && newXValue < cam.transform.position.x)
        {
            cam.transform.position = new Vector3(newXValue, cam.transform.position.y, -100);
        }
        if (newYValue + offsetY >= topLeft.position.y && newYValue < cam.transform.position.y)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, newYValue, -100);
        }
        if (newYValue - offsetY <= bottomRight.position.y && newYValue > cam.transform.position.y)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, newYValue, -100);
        }
    }

    private void KeyboardZoom()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SwitchZoom(Zoom.Zoom0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SwitchZoom(Zoom.Zoom1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SwitchZoom(Zoom.Zoom2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SwitchZoom(Zoom.Zoom3);
        }
    }

    private void SwitchZoom(Zoom nextZoom)
    {
        float zoomN = (float)nextZoom + 1.0f;
        cam.orthographicSize = zoomN;
        camZoom = nextZoom;
        foreach (KeyValuePair<Damageable, HealthBar> element in _activeHealthBars)
        {
            element.Value.GetComponent<RectTransform>().localScale = Vector3.one * Mathf.Pow(1.2f, 3 - (int)nextZoom) * 2;
        }
    }

    public void OnClickSRepair()
    {
        SceneController controller = GetComponent<SceneController>();
        Wall Wall = controller.GetFactionWall(0);
        Messenger<Wall, bool>.Broadcast(GameEvents.REPAIR_WALL, Wall, true);
    }

    public void OnClickHRepair()
    {
        SceneController controller = GetComponent<SceneController>();
        Wall Wall = controller.GetFactionWall(0);
        Messenger<Wall, bool>.Broadcast(GameEvents.REPAIR_WALL, Wall, false);
    }

    public void OnClickUpgrade()
    {
        SceneController controller = GetComponent<SceneController>();
        Wall Wall = controller.GetFactionWall(0);
        Messenger<Wall>.Broadcast(GameEvents.UPGRADE_WALL, Wall);
    }

    public void SetWallButtons()
    {
        SceneController controller = GetComponent<SceneController>();
        Wall Wall = controller.GetFactionWall(0);
        int MaxHP = Wall.MaxHP();
        int HP = Wall.HP();
        int playerResources = controller.GetPlayerSavings();
        simpleRepairButton.interactable = false;
        halfRepairButton.interactable = false;
        upgradeButton.interactable = false;
        if (HP < MaxHP)
        {
            if (playerResources >= 10)
            {
                simpleRepairButton.interactable = true;
            }
            if (playerResources >= MaxHP * 4)
            {
                halfRepairButton.interactable = true;
            }
        }
        if (playerResources >= MaxHP * 10)
        {
            upgradeButton.interactable = true;
        }

    }
}
