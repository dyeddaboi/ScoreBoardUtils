﻿using BepInEx;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using GorillaNetworking;
using GorillaGameModes;
using HarmonyLib;
using System.Reflection;

namespace ScoreboardUtils
{
	[BepInPlugin("Lofiat.ScoreBoardUtils", "ScoreBoardUtils", "1.0.0")]
	public class ScoreBoard : BaseUnityPlugin
    {
        public static Dictionary<string, string> playerNickNames = new Dictionary<string, string>();
        public static Dictionary<string, string> playerColors = new Dictionary<string, string>();
        public static List<string> changedPlayers = new List<string>();
        public static GorillaScoreBoard GorillaScoreBoardComp;
        internal static string initialGameMode = "NONE";
        public static GorillaScoreBoard currentScoreBoard;
        internal static string scoreBoardText;
        internal static List<string> gmNames;
        public static string ScoreBoardText;
        internal static string colorBuffer;
        internal static string nameBuffer;
        internal static string tempGmName;
        internal static string gmName;
        internal static bool ran;
        object obj;
        void Start() => new Harmony("Lofiat.ScoreBoardUtils").PatchAll(Assembly.GetExecutingAssembly());

        void Update()
        {
            if (PhotonNetwork.InRoom)
            {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out obj))
            {
                string text = obj as string;
                if (text != null)
                {
                    initialGameMode = text;
                }
            }
            }
        }

        //the only purpose of this is to make this more accesable
        public static void UpdateScoreboard()
        {
            currentScoreBoard.RedrawPlayerLines();
        }

        public static void SetNameColorFromID(string ID, string colorhex)
        {
            if (!playerColors.ContainsKey(ID))
            {
                playerColors.Add(ID, colorhex);
                changedPlayers.Add(ID);
                currentScoreBoard.RedrawPlayerLines();
            }
            else
            {
                playerColors.Remove(ID);
                playerColors.Add(ID, colorhex);
                currentScoreBoard.RedrawPlayerLines();
            }
        }

        public static void RemoveNameColorFromID(string ID)
        {
            playerColors.Remove(ID);
            changedPlayers.Remove(ID);
            currentScoreBoard.RedrawPlayerLines();
        }

        public static void SetNickNameFromID(string ID, string NickName)
        {
            if (!playerNickNames.ContainsKey(ID))
            {
                playerNickNames.Add(ID, NickName);
                changedPlayers.Add(ID);
                currentScoreBoard.RedrawPlayerLines();
            }
            else
            {
                playerNickNames.Remove(ID);
                playerNickNames.Add(ID, NickName);
                currentScoreBoard.RedrawPlayerLines();
            }
        }

        public static void RemoveNickNameFromID(string ID)
        {
            playerNickNames.Remove(ID);
            changedPlayers.Remove(ID);
            currentScoreBoard.RedrawPlayerLines();
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

        public static string GetPlayerNameString(string ID)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == ID)
                {
                    if (playerColors.TryGetValue(player.UserId, out colorBuffer) && playerNickNames.TryGetValue(player.UserId, out nameBuffer))
                    {
                        return "<color=#" + colorBuffer + ">" + nameBuffer + "</color>";
                    }
                    if (playerColors.TryGetValue(player.UserId, out colorBuffer))
                    {
                        return "<color=#" + colorBuffer + ">" + NormalizeName(true, true, true, player.NickName) + "</color>";
                    }
                    if (playerNickNames.TryGetValue(player.UserId, out nameBuffer))
                    {
                        return nameBuffer;
                    }
                }
            }
            return "Couldn't get name string!";
        }

        public static void ScoreBoardGen()
        {
            scoreBoardText = "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + RoomType() + "\n   PLAYER      COLOR   MUTE   REPORT";
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (playerColors.ContainsKey(player.UserId) || playerNickNames.ContainsKey(player.UserId))
                {
                    scoreBoardText = scoreBoardText + "\n" + " " + GetPlayerNameString(player.UserId);
                }
                else
                {
                    scoreBoardText = scoreBoardText + "\n" + " " + NormalizeName(true, true, true, player.NickName);
                }
            }
            ScoreBoardText = scoreBoardText;
        }

        public static string NormalizeName(bool Upper, bool Short, bool BadName, string text)
        {
            if (Short)
            {
                text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
                if (text.Length > 12)
                {
                    text = text.Substring(0, 10);
                }
            }

            if (!GorillaComputer.instance.CheckAutoBanListForName(text) && BadName) //Checks if the name is bypassed
                text = "BADGORILLA";

            if (Upper)
                text = text.ToUpper();

            return text;
        }
    }
}
