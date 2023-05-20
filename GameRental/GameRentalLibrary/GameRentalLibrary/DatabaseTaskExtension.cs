using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using GameRental.Rep0;
using GameRental.Rep8;
using GameRental.Adapters;
using GameRental.Extensions;

namespace GameRental.Extensions
{
    public static class DatabaseTaskExtension
    {
        public static void Task(this Database database)
        {
            foreach (var game in database.Games)
            {
                var reviews = game.Value.Reviews.GetSynced();

                float ratingAverage = (float)reviews.Sum(item => item.Rating);
                if (reviews.Count > 0)
                    ratingAverage /= (float)reviews.Count;
                else
                    ratingAverage = 0;

                if (ratingAverage >= 10)
                    Console.WriteLine($"{game.Value.Name}, {game.Value.Genre}, " +
                                      $"{ratingAverage}, [{String.Join(", ", reviews.Select(item => item.Id))}]");

            }
        }

        public static void FillWithR8ToR0Adapters(this Database database)
        {
            database.Clear();
            database.AddGame(new GameR8ToR0(new GameRep8(
                "Garbage Collector",
                "simulation",
                new int[] { },
                new int[] { },
                new int[] { 1 },
                "PC")));
            database.AddGame(new GameR8ToR0(new GameRep8(
                "Universe of Technology",
                "4X",
                new int[] { },
                new int[] { 3 },
                new int[] { 1, 3 },
                "bitnix")));
            database.AddGame(new GameR8ToR0(new GameRep8(
                "Moo",
                "rogue-like",
                new int[] { 2 },
                new int[] { 2, 4 },
                new int[] { 1, 2, 3 },
                "bitstation")));
            database.AddGame(new GameR8ToR0(new GameRep8(
                "Tickets Please",
                "platformer",
                new int[] { 1 },
                new int[] { 1 },
                new int[] { 1, 3, 4 },
                "bitbox")));
            database.AddGame(new GameR8ToR0(new GameRep8(
                 "Cosmic",
                "MOBA",
                new int[] { 5 },
                new int[] { 5 },
                new int[] { 1, 5 },
                "cross platform")));

            database.AddReview(new ReviewR8ToR0(new ReviewRep8(
                "I’m Commander Shepard and this is my favorite game on Smoke",
            10,
                4)));
            database.AddReview(new ReviewR8ToR0(new ReviewRep8(
                "The Moo remake sets a new standard for the future of the survival horror series⁠, even if it isn't the sequel I've been pining for.",
            12,
            2)));
            database.AddReview(new ReviewR8ToR0(new ReviewRep8(
                "Universe of Technology is a spectacular 4X game, that manages to shine even when the main campaign doesn't.",
            15,
            7)));
            database.AddReview(new ReviewR8ToR0(new ReviewRep8(
                 "Moo’s interesting art design can't save it from its glitches, bugs, and myriad terrible game design decisions, but I love its sound design",
            2,
            8)));
            database.AddReview(new ReviewR8ToR0(new ReviewRep8(
                "I've played this game for years nonstop. Over 8k hours logged (not even joking). And I'm gonna tell you: at this point, the game's just not worth playing anymore. I think it hasn't been worth playing for a year or two now, but I'm only just starting to realize it. It breaks my heart to say that, but that's just the truth of the matter.",
                5,
            1)));

            database.AddMod(new ModR8ToR0(new ModRep8(
            "Clouds",
            "Super clouds",
            new int[] { 3 },
            new int[] { 2, 3, 4, 5 })));

            database.AddMod(new ModR8ToR0(new ModRep8(
             "T-pose",
            "Cow are now T-posing",
            new int[] { 6 },
            new int[] { 1, 3 })));

            database.AddMod(new ModR8ToR0(new ModRep8(
            "Commander Shepard",
            "I’m Commander Shepard and this is my favorite mod on Smoke",
            new int[] { 4 },
            new int[] { 1, 2, 4 })));

            database.AddMod(new ModR8ToR0(new ModRep8(
                "BTM",
                "You can now play in BTM’s trains and bytebuses",
                new int[] { 7, 8 },
                new int[] { 1, 3 })));

            database.AddMod(new ModR8ToR0(new ModRep8(
                "Cosmic - black hole edition",
                "Adds REALISTIC black holes",
                new int[] { 2 },
            new int[] { 1 })));

            database.AddUser(new UserR8ToR0(new UserRep8(
                "Szredor",
                new int[] { 1, 2, 3, 4, 5 })));

            database.AddUser(new UserR8ToR0(new UserRep8(
                  "Driver",
                new int[] { 1, 2, 3, 4, 5 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "Pek",
                new int[] { 1 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "Commander Shepard",
                new int[] { 1, 2, 4 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "MLG",
                new int[] { 1, 5 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "Rondo",
                new int[] { 1 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "lemon",
                new int[] { 3 })));
            database.AddUser(new UserR8ToR0(new UserRep8(
                "Bonet",
                new int[] { 2 })));
        }

        public static void FillWithRep0(this Database database)
        {
            database.Clear();
            database.AddGame(new GameRep0("Garbage Collector", "simulation", "PC"));
            database.AddGame(new GameRep0("Universe of Technology", "4X", "bitnix"));
            database.AddGame(new GameRep0("Moo", "rogue-like", "bitstation"));
            database.AddGame(new GameRep0("Tickets Please", "platformer", "bitbox"));
            database.AddGame(new GameRep0("Cosmic", "MOBA", "cross platform"));

            database.AddReview(new ReviewRep0("I’m Commander Shepard and this is my favorite game on Smoke", 10));
            database.AddReview(new ReviewRep0("The Moo remake sets a new standard for the future of the survival horror series⁠, even if it isn't the sequel I've been pining for.", 12));
            database.AddReview(new ReviewRep0("Universe of Technology is a spectacular 4X game, that manages to shine even when the main campaign doesn't.", 15));
            database.AddReview(new ReviewRep0("Moo’s interesting art design can't save it from its glitches, bugs, and myriad terrible game design decisions, but I love its sound design", 2));
            database.AddReview(new ReviewRep0("I've played this game for years nonstop. Over 8k hours logged (not even joking). And I'm gonna tell you: at this point, the game's just not worth playing anymore. I think it hasn't been worth playing for a year or two now, but I'm only just starting to realize it. It breaks my heart to say that, but that's just the truth of the matter.", 5));

            database.AddMod(new ModRep0("Clouds", "Super clouds"));
            database.AddMod(new ModRep0("T-pose", "Cow are now T-posing"));
            database.AddMod(new ModRep0("Commander Shepard", "I’m Commander Shepard and this is my favorite mod on Smoke"));
            database.AddMod(new ModRep0("BTM", "You can now play in BTM’s trains and bytebuses"));
            database.AddMod(new ModRep0("Cosmic - black hole edition", "Adds REALISTIC black holes"));

            database.AddUser(new UserRep0("Szredor"));
            database.AddUser(new UserRep0("Driver"));
            database.AddUser(new UserRep0("Pek"));
            database.AddUser(new UserRep0("Commander Shepard"));
            database.AddUser(new UserRep0("MLG"));
            database.AddUser(new UserRep0("Rondo"));
            database.AddUser(new UserRep0("lemon"));
            database.AddUser(new UserRep0("Bonet"));

            // ADD REFERENCES FOR GAMES
            database.Games[1].SetModsByIds(new[] { 1 });

            database.Games[2].SetReviewsByIds(new[] { 3 });
            database.Games[2].SetModsByIds(new[] { 1, 3 });

            database.Games[3].SetAuthorsByIds(new[] { 2 });
            database.Games[3].SetReviewsByIds(new[] { 2, 4 });
            database.Games[3].SetModsByIds(new[] { 1, 2, 3 });

            database.Games[4].SetAuthorsByIds(new[] { 1 });
            database.Games[4].SetReviewsByIds(new[] { 1 });
            database.Games[4].SetModsByIds(new[] { 1, 3, 4 });

            database.Games[5].SetAuthorsByIds(new[] { 5 });
            database.Games[5].SetReviewsByIds(new[] { 5 });
            database.Games[5].SetModsByIds(new[] { 1, 5 });

            // ADD REFERENCES FOR REVIEWS
            database.Reviews[1].SetAuthorById(4);
            database.Reviews[2].SetAuthorById(2);
            database.Reviews[3].SetAuthorById(7);
            database.Reviews[4].SetAuthorById(8);
            database.Reviews[5].SetAuthorById(1);

            // ADD REFERENCES FOR MODS
            database.Mods[1].SetAuthorsByIds(new[] { 3 });
            database.Mods[1].SetModsByIds(new[] { 2, 3, 4, 5 });

            database.Mods[2].SetAuthorsByIds(new[] { 6 });
            database.Mods[2].SetModsByIds(new[] { 1, 3 });

            database.Mods[3].SetAuthorsByIds(new[] { 4 });
            database.Mods[3].SetModsByIds(new[] { 1, 2, 4 });

            database.Mods[4].SetAuthorsByIds(new[] { 7, 8 });
            database.Mods[4].SetModsByIds(new[] { 1, 3 });

            database.Mods[5].SetAuthorsByIds(new[] { 2 });
            database.Mods[5].SetModsByIds(new[] { 1 });

            // ADD REFERENCES FOR USERS
            database.Users[1].SetGamesByIds(new[] { 1, 2, 3, 4, 5 });
            database.Users[2].SetGamesByIds(new[] { 1, 2, 3, 4, 5 });
            database.Users[3].SetGamesByIds(new[] { 1, 2, 3, 4, 5 });
            database.Users[4].SetGamesByIds(new[] { 1, 2, 4 });
            database.Users[5].SetGamesByIds(new[] { 1, 5 });
            database.Users[6].SetGamesByIds(new[] { 1 });
            database.Users[7].SetGamesByIds(new[] { 3, 4 });
            database.Users[8].SetGamesByIds(new[] { 2 });
        }
    }

}
