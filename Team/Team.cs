﻿using JRPG_Game.Characters;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Team;

public class Team : ITarget
{
    private Team(string name)
    {
        Name = name;
        List.Add(this);
    }

    public List<Character> Characters { get; } = [];
    public string Name { get; }

    public static readonly List<Team> List = [];

    //

    private void Add(Character character) => Characters.Add(character);

    //

    public static Team Create(int size)
    {
        Team newTeam = new(Prompt.Get<string>("Entrez le nom de votre équipe :"));

        for (var i = 0; i < size; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"Creation du {i + 1}{(i + 1 == 1 ? "er" : "ème")} personnage...");
            newTeam.Add(Character.Create(newTeam));
        }

        return newTeam;
    }

    public static bool IsCombatOn() =>
        List.Count(team => team.Characters.Any(character => character.IsAlive(false))) >= 2;

    //

    public override string ToString()
    {
        var charactersInfo = Characters.Count != 0
            ? string.Join(", ", Characters.Select(character => character.Name))
            : "Aucun personnage";

        return $"Équipe : {Name}\n" +
               $"Membres : {charactersInfo}";
    }
}