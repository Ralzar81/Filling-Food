// Project:         Climates & Cloaks mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2019 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace FillingFood
{

    public class FillingFood : MonoBehaviour
    {

        static Mod mod;

        [Invoke(StateManager.StateTypes.Game, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<FillingFood>();
            EntityEffectBroker.OnNewMagicRound += FoodEffects_OnNewMagicRound;

        }

        void Awake()
        {
            mod.IsReady = true;
            Debug.Log("Filling Food ready");
        }

        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static private bool foodTxt = true;
        static private bool hungerTxt = false;
        static private int foodCount = 0;

        private static void FoodEffects_OnNewMagicRound()
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            uint ateTime = GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern;
            uint hunger = (gameMinutes - ateTime);

            if ( hunger <= 240)
            {
                if (foodTxt)
                {
                    DaggerfallUI.AddHUDText("You feel invigorated by the meal.");
                    foodTxt = false;
                    hungerTxt = true;
                }

                foodCount += (200 - (int)hunger);

                Debug.Log(foodCount.ToString());

                if (foodCount >= 500)
                {
                    playerEntity.IncreaseFatigue(1, true);
                    foodCount = 0;
                    Debug.Log("FillingFood +1 Fatigue");
                }
            }
            else
            {
                foodTxt = true;
                if (hunger >= 240 && hungerTxt)
                {
                    DaggerfallUI.AddHUDText("Your stomach rumbles...");
                    hungerTxt = false;
                }
                Debug.Log("FillingFood Hungry");
            }
        }


    }
}
