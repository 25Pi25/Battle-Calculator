using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    void Start()
    {
        //StartCoroutine(Wait());
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(10f);
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name == "Player").ToArray();
        PlayerInput p1 = objects[0].GetComponent<PlayerInput>();
        string p1cs = p1.currentControlScheme;
        PlayerInput p2 = objects[1].GetComponent<PlayerInput>();
        string p2cs = p2.currentControlScheme;
        p2.SwitchCurrentControlScheme(p1cs);
        p1.SwitchCurrentControlScheme(p2cs);
    }
}