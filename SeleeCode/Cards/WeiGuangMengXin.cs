using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Selee.SeleeCode.Cards;

public class WeiGuangMengXin() : SeleeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<YuHePower>(),
        HoverTipFactory.FromPower<WeakPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("YuHeMultiplier", 3m),
        new DynamicVar("WeakBase", 0m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        int yuHeAmount = (int)(DynamicVars["YuHeMultiplier"].BaseValue * xValue);
        int weakAmount = xValue + DynamicVars["WeakBase"].IntValue;

        await PowerCmd.Apply<YuHePower>(choiceContext, base.Owner.Creature, yuHeAmount, base.Owner.Creature, this);

        foreach (var enemy in base.CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, weakAmount, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["YuHeMultiplier"].UpgradeValueBy(1m);
        DynamicVars["WeakBase"].UpgradeValueBy(1m);
    }
}
