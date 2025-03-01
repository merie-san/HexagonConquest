using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ClassText;
    [SerializeField] private TextMeshProUGUI EXPText;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI AttackText;
    [SerializeField] private TextMeshProUGUI DefenseText;
    [SerializeField] private TextMeshProUGUI AvoidText;
    [SerializeField] private TextMeshProUGUI HitText;
    [SerializeField] private GameObject WatchedUnitMarker;

    public void DefaultStatsMenu() 
    {
        ClassText.text = "";
        EXPText.text = "";
        HPText.text = "";
        AttackText.text = " Select a \n unit";
        DefenseText.text = "";
        AvoidText.text = "";
        HitText.text = "";
        WatchedUnitMarker.gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            WatchedUnitMarker.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            WatchedUnitMarker.gameObject.SetActive(true);
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void WriteStats(BaseUnit unitClass, string unitLevel, int unitEXP, int unitHP, int unitMaxHP, int unitAP, int unitDef, int unitAvoid, int unitHit, int unitMAP, int unitHeal, int unitBonusDef, int unitBonusAgility, Vector3 unitPosition)
    {
        ClassText.text = " " + unitClass.ToString().PartBefore('(') + " (" + unitLevel + ")";
        EXPText.text = " EXP: " + unitEXP + "/100";
        HPText.text = " HP: " + unitHP + "/" + unitMaxHP;
        if (unitMAP > unitAP)
        {
            AttackText.text = " M. Attack:" + unitMAP;
        }
        else if (unitHeal > unitAP)
        {
            AttackText.text = " Heals: " + unitHeal;
        }
        else
        {
            AttackText.text = " Attack: " + unitAP;
        }

        if (unitBonusDef != 0)
        {
            DefenseText.text = " Def: " + (unitDef + unitBonusDef) + " (+" + unitBonusDef + ")";
        }
        else
        {
            DefenseText.text = " Def: " + unitDef;
        }

        if (unitBonusAgility != 0)
        {
            AvoidText.text = " Avo: "  + (unitAvoid + unitBonusAgility) + " (+" + unitBonusAgility + ")";
        }
        else
        {
            AvoidText.text = " Avo: " + unitAvoid;
        }
        HitText.text = " Hit: " + unitHit;

        WatchedUnitMarker.transform.position = new Vector3(unitPosition.x, unitPosition.y - 0.50f, 0);

    }

}
