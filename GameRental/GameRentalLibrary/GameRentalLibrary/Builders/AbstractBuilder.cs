using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameRental.Builders;
using GameRental.Adapters;

namespace GameRental.Builders
{
    public abstract class AbstractBuilder
    {
        protected static readonly Dictionary<string, Func<AbstractBuilder>> Builders = new()
        {
            ["game"] = () => new GameBuilder(),
            ["mod"] = () => new ModBuilder(),
            ["review"] = () => new ReviewBuilder(),
            ["user"] = () => new UserBuilder()
        };

        protected Dictionary<string, Func<object, AbstractBuilder>> Setters { get; init; }

        public Dictionary<string, Type> Types { get; init; }

        public AbstractBuilder With(string name, object obj)
        {
            if (!Setters.ContainsKey(name))
                throw new Exception($"Member \"{name}\" doesn't exist");
            return Setters[name](obj);
        }

        public static AbstractBuilder GetBuilderByType(string type)
        {
            if (!Builders.ContainsKey(type))
                throw new Exception($"Type \"{type}\" doesn't exist");

            return Builders[type]();
        }

        private static Dictionary<string, AbstractBuilder> getFieldTypebuilders =
            new Dictionary<string, AbstractBuilder>();
        public static Type GetFieldType(string type, string field)
        {
            if (!Builders.ContainsKey(type))
                throw new Exception("Type doesn't exist");

            if (!getFieldTypebuilders.ContainsKey(type))
                getFieldTypebuilders.Add(type, Builders[type]());

            return getFieldTypebuilders[type].Types[field];
        }

        public static string[] GetAvailableFieldTypes(string type)
        {
            return Builders[type]().Types.Keys.ToArray();
        }
    }
}
