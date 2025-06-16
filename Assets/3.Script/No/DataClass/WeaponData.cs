using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public int ID;
    public string WeaponName;
    public int Level;
    public int BaseDamage;

    public int GetDamage() => BaseDamage + Level;
}