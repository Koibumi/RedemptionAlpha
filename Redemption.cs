using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Items.Armor.PreHM;
using Redemption.Items.Donator.Arche;
using Redemption.Items.Usable;
using Redemption.StructureHelper;
using Redemption.StructureHelper.ChestHelper.GUI;
using Redemption.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using static Redemption.Globals.RedeNet;

namespace Redemption
{
    public class Redemption : Mod
    {
        public static Redemption Instance { get; private set; }

        public const string Abbreviation = "MoR";
        public const string EMPTY_TEXTURE = "Redemption/Empty";
        public Vector2 cameraOffset;

        private List<ILoadable> _loadCache;

        public static int AntiqueDorulCurrencyId;
        public static int dragonLeadCapeID;
        public static int archeFemLegID;

        public Redemption()
        {
            Instance = this;
        }

        public override void Load()
        {
            LoadCache();

            if (!Main.dedServ)
            {
                dragonLeadCapeID = AddEquipTexture(ModContent.GetInstance<DragonLeadRibplate>(), EquipType.Back, "Redemption/Items/Armor/PreHM/DragonLeadRibplate_Back");

                Main.QueueMainThreadAction(() =>
                {
                    Texture2D bubbleTex = ModContent.Request<Texture2D>("Redemption/Textures/BubbleShield",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref bubbleTex);
                    Texture2D portalTex = ModContent.Request<Texture2D>("Redemption/Textures/PortalTex",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref portalTex);
                    Texture2D holyGlowTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref holyGlowTex);
                    Texture2D whiteFlareTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteFlareTex);
                    Texture2D whiteOrbTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteOrb",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteOrbTex);
                    Texture2D whiteLightBeamTex = ModContent.Request<Texture2D>("Redemption/Textures/WhiteLightBeam",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref whiteLightBeamTex);
                    Texture2D transitionTex = ModContent.Request<Texture2D>("Redemption/Textures/TransitionTex",
                        ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    PremultiplyTexture(ref transitionTex);
                });
            }

            Filters.Scene["MoR:WastelandSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0.2f, 0f).UseOpacity(0.5f), EffectPriority.High);

            AntiqueDorulCurrencyId = CustomCurrencyManager.RegisterCurrency(new AntiqueDorulCurrency(ModContent.ItemType<AncientGoldCoin>(), 999L, "Antique Doruls"));
        }

        public static void PremultiplyTexture(ref Texture2D texture)
        {
            Color[] buffer = new Color[texture.Width * texture.Height];
            texture.GetData(buffer);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Color.FromNonPremultiplied(
                        buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
            }
            texture.SetData(buffer);
        }

        private void LoadCache()
        {
            _loadCache = new List<ILoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(ILoadable)))
                {
                    _loadCache.Add(Activator.CreateInstance(type) as ILoadable);
                }
            }

            _loadCache.Sort((x, y) => x.Priority > y.Priority ? 1 : -1);

            for (int i = 0; i < _loadCache.Count; ++i)
            {
                if (Main.dedServ && !_loadCache[i].LoadOnDedServer)
                {
                    continue;
                }

                _loadCache[i].Load();
            }
        }

        public ModPacket GetPacket(ModMessageType type, int capacity)
        {
            ModPacket packet = GetPacket(capacity + 1);
            packet.Write((byte)type);
            return packet;
        }
        public static ModPacket WriteToPacket(ModPacket packet, byte msg, params object[] param)
        {
            packet.Write(msg);

            for (int m = 0; m < param.Length; m++)
            {
                object obj = param[m];
                if (obj is bool boolean) packet.Write(boolean);
                else
                if (obj is byte @byte) packet.Write(@byte);
                else
                if (obj is int @int) packet.Write(@int);
                else
                if (obj is float single) packet.Write(single);
            }
            return packet;
        }
        public override void HandlePacket(BinaryReader bb, int whoAmI)
        {
            ModMessageType msgType = (ModMessageType)bb.ReadByte();
            //byte player;
            switch (msgType)
            {
                case ModMessageType.BossSpawnFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int bossType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        if (NPC.AnyNPCs(bossType))
                        {
                            return;
                        }

                        int npcID = NPC.NewNPC(npcCenterX, npcCenterY, bossType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
                        Main.npc[npcID].netUpdate2 = true;
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npcID].GetTypeNetName()), new Color(175, 75, 255));
                    }
                    break;
                case ModMessageType.NPCSpawnFromClient:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int NPCType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        if (NPC.AnyNPCs(NPCType))
                        {
                            return;
                        }

                        int npcID = NPC.NewNPC(npcCenterX, npcCenterY, NPCType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.NPCSpawnFromClient2:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        int NPCType = bb.ReadInt32();
                        int npcCenterX = bb.ReadInt32();
                        int npcCenterY = bb.ReadInt32();

                        int npcID = NPC.NewNPC(npcCenterX, npcCenterY, NPCType);
                        Main.npc[npcID].Center = new Vector2(npcCenterX, npcCenterY);
                        Main.npc[npcID].netUpdate2 = true;
                    }
                    break;
                case ModMessageType.SpawnTrail:
                    int projindex = bb.ReadInt32();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        WriteToPacket(GetPacket(), (byte)ModMessageType.SpawnTrail, projindex).Send();
                        break;
                    }

                    ITrailProjectile trailproj = Main.projectile[projindex].ModProjectile as ITrailProjectile;
                    if (trailproj != null)
                        trailproj.DoTrailCreation(RedeSystem.TrailManager);

                    break;
                    /*case ModMessageType.Dash:
                        player = bb.ReadByte();
                        DashType dash = (DashType)bb.ReadByte();
                        sbyte dir = bb.ReadSByte();
                        if (Main.netMode == NetmodeID.Server)
                        {
                            ModPacket packet = GetPacket(ModMessageType.Dash, 3);
                            packet.Write(player);
                            packet.Write((byte)dash);
                            packet.Write(dir);
                            packet.Send(-1, whoAmI);
                        }
                        Main.player[player].GetModPlayer<DashPlayer>().PerformDash(dash, dir, false);
                        break;
                    case ModMessageType.StartChickArmy:
                        ChickWorld.chickArmy = true;
                        ChickWorld.ChickArmyStart();
                        break;
                    case ModMessageType.ChickArmyData:
                        ChickWorld.HandlePacket(bb);
                        break;*/
            }
        }
    }
    public class RedeSystem : ModSystem
    {
        public static RedeSystem Instance { get; private set; }
        public RedeSystem()
        {
            Instance = this;
        }

        public static bool Silence;

        public override void PostUpdatePlayers()
        {
            Silence = false;
        }

        UserInterface GeneratorMenuUI;
        internal ManualGeneratorMenu GeneratorMenu;

        UserInterface ChestMenuUI;
        internal ChestCustomizerState ChestCustomizer;

        public UserInterface DialogueUILayer;
        public MoRDialogueUI DialogueUIElement;
        public UserInterface ChaliceUILayer;
        public ChaliceAlignmentUI ChaliceUIElement;

        public UserInterface TitleUILayer;
        public TitleCard TitleCardUIElement;

        public UserInterface NukeUILayer;
        public NukeDetonationUI NukeUIElement;

        public static TrailManager TrailManager;
        public bool Initialized;

        public override void Load()
        {
            RedeDetours.Initialize();
            if (!Main.dedServ)
            {
                On.Terraria.Main.Update += LoadTrailManager;

                TitleUILayer = new UserInterface();
                TitleCardUIElement = new TitleCard();
                TitleUILayer.SetState(TitleCardUIElement);

                DialogueUILayer = new UserInterface();
                DialogueUIElement = new MoRDialogueUI();
                DialogueUILayer.SetState(DialogueUIElement);

                ChaliceUILayer = new UserInterface();
                ChaliceUIElement = new ChaliceAlignmentUI();
                ChaliceUILayer.SetState(ChaliceUIElement);

                NukeUILayer = new UserInterface();
                NukeUIElement = new NukeDetonationUI();
                NukeUILayer.SetState(NukeUIElement);

                GeneratorMenuUI = new UserInterface();
                GeneratorMenu = new ManualGeneratorMenu();
                GeneratorMenuUI.SetState(GeneratorMenu);

                ChestMenuUI = new UserInterface();
                ChestCustomizer = new ChestCustomizerState();
                ChestMenuUI.SetState(ChestCustomizer);


            }
        }
        private void LoadTrailManager(On.Terraria.Main.orig_Update orig, Main self, GameTime gameTime)
        {
            if (!Initialized)
            {
                TrailManager = new TrailManager(Redemption.Instance);
                Initialized = true;
            }

            orig(self, gameTime);
        }

        public override void Unload()
        {
            TrailManager = null;
            On.Terraria.Main.Update -= LoadTrailManager;
        }

        public override void PreUpdateProjectiles()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                TrailManager.UpdateTrails();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (index >= 0)
                {
                    LegacyGameInterfaceLayer SkeleUI = new("Redemption: SkeleInvasion",
                        delegate
                        {
                            DrawSkeletonInvasionUI(Main.spriteBatch);
                            return true;
                        },
                        InterfaceScaleType.UI);
                    layers.Insert(index, SkeleUI);
                }
            }
            layers.Insert(layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text")), new LegacyGameInterfaceLayer("GUI Menus",
                delegate
                {
                    if (ManualGeneratorMenu.Visible)
                    {
                        GeneratorMenuUI.Update(Main._drawInterfaceGameTime);
                        GeneratorMenu.Draw(Main.spriteBatch);
                    }

                    if (ChestCustomizerState.Visible)
                    {
                        ChestMenuUI.Update(Main._drawInterfaceGameTime);
                        ChestCustomizer.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));
            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                AddInterfaceLayer(layers, ChaliceUILayer, ChaliceUIElement, MouseTextIndex, ChaliceAlignmentUI.Visible, "Chalice");
                AddInterfaceLayer(layers, DialogueUILayer, DialogueUIElement, MouseTextIndex + 1, MoRDialogueUI.Visible, "Dialogue");
                AddInterfaceLayer(layers, TitleUILayer, TitleCardUIElement, MouseTextIndex + 2, TitleCard.Showing, "Title Card");
                AddInterfaceLayer(layers, NukeUILayer, NukeUIElement, MouseTextIndex + 3, NukeDetonationUI.Visible, "Nuke UI");
            }
        }

        public static void AddInterfaceLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, string customName = null) //Code created by Scalie
        {
            string name;
            if (customName == null)
            {
                name = state.ToString();
            }
            else
            {
                name = customName;
            }
            layers.Insert(index, new LegacyGameInterfaceLayer("Redemption: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.UI));
        }

        #region Skele Invasion UI
        public static void DrawSkeletonInvasionUI(SpriteBatch spriteBatch)
        {
            if (RedeWorld.SkeletonInvasion)
            {
                float alpha = 0.5f;
                Texture2D backGround1 = TextureAssets.ColorBar.Value;
                Texture2D progressColor = TextureAssets.ColorBar.Value;
                Texture2D InvIcon = ModContent.Request<Texture2D>("Redemption/Items/Armor/Vanity/EpidotrianSkull").Value;
                float scmp = 0.5f + 0.75f * 0.5f;
                Color descColor = new(77, 39, 135);
                Color waveColor = new(255, 241, 51);
                const int offsetX = 20;
                const int offsetY = 20;
                int width = (int)(200f * scmp);
                int height = (int)(46f * scmp);
                Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 23f), new Vector2(width, height));
                Utils.DrawInvBG(spriteBatch, waveBackground, new Color(63, 65, 151, 255) * 0.785f);
                float cleared = (float)Main.time / 16200;
                string waveText = "Cleared " + Math.Round(100 * cleared) + "%";
                Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.X + waveBackground.Width / 2, waveBackground.Y + 5), Color.White, scmp * 0.8f, 0.5f, -0.1f);
                Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.X + waveBackground.Width * 0.5f, waveBackground.Y + waveBackground.Height * 0.75f), new Vector2(progressColor.Width, progressColor.Height));
                Rectangle waveProgressAmount = new(0, 0, (int)(progressColor.Width * MathHelper.Clamp(cleared, 0f, 1f)), progressColor.Height);
                Vector2 offset = new((waveProgressBar.Width - (int)(waveProgressBar.Width * scmp)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * scmp)) * 0.5f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, null, Color.White * alpha, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                spriteBatch.Draw(backGround1, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), scmp, SpriteEffects.None, 0f);
                const int internalOffset = 6;
                Vector2 descSize = new Vector2(154, 40) * scmp;
                Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - offsetX - 100f, Main.screenHeight - offsetY - 19f), new Vector2(width, height));
                Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), descSize * .8f);
                Utils.DrawInvBG(spriteBatch, descBackground, descColor * alpha);
                int descOffset = (descBackground.Height - (int)(32f * scmp)) / 2;
                Rectangle icon = new(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * scmp), (int)(32 * scmp));
                spriteBatch.Draw(InvIcon, icon, Color.White);
                Utils.DrawBorderString(spriteBatch, "Raveyard", new Vector2(barrierBackground.X + barrierBackground.Width * 0.5f, barrierBackground.Y - internalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
            }
        }
        #endregion

        #region StructureHelper Draw
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is CopyWand)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/corner");
                Texture2D tex2 = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/box1");
                Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).TopLeft;
                int Width = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).Width;
                int Height = (Main.LocalPlayer.HeldItem.ModItem as CopyWand).Height;

                float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
                Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
                pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
                pos = Vector2.Transform(pos, Main.UIScaleMatrix);

                spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

                if (Width != 0)
                {
                    spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
                    spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Red, 0, tex.Frame().Size() / 2, 1, 0, 0);
                }
                spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.Cyan, 0, tex.Frame().Size() / 2, 1, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }

            if (Main.LocalPlayer.HeldItem.ModItem is MultiWand)
            {
                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

                Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/corner");
                Texture2D tex2 = (Texture2D)ModContent.Request<Texture2D>("Redemption/StructureHelper/box1");
                Point16 TopLeft = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).TopLeft;
                int Width = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).Width;
                int Height = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).Height;
                int count = (Main.LocalPlayer.HeldItem.ModItem as MultiWand).StructureCache.Count;

                float tileScale = 16 * Main.GameViewMatrix.Zoom.Length() * 0.707106688737f;
                Vector2 pos = (Main.MouseWorld / tileScale).ToPoint16().ToVector2() * tileScale - Main.screenPosition;
                pos = Vector2.Transform(pos, Matrix.Invert(Main.GameViewMatrix.ZoomMatrix));
                pos = Vector2.Transform(pos, Main.UIScaleMatrix);

                spriteBatch.Draw(tex, pos, tex.Frame(), Color.White * 0.5f, 0, tex.Frame().Size() / 2, 1, 0, 0);

                if (Width != 0)
                {
                    spriteBatch.Draw(tex2, new Rectangle((int)(TopLeft.X * 16 - Main.screenPosition.X), (int)(TopLeft.Y * 16 - Main.screenPosition.Y), Width * 16 + 16, Height * 16 + 16), tex2.Frame(), Color.White * 0.15f);
                    spriteBatch.Draw(tex, (TopLeft.ToVector2() + new Vector2(Width + 1, Height + 1)) * 16 - Main.screenPosition, tex.Frame(), Color.Yellow, 0, tex.Frame().Size() / 2, 1, 0, 0);
                }
                spriteBatch.Draw(tex, TopLeft.ToVector2() * 16 - Main.screenPosition, tex.Frame(), Color.LimeGreen, 0, tex.Frame().Size() / 2, 1, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin();
                Utils.DrawBorderString(spriteBatch, "Structures to save: " + count, Main.MouseScreen + new Vector2(0, 30), Color.White);

                spriteBatch.End();
                spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            }
        }
        #endregion
    }
}