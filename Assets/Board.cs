using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject tile;
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // GameObject newTile = Instantiate(tile);
                // newTile.transform.SetParent(gameObject.transform);
                // newTile.transform.localPosition = new Vector3(i-1, j-1, 0);
            }
        }
    }
}
