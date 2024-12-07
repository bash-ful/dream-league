using UnityEngine;
using UnityEngine.UI;

public class MonsterDeckEntry : MonoBehaviour
{
    public Monster monster;
    public MonsterDeckManager monsterDeckManager;
    public int inventoryIndex;

    public SelectedMonsterCard card;


    public void UpdateEntry()
    {
        if (monster == null)
        {
            GetComponent<Button>().image.sprite = null;
            // GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(false);
            ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
            return;
        }
        // if (DataSaver.Instance.IsMonsterEquipped(monster.id))
        // {
        //     GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(true);
        //     int slotIndex = DataSaver.Instance.dts.equippedMonsters.FindIndex(x => x.id == monster.id) + 1;
        //     GetComponentInChildren<TMP_Text>(true).text = slotIndex.ToString();
        // }
        // else
        // {
        //     GetComponentInChildren<TMP_Text>(true).gameObject.SetActive(false);
        // }
        GetComponent<Button>().image.sprite = MonsterManager.Instance.GetMonsterSprite(monster);
        ImageTransparencyScript.UpdateImageTransparency(GetComponent<Button>().image);
        GetComponent<Button>().image.preserveAspect = true;
    }

    public void ClearEntry()
    {
        monster = null;
        UpdateEntry();
    }

    public void OnImagePress()
    {
        card.UpdateCard(inventoryIndex);
        monsterDeckManager.selectedMonsterSlot = inventoryIndex;
    }
}
