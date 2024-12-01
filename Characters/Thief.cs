﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Thief : Character
{
    public Thief(string name, Team.Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 80,
            speed: 100,
            armor: ArmorType.Leather,
            physicalAttack: 55,
            magicalAttack: 0,
            dodgeChance: 0.15m,
            paradeChance: 0.25m,
            spellResistanceChance: 0.25m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Coup bas",
                description: $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.\n" +
                             $"Inflige 150% si la cible a moins de la moitié de ses points de vie.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: target =>
                    (int)(PhysicalAttack *
                          (target.CurrentHealth < target.MaxHealth / 2 ? 1.50m : 1.00m)),
                attackType: DamageType.Physical),
            new SpecialAbility<Character>(
                name: "Evasion",
                description:
                $"Augmente les chances d'esquive de 20% ({DodgeChance:P} {(DodgeChance == 0.5m ? "MAX" : $"+ {0.20m:P}")})\n" +
                $"Augmente les chances de resister aux sorts de 20% ({SpellResistanceChance:P} {(SpellResistanceChance == 0.5m ? "MAX" : $"+ {0.20m:P}")})",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 0,
                effect: _ =>
                {
                    var output = "";
                    var oldDodgeChance = DodgeChance;
                    DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
                    output += oldDodgeChance != DodgeChance
                        ? $"{Name} augmente ses chances d'esquive de {DodgeChance - oldDodgeChance:P} ({oldDodgeChance:P} -> {DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)})"
                        : $"{Name} a ses chances d'esquive au max : {DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)}";

                    var oldSpellResistanceChance = SpellResistanceChance;
                    SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
                    output += oldSpellResistanceChance != SpellResistanceChance
                        ? $"{Name} augmente ses chances  de resister aux sorts de {SpellResistanceChance - oldSpellResistanceChance:P} ({oldSpellResistanceChance:P} -> {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})"
                        : $"{Name} a ses chances de resister aux sorts au maximum : {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)}";

                    return output;
                })
        ]);
    }

    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.StatusInfo.Dodged)
        {
            var counterAttack = new Attack<Character>(
                name: "Poignard dans le dos",
                owner: this,
                description:
                "Lorsque le voleur esquive une attaque ennemie, il déclenche une attaque qui inflige 15 points de dégâts physiques à l’attaquant.",
                target: from.Owner,
                targetType: TargetType.Enemy,
                reloadTime: 0,
                manaCost: 0,
                damage: 15,
                attackType: DamageType.Physical
            );

            if (from.Additional != null)
                from.Additional.Add(_ => counterAttack.Execute());
            else from.Additional = [_ => counterAttack.Execute()];
        }

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /*protected override void SpecialAbility()
    {
        DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
        SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Evasion\"\n" +
                          $" -> Augmente les chances d'esquive ({DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)})\n" +
                          $" -> Augmente sa resistance aux sorts ({SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})");
    }

    protected override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Coup bas",
            attacker: this,
            target: character,
            damage: (int)(PhysicalAttack * (character.CurrentHealth < character.MaxHealth / 2 ? 1.5 : 1)),
            attackType: DamageType.Physical
        );
        attack.Execute();
    }*/
    protected override void SpecialAbility()
    {
        throw new NotImplementedException();
    }

    protected override void Attack(Character character)
    {
        throw new NotImplementedException();
    }
}