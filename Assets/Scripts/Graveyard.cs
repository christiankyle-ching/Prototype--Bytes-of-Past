﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graveyard : MonoBehaviour
{
    private Stack<CardData> cards;

    private Deck deck;

    private void Awake()
    {
        this.cards = new Stack<CardData>();
        this.deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<Deck>();
    }

    public void AddCard(CardData cardData)
    {
        cards.Push(cardData);

        Debug.Log("Graveyard: " + cards.Count);
    }

    public void PushAllToDeck()
    {
        Debug.Log("Graveyard to Deck: " + cards.Count);
        foreach (CardData cardData in cards)
        {
            deck.AddCard(cardData);
        }
    }
}
