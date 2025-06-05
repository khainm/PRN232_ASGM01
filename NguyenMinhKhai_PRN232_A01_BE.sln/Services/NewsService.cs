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
            var news = _newsRepository.GetAll();
            return _mapper.ProjectTo<NewsDTO>(news);
        }

        public IQueryable<NewsDTO> GetActiveNews()
        {
            _logger.LogInformation("Getting active news");
            var news = _newsRepository.GetActiveNews();
            return _mapper.ProjectTo<NewsDTO>(news);
        }

        public IQueryable<NewsDTO> GetNewsByAccountId(int accountId)
        {
            _logger.LogInformation($"Getting news for account ID: {accountId}");
            var news = _newsRepository.GetNewsByAccountId(accountId);
            return _mapper.ProjectTo<NewsDTO>(news);
        }

        public IQueryable<NewsDTO> SearchNews(string searchTerm)
        {
            _logger.LogInformation($"Searching news with term: {searchTerm}");
            var news = _newsRepository.SearchNews(searchTerm);
            return _mapper.ProjectTo<NewsDTO>(news);
        }

        public IQueryable<NewsDTO> GetNewsByDateRange(DateTime startDate, DateTime endDate)
        {
            _logger.LogInformation($"Getting news from {startDate} to {endDate}");
            var news = _newsRepository.GetNewsByDateRange(startDate, endDate);
            return _mapper.ProjectTo<NewsDTO>(news);
        }

        public async Task<NewsDTO?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Getting news with ID: {id}");
            var news = await _newsRepository.GetByIdAsync(id);
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