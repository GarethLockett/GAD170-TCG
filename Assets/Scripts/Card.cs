using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Name: Card
	Author: Gareth Lockett
	Version: 1.0

    Description: Parent card class. All other types of cards inherit from this class.
 */

public abstract class Card : MonoBehaviour
{
    // Properties
    protected GameManager gameManager;

    public string cardName;         // Name text displayed on card.
    public string flavorText;       // Description text displayed on card.

    protected CardTexts cardTexts;

    // Methods
    private void Awake()
    {
        // Automatically get the gameManager as soon as this card is instantiated.
        this.gameManager = GameObject.FindObjectOfType<GameManager>();

        // Populate card name and flavor texts.
        this.cardTexts = this.GetComponentInChildren<CardTexts>();
        if( this.cardTexts != null )
        {
            this.cardTexts.cardNameText.text = this.cardName;
            this.cardTexts.flavorText.text = this.flavorText;
        }
        else{ Debug.LogWarning( "Card has no CardTexts children! " +this.cardName ); }
    }

    public void SetCardColor( Color color ) // Convenience method for setting a card's tint color.
    {
        this.gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    // Special 'abstract' method (eg MUST be overidden in inheriting child classes!) Called from GameManager.StartBattle() for each card in play.
    public abstract void StartBattling();

    // Special 'abstract' method (eg MUST be overidden in inheriting child classes!) Called from GameManager.EndBattle() for each card in play to resolve if this card should be removed at the end of a battle.
    public abstract void RemoveCardCheck();
}
