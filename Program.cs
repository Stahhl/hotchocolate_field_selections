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
    query {
        human {
            pets {
                ... on Cat {
                    sound
                }
                ... on Dog {
                    sound
                }
            }
        }
    }
    */

    public Human GetHuman(
        [IsSelected("pets")] bool petsIsSelected // true
        , [IsSelected("pets { sound }")] bool soundOnPetIsSelected // false
        , [IsSelected(
            """
            pets {
              ... on Cat {
                sound
              }
              ... on Dog {
                sound
              }
            }
            """)]
        bool soundOnFragmentIsSelected // The type condition `Cat` of the inline fragment is not assignable from the parent field type `IPet`.
    )
    {
        return new Human { Pets = [new Cat(), new Dog()] };
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