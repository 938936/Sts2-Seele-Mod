using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class PianLuoLiuYing() : SeleeCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.GongMing];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("GongMingDamage", 6m),
        new DynamicVar("HitCount", 2m),
        new DynamicVar("GongMingHitCount", 3m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<JianBingDongNengPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var dongNengPower = base.Owner.Creature.GetPower<JianBingDongNengPower>();
        bool isGongMing = dongNengPower != null;

        decimal damage = isGongMing ? DynamicVars["GongMingDamage"].BaseValue : base.DynamicVars.Damage.BaseValue;
        int hitCount = isGongMing ? DynamicVars["GongMingHitCount"].IntValue : DynamicVars["HitCount"].IntValue;

        await DamageCmd.Attack(damage)
            .WithHitCount(hitCount)
            .FromCard(this,cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["GongMingDamage"].UpgradeValueBy(2m);
    }
}
