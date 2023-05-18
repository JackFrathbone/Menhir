using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    [Header("Things To Run")]
    [SerializeField, TextArea(1, 6)] string _messageBoxText;

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_messageBoxText != null && _messageBoxText != "")
        {
            MessageBox.instance.Create(_messageBoxText, true);
        }

        Destroy(gameObject);
    }
}
