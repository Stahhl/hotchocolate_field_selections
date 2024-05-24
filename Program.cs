using HotChocolate.Language;
using HotChocolate.Resolvers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQLServer()
    .AddType<Cat>()
    .AddType<Dog>()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGraphQL();

app.Run();

public class Query
{
    /*
    query{
        human{
            pets{
                ... on Cat{
                    __typename
                }
                ... on Dog{
                    __typename
                    sound
                }
            }
        }
    }
    */

    public Human GetHuman(IResolverContext context)
    {
        var soundIsSelected = (bool)context.GetSelections((ObjectType)context.Selection.Type.NamedType())
            .FirstOrDefault(f => f.Field.Name.Equals("pets"))?.SelectionSet?.Selections.Any(
                x =>
                {
                    var fields = (x as InlineFragmentNode)?.SelectionSet.Selections.OfType<FieldNode>() ??
                                 Array.Empty<FieldNode>();
                    return fields.Any(f => f.Name.Value.Equals("sound"));
                });

        Console.WriteLine($"result: {soundIsSelected}");
        return new Human { Pets = [new Cat(), new Dog()] };
    }

    public IPet GetPet([IsSelected("sound")] bool soundIsSelected)
    {
        Console.WriteLine($"result: {soundIsSelected}");
        return new Cat();
    }
}

public class Human
{
    public IPet[] Pets { get; set; } = [];
}

public interface IPet
{
    string Sound { get; }
}

public class Cat : IPet
{
    public string Sound => "meow";
}

public class Dog : IPet
{
    public string Sound => "bark";
}