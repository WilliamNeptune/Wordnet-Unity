using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchBarController : MonoBehaviour
{
    public InputField searchInputField;
    private TouchScreenKeyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            searchInputField.text = keyboard.text;
            keyboard = null;
        }


    }

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open(searchInputField.text, TouchScreenKeyboardType.URL, false, false, false, false);
    }
}
