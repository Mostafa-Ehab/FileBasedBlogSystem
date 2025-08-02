namespace BlogSystem.Infrastructure.Migrations;

public interface IFileMigration
{
    string Id { get; }
    void Up();
}
