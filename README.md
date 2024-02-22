# ScoreboardUtils #
A universal way to change ScoreBoard name colors!
## Usage ##
Simply add as a dependency (or perferably add as an embedded resource. No credit required!), and it's ready for use.
I encourage making this an embedded resource instead of a reference, no one likes when they have to download external libraries.
## How to change name colors ##
Just call the method, "SetNameColorFromID("playerID", "6 digit hex code");", if you need to remove a player from the color list, just call "RemoveNameColorFromID("playerID")".
## How to change nicknames ##
Similar to colors, you can change Nicknames using a simple Method, "SetNickNameFromID("playerID", "newnickname")", nicknames are client sided of course and do not need to follow Gorilla Tag's formatting, you remove them the same way you remove colors.
## Normalizing Text ##
All text can be put into leaderboard format by calling "NormalizeName(isUpperCase? isTruncated? addBadNameFilter? "text")", the first 3 bools can be used to configure the way that the text is normalized.
## Making External ScoreBoards ##
If you want to use the base ScoreBoard text, just use "ScoreBoardText", otherwise, you can get the player color by getting their value in the playerColors dictionary. You can Get the full, normalized, colored name by running "GetPlayerColorString(Player type or ID)"
