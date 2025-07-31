using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    [SerializeField] public Sprite[] cards;
    [SerializeField] private Image[] cells;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            cells[0].sprite = cards[0];
        }
    }

    public void AddNewCard(int index)
    {
        cells[index].sprite = cards[index];
    }    
}
