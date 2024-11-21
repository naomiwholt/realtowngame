using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
  
    bool buildmode = false;
    public void ActivateInventoryUI()
    {
        GameObject inventoryUIObject = EssentialsManager._instance.uiManager.inventoryUIPanel;
        inventoryUIObject.SetActive(true);
 
    }

    public void ToggleBuildMode()
    {
        if (buildmode == false)
        {
            EssentialsManager._instance.gridManager.gameObject.SetActive(true);
            print("Build Mode Activated");
            buildmode = true;
        }
        else if (buildmode == true)
        {
            EssentialsManager._instance.gridManager.gameObject.SetActive(false);
            print("Build Mode Deactivated");
            buildmode = false;
        }

    }

}
