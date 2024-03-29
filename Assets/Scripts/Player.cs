using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Direction
{
    RIGHT,
    LEFT,
    UP,
    DOWN,
    CENTER
}
public class Player : MonoBehaviour
{
    static int currentID = 1;
    public int playerID { get; private set; }
    bool isDead_ = false;
    public bool isDead
    {
        get => isDead_;
        private set
        {
            isDead_ = value;
            spriteRenderer.color = GetPlayerColor(playerID, value);
        }
    }
    float totalHealth = 200f;
    float health_ = 200f;
    public float health
    {
        get => health_;
        set
        {
            if (isDead_) return;
            this.health_ = Mathf.Clamp(value, 0, totalHealth);
            terminal.SetHealth(value / totalHealth);

            if (health_ != 0) return;
            isDead = true;
            StartCoroutine(RevivePlayer());
        }
    }
    float reviveCooldownSeconds = 5f;
    public int x { get; private set; }
    public int y { get; private set; }
    int wins;
    Terminal terminal;
    public Button button { get; private set; }
    public PlayerInput playerInput { get; private set; }
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

        for (int player = 0; player < transform.childCount; player++)
        {
            Transform particleTransform = transform.GetChild(player);
            ParticleSystem particleSystem = particleTransform.GetComponent<ParticleSystem>();
            particleDirections.Add((Direction)player, particleSystem);
            particleSystem.main.startColor = GetPlayerColor(playerID);
        }

        playerInput.actions["Select"].performed += ctx =>
        {
            if (isDead) return;
            if (button.buttonValue == "MR") health -= 40f;
            particleDirections[Direction.CENTER].Emit(1);
            terminal.AppendItem(button.buttonType, button.buttonValue);
        };
        playerInput.actions["Back"].performed += ctx => terminal.AppendItem(ButtonType.DELETE, "DEL");
        playerInput.actions["Right"].performed += ctx => MoveButton(Direction.RIGHT);
        playerInput.actions["Left"].performed += ctx => MoveButton(Direction.LEFT);
        playerInput.actions["Up"].performed += ctx => MoveButton(Direction.UP);
        playerInput.actions["Down"].performed += ctx => MoveButton(Direction.DOWN);
    }
    void OnDestroy()
    {
        playerInput.actions["Select"].performed -= ctx =>
        {
            if (isDead) return;
            if (button.buttonValue == "MR") health -= 40f;
            particleDirections[Direction.CENTER].Emit(1);
            terminal.AppendItem(button.buttonType, button.buttonValue);
        };
        playerInput.actions["Back"].performed -= ctx => terminal.AppendItem(ButtonType.DELETE, "DEL");
        playerInput.actions["Right"].performed -= ctx => MoveButton(Direction.RIGHT);
        playerInput.actions["Left"].performed -= ctx => MoveButton(Direction.LEFT);
        playerInput.actions["Up"].performed -= ctx => MoveButton(Direction.UP);
        playerInput.actions["Down"].performed -= ctx => MoveButton(Direction.DOWN);
        playerInput.DeactivateInput();
    }
    public void OnEnable() => playerInput.actions.Enable();
    public void OnDisable() => playerInput.actions.Disable();
    public static Color GetPlayerColor(int playerID, bool isDark = false)
    {
        float brightness = isDark ? 0.5f : 1f;
        return playerID switch
        {
            1 => new Color(brightness, 0f, 0f),
            2 => new Color(0f, 0f, brightness),
            3 => new Color(brightness, brightness, 0f),
            4 => new Color(0f, brightness, 0f),
            _ => new Color(brightness, brightness, brightness, 1f)
        };
    }
    IEnumerator Start()
    {
        yield return null;
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

        health += 5f;
    }
    bool MovementAllowed(Button button, Direction delta)
    {
        Player playerCollided = PlayerOccupyingButton(button);
        if (!playerCollided) return true;
        if (playerCollided.isDead)
        {
            playerCollided.SetButtonFromPosition(x, y, checkForPlayer: false);
            return true;
        }
        if (isDead) return false;
        playerCollided.health -= 10f;

        particleDirections[delta].Emit(30);
        return false;
    }
    public void MoveButton(Direction delta) => delta switch
    {
        Direction.RIGHT => SetButtonFromPosition(x + 1, y, delta: delta),
        Direction.LEFT => SetButtonFromPosition(x - 1, y, delta: delta),
        Direction.UP => SetButtonFromPosition(x, y - 1, delta: delta),
        Direction.DOWN => SetButtonFromPosition(x, y + 1, delta: delta),
    };
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
        isDead = false;
        health = totalHealth;
    }
}