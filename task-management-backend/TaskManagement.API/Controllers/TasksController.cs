using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpPost]

    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskService.CreateAsync(dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return CreatedAtAction(nameof(GetTasks), new { id = result.Value!.Id }, result.Value);
    }

    [HttpGet]

    public async Task<IActionResult> GetTasks([FromQuery] TaskStatus? status = null)
    {
        var result = await _taskService.GetAllAsync(status);
        return Ok(result.Value);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskService.UpdateTaskAsync(id, dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/status")]

    public async Task<IActionResult> ChangeTaskStatus([FromRoute] Guid id, [FromBody] ChangeTaskStatusDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _taskService.ChangeStatusAsync(id, dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return NoContent();
    }

    [HttpGet("by-priority")]
  
    public async Task<IActionResult> GetTasksByPriority([FromQuery] string priority)
    {
        var result = await _taskService.GetByJsonPriorityAsync(priority);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpPatch("{id:guid}/additional-info")]

    public async Task<IActionResult> UpdateAdditionalInfo([FromRoute] Guid id, [FromBody] UpdateAdditionalInfoDto dto)
    {
        var result = await _taskService.UpdateAdditionalInfoAsync(id, dto);

        if (!result.IsSuccess)
            return StatusCode(result.ErrorStatusCode, new { error = result.Error });

        return Ok(result.Value);
    }
}
