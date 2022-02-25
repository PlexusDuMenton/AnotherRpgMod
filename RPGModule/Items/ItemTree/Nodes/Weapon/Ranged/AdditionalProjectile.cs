using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnotherRpgMod.Utils;
using Microsoft.Xna.Framework;
using Terraria;

namespace AnotherRpgMod.Items
{
    class AdditionalProjectile : ItemNodeAdvanced
    {

        new protected string m_Name = "(Ascended) Multiple Projectile";
        new protected string m_Desc = "+ X Projectile";
        new protected NodeCategory m_NodeCategory = NodeCategory.Other;
        new protected bool m_isAscend = true;

        public override bool IsAscend
        {
            get
            {
                return m_isAscend;
            }
        }

        public override string GetName
        {
            get
            {
                return m_Name;
            }
        }

        public override string GetDesc
        {
            get
            {
                return "+ " + (ProjectileAmmount * Utils.Mathf.Clamp(GetLevel, 1, GetMaxLevel)) + " Projectile";
            }
        }

        
        public int ProjectileAmmount;

        public override void OnShoot(Terraria.DataStructures.IProjectileSource source,Item item, Player Player, ref Vector2 position, ref Vector2 Velocity, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < ProjectileAmmount* GetLevel; i++)
            {
                float spread = 10 * 0.0174f; //20 degree cone
                float baseSpeed = Velocity.Length() * (0.9f + 0.2f * Main.rand.NextFloat());
                float baseAngle = MathF.Atan2(Velocity.X, Velocity.Y);
                float randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                Vector2 newVelocity = baseSpeed * new Vector2(MathF.Sin(randomAngle), MathF.Cos(randomAngle));

                int projnum = Projectile.NewProjectile(source, position, newVelocity, type, damage, knockBack, Player.whoAmI);
                Main.projectile[projnum].friendly = true;
                Main.projectile[projnum].hostile = false;
                Main.projectile[projnum].originalDamage = damage;
            }
        }

        public override void SetPower(float value)
        {
            ProjectileAmmount = Mathf.FloorInt(Mathf.Pow(value, 0.25f));
            power = value;
        }

        public override void LoadValue(string saveValue)
        {
            power = saveValue.SafeFloatParse();
            SetPower(power);
        }

        public override string GetSaveValue()
        {
            return power.ToString();
        }
    }
}
