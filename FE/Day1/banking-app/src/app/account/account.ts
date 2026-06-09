import { Component } from '@angular/core';
import { BankingApiService } from '../services/bankingapi.service';
import { debounceTime, distinctUntilChanged,  of, Subject, switchMap } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { Router, RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-account',
  imports: [FormsModule, RouterOutlet],
  templateUrl: './account.html',
  styleUrl: './account.css',
})
export class Account {
  
   searchAccountNumber: string = '';
    accountDetails: any = null;

  private searchSubject = new Subject<string>();
  
  constructor(private bankingApiService: BankingApiService, private router: Router) {
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
        this.accountDetails = response;
        
      },
      error: (error) => {
        console.error("Failed to fetch account details", error);
        
      }
    }
    )
  }

  handleSendMoneyClick(){
    // this.router.navigate(['/account/transaction/'+this.accountDetails.accountNumber]);
      this.router.navigate(['/account/transaction'],{state:{accNum:this.accountDetails.accountNumber}});
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


