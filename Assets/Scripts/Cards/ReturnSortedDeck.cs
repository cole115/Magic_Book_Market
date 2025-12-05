using UnityEngine;
using System.Collections.Generic;

public static class ReturnSortedDeck
{
    public static List<GameCard> SortDeck(List<GameCard> deck)
    {
        deck.Sort((a, b) =>
        {
            int idA = int.Parse(a.Card.CardID[^3..]);
            int idB = int.Parse(a.Card.CardID[^3..]);
            return idA.CompareTo(idB);

        });
        return deck;
    }

}
