using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.Utils
{
    class MPPacketHandler
    {
        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>()
        {
            { Message.AddXP, new List<DataTag>(){ DataTag.amount, DataTag.level } },
            { Message.SyncLevel, new List<DataTag>(){ DataTag.PlayerId, DataTag.amount,DataTag.buffer,DataTag.life, DataTag.maxLife } },
            { Message.SyncPlayerHealth, new List<DataTag>(){ DataTag.PlayerId,DataTag.life, DataTag.maxLife } },
            { Message.SyncNPCSpawn, new List<DataTag>(){ DataTag.PlayerId, DataTag.npcId, DataTag.level, DataTag.tier, DataTag.rank,DataTag.modifiers,DataTag.buffer, DataTag.WorldTier } },
            { Message.SyncNPCUpdate, new List<DataTag>(){ DataTag.PlayerId, DataTag.npcId, DataTag.life, DataTag.maxLife, DataTag.damage } },
            { Message.AskNpc, new List<DataTag>(){ DataTag.PlayerId,DataTag.npcId } },
            { Message.Log, new List<DataTag>(){ DataTag.buffer } }
        };

        static public void SendXPPacket(Mod mod,int XPToDrop,int xplevel)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AddXP);
                packet.Write(XPToDrop);
                packet.Write(xplevel);
                packet.Send();
            }
        }



        static public string ParseBuffer(Dictionary<string, string> buffer)
        {
           string parsed = "";

            if (buffer.Count == 0)
                return parsed;
            foreach (KeyValuePair<string, string> entry in buffer)
            {
                parsed += entry.Key + ":" + entry.Value + ",";
            }
            
            return parsed.Remove(parsed.Length-1);
        }

        static public Dictionary<string, string> Unparse(string parsed)
        {
            Dictionary<string, string> unparsed = new Dictionary<string, string>();
            if (parsed == "")
                return unparsed;
            String[] KVString = parsed.Split(',');
            for (int i = 0; i < KVString.Length; i++)
            {
                KeyValuePair<string, string> KVPair = new KeyValuePair<string, string>(KVString[i].Split(':')[0], KVString[i].Split(':')[1]);
                unparsed.Add(KVPair.Key, KVPair.Value);
            }
            return unparsed;
        }

        static public void SendNpcSpawn(Mod mod, int askingClientId, NPC npc, int Tier, int Level, ARPGGlobalNPC ARPGNPC)
        {

            if (Main.netMode == NetmodeID.Server)
            {

                ModPacket packet = mod.GetPacket();
                
                packet.Write((byte)Message.SyncNPCSpawn);
                packet.Write((byte)askingClientId);
                packet.Write((byte)npc.whoAmI);
                packet.Write(Level);
                packet.Write(Tier);

                packet.Write(ARPGNPC.getRank);
                packet.Write((int)ARPGNPC.modifier);
                packet.Write((string)ParseBuffer(ARPGNPC.specialBuffer));
                packet.Write(WorldManager.BossDefeated);

                packet.Send(toClient: askingClientId);
            }
        }

        static public void SendPlayerHealthSync(Mod mod, int playerIndex, int ignore = -1)
        {

            ModPacket packet = mod.GetPacket();
            packet.Write((byte)Message.SyncPlayerHealth);
            packet.Write((byte)playerIndex);
            packet.Write(Main.player[playerIndex].statLife);
            packet.Write(Main.player[playerIndex].statLifeMax);
            packet.Send(ignoreClient: ignore);
        }

        static public void AskNpcInfo(Mod mod, NPC npc, int playerindex, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //AnotherRpgMod.Instance.Logger.Info("ask npc to server");
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AskNpc);
                packet.Write((byte)playerindex);
                packet.Write((byte)npc.whoAmI);
                packet.Send(ignoreClient: ignore);
            }
        }

        static public void SendNpcUpdate(Mod mod, NPC npc, int playerindex = - 1, int ignore = -1)
        {


            if (!Main.npc[npc.whoAmI].active)
                return;

            if (Main.netMode == NetmodeID.Server)
            {

                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SyncNPCUpdate);
                packet.Write((byte)playerindex);
                packet.Write((byte)npc.whoAmI);
                packet.Write(npc.life);
                packet.Write(npc.lifeMax);
                packet.Write(npc.damage);
                packet.Send(ignoreClient: ignore);
                
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                AskNpcInfo(mod, npc, playerindex, ignore);
            }



        }

        static public void HandlePacket(BinaryReader reader, int whoAmI)
        {

            Message msg = (Message)reader.ReadByte();
            Dictionary<DataTag, object> tags = new Dictionary<DataTag, object>();
            foreach (DataTag tag in dataTags[msg])
                tags.Add(tag, tag.read(reader));
            switch (msg)
            {
                case Message.SyncLevel:
                    byte playerID = (byte)tags[DataTag.PlayerId];
                    RPGPlayer p = Main.player[playerID].GetModPlayer<RPGPlayer>();
                    /*
                    if (p.baseName == "")
                        p.baseName = Main.player[(int)tags[DataTag.playerId]].name;
                    */
                    
                    if((byte)tags[DataTag.PlayerId] != Main.myPlayer &&  Main.player[playerID] != null)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            p.SyncLevel((int)tags[DataTag.amount]);
                        //Main.player[(int)tags[DataTag.playerId]].name = (string)tags[DataTag.buffer];

                        if (Main.netMode != NetmodeID.Server)
                        {
                            Main.player[playerID].statLife = (int)tags[DataTag.life];
                            Main.player[playerID].statLifeMax2 = (int)tags[DataTag.maxLife];
                            if (WorldManager.instance != null)
                            {
                                WorldManager.instance.NetUpdateWorld();
                            }
                            
                        }
                    }

                    WorldManager.PlayerLevel = Math.Max(WorldManager.PlayerLevel, p.GetLevel());

                    break;
                case Message.AddXP:
                    Main.LocalPlayer.GetModPlayer<RPGPlayer>().AddXp((int)tags[DataTag.amount], (int)tags[DataTag.level]);
                    break;
                case Message.SyncNPCSpawn:
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        break;
                        

                    NPC npc = Main.npc[(byte)tags[DataTag.npcId]];

                    ARPGGlobalNPC rpgNPC;

                    if (npc.TryGetGlobalNPC<ARPGGlobalNPC>(out rpgNPC))
                        AnotherRpgMod.Instance.Logger.Info("Sync NPC Spawn | name :" + npc.GivenName);
                    else 
                    //Request this npc again in a few frame

                    if (npc == null || rpgNPC == null)
                        break;

                    if (rpgNPC.StatsCreated == true)
                        break;

                    int tier = (int)tags[DataTag.tier];
                    int level = (int)tags[DataTag.level];
                    NPCRank rank = (NPCRank)tags[DataTag.rank];

                    NPCModifier modifiers = (NPCModifier)tags[DataTag.modifiers];
                        
                    AnotherRpgMod.Instance.Logger.Info(npc.GivenOrTypeName + "\nTier : " + tier + "   Level : " + level + "   rank : " + rank + "   Modifier  : " + modifiers + " \n Buffer : " + (string)tags[DataTag.buffer]);

                    Dictionary<string, string> bufferStack = Unparse((string)tags[DataTag.buffer]);

                    WorldManager.BossDefeated = (int)tags[DataTag.WorldTier];

                    npc.GetGlobalNPC<ARPGGlobalNPC>().StatsCreated = true;
                    npc.GetGlobalNPC<ARPGGlobalNPC>().modifier = modifiers;
                    npc.GetGlobalNPC<ARPGGlobalNPC>().SetLevelTier(level, tier, (byte)rank);
                    npc.GetGlobalNPC<ARPGGlobalNPC>().specialBuffer = bufferStack;

                    npc.GetGlobalNPC<ARPGGlobalNPC>().SetStats(npc);
                       
                    npc.GivenName = NPCUtils.GetNpcNameChange(npc, tier, level, rank);
                    npc.life = npc.lifeMax;
                        

                    //AnotherRpgMod.Instance.Logger.Info("NPC created with id : " + npc.whoAmI);
                    //AnotherRpgMod.Instance.Logger.Info( "Client Side : \n" + npc.GetGivenOrTypeNetName() + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n\n");

                    break;

                case Message.SyncNPCUpdate:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {

                        byte npcId = (byte)tags[DataTag.npcId];

                        

                        NPC npcu = Main.npc[npcId];

                        if (!npcu.active)
                            return;

                        if (npcu.lifeMax != (int)tags[DataTag.maxLife])
                        {
                            AskNpcInfo(AnotherRpgMod.Instance, npcu, Main.myPlayer);
                        }
                        npcu.lifeMax = (int)tags[DataTag.maxLife];
                        npcu.life = (int)tags[DataTag.life];
                        npcu.damage = (int)tags[DataTag.damage];
                    }
                    break;
                case Message.Log:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        AnotherRpgMod.Instance.Logger.Info((string)tags[DataTag.buffer]);
                    }

                    break;
                case Message.AskNpc:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int askplayerID = (byte)tags[DataTag.PlayerId];

                        NPC asknpc = Main.npc[(byte)tags[DataTag.npcId]];
                        
                        ARPGGlobalNPC askrpgnpc;
                        if (!asknpc.TryGetGlobalNPC<ARPGGlobalNPC>(out askrpgnpc))
                        {
                            MPDebug.Log(AnotherRpgMod.Instance, "Couldn't find Global NPC of asked NPC");

                            return;
                        }
                            
                        int asktier = askrpgnpc.getTier;
                        int asklevel = askrpgnpc.getLevel;
                        int askrank = askrpgnpc.getRank;
                        Mod mod = AnotherRpgMod.Instance;
                        //MPDebug.Log(mod, "Server Side : \n" + npc.GetGivenOrTypeNetName() + " ID : " + npc.whoAmI + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n");

                        SendNpcSpawn(mod, askplayerID, asknpc, asktier, asklevel, askrpgnpc);
                    }
                    break;
                case Message.SyncPlayerHealth:


                    byte pID = (byte)tags[DataTag.PlayerId];

                    if (pID == Main.myPlayer && !Main.ServerSideCharacter)
                        break;


                    if (Main.netMode == NetmodeID.Server)
                        pID = (byte)whoAmI;

                    Player player = Main.player[pID];
                    player.statLife = (int)tags[DataTag.life];
                    player.statLifeMax = (int)tags[DataTag.maxLife];
                    if (player.statLifeMax < 100)
                        player.statLifeMax = 100;
                    player.dead = player.statLife <= 0;

                    if (Main.netMode != NetmodeID.Server)
                        break;

                    try
                    {
                        SendPlayerHealthSync(AnotherRpgMod.Instance, pID, whoAmI);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }

                    
                    break;
                case Message.syncWorld:
                    break;
            }
        }
    }
}
