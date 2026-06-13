using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class TianZhuiDeXingHui() : SeleeCard(1, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    private decimal _extraDamageFromPlays;
    private decimal _extraHitCountFromPlays;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar("HitCount", 1m),
        new DynamicVar("Increase", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(base.DynamicVars["HitCount"].IntValue)
            .FromCard(this)
#pragma warning disable CS8604
            .TargetingRandomOpponents(base.CombatState)
#pragma warning restore CS8604
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        base.DynamicVars.Damage.BaseValue += base.DynamicVars["Increase"].BaseValue;
        _extraDamageFromPlays += base.DynamicVars["Increase"].BaseValue;
        base.DynamicVars["HitCount"].BaseValue += base.DynamicVars["Increase"].BaseValue;
        _extraHitCountFromPlays += base.DynamicVars["Increase"].BaseValue;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue += _extraDamageFromPlays;
        base.DynamicVars["HitCount"].BaseValue += _extraHitCountFromPlays;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HitCount"].UpgradeValueBy(1m);
    }
}
