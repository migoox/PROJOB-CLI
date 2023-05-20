using GameRental.Adapters;
using GameRental.Rep0;
using GameRental.Rep8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRental.Builders
{
    public class ReviewBuilder : AbstractBuilder, IDatabaseBuilder
    {

        private string text = "null";
        private int rating = -1;
        private IUser? author = null;
        public ReviewBuilder()
        {
            Setters = new Dictionary<string, Func<object, AbstractBuilder>>
            {
                ["text"] = x => WithText((string)x),
                ["author"] = x => WithAuthorById((int)x),
                ["rating"] = x => WithRating((int)x)
            };

            Types = new Dictionary<string, Type>
            {
                ["text"] = typeof(string),
                ["author"] = typeof(int),
                ["rating"] = typeof(int)
            };
        }

        public ReviewBuilder WithText(string text)
        {
            this.text = text;
            return this;
        }

        public ReviewBuilder WithRating(int rating)
        {
            this.rating = rating;
            return this;
        }

        public ReviewBuilder WithAuthor(IUser author)
        {
            this.author = author;
            return this;
        }
        public ReviewBuilder WithAuthorById(int authorId)
        {
            this.author = Database.Instance.Users.Values.First(x => x.Id == authorId);
            return this;
        }

        public IDatabaseEntity BuildRep0()
        {
            if (author == null)
                return new ReviewRep0(text, rating);
            return new ReviewRep0(text, rating, author);
        }

        public IStackRepresentation BuildRep8()
        {
            if (author == null)
                return new ReviewRep8(text, rating, -1);
            return new ReviewRep8(text, rating, author.Id);
        }

        public IDatabaseEntity BuildRep8AndAdapt()
        {
            return new ReviewR8ToR0((ReviewRep8)BuildRep8());
        }

        public override string ToString()
        {
            if (author == null)
                return text + ", " + rating;
            return text + ", " + rating + ", " + author.Id;
        }
    }
}
