using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private int number;
    [SerializeField] private string name;
    [SerializeField] private string discription;
    [SerializeField] private float price;
    [SerializeField] private string type;
    [SerializeField] private double tax;
    [SerializeField] private bool isBought;
    [SerializeField] private int owner;
}
