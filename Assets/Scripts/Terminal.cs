using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Data;
using System.Linq;

public class Terminal : MonoBehaviour
{
    public int terminalID { get; private set; }
    public Transform healthBar { get; private set; }
    List<string> operationQueue = new List<string> { "0" };
    string memory = "0";
    bool LookingForNumber() => operationQueue.Count % 2 == 0;
    TextMeshPro evaluationText;
    TextMeshPro winText;

    void OnValidate()
    {
        TextMeshPro playerText = transform.Find("Player Indicator").gameObject.GetComponent<TextMeshPro>();
        playerText.text = "P" + terminalID;
        playerText.color = Player.GetPlayerColor(terminalID, true);
        SpriteRenderer border = transform.Find("Border").gameObject.GetComponent<SpriteRenderer>();
        border.color = Player.GetPlayerColor(terminalID);
        TextMeshPro winText = transform.Find("Wins").gameObject.GetComponent<TextMeshPro>();
        winText.color = Player.GetPlayerColor(terminalID, true);
        Transform healthBar = transform.Find("Health Bar");
        healthBar.Find("Health Border").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
        healthBar.Find("Health Fill").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
        healthBar.Find("Health Background").GetComponent<SpriteRenderer>().color = Player.GetPlayerColor(terminalID);
    }
    void Awake()
    {
        operationQueue.Capacity = 3;
        evaluationText = transform.Find("Evaluation").gameObject.GetComponent<TextMeshPro>();
        winText = transform.Find("Wins").gameObject.GetComponent<TextMeshPro>();
        healthBar = transform.Find("Health Bar").Find("Health Fill");
    }
    void Start()
    {
        if (GameManager.players.Length < terminalID) DestroyImmediate(gameObject);
    }
    public void AppendItem(ButtonType buttonType, string buttonValue)
    {
        string lastItem = operationQueue[operationQueue.Count - 1];

        switch (buttonType)
        {
            case ButtonType.CLEAR:
                operationQueue.Clear();
                operationQueue.Add("0");
                if (buttonValue == "AC") UpdateMemory("MC");
                break;
            case ButtonType.EQUALS:
                float number = EvaluateCurrentOperation();
                if (number == GameManager.correctNumber)
                {
                    StartCoroutine(GameManager.GetWin(terminalID));
                }
                break;
            case ButtonType.INSTANT_FUNCTION:
                float newValue = ApplyInstantFunction(buttonValue, EvaluateCurrentOperation());
                newValue = FilterValue(newValue);
                operationQueue[0] = newValue.ToString();
                EvaluateCurrentOperation();
                break;
            case ButtonType.MEMORY:
                UpdateMemory(buttonValue);
                break;
            default:
                if (LookingForNumber())
                {
                    AppendButtonFromFunction(lastItem, buttonType, buttonValue);
                }
                else
                {
                    AppendButtonFromNumber(lastItem, buttonType, buttonValue);
                }
                break;
        }

        UpdateQueueDisplay();
    }

    void UpdateMemory(string buttonValue)
    {
        switch (buttonValue)
        {
            case "MC":
                memory = "0";
                break;
            case "MR":
                AppendItem(ButtonType.NUMBER, memory);
                break;
            case "M-":
                memory = EvaluateOperation(memory, "-", EvaluateCurrentOperation().ToString()).ToString();
                break;
            case "M+":
                memory = EvaluateOperation(memory, "+", EvaluateCurrentOperation().ToString()).ToString();
                break;
        }
    }

    void AppendButtonFromNumber(string lastItem, ButtonType buttonType, string buttonValue)
    {
        switch (buttonType)
        {
            case ButtonType.NUMBER:
                if (lastItem == "0." && buttonValue != "0") SetLastItem("0." + buttonValue);
                else SetLastItem(buttonValue);
                break;

            case ButtonType.FUNCTION:
                if (lastItem == "0.") SetLastItem("0");
                AddToQueue(buttonValue);
                break;

            case ButtonType.DECIMAL:
                SetLastItem("0.");
                break;

            case ButtonType.DELETE:
                if (operationQueue.Count == 1) operationQueue[0] = "0";
                else if (lastItem != "0") SetLastItem("0");
                else operationQueue.RemoveAt(operationQueue.Count - 1);
                break;
        }
    }
    void AppendButtonFromFunction(string lastItem, ButtonType buttonType, string buttonValue)
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
    void AddToQueue(string value)
    {
        if (operationQueue.Count == operationQueue.Capacity) EvaluateCurrentOperation();
        operationQueue.Add(value);
    }
    float EvaluateCurrentOperation()
    {
        if (LookingForNumber()) operationQueue.RemoveAt(operationQueue.Count - 1);
        if (operationQueue.Count == 1) return float.Parse(operationQueue[0]);

        float result = EvaluateOperation(operationQueue[0], operationQueue[1], operationQueue[2]);

        operationQueue.Clear();
        operationQueue.Add(result.ToString());
        return result;
    }
    float EvaluateOperation(string num1, string operation, string num2)
    {
        float result = ApplyFunction(num1, operation, num2);
        result = Mathf.Round(result * 100) / 100;
        result = FilterValue(result);
        return result;
    }
    float ApplyFunction(string num1String, string operation, string num2String)
    {
        float num1 = float.Parse(num1String);
        float num2 = float.Parse(num2String);
        switch (operation)
        {
            case "+":
                return num1 + num2;
            case "-":
                return num1 - num2;
            case "*":
                return num1 * num2;
            case "/":
                if (num2 == 0) HandleIllegalOperation();
                return num1 / num2;
        }
        return 0;
    }
    float ApplyInstantFunction(string buttonValue, float x) => buttonValue switch
    {
        "1/x" => x != 0 ? 1 / x : HandleIllegalOperation(),
        "+/-" => x * -1,
        "round" => Mathf.Round(x),
        "x^2" => Mathf.Pow(x, 2),
        "2^x" => Mathf.Pow(2, x),
        "sqrtx" => x >= 0 ? Mathf.Sqrt(x) : HandleIllegalOperation(),
        _ => x
    };
    float FilterValue(float value) => float.IsNaN(value) || float.IsSubnormal(value) ? 0 : Mathf.Clamp(value, -1_000_000, 1_000_000);
    float HandleIllegalOperation()
    {
        return 0;
    }
    void SetLastItem(string value) => operationQueue[operationQueue.Count - 1] = value;
    void AddLastItem(string value) => operationQueue[operationQueue.Count - 1] += value;

    public void UpdateWinDisplay(string text) => winText.text = text;
    void UpdateQueueDisplay()
    {
        evaluationText.text = string.Join(" ", operationQueue);
    }
    public void SetHealth(float percentage)
    {
        healthBar.localScale = new Vector3(8f * percentage, healthBar.localScale.y, healthBar.localScale.z);
        healthBar.localPosition = new Vector3(-1.76f * (1 - percentage), healthBar.localPosition.y, healthBar.localPosition.z);
    }
}
