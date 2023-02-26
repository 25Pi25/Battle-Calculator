using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerBox : MonoBehaviour
{
    [SerializeField] Sprite keyboard;
    [SerializeField] Sprite gamepad;
    public PlayerInput playerInput { get; private set; }
    int playerID;
    void Awake()
    {
        transform.SetParent(GameObject.Find("Player Boxes").transform);
        playerInput = GetComponent<PlayerInput>();
        playerID = FindObjectsOfType<PlayerBox>().Length;

        TextMeshPro playerNumber = transform.Find("Player Number").gameObject.GetComponent<TextMeshPro>();
        playerNumber.text = "Player " + playerID;
        playerNumber.color = Player.GetPlayerColor(playerID);

        TextMeshPro inputName = transform.Find("Input Name").gameObject.GetComponent<TextMeshPro>();
        inputName.text = playerInput.devices[0].ToString();
        inputName.color = Player.GetPlayerColor(playerID);

        if (playerInput.devices[0] == Keyboard.current)
        {
            transform.Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite = keyboard;
        }
    }
    void OnBack()
    {
        DestroyImmediate(gameObject);
    }
}
