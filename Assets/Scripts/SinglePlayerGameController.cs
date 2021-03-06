using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SinglePlayerGameController : MonoBehaviour
{
    private int startingCardsCount = 4;
    private int playerLives = 5;

    // Scene References
    private Transform player;
    private PlayerStats playerStats;

    private DropZone timeline;
    private Transform timelineCardContainer;

    private Deck deck;
    private Graveyard graveyard;

    private Transform livesContainer;
    [SerializeField]
    private GameObject lifePrefab;

    private SinglePlayerMenuManager menuManager;

    // GAME: Start
    private void Awake()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").transform;
        this.playerStats = player.gameObject.GetComponent<PlayerStats>();

        GameObject _timelineObj = GameObject.FindGameObjectWithTag("Timeline");
        this.timeline = _timelineObj.GetComponent<DropZone>();
        this.timelineCardContainer = _timelineObj.transform.GetChild(0);

        this.deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
        this.graveyard = GameObject.FindGameObjectWithTag("Graveyard").GetComponent<Graveyard>();

        this.livesContainer = GameObject.FindGameObjectWithTag("PlayerLives").transform;

        this.menuManager = GameObject.FindGameObjectWithTag("Menu").GetComponent<SinglePlayerMenuManager>();
    }

    void Start()
    {
        InitLives();
        DrawCards();
    }

    void InitLives()
    {
        for (int i = 0; i < playerLives; i++)
        {
            Instantiate(lifePrefab, livesContainer);
        }
    }

    void DrawCards()
    {
        deck.GiveCard(player, startingCardsCount);
    }

    // Game Actions
    public void DecreaseLife()
    {
        playerLives--;

        int lastIndex = livesContainer.transform.childCount - 1;

        if (lastIndex > 0)
        {
            Destroy(livesContainer.transform.GetChild(lastIndex).gameObject);
        }
        else
        {
            menuManager.EndGame(playerStats);
        }
    }

    // Game Flow Functions
    public void HandleDropInTimeline(Card droppedCard, int dropPos)
    {
        if (IsDropValid(droppedCard, dropPos))
        {
            timeline.AcceptDrop(droppedCard);

            playerStats.CorrectDrop();

            if (IsHandEmpty()) menuManager.EndGame(playerStats);
        }
        else
        {
            HandleInvalidDrop(droppedCard);

            playerStats.IncorrectDrop();
        }
    }

    private bool IsHandEmpty()
    {
        // GetChild is necessary since cards are placed in a container inside a DropZone
        return player.GetChild(0).childCount <= 0;
    }

    private void HandleInvalidDrop(Card droppedCard)
    {
        DecreaseLife();

        // Add card first in graveyard, so that
        // if there's no card left in deck, something in graveyard
        // can be pulled by the deck
        // then give it back again
        graveyard.AddCard(droppedCard.CardData);
        Destroy(droppedCard.gameObject);

        try
        {
            deck.GiveCard(player, 1);
        }
        catch (InvalidOperationException)
        {
            // if deck is empty
            graveyard.PushAllToDeck();
            deck.GiveCard(player, 1);
        }

    }

    private bool IsDropValid(Card droppedCard, int dropPos)
    {
        Card[] timelineCards = timelineCardContainer.GetComponentsInChildren<Card>();

        // Shorten code
        int yearBefore, cardYear, yearAfter;

        // DEBUG: Use Card.CardData.Year on implementation, replace all randomYear
        cardYear = droppedCard.randomYear;
        try
        {
            yearBefore = timelineCards[dropPos - 1].randomYear;
        }
        catch
        {
            yearBefore = int.MinValue;
        }

        try
        {
            yearAfter = timelineCards[dropPos].randomYear;
        }
        catch
        {
            yearAfter = int.MaxValue;
        }

        //Debug.Log(yearBefore + ", " + cardYear + ", " + yearAfter);

        return (yearBefore <= cardYear && cardYear <= yearAfter);
    }

}
