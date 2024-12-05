﻿namespace Library.Application.DTOs
{
    public class CreateBookDTO
    {
        public string ISBN { get; }
        public string? Title { get; }
        public string? Description { get; }
        public int Quantity { get; }
        public int? GenreId { get; }
        public int AuthorId { get; }
    }
}
