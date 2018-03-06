using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.UI;
using AnotherRpgMod.UI;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.Utils;

namespace AnotherRpgMod
{
    public enum DamageType : byte
    {
        Melee,
        Ranged,
        Throw,
        Magic,
        Summon
    }

    public enum Message : byte {
        AddXP,
        SyncLevel,
        SyncNPC
    };

    public class DataTag
    {
        public static DataTag amount = new DataTag(reader => reader.ReadInt32());
        
        public static DataTag amount_single = new DataTag(reader => reader.ReadSingle());
        public static DataTag playerId = new DataTag(reader => reader.ReadInt32());
        public static DataTag npcId = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemId = new DataTag(reader => reader.ReadInt32());


        public static DataTag level = new DataTag(reader => reader.ReadInt32());
        public static DataTag damage = new DataTag(reader => reader.ReadInt32());
        public static DataTag life = new DataTag(reader => reader.ReadInt32());
        public static DataTag defense = new DataTag(reader => reader.ReadInt32());


        public Func<BinaryReader, object> read;

        public DataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }


    class Arpg : Mod
	{

        public UserInterface customResources;
        public HealthBar healthBar;
        public UserInterface customstats;
        public UserInterface customOpenstats;
        public Stats statMenu;
        public OpenStatsButton openStatMenu;

        public static Dictionary<Message, List<DataTag>> dataTags = new Dictionary<Message, List<DataTag>>()
        {
            { Message.AddXP, new List<DataTag>(){ DataTag.amount, DataTag.level } },
            { Message.SyncLevel, new List<DataTag>(){ DataTag.playerId, DataTag.amount } },
            { Message.SyncNPC, new List<DataTag>(){ DataTag.npcId, DataTag.level, DataTag.life, DataTag.damage, DataTag.defense } },
};


        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {

            Message msg = (Message)reader.ReadByte();
            Dictionary<DataTag, object> tags = new Dictionary<DataTag, object>();
            foreach (DataTag tag in dataTags[msg])
                tags.Add(tag, tag.read(reader));
            switch (msg)
            {
                case Message.SyncLevel:
                    if (Main.netMode == 2)
                        Main.player[(int)tags[DataTag.playerId]].GetModPlayer<RPGPlayer>().SyncLevel((int)tags[DataTag.amount]);
                    break;
                case Message.AddXP:
                    if (Main.netMode == 1)
                    {
                        Main.LocalPlayer.GetModPlayer<RPGPlayer>().AddXp((int)tags[DataTag.amount], (int)tags[DataTag.level]);
                    }
                    break;
                case Message.SyncNPC:
                    if (Main.netMode == 1)
                    {
                        NPC npc = Main.npc[(int)tags[DataTag.npcId]];

                        npc.lifeMax = (int)tags[DataTag.life];
                        npc.life = npc.lifeMax;
                        npc.damage = (int)tags[DataTag.damage];
                        npc.defense = (int)tags[DataTag.defense];
                        if (npc.GivenName == "")
                        {
                            npc.GivenName = ("Lvl. " + ((int)tags[DataTag.level]) + " " + npc.TypeName);
                        }
                        else
                            npc.GivenName = ("Lvl. " + ((int)tags[DataTag.level]) + " " + npc.GivenName);
                    }
                    break;
            }
        }
        public override void Load()
        {
            ConfigFile.Init();
            if (!Main.dedServ)
            {
                customResources = new UserInterface();
                healthBar = new HealthBar();
                HealthBar.visible = true;
                customResources.SetState(healthBar);

                customstats = new UserInterface();
                statMenu = new Stats();
                Stats.visible = false;
                customstats.SetState(statMenu);



                customOpenstats = new UserInterface();
                openStatMenu = new OpenStatsButton();
                OpenStatsButton.visible = true;
                customOpenstats.SetState(openStatMenu);


                /*
                
                statMenu = new Stats();
                Stats.visible = true;
                customstats.SetState(statMenu);
                */
            }
        }

        public Arpg()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

        public override void UpdateUI(GameTime gameTime)
        {
            if (customstats != null)
                customstats.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.netMode == 2)
                return;

            int id = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (id != -1)
            {
                //layers.RemoveAt(id);

                    //Add you own layer
                    layers.Insert(id, new LegacyGameInterfaceLayer(
                        "AnotherRpgMod: Custom Health Bar",
                        delegate
                        {
                            if (HealthBar.visible)
                            {
                                //Update CustomBars
                                customResources.Update(Main._drawInterfaceGameTime);
                                healthBar.Draw(Main.spriteBatch);
                                
                            }
                            return true;
                        }, InterfaceScaleType.UI)
                    );
            }

            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Map / Minimap"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "AnotherRpgMod: StatWindows",
                    delegate
                    {
                        if (Stats.visible)
                        {
                            
                            statMenu.Draw(Main.spriteBatch);
                            
                        }
                        if (OpenStatsButton.visible)
                        {
                            customOpenstats.Update(Main._drawInterfaceGameTime);
                            openStatMenu.Draw(Main.spriteBatch);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    

}
