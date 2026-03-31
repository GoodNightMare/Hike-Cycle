using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HikeCycle.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddDepositAmountToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Bookings_BookingId",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_products_ProductId",
                table: "BookingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_Bookings_BookingId",
                table: "Returns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingItems",
                table: "BookingItems");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payments");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "bookings");

            migrationBuilder.RenameTable(
                name: "BookingItems",
                newName: "booking_items");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "user_profiles",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "user_profiles",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "reviews",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "reviews",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "reviews",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "Returns",
                newName: "return_date");

            migrationBuilder.RenameColumn(
                name: "IsExtraFeePaid",
                table: "Returns",
                newName: "is_extra_fee_paid");

            migrationBuilder.RenameColumn(
                name: "ExtraFee",
                table: "Returns",
                newName: "extra_fee");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Returns",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_Returns_BookingId",
                table: "Returns",
                newName: "IX_Returns_booking_id");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "promotions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "promotions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "promotions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "active",
                table: "promotions",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "promotions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "promotion_conditions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "promotion_benefits",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "variants",
                table: "products",
                newName: "Variants");

            migrationBuilder.RenameColumn(
                name: "stock",
                table: "products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "products",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "specs",
                table: "products",
                newName: "Specs");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "products",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "products",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "products",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "brand",
                table: "products",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "payments",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payments",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "payments",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_BookingId",
                table: "payments",
                newName: "IX_payments_booking_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "bookings",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "bookings",
                newName: "total_amount");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "bookings",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "FinalAmount",
                table: "bookings",
                newName: "final_amount");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "bookings",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "bookings",
                newName: "discount_amount");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "bookings",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "bookings",
                newName: "IX_bookings_user_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "booking_items",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "PricePerDay",
                table: "booking_items",
                newName: "price_per_day");

            migrationBuilder.RenameColumn(
                name: "ItemTotal",
                table: "booking_items",
                newName: "item_total");

            migrationBuilder.RenameColumn(
                name: "IsFree",
                table: "booking_items",
                newName: "is_free");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "booking_items",
                newName: "booking_id");

            migrationBuilder.RenameIndex(
                name: "IX_BookingItems_ProductId",
                table: "booking_items",
                newName: "IX_booking_items_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_BookingItems_BookingId",
                table: "booking_items",
                newName: "IX_booking_items_booking_id");

            migrationBuilder.AddColumn<int>(
                name: "booking_id",
                table: "reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "recommended_routes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "deposit_amount",
                table: "bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "shipping_address",
                table: "bookings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payments",
                table: "payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_bookings",
                table: "bookings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_booking_items",
                table: "booking_items",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "user_vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    promotion_id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_vouchers_promotions_promotion_id",
                        column: x => x.promotion_id,
                        principalTable: "promotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_vouchers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_booking_id",
                table: "reviews",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_vouchers_promotion_id",
                table: "user_vouchers",
                column: "promotion_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_vouchers_user_id",
                table: "user_vouchers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_items_bookings_booking_id",
                table: "booking_items",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_items_products_product_id",
                table: "booking_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_users_user_id",
                table: "bookings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_bookings_booking_id",
                table: "payments",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_bookings_booking_id",
                table: "Returns",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_bookings_booking_id",
                table: "reviews",
                column: "booking_id",
                principalTable: "bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_items_bookings_booking_id",
                table: "booking_items");

            migrationBuilder.DropForeignKey(
                name: "FK_booking_items_products_product_id",
                table: "booking_items");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_users_user_id",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_bookings_booking_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Returns_bookings_booking_id",
                table: "Returns");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_bookings_booking_id",
                table: "reviews");

            migrationBuilder.DropTable(
                name: "user_vouchers");

            migrationBuilder.DropIndex(
                name: "IX_reviews_booking_id",
                table: "reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bookings",
                table: "bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_booking_items",
                table: "booking_items");

            migrationBuilder.DropColumn(
                name: "booking_id",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "recommended_routes");

            migrationBuilder.DropColumn(
                name: "deposit_amount",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "shipping_address",
                table: "bookings");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "bookings",
                newName: "Bookings");

            migrationBuilder.RenameTable(
                name: "booking_items",
                newName: "BookingItems");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "user_profiles",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "user_profiles",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "reviews",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "reviews",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "reviews",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "return_date",
                table: "Returns",
                newName: "ReturnDate");

            migrationBuilder.RenameColumn(
                name: "is_extra_fee_paid",
                table: "Returns",
                newName: "IsExtraFeePaid");

            migrationBuilder.RenameColumn(
                name: "extra_fee",
                table: "Returns",
                newName: "ExtraFee");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "Returns",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Returns_booking_id",
                table: "Returns",
                newName: "IX_Returns_BookingId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "promotions",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "promotions",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "promotions",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "promotions",
                newName: "active");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promotions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promotion_conditions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "promotion_benefits",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Variants",
                table: "products",
                newName: "variants");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "products",
                newName: "stock");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "products",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Specs",
                table: "products",
                newName: "specs");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "products",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "products",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "products",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "products",
                newName: "brand");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Payments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "Payments",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_payments_booking_id",
                table: "Payments",
                newName: "IX_Payments_BookingId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Bookings",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                table: "Bookings",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Bookings",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "final_amount",
                table: "Bookings",
                newName: "FinalAmount");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "Bookings",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "discount_amount",
                table: "Bookings",
                newName: "DiscountAmount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Bookings",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_bookings_user_id",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "BookingItems",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "price_per_day",
                table: "BookingItems",
                newName: "PricePerDay");

            migrationBuilder.RenameColumn(
                name: "item_total",
                table: "BookingItems",
                newName: "ItemTotal");

            migrationBuilder.RenameColumn(
                name: "is_free",
                table: "BookingItems",
                newName: "IsFree");

            migrationBuilder.RenameColumn(
                name: "booking_id",
                table: "BookingItems",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_items_product_id",
                table: "BookingItems",
                newName: "IX_BookingItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_booking_items_booking_id",
                table: "BookingItems",
                newName: "IX_BookingItems_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookings",
                table: "Bookings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingItems",
                table: "BookingItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Bookings_BookingId",
                table: "BookingItems",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_products_ProductId",
                table: "BookingItems",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingId",
                table: "Payments",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Returns_Bookings_BookingId",
                table: "Returns",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
