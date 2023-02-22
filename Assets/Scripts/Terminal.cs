using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Data;
using System.Linq;

public class Terminal : MonoBehaviour
{
    public int terminalID;
    public Transform healthBar;
    List<string> operationQueue = new List<string> { "0" };
    bool LookingForNumber() => operationQueue.Count % 2 == 0;
    TextMeshPro operationText;
    TextMeshPro evaluationText;

    void OnValidate()
    {
        TextMeshPro playerText = transform.Find("Player Indicator").gameObject.GetComponent<TextMeshPro>();
        SpriteRenderer border = transform.Find("Border").gameObject.GetComponent<SpriteRenderer>();
        Transform healthBar = transform.Find("Health Bar");
        playerText.text = "P" + terminalID;
        playerText.color = Player.GetPlayerColor(terminalID, true);
        border.color = Player.GetPlayerColor(terminalID);
        healthBar.Find("Health Border").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
        healthBar.Find("Health Fill").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
        healthBar.Find("Health Background").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
    }
    void Awake()
    {
        operationQueue.Capacity = 3;
        operationText = transform.Find("Operation").gameObject.GetComponent<TextMeshPro>();
        evaluationText = transform.Find("Evaluation").gameObject.GetComponent<TextMeshPro>();
        healthBar = transform.Find("Health Bar").Find("Health Fill");
    }
    public void AppendItem(ButtonType buttonType, string buttonValue)
    {
        string lastItem = operationQueue[operationQueue.Count - 1];

        switch (buttonType)
        {
            case ButtonType.CLEAR:
                operationQueue.Clear();
                operationQueue.Add("0");
                break;
            case ButtonType.EQUALS:
                EvaluateOperation();
                break;
            case ButtonType.INSTANT_FUNCTION:
                float newValue = ApplyInstantFunction(buttonValue, EvaluateOperation());
                newValue = FilterValue(newValue);
                operationQueue[0] = newValue.ToString();
                EvaluateOperation();
                break;
        }

        if (!LookingForNumber())
            appendButtonFromNumber(lastItem, buttonType, buttonValue);
        else
            appendButtonFromFunction(lastItem, buttonType, buttonValue);

        UpdateQueueDisplay();
    }

    void appendButtonFromNumber(string lastItem, ButtonType buttonType, string buttonValue)
    {
        switch (buttonType)
        {
            case ButtonType.NUMBER:
                if (lastItem == "0." && buttonValue != "0")
                    SetLastItem("0." + buttonValue);
                else
                    SetLastItem(buttonValue);
                break;

            case ButtonType.FUNCTION:
                if (lastItem == "0.")
                    SetLastItem("0");
                AddToQueue(buttonValue);
                break;

            case ButtonType.DECIMAL:
                SetLastItem("0.");
                break;

            case ButtonType.DELETE:
                if (operationQueue.Count == 1)
                    operationQueue[0] = "0";
                else if (lastItem != "0")
                    SetLastItem("0");
                else
                    operationQueue.RemoveAt(operationQueue.Count - 1);
                break;
        }
    }
    void appendButtonFromFunction(string lastItem, ButtonType buttonType, string buttonValue)
    {
        switch (buttonType)
        {
            case ButtonType.NUMBER:
                AddToQueue(buttonValue);
                break;

            case ButtonType.FUNCTION:
                SetLastItem(buttonValue);
                break;

            case ButtonType.DECIMAL:
                AddToQueue("0.");
                break;

            case ButtonType.DELETE:
                operationQueue.RemoveAt(operationQueue.Count - 1);
                break;
        }
    }

    float ApplyInstantFunction(string buttonValue, float x)
    {
        switch (buttonValue)
        {
            case "1/x":
                if (x == 0) return HandleIllegalOperation();
                return 1 / x;
            case "+/-":
                return x * -1;
            case "round":
                return Mathf.Round(x);
            case "x^2":
                return Mathf.Pow(x, 2);
            case "2^x":
                return Mathf.Pow(2, x);
            case "sqrtx":
                if (x < 0) return HandleIllegalOperation();
                return Mathf.Sqrt(x);
        }
        return x;
    }

    void AddToQueue(string value)
    {
        if (operationQueue.Count == operationQueue.Capacity) EvaluateOperation();
        operationQueue.Add(value);
    }
    float EvaluateOperation()
    {
        if (LookingForNumber()) operationQueue.RemoveAt(operationQueue.Count - 1);

        string toEval = string.Join(" ", operationQueue);
        float result = Convert.ToSingle(new DataTable().Compute(toEval, ""));
        result = Mathf.Round(result * 100) / 100;
        result = FilterValue(result);

        operationQueue.Clear();
        operationQueue.Add(result.ToString());
        return result;
    }
    float FilterValue(float value)
    {
        if (float.IsNaN(value) || float.IsSubnormal(value)) return 0;
        if (float.IsPositiveInfinity(value)) return 1_000_000;
        if (float.IsNegativeInfinity(value)) return -1_000_000;
        return Mathf.Clamp(value, -1_000_000, 1_000_000);
    }
    float HandleIllegalOperation()
    {
        return 0;
    }
    void SetLastItem(string value) => operationQueue[operationQueue.Count - 1] = value;
    void AddLastItem(string value) => operationQueue[operationQueue.Count - 1] += value;

    void UpdateQueueDisplay()
    {
        operationText.text = string.Join(" ", operationQueue.Skip(1));
        evaluationText.text = operationQueue[0];
    }
    public void SetHealth(float percentage)
    {
        healthBar.localScale = new Vector3(6.25f * percentage, healthBar.localScale.y, healthBar.localScale.z);
        healthBar.localPosition = new Vector3(-1.375f * (1 - percentage), healthBar.localPosition.y, healthBar.localPosition.z);
    }
}
