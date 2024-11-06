using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
  
  
    public void ActivateInventoryUI()
    {
        GameObject inventoryUIObject = EssentialsManager._instance.uiManager.inventoryUIPanel;
        inventoryUIObject.SetActive(true);
 
    }
}
