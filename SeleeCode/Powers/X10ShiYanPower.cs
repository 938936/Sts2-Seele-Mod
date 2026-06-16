using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Powers;

public class X10ShiYanPower() : SeleePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _drawReduction = 0;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player == base.Owner.Player)
        {
            _drawReduction = base.Amount;
            var dieJiaPower = base.Owner.GetPower<LiangZiDieJiaPower>();
            if (dieJiaPower != null && dieJiaPower.Amount >= _drawReduction)
            {
                await PowerCmd.ModifyAmount(choiceContext, dieJiaPower, -_drawReduction, base.Owner, null);
                _drawReduction = 0;
            }
        }
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner.Player || _drawReduction <= 0) return count;
        return Math.Max(0m, count - _drawReduction);
    }

    public override Task AfterModifyingHandDraw()
    {
        Flash();
        return Task.CompletedTask;
    }
}
