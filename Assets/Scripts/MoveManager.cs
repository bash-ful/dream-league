using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    public int id;
    public string name;
    public string type;
    public string elementType;
    public int baseDamage;
    public string info;
    public List<Effect> effects;
    public string moveTypeIconPath;
    public string moveElementTypeIconPath;
    
}

[System.Serializable]
public class MoveList
{
    public List<Move> moveList;
}

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance;
    private MoveList moveList;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();

    }
    private void Init()
    {
        TextAsset MovesJson = Resources.Load<TextAsset>("Json/Moves");

        if (MovesJson != null)
        {
            moveList = JsonUtility.FromJson<MoveList>(MovesJson.text);
            foreach (Move move in moveList.moveList)
            {
                print($"name: {move.name}");
            }
        }
        else
        {
            Debug.LogError("error reading Moves JSON");
        }
    }

    public Move GetMoveFromID(int moveID)
    {
        return moveList.moveList.Find(move => move.id == moveID);
    }

    public List<Move> MoveList
    {
        get { return new List<Move>(moveList.moveList); }
    }

    public Sprite GetMoveTypeIcon(Move move)
    {
        return Resources.Load<Sprite>(move.moveTypeIconPath);
    }
    public Sprite GetMoveElementTypeIcon(Move move)
    {
        return Resources.Load<Sprite>(move.moveElementTypeIconPath);
    }

}
