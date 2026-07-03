using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Powers;

public class QiQuanSuiHaiPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("DamageAdd", 3m),
        new DynamicVar("LastDamageAdd", 10m),
    ];

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != base.Owner) return 0m;
        if (!props.IsPoweredAttack() || cardSource == null) return 0m;
        if (base.Amount <= 0) return 0m;

        if (base.Amount == 1)
        {
            return DynamicVars["DamageAdd"].BaseValue + DynamicVars["LastDamageAdd"].BaseValue;
        }
        return DynamicVars["DamageAdd"].BaseValue;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner.Creature == Owner)
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }
}
