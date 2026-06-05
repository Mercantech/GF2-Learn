using GF2Learn.Web.Data;
using GF2Learn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace GF2Learn.Web.Services;

public interface IPlaygroundProjectService
{
    Task<IReadOnlyList<PlaygroundProjectSummaryDto>> ListProjectsAsync(
        string userSub,
        CancellationToken cancellationToken = default);

    Task<PlaygroundProjectDetailDto?> GetProjectAsync(
        string userSub,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<PlaygroundProjectDetailDto> CreateProjectAsync(
        string userSub,
        string? name,
        CancellationToken cancellationToken = default);

    Task<PlaygroundProjectDetailDto?> RenameProjectAsync(
        string userSub,
        Guid projectId,
        string name,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteProjectAsync(
        string userSub,
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<PlaygroundProjectDetailDto?> SaveFilesAsync(
        string userSub,
        Guid projectId,
        IReadOnlyList<SavePlaygroundFileItem> files,
        CancellationToken cancellationToken = default);

    Task<PlaygroundFileDto?> AddFileAsync(
        string userSub,
        Guid projectId,
        string fileName,
        string? content,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(
        string userSub,
        Guid projectId,
        long fileId,
        CancellationToken cancellationToken = default);
}

public sealed class PlaygroundProjectService(Gf2LearnDbContext db) : IPlaygroundProjectService
{
    private const int MaxProjectsPerUser = 10;
    private const int MaxFilesPerProject = 20;
    private const int MaxFileLength = 64_000;

    private const string DefaultProgramTemplate = """
        Console.WriteLine("Hej fra playground!");
        Console.Write("Dit navn: ");
        string name = Console.ReadLine()!;
        Console.WriteLine($"Hej, {name}!");
        """;

    public async Task<IReadOnlyList<PlaygroundProjectSummaryDto>> ListProjectsAsync(
        string userSub,
        CancellationToken cancellationToken = default)
    {
        return await db.PlaygroundProjects
            .AsNoTracking()
            .Where(p => p.UserSub == userSub)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => new PlaygroundProjectSummaryDto(
                p.Id,
                p.Name,
                p.UpdatedAt,
                p.Files.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<PlaygroundProjectDetailDto?> GetProjectAsync(
        string userSub,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await db.PlaygroundProjects
            .AsNoTracking()
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserSub == userSub, cancellationToken);

        return project is null ? null : ToDetailDto(project);
    }

    public async Task<PlaygroundProjectDetailDto> CreateProjectAsync(
        string userSub,
        string? name,
        CancellationToken cancellationToken = default)
    {
        var count = await db.PlaygroundProjects.CountAsync(p => p.UserSub == userSub, cancellationToken);
        if (count >= MaxProjectsPerUser)
            throw new InvalidOperationException($"Du kan højst have {MaxProjectsPerUser} projekter.");

        var now = DateTimeOffset.UtcNow;
        var projectName = NormalizeProjectName(name) ?? await NextProjectNameAsync(userSub, cancellationToken);

        var project = new PlaygroundProject
        {
            Id = Guid.NewGuid(),
            UserSub = userSub,
            Name = projectName,
            CreatedAt = now,
            UpdatedAt = now,
            Files =
            [
                new PlaygroundFile
                {
                    FileName = "Program.cs",
                    Content = DefaultProgramTemplate,
                    SortOrder = 0,
                    UpdatedAt = now
                }
            ]
        };

        db.PlaygroundProjects.Add(project);
        await db.SaveChangesAsync(cancellationToken);
        return ToDetailDto(project);
    }

    public async Task<PlaygroundProjectDetailDto?> RenameProjectAsync(
        string userSub,
        Guid projectId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeProjectName(name);
        if (normalized is null)
            return null;

        var project = await db.PlaygroundProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserSub == userSub, cancellationToken);

        if (project is null)
            return null;

        project.Name = normalized;
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return await GetProjectAsync(userSub, projectId, cancellationToken);
    }

    public async Task<bool> DeleteProjectAsync(
        string userSub,
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var project = await db.PlaygroundProjects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserSub == userSub, cancellationToken);

        if (project is null)
            return false;

        db.PlaygroundProjects.Remove(project);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PlaygroundProjectDetailDto?> SaveFilesAsync(
        string userSub,
        Guid projectId,
        IReadOnlyList<SavePlaygroundFileItem> files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0 || files.Count > MaxFilesPerProject)
            return null;

        var project = await db.PlaygroundProjects
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserSub == userSub, cancellationToken);

        if (project is null)
            return null;

        var now = DateTimeOffset.UtcNow;
        var incomingIds = files.Where(f => f.Id.HasValue).Select(f => f.Id!.Value).ToHashSet();
        var toRemove = project.Files.Where(f => !incomingIds.Contains(f.Id)).ToList();
        foreach (var file in toRemove)
            db.PlaygroundFiles.Remove(file);

        foreach (var item in files.OrderBy(f => f.SortOrder))
        {
            var fileName = NormalizeFileName(item.FileName);
            if (fileName is null)
                continue;

            var content = TruncateContent(item.Content);
            PlaygroundFile? existing = item.Id.HasValue
                ? project.Files.FirstOrDefault(f => f.Id == item.Id.Value)
                : project.Files.FirstOrDefault(f => f.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                project.Files.Add(new PlaygroundFile
                {
                    FileName = fileName,
                    Content = content,
                    SortOrder = item.SortOrder,
                    UpdatedAt = now
                });
            }
            else
            {
                existing.FileName = fileName;
                existing.Content = content;
                existing.SortOrder = item.SortOrder;
                existing.UpdatedAt = now;
            }
        }

        project.UpdatedAt = now;
        await db.SaveChangesAsync(cancellationToken);
        return await GetProjectAsync(userSub, projectId, cancellationToken);
    }

    public async Task<PlaygroundFileDto?> AddFileAsync(
        string userSub,
        Guid projectId,
        string fileName,
        string? content,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeFileName(fileName);
        if (normalized is null)
            return null;

        var project = await db.PlaygroundProjects
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserSub == userSub, cancellationToken);

        if (project is null || project.Files.Count >= MaxFilesPerProject)
            return null;

        if (project.Files.Any(f => f.FileName.Equals(normalized, StringComparison.OrdinalIgnoreCase)))
            return null;

        var now = DateTimeOffset.UtcNow;
        var file = new PlaygroundFile
        {
            FileName = normalized,
            Content = TruncateContent(content) ?? "",
            SortOrder = project.Files.Count == 0 ? 0 : project.Files.Max(f => f.SortOrder) + 1,
            UpdatedAt = now
        };

        project.Files.Add(file);
        project.UpdatedAt = now;
        await db.SaveChangesAsync(cancellationToken);

        return new PlaygroundFileDto(file.Id, file.FileName, file.Content, file.SortOrder, file.UpdatedAt);
    }

    public async Task<bool> DeleteFileAsync(
        string userSub,
        Guid projectId,
        long fileId,
        CancellationToken cancellationToken = default)
    {
        var file = await db.PlaygroundFiles
            .Include(f => f.Project)
            .FirstOrDefaultAsync(
                f => f.Id == fileId && f.ProjectId == projectId && f.Project!.UserSub == userSub,
                cancellationToken);

        if (file is null)
            return false;

        var project = file.Project!;
        if (project.Files.Count <= 1)
            return false;

        db.PlaygroundFiles.Remove(file);
        project.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PlaygroundProjectDetailDto ToDetailDto(PlaygroundProject project) =>
        new(
            project.Id,
            project.Name,
            project.CreatedAt,
            project.UpdatedAt,
            project.Files
                .OrderBy(f => f.SortOrder)
                .ThenBy(f => f.FileName)
                .Select(f => new PlaygroundFileDto(f.Id, f.FileName, f.Content, f.SortOrder, f.UpdatedAt))
                .ToList());

    private async Task<string> NextProjectNameAsync(string userSub, CancellationToken cancellationToken)
    {
        var baseName = "Mit projekt";
        var names = await db.PlaygroundProjects
            .AsNoTracking()
            .Where(p => p.UserSub == userSub)
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        if (!names.Contains(baseName, StringComparer.OrdinalIgnoreCase))
            return baseName;

        for (var i = 2; i <= 99; i++)
        {
            var candidate = $"{baseName} {i}";
            if (!names.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                return candidate;
        }

        return $"{baseName} {Guid.NewGuid():N}"[..128];
    }

    private static string? NormalizeProjectName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var trimmed = name.Trim();
        return trimmed.Length > 128 ? trimmed[..128] : trimmed;
    }

    private static string? NormalizeFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return null;

        var trimmed = fileName.Trim().Replace('\\', '/');
        if (trimmed.Contains('/') || trimmed.Contains(".."))
            return null;

        if (!trimmed.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            trimmed += ".cs";

        return trimmed.Length > 128 ? trimmed[..128] : trimmed;
    }

    private static string? TruncateContent(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return content?.Trim();

        var trimmed = content.Trim();
        return trimmed.Length > MaxFileLength ? trimmed[..MaxFileLength] : trimmed;
    }
}
