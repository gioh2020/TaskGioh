import { Component, OnInit, output, input, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.css'
})
export class UserFormComponent implements OnInit {
  readonly userToEdit = input<User | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  userForm!: FormGroup;
  submitting = false;
  successMsg = '';

  constructor(
    private fb: FormBuilder,
    readonly userService: UserService
  ) {
    effect(() => {
      const user = this.userToEdit();
      if (user && this.userForm) {
        this.userForm.patchValue({
          name: user.name,
          email: user.email
        });
        this.userForm.get('email')?.disable();
      } else if (this.userForm) {
        this.userForm.get('email')?.enable();
      }
    });
  }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });


    const user = this.userToEdit();
    if (user) {
      this.userForm.patchValue({
        name: user.name,
        email: user.email
      });
    }
  }

  isInvalid(field: string): boolean {
    const control = this.userForm.get(field);
    return !!(control?.invalid && control?.touched);
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.successMsg = '';
    const { name, email } = this.userForm.getRawValue();
    const user = this.userToEdit();

    if (user) {
      this.userService.updateUser(user.id, name).subscribe({
        next: () => {
          this.submitting = false;
          this.successMsg = `Usuario "${name}" actualizado exitosamente.`;
          setTimeout(() => this.saved.emit(), 1500);
        },
        error: () => {
          this.submitting = false;
        }
      });
    } else {
      this.userService.createUser(name, email).subscribe({
        next: () => {
          this.submitting = false;
          this.successMsg = `Usuario "${name}" creado exitosamente.`;
          this.userForm.reset();
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
