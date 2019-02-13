using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Utilities;
using AnotherRpgMod.RPGModule.Entities;
using AnotherRpgMod.Utils;
using AnotherRpgMod.RPGModule;

namespace AnotherRpgMod.Items
{
    public class ItemDataTag
    {
        public static ItemDataTag level = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag xp = new ItemDataTag(reader => reader.ReadInt64());
        public static ItemDataTag ascendedlevel = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag modifier = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag init = new ItemDataTag(reader => reader.ReadBoolean());

        public static ItemDataTag rarity = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statsamm = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst1 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat1 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst2 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat2 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst3 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat3 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst4 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat4 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst5 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat5 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag statst6 = new ItemDataTag(reader => reader.ReadSByte());
        public static ItemDataTag stat6 = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag baseDamage = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag baseArmor = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag baseAutoReuse = new ItemDataTag(reader => reader.ReadBoolean());
        public static ItemDataTag baseName = new ItemDataTag(reader => reader.ReadString());
        public static ItemDataTag baseUseTime = new ItemDataTag(reader => reader.ReadInt32());
        public static ItemDataTag baseMana = new ItemDataTag(reader => reader.ReadInt32());

        public static ItemDataTag itemTree = new ItemDataTag(reader => reader.ReadString());


        public Func<BinaryReader, object> read;

        public ItemDataTag(Func<BinaryReader, object> read)
        {
            this.read = read;
        }
    }

}
