namespace Server.Features.Pictures;

public class UploadPictureRequest {
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Tags { get; set; }
    public IFormFile File { get; set; }
}

public class ListResponse<T> {
    /// <summary>
    /// Number of page
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Total items avaliable
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// Total items in response
    /// </summary>
    public int Count { get; set; }

    public int? PrevPage { get; set; }
    public int? NextPage { get; set; }
    public int LastPage { get; set; }
    public IEnumerable<T> Data { get; set; }
}