import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpParams, HttpContext } from '@angular/common/http';
import { Observable, from, of, throwError } from 'rxjs';
import { catchError, switchMap, tap, finalize } from 'rxjs/operators';
import { Task, CreateTaskRequest, ChangeStatusRequest, UpdateAdditionalInfoRequest, TaskStatus } from '../models/task.model';
import { SkipLoading } from '../interceptors/loading.interceptor';



@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly apiUrl = 'http://localhost:5000/api/tasks';

  private _tasks = signal<Task[]>([]);
  private _loading = signal(false);
  private _error = signal<string | null>(null);
  private _activeFilter = signal<TaskStatus | null>(null);
  private _activeUserFilter = signal<string | null>(null);

  readonly tasks = this._tasks.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly activeFilter = this._activeFilter.asReadonly();
  readonly activeUserFilter = this._activeUserFilter.asReadonly();

  readonly filteredTasks = computed(() => {
    let result = this._tasks();
    const statusFilter = this._activeFilter();
    const userFilter = this._activeUserFilter();

    if (statusFilter) {
      result = result.filter(t => t.status === statusFilter);
    }
    if (userFilter) {
      result = result.filter(t => t.assignedUserId === userFilter);
    }

    return [...result].sort((a, b) =>
      new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
    );
  });

  readonly tasksByStatus = computed(() => ({
    pending: this._tasks().filter(t => t.status === 'Pending').length,
    inProgress: this._tasks().filter(t => t.status === 'InProgress').length,
    done: this._tasks().filter(t => t.status === 'Done').length,
  }));

  constructor(private http: HttpClient) { }

  setFilter(status: TaskStatus | null): void {
    this._activeFilter.set(status);
  }

  setUserFilter(userId: string | null): void {
    this._activeUserFilter.set(userId);
  }

  loadTasks(status?: TaskStatus, silent = false): void {
    if (!silent) {
      this._loading.set(true);
    }
    this._error.set(null);

    let params = new HttpParams();
    if (status) params = params.set('status', status);

    this.http.get<Task[]>(this.apiUrl, {
      params,
      context: new HttpContext().set(SkipLoading, silent)
    }).pipe(
      catchError(err => {
        this._error.set('No se pudieron cargar las tareas.');
        return throwError(() => err);
      }),
      finalize(() => {
        if (!silent) {
          this._loading.set(false);
        }
      })
    ).subscribe({
      next: (tasks) => this._tasks.set(tasks),
      error: () => { }
    });
  }

  createTask(request: CreateTaskRequest): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, request, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(task => {
        this._tasks.update(list => [...list, task]);
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'Error al crear la tarea.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }

  updateTask(id: string, title: string, description?: string): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, { title, description }, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(updatedTask => {
        this._tasks.update(list => list.map(t => t.id === id ? updatedTask : t));
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'Error al actualizar la tarea.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }

  changeStatus(taskId: string, newStatus: TaskStatus): Observable<void> {
    const body: any = { newStatus };
    return this.http.put<void>(`${this.apiUrl}/${taskId}/status`, body, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(() => {
        this._tasks.update(list =>
          list.map(t => t.id === taskId ? { ...t, status: newStatus } : t)
        );
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'No se pudo cambiar el estado.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }

  updateEstimatedEndDate(taskId: string, estimatedEndDate: string | null): Observable<Task> {
    const body: UpdateAdditionalInfoRequest = { estimatedEndDate };
    return this.http.patch<Task>(`${this.apiUrl}/${taskId}/additional-info`, body, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(updatedTask => {
        this._tasks.update(list => list.map(t => t.id === taskId ? updatedTask : t));
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'No se pudo actualizar la fecha estimada.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }

  clearError(): void {
    this._error.set(null);
  }
}
