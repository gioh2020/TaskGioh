import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user.service';
import { UserFormComponent } from '../user-form/user-form.component';
import { Router } from '@angular/router';
import { TaskService } from '../../../core/services/task.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, UserFormComponent],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  showForm = signal(false);
  editingUser = signal<User | null>(null);

  constructor(
    readonly userService: UserService,
    private taskService: TaskService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userService.loadUsers();
  }

  onUserSaved(): void {
    this.showForm.set(false);
    this.editingUser.set(null);
    this.userService.loadUsers(true);
  }

  onCancelForm(): void {
    this.showForm.set(false);
    this.editingUser.set(null);
  }

  openNewUserForm(): void {
    if (this.showForm() && !this.editingUser()) {
      this.showForm.set(false);
    } else {
      this.editingUser.set(null);
      this.showForm.set(true);
    }
  }

  editUser(user: User, event: Event): void {
    event.stopPropagation();
    this.editingUser.set(user);
    this.showForm.set(true);
  }

  viewUserTasks(userId: string, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }
    this.taskService.setUserFilter(userId);
    this.router.navigate(['/tasks']);
  }
}
