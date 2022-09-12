using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;

public class UiPanelManager : Singleton<UiPanelManager>
{
    private readonly Dictionary<PanelId, UiPanel> uiPanels = new Dictionary<PanelId, UiPanel>();

    public void AddPanel(PanelId id, UiPanel panel)
    {
        if(uiPanels.ContainsKey(id))
        {
            Debug.LogError($"Panel with id \"{id}\" already exists");
            return;
        }

        if (uiPanels.ContainsValue(panel))
        {
            Debug.LogError($"Panel on gameObject \"{panel.gameObject.name}\" already added");
            return;
        }
        
        uiPanels.Add(id, panel);
    }

    public void RemovePanel(PanelId id)
    {
        if(!CheckPanelExistence(id)) return;
        
        uiPanels.Remove(id);
    }
    
    public void SetPanelState(PanelId id, bool state)
    {
        if(!CheckPanelExistence(id)) return;
        
        uiPanels[id].gameObject.SetActive(state);
    }

    private bool CheckPanelExistence(PanelId id)
    {
        if (!uiPanels.ContainsKey(id))
        {
            Debug.LogError($"Panel with id \"{id}\" is not added in dictionary");
            return false;
        }

        return true;
    }
}
