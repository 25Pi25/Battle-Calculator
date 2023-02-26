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
    public int playerID { get; private set; }
    void Awake()
    {
        transform.SetParent(GameObject.Find("Player Boxes").transform);
        playerInput = GetComponent<PlayerInput>();

        if (playerInput.devices[0] == Keyboard.current)
        {
            transform.Find("Icon").gameObject.GetComponent<SpriteRenderer>().sprite = keyboard;
        }

        UpdatePlayer();
    }
    public void UpdatePlayer()
    {
        playerID = transform.GetSiblingIndex() + 1;

        TextMeshPro playerNumber = transform.Find("Player Number").gameObject.GetComponent<TextMeshPro>();
        playerNumber.text = "Player " + playerID;
        playerNumber.color = Player.GetPlayerColor(playerID);

        TextMeshPro inputName = transform.Find("Input Name").gameObject.GetComponent<TextMeshPro>();
        inputName.text = playerInput.devices[0].ToString();
        inputName.color = Player.GetPlayerColor(playerID);
    }
    void OnBack()
    {
        Transform parent = transform.parent;
        transform.SetParent(null);
        foreach (PlayerBox playerBox in parent.GetComponentsInChildren<PlayerBox>()) playerBox.UpdatePlayer();
        DestroyImmediate(gameObject);
    }
}
