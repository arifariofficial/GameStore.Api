using GameStore.Api.Dto;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string GetGameEndpointName = "GetGameById";

List<GameDto> games = [
    new(
        1,
        "Street Fighter 2",
        "Fighting",
        19.99M,
        new DateOnly(2000, 12, 31)),
    new (
        2,
        "Final Fantasy XIV",
        "Roleplaying",
        59.99M,
        new DateOnly(2010, 9, 30)),
    new (
        3,
        "FIFA 23",
        "Sports",
        69.99M,
        new DateOnly(2022, 9, 27)),
];


// GET /games
app.MapGet("/games", () => games);

// GET /games/{id}
app.MapGet("games/{id}", (int id) => games.Find(g => g.Id == id))
    .WithName(GetGameEndpointName);

// POST /games
app.MapPost("/games", (CreateGameDto createGameDto) =>
{
    GameDto newGame = new(
        games.Max(g => g.Id) + 1,
        createGameDto.Name,
        createGameDto.Genre,
        createGameDto.Price,
        createGameDto.ReleaseDate
    );
    games.Add(newGame);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id = newGame.Id }, newGame);
});

app.Run();
