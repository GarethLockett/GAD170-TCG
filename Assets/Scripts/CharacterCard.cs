using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Name: CharacterCard
	Author: Gareth Lockett
	Version: 1.0

    Description: Character card that is inherited from parent Card class.
 */

 [ RequireComponent( typeof( SpriteRenderer ) ) ]
public class CharacterCard : Card
{
    // Enumerators
    public enum CharacterType{ angel, demon, hero, monster }

    // Custom data structures
    [ System.Serializable ] // This allows Stats to be seen and edited in the Unity inspector.
    public struct Stats
    {
        public float power;                 // Power stat of a character (eg power is what does damage)
        public float health;                // Health stat of this character (eg when a card has <=0 health it loses)
        public float timeBetweenAttacks;    // Time Between Attacks stat of this character.
    }

    // Properties
    public CharacterType type;                      // Type of character card. NOTE: Not used at the moment but could be implemented for ChanceCards etc
    public Stats stats;                             // Base stats for this character (eg Without any ability modifications)
    public Color damageotherCardColor = Color.red;  // Color to tint other character cards when dealing them damage.

    // Methods
    private void Start()
    {
        // Populate card stats texts.
        this.UpdateCardStatsTexts();
    }

    public void UpdateCardStatsTexts()
    {
        if( this.cardTexts != null )
        {
            this.cardTexts.powerText.text = this.stats.power.ToString();
            this.cardTexts.healthText.text = this.stats.health.ToString();
            this.cardTexts.timeText.text = this.stats.timeBetweenAttacks.ToString();
        }
    }

    public override void StartBattling()
    {
        // Start a 'coroutine'. These are special methods which can run automatically in the background.
        StartCoroutine( this.DoDamage() );
    }

    private IEnumerator DoDamage()
    {
        // Get all cards currently in play.
        Card[] allCardsInPlay = GameObject.FindObjectsOfType<Card>();

        // This while loop will run FOREVER until this coroutine is stopped.
        while( true )
        {
            // Wait so many seconds before this character attacks (again)
            yield return new WaitForSeconds( this.stats.timeBetweenAttacks );

            // Tell other card(s) to take damage from this card.
            for( int i=0; i<allCardsInPlay.Length; i++ )
            {
                CharacterCard characterCard = allCardsInPlay[ i ] as CharacterCard;

                // If characterCard is null then is a non-character so don't tell it to take damage.
                if( characterCard == null ){ continue; }

                // Don't damage self!
                if( characterCard == this ){ continue; }

                // Tell other character that this character wants to damage it.
                characterCard.TakeDamage( this );
            }
        }
    }

    public void TakeDamage( CharacterCard characterCardDoingDamage )
    {
        // Do damage to this character's health equal to characterCardDoingDamage power stat.
        this.stats.health -= characterCardDoingDamage.stats.power;
        if( this.stats.health < 0f ) { this.stats.health = 0f; } // Clamp health to greater than zero for safety?

        // Update stats text
        this.UpdateCardStatsTexts();

        // Check if damage has resulted in health been zero (AKA killed)
        if( this.stats.health == 0f )
        {
            Debug.Log( characterCardDoingDamage.cardName +" defeated " + this.cardName );

            // Set defeated characters card color tint to gray.
            this.SetCardColor( Color.gray );
            this.StopAllCoroutines();

            // Reset the winner character color tint back to white.
            characterCardDoingDamage.SetCardColor( Color.white );
            characterCardDoingDamage.StopAllCoroutines();

            // Get gameManager to show the battle winner text.
            this.gameManager.EndBattle( characterCardDoingDamage.cardName + " defeated " + this.cardName );
        }
        else
        {
            Debug.Log( characterCardDoingDamage.cardName + " did " + characterCardDoingDamage.stats.power + " damage to " + this.cardName );

            // Start the damage color fading coroutine.
            StartCoroutine( this.DamageColorCharacter( Color.red ) );
        }
    }

    private IEnumerator DamageColorCharacter( Color color )
    {
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();    // Get the SpriteRenderer of this card so can update the color tint.
        this.SetCardColor( color );                                             // Set the color tint.
        while( spriteRenderer.color != Color.white )                            // Continue to run the coroutine loop until color tint is back to white.
        {
            this.SetCardColor( Color.Lerp( spriteRenderer.color, Color.white, Time.deltaTime * 2f ) );  // Fade the color tint back to white.
            yield return new WaitForEndOfFrame();
        }
    }

    public override void RemoveCardCheck()
    {
        // Only remove a CharacterCard if health is 0 at the end of a battle.
        if( this.stats.health == 0f ){ DestroyImmediate( this.gameObject ); }
    }

}
