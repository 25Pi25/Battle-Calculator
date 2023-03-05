using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulter : MonoBehaviour
{
    [SerializeField] Sprite[] difficultySprites;
    public void SetSprite(int spriteNumber)
    {
        GetComponent<SpriteRenderer>().sprite = difficultySprites[spriteNumber-1];
    }
}
