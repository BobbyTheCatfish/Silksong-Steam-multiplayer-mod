using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using SilksongMultiplayer.NetworkData;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace SilksongMultiplayer
{
    

    public static class SilksongMultiplayerAPI
    {
        public static bool startGame = false;
        public static bool enterRoom = false;
        public static bool roomOwner = false;
        public static bool ownTheScene = false;
        public static GameObject RoomManagerObject;
        public static RoomManager RoomManager;
        public static PlayerNetworkSync playerNetworkSync;
        public static EnemyHitEffectsProfile sampleEnemyHitEffectsProfile;
        public static Font savedFont;
        public static bool enablePvP;
        public static float BossHPmultiplier = 1f;
        public static float EnemyHPmultiplier = 0f;

        public static GameObject compassIcon;
        public static GameObject wideCompassIcon;
        public static GameObject Hero_Hornet;

        public static GameObject createLobbyButton;
        public static GameObject inviteLobbyButton;

        public static ManualLogSource Logger;

        public static ToolCrest Hunter_v3;
        public static ToolCrest Reaper;
        public static ToolCrest Wanderer;
        public static ToolCrest Warrior;
        public static ToolCrest Witch;
        public static ToolCrest Toolmaster;
        public static ToolCrest Spell;

        public static bool SyncState = false;
        public static bool AllPlayerKnockedDown = false;
        public static bool KnockedDown = false;
        public static bool Suicide = false;

        public static Text PlayerListText;
        public static Text DebugText;

        public static Dictionary<CSteamID, PlayerAvatar> remotePlayers = new Dictionary<CSteamID, PlayerAvatar>();
        public static List<Transform> remotePlayersTransformList = new List<Transform>();

        public static string currentScene = "";

        public static Dictionary<string, CSteamID> sceneOwnersList = new Dictionary<string, CSteamID>();
        public static Dictionary<CSteamID, string> playerSceneMap = new Dictionary<CSteamID, string>();

        public static string currentOwnedScene = "";

        public static string skinName;
        public static string skinLink1;
        public static string skinLink2;
        public static string skinLink3;
        public static string skinLink4;

        public static bool debug = false;
        public static bool hideOuther = false;
        public static bool showComments = true;
        public static bool showNametags = true;

        internal static Dictionary<int, NetworkCustomPacket> customPackets = new Dictionary<int, NetworkCustomPacket>();

        public static List<CSteamID> GetRoomMembers()
        {
            return RoomManager.GetRoomMembers();
        }

        public static HashSet<string> fsmObject = new HashSet<string>
        {
            "Mossbone Mother",
            "Bone Beast",
            "Lace Boss1",
            "song_golem",
            "Skull King",
            "Bone Flyer Giant",
            "Vampire Gnat",
            "Splinter Queen",
            "Spinner Boss",
            "Driller A",
            "Driller B",
        };

        public static HashSet<string> hpObject = new HashSet<string>
        {
            "Mossbone Mother",
            "Bone Beast",
            "Lace Boss1",
            "SG_head",
            "Skull King",
            "Bone Flyer Giant",
            "Vampire Gnat",
            "Splinter Queen",
            "Spinner Boss",
            "Driller A",
            "Driller B",
        };

        public static void AddCustomPacket(NetworkCustomPacket packetHandler)
        {
            if (customPackets.ContainsKey(packetHandler.packetNum))
            {
                Debug.LogError(Environment.StackTrace);
                Debug.LogError($"Error while registering custom packet {packetHandler.receiveHandler.Method.Name}");
                Debug.LogError($"Custom packet with ID {packetHandler.packetNum} already exists. Not registering.");
                return;
            }

            customPackets.Add(packetHandler.packetNum, packetHandler);
        }

        public static void SetDamageScalingToCustom(this HealthManager hm)
        {
            // Get the type of a private nested class.
            Type nestedType = typeof(HealthManager).GetNestedType("DamageScalingConfig", BindingFlags.NonPublic);
            if (nestedType == null)
            {
                Debug.LogError("The type `DamageScalingConfig` could not be found.");
                return;
            }

            // Create an instance (using the default constructor)
            object newConfig = Activator.CreateInstance(nestedType);
            if (newConfig == null)
            {
                Debug.LogError("Unable to create DamageScalingConfig instance.");
                return;
            }

            // You can use reflection to modify the internal fields of `newConfig`, if you know the field names.
            FieldInfo multField = nestedType.GetField("someMultiplierField", BindingFlags.NonPublic | BindingFlags.Instance);
            if (multField != null)
            {
                multField.SetValue(newConfig, 2.0f); // Example: Change the multiplier to 2
            }

            // Set the private field of the `hm` instance.
            FieldInfo damageScalingField = typeof(HealthManager).GetField("damageScaling", BindingFlags.NonPublic | BindingFlags.Instance);
            if (damageScalingField != null)
            {
                damageScalingField.SetValue(hm, newConfig);
                Debug.Log("damageScaling has been replaced.");
            }
        }

        public static void ReplaceItemDropGroups(HealthManager hm)
        {
            if (hm == null)
            {
                Debug.LogError("HealthManager instance is null.");
                return;
            }

            // Find the ItemDropGroup type.
            var itemDropGroupType = typeof(HealthManager).GetNestedType(
                "ItemDropGroup", BindingFlags.NonPublic);

            if (itemDropGroupType == null)
            {
                Debug.LogError("The type HealthManager.ItemDropGroup could not be found.");
                return;
            }

            // Constructing a new List<ItemDropGroup>
            var listType = typeof(List<>).MakeGenericType(itemDropGroupType);
            var newList = Activator.CreateInstance(listType);

            // The `newList` is still empty here; you can also add elements to it using reflection.
            // For example, the `listType.GetMethod("Add")` call is used to add an `ItemDropGroup` instance.

            // Find the `itemDropGroups` field.
            var field = typeof(HealthManager).GetField("itemDropGroups",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError("The `itemDropGroups` field could not be found.");
                return;
            }

            // 替换掉原有的掉落组
            field.SetValue(hm, newList);

            Debug.Log("Successfully replaced the itemDropGroups list.");
        }

        public static void CloneAnimatorOfObject(GameObject gameObject, GameObject cloneTarget)
        {
            if(cloneTarget.GetComponent<tk2dSpriteAnimator>() && cloneTarget.GetComponent<tk2dSprite>())
            {
                tk2dSprite spriteRenderer = gameObject.AddComponent<tk2dSprite>();
                tk2dSpriteAnimator animator = gameObject.AddComponent<tk2dSpriteAnimator>();
                animator.Library = cloneTarget.GetComponent<tk2dSpriteAnimator>().Library;
                animator.Play(cloneTarget.GetComponent<tk2dSpriteAnimator>().CurrentClip);

                //spriteRenderer.SetSprite(cloneTarget.GetComponent<tk2dSprite>().Collection, cloneTarget.GetComponent<tk2dSprite>().spriteId);
            }
        }

        public static void ResetPlayer()
        {
            SilksongMultiplayerAPI.Hero_Hornet.GetComponent<HeroController>().acceptingInput = true;
            SilksongMultiplayerAPI.Hero_Hornet.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            SilksongMultiplayerAPI.Hero_Hornet.transform.Find("HeroBox").GetComponent<Collider2D>().enabled = true;
            SilksongMultiplayerAPI.KnockedDown = false;
            SilksongMultiplayerAPI.AllPlayerKnockedDown = false;
        }

        public static void ChangeEnemyTarget(ulong targetID,string enemyName)
        {

            CSteamID memberID = new CSteamID(targetID);

            Debug.Log("Switching boss target to：" + memberID.m_SteamID);

            if(GameObject.Find(enemyName) == false || GameObject.Find(enemyName).GetComponent<EnemyAvatar>() == false)
                return;

            EnemyAvatar enemy = GameObject.Find(enemyName).GetComponent<EnemyAvatar>();

            if (SilksongMultiplayerAPI.remotePlayers.TryGetValue(memberID, out PlayerAvatar playerAvatar))
            {
                Debug.Log("Switch target " + enemy.name + " to another player");
                enemy.TargetPlayer = playerAvatar.gameObject;
            }
            else
            {
                Debug.Log("Switch target " + enemy.name + " to self");
                enemy.TargetPlayer = SilksongMultiplayerAPI.Hero_Hornet;
            }
        }

        public static void ChangeCurrentOwnedScene(string sceneName)
        {
            currentOwnedScene = sceneName;

            if(currentScene == currentOwnedScene)
            {
                foreach (HealthManager enemyHealthManager in HealthManager.EnumerateActiveEnemies())
                {
                    if (enemyHealthManager.gameObject.GetComponent<EnemyAvatar>())
                    {
                        enemyHealthManager.gameObject.GetComponent<EnemyAvatar>().isOwner = true;
                    }
                }
            }
        }

        public static void OnChangeScene(string sceneName)
        {
            // ===== Added: Record the scene before you leave（oldScene）=====
            string oldScene = null;
            if (roomOwner)
            {
                playerSceneMap.TryGetValue(SteamUser.GetSteamID(), out oldScene);
            }

            currentScene = sceneName;

            if (roomOwner)
            {
                if (!sceneOwnersList.TryGetValue(sceneName, out CSteamID ownerId))
                {
                    // First, clear any existing scene mappings associated with the old "me" (if any).
                    string owned = GetSceneNameBySceneOwnersSteamID(SteamUser.GetSteamID());
                    if (owned != null)
                    {
                        sceneOwnersList.Remove(owned);
                    }

                    ownerId = SteamUser.GetSteamID();
                    sceneOwnersList[sceneName] = ownerId;

                    ChangeCurrentOwnedScene(sceneName);
                    NetworkDataSender.SendSceneOwner(ownerId.m_SteamID, sceneName);
                }
                else
                {
                    NetworkDataSender.SendSceneOwner(ownerId.m_SteamID, sceneName);
                }

                // ===== Original logic: Update the scene where the user is located. =====
                playerSceneMap[SteamUser.GetSteamID()] = sceneName;

                // ===== Added: If I am the owner of the oldScene I'm leaving, and no one is left in that scene after I leave -> Clear ownership of that scene. =====
                if (!string.IsNullOrEmpty(oldScene) && oldScene != sceneName)
                {
                    if (sceneOwnersList.TryGetValue(oldScene, out var oldOwner) && oldOwner == SteamUser.GetSteamID())
                    {
                        bool anyoneLeftInOldScene = false;
                        foreach (var kv in playerSceneMap)
                        {
                            if (kv.Key == SteamUser.GetSteamID()) continue; // I have already left oldScene.
                            if (kv.Value == oldScene)
                            {
                                anyoneLeftInOldScene = true;
                                break;
                            }
                        }

                        if (!anyoneLeftInOldScene)
                        {
                            sceneOwnersList.Remove(oldScene);
                        }
                    }
                }
            }

            // The following logic remains unchanged.
            if (currentScene == currentOwnedScene)
            {
                foreach (HealthManager hm in HealthManager.EnumerateActiveEnemies())
                {
                    if (hm.gameObject.TryGetComponent<EnemyAvatar>(out var ea))
                        ea.isOwner = true;
                }
            }
            else
            {
                foreach (HealthManager hm in HealthManager.EnumerateActiveEnemies())
                {
                    if (hm.gameObject.TryGetComponent<EnemyAvatar>(out var ea))
                        ea.isOwner = false;
                }
            }
        }


        public static void OnOutherChangeScene(string sceneName, CSteamID steamID)
        {
            if (!roomOwner) return;

            // Update the player's current scene.
            playerSceneMap[steamID] = sceneName;

            if (!sceneOwnersList.TryGetValue(sceneName, out CSteamID existing))
            {
                // This player previously owned the scene.
                string prev = GetSceneNameBySceneOwnersSteamID(steamID);

                if (prev != null && prev != sceneName)
                {
                    HandleOwnerLeavingScene(prev, steamID);
                }

                sceneOwnersList[sceneName] = steamID;
                NetworkDataSender.SendSceneOwner(steamID.m_SteamID, sceneName);
            }
            else
            {
                if(GetSceneNameBySceneOwnersSteamID(steamID) != null && GetSceneNameBySceneOwnersSteamID(steamID) != sceneName)
                {
                    sceneOwnersList.Remove(GetSceneNameBySceneOwnersSteamID(steamID));
                }

                NetworkDataSender.SendSceneOwner(existing.m_SteamID, sceneName);
            }
        }


        private static bool TryTransferSceneOwner(string sceneName, CSteamID leavingOwner)
        {
            if (!sceneOwnersList.TryGetValue(sceneName, out var currentOwner)) return false;
            if (currentOwner != leavingOwner) return false;

            foreach (var kv in playerSceneMap)
            {
                var playerId = kv.Key;
                var playerScene = kv.Value;

                if (playerId == leavingOwner) continue;
                if (playerScene != sceneName) continue;

                sceneOwnersList[sceneName] = playerId;
                NetworkDataSender.SendSceneOwner(playerId.m_SteamID, sceneName);
                return true;
            }

            return false; // No one is taking over.
        }

        private static int CountPlayersInScene(string sceneName, CSteamID? excluding = null)
        {
            int count = 0;
            foreach (var kv in playerSceneMap)
            {
                if (excluding.HasValue && kv.Key == excluding.Value) continue;
                if (kv.Value == sceneName) count++;
            }
            return count;
        }

        private static bool IsSceneEmpty(string sceneName, CSteamID? excluding = null)
        {
            return CountPlayersInScene(sceneName, excluding) == 0;
        }

        private static void HandleOwnerLeavingScene(string sceneName, CSteamID leavingOwner)
        {
            if (!sceneOwnersList.TryGetValue(sceneName, out var currentOwner)) return;
            if (currentOwner != leavingOwner) return;

            // Try transferring it first.
            bool transferred = TryTransferSceneOwner(sceneName, leavingOwner);

            if (transferred) return;

            // Transfer failed: If the scene is already empty, clear the owner.
            // Note: Exclude leavingOwner (treat them as already having left).
            if (IsSceneEmpty(sceneName, excluding: leavingOwner))
            {
                sceneOwnersList.Remove(sceneName);
                // Optional: Broadcast "no owner" (depending on whether your protocol supports this).
                // NetworkDataSender.SendSceneOwner(0, sceneName);
            }
        }



        public static string GetSceneNameBySceneOwnersSteamID(CSteamID steamID)
        {
            foreach (var kvp in sceneOwnersList)
            {
                if (kvp.Value == steamID)
                {
                    return kvp.Key;
                }
            }
            return null; // Not found
        }
    }
}
