using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Name: ChanceCard_Death
	Author: Gareth Lockett
	Version: 1.0

    Description: This 'Death' ChanceCard will set any character card's health to 0.
 */

public class ChanceCard_Death : ChanceCard
{    
    protected override void DoChanceCardEffect()
    {
        // Get all cards currently in play.
        Card[] allCardsInPlay = GameObject.FindObjectsOfType<Card>();

        // Set health to 0 on each (If a character card)
        for( int i=0; i<allCardsInPlay.Length; i++ )
        {
            CharacterCard characterCard = allCardsInPlay[ i ] as CharacterCard;
            if( characterCard != null )
            {
                characterCard.stats.health = 0f;
                characterCard.SetCardColor( this.colorCharacterCard );
                characterCard.UpdateCardStatsTexts();
            }
        }
    }
    
}
