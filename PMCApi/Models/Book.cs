using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Test.Services;

namespace Test.Models
{
    [ApiEntityAttribute]
    public class Book : ApplicationEntity
    {
        public string Title { get; set; }

        public string Author { get; set; }
    }
}