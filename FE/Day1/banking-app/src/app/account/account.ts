import { Component } from '@angular/core';
import { BankingApiService } from '../services/bankingapi.service';

@Component({
  selector: 'app-account',
  imports: [],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {
  
  constructor(private bankingApiService: BankingApiService) {
  }

  getAccountDetails(accNumber:string){
    this.bankingApiService.getAccountDetails(accNumber).subscribe({
      next: (response) => {
        console.log("Account details", response);
        alert("Account details fetched successfully!")
      },
      error: (error) => {
        console.error("Failed to fetch account details", error);
        alert("Failed to fetch account details. Please try again.");
      }
    });
  }
}
