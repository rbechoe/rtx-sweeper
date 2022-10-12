using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Text>().text = "Version " + Application.version;
    }
}
