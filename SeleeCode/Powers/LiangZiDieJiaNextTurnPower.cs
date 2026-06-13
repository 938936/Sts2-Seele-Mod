using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Powers;

public class LiangZiDieJiaNextTurnPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner) && base.AmountOnTurnStart != 0)
        {
            Flash();
            await PowerCmd.Apply<LiangZiDieJiaPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
            await PowerCmd.Remove(this);
        }
    }
}
