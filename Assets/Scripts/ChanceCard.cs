using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Name: ChanceCard
	Author: Gareth Lockett
	Version: 1.0

    Description: Parent class of special 'chance' cards which modify a character between battles with other characters.
 */

public abstract class ChanceCard : Card
{
    public string chanceText;   // Will be shown on UI instead of usual battle winner.
    
    public Color colorCharacterCard = Color.white;    // Color the other character card when executing this chance card.

    private void Start()
    {
        // Start the battle once this card has been instantiated.
        this.gameManager.StartBattle();
    }

    public override void StartBattling()
    {
        // Check if all cards in play are ChanceCards, if so immediately destroy chance cards start a new battle.
        Card[] allCardsInPlay = GameObject.FindObjectsOfType<Card>();
        bool hasCharacterCardInPlay = false;
        for( int i=0; i<allCardsInPlay.Length; i++ ){ if( allCardsInPlay[ i ] as CharacterCard != null ){ hasCharacterCardInPlay = true; break; } }
        if( hasCharacterCardInPlay == false )
        {
            for( int i = 0; i < allCardsInPlay.Length; i++ ){ DestroyImmediate( allCardsInPlay[ i ].gameObject ); }
            this.gameManager.EndBattle( "" );
            return;
        }

        // Override this method in child classes of ChanceCard.
        this.DoChanceCardEffect();

        // End the battle after the ChanceCard has done it's DoChanceCardEffect() method.
        this.gameManager.EndBattle( this.chanceText );
    }

    protected virtual void DoChanceCardEffect()
    {
        // A dummy method. Child cards of this ChanceCard will override this method to do their own thing.
    }

    public override void RemoveCardCheck()
    {
        // Always remove ChanceCards at the end of a battle!
        DestroyImmediate( this.gameObject );
    }
}
