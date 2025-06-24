using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using NguyenMinhKhai_PRN232_A01_BE.sln.DTOs;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;

namespace NguyenMinhKhai_PRN232_A01_BE.sln.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository newsRepository, IMapper mapper, ILogger<NewsService> logger)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public IQueryable<NewsDTO> GetAll()
        {
            _logger.LogInformation("Getting all news");
            var news = _newsRepository.GetAll().ToList();
            return _mapper.Map<List<NewsDTO>>(news).AsQueryable();
        }

        public async Task<IEnumerable<NewsDTO>> GetActiveNews()
        {
            _logger.LogInformation("Getting active news from repository");
            var activeNews = await _newsRepository.GetActiveNews()
                .Where(n => n.Status == 1)  // Double check status is active
                .ToListAsync();
            _logger.LogInformation($"Found {activeNews.Count} active news articles in repository");
            
            var newsDto = _mapper.Map<IEnumerable<NewsDTO>>(activeNews);
            _logger.LogInformation($"Mapped {newsDto.Count()} news articles to DTOs");
            foreach (var news in newsDto)
            {
                _logger.LogInformation($"Mapped News - ID: {news.NewsId}, Title: {news.Title}, Status: {news.Status}");
            }
            return newsDto;
        }

        public IQueryable<NewsDTO> GetNewsByAccountId(int accountId)
        {
            _logger.LogInformation($"Getting news for account ID: {accountId}");
            var news = _newsRepository.GetNewsByAccountId(accountId).ToList();
            return _mapper.Map<List<NewsDTO>>(news).AsQueryable();
        }

        public IQueryable<NewsDTO> SearchNews(string searchTerm)
        {
            _logger.LogInformation($"Searching news with term: {searchTerm}");
            var news = _newsRepository.SearchNews(searchTerm).ToList();
            return _mapper.Map<List<NewsDTO>>(news).AsQueryable();
        }

        public IQueryable<NewsDTO> GetNewsByDateRange(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation($"Getting news from {startDate} to {endDate}");
            var news = _newsRepository.GetNewsByDateRange(startDate, endDate).ToList();
            return _mapper.Map<List<NewsDTO>>(news).AsQueryable();
        }

        public async Task<NewsDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting news with ID: {id}");
            var news = await _newsRepository.GetByIdAsync(id);
            return news != null ? _mapper.Map<NewsDTO>(news) : null;
        }

        public async Task<NewsDTO?> GetActiveByIdAsync(int id)
        {
            _logger.LogInformation($"Getting active news with ID: {id}");
            var news = await _newsRepository.GetActiveByIdAsync(id);
            return news != null ? _mapper.Map<NewsDTO>(news) : null;
        }

        public async Task<NewsDTO> CreateAsync(CreateNewsDTO newsDto, int accountId)
        {
            _logger.LogInformation($"Creating news for account ID: {accountId}");
            var news = await _newsRepository.CreateAsync(newsDto, accountId);
            return _mapper.Map<NewsDTO>(news);
        }

        public async Task<NewsDTO?> UpdateAsync(int id, UpdateNewsDTO newsDto)
        {
            _logger.LogInformation($"Updating news with ID: {id}");
            var news = await _newsRepository.UpdateAsync(id, newsDto);
            return news != null ? _mapper.Map<NewsDTO>(news) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting news with ID: {id}");
            return await _newsRepository.DeleteAsync(id);
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            _logger.LogInformation($"Incrementing view count for news ID: {id}");
            return await _newsRepository.IncrementViewCountAsync(id);
        }

        public async Task<NewsStatisticsDTO> GetNewsStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation($"Getting news statistics from {startDate} to {endDate}");
            return await _newsRepository.GetNewsStatisticsAsync(startDate, endDate);
        }
    }
} 