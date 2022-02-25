using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AnotherRpgMod.Utils;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using AnotherRpgMod.Items;
using System.Reflection;
using Terraria.UI.Chat;

namespace AnotherRpgMod.RPGModule.Entities
{

	class RPGPlayer : ModPlayer
	{
		public readonly static UInt16 ACTUALSAVEVERSION = 2;
		private int Exp = 0;
		private int skillPoints = 5;
		private string basename = "";


		private DateTime LastLeech = DateTime.MinValue;

		public void SpentSkillPoints(int value)
		{
			skillPoints -= value;
			Mathf.Clamp(skillPoints, 0, int.MaxValue);
		}
		public int GetSkillPoints
		{
			get { return skillPoints; }
		}
		private RPGStats Stats;
		private SkillTree skilltree;
		public SkillTree GetskillTree { get { return skilltree; } }
		float damageToApply = 0;


		private bool initiated = false;

		private bool XpLimitMessage = false;

		private int level = 1;
		private int armor;
		public int BaseArmor { get { return armor; } }
		public float m_virtualRes = 0;

		public long EquipedItemXp = 0;
		public long EquipedItemMaxXp = 1;

		public float ManaRegenBuffer = 0;
		public float ManaRegenPerSecond = 0;
		public int manaShieldDelay = 0;


		public float statMultiplier = 1;

		static public float MAINSTATSMULT = 0.0025f;
		static public float SECONDARYTATSMULT = 0.001f;
		public string baseName = "";


		public void SyncLevel(int _level) //only use for sync
		{
			level = _level;
		}
		public void SyncStat(StatData data, Stat stat) //only use for sync
		{
			Stats.SetStats(stat, level, data.GetLevel, data.GetXP);
		}

		public int GetStatXP(Stat s)
		{
			return Stats.GetStatXP(s);
		}
		public int GetStatXPMax(Stat s)
		{
			return Stats.GetStatXPMax(s);
		}
		public int GetStatImproved(Stat stat)
		{
			float VampireMultiplier = 1;
			float DayPerkMultiplier = 1;
			if (skilltree.HavePerk(Perk.Vampire))
			{

				switch (skilltree.nodeList.GetPerk(Perk.Vampire).GetLevel)
				{
					case 1:
						if (Main.dayTime)
							VampireMultiplier = 0.5f;
						else
							VampireMultiplier = 1.25f;
						break;
					case 2:
						if (Main.dayTime)
							VampireMultiplier = 0.75f;
						else
							VampireMultiplier = 1.5f;
						break;
					case 3:
						if (Main.dayTime)
							VampireMultiplier = 0.9f;
						else
							VampireMultiplier = 2f;
						break;
				}
			}

			if (skilltree.HavePerk(Perk.Chlorophyll))
			{

				switch (skilltree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel)
				{
					case 1:
						if (Main.dayTime)
							DayPerkMultiplier = 1.25f;
						else
							DayPerkMultiplier = 0.5f;
						break;
					case 2:
						if (Main.dayTime)
							DayPerkMultiplier = 1.5f;
						else
							DayPerkMultiplier = 0.75f;
						break;
					case 3:
						if (Main.dayTime)
							DayPerkMultiplier = 2f;
						else
							DayPerkMultiplier = 0.9f;
						break;
				}
			}
			return Mathf.CeilInt(GetStat(stat) * GetStatImprovementItem(stat) * VampireMultiplier * DayPerkMultiplier);

		}
		public int GetStat(Stat s)
		{
			return Stats.GetStat(s) + skilltree.GetStats(s);
		}
		public int GetNaturalStat(Stat s)
		{
			return Stats.GetNaturalStat(s) + skilltree.GetStats(s);
		}
		public int GetAddStat(Stat s)
		{
			return Stats.GetLevelStat(s);
		}
		public int GetLevel()
		{
			return level;
		}
		public int GetExp()
		{
			return Exp;
		}

		private int totalPoints = 0;
		private int freePoints = 0;


		public int FreePtns { get { return freePoints; } }
		public int TotalPtns { get { return totalPoints; } }


		public float GetLifeLeechLeft { get { return HealthRegenLeech * LifeLeechDuration * Player.statLifeMax2; } }
		private float HealthRegenLeech = 0.02f;

		private float LifeLeechDuration;
		public float LifeLeechMaxDuration = 3;

		#region WEAPONXP
		public void AddWeaponXp(int damage, Item Xitem, float multiplier = 1)
		{
			if (damage < 0)
				damage = -damage;
			if (Xitem != null && Xitem.damage > 0 && Xitem.maxStack <= 1)
			{
				ItemUpdate Item = Xitem.GetGlobalItem<Items.ItemUpdate>();
				if (Item != null && ItemUpdate.NeedSavingStatic(Xitem))
				{
					Item.AddExp(Mathf.Ceillong(damage * multiplier), Player, Xitem);
				}
			}
		}

		public void HealSlow(int lifeHeal)
		{

			Player.GetModPlayer<RPGPlayer>().ApplyReduction(ref lifeHeal);

			if (lifeHeal > 0)
			{
				if (lifeHeal < 1)
					lifeHeal = 1;
				float duration = lifeHeal / (Player.statLifeMax2 * HealthRegenLeech);
				float buffer = 0;
				float totalHeal = 0;

				if (LifeLeechDuration < 1)
				{
					buffer = LifeLeechDuration;
					LifeLeechDuration = Mathf.Clamp(LifeLeechDuration + duration, LifeLeechDuration, 1);
					buffer = LifeLeechDuration - buffer;
					totalHeal = lifeHeal * buffer;
					duration -= buffer;
				}
				if (duration > 0)
				{

					int LifeLifeHealInfo = (int)(lifeHeal * 0.2f);

					duration *= 0.2f;

					buffer = LifeLeechDuration;

					LifeLeechDuration = Mathf.Clamp(LifeLeechDuration + duration, LifeLeechDuration, LifeLeechMaxDuration);
					buffer = LifeLeechDuration - buffer;
					totalHeal += lifeHeal * buffer;

				}
				if (totalHeal < 1)
					totalHeal = 1;
				CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + (int)totalHeal);

			}
		}
	


	public void Leech(int damage)
	{

		TimeSpan CD = TimeSpan.FromSeconds(Config.gpConfig.LifeLeechCD);
		if (LastLeech + CD < DateTime.Now)
		{
			LastLeech = DateTime.Now;
			HealSlow(Mathf.CeilInt(GetLifeLeech(damage)));
		}
			
		int manaHeal = (int) (Player.statManaMax2 * GetManaLeech());
		

		if (manaHeal > 0)
		{
			Player.statMana = Mathf.Clamp(Player.statMana + manaHeal, 0, Player.statManaMax2);
		}
			
	}

	private float BufferLife;

	private void CustomPostUpdates()
	{

		if ( LifeLeechDuration > 0 && Player.statLife < Player.statLifeMax2)
		{
			LifeLeechDuration -= 1f / 60f;

			float newLife = BufferLife + Player.statLife + (Player.statLifeMax2 * HealthRegenLeech) / 60;
			int LifeGain = Mathf.Clamp(Mathf.FloorInt(newLife), Player.statLife, Player.statLifeMax2) ;
			BufferLife = newLife - LifeGain;
			Player.statLife = LifeGain;

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}
		}
		else
		{
			BufferLife = 0;
			LifeLeechDuration = 0;
		}
			

	}


	public float GetHealthRatio()
		{
			float hp = Player.statLife;
			float maxhp = Player.statLifeMax2;
			return hp / maxhp;
		}
		public float GetManaRatio()
		{
			float mp = Player.statMana;
			float maxmp = Player.statManaMax2;
			return mp / maxmp;
		}


		int GetBuffAmmount(int[] bufftime)
		{
			int count =0;
			for (int i = 0; i < bufftime.Length; i++)
			{
				if (bufftime[i] > 0)
					count++;
			}
			return count;
   
		}

		float GetXpMult()
		{
			float value = 1;
			ItemUpdate instance;
			Item item;
			for (int i = 0; i <= Player.armor.Length; i++)
			{
				if (i == Player.armor.Length)
					item = Player.HeldItem;
				else
					item = Player.armor[i];

				if (item.netID > 0)
				{
					instance = item.GetGlobalItem<ItemUpdate>();
					if (ModifierManager.HaveModifier(Modifier.Smart, instance.modifier) && !Main.dayTime)
					{
						value += ModifierManager.GetModifierBonus(Modifier.Smart, instance) * 0.01f;
					}
				}
			}
				
			return value;
		}

		void UpdateModifier()
		{
			ItemUpdate instance;
			Item item;
			int maxslot = Player.armor.Length;
			if (!Config.gpConfig.VanityGiveStat)
			{
				maxslot = 9;
			}

			for (int i = 0; i <= maxslot; i++)
			{
				if (i == maxslot)
					item = Player.HeldItem;
				else
					item = Player.armor[i];

                if (item.TryGetGlobalItem<ItemUpdate>(out instance))
                {
					instance.EquipedUpdateModifier(item, Player);
				}
				
			}
		}

		float DamageMultiplierFromModifier(NPC target, int damage)
		{
			float value = 1;
			ItemUpdate instance;
			Item item;

			for (int i = 0; i <= Player.armor.Length; i++)
			{
				if (i == Player.armor.Length)
					item = Player.HeldItem;
				else
					item = Player.armor[i];
					
				if (item.netID > 0)
				{

						
					instance = item.GetGlobalItem<ItemUpdate>();
					if (ModifierManager.HaveModifier(Modifier.MoonLight, instance.modifier) && !Main.dayTime)
					{
						value += ModifierManager.GetModifierBonus(Modifier.MoonLight, instance) * 0.01f;
					}
					if (ModifierManager.HaveModifier(Modifier.SunLight, instance.modifier) && Main.dayTime)
					{
						value += ModifierManager.GetModifierBonus(Modifier.SunLight, instance) * 0.01f;
					}
					if (ModifierManager.HaveModifier(Modifier.Berserker, instance.modifier))
					{
						value += ModifierManager.GetModifierBonus(Modifier.Berserker, instance)  * (1- GetHealthRatio());
					}
					if (ModifierManager.HaveModifier(Modifier.MagicConnection, instance.modifier))
					{
						value += ModifierManager.GetModifierBonus(Modifier.MagicConnection, instance) * (1 - GetManaRatio());
					}
					if (ModifierManager.HaveModifier(Modifier.Sniper, instance.modifier))
					{
						value += ModifierManager.GetModifierBonus(Modifier.Sniper, instance) * 0.01f * Vector2.Distance(Player.position, target.position);
					}
					if (ModifierManager.HaveModifier(Modifier.Brawler, instance.modifier) && Vector2.Distance(Player.position, target.position) < 130)
					{
						value += ModifierManager.GetModifierBonus(Modifier.Brawler, instance) * 0.01f;
					}
					if (ModifierManager.HaveModifier(Modifier.Executor, instance.modifier))
					{
						value += ModifierManager.GetModifierBonus(Modifier.Executor, instance) * (1 - (target.life / target.lifeMax));
					}
					if (ModifierManager.HaveModifier(Modifier.Venom, instance.modifier) && target.HasBuff(20))
					{
						value += ModifierManager.GetModifierBonus(Modifier.Venom, instance) * 0.01f;
					}
					if (ModifierManager.HaveModifier(Modifier.Chaotic, instance.modifier) )
					{
						value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(Player.buffTime);
					}
					if (ModifierManager.HaveModifier(Modifier.Cunning, instance.modifier))
					{
						value += ModifierManager.GetModifierBonus(Modifier.Chaotic, instance) * 0.01f * GetBuffAmmount(target.buffTime);
					}
						

					//APPLY DEBUFF
					if (ModifierManager.HaveModifier(Modifier.Poisones, instance.modifier))
					{
						if(Mathf.Random(0,100)< ModifierManager.GetModifierBonus(Modifier.Poisones, instance))
							target.AddBuff(BuffID.Venom ,360);
					}
					if (ModifierManager.HaveModifier(Modifier.Confusion, instance.modifier))
					{
						if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Confusion, instance))
							target.AddBuff(BuffID.Confused, 360);
					}

					if (ModifierManager.HaveModifier(Modifier.Cleave, instance.modifier))
					{
						int damageToApply = (int)(ModifierManager.GetModifierBonus(Modifier.Cleave, instance) * damage * value*0.01f);
						for (int j = 0; j< Main.npc.Length; j++)
						{
							if (Vector2.Distance(Main.npc[j].position, target.position) < 200 && !Main.npc[j].townNPC)
							{
								Main.npc[j].GetGlobalNPC<ARPGGlobalNPC>().ApplyCleave(Main.npc[j], damageToApply);
							}
						}
					}

					if (ModifierManager.HaveModifier(Modifier.BloodSeeker, instance.modifier))
					{
						if (ModifierManager.GetModifierBonus(Modifier.BloodSeeker, instance) > ((float)target.life / (float)target.lifeMax) * 100)
						{
							
							int heal = (int)(damage * value * 0.01f * ModifierManager.GetModifierBonusAlt(Modifier.BloodSeeker, instance));
							Player.GetModPlayer<RPGPlayer>().ApplyReduction(ref heal);
							HealSlow(heal);

							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
							}
						}
					}
						
				}
			}
			return value;
		}



		int GetMaxMinion()
		{
			int value = 0;
			ItemUpdate instance;
			Item item;
			for (int i = 0; i < Player.inventory.Length; i++)
			{
				item = Player.inventory[i];

				if (item.netID > 0)
				{
					instance = item.GetGlobalItem<ItemUpdate>();
					if (item.DamageType == DamageClass.Summon && instance.Ascention > value)
						value = instance.Ascention;
				}
			}
			return value;
		}

		public int ApplyPiercing(float piercing,int def,int damage)
		{
			if (damage < def)
			{
				damage = Mathf.RoundInt(def + damage * piercing);
			}
			else if (damage > def + damage * piercing)
			{
				damage += Mathf.RoundInt(def * piercing);
			}
			return damage;
		}

		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			modifyHitGeneral(item, target, ref damage, ref knockback, ref crit);
		}
	   

		private void modifyHitGeneral(Item item, NPC target, ref int damage, ref float knockback, ref bool crit,int pen = 1,bool projMinion = false)
		{
			if (Config.gpConfig.RPGPlayer)
			{
				if (crit)
				{
					damage = (int)(0.5f * damage * GetCriticalDamage());
				}
			}
			damage = (int)(damage * DamageMultiplierFromModifier(target, damage));
			

			if (ModifierManager.HaveModifier(Modifier.Piercing, Player.HeldItem.GetGlobalItem<ItemUpdate>().modifier))
			{
				damage = ApplyPiercing(ModifierManager.GetModifierBonus(Modifier.Piercing, Player.HeldItem.GetGlobalItem<ItemUpdate>()), target.defense, damage);
			}


			if (Config.gpConfig.RPGPlayer)
			{
				//CupidonPerk
				float CupidonChance = 0;
				if (skilltree.HavePerk(Perk.Cupidon))
				{
					CupidonChance = skilltree.nodeList.GetPerk(Perk.Cupidon).GetLevel * 0.05f;
				}
				if (Mathf.Random(0, 1) < CupidonChance)
				{
					Item.NewItem(target.getRect(), ItemID.Heart);
				}
				//StarGatherer Perk
				float StarChance = 0;
				if (skilltree.HavePerk(Perk.StarGatherer))
				{
					StarChance = skilltree.nodeList.GetPerk(Perk.StarGatherer).GetLevel * 0.05f;
				}
				if (Mathf.Random(0, 1) < StarChance)
				{
					Item.NewItem(target.getRect(), ItemID.Star);
				}
			}
			if (projMinion)
			{
				if (target.type != NPCID.TargetDummy)
				{
					if (item.DamageType == DamageClass.Summon)
						AddWeaponXp(damage / pen, item, 1);
					else
						AddWeaponXp(damage / pen, item, 0.5f);
				}

			}
			else if (!projMinion)
			{
				if (target.type != NPCID.TargetDummy)
					AddWeaponXp(damage / pen, Player.HeldItem);
			}

			Leech(damage);
			MPPacketHandler.SendNpcUpdate(Mod, target);
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			int pen = proj.penetrate;
			if (pen == 0) pen++;
			modifyHitGeneral(Player.HeldItem, target, ref damage, ref knockback, ref crit, pen) ;

		}

		#endregion

		private void DemonEaterPerk(NPC target, int damage)
		{
			if (skilltree.HavePerk(Perk.DemonEater))
			{
				if (damage > target.life) 
				{ 
					int heal = (int)(0.05f * skilltree.nodeList.GetPerk(Perk.DemonEater).GetLevel * Player.statLifeMax2);
					Player.statLife = Mathf.Clamp(heal + Player.statLife, Player.statLife, Player.statLifeMax2);

					CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + (int)heal);
				}
			}
		}

		public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
		{

			if (skilltree.HavePerk(Perk.Biologist))
			{
				float bonus = 1 + 0.2f * skilltree.nodeList.GetPerk(Perk.Biologist).GetLevel;
				healValue = Mathf.RoundInt(healValue * bonus);
			}
			base.GetHealLife(item, quickHeal, ref healValue);
		}

		public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
		{
			DemonEaterPerk(target,damage);
			base.OnHitNPC(item, target, damage, knockback, crit);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			DemonEaterPerk(target,damage);
			base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
		}

		#region ARMORXP
		private void AddArmorXp(int damage, float multiplier = 1)
	{
			damage = Mathf.Clamp(damage + Player.statDefense, 0, Player.statLifeMax2);
			Item armorItem;
			for (int i = 0;i< 3; i++)
			{
				armorItem = Player.armor[i];
				if (armorItem.Name!= "")
				{
					armorItem.GetGlobalItem<Items.ItemUpdate>().AddExp(Mathf.Ceillong(damage*multiplier), Player, armorItem);
				}
					
			}
		}

		public void dodge(ref int damage)
		{
			if (skilltree.ActiveClass != null )
			{
				float rand = Mathf.Random(0, 1);
				if (rand < JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Dodge)
				{
					CombatText.NewText(Player.getRect(), new Color(64, 255, 150), "Dodged");
					Player.ShadowDodge();
					//qint heal = damage;
					//Player.GetModPlayer<RPGPlayer>().ApplyReduction(ref damage);
					//Player.statLife = Mathf.Clamp(Player.statLife + damage, 0, Player.statLifeMax2);
					damage = 0;
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				}
			}
		}


		private void ModifyHitByGeneral(ref int damage)
		{
			if (Config.gpConfig.RPGPlayer) { 

				dodge(ref damage);
				if (damage == 0)
					return;

				if (skilltree.HavePerk(Perk.TheGambler))
				{
					if (Mathf.Random(0, 1) < 0.5f)
					{
						CombatText.NewText(Player.getRect(), new Color(64, 255, 64), "+" + damage);
						Player.statLife = Mathf.Clamp(Player.statLife + damage, 0, Player.statLifeMax2);
						damage = 0;
						return;
					}
					else
					{
						damage *= 2;
					}
				}
				ApplyReduction(ref damage);
				damage = ManaShieldReduction(damage);
			}
		}

		public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
		{

			ModifyHitByGeneral(ref damage);
			base.ModifyHitByNPC(npc, ref damage, ref crit);
		}
		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			ModifyHitByGeneral(ref damage);
			base.ModifyHitByProjectile(proj, ref damage, ref crit);
		}

		public override void OnHitByNPC(NPC npc, int damage, bool crit)
		{
			AddArmorXp(damage);
			

			

			if (damage > Player.statLife)
			{
				if (IsSaved())
					Player.statLife = Player.statLifeMax2;
			}

			base.OnHitByNPC(npc, damage, crit);
		}

		public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
		{
			AddArmorXp(damage);

			if (damage > Player.statLife)
			{
				if (IsSaved())
					Player.statLife = Player.statLifeMax2;
			}

			base.OnHitByProjectile(proj, damage, crit);
		}

		#endregion

		public struct ManaShieldInfo
		{
			public float DamageAbsorbtion;
			public float ManaPerDamage;

			public ManaShieldInfo(float _Absortion,float _ManaPerDamage)
			{
				DamageAbsorbtion = _Absortion;
				ManaPerDamage = _ManaPerDamage;

			}
		}
			
		public int ManaShieldReduction(int damage)
		{
			if (skilltree.ActiveClass == null || manaShieldDelay > 0 ||GetManaRatio() < 0.1f)
			{
				return damage;
			}

			ManaShieldInfo ShieldInfo = GetManaShieldInfo();
			if (ShieldInfo.DamageAbsorbtion == 0 || damage == 1)
				return damage;

			float defenseMult = 0.5f;
			if (Main.expertMode)
				defenseMult = 0.75f;

			int damageafterArmor = (int)(damage - Player.statDefense * defenseMult);

			float maxDamageAbsorbed = damageafterArmor * ShieldInfo.DamageAbsorbtion;
			float ManaCost = maxDamageAbsorbed / ShieldInfo.ManaPerDamage;
			ManaCost = Mathf.Clamp(ManaCost, 0, Player.statMana);
			if (ManaCost == 0)
				return damage;
			SoundEngine.PlaySound(SoundID.Drip);
			int reducedDamage = Mathf.CeilInt(ManaCost * ShieldInfo.ManaPerDamage);
			Player.statMana -= Mathf.CeilInt(ManaCost);
			manaShieldDelay = 120;
			Player.manaRegenDelay = 120;
			CombatText.NewText(Player.getRect(), new Color(64, 196, 255), "-"+ Mathf.CeilInt(ManaCost));


			return damage - reducedDamage;
		}

		public ManaShieldInfo GetManaShieldInfo()
		{
			float abs = JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaShield;
			float mp = JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaBaseEfficiency + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaEfficiency * Stats.GetStat(Stat.Int) ;
			return new ManaShieldInfo(abs,mp);
		}
		public float GetDefenceMult()
		{
				return (GetStatImproved(Stat.Vit) * 0.0025f + GetStatImproved(Stat.Cons) * 0.006f) * statMultiplier + 1f ;
		}

		public override void OnConsumeMana(Item item, int manaConsumed)
		{
			if (skilltree.HavePerk(Perk.BloodMage))
			{
				switch (skilltree.nodeList.GetPerk(Perk.BloodMage).GetLevel)
				{
					case 1:
						Player.statLife = Mathf.Clamp(Player.statLife - manaConsumed, 1, Player.statLifeMax2);
						break;
					case 2:
						Player.statLife = Mathf.Clamp(Player.statLife - manaConsumed * 3, 1, Player.statLifeMax2);
						break;
					case 3:
						Player.statLife = Mathf.Clamp(Player.statLife - manaConsumed * 9, 1, Player.statLifeMax2);
						break;
				}
					
			}
			base.OnConsumeMana(item, manaConsumed);
		}

		public bool IsSaved()
		{
			ItemUpdate instance;
			Item item;
			for (int i = 0; i < Player.armor.Length; i++)
			{
				item = Player.armor[i];

				if (item.netID > 0)
				{
					instance = item.GetGlobalItem<ItemUpdate>();
					if (instance != null)
						if (ModifierManager.HaveModifier(Modifier.Savior, instance.modifier))
							if (Mathf.Random(0, 100) < ModifierManager.GetModifierBonus(Modifier.Savior, instance))
							{
								CombatText.NewText(Player.getRect(), new Color(64, 196, 255), "SAVED !",true,true);
								return true;
							} 

				}
			}
			return false;
		}

		public float GetStatImprovementItem(Stat s)
		{
			float value = 1;
			ItemUpdate instance;
			Item item;
			if (!Config.gpConfig.ItemRarity)
				return value;
			int maxslot = Player.armor.Length;
			if (!Config.gpConfig.VanityGiveStat)
			{
				maxslot = 9;
			}

			for (int i = 0;i< maxslot; i++)
			{
				item = Player.armor[i];
					
				if (item.netID>0) {
					instance = item.GetGlobalItem<ItemUpdate>();
					if (instance != null)
						value += instance.GetStat(s)*0.01f;
						
				}   
			}   
			return value;
		}
		public float GetHealthPerHeart()
		{
				return (GetStatImproved(Stat.Vit) * 0.65f + GetStatImproved(Stat.Cons) * 0.325f)* statMultiplier + 10;
		}
		public float GetManaPerStar()
		{
				return (GetStatImproved(Stat.Foc) * 0.2f + GetStatImproved(Stat.Int) * 0.05f) * statMultiplier + 10;
		}

		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
		{
			if (basename == "")
				basename = Player.name;
			SendClientChanges(this);
			if (WorldManager.instance != null)
				WorldManager.instance.NetUpdateWorld();
			base.SyncPlayer(toWho, fromWho, newPlayer);
		}

		public override void SendClientChanges(ModPlayer clientPlayer)
		{
			if (clientPlayer == null || level <= 0)
				return;
			string classname = "Hobo";

			if (basename == "")
				basename = Player.name;

			if (skilltree!= null && skilltree.ActiveClass != null)
				classname = "" + skilltree.ActiveClass.GetClassType;
			string name = basename + " the lvl." + level + " " + classname;


			if (Config.gpConfig.RPGPlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)Message.SyncLevel);
				packet.Write((byte)Player.whoAmI);
				packet.Write(level);
				packet.Write(name);
				packet.Write(Player.statLife);
				packet.Write(Player.statLifeMax2);
				packet.Send();
			}
			base.SendClientChanges(clientPlayer);
		}

		public override void PreUpdateBuffs()
		{
			if (basename == "")
				basename = Player.name;
			if (Config.gpConfig.RPGPlayer)
			{

				if (!initiated)
				{
					initiated = true;
					
					skilltree.Init();
				}
				if (Player.HasBuff(24) || Player.HasBuff(20) || Player.HasBuff(67))
				{
					damageToApply += Mathf.Clamp(Mathf.Logx(Player.statLifeMax2, 1.002f) * 0.05f * NPCUtils.DELTATIME, 0, Player.statLifeMax2 * 0.015f * NPCUtils.DELTATIME);
				}
				if (Player.HasBuff(70) || Player.HasBuff(39) || Player.HasBuff(68))
				{
					damageToApply += Mathf.Clamp(Mathf.Logx(Player.statLifeMax2, 1.002f) * 0.08f * NPCUtils.DELTATIME, 0, Player.statLifeMax2 * 0.025f * NPCUtils.DELTATIME);
				}
				if (Player.onFire2 || Player.HasBuff(38) || Player.breath <= 0)
				{
					damageToApply += Mathf.Clamp(Mathf.Logx(Player.statLifeMax2, 1.002f)*0.1f * NPCUtils.DELTATIME, 0, Player.statLifeMax2 * 0.04f * NPCUtils.DELTATIME);
				}

				if (AnotherRpgMod.LoadedMods[SupportedMod.DBZMOD])
					IncreaseDBZKi(Player);

			if (damageToApply > 1)
				{
					int BuffDamage = Mathf.FloorInt(damageToApply);
					damageToApply -= damageToApply;
					ApplyReduction(ref BuffDamage);
					Player.statLife -= BuffDamage;
				}

				if (Main.netMode != NetmodeID.Server)
				{

					Player.GetCritChance(DamageClass.Melee) = (int)(Player.GetCritChance(DamageClass.Melee) + GetCriticalChanceBonus());
					Player.GetCritChance(DamageClass.Throwing) = (int)(Player.GetCritChance(DamageClass.Throwing) + GetCriticalChanceBonus());
					Player.GetCritChance(DamageClass.Magic) = (int)(Player.GetCritChance(DamageClass.Magic) + GetCriticalChanceBonus());
					Player.GetCritChance(DamageClass.Ranged) = (int)(Player.GetCritChance(DamageClass.Ranged) + GetCriticalChanceBonus());

					Player.maxMinions += skilltree.GetSummonSlot();
					if (!Config.gpConfig.ItemTree)
						Player.maxMinions += GetMaxMinion();

					Player.lifeRegen = Mathf.FloorInt(GetHealthRegen());
					Player.manaRegen = 0;

					

					//MODED DAMAGE
					if (AnotherRpgMod.LoadedMods[SupportedMod.Thorium])
					{
						UpdateThoriumDamage(Player);
					}

					if (AnotherRpgMod.LoadedMods[SupportedMod.DBZMOD])
					{
						UpdateDBZDamage(Player);
					}

				}
			}
				if (Player.HeldItem != null && Player.HeldItem.damage>0&& Player.HeldItem.maxStack <= 1) { 
					Items.ItemUpdate Item = Player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
					if (Item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
					{
							
						EquipedItemXp = Item.GetXp;
						EquipedItemMaxXp = Item.GetMaxXp;
					}
				}

		}
	private void IncreaseDBZKi(Player Player)
	{
		Mod DBZ = ModLoader.GetMod("DBZMOD");
		//Player.GetModPlayer<DBZMOD.MyPlayer>().kiMaxMult *= Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 10)),1,10);
		//Player.GetModPlayer<DBZMOD.MyPlayer>().kiChargeRate += Mathf.FloorInt( Mathf.Clamp((Mathf.Logx(GetStatImproved(Stat.Foc), 6)), 0, 25));
	}
	private void UpdateDBZDamage(Player Player)
	{
		Mod DBZ = ModLoader.GetMod("DBZMOD");
		//Player.GetModPlayer<DBZMOD.MyPlayer>().kiDamage *= GetDamageMult(DamageType.KI);
		//Player.GetModPlayer<DBZMOD.MyPlayer>().kiCrit += (int)GetCriticalChanceBonus();
	}
	private void UpdateThoriumDamage(Player Player)
	{
		Mod Thorium = ModLoader.GetMod("thoriumLoaded");
			/*
		Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicDamage *= GetDamageMult(DamageType.Symphonic);
		Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantBoost *= GetDamageMult(DamageType.Radiant);

		Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().radiantCrit += (int)GetCriticalChanceBonus();
		Player.GetModPlayer<ThoriumMod.ThoriumPlayer>().symphonicCrit += (int)GetCriticalChanceBonus();
			*/

	}

	public void ApplyReduction(ref int damage,bool heal = false)
	{
		if (m_virtualRes>0)
			CombatText.NewText(Player.getRect(), new Color(50, 26, 255,1), "("+damage + ")");
		damage = (int)(damage * (1 - m_virtualRes));
			   
	}

	public void ApplyReduction(ref float damage, bool heal = false)
	{
		if (m_virtualRes > 0)
			CombatText.NewText(Player.getRect(), new Color(50, 26, 255, 1), "(" + damage + ")");
		damage = (float)(damage * (1 - m_virtualRes));

	}

	public float GetArmorPenetrationMult()
		{
			return 1f + Stats.GetStat(Stat.Dex) * 0.01f;
		}

	public int GetArmorPenetrationAdd()
		{
			return Mathf.FloorInt( Stats.GetStat(Stat.Dex) * 0.1f);
		}



	public override void PostUpdateEquips()
	{
		   
		if (Config.gpConfig.RPGPlayer)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				m_virtualRes = 0;
				armor = Player.statDefense;

				 Player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue);
					/*
				if (Main.netMode == 1)
				{
					Player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10,10,Int16.MaxValue);
					if (Player.statLifeMax2>= Int16.MaxValue)
					{
						float HealthVirtualMult = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue) / Player.statLifeMax2;
						m_virtualRes = 1-(1f / HealthVirtualMult);
					}
							
				}
				else
				{
					Player.statLifeMax2 = Mathf.Clamp((int)(GetHealthMult() * Player.statLifeMax2 * GetHealthPerHeart() / 20) + 10, 10, int.MaxValue);
				}
				*/
				Player.statManaMax2 = (int)(Player.statManaMax2 * GetManaPerStar() / 20) + 10;
				Player.statDefense = (int)(GetDefenceMult() * Player.statDefense * GetArmorMult());
				Player.GetDamage(DamageClass.Melee) *= GetDamageMult(DamageType.Melee, 2);
				Player.GetDamage(DamageClass.Throwing) *= GetDamageMult(DamageType.Throw, 2);
				Player.GetDamage(DamageClass.Ranged) *= GetDamageMult(DamageType.Ranged, 2);
				Player.GetDamage(DamageClass.Magic) *= GetDamageMult(DamageType.Magic, 2);
				Player.GetDamage(DamageClass.Summon) *= GetDamageMult(DamageType.Summon, 2);



				Player.armorPenetration = Mathf.FloorInt( Player.armorPenetration*GetArmorPenetrationMult());
				Player.armorPenetration += GetArmorPenetrationAdd();

				manaShieldDelay = Mathf.Clamp(manaShieldDelay - 1, 0, manaShieldDelay);
				Player.manaCost *= (float)Math.Sqrt(GetDamageMult(DamageType.Magic, 2));

				
				ManaRegenPerSecond = Mathf.FloorInt(GetManaRegen());
				float manaRegenTick = (ManaRegenPerSecond / 60) + ManaRegenBuffer;
				int manaRegenThisTick = Mathf.Clamp(Mathf.FloorInt(manaRegenTick), 0, int.MaxValue);
					ManaRegenBuffer = Mathf.Clamp(manaRegenTick - manaRegenThisTick, 0, int.MaxValue);
				Player.statMana = Mathf.Clamp(Player.statMana + manaRegenThisTick, Player.statMana, Player.statManaMax2);
				
				if (skilltree.HavePerk(Perk.BloodMage))
				{
					switch (skilltree.nodeList.GetPerk(Perk.BloodMage).GetLevel)
					{
						case 1:
								Player.manaCost *= 0.5f;
							break;
						case 2:
								Player.manaCost *= 0.25f;
								break;
						case 3:
								Player.manaCost *= 0.1f;
								break;
					}

				}


				if (skilltree.ActiveClass != null)
				{
					Player.accRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
					Player.moveSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
					Player.maxRunSpeed *= (1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).MovementSpeed);
					Player.meleeSpeed *= 1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Speed;
					Player.manaCost *= 1 + JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).ManaCost;
				}
			}

			PostUpdatePerk();
		}
		
		UpdateModifier();
		//Issue : After one use of item , Player can no longer do anything wiht item&inventory
		//ErrorLogger.Log(Player.can);
		CustomPostUpdates();



		if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
		{
				MPPacketHandler.SendPlayerHealthSync(Mod, Player.whoAmI);
		}
	}

	private void PostUpdatePerk()
		{
			if (skilltree.HavePerk(Perk.Masochist))
			{
				int def = Player.statDefense;
				Player.statDefense = 0;
				Player.GetDamage(DamageClass.Generic) *= 1 + def * 0.01f * skilltree.nodeList.GetPerk(Perk.Masochist).GetLevel;
			}

			if (skilltree.HavePerk(Perk.Berserk))
			{
				float PerkMultiplier = 1 + 0.01f * skilltree.nodeList.GetPerk(Perk.Berserk).GetLevel * (1 - GetHealthRatio());
				Player.GetDamage(DamageClass.Generic) *= PerkMultiplier;
			}

			if (skilltree.HavePerk(Perk.Survivalist))
			{
				Player.GetDamage(DamageClass.Generic) *= 0.5f;

				Player.statDefense = Mathf.CeilInt(Player.statDefense * ( 1 + .2f * skilltree.nodeList.GetPerk(Perk.Survivalist).GetLevel));
			}
			if (skilltree.HavePerk(Perk.ManaOverBurst))
			{
				float bonusmanacost = Player.statMana * (0.1f + ((float)skilltree.nodeList.GetPerk(Perk.ManaOverBurst).GetLevel - 1) * 0.15f);
				float multiplier = 1 + (1 - 1 / (bonusmanacost / 1000 + 1));
				Player.GetDamage(DamageClass.Generic) *= multiplier;
			}
		}
	  

	public void SpendPoints(Stat _stat,int ammount)
	{
		ammount = Mathf.Clamp(ammount, 1, freePoints);
		Stats.UpgradeStat(_stat, ammount);
		freePoints -= ammount;
	}

	public void ResetStats()
	{
		Stats.Reset(level);
		freePoints = totalPoints;
	}

	public float GetCriticalChanceBonus()
	{
		float X = Mathf.Pow(GetStatImproved(Stat.Foc) * statMultiplier + GetStatImproved(Stat.Dex) * statMultiplier, 0.8f) * 0.05f;
		Mathf.Clamp(X, 0, 75);
		return X;
	}
	public float GetCriticalDamage()
	{
		float X = Mathf.Pow(GetStatImproved(Stat.Agi) * statMultiplier + GetStatImproved(Stat.Str) * statMultiplier, 0.8f)*0.005f;
		return 1.4f+X;
	}

		public override void UpdateAutopause()
		{

			//Main.npcChatRelease;
			if (!Main.drawingPlayerChat)
				HandleTrigger();

			base.UpdateAutopause();
		}

		private void HandleTrigger()
		{
			if (Config.gpConfig.RPGPlayer)
			{
				if (AnotherRpgMod.StatsHotKey.JustPressed)
				{



					SoundEngine.PlaySound(SoundID.MenuOpen);
					UI.Stats.Instance.LoadChar();
					UI.Stats.visible = !UI.Stats.visible;
				}
				if (AnotherRpgMod.SkillTreeHotKey.JustPressed)
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);


					UI.SkillTreeUi.visible = !UI.SkillTreeUi.visible;
					if (UI.SkillTreeUi.visible)
						UI.SkillTreeUi.Instance.LoadSkillTree();
				}
			}

			if (AnotherRpgMod.ItemTreeHotKey.JustPressed && Config.gpConfig.ItemTree)
			{
				if (ItemUpdate.NeedSavingStatic(Player.HeldItem))
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					UI.ItemTreeUi.visible = !UI.ItemTreeUi.visible;
					if (UI.ItemTreeUi.visible)
					{

						if (ItemUpdate.HaveTree(Player.HeldItem))
							UI.ItemTreeUi.Instance.Open(Player.HeldItem.GetGlobalItem<ItemUpdate>());
						else
							UI.ItemTreeUi.visible = false;
					}
				}
				else if (UI.ItemTreeUi.visible)
					UI.ItemTreeUi.visible = false;

			}

			for (int i = 0; i < 9; i++)
            {

            }


			if (AnotherRpgMod.HelmetItemTreeHotKey.JustPressed && Config.gpConfig.ItemTree)
			{
				if (ItemUpdate.NeedSavingStatic(Player.armor[0]))
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					UI.ItemTreeUi.visible = !UI.ItemTreeUi.visible;
					if (UI.ItemTreeUi.visible)
					{

						if (ItemUpdate.HaveTree(Player.armor[0]))
							UI.ItemTreeUi.Instance.Open(Player.armor[0].GetGlobalItem<ItemUpdate>());
						else
							UI.ItemTreeUi.visible = false;
					}
				}
				else if (UI.ItemTreeUi.visible)
					UI.ItemTreeUi.visible = false;
			}

			if (AnotherRpgMod.ChestItemTreeHotKey.JustPressed && Config.gpConfig.ItemTree)
			{
				if (ItemUpdate.NeedSavingStatic(Player.armor[1]))
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					UI.ItemTreeUi.visible = !UI.ItemTreeUi.visible;
					if (UI.ItemTreeUi.visible)
					{

						if (ItemUpdate.HaveTree(Player.armor[1]))
							UI.ItemTreeUi.Instance.Open(Player.armor[1].GetGlobalItem<ItemUpdate>());
						else
							UI.ItemTreeUi.visible = false;
					}
				}
				else if (UI.ItemTreeUi.visible)
					UI.ItemTreeUi.visible = false;
			}

			if (AnotherRpgMod.LegsItemTreeHotKey.JustPressed && Config.gpConfig.ItemTree)
			{
				if (ItemUpdate.NeedSavingStatic(Player.armor[2]))
				{
					SoundEngine.PlaySound(SoundID.MenuOpen);
					UI.ItemTreeUi.visible = !UI.ItemTreeUi.visible;
					if (UI.ItemTreeUi.visible)
					{

						if (ItemUpdate.HaveTree(Player.armor[2]))
							UI.ItemTreeUi.Instance.Open(Player.armor[2].GetGlobalItem<ItemUpdate>());
						else
							UI.ItemTreeUi.visible = false;
					}
				}
				else if (UI.ItemTreeUi.visible)
					UI.ItemTreeUi.visible = false;
			}
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			HandleTrigger();
		}
		public float GetBonusHeal()
		{
			if (Config.gpConfig.RPGPlayer)
				return GetHealthPerHeart() /20;
			return 1;
		}
		public float GetBonusHealMana()
		{
			if (Config.gpConfig.RPGPlayer)
				return (float)Math.Sqrt(GetManaPerStar() / 20);
			return 1;
		}

		public float GetHealthRegen()
		{
			float RegenMultiplier = 1f;
			if (skilltree.HavePerk(Perk.Chlorophyll))
			{
				RegenMultiplier = 0.25f + 0.25f * skilltree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel;
			}

			return (GetStatImproved(Stat.Vit) + GetStatImproved(Stat.Cons)) * 0.02f * statMultiplier * RegenMultiplier;
		}
		public float GetManaRegen()
		{
			float RegenMultiplier = 1f;
			if (skilltree.HavePerk(Perk.Chlorophyll))
			{
				RegenMultiplier = 0.25f + 0.25f * skilltree.nodeList.GetPerk(Perk.Chlorophyll).GetLevel;
			}
			if (Player.manaRegenDelay > 0)
				RegenMultiplier *= 0.5f;

			return (GetStatImproved(Stat.Int) + GetStatImproved(Stat.Spr)) * 0.02f * statMultiplier * RegenMultiplier;
		}

		

		public bool HaveRangedWeapon()
		{
			if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
			{
				return Player.HeldItem.DamageType == DamageClass.Ranged;
			}
			return false;
		}
		public bool HaveBow()
		{
			if (HaveRangedWeapon())
			{
				if (Player.HeldItem.useAmmo == 40)
					return true;
			}
			return false;
		}
		public float GetHealthMult()
		{
			float mult = 1;
			if (skilltree.ActiveClass != null)
				mult += JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Health;
			return mult;
		}
		public float GetArmorMult()
		{
			float mult = 1;
			if (skilltree.ActiveClass != null)
				mult += JsonCharacterClass.GetJsonCharList.GetClass(skilltree.ActiveClass.GetClassType).Armor;
			return mult;
		}
		public float GetDamageMult(DamageType type,int skill = 0)
		{
			if (skill == 0) { 
				switch (type)
				{
					case DamageType.Magic:
						return (GetStatImproved(Stat.Int) * MAINSTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
					case DamageType.Ranged:
						return (GetStatImproved(Stat.Agi) * MAINSTATSMULT + GetStatImproved(Stat.Dex) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
					case DamageType.Summon:
						return (GetStatImproved(Stat.Spr) * MAINSTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
					case DamageType.Throw:
						return (GetStatImproved(Stat.Dex) * MAINSTATSMULT + GetStatImproved(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
					case DamageType.Symphonic:
						return (GetStatImproved(Stat.Agi) * SECONDARYTATSMULT + GetStatImproved(Stat.Foc) * SECONDARYTATSMULT) *statMultiplier + 0.8f;
					case DamageType.Radiant:
						return (GetStatImproved(Stat.Int) * SECONDARYTATSMULT + GetStatImproved(Stat.Spr) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
				case DamageType.KI:
						return (GetStatImproved(Stat.Spr) * MAINSTATSMULT + GetStatImproved(Stat.Str) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
				default:
						return (GetStatImproved(Stat.Str) * MAINSTATSMULT + GetStatImproved(Stat.Agi) * SECONDARYTATSMULT) * statMultiplier + 0.8f;
				}
			}
			else if ( skill == 2 )
			{

				return skilltree.GetDamageMult(type) * statMultiplier * GetDamageMult(type, 0);
			} 
			else
			{
				return skilltree.GetDamageMult(type);
			}
		}


		public void CheckExp()
		{
			int actualLevelGained = 0;
			while (this.Exp >= XPToNextLevel())
			{
					
				actualLevelGained++;
				this.Exp -= XPToNextLevel();
				LevelUp();
				if (actualLevelGained > 5)
				{
					this.Exp = 0;
				}
			}
		}

		int ReduceExp(int xp, int _level)
		{
			int exp = xp;
			
			if (_level <= level - Config.gpConfig.XPReductionDelta)
			{
				float expMult = 1 - (level - _level) * 0.1f;
				exp = (int)(exp * expMult);
			}

			if (exp < 1)
				exp = 1;

			return exp;
		}
		public void AddXp(int exp,int _level)
		{
			if (Config.gpConfig.RPGPlayer)
			{

				

				exp = (int)(exp * GetXpMult());

				if (Config.gpConfig.XPReduction)
				{
					exp = ReduceExp(exp, _level);
				}

				if (level >= 1000 && !skilltree.IsLimitBreak())
				{

					if (!XpLimitMessage)
					{
						XpLimitMessage = true;
						Main.NewText("You character has Reached the limit of his mortal body and is unable to gain more power !");
					}
					exp = 0;
				}

				if (exp >= XPToNextLevel() * 0.1f)
				{
					CombatText.NewText(Player.getRect(), new Color(50, 26, 255), exp + " XP !!");
				}
				else
				{
					CombatText.NewText(Player.getRect(), new Color(127, 159, 255), exp + " XP");
				}

				

				this.Exp += exp;
				CheckExp();
			}
		}
		public void commandLevelup()
		{
			LevelUp(true);
		}

		public void ResetLevel()
		{
			ResetSkillTree();
			totalPoints = 0;
			freePoints = 0;
			skillPoints = 0;
			level = 1;
			Stats.Reset(1);
		}
		
		public void RecalculateStat()
		{
			int _level = level;
			level = 0;
			totalPoints = 0;
			freePoints = 0;
			Stats = new RPGStats();
			for (int i = 0; i< _level; i++)
			{
				LevelUp(true);
			}

		}

		public float GetLifeLeech(int damage)
		{
			float value = 0;
			if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
			{
				Items.ItemUpdate Item = Player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
				if (Item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
				{
					if (Config.gpConfig.ItemTree)
						value = Item.leech;
					else
						value = Item.GetLifeLeech*0.01f;
				}
			}
			
			

			if (Config.gpConfig.RPGPlayer)
			{
				if (skilltree.HavePerk(Perk.Vampire) && !Main.dayTime)
				{
					switch (skilltree.nodeList.GetPerk(Perk.Vampire).GetLevel)
					{
						case 1:
							value += 0.005f;
							break;
						case 2:
							value += 0.0075f;
							break;
						case 3:
							value += 0.01f;
							break;
					}
				}
				value += skilltree.GetLeech(LeechType.Life) + skilltree.GetLeech(LeechType.Both);
			}
			value *= Player.statLifeMax2;
			return value;
		}
		public float GetManaLeech()
		{
			float value = 0;
			if (Player.HeldItem != null && Player.HeldItem.damage > 0 && Player.HeldItem.maxStack <= 1)
			{
				Items.ItemUpdate Item = Player.HeldItem.GetGlobalItem<Items.ItemUpdate>();
				if (Item != null && ItemUpdate.NeedSavingStatic(Player.HeldItem))
				{
				if (!Config.gpConfig.ItemTree)
					value = Item.GetManaLeech * 0.01f;
				}
			}
			if (Config.gpConfig.RPGPlayer)
				value += skilltree.GetLeech(LeechType.Magic) + skilltree.GetLeech(LeechType.Both);
			return value;
		}


		private void LevelUpMessage(int pointsToGain)
		{
			CombatText.NewText(Player.getRect(), new Color(255, 25, 100), "LEVEL UP !!!!");
			CombatText.NewText(Player.getRect(), new Color(255, 125, 255), "+1 SKILL POINTS", true);
			CombatText.NewText(Player.getRect(), new Color(150, 100, 200), "+" + pointsToGain + " Ability points", true);
			Main.NewText(Player.name + " Is now level : " + level.ToString() + " .Congratulation !", 255, 223, 63);
		}

		private void LevelUp(bool silent = false)
		{
			int pointsToGain = 5 + Mathf.FloorInt(Mathf.Pow(level,0.5f));
			totalPoints += pointsToGain;
			freePoints += pointsToGain;
			skillPoints++;
			Stats.OnLevelUp();
			level++;
			if (!silent)
				LevelUpMessage(pointsToGain);

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				SendClientChanges(this);
			}
			else
				WorldManager.PlayerLevel = level;
		}
		public int XPToNextLevel()
		{
			return 15 * level + 5 * Mathf.CeilInt(Mathf.Pow(level, 1.8f)) + 40;
		}

		public int[] ConvertStatToInt()
		{
			int[] convertedStats = new int[8];
			for (int i = 0; i < 8; i++)
			{
				convertedStats[i] = GetAddStat((Stat)i);
			}
			return convertedStats;
		}

		public int[] ConvertStatXPToInt()
		{
			int[] convertedStats = new int[8];
			for (int i = 0; i < 8; i++)
			{
				convertedStats[i] = GetStatXP((Stat)i);
			}
			return convertedStats;
		}

		void LoadStats(int[] _level, int[] _xp)
		{
			if (_xp.Length != 8) //if save is not correct , will try to port
			{

				RecalculateStat();
				if (_level.Length != 8) //if port don't work
				{
					return;
				}
				for (int i = 0; i < 8; i++)
				{
					Stats.UpgradeStat((Stat)i, _level[i]);
				}
			}
			else
			{
				for (int i = 0; i < 8; i++)
				{
					Stats.SetStats((Stat)i,level+3,_level[i], _xp[i]);
				}
			}
				
		}

		public int[] SaveSkills ()
		{
			int[] skillLevels = new int[skilltree.nodeList.nodeList.Count];

			for(int i = 0; i< skillLevels.Length; i++)
			{
				skillLevels[i] = skilltree.nodeList.nodeList[i].GetLevel;
			}

			return skillLevels;

		}

		public void ResetSkillTree()
		{
			
			skilltree = new SkillTree();
			skillPoints = level - 1;

			UI.SkillTreeUi.Instance.LoadSkillTree();
			return;
		}

		void LoadSkills(int[] _skillLevel)
		{
			Reason CanUp;
			if (skilltree.nodeList.nodeList.Count < _skillLevel.Length)
			{
				AnotherRpgMod.Instance.Logger.Warn("Saved skill tree and Actual skill tree are of diferent size");
				ResetSkillTree();
			}
			for (int i = 0; i < _skillLevel.Length; i++)
			{

					
				if (_skillLevel[i] > 0) { 
					CanUp = skilltree.nodeList.nodeList[i].CanUpgrade(_skillLevel[i]* skilltree.nodeList.nodeList[i].GetCostPerLevel, level);
					if (!(CanUp == Reason.LevelRequirement || CanUp == Reason.MaxLevelReach))
					{
						for (int U = 0; U < _skillLevel[i]; U++)
						{
							if (skillPoints- skilltree.nodeList.nodeList[i].GetCostPerLevel >= 0) { 
								skillPoints -= skilltree.nodeList.nodeList[i].GetCostPerLevel;
								if (skilltree.nodeList.nodeList[i].GetNodeType != NodeType.Class)
									skilltree.nodeList.nodeList[i].Upgrade();
								else
									skilltree.nodeList.nodeList[i].Upgrade(true);

							}
						}
					}
					else
					{
						AnotherRpgMod.Instance.Logger.Warn("Can't level up node at rank : " + i);
						//ResetSkillTree();
						return;
					}
				}
			}
		}

		int GetActiveClassID()
		{
			if (Config.gpConfig.RPGPlayer)
				return -1;

			if (skilltree.ActiveClass == null)
			{
				return -1;
			}
			if (skilltree.ActiveClass.GetParent == null)
			{
				return -1;
			}
			return (int)skilltree.ActiveClass.GetParent.ID;
		}

		public override void SaveData(TagCompound tag)
		{
			//AnotherRpgMod.Instance.Logger.Info("Is it saving Player Data ? ....");
			base.SaveData(tag);
			if (Stats == null)
			{
				Stats = new RPGStats();
			}
			if (skilltree == null)
			{
				AnotherRpgMod.Instance.Logger.Warn("save skillTree reset");
				skilltree = new SkillTree();
			}
			AnotherRpgMod.Instance.Logger.Info("Saving this skilltree");
			AnotherRpgMod.Instance.Logger.Info(SkillTree.SKILLTREEVERSION);
			tag.Add("Exp", Exp);
			tag.Add("level", level);
			tag.Add("Stats", ConvertStatToInt());
			tag.Add("StatsXP", ConvertStatXPToInt());
			tag.Add("totalPoints", totalPoints);
			tag.Add("freePoints", freePoints);
			tag.Add("skillLevel", SaveSkills());
			tag.Add("activeClass", GetActiveClassID());
			tag.Add("AnRPGSkillVersion", SkillTree.SKILLTREEVERSION);
			tag.Add("AnRPGSaveVersion", ACTUALSAVEVERSION);
		}

		public override void Initialize()
		{
			if (Stats == null)
				Stats = new RPGStats();
			NodeParent.ResetID();
			if (skilltree == null)
			{
				skilltree = new SkillTree();
			}
				
		}

		public override void LoadData(TagCompound tag)
		{
			//base.LoadData(tag);
			//AnotherRpgMod.Instance.Logger.Info(tag);
			//AnotherRpgMod.Instance.Logger.Info("Load Player Data");
			NodeParent.ResetID();
			damageToApply = 0;
			Exp = tag.GetInt("Exp");
			level = tag.GetInt("level");

			LoadStats(tag.GetIntArray("Stats"), tag.GetIntArray("StatsXP"));
			totalPoints = tag.GetInt("totalPoints");

			freePoints = tag.GetInt("freePoints");
			skillPoints = level - 1;

			if (tag.GetInt("AnRPGSkillVersion") != SkillTree.SKILLTREEVERSION)
			{
				AnotherRpgMod.Instance.Logger.Warn(tag.GetInt("AnRPGSkillVersion"));
				AnotherRpgMod.Instance.Logger.Warn(SkillTree.SKILLTREEVERSION);
				AnotherRpgMod.Instance.Logger.Warn("AnRPG SkillTree Is Outdated, reseting skillTree");

			}
			else
			{
				LoadSkills(tag.GetIntArray("skillLevel"));
				if (tag.GetInt("activeClass") < skilltree.nodeList.nodeList.Count && tag.GetInt("activeClass") > 0)
				{
					skilltree.ActiveClass = (ClassNode)skilltree.nodeList.nodeList[tag.GetInt("activeClass")].GetNode;
				}
			}



			if (skilltree.ActiveClass == null)
			{
				skilltree.ActiveClass = (ClassNode)skilltree.nodeList.nodeList[0].GetNode;
			}

			for (int i = 0; i < skilltree.nodeList.nodeList.Count; i++)
			{
				NodeParent _node = skilltree.nodeList.nodeList[i];
				if (_node.GetNodeType == NodeType.Class)
				{
					ClassNode classNode = (ClassNode)_node.GetNode;
					if (classNode.GetClassType != skilltree.ActiveClass.GetClassType && classNode.GetActivate)
					{
						classNode.Disable(this);
					}

				}
			}

			NodeParent.ResetID();
			initiated = true;
		}

	}

		
		
		
}
