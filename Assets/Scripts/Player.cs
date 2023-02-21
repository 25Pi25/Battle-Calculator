using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Player : MonoBehaviour
{
    static int currentID = 1;
    public int playerID;
    public bool isDead = false;
    [SerializeField] int x;
    [SerializeField] int y;
    public Terminal terminal;
    public Button button;
    PlayerInput playerInput;

    void Awake()
    {
        playerID = currentID;
        currentID++;
        x = playerID - 1;

        GetComponent<SpriteRenderer>().color = GetPlayerColor(playerID);
        terminal = FindObjectsOfType<Terminal>().FirstOrDefault(terminal => terminal.terminalID == playerID);
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Select"].performed += ctx => terminal.AppendItem(button.buttonType, button.buttonValue);
        playerInput.actions["Right"].performed += ctx => MoveButton(1, 0);
        playerInput.actions["Left"].performed += ctx => MoveButton(-1, 0);
        playerInput.actions["Up"].performed += ctx => MoveButton(0, -1);
        playerInput.actions["Down"].performed += ctx => MoveButton(0, 1);
    }
    public static Color GetPlayerColor(int playerID)
    {
        switch (playerID)
        {
            case 1:
                return new Color(1f, 0f, 0f);
            case 2:
                return new Color(0f, 0f, 1f);
            case 3:
                return new Color(1f, 1f, 0f);
            case 4:
                return new Color(0f, 1f, 0f);
        }
        return new Color();
    }
    void Start()
    {
        StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return null;
        SetButtonFromPosition(x, y);
    }
    Player PlayerOccupyingButton(Button button)
    {
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.button == button) return player;
        }
        return null;
    }
    Button GetButtonFromPosition(int x, int y)
    {
        return GameObject.Find("Buttons").transform.GetChild(x + y * 4).GetComponent<Button>();
    }
    // Handles player checking before setting the button
    void SetButtonFromPosition(int x, int y)
    {
        x %= 4;
        if (x < 0) x += 4;
        y %= 5;
        if (y < 0) y += 5;
        Button button = GetButtonFromPosition(x, y);
        // TODO: Change this to handle attacks/dead players
        if (PlayerOccupyingButton(button)) return;
        this.x = x;
        this.y = y;
        this.button = button;
        transform.position = button.transform.position;
        transform.Translate(Vector3.back);
    }
    public void MoveButton(int xDelta, int yDelta)
    {
        SetButtonFromPosition(x + xDelta, y + yDelta);
    }
}