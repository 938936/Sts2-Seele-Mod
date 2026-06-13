using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Selee.SeleeCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Selee.SeleeCode.Cards;
using Selee.SeleeCode.Relics;

namespace Selee.SeleeCode.Character;

public class Selee : PlaceholderCharacterModel
{
    public override string PlaceholderID => "necrobinder";
    public const string CharacterId = "Selee";

    public static readonly Color Color = new("B2F0FB");

    public override Color NameColor => Color;
    public override Color MapDrawingColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<LiangZiHua>(),
        ModelDb.Card<TanSuoGongJi>(),
        ModelDb.Card<SeleeDaJi>(),
        ModelDb.Card<SeleeDaJi>(),
        ModelDb.Card<SeleeDaJi>(),
        ModelDb.Card<SeleeDaJi>(),
        ModelDb.Card<SeleeFangYu>(),
        ModelDb.Card<SeleeFangYu>(),
        ModelDb.Card<SeleeFangYu>(),
        ModelDb.Card<SeleeFangYu>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<TongYao>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<SeleeCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SeleeRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SeleePotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "希儿头像128.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "角色选择头像.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "希儿头像128.png".CharacterUiPath();

    public override string CustomCharacterSelectBg => "scenes/char_select_bg_selee.tscn".ImagePath();
    public override string CustomVisualPath =>"scenes/selee_creature.tscn".ImagePath();
}