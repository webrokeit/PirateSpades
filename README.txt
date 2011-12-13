Pirate Spades

Authors:
» Helena Charlotte Lyn Krüger (hclk@itu.dk) - Graphics + XNA
» Morten Chabert Eskesen (mche@itu.dk) - XNA + graphics
» Andreas Hallberg Kjeldsen (ahal@itu.dk) - GameLogic and network

Abstract:
Card game for two players and up. You can play for fun, for money or
for the glory. A typical game consists of 20 rounds, first round 
each player is dealt 10 cards, next round each player is dealt 9 
cards and so forth until 1 card is being dealt. Then you go from 1 
card, to 2 cards and up to 10 cards again, then the game is over.
Each round you have to make a prediction as to how many tricks you 
will get during the round. When everyone has made a prediction, 
they're revealed.
When all cards have been played, each player is given 10 points plus
one additional point per trick if they guessed correctly, otherwise
they get subtracted one point per trick they're wrong.
The player with the most points at the end of the game is the winner.
Ace is highest, 2 is lowest. You have to follow suit when possible.
Spades are trump (Ace of Spades is a sure trick etc.).

Requirements (Mandatory):
» Must model the game "Pirat Bridge".
» Must involve at least two players.
» Must have an interactive user interface.
» Must store player state between games.
» Must accurately model players and non-player/non-card entities 
  relevant to the game.

Requirements (Secondary):
» Should be playable over network.
» Should support different rulesets (only minor modifications to the 
  main ruleset).
