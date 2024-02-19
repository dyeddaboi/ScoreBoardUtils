﻿using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using ScoreboardUtils;
using Harmony;

[assembly: MelonInfo(typeof(ScoreBoard), "ScoreBoardUtils", "1.0.0", "Lofiat")]
[assembly: MelonGame("", "")]

namespace ScoreboardUtils
{

	public class ScoreBoard : MelonMod
    {
        public static Dictionary<string, string> playerColors = new Dictionary<string, string>();
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
            playerColors.Add(ID, colorhex);
        }

        public static void RemoveNameColorFromID(string ID)
        {
            playerColors.Remove(ID);
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

        public static string GetPlayerColorString(string ID)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.UserId == ID)
                {
                    playerColors.TryGetValue(player.UserId, out colorBuffer);
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
                if (playerColors.ContainsKey(player.UserId))
                {
                    scoreBoardText = scoreBoardText + "\n" + GetPlayerColorString(player.UserId);
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