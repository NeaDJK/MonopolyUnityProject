using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuManager : MonoBehaviour
{
    [SerializeField] GameObject helpMenu;
    [SerializeField] GameObject voidMenuBtn;
    [SerializeField] GameObject closeMenuBtn;

    private void Start()
    {
        voidMenuBtn.SetActive(false);
        helpMenu.SetActive(true);
        closeMenuBtn.SetActive(true);
    }

    public void VoidHelpMenu()
    {
        voidMenuBtn.SetActive(false);
        helpMenu.SetActive(true);
        closeMenuBtn.SetActive(true);
    }

    public void CloseHelpMenu()
    {
        voidMenuBtn.SetActive(true);
        helpMenu.SetActive(false);
        closeMenuBtn.SetActive(false);
    }
}
