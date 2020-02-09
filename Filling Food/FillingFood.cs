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

        }

        void Awake()
        {
            mod.IsReady = true;
            Debug.Log("[FillingFood Food] Mod Is Ready");
        }

        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static private bool hungry = true;
        static private int foodCount = 0;


        void Update()
        {
            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            uint ateTime = GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern;
            uint hunger = (gameMinutes - ateTime);
            if (hunger <= 240 && hungry)
            {
                hungry = false;
                EntityEffectBroker.OnNewMagicRound += FoodEffects_OnNewMagicRound;
                DaggerfallUI.AddHUDText("You feel invigorated by the meal.");
                Debug.Log("[FillingFood Food] Registering OnNewMagicRound");
            }
        }



        private static void FoodEffects_OnNewMagicRound()
        {
            Debug.Log("[FillingFood Food] Round Start");

            uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            uint ateTime = GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern;
            uint hunger = (gameMinutes - ateTime);

            if ( hunger <= 240)
            {
                foodCount += (200 - (int)hunger);
                Debug.Log(foodCount.ToString());
                if (foodCount >= 500)
                {
                    playerEntity.IncreaseFatigue(1, true);
                    foodCount = 0;
                    Debug.Log("[FillingFood Food] +1 Fatigue");
                }
            }
            else
            {
                Debug.Log("[FillingFood Food] Hungry");
                hungry = true;
                DaggerfallUI.AddHUDText("Your stomach rumbles...");
                EntityEffectBroker.OnNewMagicRound -= FoodEffects_OnNewMagicRound;
                Debug.Log("[FillingFood Food] De-registering from OnNewMagicRound");
            }
            Debug.Log("[FillingFood Food] Round End");
        }
    }
}
