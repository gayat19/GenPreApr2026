import { Component } from '@angular/core';
import { BankingApiService } from '../services/bankingapi.service';
import { Subject } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-account',
  imports: [FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {
  
   searchAccountNumber: string = '';

  private searchSubject = new Subject<string>();
  
  constructor(private bankingApiService: BankingApiService) {
    this.searchSubject.subscribe({
      next:(accNumber) => {
        console.log("Fetching account details for account number:", accNumber);
      },
      error: (error) => {
        console.error("Failed to fetch account details", error);
        
      },
      complete: () => {        
        console.log("Account details fetched successfully!");
      
      }
    });
  }

  // getAccountDetails(accNumber:string){
  //   this.bankingApiService.getAccountDetails(accNumber).subscribe({
  //     next: (response) => {
  //       console.log("Account details", response);
  //       alert("Account details fetched successfully!")
  //     },
  //     error: (error) => {
  //       console.error("Failed to fetch account details", error);
  //       alert("Failed to fetch account details. Please try again.");
  //     }
  //   });
  // }



    getAccountDetails(){
      this.searchSubject.next(this.searchAccountNumber);
  }

  onDestroy(){
    this.searchSubject.complete();
    this.searchSubject.unsubscribe();
  }
}
