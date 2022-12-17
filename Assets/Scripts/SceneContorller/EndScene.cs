using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndScene : MonoBehaviour
{
    public TextMeshProUGUI scoreText, gameLengthText, killsText, spellsText, totalItemsObtainedText, uniqueItemsObtainedText, difficultyText;
    // Start is called before the first frame update
    private string[] difficultTxt = new string[4]{"Easy"," Normal", "Hard", "Extreme"};
    void Start()
    {
        // update the text based on the playerstats
        scoreText.text = "Score: " + PlayerStats.Score.ToString();
        gameLengthText.text = "Game Length: " + PlayerStats.GameLength.ToString();
        killsText.text = "Monster kills: " + PlayerStats.MonsterKills.ToString();
        spellsText.text = "Spells used: " + PlayerStats.SpellsUsed.ToString();
        // deletedText.text = "Items deleted: " + PlayerStats.ItemsDeleted.ToString();
        totalItemsObtainedText.text = "Total Items Obtained: " + PlayerStats.TotalItemsObtained.ToString();
        uniqueItemsObtainedText.text = "Unique Items Obtained: " + PlayerStats.UniqueItemsObtained.ToString();

        
        difficultyText.text = "Difficulty: " + difficultTxt[PlayerStats.Difficulty];

    }
}
