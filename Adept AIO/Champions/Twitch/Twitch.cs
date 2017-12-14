namespace Adept_AIO.Champions.Twitch
{
    using Aimtec;
    using Core;
    using Drawings;
    using Miscellaneous;
    using OrbwalkingEvents;
    using SDK.Unit_Extensions;

    class Twitch
    {
        public Twitch()
        {
            new SpellManager();
            new MenuConfig();

            SpellBook.OnCastSpell += OnCastSpell;

            Render.OnPresent += DrawManager.OnPresent;
            Render.OnRender += DrawManager.OnRender;

            Global.Orbwalker.PostAttack += Combo.PostAttack;
            Game.OnUpdate += Manager.OnUpdate;
        }

        private static void OnCastSpell(Obj_AI_Base sender, SpellBookCastSpellEventArgs args)
        {
            if (sender.IsMe && SpellManager.Q.Ready && args.Slot == SpellSlot.Recall && MenuConfig.MainMenu["Stealth"].Enabled)
            {
                SpellManager.Q.Cast();
            }
        }
    }
}