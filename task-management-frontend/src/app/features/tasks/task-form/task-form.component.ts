import { Component, OnInit, output, input, effect } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { CreateTaskRequest, Task } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})
export class TaskFormComponent implements OnInit {
  readonly taskToEdit = input<Task | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  taskForm!: FormGroup;
  submitting = false;
  successMsg = '';

  constructor(
    private fb: FormBuilder,
    readonly taskService: TaskService,
    readonly userService: UserService
  ) {
    effect(() => {
      const task = this.taskToEdit();
      if (task && this.taskForm) {
        this.taskForm.patchValue({
          title: task.title,
          description: task.description || ''
        });
        
        // Disable other controls so they aren't validated or sent
        this.taskForm.get('assignedUserId')?.disable();
        this.taskForm.get('priority')?.disable();
        this.taskForm.get('estimatedEndDate')?.disable();
        this.taskForm.get('tags')?.disable();
      } else if (this.taskForm) {
        this.taskForm.get('assignedUserId')?.enable();
        this.taskForm.get('priority')?.enable();
        this.taskForm.get('estimatedEndDate')?.enable();
        this.taskForm.get('tags')?.enable();
      }
    });
  }

  ngOnInit(): void {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      assignedUserId: ['', Validators.required],
      priority: [''],
      estimatedEndDate: [''],
      tags: ['']
    });

    if (!this.taskToEdit()) {
      this.userService.loadUsers();
    }
    
    // In case effect doesn't fire immediately
    const task = this.taskToEdit();
    if (task) {
        this.taskForm.patchValue({
          title: task.title,
          description: task.description || ''
        });
        this.taskForm.get('assignedUserId')?.disable();
        this.taskForm.get('priority')?.disable();
        this.taskForm.get('estimatedEndDate')?.disable();
        this.taskForm.get('tags')?.disable();
    }
  }

  isInvalid(field: string): boolean {
    const control = this.taskForm.get(field);
    return !!(control?.invalid && control?.touched);
  }

  onSubmit(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.successMsg = '';
    this.taskService.clearError();

    const task = this.taskToEdit();

    if (task) {
      const title = this.taskForm.get('title')?.value;
      const description = this.taskForm.get('description')?.value;
      
      this.taskService.updateTask(task.id, title, description).subscribe({
        next: (result) => {
          this.submitting = false;
          this.successMsg = `Tarea "${result.title}" actualizada exitosamente.`;
          setTimeout(() => this.saved.emit(), 1500);
        },
        error: () => {
          this.submitting = false;
        }
      });
    } else {
      const { title, description, assignedUserId, priority, estimatedEndDate, tags } = this.taskForm.value;

      let additionalInfoObj: any = {};
      if (priority) additionalInfoObj.priority = priority;
      if (estimatedEndDate) additionalInfoObj.estimatedEndDate = estimatedEndDate;
      if (tags) {
        additionalInfoObj.tags = tags.split(',').map((t: string) => t.trim()).filter((t: string) => t.length > 0);
      }

      let additionalInfoStr: string | undefined = undefined;
      if (Object.keys(additionalInfoObj).length > 0) {
        additionalInfoStr = JSON.stringify(additionalInfoObj);
      }

      const request: CreateTaskRequest = {
        title,
        description: description || undefined,
        assignedUserId,
        additionalInfo: additionalInfoStr,
      };

      this.taskService.createTask(request).subscribe({
        next: (result) => {
          this.submitting = false;
          this.successMsg = `Tarea "${result.title}" creada exitosamente.`;
          this.taskForm.reset();
          setTimeout(() => this.saved.emit(), 1500);
        },
        error: () => {
          this.submitting = false;
        }
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
