using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.Dusts;
using Redemption.Base;
using Redemption.Items.Armor.Vanity;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Redemption.BaseExtension;
using Redemption.Items.Usable;
using ReLogic.Content;

namespace Redemption.NPCs.Friendly
{
    [AutoloadHead]
    public class Newb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fool");
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 5;
            NPCID.Sets.AttackFrameCount[Type] = 5;
            NPCID.Sets.DangerDetectRange[Type] = 80;
            NPCID.Sets.AttackType[Type] = 3;
            NPCID.Sets.AttackTime[Type] = 18;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;

            NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Like);
            NPC.Happiness.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike);
            NPC.Happiness.SetBiomeAffection<OceanBiome>(AffectionLevel.Hate);

            NPC.Happiness.SetNPCAffection<Zephos>(AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection<Daerel>(AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.Wizard, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 46;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 9999;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

                new FlavorTextBestiaryInfoElement("..."),
            });
        }

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
                NPC.frame.X = RedeBossDowned.downedNebuleus ? NPC.frame.Width : 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 1) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (NPC.altTexture == 1)
            {
                Asset<Texture2D> hat = ModContent.Request<Texture2D>("Terraria/Images/Item_" + ItemID.PartyHat);
                int offset;
                switch (NPC.frame.Y / 52)
                {
                    default:
                        offset = 0;
                        break;
                    case 3:
                        offset = 2;
                        break;
                    case 4:
                        offset = 2;
                        break;
                    case 5:
                        offset = 2;
                        break;
                    case 10:
                        offset = 2;
                        break;
                    case 11:
                        offset = 2;
                        break;
                    case 12:
                        offset = 2;
                        break;
                    case 18:
                        offset = 2;
                        break;
                }
                var hatEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 origin = new(hat.Value.Width / 2f, hat.Value.Height / 2f);
                spriteBatch.Draw(hat.Value, NPC.Center - new Vector2(4 * NPC.spriteDirection, 24 + offset) - screenPos, null, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, hatEffects, 0);
            }
            return false;
        }
        public override void AI()
        {
            if (RedeWorld.newbGone)
                NPC.active = false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 15; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SightDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 4);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return RedeBossDowned.foundNewb && !RedeWorld.newbGone;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> { "Newb" };
        }
        public override ITownNPCProfile TownNPCProfile() => new NewbProfile();
        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            WeightedRandom<string> chat = new(Main.rand);
            if (RedeBossDowned.downedNebuleus)
            {
                if (RedeBossDowned.nebDeath > 6)
                {
                    chat.Add("I saw what you did.");
                    chat.Add("It is all coming back to me... I saw what you did... I can comprehend more than just the dirt below my feet now. I have something to say to you, but I am still not ready.");
                }
                chat.Add("My death, my sleep within the earth, had undone myself. But as my time awake grows longer, my lost self returns.");
                chat.Add("I can feel my memories arise from their deep slumber. There is something I must do, I know that much, yet I am uncertain as to what it is.");

            }
            else
            {
                if (BasePlayer.HasHelmet(player, ModContent.ItemType<KingSlayerMask>(), true))
                    chat.Add("Heheh! Hewwo mister slayer! Wait... who's that?");
                if (player.RedemptionPlayerBuff().ChickenForm)
                    chat.Add("IT'S A CHICKEN! Come on mister chicken, time for your walk!");
                else
                    chat.Add("Chickens very funny! I fed chicken grain but I threw a crown on floor instead, but chicken pecked it anyway! ... And then it exploded!!");/*
                if (BasePlayer.HasHelmet(player, ModContent.ItemType<ArmorHKHead>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHK>(), true) && BasePlayer.HasChestplate(player, ModContent.ItemType<ArmorHKLeggings>(), true))
                {
                    chat.Add("Do I know you?");
                }*/
                chat.Add("My shoes aren't muddy! Where is all the mud!?"); // 9.7%
                chat.Add("Trees here are funny colours! Where are yellow leaves! They all green! ... Green is good colour too.", 0.6); // 5.8%
                chat.Add("What's your name? Is it Garry? I bet it's Garry! Garry the Gentle is your name now!", 0.4); // 3.9%
                chat.Add("This island is not MY island! Where are my people!?", 0.4);
                chat.Add("They're coming, the red is coming! Don't stay! ... Oh hewwo!", 0.2); // 1.9%
                chat.Add("Me like emeralds, they green! Rubies me hate! Too red!", 0.2);
                chat.Add("What is beyond portal? Let's find out Johnny! ... Wait that isn't right name...", 0.2);
                chat.Add("Me sowwy! Me go with shiny knight!", 0.2);
                if (RedeWorld.alignment < 0)
                    chat.Add("Your ambitions are futile and will decayed, dare not proceed down the path of sin lest you face the very earth you walk upon. The death which lingers on your soul will consume you from within until you are but a husk unworthy of swift retribution.", 0.05); // 0.48%
                chat.Add("Who you? You human?");
                chat.Add("Me find shiny stones!");
                chat.Add("You look stupid! Haha!");
                chat.Add("My dirt is 10% off!");
                chat.Add("Heheheh!");
                chat.Add("Hewwo! I am Newb!");
            }
            return chat;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (!RedeBossDowned.downedNebuleus)
                button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = "Shop";
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add(ItemID.DirtBlock)
                .Add(ItemID.MudBlock)
                .Add(ItemID.Amethyst, RedeConditions.DownedEarlyGameBossAndMoR)
                .Add(ItemID.Topaz, RedeConditions.DownedEarlyGameBossAndMoR)
                .Add(ItemID.Sapphire, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(ItemID.Emerald, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(new Item(ItemID.Geode) { shopCustomPrice = Item.buyPrice(0, 2) }, RedeConditions.DownedEoCOrBoCOrKeeper)
                .Add(ItemID.Ruby, RedeConditions.DownedSkeletronOrSeed)
                .Add(ItemID.Diamond, RedeConditions.DownedSkeletronOrSeed)
                .Add<OreBomb>(Condition.InBelowSurface)
                .Add<OrePowder>(Condition.InBelowSurface, Condition.Hardmode);

            npcShop.Register();
        }
        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.type == ItemID.None)
                    continue;

                if (item.type is ItemID.Geode && NPC.downedBoss3)
                    item.shopCustomPrice = Item.buyPrice(0, 1);
            }
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 18;
            knockback = 4f;
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 34;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureAssets.Item[ItemID.WoodenSword].Value;
        }
    }
    public class NewbProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) => ModContent.Request<Texture2D>("Redemption/NPCs/Friendly/Newb");
        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Redemption/NPCs/Friendly/Newb_Head");
    }
}