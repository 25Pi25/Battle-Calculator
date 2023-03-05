using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static int maxWidth;
    public static int maxHeight;
    public static float correctNumber;
    public GameObject playerPrefab;
    public static Player[] players;
    public static InputDevice[] devices;
    Transform buttons;
    static TextMeshPro number;
    [SerializeField] Sprite[] difficultySprites;
    static int difficulty = 1;
    public static int Difficulty
    {
        get => difficulty;
        set
        {
            difficulty = value;
            GameObject.FindObjectOfType<Difficulter>().SetSprite(value);
        }
    }
    void Awake()
    {
        buttons = GameObject.Find("Buttons").transform;
        maxWidth = buttons.childCount / 5;
        maxHeight = buttons.gameObject.GetComponent<GridLayoutGroup>().constraintCount;
        number = transform.Find("Number").gameObject.GetComponent<TextMeshPro>();

        foreach (InputDevice device in devices)
        {
            string controlScheme = device == Keyboard.current ? "Keyboard&Mouse" : "Gamepad";
            PlayerInput person = PlayerInput.Instantiate(playerPrefab, controlScheme: controlScheme, pairWithDevice: device);
        }
        players = FindObjectsOfType<Player>().OrderBy(player => player.playerID).ToArray();
        RandomizeNumber();
    }
    static void RandomizeNumber()
    {
        float newNumber;
        switch (Random.Range(0, difficulty))
        {
            case 0:
                newNumber = Random.Range(10, 100);
                break;
            case 1:
                newNumber = Random.Range(100, 1000);
                break;
            default:
                newNumber = Random.Range(1000, 10000);
                break;
        }
        if (Random.Range(0, 15 / difficulty + 1) == 0) newNumber *= -1f;
        number.text = newNumber.ToString();
        correctNumber = newNumber;
    }
    public static IEnumerator GetWin(int playerID)
    {
        players[playerID - 1].GetWin();
        Debug.Log("Player " + playerID + " got it!");
        DisableAll();
        for (float time = 0; time < devices.Length; time += Time.deltaTime)
        {
            number.text = Random.Range(0, 99999).ToString();
            yield return null;
        }
        RandomizeNumber();
        EnableAll();
    }
    public static void EnableAll()
    {
        foreach (Player player in players) player.OnEnable();
    }
    public static void DisableAll()
    {
        foreach (Player player in players) player.OnDisable();
    }
}