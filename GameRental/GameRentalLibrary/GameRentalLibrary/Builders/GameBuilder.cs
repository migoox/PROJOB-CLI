using GameRental.Adapters;
using GameRental.Rep0;
using GameRental.Rep8;

namespace GameRental.Builders
{
    public class GameBuilder : AbstractBuilder, IDatabaseBuilder
    {
        private string name = "null";
        private string genre = "null";
        private string devices = "null";
        private List<IUser> authors = new List<IUser>();
        private List<IMod> mods = new List<IMod>();
        private List<IReview> reviews = new List<IReview>();

        public GameBuilder()
        {
            Setters = new Dictionary<string, Func<object, AbstractBuilder>>
            {
                ["name"] = x => WithName((string)x),
                ["genre"] = x => WithGenre((string)x),
                ["devices"] = x => WithDevices((string)x),
                ["reviews"] = x => WithReviewsByIds((int[])x),
                ["mods"] = x => WithModsByIds((int[])x),
                ["authors"] = x => WithAuthorsByIds((int[])x)
            };

            Types = new Dictionary<string, Type>
            {
                ["name"] = typeof(string),
                ["genre"] = typeof(string),
                ["devices"] = typeof(string),
                ["reviews"] = typeof(int[]),
                ["mods"] = typeof(int[]),
                ["authors"] = typeof(int[])
            };
        }

        public GameBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public GameBuilder WithGenre(string genre)
        {
            this.genre = genre;
            return this;
        }

        public GameBuilder WithDevices(string devices)
        {
            this.devices = devices;
            return this;
        }

        public GameBuilder WithAuthors(List<IUser> authors)
        {
            this.authors = authors;
            return this;
        }

        public GameBuilder WithAuthorsByIds(int[] authorsIds)
        {
            this.authors = Database.Instance.Users
                .Select(x => x.Value)
                .Where(x => authorsIds.Contains(x.Id))
                .ToList();
            return this;
        }

        public GameBuilder WithReviews(List<IReview> reviews)
        {
            this.reviews = reviews;
            return this;
        }
        public GameBuilder WithReviewsByIds(int[] reviewsIds)
        {
            this.reviews = Database.Instance.Reviews
                .Select(x => x.Value)
                .Where(x => reviewsIds.Contains(x.Id))
                .ToList();
            return this;
        }
        public GameBuilder WithMods(List<IMod> mods)
        {
            this.mods = mods;
            return this;
        }
        public GameBuilder WithModsByIds(int[] modsIds)
        {
            this.mods = Database.Instance.Mods
                .Select(x => x.Value)
                .Where(x => modsIds.Contains(x.Id))
                .ToList();

            return this;
        }

        public IDatabaseEntity BuildRep0()
        {
            return new GameRep0(name, genre, devices, authors, mods, reviews);
        }

        public IStackRepresentation BuildRep8()
        {
            return new GameRep8(name, genre,
                authors.Select(x => x.Id).ToArray(),
                reviews.Select(x => x.Id).ToArray(),
                mods.Select(x => x.Id).ToArray(),
                devices);
        }

        public IDatabaseEntity BuildRep8AndAdapt()
        {
            return new GameR8ToR0((GameRep8)BuildRep8());
        }

        public override string ToString()
        {
            return name + 
                   genre + ", " +
                   "[" + string.Join(", ", authors.Select(x => x.Id).ToArray()) + "]" +
                   "[" + string.Join(", ", reviews.Select(x => x.Id).ToArray()) + "]" +
                   "[" + string.Join(", ", mods.Select(x => x.Id).ToArray()) + "]" +
                   devices;
        }
    }
}
