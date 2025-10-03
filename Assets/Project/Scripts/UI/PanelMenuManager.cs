using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private GameObject _menu;
    private int _currentPanelIndex = 0;

    public void SwitchToPanel(int index)
    {
        if (index >= 0 && index < _panels.Length)
        {
            _panels[_currentPanelIndex].gameObject.SetActive(false);
            _panels[index].gameObject.SetActive(true);
            _currentPanelIndex = index;
        }
    }
    
    
}
