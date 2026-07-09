import { Component, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <div style="font-family: sans-serif; padding: 2rem;">
      <h1>Kubernetes 3-Tier Demo</h1>
      <button (click)="checkDb()">Check Database Connection</button>
      <pre>{{ result() }}</pre>
    </div>
  `,
})
export class App {
  result = signal('Click the button to test the API -> Postgres connection');

  constructor(private http: HttpClient) {}

  checkDb() {
    this.http.get('/api/db-check').subscribe({
      next: (res) => this.result.set(JSON.stringify(res, null, 2)),
      error: (err) => this.result.set('Error: ' + JSON.stringify(err.message)),
    });
  }
}
