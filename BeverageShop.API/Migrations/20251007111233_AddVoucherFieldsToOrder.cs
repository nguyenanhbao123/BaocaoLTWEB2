using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeverageShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicableBeverageIds",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Vouchers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Type" },
                values: new object[] { "Trà sữa trân châu đường đen thơm ngon, ngọt dịu", "Trà Sữa Trân Châu", "Trà sữa" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Type" },
                values: new object[] { "Cà phê đen nguyên chất, đậm đà hương vị Việt Nam", "Cà Phê Đen Đá", "Cà phê đen" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Name", "Type" },
                values: new object[] { "Nước ép", "Nước ép cam tươi 100%, giàu vitamin C", "Nước Ép Cam", "Nước ép" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Name", "Type" },
                values: new object[] { "Sinh tố", "Sinh tố bơ béo ngậy, thơm ngon, bổ dưỡng", "Sinh Tố Bơ", "Sinh tố" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Trà đào cam sả thanh mát, hương vị độc đáo", "Trà Đào Cam Sả" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "Soda chanh dây tươi mát, giải khát tuyệt vời");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Các loại trà");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Các loại cà phê");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Nước ép trái cây tươi", "Nước ép" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Sinh tố hoa quả", "Sinh tố" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Soda và nước có ga");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Address",
                value: "Hà Nội");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicableBeverageIds", "CreatedDate", "Description", "MaxDiscountAmount", "Name" },
                values: new object[] { null, new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9911), "Giảm 10% cho khách hàng mới - Không giới hạn đơn hàng", 0m, "" });

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicableBeverageIds", "CreatedDate", "Description", "MaxDiscountAmount", "Name" },
                values: new object[] { null, new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9918), "Miễn phí ship cho đơn từ 100k", 0m, "" });

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApplicableBeverageIds", "CreatedDate", "Description", "MaxDiscountAmount", "Name" },
                values: new object[] { null, new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9921), "Giảm 50k cho đơn hàng từ 200k", 0m, "" });

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApplicableBeverageIds", "CreatedDate", "Description", "MaxDiscountAmount", "Name" },
                values: new object[] { null, new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9923), "Flash Sale - Giảm 20% (Đơn từ 150k, giới hạn 20 lượt)", 0m, "" });

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ApplicableBeverageIds", "CreatedDate", "Description", "MaxDiscountAmount", "Name" },
                values: new object[] { null, new DateTime(2025, 10, 7, 18, 12, 30, 937, DateTimeKind.Local).AddTicks(9926), "Giảm 15% cho đồ uống yêu thích (Đơn từ 80k)", 0m, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicableBeverageIds",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Type" },
                values: new object[] { "Trà s?a trân châu du?ng den thom ngon, ng?t d?u", "Trà S?a Trân Châu", "Trà s?a" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Type" },
                values: new object[] { "Cà phê den nguyên ch?t, d?m dà huong v? Vi?t Nam", "Cà Phê Ðen Ðá", "Cà phê den" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "Name", "Type" },
                values: new object[] { "Nu?c ép", "Nu?c ép cam tuoi 100%, giàu vitamin C", "Nu?c Ép Cam", "Nu?c ép" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "Description", "Name", "Type" },
                values: new object[] { "Sinh t?", "Sinh t? bo béo ng?y, thom ngon, b? du?ng", "Sinh T? Bo", "Sinh t?" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Trà dào cam s? thanh mát, huong v? d?c dáo", "Trà Ðào Cam S?" });

            migrationBuilder.UpdateData(
                table: "Beverages",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "Soda chanh dây tuoi mát, gi?i khát tuy?t v?i");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Các lo?i trà");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Các lo?i cà phê");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Nu?c ép trái cây tuoi", "Nu?c ép" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Sinh t? hoa qu?", "Sinh t?" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Soda và nu?c có ga");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Address",
                value: "Hà N?i");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Gi?m 10% cho khách hàng m?i - Không gi?i h?n don hàng");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Mi?n phí ship cho don t? 100k");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Gi?m 50k cho don hàng t? 200k");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Flash Sale - Gi?m 20% (Ðon t? 150k, gi?i h?n 20 lu?t)");

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Gi?m 15% cho d? u?ng yêu thích (Ðon t? 80k)");
        }
    }
}
