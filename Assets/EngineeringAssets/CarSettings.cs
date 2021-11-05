using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Cars/Create car settings", order = 1)]
public class CarSettings : ScriptableObject
{
    public enum CarType
    {
        Car1,Car2,Car3,Car4
    }
    
    public CarType carType;
    public string Name;
    public GameObject CarPrefab;
    public Sprite Icon;
}
