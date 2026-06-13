using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Selee.SeleeCode.Character;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Relics;

[Pool(typeof(SeleeRelicPool))]
public class TongYao() : SeleeRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Amount", 3m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LiangZiDieJiaPower>()];

    public override async Task BeforeCombatStart()
    {
        Flash();
        await PowerCmd.Apply<LiangZiDieJiaPower>(
            new ThrowingPlayerChoiceContext(),
            base.Owner.Creature,
            base.DynamicVars["Amount"].IntValue,
            base.Owner.Creature, null);
    }

    public override RelicModel? GetUpgradeReplacement()
    {
        return ModelDb.Relic<ZhuoHaiZhiRui>();
    }
}