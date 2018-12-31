/*
  This menu allows the player to view and interact with their inventory.
*/
using Godot;
using System.Collections.Generic;
using System;

public class InventoryMenu : Container, IMenu {

  public Godot.Button closeButton;
  public Godot.ItemList itemList;
  public Godot.TextEdit itemInfo;
  public Godot.Button useButton;
  public Godot.Button dropButton;
  public Godot.Button stashButton;
  public Godot.TextEdit weightInfo;

  // Category buttons.
  public Godot.Button weaponsButton;
  public Godot.Button apparelButton;
  public Godot.Button aidButton;
  public Godot.Button miscButton;
  public Godot.Button ammoButton;
  public Item.Categories activeFilter;
  public List<ItemData> itemData;
  public int itemSelected;

  public void Init(float minX, float minY, float maxX, float maxY){
    InitControls();
    UpdateFilter(Item.Categories.Weapons);
    ScaleControls();
  }
  
  public void Resize(float minX, float minY, float maxX, float maxY){}

  public bool IsSubMenu(){
    return false;
  }

  public void Clear(){
    this.QueueFree();
  }
  
  public void InitControls(){
    activeFilter = Item.Categories.Weapons;

    closeButton = (Godot.Button)Menu.Button(text : "Close", onClick : Close);
    AddChild(closeButton);

    itemList = new ItemList();
    AddChild(itemList);
    itemList.AllowRmbSelect = true;
    itemList.Connect("item_selected", this, nameof(SelectItem));
    itemList.Connect("item_activated", this, nameof(UseItem));
    itemList.Connect("item_rmb_selected", this, nameof(DropItem));

    itemInfo = (Godot.TextEdit)Menu.TextBox("");
    itemInfo.Readonly = true;
    itemInfo.Hide();
    AddChild(itemInfo);

    useButton = (Godot.Button)Menu.Button(text : "Use", onClick : UseItemAdapter);
    AddChild(useButton);
    useButton.Hide();

    dropButton = (Godot.Button)Menu.Button(text : "Drop", onClick : DropItemAdapter);
    AddChild(dropButton);
    dropButton.Hide();

    stashButton = (Godot.Button)Menu.Button(text : "Stash", onClick : StashItem);
    AddChild(stashButton);
    RefreshStashItemInfo();

    weaponsButton = (Godot.Button)Menu.Button(text : "Weapons", onClick : FilterWeapons);
    AddChild(weaponsButton);

    apparelButton = (Godot.Button)Menu.Button(text : "Apparel", onClick : FilterApparel);
    AddChild(apparelButton);

    aidButton = (Godot.Button)Menu.Button(text : "Aid", onClick : FilterAid);
    AddChild(aidButton);

    miscButton = (Godot.Button)Menu.Button(text : "Misc", onClick : FilterMisc);
    AddChild(miscButton);

    ammoButton = (Godot.Button)Menu.Button(text : "Ammo", onClick : FilterAmmo);
    AddChild(ammoButton);

    weightInfo = (Godot.TextEdit)Menu.TextBox("WeightInfo");
    weightInfo.Readonly = true;
    AddChild(weightInfo);
  }

  public void ScaleControls(){
      Rect2 screen = this.GetViewportRect();
      float width = screen.Size.x;
      float height = screen.Size.y;
      float wu = width / 10; // relative height and width units
      float hu = height / 10;
    	
    	Menu.ScaleControl(closeButton, 2 * wu, 1 * hu, 0, 0);
    	Menu.ScaleControl(itemList, 5 * wu, 4 * hu, 0, 6 * hu);
    	Menu.ScaleControl(itemInfo, 5 * wu, 4 * hu, 5 * wu, 6 * hu);
    	Menu.ScaleControl(useButton, 2 * wu, 0.5f * hu, 5 * wu, 5.5f * hu);
    	Menu.ScaleControl(dropButton, 2 * wu, 0.5f * hu, 7 * wu, 5.5f * hu);
    	Menu.ScaleControl(stashButton,2 * wu, hu, 0, 4.5f * hu);

    	Menu.ScaleControl(weaponsButton, wu, 0.5f * hu, 0, 5.5f * hu);
    	Menu.ScaleControl(apparelButton, wu, 0.5f * hu, wu, 5.5f * hu);
    	Menu.ScaleControl(aidButton, wu, 0.5f * hu, 2 * wu, 5.5f * hu);
    	Menu.ScaleControl(miscButton, wu, 0.5f * hu, 3 * wu, 5.5f * hu);
    	Menu.ScaleControl(ammoButton, wu, 0.5f * hu, 4 * wu, 5.5f * hu);
    	Menu.ScaleControl(weightInfo, 2 * wu, hu, 2 * wu, 4.5f * hu);
  }

  public void RefreshStashItemInfo(){
  	Item item = Session.session.player.PrimaryItem();
  	if(item != null){
  		stashButton.SetText("Arms: " + item.GetInfo());
  	}
  }

  public void Close(){
  	Session.session.player.SetMenuActive(false);
  }

  public void UpdateWeightInfo(int weight){
  	string text = "Weight: " + weight;
  	weightInfo.SetText(text);
  }

  public void RefreshInventory(){
  	itemList.Clear();
  	itemData = Session.session.player.GetAllItems();
  	UpdateWeightInfo(Item.TotalWeight(itemData));

  	itemData = Item.FilterByCategory(itemData, activeFilter);
  	foreach(ItemData item in itemData){
  		itemList.AddItem(item.ToString());
  	}
  	itemSelected = -1;
  	if(itemInfo != null){
  		itemInfo.Hide();
  	}
  	if(useButton != null){
  		useButton.Hide();
  	}
  	if(dropButton != null){
  		dropButton.Hide();
  	}
  }

  public void SelectItem(int index){
  	GD.Print("Item " + index + " selected");
  	itemSelected = index;
  	UpdateItemInfo();
  }

  public void UpdateItemInfo(){
  	if(itemSelected == -1){
  		return;
  	}
  	itemInfo.Show();
  	useButton.Show();
  	dropButton.Show();
  	ItemData item = itemData[itemSelected];
  	string text = item.GetInfo() + ":\n";
  	text += item.GetMoreInfo();
  	itemInfo.SetText(text);
  }

  public void UseItemAdapter(){
  	UseItem(itemSelected);
  }

  public void UseItem(int selected){
    if(activeFilter == Item.Categories.Misc  || activeFilter == Item.Categories.Ammo){
      GD.Print("Nothing to do with these.");
      return;
    }
    ItemData dat = itemData[selected];
    Actor player = Session.session.player;
    int playerItemIndex = player.IndexOf(dat.type, dat.name);
    switch(activeFilter){
      case Item.Categories.Weapons:
        player.EquipItem(playerItemIndex); 
        break;
      case Item.Categories.Apparel: 
        player.EquipItem(playerItemIndex);
        break;
      case Item.Categories.Aid:
        ItemData retrieved = player.RetrieveItem(playerItemIndex);
        IConsume consumable = Item.FromData(retrieved) as IConsume;
        if(consumable != null){
          consumable.Consume(player);
        }
        break;
    }

    
    RefreshInventory();
    RefreshStashItemInfo();
  }

  public void DropItemAdapter(){
  	DropItem(itemSelected, new Vector2());
  }
  public void DropItem(int selected, Vector2 atPosition){
    GD.Print("Dropped " + selected);
    ItemData dat = itemData[selected];
    Actor player = Session.session.player;
    int playerItemIndex = player.IndexOf(dat.type, dat.name);
    player.DiscardItem(playerItemIndex);
    
    RefreshInventory();
  }

  public void StashItem(){
  	GD.Print("Stashing item");
  	Session.session.player.StashItem();
  	RefreshInventory();
  	RefreshStashItemInfo();
  }

  public void FilterWeapons(){
  	UpdateFilter(Item.Categories.Weapons);
  }

  public void FilterApparel(){
  	UpdateFilter(Item.Categories.Apparel);
  }

  public void FilterAid(){
  	UpdateFilter(Item.Categories.Aid);
  }

  public void FilterMisc(){
  	UpdateFilter(Item.Categories.Misc);
  }

  public void FilterAmmo(){
  	UpdateFilter(Item.Categories.Ammo);
  }

  public void UpdateFilter(Item.Categories category){
  	activeFilter = category;
  	weaponsButton.Disabled = false;
  	apparelButton.Disabled = false;
  	aidButton.Disabled = false;
  	miscButton.Disabled = false;
  	ammoButton.Disabled = false;

  	switch(activeFilter){
  		case Item.Categories.Weapons:
  			weaponsButton.Disabled = true;
  			useButton.SetText("Equip");
  			break;
  		case Item.Categories.Apparel:
  			apparelButton.Disabled = true;
  			useButton.SetText("Equip");
  			break;
  		case Item.Categories.Aid:
  			aidButton.Disabled = true;
  			useButton.SetText("Use");
  			break;
  		case Item.Categories.Misc:
  			miscButton.Disabled = true;
  			useButton.SetText("Use");
  			break;
  		case Item.Categories.Ammo:
  			ammoButton.Disabled = true;
  			useButton.SetText("Select");
  			break;
  	}

  	RefreshInventory();
  }
}