namespace Server.Features.Pictures;

/// <summary>
/// Запрос для загрузки нового файла
/// </summary>
public class UploadPictureRequest {

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// List of tags separated by ',' char
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// File to upload
    /// </summary>
    public IFormFile File { get; set; }
}
