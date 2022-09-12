using UnityEngine;

public enum PanelId
{
    Network,
    Connection,
    Status,
    Scoreboard,
    WinnerNotification
}

public class UiPanel: MonoBehaviour
{
    [SerializeField] private bool activeOnStart;
    [SerializeField] private PanelId id;
    
    private void Awake()
    {
        UiPanelManager.Instance.AddPanel(id, this);

        if (!activeOnStart)
            UiPanelManager.Instance.SetPanelState(id, false);
    }

    private void OnDestroy()
    {
        if(UiPanelManager.IsInstanceExist())
            UiPanelManager.Instance.RemovePanel(id);
    }
}
