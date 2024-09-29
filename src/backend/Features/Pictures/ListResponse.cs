namespace Server.Features.Pictures;

/// <summary>
/// Результат запроса с пагинацией
/// </summary>
/// <typeparam name="T">Тип данных</typeparam>
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

    /// <summary>
    /// Номер предыдущей страниц
    /// </summary>
    public int? PrevPage { get; set; }

    /// <summary>
    /// Номер следующей страницы
    /// </summary>
    public int? NextPage { get; set; }

    /// <summary>
    /// Последняя страница
    /// </summary>
    public int LastPage { get; set; }

    /// <summary>
    /// Данные
    /// </summary>
    public IEnumerable<T> Data { get; set; }
}