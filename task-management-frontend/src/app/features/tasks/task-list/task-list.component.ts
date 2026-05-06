import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../core/services/task.service';
import { Task, TaskStatus } from '../../../core/models/task.model';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { TaskFormComponent } from '../task-form/task-form.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, StatusBadgeComponent, TaskFormComponent],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  showForm = signal(false);
  editingTask = signal<Task | null>(null);
  changingStatus: string | null = null;
  editingDateTaskId: string | null = null;

  constructor(readonly taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.loadTasks();
  }

  applyFilter(status: TaskStatus | null): void {
    this.taskService.setFilter(status);
  }

  changeStatus(taskId: string, newStatus: TaskStatus): void {
    this.changingStatus = taskId;
    this.taskService.changeStatus(taskId, newStatus).subscribe({
      next: () => {
        this.changingStatus = null;
      },
      error: () => {
        this.changingStatus = null;
      }
    });
  }

  startDateEdit(taskId: string, event: Event): void {
    event.stopPropagation();
    this.editingDateTaskId = taskId;
    // Auto-focus the input after render
    setTimeout(() => {
      const input = document.querySelector('.date-inline-input') as HTMLInputElement;
      if (input) {
        input.showPicker?.();
        input.focus();
      }
    }, 50);
  }

  cancelDateEdit(): void {
    // Small delay so (change) fires before (blur)
    setTimeout(() => {
      this.editingDateTaskId = null;
    }, 200);
  }

  saveEstimatedDate(taskId: string, event: Event): void {
    const input = event.target as HTMLInputElement;
    const newDate = input.value || null;
    this.editingDateTaskId = null;

    this.taskService.updateEstimatedEndDate(taskId, newDate).subscribe({
      error: () => { /* error already handled in service */ }
    });
  }

  openNewTaskForm(): void {
    if (this.showForm() && !this.editingTask()) {
      this.showForm.set(false);
    } else {
      this.editingTask.set(null);
      this.showForm.set(true);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  editTask(task: Task): void {
    this.editingTask.set(task);
    this.showForm.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onCancelForm(): void {
    this.showForm.set(false);
    this.editingTask.set(null);
  }

  onTaskSaved(): void {
    this.showForm.set(false);
    this.editingTask.set(null);
    this.taskService.loadTasks(undefined, true);
  }

  refresh(): void {
    this.taskService.loadTasks();
  }

  parseAdditionalInfo(jsonStr: string | undefined): any {
    if (!jsonStr) return null;
    try {
      return JSON.parse(jsonStr);
    } catch {
      return null;
    }
  }
}
