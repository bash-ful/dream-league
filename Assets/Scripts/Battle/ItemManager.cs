using System.Collections.Generic;

[System.Serializable]
public class Effect
{
    public string type;
    public float value;
}

[System.Serializable]
public class Item
{
    public string name;
    public string info;
    public List<Effect> effects;
}

[System.Serializable]
public class ItemList
{
    public List<Item> itemList;
}
