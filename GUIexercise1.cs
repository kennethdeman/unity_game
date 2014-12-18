using UnityEngine;
using System.Collections;

public class GUIexercise1 : MonoBehaviour {
    string stringToEdit = "";
    string stringName = "";
    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 120, 200), "Loader Menu");

        GUI.Label(new Rect(20, 30, 80, 20), stringName);       
        
        stringToEdit = GUI.TextField(new Rect(20, 50, 100, 20), stringToEdit, 25);

        if (GUI.Button(new Rect(20, 70, 80, 20), "Button"))
        {
            stringName = stringToEdit;
        }
        
    }
}
