using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class MingGuangAnYing() : SeleeCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<VigorPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(9m, ValueProp.Move),
        new PowerVar<VigorPower>(3),
        new DynamicVar("DieJiaVigorPower", 6m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
#pragma warning disable CS8604
            .Targeting(cardPlay.Target)
#pragma warning restore CS8604
            .Execute(choiceContext);

        decimal vigorAmount = DynamicVars["VigorPower"].BaseValue;

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower != null)
        {
            vigorAmount += DynamicVars["DieJiaVigorPower"].BaseValue;
        }

        await PowerCmd.Apply<VigorPower>(choiceContext, base.Owner.Creature, vigorAmount, base.Owner.Creature, this);

        if (dieJiaPower != null)
        {
            await SeleeHook.AfterDieJiaTrigger(Owner, this, choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["VigorPower"].UpgradeValueBy(2m);
    }
}
