import { Component, signal } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BankingApiService } from '../services/bankingapi.service';
import { changeUsername } from '../rxjs/auth.operator';
import { form, minLength, required, FormField } from '@angular/forms/signals';


@Component({
  selector: 'app-login',
  imports: [FormsModule, ReactiveFormsModule, FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginModel = signal(new LoginModel());
  progress = signal(false);
  constructor(private bankingApiService: BankingApiService) {
  }
  loginForm = form(this.loginModel,(path)=>{
    required(path.username);
    required(path.password);
    minLength(path.username, 3);
  });
  handleLoginClick(){
    console.log("Login button clicked", this.loginForm.username().errors()[0]);
    // this.progress.set(true);
    // this.bankingApiService.loginApiCall(this.loginModel()).subscribe({
    //   next: (response:any) => {
    //     console.log("Login successful", response);
    //     sessionStorage.setItem('token', response.token);
    //     alert("Login successful!")
    //     this.progress.set(false);
    //     changeUsername();
    //   },
    //   error: (error) => {
    //     console.error("Login failed", error);
    //     alert("Login failed. Please try again.");
    //     this.progress.set(false);
    //   }
    // });
    
  }

}
