using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<IEnumerable<Tag>> SearchAsync(string term)
        {
            return await _context.Tags
                .Where(t => t.Name.Contains(term))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> UpdateAsync(int id, string name)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return null;

            tag.Name = name;
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
                return false;

            var isUsed = await _context.NewsArticles
                .AnyAsync(n => n.Tags.Any(t => t.TagId == id));

            if (isUsed)
                return false;

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 