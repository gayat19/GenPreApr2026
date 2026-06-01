import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Customers } from './customers/customers';

@Component({
  selector: 'app-root',
  imports: [ Customers],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('banking-app');
}
