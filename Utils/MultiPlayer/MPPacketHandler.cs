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
            { Message.SyncLevel, new List<DataTag>(){ DataTag.playerId, DataTag.amount,DataTag.buffer } },
            { Message.SyncNPCSpawn, new List<DataTag>(){ DataTag.npcId, DataTag.level, DataTag.tier, DataTag.rank,DataTag.modifiers,DataTag.buffer, DataTag.WorldTier } },
            { Message.SyncNPCUpdate, new List<DataTag>(){ DataTag.npcId, DataTag.life, DataTag.maxLife } },
            { Message.AskNpc, new List<DataTag>(){ DataTag.npcId } },
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

        static public void SendNpcSpawn(Mod mod, NPC npc, int Tier, int Level, ARPGGlobalNPC ARPGNPC)
        {

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);

                ModPacket packet = mod.GetPacket();
                
                packet.Write((byte)Message.SyncNPCSpawn);
                packet.Write(npc.whoAmI);
                packet.Write(Level);
                packet.Write(Tier);

                packet.Write(ARPGNPC.getRank);
                packet.Write((int)ARPGNPC.modifier);
                packet.Write((string)ParseBuffer(ARPGNPC.specialBuffer));
                packet.Write(WorldManager.BossDefeated);

                packet.Send();
            }
        }

        static public void AskNpcInfo(Mod mod, NPC npc)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //AnotherRpgMod.Instance.Logger.Info("ask npc to server");
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.AskNpc);
                packet.Write(npc.whoAmI);
                packet.Send();
            }
        }

        static public void SendNpcUpdate(Mod mod, NPC npc, int ignore = -1)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                //NetMessage.SendData(23, -1, ignore, null, npc.whoAmI);
                
                
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.SyncNPCUpdate);
                packet.Write(npc.whoAmI);
                packet.Write(npc.life);
                packet.Write(npc.lifeMax);
                packet.Send();
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
                    RPGPlayer p = Main.player[(int)tags[DataTag.playerId]].GetModPlayer<RPGPlayer>();
                    if (p.baseName == "")
                        p.baseName = Main.player[(int)tags[DataTag.playerId]].name;

                    
                    if((int)tags[DataTag.playerId] != Main.myPlayer)
                    {
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            p.SyncLevel((int)tags[DataTag.amount]);
                        Main.player[(int)tags[DataTag.playerId]].name = p.baseName + " The Lvl." + p.GetLevel() + " " + (string)tags[DataTag.buffer];
                    }
                    
                    break;
                case Message.AddXP:
                    Main.LocalPlayer.GetModPlayer<RPGPlayer>().AddXp((int)tags[DataTag.amount], (int)tags[DataTag.level]);
                    break;
                case Message.SyncNPCSpawn:
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        

                        NPC npc = Main.npc[(int)tags[DataTag.npcId]];

                        if (npc.GetGlobalNPC<ARPGGlobalNPC>() == null)
                            AnotherRpgMod.Instance.Logger.Info(npc.GivenName);

                        //npc.SetDefaults(npc.type);
                        if (npc.GetGlobalNPC<ARPGGlobalNPC>().StatsCreated == true)
                            return;
                        int tier = (int)tags[DataTag.tier];
                        int level = (int)tags[DataTag.level];
                        NPCRank rank = (NPCRank)tags[DataTag.rank];

                        NPCModifier modifiers = (NPCModifier)tags[DataTag.modifiers];
                        if (npc == null || npc.GetGlobalNPC<ARPGGlobalNPC>() == null)
                            return;
                        AnotherRpgMod.Instance.Logger.Info(npc.GivenOrTypeName + "\nTier : " + tier + "   Level : " + level + "   rank : " + rank + "   Modifier  : " + modifiers + " \n Buffer : " + (string)tags[DataTag.buffer]);

                        Dictionary<string, string> bufferStack = Unparse((string)tags[DataTag.buffer]);

                        WorldManager.BossDefeated = (int)tags[DataTag.WorldTier];

                        npc.GetGlobalNPC<ARPGGlobalNPC>().StatsCreated = true;
                        npc.GetGlobalNPC<ARPGGlobalNPC>().modifier = modifiers;
                        npc.GetGlobalNPC<ARPGGlobalNPC>().SetLevelTier(level, tier, (byte)rank);
                        npc.GetGlobalNPC<ARPGGlobalNPC>().specialBuffer = bufferStack;

                        npc.GetGlobalNPC<ARPGGlobalNPC>().SetStats(npc);
                       
                        npc.GivenName = NPCUtils.GetNpcNameChange(npc, tier, level, rank);

                        

                        //AnotherRpgMod.Instance.Logger.Info("NPC created with id : " + npc.whoAmI);
                        //AnotherRpgMod.Instance.Logger.Info( "Client Side : \n" + npc.GetGivenOrTypeNetName() + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n\n");

                    }
                    break;

                case Message.SyncNPCUpdate:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NPC npcu = Main.npc[(int)tags[DataTag.npcId]];

                        if (npcu.lifeMax != (int)tags[DataTag.maxLife] || npcu.life != (int)tags[DataTag.life])
                        {
                            AnotherRpgMod.Instance.Logger.Warn("DESYNC ERROR SPOTTED FOR : ");
                            AnotherRpgMod.Instance.Logger.Warn(npcu.GivenOrTypeName + "\n" + (int)tags[DataTag.life] + " / " + (int)tags[DataTag.maxLife] + "\n" + npcu.life + " / " + npcu.lifeMax);
                        }
                        Main.npc[(int)tags[DataTag.npcId]].lifeMax = (int)tags[DataTag.maxLife];
                        Main.npc[(int)tags[DataTag.npcId]].life = (int)tags[DataTag.life];

                    }
                    break;
                case Message.Log:
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        //ErrorLogger.Log("LOG FROM SERVER");
                        AnotherRpgMod.Instance.Logger.Info((string)tags[DataTag.buffer]);
                    }

                    break;
                case Message.AskNpc:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NPC npc = Main.npc[(int)tags[DataTag.npcId]];
                        if (npc.GetGlobalNPC<ARPGGlobalNPC>() == null)
                            return;
                        int tier = npc.GetGlobalNPC<ARPGGlobalNPC>().getTier;
                        int level = npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel;
                        int rank = npc.GetGlobalNPC<ARPGGlobalNPC>().getRank;
                        Mod mod = AnotherRpgMod.Instance;
                        //MPDebug.Log(mod, "Server Side : \n" + npc.GetGivenOrTypeNetName() + " ID : " + npc.whoAmI + "\nLvl." + (npc.GetGlobalNPC<ARPGGlobalNPC>().getLevel + npc.GetGlobalNPC<ARPGGlobalNPC>().getTier) + "\nHealth : " + npc.life + " / " + npc.lifeMax + "\nDamage : " + npc.damage + "\nDef : " + npc.defense + "\nTier : " + npc.GetGlobalNPC<ARPGGlobalNPC>().getRank + "\n");

                        SendNpcSpawn(mod, npc, tier, level, npc.GetGlobalNPC<ARPGGlobalNPC>());
                    }
                    break;
            }
        }
    }
}
