import { Routes } from '@angular/router';

export const tasksRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./task-list/task-list.component').then(m => m.TaskListComponent),
  },
];
