using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Powers;

public class TianChengBengLuoZhiKePower() : SeleePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (wasRemovalPrevented || creature != base.Owner)
        {
            return;
        }

        Flash();

        int damage = base.Owner.MaxHp * base.Amount;
        var otherEnemies = base.CombatState!.HittableEnemies.Where(e => e != base.Owner && !e.IsDead).ToList();
        foreach (var enemy in otherEnemies)
        {
            await CreatureCmd.Damage(choiceContext, enemy, damage, ValueProp.Unpowered, Applier, null, null);
        }

        await PowerCmd.Remove(this);
    }
}
