using HarmonyLib;

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
            __instance.boardText.supportRichText = true;
        }
    }
}
