using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using AnotherRpgMod.RPGModule.Entities;
using Terraria.ID;

namespace AnotherRpgMod.Utils
{
    class MPDebug
    {
        static public void Log(Mod mod, object message)
        {
            Log(mod, message.ToString());
        }


        static public void Log(Mod mod, string message)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.Log);
                packet.Write(message);
                packet.Send();
            }
        }
    }
}
