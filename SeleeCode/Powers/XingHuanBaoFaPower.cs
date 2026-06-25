using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Selee.SeleeCode.Patch;

namespace Selee.SeleeCode.Powers;

public class XingHuanBaoFaPower() : SeleePower,ISeleeHook
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public static int BaseDurationTurn = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("InitialDongNeng", 6m),
        new DynamicVar("StackDongNeng", 4m),
        new CardsVar(1)
    ];

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0m)
        {
            bool alreadyActive = base.Amount - amount > 0;
            int dongNengAmount = alreadyActive ? DynamicVars["StackDongNeng"].IntValue : DynamicVars["InitialDongNeng"].IntValue;
            await PowerCmd.Apply<JianBingDongNengPower>(new ThrowingPlayerChoiceContext(), base.Owner, dongNengAmount, base.Owner, null);
        }
    }

    public async Task AfterGongMingTrigger(Player owner, CardModel triggerCard,PlayerChoiceContext? choiceContext)
    {
        if (owner.Creature == Owner)
        {
            await CardPileCmd.Draw(
                choiceContext ?? new HookPlayerChoiceContext(owner, LocalContext.NetId ?? 0, GameActionType.Combat)
                , DynamicVars.Cards.BaseValue, owner);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && participants.Contains(base.Owner))
        {
            Flash();
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterRemoved(Creature oldOwner)
    {
        if (oldOwner.HasPower<JianBingDongNengPower>())
        {
            await PowerCmd.Remove<JianBingDongNengPower>(base.Owner);
        }
    }
}
