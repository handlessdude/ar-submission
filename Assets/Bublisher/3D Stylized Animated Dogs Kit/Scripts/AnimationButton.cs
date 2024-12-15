using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DemoKitStylizedAnimatedDogs
{

[RequireComponent(typeof(Button))]
public class AnimationButton : MonoBehaviour
{
    [SerializeField] public int _animationID;

    public event Action<int> Click;

    private void Awake()
    {
        var button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(()=>{Click?.Invoke(_animationID);});
        }
    }
}

}
