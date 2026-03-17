using UnityEngine;

public class LobbyDetails : MonoBehaviour
{
    private int difficultyIndex;
    private int durationSeconds;

    public void SetRaidDetails(int difficultyIndex, int durationSeconds)
    {
        this.difficultyIndex = difficultyIndex;
        this.durationSeconds = durationSeconds;
    }

    public (int difficultyIndex, int durationSeconds) GetRaidDetails()
    {
        return (difficultyIndex, durationSeconds);
    }
}
