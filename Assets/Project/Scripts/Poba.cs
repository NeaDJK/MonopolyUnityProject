using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poba : MonoBehaviour
{
    [SerializeField] Transform endPosition;
    [SerializeField] Transform player;

    // Start is called before the first frame update
    void Start()
    {
        Action act = Test;

        act.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("HUII0");
        }
    }

    void Test()
    {
        Debug.Log("Privao");
    }

}
