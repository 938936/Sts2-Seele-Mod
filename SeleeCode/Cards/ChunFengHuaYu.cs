using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class ChunFengHuaYu() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa, SeleeCardKeyword.DieJia];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(6m),
        new CalculationExtraVar(4m),
        new CalculatedBlockVar( ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => card.Owner.Creature.HasPower<LiangZiDieJiaPower>()?1m:0m),
        new DynamicVar("XingHuanNengJi", 15m),
        new DynamicVar("DieJiaXingHuanNengJi", 20m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal nengJi = DynamicVars["XingHuanNengJi"].BaseValue;

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower != null)
        {
            nengJi += DynamicVars["DieJiaXingHuanNengJi"].BaseValue;
        }

        await CreatureCmd.GainBlock(base.Owner.Creature, DynamicVars.CalculatedBlock.Calculate(cardPlay.Target), ValueProp.Move, cardPlay);
        await PowerCmd.Apply<XingHuanNengJiPower>(choiceContext, base.Owner.Creature, nengJi, base.Owner.Creature, this);

        if (dieJiaPower != null)
        {
            await SeleeHook.AfterDieJiaTrigger(Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.CalculationExtra.UpgradeValueBy(2m);
        DynamicVars["DieJiaXingHuanNengJi"].UpgradeValueBy(10m);
    }
}
