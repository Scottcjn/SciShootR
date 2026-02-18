using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ShopItem", menuName = "ShopItem")]
public class ShopItem : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public Sprite Image;
}
