using System.Linq;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class RuXiYuJingRunBuChuo() : SeleeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<YuHePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move),
        new PowerVar<YuHePower>(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var nonAttackCards = PileType.Hand.GetPile(base.Owner).Cards
            .Where(c => c.Type != CardType.Attack)
            .ToList();

        foreach (var card in nonAttackCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        int exhaustCount = nonAttackCards.Count;

        if (exhaustCount > 0)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .WithHitCount(exhaustCount)
                .FromCard(this)
                .TargetingRandomOpponents(base.CombatState!)
                .Execute(choiceContext);

            await PowerCmd.Apply<YuHePower>(choiceContext, base.Owner.Creature, DynamicVars["YuHePower"].IntValue * exhaustCount, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["YuHePower"].UpgradeValueBy(1m);
    }
}
