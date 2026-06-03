import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Customers } from './customers/customers';
import { Products } from "./products/products";
import { Login } from "./login/login";
import { Account } from "./account/account";
import { usernameSubject } from './rxjs/auth.operator';

@Component({
  selector: 'app-root',
  imports: [Customers, Products, Login, Account],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  username = signal('Guest');

  constructor() {
    usernameSubject.subscribe({
      next:(un)=>{
        this.username.set(un);
      }
    })
  }

  onDestroy(){
    usernameSubject.unsubscribe();
  }

  protected readonly title = signal('banking-app');
}
