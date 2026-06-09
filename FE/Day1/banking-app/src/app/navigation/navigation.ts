import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { isLoggedIn, logout } from '../rxjs/auth.operator';

@Component({
  selector: 'app-navigation',
  imports: [RouterLink],
  templateUrl: './navigation.html',
  styleUrl: './navigation.css',
})
export class Navigation {


  logout() {
    logout();
    
  }
}
