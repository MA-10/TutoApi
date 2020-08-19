using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Test.Services;

namespace Test.Models
{
    [ApiEntityAttribute]

    public class Customer : ApplicationEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}