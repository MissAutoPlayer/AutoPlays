using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoSharp.Auto;
using AutoSharp.Utils;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using AutoSharp.MainCore;
using AutoSharp.Spells;
using Autosharp;
using EloBuddy.SDK.Events;
using AutoSharp.Auto.HowlingAbyss;
using AutoSharp.MainCore.Utility;
using EloBuddy.SDK.Rendering;
using AutoSharp.MainCore.Logics;

namespace AutoSharp
{
    class Program
    {
        public static GameMapId Map;
        public static Menu Config;
        public static bool Loaded;

        public static float Timer;

        public static string Activemode;

        public static string Moveto;

        public static bool Moving;

        public static Menu MenuIni;

        private static bool _loaded = false;


        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingCompleteNoice;
        }

        private static void Loading_OnLoadingCompleteNoice(EventArgs args)
        {
            Map = Game.MapId;
            //Cache.Load();
            Game.OnEnd += OnEnd;
            Chat.Print("AutoSharp loaded", Color.CornflowerBlue);
            Timer = Game.Time;
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnLevelUp += LvlupSpells.Obj_AI_BaseOnOnLevelUp;
        }

        public static void Init()
        {
            MenuIni = MainMenu.AddMenu("DNK", "DNK");
            MenuIni.AddGroupLabel("DNK Settings");
            MenuIni.Add("DisableSpells", new CheckBox("Disable Built-in Casting Logic", false));
            MenuIni.Add("Safe", new Slider("Safe Slider (Recommended 1250)", 1250, 0, 2500));
            MenuIni.AddLabel("More Value = more defensive playstyle");
            MenuIni.AddSeparator();
            MenuIni.AddLabel("Edit by MissAuto @credits to DNK");
            // Initialize Bot Functions.
            Brain.Init();
            Autoplay.Load();
            Drawing.OnEndScene += Drawing_OnEndScene;
            Chat.Print("Autoplayer Loaded !");
        }

        private static void Events_OnGameEnd(EventArgs args)
        {
            Core.DelayAction(() => Game.QuitGame(), 250);
        }

        public static void OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender == null) return;
            if (args.Target.NetworkId == ObjectManager.Player.NetworkId && (sender is Obj_AI_Turret || sender is Obj_AI_Minion))
            {
                Orbwalker.OrbwalkTo(
                    Heroes.Player.Position.Extend(Wizard.GetFarthestMinion().Position, 500).RandomizePosition());
            }
        }

        private static void AntiShrooms2(EventArgs args)
        {
                if (Heroes.Player.HealthPercent > 0 && Heroes.Player.CountEnemiesInRange(1800) == 0 &&
                    !Turrets.EnemyTurrets.Any(t => t.Distance(Heroes.Player) < 950) &&
                    !Minions.EnemyMinions.Any(m => m.Distance(Heroes.Player) < 950))
                {
                 
                }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            Drawing.DrawText(
                Drawing.Width * 0.01f,
                Drawing.Height * 0.025f,
                System.Drawing.Color.White,
                " | MoveTo: " + Moveto + " | ActiveMode: " + Orbwalker.ActiveModesFlags + " | SafeToDive: " + Misc.SafeToDive);

            Drawing.DrawText(
                Game.CursorPos.WorldToScreen().X + 50,
                Game.CursorPos.WorldToScreen().Y,
                System.Drawing.Color.Goldenrod,
                (Misc.TeamTotal(Game.CursorPos) - Misc.TeamTotal(Game.CursorPos, true)).ToString(),
                5);

            foreach (var hr in ObjectsManager.HealthRelics.Where(h => h.IsValid))
            {
                Circle.Draw(Color.White, hr.BoundingRadius * 2, hr.Position);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (!Loaded)
            {
                if (Game.Time - Timer >= 10)
                {
                    Loaded = true;

                    // Initialize The Bot.
                    Init();
                }
            }
            else
            {
                if (Player.Instance.IsDead)
                {
                    return;
                }
                Brain.Decisions();
            }
        }

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }
        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

            if (sender.Owner.IsMe)
            {
                if (sender.Owner.IsDead)
                {
                    args.Process = false;
                    return;
                }

                }
            var target = TargetSelector.GetTarget(1000, DamageType.Magical);
            if (Heroes.Player.UnderTurret(true) && target.IsValidTarget(1000))
                {
                    args.Process = false;
                    return;
                }
        }

        private static void OnEnd(GameEndEventArgs args)
        {
            /*
            if (Config.Item("autosharp.quit").GetValue<bool>())
            {
                Thread.Sleep(30000);
                Game.QuitGame();
            }
             * */
            Thread.Sleep(35000);
            Game.QuitGame();
        }

        public static void AntiShrooms(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != null && sender.IsMe)
            {
                if (sender.IsDead)
                {
                    args.Process = false;
                    return;
                }
                var turret = Turrets.ClosestEnemyTurret;
                if (Heroes.Player.CountEnemiesInRange(1800) == 0 &&
                    turret.Distance(Heroes.Player) > 950 && !Minions.EnemyMinions.Any(m => m.Distance(Heroes.Player) < 950))
                {
                    args.Process = false;
                    return;
                }

                //if (args.Order == GameObjectOrder.MoveTo)
                //{
                //    if (args.TargetPosition.IsZero)
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //    if (!args.TargetPosition.IsValid())
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //    if (turret != null && turret.Distance(args.TargetPosition) < 950 &&
                //        turret.CountNearbyAllyMinions(950) < 3)
                //    {
                //        args.Process = false;
                //        return;
                //    }
                //}

                #region BlockAttack
                var target = TargetSelector.GetTarget(1000, DamageType.Magical);
                if (target.IsValidTarget(800))
                {

                    //if (Config.Item("onlyfarm").GetValue<bool>() && args.Target.IsValid<Obj_AI_Hero>())
                    //{
                    //    args.Process = false;
                    //    return;
                    //}

                        if (Minions.AllyMinions.Count(m => m.Distance(Heroes.Player) < 900) <
                            Minions.EnemyMinions.Count(m => m.Distance(Heroes.Player) < 900))
                        {
                            args.Process = false;
                            return;
                        }
                        //if (((myHero)args.Target).UnderTurret(true))
                        //{
                        //    args.Process = false;
                        //    return;
                        //}
                    }
                    if (Heroes.Player.UnderTurret(true))
                    {
                        args.Process = false;
                        return;
                    }
                    if (turret != null && turret.Distance(ObjectManager.Player) < 950 && turret.CountNearbyAllyMinions(950) < 3)
                    {
                        args.Process = false;
                        return;
                    }
                }

                #endregion
            }
            //if (sender != null && args.Target != null && args.Target.IsMe)
            //{
            //    if (sender is Obj_AI_Turret || sender is Obj_AI_Minion)
            //    {
            //        var minion = Wizard.GetClosestAllyMinion();
            //        if (minion != null)
            //        {
            //            Orbwalker.SetOrbwalkingPoint(
            //                Heroes.Player.Position.Extend(Wizard.GetClosestAllyMinion().Position, Heroes.Player.Distance(minion) + 100));
            //        }
            //    }
            //}
        }
    }

