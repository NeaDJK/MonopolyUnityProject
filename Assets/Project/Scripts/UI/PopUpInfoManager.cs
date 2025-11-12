using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpInfoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _popUpInfoText;

    private void Start()
    {
        EventManager.OnGameEvent.AddListener(UpdateInfo);
    }

    private void UpdateInfo(string message)
    {
        EventManager.UpdateGameEvent(message);
    }
}
