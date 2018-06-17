// Interface for anything that can be used. 
interface IUse {

	void Use(Item.Uses use, bool released = false);
  
  bool IsBusy();
}
