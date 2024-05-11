using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.DTOs;
using task_timer.Models;
using task_timer.Repositories;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public CategoriesController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager,
                                IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoryDTO>> GetByUserId()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        var categories = _unitOfWork.CategoriesRepository.GetByUserId(userId);
        var categoriesDTO = _mapper.Map<List<CategoryDTO>>(categories);

        return Ok(categoriesDTO);

    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public ActionResult<CategoryDTO> Get(int id)
    {
        var userId = _userManager.GetUserId(User);
        var category = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);
        
        if (category == null)
        {
            return NotFound("The category doesn't exist.");
        }

        var categoryDTO = _mapper.Map<CategoryDTO>(category);

        return Ok(categoryDTO);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CategoryDTO categoryDTO)
    {
        if (categoryDTO is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbCategory = _unitOfWork.CategoriesRepository.Get(c => c.Name == categoryDTO.Name);
       
        if (dbCategory != null)
        {
            return BadRequest("This category already exists.");
        }

        var userId = _userManager.GetUserId(User);
        var category = _mapper.Map<Category>(categoryDTO);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        category.AspNetUsersId = userId;

        _unitOfWork.CategoriesRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        return Ok($"Created:\nName: {categoryDTO.Name}\nDescription: {categoryDTO.Description}");

    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Put([FromRoute] int id, [FromBody] CategoryDTO category)
    {
        if (category is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbCategoryDTO = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);

        if (dbCategoryDTO is null)
        {
            return BadRequest($"Could not find category {id}");
        }

        dbCategoryDTO.Name = category.Name;
        dbCategoryDTO.Description = category.Description;

        var dbCategory = _mapper.Map<Category>(dbCategoryDTO);

        dbCategory.AspNetUsersId = dbCategory.AspNetUsersId;

        _unitOfWork.CategoriesRepository.UpdateAsync(dbCategory);
        await _unitOfWork.CommitAsync();

        return Ok($"{dbCategory.Name} has been updated.");
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var category = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);

        if (category is null)
        {
            return NotFound("The category you're trying to delete doesn't exist.");
        }

        if (category.appTasks.Any())
        {
            return BadRequest($"{category.Name} cannot be deleted because it's associated with existing tasks.");
        }

        _unitOfWork.CategoriesRepository.DeleteAsync(category);
        await _unitOfWork.CommitAsync();

        return Ok($"{category.Name} was successfully deleted.");

    }
}
