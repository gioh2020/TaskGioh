import { Component, input } from '@angular/core';
import { TaskStatus } from '../../../core/models/task.model';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.css'
})
export class StatusBadgeComponent {
  status = input.required<TaskStatus>();

  badgeClass() {
    const map: Record<TaskStatus, string> = {
      Pending: 'badge badge-pending',
      InProgress: 'badge badge-progress',
      Done: 'badge badge-done',
    };
    return map[this.status()];
  }

  label() {
    const map: Record<TaskStatus, string> = {
      Pending: 'Pendiente',
      InProgress: 'En Progreso',
      Done: 'Completada',
    };
    return map[this.status()];
  }
}
