using UnityEngine;

public class GoalPlatform : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private float resetDelay = 2f;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            Debug.Log($"Finished in: {gameTimer.GetElapsedTime():F2} seconds");
        }

        Invoke(nameof(TriggerReset), resetDelay);
    }

    private void TriggerReset()
    {
        SceneResetter.ResetScene();
    }
}