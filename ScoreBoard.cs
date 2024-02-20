using BepInEx;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel.Design;
using GorillaNetworking;

namespace ScoreboardUtils
{
	[BepInPlugin("Lofiat.ScoreBoardUtils", "ScoreBoardUtils", "1.0.0")]
	public class ScoreBoard : BaseUnityPlugin
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

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void Update()
        {
            if (GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard/Board Text") != null && !ran)
            {
                OnGameInitialized();
                ran = true;
            }
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
                ScoreBoardTextComp.text = ScoreBoardText;
                ScoreBoardTextComp.supportRichText = true;
                ScoreBoardGen();
            }
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
                    return "<color=#" + colorBuffer + ">" + player.NickName + "</color>";
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
                    scoreBoardText = scoreBoardText + "\n" + NormalizeName(true, true, true, GetPlayerColorString(player.UserId));
                }
                else
                {
                    scoreBoardText = scoreBoardText + "\n" + NormalizeName(true, true, true, player.NickName);
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

            return $" {text}"; //Space for fixing the "offset"
        }
    }
}
