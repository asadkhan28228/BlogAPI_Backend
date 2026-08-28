using BlogApi.BLL.DTOs.Tag;
using BlogApi.BLL.Interfaces;
using BlogApi.DAL.Entities;
using BlogApi.DAL.InterfacesRepositories;

namespace BlogApi.BLL.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        // =========================
        // GET ALL TAGS
        // =========================
        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();

            return tags.Select(MapToDto);
        }

        // =========================
        // GET TAG BY ID
        // =========================
        public async Task<TagDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return null;
            }

            return MapToDto(tag);
        }

        // =========================
        // CREATE TAG
        // =========================
        public async Task<TagDto> CreateAsync(
            CreateTagDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(
                    nameof(dto));
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Tag name is required.");
            }

            var name = dto.Name.Trim();

            var existingTag =
                await _tagRepository.GetByNameAsync(name);

            if (existingTag != null)
            {
                throw new ArgumentException(
                    "Tag already exists.");
            }

            var tag = new Tag
            {
                Name = name
            };

            var createdTag =
                await _tagRepository.AddAsync(tag);

            return MapToDto(createdTag);
        }

        // =========================
        // UPDATE TAG
        // =========================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateTagDto dto)
        {
            if (id <= 0)
            {
                return false;
            }

            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Name))
            {
                return false;
            }

            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return false;
            }

            var name = dto.Name.Trim();

            var existingTag =
                await _tagRepository.GetByNameAsync(name);

            if (existingTag != null &&
                existingTag.Id != id)
            {
                return false;
            }

            tag.Name = name;

            await _tagRepository.UpdateAsync(tag);

            return true;
        }

        // =========================
        // DELETE TAG
        // =========================
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                return false;
            }

            await _tagRepository.DeleteAsync(id);

            return true;
        }

        // =========================
        // MAP ENTITY TO DTO
        // =========================
        private static TagDto MapToDto(Tag tag)
        {
            return new TagDto
            {
                Id = tag.Id,

                Name = tag.Name
            };
        }
    }
}