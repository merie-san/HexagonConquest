using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Troop { SWD, SPR, ARM, ARC, WIZ, SUP, CVR, SWDCVR, ARMCVR, ARCCVR, CAT, BAL }
public class Popup : MonoBehaviour
{
    [SerializeField] private Button buttonSWD;
    [SerializeField] private Button buttonSPR;
    [SerializeField] private Button buttonARM;
    [SerializeField] private Button buttonARC;
    [SerializeField] private Button buttonWIZ;
    [SerializeField] private Button buttonSUP;
    [SerializeField] private Button buttonCVR;
    [SerializeField] private Button buttonSWDCVR;
    [SerializeField] private Button buttonARMCVR;
    [SerializeField] private Button buttonARCCVR;
    [SerializeField] private Button buttonCAT;
    [SerializeField] private Button buttonBAL;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void SetButtons()
    {
        SceneController controller = FindAnyObjectByType<SceneController>();
        int playerResources = controller.GetPlayerSavings();
        buttonSWD.interactable = false;
        buttonSPR.interactable = false;
        buttonARM.interactable = false;
        buttonARC.interactable = false;
        buttonWIZ.interactable = false;
        buttonSUP.interactable = false;
        buttonCVR.interactable = false;
        buttonSWDCVR.interactable = false;
        buttonARMCVR.interactable = false;
        buttonARCCVR.interactable = false;
        buttonCAT.interactable = false;
        buttonBAL.interactable = false;
        if (!controller.IsBaseOccupied(null) && FindAnyObjectByType<SceneController>().CanPlayerCreateUnits())
        {
            if (playerResources >= 500)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonWIZ.interactable = true;
                buttonSUP.interactable = true;
                buttonCVR.interactable = true;
                buttonSWDCVR.interactable = true;
                buttonARMCVR.interactable = true;
                buttonARCCVR.interactable = true;
                buttonCAT.interactable = true;
                buttonBAL.interactable = true;
            }
            else if (playerResources >= 300)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonSUP.interactable = true;
                buttonWIZ.interactable = true;
                buttonCVR.interactable = true;
                buttonSWDCVR.interactable = true;
                buttonARMCVR.interactable = true;
                buttonARCCVR.interactable = true;
            }
            else if (playerResources >= 280)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonSUP.interactable = true;
                buttonWIZ.interactable = true;
                buttonCVR.interactable = true;
                buttonSWDCVR.interactable = true;
                buttonARMCVR.interactable = true;
            }
            else if (playerResources >= 180)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonSUP.interactable = true;
                buttonWIZ.interactable = true;
                buttonCVR.interactable = true;
                buttonSWDCVR.interactable = true;
            }
            else if (playerResources >= 160)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonSUP.interactable = true;
                buttonWIZ.interactable = true;
            }
            else if (playerResources >= 150)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
                buttonSUP.interactable= true;
            }
            else if (playerResources >= 100)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARM.interactable = true;
                buttonARC.interactable = true;
            }
            else if (playerResources >= 80)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
                buttonARC.interactable = true;
            }
            else if (playerResources >= 50)
            {
                buttonSWD.interactable = true;
                buttonSPR.interactable = true;
            }
        }
    }

    public void OnClickButton1()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, 0, null);
    }

    public void OnClickButton2()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)1, null);
    }

    public void OnClickButton3()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)2, null);
    }

    public void OnClickButton4()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)3, null);
    }

    public void OnClickButton5()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)4, null);
    }

    public void OnClickButton6()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)5, null);
    }

    public void OnClickButton7()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)6, null);
    }

    public void OnClickButton8()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)7, null);
    }

    public void OnClickButton9()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)8, null);
    }

    public void OnClickButton10()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)9, null);
    }

    public void OnClickButton11()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)10, null);
    }

    public void OnClickButton12()
    {
        Messenger<Troop, Faction>.Broadcast(GameEvents.CREATING_UNIT, (Troop)11, null);
    }
}
