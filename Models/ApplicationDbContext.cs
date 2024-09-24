using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrsaDemoModels.Entity;

namespace OrsaDemoWebApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Personnels> Personnels { get; set; }
        public DbSet<Geography> Geography { get; set; }
        public DbSet<MediaLibrary> MediaLibrary { get; set; }
        public DbSet<PersonnelMedia> PersonnelMedia { get; set; }
        public DbSet<Institution> Institution { get; set; }
        public DbSet<ParamInstitution> ParamInstitution { get; set; }
        public DbSet<PersonnelInstitutionMediaRelation> PersonnelInstitutionMediaRelation { get; set; }

    }


}
