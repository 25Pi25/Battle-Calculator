using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartButton : MonoBehaviour
{
    void Awake()
    {
        InputSystem.DisableDevice(Mouse.current);
    }
    void OnMouseDown()
    {
        PlayerBox[] players = FindObjectsOfType<PlayerBox>();
        if (players.Length == 0) return;
        GameManager.devices = players.OrderBy(x => x.playerID).Select(x => x.playerInput.devices[0]).ToArray();
        SceneManager.LoadScene("Main Game");
    }
}
