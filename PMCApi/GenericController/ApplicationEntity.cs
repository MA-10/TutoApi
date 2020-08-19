using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Test.Services
{
    public interface IApplicationEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        Guid Id { get; set; }

        DateTime CreatedOn { get; set; }

        Guid CreatedBy { get; set; }

        DateTime ModifiedOn { get; set; }

        Guid ModifiedBy { get; set; }

        //byte[] RowVersion { get; set; }

        //bool IsDeleted { get; set; }
    }

    public abstract class ApplicationEntity : IApplicationEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime ModifiedOn { get; set; }

        public Guid ModifiedBy { get; set; }

        
    }
}
