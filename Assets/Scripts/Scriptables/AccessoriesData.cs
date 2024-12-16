using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableProps/AccessoriesData")]
public class AccessoriesData : ScriptableObject
{
    public List<GameObject> HeadAccessories;
    public List<GameObject> NeckAccessories;
}