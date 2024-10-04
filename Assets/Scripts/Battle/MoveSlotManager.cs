using TMPro;
using UnityEngine;

[System.Serializable]
public class MoveSlot
{
    public Move move;
    public TMP_Text moveName;
}
[System.Serializable]
public class MoveSlots
{
    public MoveSlot[] moveSlots;
}
public class MoveSlotManager : MonoBehaviour
{

    public MoveSlots moveSlots;
    public MasterScript masterScript;

    void Start()
    {

        Init();
    }

    private void Init()
    {
        DataSaver.Instance.LoadDataFn();
        Move move;
        for (int i = 0; i < 4; i++)
        {
            move = MoveManager.Instance.GetMoveFromID(masterScript.player.GetMoveID(i));
            moveSlots.moveSlots[i].move = move;
            moveSlots.moveSlots[i].moveName.text = move.name;
        }
    }
}
