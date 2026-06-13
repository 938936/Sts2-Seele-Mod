using BaseLib.Abstracts;
using BaseLib.Utils;
using Selee.SeleeCode.Character;

namespace Selee.SeleeCode.Potions;

[Pool(typeof(SeleePotionPool))]
public abstract class SeleePotion : CustomPotionModel;