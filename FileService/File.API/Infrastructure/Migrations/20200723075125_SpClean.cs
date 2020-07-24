using Microsoft.EntityFrameworkCore.Migrations;

namespace FileService.File.API.Infrastructure.Migrations
{
    public partial class SpClean : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var getNotUsedImages = @"CREATE PROCEDURE [dbo].[GetNotUsedAppImages] @Tag int
									AS
									BEGIN
										SET NOCOUNT ON;

										select * from [Arise.File].[dbo].[FileInfos] where Tag = @Tag
										and Name not in 
										(
											select distinct BackgroundImage as Name from [Photography.Post].[dbo].[Circles] where BackgroundImage is not null
											union 
											select distinct Name from [Photography.Post].[dbo].[PostAttachments] where Name is not null and AttachmentType = 0
											union 
											select distinct Name from [Photography.User].[dbo].[AlbumPhotos] where Name is not null
											union 
											select distinct Avatar as Name from [Photography.User].[dbo].[Groups] where Avatar is not null
											union 
											select distinct Avatar as Name from [Photography.User].[dbo].[Users] where Avatar is not null
											union 
											select distinct BackgroundImage as Name from [Photography.User].[dbo].[Users] where BackgroundImage is not null
											union 
											select distinct Name from [Photography.Order].[dbo].[Attachments] where Name is not null and AttachmentType = 0
										)
									END";

			var getNotUsedVideos = @"CREATE PROCEDURE [dbo].[GetNotUsedAppVideos] @Tag int
									AS
									BEGIN
										SET NOCOUNT ON;

										select * from [Arise.File].[dbo].[FileInfos] where Tag = @Tag
										and Name not in 
										(
											select distinct Name from [Photography.Post].[dbo].[PostAttachments] where Name is not null and AttachmentType = 1
											union 
											select distinct Name from [Photography.Order].[dbo].[Attachments] where Name is not null and AttachmentType = 1
										)
									END";

			migrationBuilder.Sql(getNotUsedImages);
			migrationBuilder.Sql(getNotUsedVideos);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
