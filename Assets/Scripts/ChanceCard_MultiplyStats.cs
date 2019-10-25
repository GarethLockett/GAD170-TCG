using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Name: ChanceCard_MultiplyStats
	Author: Gareth Lockett
	Version: 1.0

    Description: This 'MultiplyStats' ChanceCard will multiply the power, health, and/or speed stats of all characters in play.
                 Could be used for a variety of ChanceCards (eg increase/decrease power, health, timeBetweenAttacks)
 */

public class ChanceCard_MultiplyStats : ChanceCard
{
    public float powerModifier = 1f;
    public float healthModifer = 1f;
    public float timeBetweenAttacksModifier = 1f;
   
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
                // Multiply stats.
                characterCard.stats.power *= this.powerModifier;
                characterCard.stats.health *= this.healthModifer;
                characterCard.stats.timeBetweenAttacks *= this.timeBetweenAttacksModifier;

                // Clamp stats.
                characterCard.stats.power = Mathf.Max( characterCard.stats.power, 0f );
                characterCard.stats.health = Mathf.Max( characterCard.stats.health, 0f );
                characterCard.stats.timeBetweenAttacks = Mathf.Max( characterCard.stats.timeBetweenAttacks, 0.1f ); // Leave a little time between attacks?

                // Set card effect color.
                characterCard.SetCardColor( this.colorCharacterCard );
            }
        }
    }
    
}
