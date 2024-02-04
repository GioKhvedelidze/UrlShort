using Microsoft.EntityFrameworkCore;
using UrlShortProject.Data;
using UrlShortProject.Models;
using UrlShortProject.Models.Dto;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
    ));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/shortUrl", async (ShortUrlDto url, AppDbContext appDbContext, HttpContext ctx) =>
{
    //Validating user input
    if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
    {
        return Results.BadRequest("Invalid URL!");
    }
    
    //Creating Short version of URL
    var random = new Random();
    const string charsForRandom = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz";
    var randomString = new string(Enumerable.Repeat(charsForRandom, 10)
        .Select(x => x[random.Next(x.Length)]).ToArray());

    //Mapping short url
    var sUrl = new ShortUrl()
    {
        Url = url.Url,
        CompressedUrl = randomString
    };

    //Saving the mapping into the DB
    appDbContext.Add(sUrl);
    await appDbContext.SaveChangesAsync();
    
    //construct url
    var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.CompressedUrl}";

    return Results.Ok(new UrlShortResponseDto()
    {
        Url = result
    });
});

app.MapFallback(async (AppDbContext appDbContext, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Trim('/');
    var urlMatch = await appDbContext.Urls.FirstOrDefaultAsync(x =>
        x.CompressedUrl.Trim() == path.Trim());

    return urlMatch == null ? Results.NotFound("Url is not found!") : Results.Redirect(urlMatch.Url);
});

app.Run();
