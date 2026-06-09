import { Component, signal } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BankingApiService } from '../services/bankingapi.service';
import { changeUsername } from '../rxjs/auth.operator';
import { form, minLength, required, FormField } from '@angular/forms/signals';
import { Router } from '@angular/router';


@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule, FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginModel = signal(new LoginModel());
  progress = signal(false);
  constructor(private bankingApiService: BankingApiService,
              private router:Router
  ) {
  }
  loginForm = form(this.loginModel,(path)=>{
    required(path.username, {message:"Username is required"});
    minLength(path.username, 4, {message:"Username must be at least 4 characters long"});
    required(path.password, {message:"Password is required"});
   
  });
  handleLoginClick(){
    if(this.loginForm().invalid()){
      alert("Please fix the errors in the form before submitting.");
      return;
    }

    this.progress.set(true);
    this.bankingApiService.loginApiCall(this.loginModel()).subscribe({
      next: (response:any) => {
        console.log("Login successful", response);
        sessionStorage.setItem('token', response.token);
        alert("Login successful!")
        this.progress.set(false);
        changeUsername();
        this.router.navigate(['/home']);
      },
      error: (error) => {
        console.error("Login failed", error);
        alert("Login failed. Please try again.");
        this.progress.set(false);
      }
    });
    
  }

}
