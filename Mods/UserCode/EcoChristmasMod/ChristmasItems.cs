namespace Eco.Mods.TechTree
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Eco.Core.Items;
    using Eco.Gameplay.Blocks;
    using Eco.Gameplay.Components;
    using Eco.Gameplay.Components.Auth;
    using Eco.Gameplay.DynamicValues;
    using Eco.Gameplay.Economy;
    using Eco.Gameplay.Housing;
    using Eco.Gameplay.Interactions;
    using Eco.Gameplay.Items;
    using Eco.Gameplay.Modules;
    using Eco.Gameplay.Minimap;
    using Eco.Gameplay.Objects;
    using Eco.Gameplay.Occupancy;
    using Eco.Gameplay.Players;
    using Eco.Gameplay.Property;
    using Eco.Gameplay.Skills;
    using Eco.Gameplay.Systems;
    using Eco.Gameplay.Systems.TextLinks;
    using Eco.Gameplay.Pipes.LiquidComponents;
    using Eco.Gameplay.Pipes.Gases;
    using Eco.Shared;
    using Eco.Shared.Math;
    using Eco.Shared.Localization;
    using Eco.Shared.Serialization;
    using Eco.Shared.Utils;
    using Eco.Shared.View;
    using Eco.Shared.Items;
    using Eco.Shared.Networking;
    using Eco.Gameplay.Pipes;
    using Eco.World.Blocks;
    using Eco.Gameplay.Housing.PropertyValues;
    using Eco.Gameplay.Civics.Objects;
    using Eco.Gameplay.Settlements;
    using Eco.Gameplay.Systems.NewTooltip;
    using Eco.Core.Controller;
    using Eco.Core.Utils;
    using Eco.Gameplay.Components.Storage;
    using static Eco.Gameplay.Housing.PropertyValues.HomeFurnishingValue;
    using Eco.Gameplay.Items.Recipes;
    using Eco.Core.Plugins.Interfaces;

    public class ChristmasItems : IModInit
    {
        public static ModRegistration Register() => new()
        {
            ModName = "ChristmasItems",
            ModDescription = "ChristmasItems est un mod qui ajoute des décorations de Noël festives au jeu Eco.",
            ModDisplayName = "Christmas Items",
        };
    }

    static class ChristmasHousingValues
    {
        private static readonly LocString HolidayLimit = Localizer.DoStr("Christmas decoration");

        public static HomeFurnishingValue Create<TObject>(float baseValue, string roomCategory = "Decoration")
            where TObject : WorldObject
            => new()
            {
                ObjectName = typeof(TObject).UILink(),
                Category = HousingConfig.GetRoomCategory(roomCategory),
                BaseValue = baseValue,
                TypeForRoomLimit = HolidayLimit,
                DiminishingReturnMultiplier = 0.75f,
            };
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Père Noël gonflable")]
    public partial class SmallInflatableSantaObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(SmallInflatableSantaItem);
        public override LocString DisplayName => Localizer.DoStr("Inflatable Santa");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = SmallInflatableSantaItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Inflatable Santa")]
    [LocDescription("An inflatable decoration that instantly brings the Christmas spirit to your garden.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(500)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class SmallInflatableSantaItem : WorldObjectItem<SmallInflatableSantaObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext( 0  | DirectionAxisFlags.Down , WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = new()
        {
            ObjectName = typeof(SmallInflatableSantaObject).UILink(),
            Category = HousingConfig.GetRoomCategory("Decoration"),
            BaseValue = 3f,
            TypeForRoomLimit = Localizer.DoStr("Christmas decoration"),
            DiminishingReturnMultiplier = 0.5f,
        };
    }

    [RequiresSkill(typeof(TailoringSkill), 4)]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Père Noël gonflable")]
    public partial class SmallInflatableSantaRecipe : RecipeFamily
    {
        public SmallInflatableSantaRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "SmallInflatableSanta",
                displayName: Localizer.DoStr("Inflatable Santa"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement("Fabric", 20, typeof(TailoringSkill), typeof(TailoringLavishResourcesTalent)),
                    new IngredientElement(typeof(PlasticItem), 5, typeof(TailoringSkill), typeof(TailoringLavishResourcesTalent)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<SmallInflatableSantaItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 3;

            this.LaborInCalories = CreateLaborInCaloriesValue(200, typeof(TailoringSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(SmallInflatableSantaRecipe), start: 8, skillType: typeof(TailoringSkill), typeof(TailoringFocusedSpeedTalent), typeof(TailoringParallelSpeedTalent));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Inflatable Santa"), recipeType: typeof(SmallInflatableSantaRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(TailoringTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    // ______________________________________________________ Sapins de Noël ______________________________________________________ \

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [RequireRoomVolume(1)]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Petit sapin de Noël")]
    public partial class ChristmasTreeSmallObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChristmasTreeSmallItem);
        public override LocString DisplayName => Localizer.DoStr("Small Christmas Tree");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = ChristmasTreeSmallItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [RequireRoomVolume(3)]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Sapin de Noël moyen")]
    public partial class ChristmasTreeMediumObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChristmasTreeMediumItem);
        public override LocString DisplayName => Localizer.DoStr("Medium Christmas Tree");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = ChristmasTreeMediumItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Grand sapin de Noël")]
    public partial class ChristmasTreeLargeObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChristmasTreeLargeItem);
        public override LocString DisplayName => Localizer.DoStr("Large Christmas Tree");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = ChristmasTreeLargeItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Immense sapin de Noël")]
    public partial class ChristmasTreeHugeObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(ChristmasTreeHugeItem);
        public override LocString DisplayName => Localizer.DoStr("Huge Christmas Tree");
        public override TableTextureMode TableTexture => TableTextureMode.Wood;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = ChristmasTreeHugeItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Small Christmas Tree")]
    [LocDescription("A compact tree that's perfect for tight spaces.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(500)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class ChristmasTreeSmallItem : WorldObjectItem<ChristmasTreeSmallObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<ChristmasTreeSmallObject>(4f);
    }

    [Serialized]
    [LocDisplayName("Medium Christmas Tree")]
    [LocDescription("A generously decorated tree that brightens any living room.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(1500)]
    public partial class ChristmasTreeMediumItem : WorldObjectItem<ChristmasTreeMediumObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<ChristmasTreeMediumObject>(5f);
    }

    [Serialized]
    [LocDisplayName("Large Christmas Tree")]
    [LocDescription("A majestic centerpiece that draws every eye.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(2000)]
    public partial class ChristmasTreeLargeItem : WorldObjectItem<ChristmasTreeLargeObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<ChristmasTreeLargeObject>(6f);
    }

    [Serialized]
    [LocDisplayName("Huge Christmas Tree")]
    [LocDescription("The ultimate holiday symbol, covered in sparkling ornaments.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(3000)]
    public partial class ChristmasTreeHugeItem : WorldObjectItem<ChristmasTreeHugeObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<ChristmasTreeHugeObject>(7f);
    }

    [RequiresSkill(typeof(LoggingSkill), 2)]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Petit sapin de Noël")]
    public partial class ChristmasTreeSmallRecipe : RecipeFamily
    {
        public ChristmasTreeSmallRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "ChristmasTreeSmall",
                displayName: Localizer.DoStr("Small Christmas Tree"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(FirLogItem), 10, typeof(LoggingSkill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ChristmasTreeSmallItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 3;

            this.LaborInCalories = CreateLaborInCaloriesValue(250, typeof(LoggingSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ChristmasTreeSmallRecipe), start: 6, skillType: typeof(LoggingSkill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Small Christmas Tree"), recipeType: typeof(ChristmasTreeSmallRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [RequiresSkill(typeof(LoggingSkill), 4)]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Sapin de Noël moyen")]
    public partial class ChristmasTreeMediumRecipe : RecipeFamily
    {
        public ChristmasTreeMediumRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "ChristmasTreeMedium",
                displayName: Localizer.DoStr("Medium Christmas Tree"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(FirLogItem), 25, typeof(LoggingSkill)),
                    new IngredientElement(typeof(ClayItem), 8, typeof(LoggingSkill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ChristmasTreeMediumItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 4;

            this.LaborInCalories = CreateLaborInCaloriesValue(320, typeof(LoggingSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ChristmasTreeMediumRecipe), start: 7, skillType: typeof(LoggingSkill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Medium Christmas Tree"), recipeType: typeof(ChristmasTreeMediumRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [RequiresSkill(typeof(LoggingSkill), 5)]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Grand sapin de Noël")]
    public partial class ChristmasTreeLargeRecipe : RecipeFamily
    {
        public ChristmasTreeLargeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "ChristmasTreeLarge",
                displayName: Localizer.DoStr("Large Christmas Tree"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(FirLogItem), 45, typeof(LoggingSkill)),
                    new IngredientElement(typeof(ClayItem), 12, typeof(LoggingSkill)),
                    new IngredientElement(typeof(GlassItem), 4, typeof(LoggingSkill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ChristmasTreeLargeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 5;

            this.LaborInCalories = CreateLaborInCaloriesValue(420, typeof(LoggingSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ChristmasTreeLargeRecipe), start: 8, skillType: typeof(LoggingSkill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Large Christmas Tree"), recipeType: typeof(ChristmasTreeLargeRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [RequiresSkill(typeof(LoggingSkill), 6)]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Immense sapin de Noël")]
    public partial class ChristmasTreeHugeRecipe : RecipeFamily
    {
        public ChristmasTreeHugeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "ChristmasTreeHuge",
                displayName: Localizer.DoStr("Huge Christmas Tree"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(FirLogItem), 60, typeof(LoggingSkill)),
                    new IngredientElement(typeof(ClayItem), 16, typeof(LoggingSkill)),
                    new IngredientElement(typeof(GlassItem), 6, typeof(LoggingSkill)),
                    new IngredientElement(typeof(LightBulbItem), 4, typeof(LoggingSkill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<ChristmasTreeHugeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 6;

            this.LaborInCalories = CreateLaborInCaloriesValue(520, typeof(LoggingSkill));

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(ChristmasTreeHugeRecipe), start: 10, skillType: typeof(LoggingSkill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Huge Christmas Tree"), recipeType: typeof(ChristmasTreeHugeRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(CarpentryTableObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    // ______________________________________________________ cadeaux ______________________________________________________ \

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Petit cadeau")]
    public partial class PresentSmallObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(PresentSmallItem);
        public override LocString DisplayName => Localizer.DoStr("Small Present");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = PresentSmallItem.homeValue;
            var storage = this.GetComponent<PublicStorageComponent>();
            storage.Initialize(1);
            storage.Storage.AddInvRestriction(new NotCarriedRestriction()); // can't store block or large items
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Cadeau moyen")]
    public partial class PresentMediumObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(PresentMediumItem);
        public override LocString DisplayName => Localizer.DoStr("Medium Present");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = PresentMediumItem.homeValue;
            var storage = this.GetComponent<PublicStorageComponent>();
            storage.Initialize(1);
            storage.Storage.AddInvRestriction(new NotCarriedRestriction());
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Grand cadeau")]
    public partial class PresentLargeObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(PresentLargeItem);
        public override LocString DisplayName => Localizer.DoStr("Large Present");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = PresentLargeItem.homeValue;
            var storage = this.GetComponent<PublicStorageComponent>();
            storage.Initialize(1);
            storage.Storage.AddInvRestriction(new NotCarriedRestriction());
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(LinkComponent))]
    [RequireComponent(typeof(PublicStorageComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Énorme cadeau")]
    public partial class PresentHugeObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(PresentHugeItem);
        public override LocString DisplayName => Localizer.DoStr("Huge Present");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;
        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = PresentHugeItem.homeValue;
            var storage = this.GetComponent<PublicStorageComponent>();
            storage.Initialize(1);
            storage.Storage.AddInvRestriction(new NotCarriedRestriction());
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Small Present")]
    [LocDescription("A small present carefully wrapped to place under the tree.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(200)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class PresentSmallItem : WorldObjectItem<PresentSmallObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<PresentSmallObject>(1f);
    }

    [Serialized]
    [LocDisplayName("Medium Present")]
    [LocDescription("A medium-sized gift that stands out around the tree.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(350)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class PresentMediumItem : WorldObjectItem<PresentMediumObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<PresentMediumObject>(1.5f);
    }

    [Serialized]
    [LocDisplayName("Large Present")]
    [LocDescription("A massive box ready to be opened at the foot of the tree.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(500)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class PresentLargeItem : WorldObjectItem<PresentLargeObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<PresentLargeObject>(2.2f);
    }

    [Serialized]
    [LocDisplayName("Huge Present")]
    [LocDescription("A very large, colorful present to celebrate the holidays.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(650)]
    [Tag(nameof(SurfaceTags.CanBeOnSurface))]
    public partial class PresentHugeItem : WorldObjectItem<PresentHugeObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<PresentHugeObject>(3f);
    }

    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Petit cadeau")]
    public partial class PresentSmallRecipe : RecipeFamily
    {
        public PresentSmallRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "PresentSmall",
                displayName: Localizer.DoStr("Small Present"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(PaperItem), 1, typeof(Skill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<PresentSmallItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 1;

            this.LaborInCalories = CreateLaborInCaloriesValue(40);

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(PresentSmallRecipe), start: 1, skillType: typeof(Skill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Little Present"), recipeType: typeof(PresentSmallRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(WorkbenchObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Cadeau moyen")]
    public partial class PresentMediumRecipe : RecipeFamily
    {
        public PresentMediumRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "PresentMedium",
                displayName: Localizer.DoStr("Medium Present"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(PaperItem), 3, typeof(Skill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<PresentMediumItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 2;

            this.LaborInCalories = CreateLaborInCaloriesValue(60);

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(PresentMediumRecipe), start: 2, skillType: typeof(Skill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Medium Present"), recipeType: typeof(PresentMediumRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(WorkbenchObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Cadeau gros")]
    public partial class PresentLargeRecipe : RecipeFamily
    {
        public PresentLargeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "PresentLarge",
                displayName: Localizer.DoStr("Big Present"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(PaperItem), 6, typeof(Skill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<PresentLargeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 3;

            this.LaborInCalories = CreateLaborInCaloriesValue(80);

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(PresentLargeRecipe), start: 3, skillType: typeof(Skill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Big Present"), recipeType: typeof(PresentLargeRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(WorkbenchObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Cadeau énorme")]
    public partial class PresentHugeRecipe : RecipeFamily
    {
        public PresentHugeRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "PresentHuge",
                displayName: Localizer.DoStr("Enormous Present"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement(typeof(PaperItem), 12, typeof(Skill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<PresentHugeItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 3;

            this.LaborInCalories = CreateLaborInCaloriesValue(100);

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(PresentHugeRecipe), start: 4, skillType: typeof(Skill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Enormous Present"), recipeType: typeof(PresentHugeRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(WorkbenchObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    // ______________________________________________________ Bonhomme de neige ______________________________________________________ \

    [Serialized]
    [RequireComponent(typeof(PropertyAuthComponent))]
    [RequireComponent(typeof(HousingComponent))]
    [RequireComponent(typeof(OccupancyRequirementComponent))]
    [RequireComponent(typeof(ForSaleComponent))]
    [RequireComponent(typeof(RoomRequirementsComponent))]
    [Tag("Usable")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Bonhomme de neige")]
    public partial class SnowmanObject : WorldObject, IRepresentsItem
    {
        public virtual Type RepresentedItemType => typeof(SnowmanItem);
        public override LocString DisplayName => Localizer.DoStr("Snowman");
        public override TableTextureMode TableTexture => TableTextureMode.Canvas;

        protected override void Initialize()
        {
            this.ModsPreInitialize();
            this.GetComponent<HousingComponent>().HomeValue = SnowmanItem.homeValue;
            this.ModsPostInitialize();
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }

    [Serialized]
    [LocDisplayName("Snowman")]
    [LocDescription("A frosty companion that lasts far longer than a real pile of snow.")]
    [Ecopedia("Crafted Objects", "Décorations de Noël", createAsSubPage: true)]
    [Tag("Housing")]
    [Tag("WinterHoliday")]
    [Weight(1400)]
    public partial class SnowmanItem : WorldObjectItem<SnowmanObject>
    {
        protected override OccupancyContext GetOccupancyContext => new SideAttachedContext(0 | DirectionAxisFlags.Down, WorldObject.GetOccupancyInfo(this.WorldObjectType));
        public override HomeFurnishingValue HomeValue => homeValue;
        public static readonly HomeFurnishingValue homeValue = ChristmasHousingValues.Create<SnowmanObject>(4.5f);
    }

    [Ecopedia("Crafted Objects", "Décorations de Noël", subPageName: "Bonhomme de neige")]
    public partial class SnowmanRecipe : RecipeFamily
    {
        public SnowmanRecipe()
        {
            var recipe = new Recipe();
            recipe.Init(
                name: "Snowman",
                displayName: Localizer.DoStr("Snowman"),

                ingredients: new List<IngredientElement>
                {
                    new IngredientElement("Fabric", 12, typeof(Skill)),
                    new IngredientElement("Crop Seed", 6, typeof(Skill)),
                    new IngredientElement("Vegetable", 4, typeof(Skill)),
                },

                items: new List<CraftingElement>
                {
                    new CraftingElement<SnowmanItem>()
                });
            this.Recipes = new List<Recipe> { recipe };
            this.ExperienceOnCraft = 4;

            this.LaborInCalories = CreateLaborInCaloriesValue(150);

            this.CraftMinutes = CreateCraftTimeValue(beneficiary: typeof(SnowmanRecipe), start: 4, skillType: typeof(Skill));

            this.ModsPreInitialize();
            this.Initialize(displayText: Localizer.DoStr("Snowman"), recipeType: typeof(SnowmanRecipe));
            this.ModsPostInitialize();

            CraftingComponent.AddRecipe(tableType: typeof(WorkbenchObject), recipeFamily: this);
        }

        partial void ModsPreInitialize();
        partial void ModsPostInitialize();
    }


}