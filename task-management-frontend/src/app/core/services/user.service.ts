import { Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';
import { Observable, from, of, throwError } from 'rxjs';
import { catchError, switchMap, tap, finalize } from 'rxjs/operators';
import { User } from '../models/user.model';
import { SkipLoading } from '../interceptors/loading.interceptor';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly backendUrl = 'http://localhost:5000/api/users';

  private _users = signal<User[]>([]);
  private _loading = signal(false);
  private _error = signal<string | null>(null);

  readonly users = this._users.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly hasUsers = computed(() => this._users().length > 0);

  constructor(
    private http: HttpClient
  ) { }

  loadUsers(silent = false): void {
    if (!silent) {
      this._loading.set(true);
    }
    this._error.set(null);

    this.http.get<User[]>(this.backendUrl, {
      context: new HttpContext().set(SkipLoading, silent)
    }).pipe(
      catchError(err => {
        this._error.set('No se pudieron cargar los usuarios.');
        return throwError(() => err);
      }),
      finalize(() => {
        if (!silent) {
          this._loading.set(false);
        }
      })
    ).subscribe({
      next: (users) => this._users.set(users),
      error: () => { }
    });
  }

  createUser(name: string, email: string): Observable<User> {
    return this.http.post<User>(this.backendUrl, { name, email }, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(user => {
        this._users.update(list => [...list, user]);
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'Error al crear el usuario.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }

  updateUser(id: string, name: string): Observable<User> {
    return this.http.put<User>(`${this.backendUrl}/${id}`, { name }, {
      context: new HttpContext().set(SkipLoading, true)
    }).pipe(
      tap(updatedUser => {
        this._users.update(list => list.map(u => u.id === id ? updatedUser : u));
      }),
      catchError(err => {
        const msg = err?.error?.error ?? 'Error al actualizar el usuario.';
        this._error.set(msg);
        return throwError(() => err);
      })
    );
  }
}
