import { Component } from '@angular/core';
import { BankingApiService } from '../services/bankingapi.service';
import { debounceTime, distinctUntilChanged, Observable, of, Subject, switchMap } from 'rxjs';
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
    this.searchSubject.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(
        accNumber=>{
          if(accNumber.trim() === '')
           return of({}); // Return an observable that emits null if the input is empty 
          return this.bankingApiService.getAccountDetails(accNumber);
        })
      ).subscribe({
      next: (response:any) => {
        console.log("Account details", response);
        
      },
      error: (error) => {
        console.error("Failed to fetch account details", error);
        
      }
    }
    )
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


