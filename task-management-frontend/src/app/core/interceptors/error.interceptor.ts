import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'Error desconocido.';

      if (error.status === 0) {
        message = 'No se puede conectar con el servidor. Verifique que la API está corriendo.';
      } else if (error.status === 400) {
        message = error.error?.error ?? 'Solicitud incorrecta.';
      } else if (error.status === 404) {
        message = error.error?.error ?? 'Recurso no encontrado.';
      } else if (error.status === 409) {
        message = error.error?.error ?? 'Conflicto de datos.';
      } else if (error.status === 422) {
        message = error.error?.error ?? 'Operación no permitida.';
      } else if (error.status >= 500) {
        message = 'Error interno del servidor.';
      }

      return throwError(() => ({ ...error, userMessage: message }));
    })
  );
};
