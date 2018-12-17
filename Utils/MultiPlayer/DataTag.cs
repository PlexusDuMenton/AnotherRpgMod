using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.Utils
{
    public class DataTag
    {
        public static DataTag amount = new DataTag(reader => reader.ReadInt32());

        public static DataTag amount_single = new DataTag(reader => reader.ReadSingle());
        public static DataTag playerId = new DataTag(reader => reader.ReadInt32());
        public static DataTag npcId = new DataTag(reader => reader.ReadInt32());
        public static DataTag itemId = new DataTag(reader => reader.ReadInt32());


        public static DataTag level = new DataTag(reader => reader.ReadInt32());
        public static DataTag tier = new DataTag(reader => reader.ReadInt32());
        public static DataTag WorldTier = new DataTag(reader => reader.ReadInt32());
        public static DataTag rank = new DataTag(reader => reader.ReadInt32());

        public static DataTag modifiers = new DataTag(reader => reader.ReadInt32());
        public static DataTag buffer = new DataTag(reader => reader.ReadString());


        public static DataTag damage = new DataTag(reader => reader.ReadInt32());
        public static DataTag life = new DataTag(reader => reader.ReadInt32());
        public static DataTag maxLife = new DataTag(reader => reader.ReadInt32());


        public static DataTag GPFlag = new DataTag(reader => reader.ReadInt32());

        public static DataTag XPReductionDelta = new DataTag(reader => reader.ReadInt32());

        public static DataTag XpMultiplier = new DataTag(reader => reader.ReadSingle());
        public static DataTag NpclevelMultiplier = new DataTag(reader => reader.ReadSingle());
        public static DataTag ItemXpMultiplier = new DataTag(reader => reader.ReadSingle());

        public static DataTag Seed = new DataTag(reader => reader.ReadInt32());


        public Func<BinaryReader, object> read;

        public DataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }

}
