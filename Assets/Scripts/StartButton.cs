using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum EventButtonType
{
    START,
    EXIT
}
public class StartButton : MonoBehaviour
{
    [SerializeField] EventButtonType eventButtonType;
    void Awake()
    {
        InputSystem.DisableDevice(Mouse.current);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.Difficulty = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) GameManager.Difficulty = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.Difficulty = 3;
    }
    void OnMouseDown()
    {
        switch (eventButtonType)
        {
            case EventButtonType.START:
                PlayerBox[] players = FindObjectsOfType<PlayerBox>();
                if (players.Length == 0) return;
                GameManager.devices = players.OrderBy(x => x.playerID).Select(x => x.playerInput.devices[0]).ToArray();
                SceneManager.LoadScene("Main Game");
                break;
            case EventButtonType.EXIT:
                SceneManager.LoadScene("Control Select");
                break;
        }
    }
}
