using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public ShopItem shopItem;
    public int amountMin;
    public int amountMax;
    public float price;
    public int boughtState = 0;
    public bool defaultShip = false;
    public int selectedSkin = 0;

    [HideInInspector]
    public int finalAmount;
    [HideInInspector]
    public int position;
    [HideInInspector] 
    public bool randomSelected;
}
