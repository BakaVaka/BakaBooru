using System.Security.Cryptography;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

using Server.Data;
using Server.Data.Models;

namespace Server.Features.Pictures;

[ApiController, Route("api/[controller]")]
public class PicturesController : ControllerBase {

    private const string _saveFilesDirrectory = "uploads/pictures";

    private readonly ApplicationDbContext _db;

    public PicturesController(ApplicationDbContext db) {
        _db = db;
    }

    /// <summary>
    /// Получение списка изображений с пагинацией и фильтрацией по тегам.
    /// </summary>
    /// <param name="page">Номер страницы (по умолчанию 1).</param>
    /// <param name="size">Количество элементов на странице (по умолчанию 10).</param>
    /// <param name="tags">Теги для фильтрации (через запятую).</param>
    /// <returns>Список изображений с информацией о пагинации.</returns>
    [HttpGet]
    public async Task<ActionResult<ListResponse<Picture>>> List(
        [FromQuery] int page = 1, 
        [FromQuery] int size = 10, 
        [FromQuery] string? tags = null) {

        if(page <= 0 || size <= 0) {
            return BadRequest("Page and size are should be greater than 0");
        }
        IQueryable<Picture> query = _db.Pictures.Include(x=>x.Tags);

        // filter by tags
        if(!string.IsNullOrWhiteSpace(tags)) {
            var tagsArray = tags.Split(',').Select(x=>x.ToUpperInvariant()).ToArray();
            query = query.Where(x => x.Tags.Any(y => tagsArray.Contains(y.Name)));
        }

        var total = query.Count();
        var lastPage = (int)Math.Ceiling(total / (double)size);

        var data = await query
            .OrderBy(x => x.Id)
            .Skip(size * (page - 1))
            .Take(size)
            .ToArrayAsync();

        var response = new ListResponse<Picture>
        {
            Page = page,
            Total = total,
            Count = data.Length,
            PrevPage = page > 1 ? page - 1 : (int?)null,
            NextPage = page < lastPage ? page + 1 : (int?)null,
            LastPage = lastPage,
            Data = data
        };

        return Ok(response);
    }


    /// <summary>
    /// Загрузка нового изображения.
    /// </summary>
    /// <param name="request">Запрос на загрузку изображения.</param>
    /// <returns>Результат операции загрузки.</returns>
    [HttpPost]
    public async Task<IActionResult> CreatePicture([FromForm] UploadPictureRequest request) {

        var ext = new FileInfo(request.File.FileName).Extension;
        if(IsNotPicture(ext)) {
            return BadRequest("Only .png and .jpg are supported");
        }

        using var ms = new MemoryStream();
        request.File.CopyTo(ms);

        using var sha = SHA512.Create();
        var hash = sha.ComputeHash(ms.ToArray());
        var hashString = WebEncoders.Base64UrlEncode(hash);
        
        var isExists = await _db.Pictures.AnyAsync(x=>x.Hash == hashString);
        if(isExists) {
            return BadRequest("File already exists");
        }

        try {
            var fileName = $"{hashString}{ext}";
            
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "static", _saveFilesDirrectory, fileName);
            var dir = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(dir);
            await System.IO.File.WriteAllBytesAsync(fullPath, ms.ToArray());

            List<Tag> tags = new List<Tag>();
            if(!string.IsNullOrWhiteSpace(request.Tags)) {
                var userTags = request.Tags
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(
                        x=>x.ToUpperInvariant()
                            .TrimStart()
                            .TrimEnd()
                    );
                foreach(var tag in userTags) {
                    var dbTag = await _db.Tags.FirstOrDefaultAsync(x=>x.Name == tag);
                    // create tag if it non exist
                    if(dbTag == null) {
                        dbTag = new Tag {
                            Name = tag,
                            Description = $"Tag created at {DateTime.Now}"
                        };
                        await _db.SaveChangesAsync();
                    }
                    tags.Add(dbTag);

                }
            }

            var dbItem = new Picture {
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                Extension = ext,
                Hash = hashString,
                Tags = tags,
                Path = $"/{_saveFilesDirrectory}/{fileName}",
            };

            _db.Pictures.Add(dbItem);

            _db.SaveChanges();
            return Created();
        }
        catch(Exception ex) { }
        return StatusCode(500);
    }

    /// <summary>
    /// Проверка, является ли расширение файла неподдерживаемым для изображений.
    /// </summary>
    /// <param name="ext">Расширение файла.</param>
    /// <returns>True, если расширение не поддерживается, иначе False.</returns>
    private Boolean IsNotPicture(String ext){
        return !string.Equals(ext, ".jpg", StringComparison.InvariantCultureIgnoreCase) 
            && !string.Equals(ext, ".png", StringComparison.InvariantCultureIgnoreCase);
    }
}
