using System.Linq;
using Selee.SeleeCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class RuZhaoLuXiaoSanYuChenXi() : SeleeCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("HpLoss", 6m),
        new DamageVar(21m, ValueProp.Move),
    ];

    private bool _killedEnemy;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _killedEnemy = false;

        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);

        var targets = base.CombatState!.HittableEnemies.ToList();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState!)
            .Execute(choiceContext);

        _killedEnemy = targets.Any(e => e.IsDead);

        if (_killedEnemy)
        {
            await CreatureCmd.Heal(base.Owner.Creature, DynamicVars["HpLoss"].BaseValue);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_killedEnemy && cardPlay.Card == this)
        {
            _killedEnemy = false;
            await CardCmd.AutoPlay(choiceContext, this, cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7);
    }
}
