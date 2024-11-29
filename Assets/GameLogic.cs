using UnityEngine;

public class GameLogic : MonoBehaviour
{
    void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example: Spawn balloons on Space key
        {
            NetworkServerProcessing.SpawnBalloon();
        }
    }
}