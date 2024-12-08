﻿namespace Library.Application.DTOs
{
    public class BookDetailsDTO : BookCatalogDTO
    {
        public bool IsAvalible { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public string ISBN { get; set; }
    }
}