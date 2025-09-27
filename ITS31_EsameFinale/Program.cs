using ITS31_EsameFinale.Classi;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()   // permette qualsiasi origine
            .AllowAnyMethod()   // permette GET, POST, PUT, DELETE, ecc.
            .AllowAnyHeader();  // permette tutti gli headers
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

#region
var raccolta = new List<Autore>
{
    new Autore
    {
        Id = 1,
        Nominativo = "Umberto Eco",
        Libri = new List<Libro>
        {
            new Libro { Id = 1, Titolo = "Il nome della rosa", Genere = "Storico" },
            new Libro { Id = 2, Titolo = "Il pendolo di Foucault", Genere = "Mistero" }
        }
    },
    new Autore
    {
        Id = 2,
        Nominativo = "Gabriel García Márquez",
        Libri = new List<Libro>
        {
            new Libro { Id = 3, Titolo = "Cent'anni di solitudine", Genere = "Narrativa" },
            new Libro { Id = 4, Titolo = "L'amore ai tempi del colera", Genere = "Romantico" }
        }
    },
     new Autore
    {
        Id = 3,
        Nominativo = "Victor Hugo",
        Libri = new List<Libro>
        {
            new Libro { Id = 5, Titolo = "L'ultimo giorno di un condannato", Genere = "Drammatico" },
            new Libro { Id = 6, Titolo = "I miserabili", Genere = "Sociale"}
        }
    },
      new Autore
    {
        Id = 4,
        Nominativo = "George Orwell",
        Libri = new List<Libro>
        {
            new Libro { Id = 7, Titolo = "1984", Genere = "Distopico" },
            new Libro { Id = 8, Titolo = "La fattoria degli animali", Genere = "Politico" }
        }
    }
};
#endregion

// visualizzo tutti gli autori con i relativi libri (raccolta)
app.MapGet("/raccolta", () =>
{
    return Results.Ok(raccolta);
});

// visualizzo un autore tramite ricerca per Id
app.MapGet("/raccolta/{varId}", (int varId) =>
{
    Autore? aut = raccolta.FirstOrDefault(a => a.Id == varId);
    if (aut is not null)
        return Results.Ok(aut);

    return Results.NotFound();
});

// aggiungo un nuovo autore alla raccolta
app.MapPost("/raccolta", (Autore aut) =>
{
    if (aut.Nominativo == "" || aut.Libri == null) // Perchè è una lista
        return Results.BadRequest();

    aut.Id = raccolta.Count + 1;
    raccolta.Add(aut);
    return Results.Ok();
});

// modifico un autore
app.MapPut("/raccolta/{varId}", (int varId, Autore autAggiornato) =>
{
    Autore? aut = raccolta.FirstOrDefault(a => a.Id == varId);
    if (aut is null)
        return Results.NotFound("Autore non trovato");

    if (autAggiornato.Nominativo != null)
        aut.Nominativo = autAggiornato.Nominativo;

    if (autAggiornato.Libri != null)
        aut.Libri = autAggiornato.Libri;

    return Results.Ok(aut);
});

// elimino un autore
app.MapDelete("/raccolta/{varId}", (int varId) => {
    Autore? aut = raccolta.FirstOrDefault(a => a.Id == varId);
    if (aut is not null)
    {
        raccolta.Remove(aut);
        return Results.Ok(aut);
    }

    return Results.NotFound();
});

// visualizzo la lista di tutti i libri in base all'id dell'autore
app.MapGet("/raccolta/{autoreId}/libri", (int autoreId) =>
{
    var autore = raccolta.FirstOrDefault(a => a.Id == autoreId);
    if (autore is null)
        return Results.NotFound("Autore non trovato");

    return Results.Ok(autore.Libri);
});


// Aggiungo un libro ad un autore
app.MapPost("/raccolta/{autoreId}/libri", (int autoreId, Libro lib) =>
{
    var autore = raccolta.FirstOrDefault(a => a.Id == autoreId);
    if (autore is null)
        return Results.NotFound("Autore non trovato");

    if (lib.Titolo == "" || lib.Genere == "")
        return Results.BadRequest("Titolo o Genere mancanti");

    lib.Id = autore.Libri.Count + 1;

    autore.Libri.Add(lib);

    return Results.Ok(lib);
});


// Aggiorno un libro di un autore
app.MapPut("/raccolta/{autoreId}/libri/{libroId}", (int autoreId, int libroId, Libro libAggiornato) =>
{
    var autore = raccolta.FirstOrDefault(a => a.Id == autoreId);
    if (autore is null)
        return Results.NotFound("Autore non trovato");

    var libro = autore.Libri.FirstOrDefault(l => l.Id == libroId);
    if (libro is null)
        return Results.NotFound("Libro non trovato");

    if (libAggiornato.Titolo != "")
        libro.Titolo = libAggiornato.Titolo;

    if (libAggiornato.Genere != "")
        libro.Genere = libAggiornato.Genere;

    return Results.Ok(libro);
});


// Elimino un libro da un autore
app.MapDelete("/raccolta/{autoreId}/libri/{libroId}", (int autoreId, int libroId) =>
{
    
    var autore = raccolta.FirstOrDefault(a => a.Id == autoreId);
    if (autore is null)
        return Results.NotFound("Autore non trovato");

    
    var libro = autore.Libri.FirstOrDefault(l => l.Id == libroId);
    if (libro is null)
        return Results.NotFound("Libro non trovato");

    
    autore.Libri.Remove(libro);
    return Results.Ok($"Libro con Id={libroId} eliminato dall'autore: {autore.Nominativo}");
});



app.Run();

