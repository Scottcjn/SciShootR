using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShopController : MonoBehaviour
{

    public bool randomizeShop;
    public int randomItemNumber;//How many Items should be in the shop
    public bool repeatRandomItems; //When the shop is random, let the shop have more than one card
    public List<Item> items; //items that will be sold on the store
    public GameObject itemCard; //Visuals for the item in the shop
    public GameObject parent; //Where the item cards will be (shop), for example, the "Content of a scrollView", change this if neccesary 
    public float defaultAmount = 5;//Amount given if not amount especified
    public TextMeshProUGUI moneyText;
    public float spacingValue = 0.1f;
    
    private GameObject[] cards; //Array where generated cards will be

    [SerializeField] GameMngr gameMngr;

    // Start is called before the first frame update
    void Start()
    {
        SaveSystem.SaveMoney(50000);
        gameMngr = FindObjectOfType<GameMngr>();
        if(moneyText != null)
            moneyText.text = SaveSystem.LoadMoney().money.ToString(); //Only if you have a text for your money
        
        if(randomizeShop)
            RandomizeShop();
        else
            StartShop();
    }
    
    public void StartShop()
    {
        //Initialize the array with the number of items
        cards = new GameObject[items.Count];

        RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();
        RectTransform itemTransform = itemCard.GetComponent<RectTransform>();
       
        //Calculate height and width of cards
        float width = itemTransform.rect.width;
        float height = itemTransform.rect.height;
        float spacing = width * spacingValue;
        
        //Resize the width of the container depending on the number of items
        float cWidth = ((width + spacing) * items.Count + spacing) - width * 4;
        
        containerRectTransform.offsetMin = new Vector2(0, containerRectTransform.offsetMin.y);
        containerRectTransform.offsetMax = new Vector2(cWidth, containerRectTransform.offsetMax.y);

        //If you want to resize the height to make a vertical shop, do the following
        /*
         Go to this page: https://forum.unity.com/threads/solved-trying-to-create-a-dynamic-scroll-view.313422/
         */
        
        //Generate itemCards for every item, the card needs an Image, a TextMeshPro text for the price and a buy button
        int i = 0;
        foreach (Item item in items)
        {
            //Check if amounts are written correctly
            if (item.amountMin > item.amountMax)
            {
                int max = item.amountMax;
                item.amountMax = item.amountMin;
                item.amountMin = max;
            }
            //Select a random amount between minAmount and maxAmount
            item.finalAmount = Random.Range(item.amountMin, item.amountMax);
            
            //Set item position on the shop
            item.position = i;

            //Spawn the card
            GameObject card = Instantiate(itemCard, transform.position, Quaternion.identity);
            //Make this object the card parent
            card.transform.SetParent(parent.transform,false);
            //Set the image to the item image
            card.transform.Find("ItemImage").GetComponent<Image>().sprite = item.shopItem.Image;
            //Set the price to the item price
            card.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = item.price + "$";
            //Get button and add listener
            card.transform.Find("CardButton").GetComponent<Button>().onClick.AddListener(() => BuyOrSelectShipSkin(item));
            //Display the amount of the item
            card.transform.Find("Amount").GetComponentInChildren<TextMeshProUGUI>().text = item.finalAmount.ToString();
            //Display Name
            card.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>().text = item.shopItem.itemName + ":";
            
            //Add the card to an array
            cards[i] = card;

            //Move the card to position
            RectTransform cardTransform = card.GetComponent<RectTransform>();
            float x = -containerRectTransform.rect.width / 2 + (width + spacing) * (i) + spacing;
            float y = -height / 2;
            cardTransform.offsetMin = new Vector2(x, y);
            x = cardTransform.offsetMin.x + width;
            y = cardTransform.offsetMin.y + height;
            cardTransform.offsetMax = new Vector2(x, y);

            ///Change later to make it able to select your ship instead of just showing that it's already purchased
            //if (item.boughtState == 1)
            //{
            //    cards[item.position].transform.Find("BuyButton").GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            //}
            string boughtItem = item.shopItem.itemName;
            if (item.defaultShip == true || PlayerPrefs.GetInt(boughtItem) == 1)
            {
                cards[item.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Select";
            }
            i++;
        }
        SelectSavedSkin();
    }

    //Pick random items for the shop
    private void RandomizeShop()
    {
        //Initialize the array with the number of items
        cards = new GameObject[items.Count];

        RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();
        RectTransform itemTransform = itemCard.GetComponent<RectTransform>();
       
        //Calculate height and width of cards
        float width = itemTransform.rect.width;
        float height = itemTransform.rect.height;
        float spacing = width * spacingValue;
        
        //Resize the width of the container depending on the number of items
        float cWidth = ((width + spacing) * randomItemNumber + spacing) - width * 4;
        
        containerRectTransform.offsetMin = new Vector2(0, containerRectTransform.offsetMin.y);
        containerRectTransform.offsetMax = new Vector2(cWidth, containerRectTransform.offsetMax.y);
        
        //If you want to resize the height to make a vertical shop, do the following
        /*
         Go to this page: https://forum.unity.com/threads/solved-trying-to-create-a-dynamic-scroll-view.313422/
         */
        
        //Generate itemCards for every item, the card needs an Image, a TextMeshPro text for the price and a buy button
        for(int i = 0; i < randomItemNumber; i++)
        {
            //Check if the shop has enough items to sell
            if (randomItemNumber > items.Count)
                randomItemNumber = items.Count;
            
            Item item = null;

            //Make sure it does not repeat items if wanted
            if (!repeatRandomItems)
            {
                bool select = false;
                while (!select)
                {
                    item = items[Random.Range(0, items.Count)];
                    if (!item.randomSelected)
                    {
                        select = true;
                        item.randomSelected = true;
                    }
                        
                }
            }
            else
            {
                item = items[Random.Range(0, items.Count)];
            }

            //Check if amounts are written correctly
            if (item.amountMin > item.amountMax)
            {
                int max = item.amountMax;
                item.amountMax = item.amountMin;
                item.amountMin = max;
            }
            //If amount = 0, set amount to 1 (It has no sense for an item in this shop to exist if amount is 0)
            if (item.amountMin == 0)
            {
                item.amountMin = 1;
                if (item.amountMax == 0)
                    item.amountMax = 1;
            }
            
            //Select a random amount between minAmount and maxAmount
            item.finalAmount = Random.Range(item.amountMin, item.amountMax);
            
            //Set item position on the shop
            item.position = i;

            //Spawn the card
            GameObject card = Instantiate(itemCard, transform.position, Quaternion.identity);
            //Make this object the card parent
            card.transform.SetParent(parent.transform,false);
            //Set the image to the item image
            card.transform.Find("ItemImage").GetComponent<Image>().sprite = item.shopItem.Image;
            //Set the price to the item price
            card.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = item.price + "$";
            //Get button and add listener
            card.transform.Find("CardButton").GetComponent<Button>().onClick.AddListener(() => BuyOrSelectShipSkin(item));
            //Display the amount of the item
            card.transform.Find("Amount").GetComponentInChildren<TextMeshProUGUI>().text = item.finalAmount.ToString();
            //Display Name
            card.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>().text = item.shopItem.itemName + ":";
            
            //Add the card to an array
            cards[i] = card;

            //Move the card to position
            RectTransform cardTransform = card.GetComponent<RectTransform>();
            float x = -containerRectTransform.rect.width / 2 + (width + spacing) * (i) + spacing;
            float y = -height / 2;
            cardTransform.offsetMin = new Vector2(x, y);
            x = cardTransform.offsetMin.x + width;
            y = cardTransform.offsetMin.y + height;
            cardTransform.offsetMax = new Vector2(x, y);
        }
    }

    //Delete all cards
    public void CleanShop()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = null;
        }
    }

    public void ResetShop()
    {
        CleanShop();
        StartShop();
    }

    public void BuyOrSelectShipSkin(Item item)
    {
        //Load how much money do I have
        float actualMoney = SaveSystem.LoadMoney().money;

        //If we have enough money, we buy the item
        string boughtItem = item.shopItem.itemName;
        if (item.defaultShip == false && PlayerPrefs.GetInt(boughtItem) == 0 && item.price <= actualMoney && item.finalAmount > 0)
        {
            //buy
            //Decrease money and save it
            actualMoney -= item.price;
            SaveSystem.SaveMoney(actualMoney);
            //Decrease item amount and change its text
            item.finalAmount--;
            cards[item.position].transform.Find("Amount").GetComponentInChildren<TextMeshProUGUI>().text = item.finalAmount.ToString();

            PlayerPrefs.SetInt(boughtItem, 1);
            cards[item.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Select";

            if (moneyText != null)
                moneyText.text = SaveSystem.LoadMoney().money.ToString(); //Only if you have a text for your money
        }
        else if (item.defaultShip == true || PlayerPrefs.GetInt(boughtItem) == 1)
        {
            SelectShipSkin(item);
        }
    }
    private void SelectShipSkin(Item selectedItem)
    {
        foreach(Item item in items)
        {
            string boughtItem = item.shopItem.itemName;
            if (item.defaultShip == false && PlayerPrefs.GetInt(boughtItem) == 0)
            {
                cards[item.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Buy";
            }
            else if (item.defaultShip == true || PlayerPrefs.GetInt(boughtItem) == 1)
            {
                cards[item.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Select";
                PlayerPrefs.SetInt(item.shopItem.id.ToString(), 0);
            }
        }
        cards[selectedItem.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
        PlayerPrefs.SetInt(selectedItem.shopItem.id.ToString(), 1);
        gameMngr.SetCurrentShipSprite(selectedItem.shopItem.Image, selectedItem);

    }
    private void SelectSavedSkin()
    {
        foreach (Item item in items)
        {
            if (PlayerPrefs.GetInt(item.shopItem.id.ToString(), item.selectedSkin) == 1)
            {
                //Update button
                cards[item.position].transform.Find("CardButton").GetComponentInChildren<TextMeshProUGUI>().text = "Selected";
                gameMngr.SetCurrentShipSprite(item.shopItem.Image, item);
            }
        }
    }
    //In case the shop has a sell tab
    public void Sell(Item item)
    {
        //The item you have to take as an argument for this function may come from your inventory, just call this function when the sell button is pressed
    }
}
