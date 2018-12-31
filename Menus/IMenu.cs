public interface IMenu {
	void Init(float minX, float minY, float maxX, float maxY);
	void Resize(float minX, float minY, float maxX, float maxY);
	bool IsSubMenu(); // Returns true if this menu cares about the scales passed.
	void Clear();
}