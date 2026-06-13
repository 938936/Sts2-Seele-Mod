using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class LiangZiBuQueDingXingPower() : SeleePower,ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int ModifyDieJiaBlockAdd(Creature target, ValueProp props, Creature owner, int oriValue, CardModel? blockCard)
    {
        if (blockCard != null)
        {
            if (blockCard.Owner.Creature != base.Owner) return oriValue;
        }
        else if (base.Owner != target)
        {
            return oriValue;
        }
        if (!props.IsPoweredCardOrMonsterMoveBlock()) return oriValue;
        return oriValue+Amount;
    }
}
