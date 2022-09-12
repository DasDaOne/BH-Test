using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private GameObject playerScorePrefab;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Transform playerScoreParent;
    
    [SerializeField] private TextMeshProUGUI winnerNotificationText;
    
    private string defaultDescription;
    private string defaultWinnerNotification;

    private void Awake()
    {
        defaultDescription = descriptionText.text;
        defaultWinnerNotification = winnerNotificationText.text;
    }

    public void RegisterHandlers()
    {
        NetworkClient.ReplaceHandler<ScoreboardMessage>(UpdateScoreboard);
        NetworkClient.ReplaceHandler<WinnerNotificationMessage>(WinnerNotification);
    }

    private void WinnerNotification(WinnerNotificationMessage message)
    {
        UiPanelManager.Instance.SetPanelState(PanelId.WinnerNotification, true);
        winnerNotificationText.text = defaultWinnerNotification.Replace("{PlayerName}", message.WinnerName);
    }

    private void UpdateScoreboard(ScoreboardMessage message)
    {
        descriptionText.text = defaultDescription.Replace("{WinScore}", message.WinScore.ToString());
        
        List<PlayerScoreData> data = message.PlayerScoreData;
        
        foreach (Transform child in playerScoreParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerScoreData playerScoreData in data)
        {
            PlayerScore playerScore = Instantiate(playerScorePrefab, playerScoreParent).GetComponent<PlayerScore>();
            playerScore.gameObject.name = playerScoreData.PlayerName;
            playerScore.Init(playerScoreData.PlayerName, playerScoreData.Score);
        }
    }
}
