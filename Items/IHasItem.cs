using System.Collections.Generic;

public interface IHasItem{

  bool HasItem(string item);
  string ItemInfo();
  /* Returns quantity of overflow, 0 if accepted full quantity. */
  int ReceiveItem(Item item);


}
