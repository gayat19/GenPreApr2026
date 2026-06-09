import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { usernameSubject } from './rxjs/auth.operator';
import { Navigation } from "./navigation/navigation";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navigation],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  username = signal('Guest');

  constructor() {
    usernameSubject.subscribe({
      next:(un)=>{
        this.username.set(un?un:'Guest');
      }
    })
  }

  onDestroy(){
    usernameSubject.unsubscribe();
  }

  protected readonly title = signal('banking-app');
}
