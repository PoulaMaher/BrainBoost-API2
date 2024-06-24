using AutoMapper;
using BrainBoost_API.DTOs.Category;
using BrainBoost_API.Models;
using BrainBoost_API.Repositories.Inplementation;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryRepository CategoryRepository;
        private readonly IMapper mapper;
        public CategoryController(ICategoryRepository CategoryRepository, IMapper mapper)
        {
            this.CategoryRepository = CategoryRepository;
            this.mapper = mapper;
        }
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            if (ModelState.IsValid)
            {
                List<Category> categories = CategoryRepository.GetAll().ToList();
                List<CategoryDTO> categoriesdata = new List<CategoryDTO>();
                foreach (Category category in categories)
                {
                    CategoryDTO categorydata = mapper.Map<CategoryDTO>(category);
                    categoriesdata.Add(categorydata);
                }
                return Ok(categoriesdata);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetCategoryById/{id}")]
        public IActionResult GetCategoryById(int id)
        {
            Category category = CategoryRepository.Get(a => a.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            CategoryDTO categoryData = new CategoryDTO();
            categoryData = mapper.Map<CategoryDTO>(category);
            return Ok(categoryData);
        }

        [HttpPost("addategory")]
        public async Task<IActionResult> addategory(CategoryDTO newCategory)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category();
                category = mapper.Map<Category>(newCategory);
                CategoryRepository.add(category);
                CategoryRepository.save();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            Category category = CategoryRepository.Get(a => a.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            category.IsDeleted = true;
            CategoryRepository.remove(category);
            CategoryRepository.save();
            return Ok("Category Deleted Successfully!");
        }
        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory(CategoryDTO updatedCategory)
        {
            if (ModelState.IsValid)
            {
                Category category = mapper.Map<Category>(updatedCategory);
                CategoryRepository.update(category);
                CategoryRepository.save();
                return Ok("Successfully Updated");
            }
            return BadRequest(ModelState);
        }

    }
}
