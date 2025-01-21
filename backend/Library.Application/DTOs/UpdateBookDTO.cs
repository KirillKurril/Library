﻿namespace Library.Application.DTOs
{
    public class UpdateBookDTO
    {
        public Guid Id { get; set; }
        public string? ISBN { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? Quantity { get; set; }
        public Guid? GenreId { get; set; }
        public Guid? AuthorId { get; set; }
    }
}
