using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionChecker : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = "Version " + Application.version;
    }
}
