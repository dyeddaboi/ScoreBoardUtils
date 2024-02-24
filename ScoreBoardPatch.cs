using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScoreboardUtils
{
    [HarmonyPatch(typeof(GorillaScoreBoard), "RedrawPlayerLines")]
    class ScoreBoardPatch
    {
        static void Postfix(GorillaScoreBoard __instance)
        {
            ScoreBoard.currentScoreBoard = __instance;
            ScoreBoard.ScoreBoardGen();
            __instance.boardText.text = ScoreBoard.ScoreBoardText;
        }
    }
}
