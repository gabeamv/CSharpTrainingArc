# Card Games

**Overview**<br>
The user chooses what type of card game they want to play. So far the user can only
choose to play Blackjack, however more card games such as Solitaire may be implemented
in the future.

**Learning Objectives**<br>
Was reading chapters 3 and 4 while implementing this project, however, a lot of 
the concepts that was implemented were more so from chapters 5 and 6. Concepts
from chapters 3 and 4 that were implemented include:
- enumerations to represent card suits and ranks
- null checking
- tuples for representing the user and dealers hands (score for ACE = 11 or 1)

Concepts from chapters 5 and 6 that I have not read about yet but implemented include:
- encapsulation of game logic for Cards, Decks, and other classes
- inheritance to represent different types of card games
- interfaces to represent functionality that all card games should have

**Extra Concepts Implemented**<br>
- Fisher-Yates shuffle algorithm to shuffle the deck of cards
- implementation of a basic Blackjack game
- implementaion of the IEnumerable interface to allow for easy iteration of a deck of cards

**Game Flow**<br>
The user is prompted to choose a card game to play. If the user chooses Blackjack, 
the game starts. The user is dealt two cards and the dealer is dealt two cards. 
The user can choose to hit or stand. The user can keep hitting as long as
they want until they either bust, get 21, or choose to stand. When the user's turn
is finished, the dealer's turn begins and they keep hitting until they legally 
have a hand greater than the user's hand, or they bust. If the user gets 21, or the 
dealer busts, the user wins.

**Improvements To Be Made Of The Current Program**
- Implement a more realistic shuffle algorithm that is closely consistent with casinos
- After every round of Blackjack, the user and dealer's cards are put back into the deck and reshuffled. In real life, they don't shuffle the deck after every round. Implement it in such a way so that it may utilize another suffled deck once the current deck being used runs out.
- Implement a visual representation of the cards and the game. Some string representation.
- Unimplemented overriden methods from the BaseCardGame class in the Blackjack class. Implement better design decisions.

**Possible Future Additions**
- More card games such as Solitaire, Poker, etc.
- GUI using Windows Presentation Foundation (WPF)
- Betting system
- Backend server to allow for multiplayer games
