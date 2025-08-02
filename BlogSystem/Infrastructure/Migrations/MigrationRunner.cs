using System.Text.Json;

namespace BlogSystem.Infrastructure.Migrations;

public class MigrationRunner
{
    private readonly string _stateFile = Path.Combine("Content", "migrations", "migration_state.json");

    public MigrationRunner()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_stateFile)!);
    }

    public void RunMigrations()
    {
        var applied = LoadAppliedMigrations();

        // Discover migrations via reflection
        var migrations = typeof(MigrationRunner).Assembly.GetTypes()
            .Where(t => typeof(IFileMigration).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Select(t => (IFileMigration)Activator.CreateInstance(t)!)
            .OrderBy(m => m.Id)
            .ToList();

        foreach (var migration in migrations)
        {
            if (!applied.Contains(migration.Id))
            {
                Console.WriteLine($"Applying {migration.Id}...");
                migration.Up();
                applied.Add(migration.Id);
                SaveAppliedMigrations(applied);
            }
        }
    }

    private List<string> LoadAppliedMigrations()
    {
        if (!File.Exists(_stateFile)) return [];
        var json = File.ReadAllText(_stateFile);
        return JsonSerializer.Deserialize<List<string>>(json)!;
    }

    private void SaveAppliedMigrations(List<string> applied)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_stateFile)!);
        File.WriteAllText(_stateFile, JsonSerializer.Serialize(applied));
    }
}