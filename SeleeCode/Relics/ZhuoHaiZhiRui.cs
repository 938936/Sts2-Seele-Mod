using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using Selee.SeleeCode.Character;
using Selee.SeleeCode.Powers;

namespace Selee.SeleeCode.Relics;

[Pool(typeof(SeleeRelicPool))]
public class ZhuoHaiZhiRui() : SeleeRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private static readonly int[] TurnAmounts = [4, 2, 1];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LiangZiDieJiaPower>()];

    [SavedProperty]
    public int TurnCounter => Owner.PlayerCombatState?.TurnNumber ?? 0;

    public override bool ShowCounter => true;

    public override int DisplayAmount => Math.Max(3 - TurnCounter, 0);

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (TurnCounter < TurnAmounts.Length)
        {
            Flash();
            await PowerCmd.Apply<LiangZiDieJiaPower>(
                new ThrowingPlayerChoiceContext(),
                base.Owner.Creature,
                TurnAmounts[TurnCounter - 1],
                base.Owner.Creature, null);
        }
    }
}
