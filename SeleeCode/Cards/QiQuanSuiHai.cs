using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class QiQuanSuiHai() : SeleeCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<QiQuanSuiHaiPower>(7),
        new DynamicVar("DamageAdd", 2m),
        new DynamicVar("LastDamageAdd", 10m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<QiQuanSuiHaiPower>(choiceContext, base.Owner.Creature, DynamicVars["QiQuanSuiHaiPower"].BaseValue, base.Owner.Creature, this);
        if (power != null)
        {
            power.DynamicVars["DamageAdd"].BaseValue = DynamicVars["DamageAdd"].BaseValue;
            power.DynamicVars["LastDamageAdd"].BaseValue = DynamicVars["LastDamageAdd"].BaseValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LastDamageAdd"].UpgradeValueBy(8m);
    }
}
