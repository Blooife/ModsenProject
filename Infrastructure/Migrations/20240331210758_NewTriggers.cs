using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Триггер для обновления PlacesLeft при вставке и обновлении записей в Events
                CREATE OR REPLACE FUNCTION update_places_left_on_insert_or_update()
                RETURNS TRIGGER AS $$
                BEGIN
                    UPDATE ""Events"" SET ""PlacesLeft"" = ""MaxParticipants"" - (
                        SELECT COUNT(*) FROM ""EventsUsers"" WHERE ""EventId"" = NEW.""Id""
                    ) WHERE ""Id"" = NEW.""Id"";
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER update_places_left_on_insert_or_update_trigger
                AFTER INSERT OR UPDATE ON ""Events""
                FOR EACH ROW
                EXECUTE FUNCTION update_places_left_on_insert_or_update();

                -- Триггер для обновления PlacesLeft при вставке записей в EventsUsers
                CREATE OR REPLACE FUNCTION update_places_left_on_insert()
                RETURNS TRIGGER AS $$
                BEGIN
                    UPDATE ""Events"" SET ""PlacesLeft"" = ""MaxParticipants"" - (
                        SELECT COUNT(*) FROM ""EventsUsers"" WHERE ""EventId"" = NEW.""EventId""
                    ) WHERE ""Id"" = NEW.""EventId"";
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER update_places_left_on_insert_trigger
                AFTER INSERT ON ""EventsUsers""
                FOR EACH ROW
                EXECUTE FUNCTION update_places_left_on_insert();

                -- Триггер для обновления PlacesLeft при удалении записей из EventsUsers
                CREATE OR REPLACE FUNCTION update_places_left_on_delete()
                RETURNS TRIGGER AS $$
                BEGIN
                    UPDATE ""Events"" SET ""PlacesLeft"" = ""MaxParticipants"" - (
                        SELECT COUNT(*) FROM ""EventsUsers"" WHERE ""EventId"" = OLD.""EventId""
                    ) WHERE ""Id"" = OLD.""EventId"";
                    RETURN OLD;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER update_places_left_on_delete_trigger
                AFTER DELETE ON ""EventsUsers""
                FOR EACH ROW
                EXECUTE FUNCTION update_places_left_on_delete();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PlacesLeft",
                table: "Events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS update_places_left_on_insert_or_update_trigger ON ""Events"";
                DROP FUNCTION IF EXISTS update_places_left_on_insert_or_update();

                DROP TRIGGER IF EXISTS update_places_left_on_insert_trigger ON ""EventsUsers"";
                DROP FUNCTION IF EXISTS update_places_left_on_insert();

                DROP TRIGGER IF EXISTS update_places_left_on_delete_trigger ON ""EventsUsers"";
                DROP FUNCTION IF EXISTS update_places_left_on_delete();
            ");
        }
    }
}
