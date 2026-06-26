using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class HuaXuWeiShi() : SeleeCard(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LiangZiTanSuoPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CalculationBaseVar(12m),
        new ExtraDamageVar(20m),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card, Creature? _) => card.Owner.Creature.HasPower<LiangZiDieJiaPower>()?1m:0m),
        new DynamicVar("DieJiaLiangZiTanSuoPower", 2m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        if (dieJiaPower != null)
        {
            await PowerCmd.Apply<LiangZiTanSuoPower>(choiceContext, cardPlay.Target, DynamicVars["DieJiaLiangZiTanSuoPower"].BaseValue, base.Owner.Creature, this);
            await SeleeHook.AfterDieJiaTrigger(Owner, this,choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
