using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public enum Direction
{
    RIGHT,
    LEFT,
    UP,
    DOWN
}
public class Player : MonoBehaviour
{
    static int currentID = 1;
    public int playerID;
    bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        private set
        {
            isDead = value;
            spriteRenderer.color = GetPlayerColor(playerID, value);
        }
    }
    float totalHealth = 200f;
    float health = 200f;
    public float Health
    {
        get { return health; }
        set
        {
            if (isDead) return;
            this.health = Mathf.Clamp(value, 0, totalHealth);
            terminal.SetHealth(value / totalHealth);

            if (health != 0) return;
            IsDead = true;
            StartCoroutine(RevivePlayer());
        }
    }
    float reviveCooldownSeconds = 5f;
    public int x;
    public int y;
    int wins;
    Terminal terminal;
    public Button button;
    PlayerInput playerInput;
    SpriteRenderer spriteRenderer;
    Dictionary<Direction, ParticleSystem> particleDirections = new Dictionary<Direction, ParticleSystem>();

    void Awake()
    {
        playerID = currentID;
        currentID++;
        x = playerID - 1;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = GetPlayerColor(playerID);
        terminal = FindObjectsOfType<Terminal>().FirstOrDefault(terminal => terminal.terminalID == playerID);
        playerInput = GetComponent<PlayerInput>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform particleTransform = transform.GetChild(i);
            ParticleSystem particleSystem = particleTransform.GetComponent<ParticleSystem>();
            particleDirections.Add((Direction)i, particleSystem);
            var main = particleSystem.main;
            main.startColor = GetPlayerColor(playerID);
        }

        playerInput.actions["Select"].performed += ctx =>
        {
            if (IsDead) return;
            if (button.buttonValue == "MR") Health -= 40f;
            terminal.AppendItem(button.buttonType, button.buttonValue);
        };
        playerInput.actions["Right"].performed += ctx => MoveButton(Direction.RIGHT);
        playerInput.actions["Left"].performed += ctx => MoveButton(Direction.LEFT);
        playerInput.actions["Up"].performed += ctx => MoveButton(Direction.UP);
        playerInput.actions["Down"].performed += ctx => MoveButton(Direction.DOWN);
    }
    public void OnEnable() => playerInput.actions.Enable();
    public void OnDisable() => playerInput.actions.Disable();
    public static Color GetPlayerColor(int playerID, bool isDark = false)
    {
        float brightness = isDark ? 0.5f : 1f;
        switch (playerID)
        {
            case 1:
                return new Color(brightness, 0f, 0f);
            case 2:
                return new Color(0f, 0f, brightness);
            case 3:
                return new Color(brightness, brightness, 0f);
            case 4:
                return new Color(0f, brightness, 0f);
        }
        return new Color(brightness, brightness, brightness, 1f);
    }
    void Start()
    {
        SetButtonFromPosition(x, y);
    }
    Player PlayerOccupyingButton(Button button)
    {
        return GameManager.players.FirstOrDefault(player => player.button == button);
    }
    Button GetButtonFromPosition(int x, int y)
    {
        return GameObject.Find("Buttons").transform.GetChild(x * GameManager.maxHeight + y).GetComponent<Button>();
    }
    // Handles player checking before setting the button
    public void SetButtonFromPosition(int x, int y, bool checkForPlayer = true, Direction delta = Direction.UP)
    {
        if (x != Mathf.Clamp(x, 0, GameManager.maxWidth - 1)) return;
        if (y != Mathf.Clamp(y, 0, GameManager.maxHeight - 1)) return;
        Button button = GetButtonFromPosition(x, y);
        // TODO: Change this to handle attacks/dead players
        if (checkForPlayer && !MovementAllowed(button, delta)) return;

        this.x = x;
        this.y = y;
        this.button = button;
        transform.position = button.transform.position;
        transform.Translate(Vector3.back);

        Health += 5f;
    }
    bool MovementAllowed(Button button, Direction delta)
    {
        Player playerCollided = PlayerOccupyingButton(button);
        if (!playerCollided) return true;
        if (playerCollided.IsDead)
        {
            playerCollided.SetButtonFromPosition(x, y, checkForPlayer: false);
            return true;
        }
        if (IsDead) return false;
        playerCollided.Health -= 10f;

        particleDirections[delta].Emit(30);
        return false;
    }
    public void MoveButton(Direction delta)
    {
        switch (delta)
        {
            case Direction.RIGHT:
                SetButtonFromPosition(x + 1, y, delta: delta);
                break;
            case Direction.LEFT:
                SetButtonFromPosition(x - 1, y, delta: delta);
                break;
            case Direction.UP:
                SetButtonFromPosition(x, y - 1, delta: delta);
                break;
            case Direction.DOWN:
                SetButtonFromPosition(x, y + 1, delta: delta);
                break;
        }
    }
    public void GetWin()
    {
        wins++;
        terminal.UpdateWinDisplay("Wins: " + wins);
    }
    IEnumerator RevivePlayer()
    {
        float reviveCooldown = reviveCooldownSeconds;
        while (reviveCooldown > 0)
        {
            reviveCooldown -= Time.deltaTime;
            yield return null;
        }
        IsDead = false;
        Health = totalHealth;
    }
}