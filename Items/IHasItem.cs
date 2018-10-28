// Interface for anything that can hold items.
using System.Collections.Generic;

public interface IHasItem{

  bool HasItem(string item);
  string ItemInfo();
  /* Returns quantity of overflow, 0 if accepted full quantity. */
  bool ReceiveItem(Item item);
  Item PrimaryItem();
  int ItemCount();
  List<ItemData> GetAllItems();
}
