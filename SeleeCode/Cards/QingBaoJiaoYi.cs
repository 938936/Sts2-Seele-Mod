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

public class QingBaoJiaoYi() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia, SeleeCardKeyword.GongMing,CardKeyword.Retain];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JianBingDongNengPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(9m, ValueProp.Move),
        new DynamicVar("DieJiaBlock", 8m),
        new DynamicVar("GongMingJianBingDongNengPower", 1m),
    ];

    private bool HasGongMing =>
        base.IsMutable && base.Owner != null && base.Owner.Creature.HasPower<JianBingDongNengPower>();

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && (base.Owner.Creature.HasPower<LiangZiDieJiaPower>() || base.Owner.Creature.HasPower<JianBingDongNengPower>());

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        decimal block = base.DynamicVars.Block.BaseValue;

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower != null)
        {
            block += DynamicVars["DieJiaBlock"].BaseValue;
        }

        await CreatureCmd.GainBlock(cardPlay.Target, block, ValueProp.Move, cardPlay);

        if (HasGongMing)
        {
            await PowerCmd.Apply<JianBingDongNengPower>(choiceContext, cardPlay.Target, DynamicVars["GongMingJianBingDongNengPower"].BaseValue, base.Owner.Creature, this);
            await SeleeHook.AfterGongMingTrigger(Owner, this);
        }

        if (dieJiaPower != null)
        {
            await SeleeHook.AfterDieJiaTrigger(Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["DieJiaBlock"].UpgradeValueBy(3m);
    }
}
