// Project:         Filling Food mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2020 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar


using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;
using DaggerfallWorkshop;

using DaggerfallConnect;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Serialization;

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

            EntityEffectBroker.OnNewMagicRound += FoodRot_OnNewMagicRound;

            ItemHelper itemHelper = DaggerfallUnity.Instance.ItemHelper;

            itemHelper.RegisterCustomItem(531, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(531, EatFood);
            itemHelper.RegisterCustomItem(532, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(532, EatFood);
            itemHelper.RegisterCustomItem(533, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(533, EatFood);
            itemHelper.RegisterCustomItem(534, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(534, EatFood);
            itemHelper.RegisterCustomItem(535, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(535, EatFood);
            itemHelper.RegisterCustomItem(536, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(536, EatFood);
            itemHelper.RegisterCustomItem(537, ItemGroups.UselessItems2);
            itemHelper.RegisterItemUseHander(537, EatFood);
        }

        void Awake()
        {
            mod.IsReady = true;
            Debug.Log("[FillingFood Food] Mod Is Ready");
        }

        static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        static private bool hungry = true;
        static private int foodCount = 0;
        static private uint gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
        static private uint ateTime = GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern;
        static private uint hunger = gameMinutes - ateTime;

        void Update()
        {
            gameMinutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            ateTime = GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern;
            hunger = gameMinutes - ateTime;
            if (hunger <= 240 && hungry)
            {
                hungry = false;
                EntityEffectBroker.OnNewMagicRound += FoodEffects_OnNewMagicRound;
                DaggerfallUI.AddHUDText("You feel invigorated by the meal.");
                Debug.Log("[FillingFood Food] Registering OnNewMagicRound");
            }
        }


        public static bool EatFood(DaggerfallUnityItem item, ItemCollection collection)
        {
            uint cal = 240;
            if (item.TemplateIndex == 531) //Provisions
            {
                cal -= 120;
            }
            else if (item.TemplateIndex == 532 || item.TemplateIndex == 533) //Apple or Orange
            {
                cal -= 60;
            }
            else if (item.TemplateIndex == 534) //Bread
            {
                cal -= 120;
            }
            else if (item.TemplateIndex == 535) //Fish
            {
                cal -= 180;
            }
            else if (item.TemplateIndex == 536) //Salted Fish
            {
                cal -= 120;
            }
            else if (item.TemplateIndex == 537) //Meat
            {
                cal -= 240;
            }

            if (hunger > 240)
            {
                GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern = gameMinutes - cal;
                collection.RemoveItem(item);
            }
            else if (hunger > 240-cal )
            {
                GameManager.Instance.PlayerEntity.LastTimePlayerAteOrDrankAtTavern += (240 - cal);
                collection.RemoveItem(item);
            }
            else
            {
                DaggerfallUI.MessageBox(HardStrings.youAreNotHungry);
            }
            return true;
        }

        static private void FoodRot()
        {
            int rot = 0;
            foreach (ItemCollection playerItems in new ItemCollection[] { GameManager.Instance.PlayerEntity.Items, GameManager.Instance.PlayerEntity.WagonItems })
            {
                for (int i = 0; i < playerItems.Count; i++)
                {
                    DaggerfallUnityItem item = playerItems.GetItem(i);
                    if (item.TemplateIndex > 530 && item.TemplateIndex < 540)
                    {
                        if (item.currentCondition >= 1)
                        {
                            rot = Random.Range(0, 4);
                            item.LowerCondition(rot);
                            Debug.LogFormat("[Filling Food] {0} rotted {1}", item.shortName, rot);
                        }                        
                    }
                }
            }
        }

        private static int rotCounter = 0;

        private static void FoodRot_OnNewMagicRound()
        {
            if (!SaveLoadManager.Instance.LoadInProgress)
            {
                rotCounter++;
                if (rotCounter > 60)
                {
                    FoodRot();
                    rotCounter = 0;
                    Debug.Log("[Filling Food] Food rotted");
                }
            }
        }

        private static void FoodEffects_OnNewMagicRound()
        {
            Debug.Log("[FillingFood Food] Round Start");
            Debug.Log("[Filling Food] Hunger = " + hunger.ToString());
            if ( hunger <= 239)
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
