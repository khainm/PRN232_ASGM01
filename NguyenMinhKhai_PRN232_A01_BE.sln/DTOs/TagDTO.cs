namespace NguyenMinhKhai_PRN232_A01_BE.sln.DTOs
{
    /// <summary>
    /// Data Transfer Object for displaying tag information
    /// </summary>
    public class TagDTO
    {
        /// <summary>
        /// The unique identifier of the tag
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// The name of the tag
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
} 