using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Patch;

/*
 在战斗中的对象（能力、卡牌、遗物等）可以实现ISeleeHook接口以监听Selee mod专属的事件。
*/
public class SeleeHook
{
    public static int ModifyDieJiaBlockAdd(Creature target, ValueProp props,Creature owner, int oriValue, CardModel? blockCard)
    {
        if (owner.CombatState==null)
        {
            return oriValue;
        }
        int value=oriValue;
        foreach (AbstractModel item in owner.CombatState.IterateHookListeners())
        {
            if (item is ISeleeHook hookItem)
            {
                value = hookItem.ModifyDieJiaBlockAdd(target, props, owner, value, blockCard);
            }
        }
        return value;
    }
    
    public static async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard,PlayerChoiceContext? choiceContext = null)
    {
        foreach (AbstractModel item in owner.Creature?.CombatState?.IterateHookListeners() ?? [])
        {
            if (item is ISeleeHook hookItem)
            {
                await hookItem.AfterDieJiaTrigger(owner, triggerCard, choiceContext);
            }
        }
        var power = owner.Creature?.GetPower<LiangZiDieJiaPower>();
        if (power != null)
        {
            await PowerCmd.ModifyAmount(
                choiceContext ?? new HookPlayerChoiceContext(owner, LocalContext.NetId ?? 0, GameActionType.Combat),
                power, -1, owner.Creature, triggerCard);
        }
    }

    public static async Task AfterGongMingTrigger(Player owner, CardModel triggerCard,PlayerChoiceContext? choiceContext = null)
    {
        foreach (AbstractModel item in owner.Creature?.CombatState?.IterateHookListeners() ?? [])
        {
            if (item is ISeleeHook hookItem)
            {
                await hookItem.AfterGongMingTrigger(owner, triggerCard,
                    choiceContext ?? new HookPlayerChoiceContext(owner, LocalContext.NetId ?? 0, GameActionType.Combat));
            }
        }
    }
}

public interface ISeleeHook
{
    public int ModifyDieJiaBlockAdd(Creature target, ValueProp props,Creature owner, int oriValue, CardModel? blockCard)
    {
        return oriValue;
    }

    public Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard,PlayerChoiceContext? choiceContext = null)
    {
        return Task.CompletedTask;
    }

    public Task AfterGongMingTrigger(Player owner, CardModel triggerCard, PlayerChoiceContext? choiceContext = null)
    {
        return Task.CompletedTask;
    }
}