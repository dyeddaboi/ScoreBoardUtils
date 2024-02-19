using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ScoreboardUtils;

[assembly: MelonInfo(typeof(ScoreBoard), "ScoreBoardUtils", "1.0.0", "Lofiat")]
[assembly: MelonGame("", "")]

namespace ScoreboardUtils
{

	public class ScoreBoard : MelonMod
    {
        public static Dictionary<Player, string> playerColors = new Dictionary<Player, string>();
        public static GorillaScoreBoard GorillaScoreBoardComp;
        internal static string initialGameMode = "NONE";
        public static GameObject ScoreBoardTextObject;
        internal static Text ScoreBoardTextComp;
        internal static string scoreBoardText;
        internal static List<string> gmNames;
        public static string ScoreBoardText;
        internal static string colorBuffer;
        internal static string tempGmName;
        internal static string gmName;
        internal static bool ran;
        object obj;

        public override void OnLateInitializeMelon()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        public override void OnUpdate()
        {
            if (GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard/Board Text") != null && !ran)
            {
                OnGameInitialized();
                ran = true;
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
            {
                string text = obj as string;
                if (text != null)
                {
                    initialGameMode = text;
                }
            }
            ScoreBoardTextComp.text = ScoreBoardText;
            ScoreBoardTextComp.supportRichText = true;
            ScoreBoardGen();
        }

        internal static void OnGameInitialized()
        {
            ScoreBoardTextObject = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard/Board Text");
            ScoreBoardTextComp = ScoreBoardTextObject.GetComponent<Text>();
            GorillaScoreBoardComp = ScoreBoardTextObject.GetComponent<GorillaScoreBoard>();
        }

        public static void SetNameColorFromID(string ID, string colorhex)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == ID)
                {
                    playerColors.Add(player, colorhex);
                }
            }
        }

        public static void RemoveNameColorFromID(string ID)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == ID)
                {
                    playerColors.Remove(player);
                }
            }
        }

        private static string RoomType()
        {
            gmNames = GameMode.gameModeNames;
            gmName = "ERROR";
            int count = gmNames.Count;
            for (int i = 0; i < count; i++)
            {
                tempGmName = gmNames[i];
                if (initialGameMode.Contains(tempGmName))
                {
                    gmName = tempGmName;
                    break;
                }
            }
            return gmName;
        }

        public static string GetPlayerColorString(Player player)
        {
            playerColors.TryGetValue(player, out colorBuffer);
            return "<color=#" + colorBuffer + ">" + NormalizeName(true, player.NickName) + "</color>";
        }

        public static string GetPlayerColorString(string ID)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == ID)
                {
                    playerColors.TryGetValue(player, out colorBuffer);
                    return "<color=#" + colorBuffer + ">" + NormalizeName(true, player.NickName) + "</color>";
                }
            }
            return "Couldn't find color!";
        }

        public static void ScoreBoardGen()
        {
            scoreBoardText = "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + RoomType() + "\n   PLAYER      COLOR   MUTE   REPORT";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (playerColors.ContainsKey(player))
                {
                    scoreBoardText = scoreBoardText + "\n" + GetPlayerColorString(player);
                }
                else
                {
                    scoreBoardText = scoreBoardText + "\n" + NormalizeName(true, player.NickName);
                }
            }
            ScoreBoardText = scoreBoardText;
        }

        public static string NormalizeName(bool doIt, string text)
        {
            if (doIt)
            {
                text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
                if (text.Length > 12)
                {
                    text = text.Substring(0, 10);
                }
                text = text.ToUpper();
            }
            return text;
        }
    }
}