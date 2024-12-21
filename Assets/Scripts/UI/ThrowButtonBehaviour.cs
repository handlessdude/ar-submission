using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ThrowButtonBehaviour : MonoBehaviour
{
    public ThrowBehaviour throwBehaviour;
    [SerializeField] public int throwableID;
    
    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                throwBehaviour.ToggleThrowMode(throwableID);
            });
        }
    }
}