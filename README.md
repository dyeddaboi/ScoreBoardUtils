# ScoreboardUtils #
A universal way to change ScoreBoard name Colors!
## Usage ##
Simply add as a dependency (or perferably add as an embedded resource. No credit required!), and it's ready for use.
I encourage making this an embedded resource instead of a reference, no one likes when they have to download external libraries.
## How to change name colors ##
Just call the method, "SetNameColorFromID(playerID, "5 digit hex code");", if you need to remove a player from the color list, just call "RemoveNameColorFromID(playerID)".
## Making External ScoreBoards ##
If you want to use the base ScoreBoard text, just use "ScoreBoardText", otherwise, you can get the player color by getting their value in the playerColors dictionary. You can Get the full, normalized, colored name by running "GetPlayerColorString(Player type or ID)"
