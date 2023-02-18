using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ButtonType {
    NUMBER,
    FUNCTION
}
public class Button : MonoBehaviour
{
    [SerializeField] ButtonType buttonType;
    string buttonValue;
    public void LoadButtonInfo(string buttonValue)
    {
        this.buttonValue = buttonValue;
        transform.Find("Text").GetComponent<TextMeshPro>().text = buttonValue;
    }
}
