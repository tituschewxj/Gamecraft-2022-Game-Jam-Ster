using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static int Score {get; set;}
    public static int GameLength {get; set;}
    public static int MonsterKills {get; set;}
    public static int SpellsUsed {get; set;}
    public static int ItemsDeleted {get; set;}
    public static int TotalItemsObtained {get; set;} // updated elsewhere when item is picked up
    public static int UniqueItemsObtained {get; set;}
    public static int Difficulty {get; set;}

}
