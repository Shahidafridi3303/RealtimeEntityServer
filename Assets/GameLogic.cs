using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public float spawnInterval = 2f; // Time in seconds between spawns
    private float spawnTimer;       // Tracks time for spawning balloons

    void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
        spawnTimer = spawnInterval; // Initialize the timer
    }

    void Update()
    {
        // Decrease the timer by the time passed since the last frame
        spawnTimer -= Time.deltaTime;

        // If the timer reaches zero, spawn a new balloon
        if (spawnTimer <= 0f)
        {
            NetworkServerProcessing.SpawnBalloon();
            spawnTimer = spawnInterval; // Reset the timer
        }
    }
}