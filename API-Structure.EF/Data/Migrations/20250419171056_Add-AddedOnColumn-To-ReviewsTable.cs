﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Structure.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAddedOnColumnToReviewsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "Reviews",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "Reviews");
        }
    }
}
