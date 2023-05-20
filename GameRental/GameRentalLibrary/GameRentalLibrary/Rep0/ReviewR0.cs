using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GameRental.Collections;

namespace GameRental.Rep0
{
    public class ReviewRep0 : AbstractDatabaseEntity, IReview
    {
        public string Text { get; set; }

        public int Rating { get; set; }

        public IUser Author { get; set; }

        public ReviewRep0(string text, int rating, IUser? author = null)
        {
            this.Text = text;
            this.Rating = rating;

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
                ["author"] = () => (int)(Database.Instance.GetByRefs(new SyncList<IUser>(new List<IUser>() { this.Author })))[0]
            };
            

            TypeName = "review";

            if (author != null)
                this.Author = author;
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

