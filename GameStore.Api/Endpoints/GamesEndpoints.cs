using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGameById";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
           await dbContext.Games
            .Include(games => games.Genre)
            .Select(g => g.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync()
        );

        // GET /games/{id}
        group.MapGet("/{id}", async (int id, GameStoreContext dbConext) =>
        {
            Game? game = await dbConext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
            .WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", async (CreateGameDto createGameDto, GameStoreContext dbContext) =>
        {

            Game game = createGameDto.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        // PUT /games/{id}

        group.MapPut("/{id}", async (int id, UpdateGameDto updateGameDto, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updateGameDto.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/{id}

        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                  .Where(game => game.Id == id)
                  .ExecuteDeleteAsync();

            return Results.NoContent();
        });

        return group;
    }
}
