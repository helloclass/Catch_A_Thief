using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    // Update is called once per frame
    public void TaskOnClick()
    {
        Application.Quit();
    }
}
