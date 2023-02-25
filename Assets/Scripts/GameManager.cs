using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static int maxWidth;
    public static int maxHeight;
    public static float correctNumber;
    public GameObject playerPrefab;
    public static Player[] players;
    Transform buttons;
    static TextMeshPro number;
    void Awake()
    {
        buttons = GameObject.Find("Buttons").transform;
        maxWidth = buttons.childCount / 5;
        maxHeight = buttons.gameObject.GetComponent<GridLayoutGroup>().constraintCount;
        number = transform.Find("Number").gameObject.GetComponent<TextMeshPro>();
    }
    void Start()
    {
        StartCoroutine(Wait());
    }
    static void RandomizeNumber()
    {
        float newNumber;
        switch (Random.Range(0, 3))
        {
            case 0:
                newNumber = Random.Range(10, 89);
                break;
            case 1:
                newNumber = Random.Range(100, 899);
                break;
            default:
                newNumber = Random.Range(1000, 8999);
                break;
        }
        if (Random.Range(0, 5) == 0) newNumber *= -1f;
        number.text = newNumber.ToString();
        correctNumber = newNumber;
    }
    static List<InputDevice> devices = new List<InputDevice>();
    IEnumerator Wait()
    {
        yield return null;
        devices.Capacity = 4;
        devices.Add(Keyboard.current);
        foreach(Gamepad gamepad in Gamepad.all) devices.Add(gamepad);

        foreach (InputDevice device in devices)
        {
            string controlScheme = device == Keyboard.current ? "Keyboard&Mouse" : "Gamepad";
            PlayerInput person = PlayerInput.Instantiate(playerPrefab, controlScheme: controlScheme, pairWithDevice: device);
        }
        players = FindObjectsOfType<Player>().OrderBy(player => player.playerID).ToArray();
        RandomizeNumber();
    }
    public static IEnumerator GetWin(int playerID)
    {
        players[playerID - 1].GetWin();
        Debug.Log("Player " + playerID + " got it!");
        DisableAll();
        for (float time = 0; time < devices.Count; time += Time.deltaTime)
        {
            number.text = Random.Range(0, 99999).ToString();
            yield return null;
        }
        RandomizeNumber();
        EnableAll();
    }
    static void EnableAll()
    {
        foreach (Player player in players) player.OnEnable();
    }
    static void DisableAll()
    {
        foreach (Player player in players) player.OnDisable();
    }
}