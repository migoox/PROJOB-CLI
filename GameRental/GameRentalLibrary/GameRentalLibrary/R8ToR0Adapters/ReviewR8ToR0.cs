using GameRental.Rep8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Collections;
using GameRental.Extensions;
using GameRental.Rep0;

namespace GameRental.Adapters
{
    public class ReviewR8ToR0 : AbstractDatabaseEntity, IReview, IDatabaseAdapter
    {
        private ReviewRep8 review;

        public IUser Author
        {
            get
            {
                return review.GetCollectionFromStack<IUser>("author").First();
            }
            set
            {
                review.SetCollectionOnStack("author", new List<IUser>{ value });
            }
        }

        public string Text
        {
            get
            {
                return string.Join("", review.GetVariableFromStack("text"));
            }
            set
            {
                review.SetVariableOnStack("text", new string[] { value });
            }
        }

        public int Rating
        {
            get
            {
                return Convert.ToInt32(string.Join("", review.GetVariableFromStack("rating")));
            }
            set
            {
                review.SetVariableOnStack("rating", new string[] { value.ToString() });
            }
        }

        public ReviewR8ToR0(ReviewRep8 review)
        {
            this.review = review;
            Setters = new Dictionary<string, Action<object>>
            {
                ["text"] = x => this.Text = (string)x,
                ["rating"] = x => this.Rating = (int)x,
                ["author"] = x => this.Author = Database.Instance.GetByIds<IUser>((int[])x).GetSynced().First()
            };

            Getters = new Dictionary<string, Func<object>>
            {
                ["id"] = () => this.Id,
                ["text"] = () => this.Text,
                ["rating"] = () => this.Rating,
                ["author"] = () => (int)(Database.Instance.GetByRefs(new SyncList<IUser>(new List<IUser>() { this.Author }))[0])
            };

            TypeName = "review";
        }

        public void Adapt(IStackRepresentation rep)
        {
            this.review = (ReviewRep8)rep;
        }

        public override string ToString()
        {
            if (Author == null)
                return $"[ID: {Id}] " +
                       $"\"{Text}\", " +
                       $"{Rating}";
            return $"[ID: {Id}] " +
                   $"\"{Text}\", " +
                   $"{Rating}, " +
                   $"{Author.Nickname}";
        }
    }
}
