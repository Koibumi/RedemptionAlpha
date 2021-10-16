using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class MoonflareDebuff : ModBuff
    {
        public override string Texture => "_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moonflare");
            Description.SetDefault(":(");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().moonflare = true;
        }
    }
}