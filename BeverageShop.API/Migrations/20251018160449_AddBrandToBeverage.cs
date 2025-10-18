using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeverageShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandToBeverage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Beverages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 2,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 3,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 4,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 5,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 6,
                column: "Brand",
                value: "");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 23, 4, 46, 547, DateTimeKind.Local).AddTicks(2370));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 23, 4, 46, 547, DateTimeKind.Local).AddTicks(2380));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 23, 4, 46, 547, DateTimeKind.Local).AddTicks(2383));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 23, 4, 46, 547, DateTimeKind.Local).AddTicks(2386));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 18, 23, 4, 46, 547, DateTimeKind.Local).AddTicks(2388));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Beverages");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9911));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9918));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9921));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9923));

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9926));
        }
    }
}
