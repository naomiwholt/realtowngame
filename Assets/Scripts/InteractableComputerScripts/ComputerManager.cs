using UnityEngine;

public class ComputerManager : MonoBehaviour
{
    bool ComputerPowerOn;


    public void OnOffSwitch()
    {
        if (ComputerPowerOn) ComputerPowerOn = false;
        else ComputerPowerOn = true; 
    }

    public void LaunchGame()
    {
        Debug.Log("Launching Game");

    }

    

    public void OpenEmailInbox()
    { 
    
    }

    


}
