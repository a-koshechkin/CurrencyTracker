namespace APIGateway.DTOs;

public record ApiDocumentationResponse
{
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public string Documentation { get; init; } = string.Empty;
    public Dictionary<string, ResourceInfo> Resources { get; init; } = new();
    public AuthenticationInfo Authentication { get; init; } = new();
    public StatusCodesInfo StatusCodes { get; init; } = new();
    public Dictionary<string, object> Examples { get; init; } = new();
}

public record ResourceInfo
{
    public string BasePath { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public EndpointInfo[] Endpoints { get; init; } = [];
}

public record EndpointInfo
{
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Auth { get; init; } = string.Empty;
}

public record AuthenticationInfo
{
    public string Type { get; init; } = string.Empty;
    public string Header { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TokenFormat { get; init; } = string.Empty;
    public string Expiration { get; init; } = string.Empty;
}

public record StatusCodesInfo
{
    public int[] Success { get; init; } = [];
    public int[] ClientError { get; init; } = [];
    public int[] ServerError { get; init; } = [];
} 