using AutoMapper;
using Microsoft.EntityFrameworkCore;
using JapaneseTrainer.Api.Data;
using JapaneseTrainer.Api.DTOs.AI;
using JapaneseTrainer.Api.Models;
using JapaneseTrainer.Api.Models.Enums;

namespace JapaneseTrainer.Api.Services
{
    public class AIQueueService : IAIQueueService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AIQueueService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AIJobDto> CreateJobAsync(AIJobRequest request, CancellationToken cancellationToken = default)
        {
            var job = new AIQueue
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Payload = request.Payload,
                Status = AIJobStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AIQueues.AddAsync(job, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AIJobDto>(job);
        }

        public async Task<AIJobDto?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var job = await _context.AIQueues.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
            return job == null ? null : _mapper.Map<AIJobDto>(job);
        }

        public async Task<List<AIJobDto>> GetJobsAsync(AIJobType? type, AIJobStatus? status, CancellationToken cancellationToken = default)
        {
            var query = _context.AIQueues.AsQueryable();

            if (type.HasValue)
            {
                query = query.Where(j => j.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(j => j.Status == status.Value);
            }

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<AIJobDto>>(jobs);
        }

        public async Task<AIJobDto?> ProcessJobAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var job = await _context.AIQueues.FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
            if (job == null)
            {
                return null;
            }

            // Mock processing: simulate AI work
            job.Status = AIJobStatus.Processing;
            job.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            // Simulate delay (in real app, this would be async background processing)
            await Task.Delay(100, cancellationToken);

            // Mock result based on type
            var mockResult = job.Type switch
            {
                AIJobType.OCR => "{\"text\": \"Mock OCR result\", \"confidence\": 0.95}",
                AIJobType.QUIZ_GEN => "{\"questions\": [{\"question\": \"Mock question\", \"options\": [\"A\", \"B\", \"C\"], \"answer\": \"A\"}]}",
                AIJobType.GRAMMAR_EXPLAIN => "{\"explanation\": \"Mock grammar explanation\", \"examples\": []}",
                AIJobType.EXERCISE_GEN => "{\"exercises\": [{\"type\": \"CLOZE\", \"prompt\": \"Mock exercise\"}]}",
                AIJobType.TRANSLATION => "{\"translation\": \"Mock translation\", \"source\": \"ja\", \"target\": \"en\"}",
                AIJobType.AUDIO_GEN => "{\"audioUrl\": \"mock://audio.mp3\", \"duration\": 5.0}",
                _ => "{\"result\": \"Mock result\"}"
            };

            job.Status = AIJobStatus.Completed;
            job.Result = mockResult;
            job.ProcessedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<AIJobDto>(job);
        }
    }
}

