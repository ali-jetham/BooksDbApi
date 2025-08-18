using LifeDbApi.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LifeDbApi.Data;

public class LifeDbContext : DbContext
{
	public LifeDbContext(DbContextOptions<LifeDbContext> options)
		: base(options) { }

	public DbSet<User> Users { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<Collection> Collections { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<UserBook> UserBooks { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(u => u.Id);
			entity.HasIndex(u => u.Email).IsUnique();

			entity.Property(u => u.Id).HasColumnType("uuid");
			entity.Property(u => u.Name).HasColumnType("varchar(128)");
			entity.Property(u => u.Email).HasColumnType("varchar(256)");
			entity.Property(u => u.OAuthProvider).HasColumnType("oauth_provider");
			entity.Property(u => u.Sub).HasColumnType("varchar(256)");
		});

		modelBuilder.Entity<Collection>(entity =>
		{
			entity.Property(u => u.Name).HasColumnType("varchar(64)");
			entity.Property(u => u.Description).HasColumnType("varchar(300)");
			entity.Property(u => u.Data).HasColumnType("jsonb");
		});

		modelBuilder.Entity<Book>(book =>
		{
			book.Property(b => b.Source).HasColumnType("book_source");
			book.Property(b => b.Isbn10).HasColumnType("varchar(10)");
			book.Property(b => b.Isbn13).HasColumnType("varchar(13)");
			book.Property(b => b.Edition).HasColumnType("smallint");
		});
	}
}
