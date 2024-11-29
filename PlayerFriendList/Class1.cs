using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerFriendList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEngine;

    public class FriendListModlet : IModApi
    {
        private const string ModName = "FriendListModlet";
        private const string FriendListFileName = "FriendList.json";
        private string SavePath => Path.Combine(GameIO.GetSaveGameDir(), FriendListFileName);

        public void InitMod(Mod _modInstance)
        {
            ModEvents.GameStartDone.RegisterHandler(OnGameStartDone);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(OnPlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(OnPlayerDisconnected);
        }

        private void OnGameStartDone()
        {
            Log.Out($"[{ModName}] Mod initialized.");
            LoadFriendList();
        }

        private void OnPlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            UpdateFriendList(_cInfo);
        }

        private void OnPlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            UpdateFriendList(_cInfo);
        }

        private void UpdateFriendList(ClientInfo _cInfo)
        {
            Dictionary<string, List<string>> friendList = LoadFriendList();

            if (_cInfo == null || string.IsNullOrEmpty(_cInfo.playerName))
            {
                return;
            }

            List<string> friendsAndAllies = new List<string>();

            // Get friends
            foreach (var friend in GameUtils.GetFriends(_cInfo.playerId))
            {
                friendsAndAllies.Add(friend.Value);
            }

            // Get allies (assuming allies are part of the same party)
            var party = GameManager.Instance.GetPartyByMember(_cInfo.entityId);
            if (party != null)
            {
                foreach (var member in party.MemberList)
                {
                    if (member.entityId != _cInfo.entityId)
                    {
                        string allyName = GameUtils.GetPlayerName(member.entityId);
                        if (!friendsAndAllies.Contains(allyName))
                        {
                            friendsAndAllies.Add(allyName);
                        }
                    }
                }
            }

            friendList[_cInfo.playerName] = friendsAndAllies;

            SaveFriendList(friendList);
        }

        private Dictionary<string, List<string>> LoadFriendList()
        {
            Dictionary<string, List<string>> friendList = new Dictionary<string, List<string>>();
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                friendList = ParseJson(json);
            }
            return friendList;
        }

        private void SaveFriendList(Dictionary<string, List<string>> friendList)
        {
            string json = SerializeToJson(friendList);
            File.WriteAllText(SavePath, json);
            Log.Out($"[{ModName}] Friend list updated and saved.");
        }

        private string SerializeToJson(Dictionary<string, List<string>> friendList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool firstPlayer = true;
            foreach (var player in friendList)
            {
                if (!firstPlayer) sb.Append(",");
                sb.Append($"\"{player.Key}\":[");
                bool firstFriend = true;
                foreach (var friend in player.Value)
                {
                    if (!firstFriend) sb.Append(",");
                    sb.Append($"\"{friend}\"");
                    firstFriend = false;
                }
                sb.Append("]");
                firstPlayer = false;
            }
            sb.Append("}");
            return sb.ToString();
        }

        private Dictionary<string, List<string>> ParseJson(string json)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            json = json.Trim();
            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                json = json.Substring(1, json.Length - 2);
                string[] playerEntries = json.Split(new[] { '}', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in playerEntries)
                {
                    string[] parts = entry.Split(':');
                    if (parts.Length == 2)
                    {
                        string playerName = parts[0].Trim().Trim('"');
                        string friendsJson = parts[1].Trim();
                        if (friendsJson.StartsWith("[") && friendsJson.EndsWith("]"))
                        {
                            friendsJson = friendsJson.Substring(1, friendsJson.Length - 2);
                            List<string> friends = new List<string>(friendsJson.Split(',')
                                .Select(f => f.Trim().Trim('"'))
                                .Where(f => !string.IsNullOrEmpty(f)));
                            result[playerName] = friends;
                        }
                    }
                }
            }
            return result;
        }
    }
}
