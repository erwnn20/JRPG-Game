﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Mage : Character, IMana
{
    public int MaxMana => 100;
    public int CurrentMana { get; set; }

    public Mage(string name, Team.Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 60,
            speed: 75,
            armor: ArmorType.Textile,
            physicalAttack: 0,
            magicalAttack: 75,
            dodgeChance: 0.05m,
            paradeChance: 0.05m,
            spellResistanceChance: 0.25m,
            skills: [])
    {
        CurrentMana = MaxMana;
        Skills.AddRange([
            new Attack<Character>(
                name: "Eclair de givre",
                description: () => $"Inflige 100% de la puissance d’attaque magique ({MagicalAttack}) à la cible.\n" +
                                   $"Réduit la vitesse de la cible de 25% si celui ci n'a pas résisté à l'attaque.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 15,
                damage: MagicalAttack,
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack.StatusInfo.Resisted) return;
                        if (attack.Target is not Character target) return;

                        target.Speed = (int)(target.Speed * 0.75m);
                        Console.WriteLine($"La vitesse de {target.Name} à diminué de 25%.");
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Barrière de givre",
                description: () => $"Réduit les dégâts des deux prochaines attaques subies.\n" +
                                   $"\t- {ReduceDamagePhysical:P} sur les attaques physiques.\n" +
                                   $"\t- {ReduceDamageMagical:P} sur les attaques magiques.",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 2,
                manaCost: 25,
                effect: _ =>
                {
                    const int attackReduced = 2;
                    ReducedAttack += attackReduced;
                    return $"Les {attackReduced} prochaines attaques subies par {Name} sont réduites.";
                }),
            new Attack<Team.Team>(
                name: "Blizzard",
                description: () =>
                    $"Inflige 50% de la puissance d’attaque magique ({(int)(MagicalAttack * 0.50m)}) à toute l’équipe ciblé.\n" +
                    $"A une chance de baisser la vitesse de chaque cible de 15%.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 25,
                damage: (int)(MagicalAttack * 0.5m),
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack.StatusInfo.Resisted) return;
                        if (attack.Target is not Character target) return;

                        target.Speed = (int)(target.Speed * 0.85m);
                        Console.WriteLine($"La vitesse de {target.Name} à diminué de 15%.");
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Brulure de mana",
                description: () => "Réduit de moitié la quantité de points de mana de la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 3,
                manaCost: 20,
                effect: target =>
                {
                    if (target is not IMana t)
                        return $"{target.Name} n'utilise pas de mana.";

                    var manaTaken = Math.Max(40, t.CurrentMana / 2);
                    return $"{target.Name} perd {t.LoseMana(manaTaken)} de mana.";
                }),
            new SpecialAbility<Character>(
                name: "Renvoi de sort",
                description: () => "Renvoie la prochaine attaque magique subie à l’assaillant.",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 25,
                effect: _ =>
                {
                    SpellReturn = true;
                    return $"La prochaine attaque magique subie par {Name} sera renvoyée.";
                }),
            ((IMana)this).Drink(this)
        ]);
    }

    private static decimal ReduceDamagePhysical => 0.60m;
    private static decimal ReduceDamageMagical => 0.50m;
    private int ReducedAttack { get; set; }
    private bool SpellReturn { get; set; }

    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, SpellReturn, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.AttackType == DamageType.Magical && SpellReturn)
        {
            (from.Additional ??= []).Add(Special);
            SpellReturn = false;
        }

        if (ReducedAttack > 0)
        {
            from.StatusInfo.Damage *= 1 - from.AttackType switch
            {
                DamageType.Physical => ReduceDamagePhysical,
                DamageType.Magical => ReduceDamageMagical,
                _ => 0
            };
            ReducedAttack--;
        }

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    private Action<Attack<Character>> Special => attackFrom =>
    {
        Console.WriteLine($"{Name} revoie {attackFrom.Name}.");
        var conterAttack = new Attack<Character>(
            name: attackFrom.Name,
            description: attackFrom.Description,
            owner: this,
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: attackFrom.Damage,
            attackType: attackFrom.AttackType
        );
        conterAttack.Execute();
        conterAttack.Damage = _ => 0;
    };

    //

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {CurrentMana}/{MaxMana}" +
               (ReducedAttack > 0
                   ? $"\n Dégâts réduits pendant {(ReducedAttack > 1 ? $"les {ReducedAttack} prochaines attaques subies." : "la prochaine attaque subie.")}"
                   : string.Empty) +
               (SpellReturn ? "\n La prochaine attaque magique subie sera renvoyée." : string.Empty);
    }
}