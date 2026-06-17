using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Powers;

public class ShengKaiBuJuDiaoLingPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("DrawCount", 0m),
    ];

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return count+DynamicVars["DrawCount"].IntValue;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == Owner.Side && participants.Contains(base.Owner))
        {
            Flash();
            await PowerCmd.Apply<YuHePower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
        }
    }
}
