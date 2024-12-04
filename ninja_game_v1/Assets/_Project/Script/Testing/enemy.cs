using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Resume();
        }
    }
    public void Resume()
    {
        Time.timeScale = 1f; // Set the time scale back to 1 to resume the game
    }
}
