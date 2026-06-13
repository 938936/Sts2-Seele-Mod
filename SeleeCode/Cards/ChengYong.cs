using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class ChengYong() : SeleeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<XingHuanNengJiPower>(),
        HoverTipFactory.FromPower<JianBingDongNengPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HpLoss", 3m),
        new PowerVar<XingHuanNengJiPower>(20m),
        new PowerVar<JianBingDongNengPower>(1m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await PowerCmd.Apply<XingHuanNengJiPower>(choiceContext, base.Owner.Creature, DynamicVars["XingHuanNengJiPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<JianBingDongNengPower>(choiceContext, base.Owner.Creature, DynamicVars["JianBingDongNengPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XingHuanNengJiPower"].UpgradeValueBy(10m);
    }
}
