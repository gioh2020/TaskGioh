import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private _loadingCount = signal(0);
  readonly isLoading = signal(false);
  private minDuration = 500; // ms
  private showTime = 0;

  show() {
    if (this._loadingCount() === 0) {
      this.showTime = Date.now();
      this.isLoading.set(true);
    }
    this._loadingCount.update(count => count + 1);
  }

  hide() {
    this._loadingCount.update(count => Math.max(0, count - 1));
    if (this._loadingCount() === 0) {
      const elapsedTime = Date.now() - this.showTime;
      const remainingTime = Math.max(0, this.minDuration - elapsedTime);
      
      setTimeout(() => {
        if (this._loadingCount() === 0) {
          this.isLoading.set(false);
        }
      }, remainingTime);
    }
  }
}
