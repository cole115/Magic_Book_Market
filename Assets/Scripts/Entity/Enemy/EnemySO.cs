using System.Collections.Generic;
using UnityEngine;

public class EnemySO : ScriptableObject
{
    public string MonsterID;
    public string MonsterName;
    public int Hp;
    public int Atk;
    public int Def;
    public List<string> StageList;
    public int GetGold;
}
