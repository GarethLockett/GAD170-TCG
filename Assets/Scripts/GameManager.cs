using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

/*
    Name: GameManager
	Author: Gareth Lockett
	Version: 1.0

    Description: Main game manager script. Controls battles and spawning new cards.    
 */

public class GameManager : MonoBehaviour
{
    public List<Card> allCards;

    public Transform cardOnePosition, cardTwoPosition;

    public Button battleButton;
    public GameObject winnerPanel;
    public Text winnerText;

    // Methods
    private void Start()
    {
        // Check to make sure game designer has added cards.
        if( this.allCards == null )
        {
            Debug.LogWarning( "No character cards added in GameManager!" );
            this.enabled = false; return;
        }
        
        // Make sure we get random(ish) values each game by using the current time millisecond as a random seed value.
        Random.InitState( Mathf.RoundToInt( ( Input.mousePosition.x + Input.mousePosition.y ) * 1000f ) );

        // Set up first battle.
        this.CreateNewBattle();
    }

    private void CreateNewBattle()
    {
        // Get all cards currently in play.
        Card[] existingCards = GameObject.FindObjectsOfType<Card>();
        
        // Get the existing cards.
        existingCards = GameObject.FindObjectsOfType<Card>();

        // Check how many character cards are left in allCards. If none then one of the currently existing cards (with health) is the final winner.
        int characterCardCounter = 0;
        for( int i = 0; i < this.allCards.Count; i++ )
        {
            if( this.allCards[ i ] as CharacterCard != null ){ characterCardCounter++; }
        }
        if( characterCardCounter == 0 )
        {
            // There are no characters left to battle so just show the final winner.
            Debug.Log( "There are no characters left to battle. Show final winner!" );
            this.allCards.Clear();
            StartCoroutine( this.ShowBanner( "", 0f ) );
            return;
        }
        
        // Reset card tint color at the start of each battle?
        for( int i = 0; i < existingCards.Length; i++ ){ existingCards[ i ].SetCardColor( Color.white ); }

        // Generate at least 2 cards to battle
        switch( existingCards.Length )
        {
            case 0:
                // No cards are currently in play so get 2 new random cards from allCards.

                // Instatiate first new card
                Card newCard = this.InstantiateRandomCard();

                // First new card in play. Position at cardOnePosition.
                newCard.transform.position = this.cardOnePosition.position;

                // Check there is another character card to add.
                if( this.allCards.Count == 0 )
                {
                    // There are no characters left to battle so just show the final winner.
                    Debug.Log( "There are no characters left to battle. Show final winner!" );
                    StartCoroutine( this.ShowBanner( "", 0f ) );
                    return;
                }

                // Instatiate second new card
                newCard = this.InstantiateRandomCard();

                // First new card in play. Position at cardOnePosition.
                newCard.transform.position = this.cardTwoPosition.position;
                break;

            case 1:
                // There is 1 card already in play so get 1 new random cards from allCards.

                // Instatiate first new card
                Card newSecondCard = this.InstantiateRandomCard();

                // Position newSecondCard at the other card position than the existing card.
                if( existingCards[ 0 ].transform.position == this.cardOnePosition.position )
                {
                    // Other card is at cardOnePosition so move newSecondCard to cardTwoPosition.
                    newSecondCard.transform.position = this.cardTwoPosition.position;
                }
                else
                {
                    // Other card is at cardTwoPosition so move newSecondCard to cardOnePosition.
                    newSecondCard.transform.position = this.cardOnePosition.position;
                }

                break;

            default:
                Debug.LogError( "There are more than 1 card in play?! " +existingCards.Length );
                break;
        }

        // Show 'battle' button when ready to start new battle.
        this.battleButton.gameObject.SetActive( true );
    }

    private Card InstantiateRandomCard()
    {
        //if( this.allCards.Count == 0 ){ return null; }

        // Choose a random card from allCards.
        Card newCardPrefab = this.allCards[ Mathf.FloorToInt( Random.value *( this.allCards.Count -0.1f ) ) ];

        // Instatiate new card into the scene.
        GameObject newCardGO = GameObject.Instantiate( newCardPrefab.gameObject );

        // Remove the prefab card from allCards when instantiated.
        this.allCards.Remove( newCardPrefab );

        // Return the newly instantiated card.
        return newCardGO.GetComponent<Card>();
    }

    public void StartBattle() // 'Battle' button pressed on the UI or triggered if ChanceCard comes into play.
    {
        // Get all cards currently in play.
        Card[] existingCards = GameObject.FindObjectsOfType<Card>();

        if( existingCards.Length != 2 ) { Debug.LogWarning( "Don't have 2 existing cards to battle?! " + existingCards.Length ); }
        else{ Debug.Log( "Starting battle: " +existingCards[ 0 ].cardName +" VS " +existingCards[ 1 ].cardName ); }

        // Call each card's StartBattling() method passing in an array of all existingCards (So that cards can easily get access to other cards)
        foreach( Card card in existingCards )
        {
            card.StartBattling();
        }

        // Hide the 'battle' button while a battle is in progress.
        this.battleButton.gameObject.SetActive( false );
    }

    public void EndBattle( string winnerText )
    {
        // Get all cards currently in play.
        Card[] existingCards = GameObject.FindObjectsOfType<Card>();

        // If there are no existing cards then create a new battle immediately.
        if( existingCards.Length == 0 ) { this.CreateNewBattle(); return; }

        // Make sure all card corountines have stopped so they don't keep doing things after the battle is over.
        for( int i=0; i<existingCards.Length; i++ ){ existingCards[ i ].StopAllCoroutines(); }
        
        // Show the winnerText via a coroutine for 5 seconds.
        StartCoroutine( this.ShowBanner( winnerText, 5f ) );
    }

    private IEnumerator ShowBanner( string winnerText, float numberOfSecondsToShowFor )
    {
        // Show winner banner.
        this.winnerText.text = winnerText;
        this.winnerPanel.SetActive( true );

        yield return new WaitForSeconds( numberOfSecondsToShowFor );

        // Have cards check if they should be removed.
        Card[] existingCards = GameObject.FindObjectsOfType<Card>();
        for( int i = 0; i < existingCards.Length; i++ ){ existingCards[ i ].RemoveCardCheck(); }
        existingCards = GameObject.FindObjectsOfType<Card>();

        if( this.allCards.Count == 0 || winnerText == "" )
        {
            // No more characters to battle.
            this.battleButton.gameObject.SetActive( false );

            // Check for no final winner (Could happen via ChanceCards!)
            if( existingCards.Length == 0 )
            {
                this.winnerText.text = "There are no winners in war!";
            }
            else if( existingCards.Length == 1 )
            {
                // Show the final winner.
                this.winnerText.text = existingCards[ 0 ].cardName + " is the FINAL winner!";

                // Reset card tint color
                existingCards[ 0 ].SetCardColor( Color.white );

                // Center the final winner.
                existingCards[ 0 ].transform.position = new Vector3( 0f, existingCards[ 0 ].transform.position.y, existingCards[ 0 ].transform.position.z );
            }
            else{ Debug.LogWarning( "Incorrect number of existing cards after all characters battled?! " + existingCards.Length ); }
        }
        else
        {
            this.winnerPanel.SetActive( false );

            // Create next battle.
            this.CreateNewBattle();
        }
    }
    
}
