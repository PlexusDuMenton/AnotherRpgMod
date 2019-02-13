using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace AnotherRpgMod.Items
{
    class AdditionalProjectile : ItemNodeAdvanced
    {

        new protected string m_Name = "Additional Porjectile";
        new protected string m_Desc = "+ X Projectile";
        new protected NodeCategory m_NodeCategory = NodeCategory.Other;



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
                return "+ " + (ProjectileAmmount) + " Projectile";
            }
        }

        protected new bool m_isAscend = true;
        public int ProjectileAmmount;

        public override void OnShoot(Item item, Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < ProjectileAmmount; i++)
            {
                float spread = 10 * 0.0174f; //20 degree cone
                float baseSpeed = (float)Math.Sqrt(speedX * speedX + speedY * speedY) * (1 + 0.1f * Main.rand.NextFloat());
                double baseAngle = Math.Atan2(speedX, speedY);
                double randomAngle = baseAngle + (Main.rand.NextFloat() - 0.5f) * spread;
                float spdX = baseSpeed * (float)Math.Sin(randomAngle);
                float spdY = baseSpeed * (float)Math.Cos(randomAngle);

                int projnum = Projectile.NewProjectile(position.X, position.Y, spdX, spdY, type, damage, knockBack, player.whoAmI);
                Main.projectile[projnum].friendly = true;
                Main.projectile[projnum].hostile = false;
            }
        }


        public override void LoadValue(string saveValue)
        {
            ProjectileAmmount = int.Parse(saveValue);
        }

        public override string GetSaveValue()
        {
            return ProjectileAmmount.ToString();
        }
    }
}
