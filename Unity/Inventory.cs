using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	ItemDB database;
	GameObject inventoryPanel;
	GameObject slotPanel;
	public GameObject inventorySlot;
	public GameObject inventoryItem;
	private bool showInventory;
	GameObject inventoryCanvas;
	int slotAmount;
	public List<Item> items = new List<Item> ();
	public List<GameObject> slots = new List<GameObject> ();
	private float mouseX, mouseY;

	void Start()
	{
		PlayerPrefs.SetInt ("drag", 0);
		database = GetComponent<ItemDB>();
		inventoryCanvas = GameObject.Find ("Canvas");
		slotAmount = 16;
		inventoryPanel = GameObject.Find ("Inventory Panel");
		slotPanel = inventoryPanel.transform.FindChild ("Slot Panel").gameObject;
		for (int i=0; i<slotAmount; i++) {
			items.Add(new Item());
			slots.Add(Instantiate(inventorySlot));
			slots[i].GetComponent<Slot>().id = i;
			slots[i].transform.SetParent(slotPanel.transform);
		}
		AddItem (0);
		AddItem (1);
		AddItem (1);
		AddItem (1);
		AddItem (1);
		AddItem (1);
		AddItem (1);
		RemoveItem (1);
		AddItems (4,3);

		AddItem (3);
		AddItem (2);

	}

	void Update()
	{
		if (Input.GetButtonDown("Inventory") && PlayerPrefs.GetInt("drag")==0)
		{
			showInventory = !showInventory;
			inventoryCanvas.SetActive(showInventory);
		}
		if(Input.GetKeyDown(KeyCode.Q))
		{
			//RemoveItemsAll(0);
			//AddItem (xxx);
			//xxx++;
		}
	}

	private void OnGUI()
	{
		if (showInventory) {
			mouseX = Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position).x + 180;//Event.current.mousePosition.x; //test Positions to appear near mouse
			mouseY = Camera.main.WorldToScreenPoint (GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ().position).y + 200;//Event.current.mousePosition.y;
			GameObject.Find ("Inventory Panel").transform.position = new Vector3 (mouseX, mouseY, 0);
		}
	}


	public void AddItem(int id)
	{
		Item itemToAdd = database.FetchItemByID (id);
		if (itemToAdd.Stackable && items.Contains (itemToAdd)) {
			for (int i=0; i<items.Count; i++) { //TODO: edit this shit
				if (items [i].ID == id) {
					ItemData data = slots [i].transform.GetChild (0).GetComponent<ItemData> ();
					data.amount++;
					data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
					break;
				}
			}
		} else {
			for (int i=0; i<items.Count; i++) {
				if(items[i].ID == -1)
				{
					items[i] = itemToAdd;
					GameObject itemObj = Instantiate(inventoryItem);
					itemObj.GetComponent<ItemData>().item = itemToAdd;
					itemObj.GetComponent<ItemData>().slot = i;
					itemObj.transform.SetParent(slots[i].transform);
					itemObj.transform.position = Vector2.zero;
					itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
					itemObj.name = itemToAdd.Title;
					break;
				}
			}
		}
	}

	public void AddItems(int id, int amount)
	{
		Item itemToAdd = database.FetchItemByID (id);
		if (itemToAdd.Stackable && items.Contains (itemToAdd)) {
			for (int i=0; i<items.Count; i++) { //TODO: edit this shit
				if (items [i].ID == id) {
					ItemData data = slots [i].transform.GetChild (0).GetComponent<ItemData> ();
					data.amount+=amount;
					data.transform.GetChild (0).GetComponent<Text> ().text = data.amount.ToString ();
					break;
				}
			}
		} else {
			for (int i=0; i<items.Count; i++) {
				if(items[i].ID == -1)
				{
					items[i] = itemToAdd;
					GameObject itemObj = Instantiate(inventoryItem);
					itemObj.GetComponent<ItemData>().item = itemToAdd;
					itemObj.GetComponent<ItemData>().slot = i;

					itemObj.transform.SetParent(slots[i].transform);
					itemObj.transform.position = Vector2.zero;
					itemObj.GetComponent<Image>().sprite = itemToAdd.Sprite;
					itemObj.name = itemToAdd.Title;
					itemObj.GetComponent<ItemData>().amount = amount;
					itemObj.GetComponent<ItemData>().transform.GetChild (0).GetComponent<Text> ().text = itemObj.GetComponent<ItemData>().amount.ToString ();
					break;
				}
			}
		}
	}

	private int RemoveAtPos(int pos, Item itemToRemove)
	{
		if (pos != -1)
		{
			if (items[pos].Stackable)
			{
				ItemData data = slots[pos].transform.GetComponentInChildren<ItemData>();
				data.amount--;
				if (data.amount == 0)
				{
					items[pos] = new Item();
					Transform t = slots[pos].transform.GetChild(0);
					Destroy(t.gameObject);
					return 0;
				}
				else
				{
					if(data.amount == 1)
						data.transform.GetComponentInChildren<Text>().text = "";
					else
						data.transform.GetComponentInChildren<Text>().text = data.amount.ToString();
					return data.amount;
				}
			}
			else
			{
				items[pos] = new Item();
				Transform t = slots[pos].transform.GetChild(0);
				Destroy(t.gameObject);
				return 0;
			}
		}
		return -1;
	}

	int ItemCheck(Item item) //TODO: edit this
	{
		for(int i = 0; i < items.Count; i++)
		{
			if (items[i].ID == item.ID)
				return i;
		}
		return -1;
	}

	public int RemoveItem(int id)
	{
		Item itemToRemove = database.FetchItemByID(id);
		int pos = ItemCheck(itemToRemove);
		return (RemoveAtPos(pos, itemToRemove));
	}
	
	public int RemoveUniqueItem(int uniqueId, int itemId)
	{
		Item itemToRemove = database.FetchItemByID(itemId);
		int pos = UniqueItemCheck(uniqueId);
		return (RemoveAtPos(pos, itemToRemove));
	}

	int UniqueItemCheck(int id)
	{
		GameObject invSlots = GameObject.Find("Slot Panel");
		foreach (Transform child in invSlots.transform) 
		{
			try
			{
				if (child.transform.GetChild(0).GetInstanceID() == id)
					return child.GetComponent<Slot>().id;
			}
			catch
			{}
		}
		return -1;
	}
}
