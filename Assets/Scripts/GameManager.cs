using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static int maxWidth;
    public static int maxHeight;
    Transform buttons;
    void Awake()
    {
        buttons = GameObject.Find("Buttons").transform;
        maxWidth = buttons.childCount / 5;
        maxHeight = buttons.gameObject.GetComponent<GridLayoutGroup>().constraintCount;
    }
}