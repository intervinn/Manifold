namespace Manifold.Api.Data.DTO;

[Serializable]
public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

[Serializable]
public sealed class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}