using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void Init(string playerName, int playerScore)
    {
        nameText.text = playerName;
        scoreText.text = playerScore.ToString();
    }
}
