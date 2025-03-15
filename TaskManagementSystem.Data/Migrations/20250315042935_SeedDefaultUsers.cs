using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            //AddFullTextIndexToTasks
            //migrationBuilder.Sql("CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;");
            //migrationBuilder.Sql("CREATE FULLTEXT INDEX ON Tasks(Title) KEY INDEX PK_Tasks;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.Sql("DROP FULLTEXT INDEX ON Tasks;");
            //migrationBuilder.Sql("DROP FULLTEXT CATALOG ftCatalog;");

        }
    }
}
