﻿using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Utils;

namespace JRPG_Game;

public static class Program
{
    public static void Main()
    {
        Console.Clear();

        // game start
        Console.WriteLine("Bienvenue dans le jeu !\n");

        var nbrTeam = Prompt.Get<int>("Entrez le nombre d'équipe :", i => i < 2);
        var nbrCharacters = Prompt.Get<int>("Entrez le nombre de joueurs par équipe :", i => i < 1);

        Next();

        for (var i = 0; i < nbrTeam; i++)
        {
            Console.WriteLine($"Creation de l'équipe {i + 1}");
            Console.WriteLine($"L'équipe {Team.Team.Create(nbrCharacters).Name} a été créée");
            Next(2000);
        }

        Console.WriteLine($"{Team.Team.List.Count} équipes créées.");
        Console.WriteLine("Bon jeu !");

        Next(2000);

        // game content
        var turn = 1;
        while (Team.Team.IsCombatOn())
        {
            Skill.UpdateReloadCooldowns();
            Console.WriteLine($"{new string('=', 10)} Turn {turn++} {new string('=', 10)}");

            List<Skill> turnActions = [];
            Team.Team.List.ForEach(team =>
            {
                Console.WriteLine($"Au tour de l'équipe {team.Name}");
                team.Characters
                    .Where(character => character.IsAlive(true)).ToList()
                    .ForEach(character =>
                    {
                        bool next;
                        do
                        {
                            Console.WriteLine($"Au tour de {character.Name} - {character.Team.Name}\n");

                            var status = character.SelectAction();

                            next = status.Next;
                            if (status.Skill != null) turnActions.Add(status.Skill);
                        } while (!next);

                        Prompt.Input("Appuyez sur 'Entrée' pour finir le tour du personnage",
                            key => key != ConsoleKey.Enter);
                        Next(0);
                    });
                Console.WriteLine($"L'équipe {team.Name} à fini de choisi ses action pour tout ses personnages.");
                Next(2500);
            });

            Console.WriteLine($"Execution du tour {turn}...");

            turnActions = turnActions.OrderByDescending(action => action.Owner.Speed).ToList();
            turnActions.ForEach(action =>
            {
                action.Execute();
                Console.WriteLine();
            });

            Prompt.Input("Appuyez sur 'Entrée' pour finir le tour", key => key != ConsoleKey.Enter);
            Next(0);
        }

        // game end
        var winners = Team.Team.List.Where(team => team.Characters.Any(character => character.IsAlive(false))).ToList();

        switch (winners.Count)
        {
            case 1:
                var winner = winners.First();
                Console.WriteLine($"{winner.Name} a gagné, Félicitations !");
                Console.WriteLine(
                    $"Membres : {(winner.Characters.Count != 0
                        ? string.Join(", ", winner.Characters.Select(character => $"{character.Name}{(!character.IsAlive(false) ? " (mort au combat)" : string.Empty)}"))
                        : "Aucun personnage")}");
                break;
            case > 1:
                Console.WriteLine("Une erreur est survenue : il n'y a pas qu'un seul gagnant.");
                break;
            default:
                Console.WriteLine("Une erreur est survenue : aucun gagnant.");
                break;
        }
    }

    private static Character CreateCharacter()
    {
        while (true)
        {
            List<Type> classList = [typeof(Mage), typeof(Paladin), typeof(Thief), typeof(Warrior)];
            var characterType =
                classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList) - 1);

            if (Activator.CreateInstance(characterType, Prompt.Get<string>("Entrez votre nom :"))
                is Character character)
                return character;

            Console.WriteLine("Une erreur s'est produite : " + characterType);
        }
    }

    public static void Next(int millisecondsTimeout = 500)
    {
        Thread.Sleep(millisecondsTimeout);
        Console.Clear();
    }
}