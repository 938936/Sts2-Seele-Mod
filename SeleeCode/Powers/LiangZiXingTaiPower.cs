using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class LiangZiXingTaiPower() : SeleePower, ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Cards", 2m),
    ];

    private bool _dieJiaTriggeredThisTurn = false;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side==CombatSide.Player && participants.Contains(base.Owner))
        {
            _dieJiaTriggeredThisTurn = false;
            Flash();
            await PowerCmd.Apply<LiangZiDieJiaPower>(new ThrowingPlayerChoiceContext(), base.Owner, Amount, base.Owner, null);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power == this)
        {
            DynamicVars["Cards"].BaseValue=Amount;
        }
    }

    public async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard)
    {
        if (!_dieJiaTriggeredThisTurn && owner.Creature == this.Owner) 
        {
            _dieJiaTriggeredThisTurn = true;
            Flash();
            await CardPileCmd.Draw(new HookPlayerChoiceContext(owner, LocalContext.NetId ?? 0, GameActionType.Combat), DynamicVars["Cards"].IntValue, owner);
        }
    }
}
