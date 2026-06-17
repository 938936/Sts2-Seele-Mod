using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class YongHengDengYuYiShun() : SeleeCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LiangZiDieJiaPower>(),
        HoverTipFactory.FromPower<LiangZiTanSuoPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10m, ValueProp.Move),
        new PowerVar<LiangZiDieJiaPower>(2),
        new PowerVar<LiangZiTanSuoPower>(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<LiangZiDieJiaPower>(choiceContext, base.Owner.Creature, DynamicVars["LiangZiDieJiaPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<LiangZiTanSuoPower>(choiceContext, cardPlay.Target, DynamicVars["LiangZiTanSuoPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["LiangZiDieJiaPower"].UpgradeValueBy(1m);
        DynamicVars["LiangZiTanSuoPower"].UpgradeValueBy(1m);
    }
}
