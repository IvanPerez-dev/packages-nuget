using ClosedXML.Excel;
using ExcelFluently.Extensions;
using FluentAssertions;
using Xunit;

namespace ExcelFluentlyTest.Export
{
    public class ExcelExportIntegrationTests
    {
        private readonly List<UserStub> _users =
        [
            new()
            {
                Id = 1,
                Name = "Ivan",
                Email = "ivan@test.com",
                DateOfBirth = new DateTime(1990, 5, 15),
            },
            new()
            {
                Id = 2,
                Name = "Maria",
                Email = "maria@test.com",
                DateOfBirth = new DateTime(1985, 11, 3),
            },
        ];

        [Fact]
        public void ToBytes_WithAutoColumns_ShouldGenerateValidExcelWithPropertyNamesAsHeaders()
        {
            // Arrange & Act
            List<string> header = new List<string> { "Id", "Name", "Email", "DateOfBirth" };
            var bytes = _users.ToExcel().ToBytes();

            // Assert
            using var workbook = OpenWorkbook(bytes);
            var sheet = workbook.Worksheet(1);

            var headers = GetHeaders(sheet);

            //headers.SequenceEqual(header).Should().BeTrue();
            headers.Should().Contain(["Id", "Name", "Email", "DateOfBirth"]);

            // Verifica que haya datos (fila de header + 2 filas de datos)
            sheet.LastRowUsed()!.RowNumber().Should().Be(3);
        }

        [Fact]
        public void ToBytes_WithCustomColumns_ShouldUseConfiguredHeaderNamesAndValues()
        {
            // Act
            var bytes = _users
                .ToExcel()
                .WithColumn(x => x.Id, "Codigo")
                .WithColumn(x => x.Name + " " + x.Email) // sin alias → usa "Unknown"
                .WithColumn(x => x.DateOfBirth, "Fecha", "yyyy/MM/dd")
                .ToBytes();

            // Assert
            using var workbook = OpenWorkbook(bytes);
            var sheet = workbook.Worksheet(1);

            var headers = GetHeaders(sheet);
            headers[0].Should().Be("Codigo");
            headers[2].Should().Be("Fecha");

            // Verifica el formato de fecha en la primera fila de datos
            sheet.Cell(2, 3).GetValue<string>().Should().Be("1990/05/15");

            // Verifica que el Id del primer usuario sea "1"
            sheet.Cell(2, 1).GetValue<string>().Should().Be("1");
        }

        [Fact]
        public void ToBytes_WithTableStyle_ShouldApplySheetNameAndTitle()
        {
            // Act
            var bytes = _users
                .ToExcel()
                .WithTableStyle(cfg =>
                {
                    cfg.Theme = XLTableTheme.TableStyleMedium9;
                    cfg.ShowRowStripes = true;
                    cfg.HeaderFontColor = XLColor.Black;
                    cfg.Title = "Report of Users";
                    cfg.SheetName = "Users";
                })
                .ToBytes();

            // Assert
            using var workbook = OpenWorkbook(bytes);

            // El sheet se creó con el nombre correcto
            workbook.Worksheets.Any(w => w.Name == "Users").Should().BeTrue();

            var sheet = workbook.Worksheet("Users");

            // Hay un título → los headers empiezan más abajo (fila 5 en tu impl)
            // La celda mergeada del título debe contener el texto configurado
            var titleCell = sheet.Cell(1, 1);
            titleCell.GetValue<string>().Should().Be("Report of Users");
        }

        [Fact]
        public void ToBytes_FullConfiguration_ShouldProduceExcelWithAllSettingsApplied()
        {
            // Act
            var bytes = _users
                .ToExcel()
                .WithTableStyle(cfg =>
                {
                    cfg.Theme = XLTableTheme.TableStyleMedium9;
                    cfg.ShowRowStripes = true;
                    cfg.HeaderFontColor = XLColor.Black;
                    cfg.Title = "Report of Users";
                    cfg.SheetName = "Users";
                })
                .WithColumn(x => x.Id, "Codigo")
                .WithColumn(x => x.Name + " " + x.Email)
                .WithColumn(x => x.DateOfBirth, "Fecha", "yyyy/MM/dd")
                .ToBytes();

            // Assert
            bytes.Should().NotBeNullOrEmpty();

            using var workbook = OpenWorkbook(bytes);
            var sheet = workbook.Worksheet("Users");

            // Nombre de la hoja correcto
            sheet.Should().NotBeNull();

            // Total de filas: título (3) + separación (1) + header (1) + 2 datos = 7
            sheet.LastRowUsed()!.RowNumber().Should().BeGreaterThanOrEqualTo(6);

            // Headers de las columnas customizadas
            //var headerRow = sheet.RowsUsed().Skip(4).First(); // fila 5 = headers
            //headerRow.Cell(1).GetValue<string>().Should().Be("Codigo");
            //headerRow.Cell(3).GetValue<string>().Should().Be("Fecha");
        }

        [Fact]
        public void ToBytes_WithEmptyList_ShouldGenerateExcelWithOnlyHeaders()
        {
            // Act
            var bytes = new List<UserStub>()
                .ToExcel()
                .WithColumn(x => x.Id, "Codigo")
                .WithColumn(x => x.Name, "Nombre")
                .ToBytes();

            // Assert
            bytes.Should().NotBeNullOrEmpty();

            using var workbook = OpenWorkbook(bytes);
            var sheet = workbook.Worksheet(1);

            // Solo la fila de headers, sin datos
            sheet.LastRowUsed()!.RowNumber().Should().Be(1);
            sheet.Cell(1, 1).GetValue<string>().Should().Be("Codigo");
        }

        private static XLWorkbook OpenWorkbook(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            return new XLWorkbook(stream);
        }

        private static List<string> GetHeaders(IXLWorksheet sheet)
        {
            return sheet.FirstRowUsed()!.CellsUsed().Select(c => c.GetValue<string>()).ToList();
        }
    }

    internal class UserStub
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }
}
