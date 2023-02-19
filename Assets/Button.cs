using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ButtonType
{
    NUMBER,
    FUNCTION,
    INSTANT_FUNCTION,
    DELETE,
    DECIMAL,
    CLEAR,
    EQUALS
}
public class Button : MonoBehaviour
{
    [SerializeField] ButtonType buttonType;
    [SerializeField] string buttonValue;
    void OnValidate() {
        transform.Find("Text").GetComponent<TextMeshPro>().text = buttonValue;
        transform.Find("Background").GetComponent<SpriteRenderer>().color = GetButtonColor();
    }
    Color GetButtonColor()
    {
        switch (buttonType)
        {
            case ButtonType.FUNCTION:
                return new Color(0.3f, 0.3f, 1f);
            case ButtonType.INSTANT_FUNCTION:
                return new Color(0f, 0.9f, 0.9f);
            case ButtonType.CLEAR:
                return new Color(1f, 0.3f, 0.3f);
            case ButtonType.EQUALS:
                return new Color(0f, 1f, 0f);
            default:
                return new Color(1f, 1f, 1f);
        }
    }
    void OnMouseDown()
    {
        GameObject.Find("Terminal").gameObject.GetComponent<Terminal>().AppendItem(buttonType, buttonValue);
    }
}
